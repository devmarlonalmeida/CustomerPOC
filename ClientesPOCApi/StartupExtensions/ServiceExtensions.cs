using ClientesPOCApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ClientesPOCApi.StartupExtensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            // Add Controllers
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

            // Add API versioning
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            return services;
        }
    }
}
