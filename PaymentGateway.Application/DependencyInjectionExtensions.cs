﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Abstractions;
using PaymentGateway.Application.WriteOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaymentGateway.Application.ReadOperations.ListOfAccounts;

namespace PaymentGateway.Application
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection RegisterBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddTransient<EnrollCustomerOperation>();
            //services.AddTransient<CreateAccountOperation>();
            //services.AddTransient<DepositMoneyOperation>();
            //services.AddTransient<WithdrawMoneyOperation>();
            //services.AddTransient<PurchaseProductOperation>();
            //services.AddTransient<QueryHandler>();

            services.AddTransient<Data.Database>();

            services.AddTransient<IValidator<Query>, Validator>();

            services.AddTransient<QueryHandler>();

            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var options = new AccountOptions
                {
                    InitialBalance = config.GetValue("AccountOptions:InitialBalance", 0)
                };
                return options;
            });


            return services;
        }
    }
}

