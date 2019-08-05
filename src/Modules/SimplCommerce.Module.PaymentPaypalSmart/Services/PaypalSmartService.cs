using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Extensions;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.PaymentPaypalSmart.Models;
using SimplCommerce.Module.PaymentPaypalSmart.ViewModels;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.ShoppingCart.Services;

namespace SimplCommerce.Module.PaymentPaypalSmart.Services
{
    public class PaypalService : IPaypalSmartService
    {
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _clientFactory;
        private readonly Lazy<PaypalSmartSettings> _settings;

        public PaypalService(
            IRepositoryWithTypedId<PaymentProvider, string> repository,
            IMemoryCache cache,
            IHttpClientFactory clientFactory
            )
        {
            _paymentProviderRepository = repository;
            _cache = cache;
            _clientFactory = clientFactory;
            _settings = new Lazy<PaypalSmartSettings>(GetSettings());
        }

        public async Task<HttpResponseMessage> CreatePayment(OrderRequest request, User user)
        {
            var tokenResult = await GetTokenResultAsync();
            var httpClient = _clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(tokenResult.token_type, tokenResult.access_token);
            var response = await httpClient.PostJsonAsync((_settings.Value.BaseApiUrl + "/v2/checkout/orders"), request);
            return response;
        }

        public async Task<HttpResponseMessage> ExecutePayment(string orderID)
        {
            var tokenResult =await GetTokenResultAsync();
            var httpClient = _clientFactory.CreateClient();
            
            var request = new HttpRequestMessage(HttpMethod.Post, _settings.Value.BaseApiUrl + $"/v2/checkout/orders/{orderID}/capture");
            request.Content = new StringContent("", Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(tokenResult.token_type, tokenResult.access_token);
            var response = await httpClient.SendAsync(request);
            return response;
        }

        /// <summary>
        /// 获取accesstoken
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAccessToken()
        {
            var token = await GetTokenResultAsync();
            if (token == null)
            {
                return string.Empty;
            }
            return token.access_token;
        }
        /// <summary>
        /// 获取accesstoken响应对象
        /// </summary>
        /// <returns></returns>
        public async Task<TokenResult> GetTokenResultAsync()
        {
            if (_cache.TryGetValue<TokenResult>(PaymentProviderHelper.PaypalSmartProviderId, out var token))
            {
                return token;
            }
            var paypalProvider = _paymentProviderRepository.Query()
                .FirstOrDefault(s => s.Id.Equals(PaymentProviderHelper.PaypalSmartProviderId));

            var setting = _settings.Value;
            //开始组装http请求
            var client = _clientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, setting.BaseApiUrl + "/v1/oauth2/token");
            var userpwd = setting.ClientId + ":" + setting.ClientSecret;
            request.Headers.Authorization=new System.Net.Http.Headers.AuthenticationHeaderValue( "Basic" , Convert.ToBase64String(Encoding.UTF8.GetBytes(userpwd)));
            request.Headers.Add("ContentType", "x-www-form-urlencoded");
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                token = JsonConvert.DeserializeObject<TokenResult>(result);

                //把token缓存起来
                _cache.Set(PaymentProviderHelper.PaypalSmartProviderId, token, DateTimeOffset.UtcNow.AddSeconds(token.expires_in));

                return token;
            }
            return null;

        }

        private PaypalSmartSettings GetSettings()
        {
            var provider = _paymentProviderRepository.Query().FirstOrDefault(s => s.Id == PaymentProviderHelper.PaypalSmartProviderId);
            var settings = JsonConvert.DeserializeObject<PaypalSmartSettings>(provider.AdditionalSettings);
            return settings;
        }
    }
}
