using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Waiter.Application.Security;
using Waiter.Domain.Constants;
using Waiter.Domain.Models;
using Waiter.Infra.Data;
using Waiter.Infra.Security;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var jwtSecretKey = configuration[ApplicationSettings.JwtKey];
        var databaseConnectionString = configuration[ApplicationSettings.DatabaseConnectionString];

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(c =>
            {
                c.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSecretKey!)
                    ),
                };
            });

        services
            .AddIdentityCore<ApplicationUser>(opt => opt.User.RequireUniqueEmail = true)
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddAuthorization();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(databaseConnectionString)
        );

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddSingleton<ITokenProvider, JwtTokenProvider>();

        return services;
    }
}
