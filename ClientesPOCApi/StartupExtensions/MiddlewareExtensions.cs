using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace ClientesPOCApi.StartupExtensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(options =>
            {
                options.Run(async context =>
                {
                    var feature = context.Features.Get<IExceptionHandlerFeature>();
                    if (feature != null)
                    {
                        var exception = feature.Error;
                        var response = new { message = exception.Message, detail = exception.StackTrace };
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;  // or appropriate status code
                        await context.Response.WriteAsJsonAsync(response);  // Send error message and details as JSON
                    }
                });
            });

            app.UseCors(policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
