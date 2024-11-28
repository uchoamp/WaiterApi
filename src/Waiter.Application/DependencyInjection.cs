using Waiter.Application.UseCases.Customers;
using Waiter.Application.UseCases.Users;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<AuthorizeUserUseCase>();
        services.AddScoped<GetAllUsersUseCase>();
        services.AddScoped<GetAvailableRolesUseCase>();
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetUserUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<DeleteUserUseCase>();

        services.AddScoped<CreateCustomerUseCase>();
        services.AddScoped<GetCustomersPaginatedUseCase>();
        services.AddScoped<GetCustomerUseCase>();
        services.AddScoped<UpdateCustomerUseCase>();

        return services;
    }
}
