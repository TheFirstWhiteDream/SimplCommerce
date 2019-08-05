using System;
using System.Collections.Generic;
using System.Text;

namespace SimplCommerce.Module.PaymentPaypalSmart.ViewModels
{
   public class PaypalSmartConfigForm
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool IsSandbox { get; set; }
    }
}
