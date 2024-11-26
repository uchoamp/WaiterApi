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

        return services;
    }
}
