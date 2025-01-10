using Application.Interfaces;
using Application.Services;
using Application.Validators;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace Application.DependencyInjection
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Services
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<TokenService>();

            // Register Validators
            services.AddValidatorsFromAssemblyContaining<CustomerValidator>();
            services.AddValidatorsFromAssemblyContaining<AddressValidator>();

            return services;
        }
    }
}
