using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.PaymentPaypalSmart.Models;
using SimplCommerce.Module.PaymentPaypalSmart.ViewModels;
using SimplCommerce.Module.Payments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplCommerce.Module.PaymentPaypalSmart.Areas.PaymentPaypalSmart.Controllers
{
    [Area("PaymentPaypalSmart")]
    [Authorize(Roles ="admin")]
    [Route("api/paypal-smart")]
    public class PaypalSmartApiController : Controller
    {
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentRepository;
        public PaypalSmartApiController(IRepositoryWithTypedId<PaymentProvider,string> repository)
        {
            _paymentRepository = repository;
        }

        [HttpGet("config")]
        public IActionResult Config()
        {
            var provider = _paymentRepository.Query()
                .FirstOrDefault(s => s.Id == PaymentProviderHelper.PaypalSmartProviderId);
            var config = JsonConvert.DeserializeObject<PaypalSmartSettings>(provider.AdditionalSettings);
            return Ok(config);
        }

        [HttpPut("config")]
        public async Task<IActionResult> Config([FromBody] PaypalSmartConfigForm model)
        {
            if(ModelState.IsValid)
            {
                var provider = _paymentRepository.Query().FirstOrDefault(s => s.Id == PaymentProviderHelper.PaypalSmartProviderId);
                provider.AdditionalSettings = JsonConvert.SerializeObject(model);
                await _paymentRepository.SaveChangesAsync();
                return Accepted();
            }
            return BadRequest(ModelState);
        }
    }
}
