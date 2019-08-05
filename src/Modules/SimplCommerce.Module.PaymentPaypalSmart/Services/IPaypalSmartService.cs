using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.PaymentPaypalSmart.Models;
using SimplCommerce.Module.PaymentPaypalSmart.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimplCommerce.Module.PaymentPaypalSmart.Services
{
    public interface IPaypalSmartService
    {
        Task<TokenResult> GetTokenResultAsync();

        Task<HttpResponseMessage> CreatePayment(OrderRequest request, User user);
        Task<HttpResponseMessage> ExecutePayment(string orderID);
    }
}
