using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Web;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.PaymentPaypalSmart.Models;
using SimplCommerce.Module.PaymentPaypalSmart.Services;
using SimplCommerce.Module.Payments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplCommerce.Module.PaymentPaypalSmart.Areas.PaymentPaypalSmart.Components
{

  public  class PaypalSmartLandingViewComponent:ViewComponent
    {
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _repository;

        public PaypalSmartLandingViewComponent(IRepositoryWithTypedId<PaymentProvider,string> repository)
        {
            _repository = repository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var provider =await _repository.Query().FirstOrDefaultAsync(s => s.Id == PaymentProviderHelper.PaypalSmartProviderId);
            var settings = JsonConvert.DeserializeObject<PaypalSmartSettings>(provider.AdditionalSettings);
            return View(this.GetViewPath(), settings);
        }
    }
}
