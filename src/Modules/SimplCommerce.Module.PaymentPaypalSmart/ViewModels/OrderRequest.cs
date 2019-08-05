using System;
using System.Collections.Generic;
using System.Text;

namespace SimplCommerce.Module.PaymentPaypalSmart.ViewModels
{
    public class OrderRequest
    {
        public string intent { get; set; }

        public List<PurchaseUnit> purchase_units { get; set; }
    }
    public class PurchaseUnit
    {
        public Amount amount { get; set; }
    }
    public class Amount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }
}
