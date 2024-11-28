using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Waiter.API.Custom;
using Waiter.API.Services;
using Waiter.Application.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Waiter API",
                    Version = "v1",
                    Contact = new OpenApiContact()
                    {
                        Name = "Marcos Uchoa",
                        Email = "marcospacheco10111@gmail.com",
                    }
                }
            );
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                }
            );

            c.OperationFilter<AuthOperationFilter>();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddSingleton<
            IAuthorizationMiddlewareResultHandler,
            CustomAuthorizationMiddlewareResultHandler
        >();

        services.AddControllers();

        services.AddScoped<IUser, CurrentUser>();
        services.AddHttpContextAccessor();

        return services;
    }
}
