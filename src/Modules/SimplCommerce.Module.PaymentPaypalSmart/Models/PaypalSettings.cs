using System;
using System.Collections.Generic;
using System.Text;

namespace SimplCommerce.Module.PaymentPaypalSmart.Models
{
    [Serializable]
    public class PaypalSmartSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool IsSandbox { get; set; }

        public string BaseApiUrl
        {
            get
            {
                return IsSandbox ? "https://api.sandbox.paypal.com" : "https://api.paypal.com";
            }
        }

        public string Envirment
        {
            get
            {
                return IsSandbox ? "sandbox" : "production";
            }
        }
    }
}
