using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Payments.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplCommerce.Module.PaymentPaypalSmart.Data
{
   public class PaymentPaypalSmartSmartCustomModelBuilder:ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentProvider>()
                .HasData(
                new PaymentProvider("PaypalSmart")
                {
                    Name = "Paypal Smart",
                    ConfigureUrl = "payments-paypalsmart-config",
                    LandingViewComponentName = "PaypalSmartLanding",
                    IsEnabled = true,
                    AdditionalSettings = "{ \"IsSandbox\":true, \"ClientId\":\"\", \"ClientSecret\":\"\" }"
                }
                );
        }
    }
}
