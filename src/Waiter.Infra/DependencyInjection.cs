using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Waiter.Application.Models.Customers;
using Waiter.Application.Security;
using Waiter.Domain.Constants;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;
using Waiter.Infra.Data;
using Waiter.Infra.Repositories;
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
            .AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequiredLength = 8;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddAuthorization();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(databaseConnectionString)
        );

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddSingleton<ITokenProvider, JwtTokenProvider>();

        AddRepositories(services);

        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerRepository<CustomerFilter>, CustomerRepository>();

        services.AddScoped<IMenuItemRepository, MenuItemRepository>();

        services.AddScoped<IOrderRepository, OrderRepository>();
    }
}
