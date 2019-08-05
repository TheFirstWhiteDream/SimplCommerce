using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Modules;
using SimplCommerce.Module.PaymentPaypalSmart.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplCommerce.Module.PaymentPaypalSmart
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IPaypalSmartService, PaypalService>();
            GlobalConfiguration.RegisterAngularModule("simplAdmin.paymentPaypalSmart");
        }
    }
}
