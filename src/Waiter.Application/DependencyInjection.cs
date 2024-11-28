using FluentValidation;
using Waiter.Application.Models.Orders;
using Waiter.Application.Services;
using Waiter.Application.UseCases.Customers;
using Waiter.Application.UseCases.MenuItems;
using Waiter.Application.UseCases.Orders;
using Waiter.Application.UseCases.Users;
using Waiter.Application.Validators.Orders;
using Waiter.Domains.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        AddServices(services);
        AddUseCases(services);
        AddValidators(services);

        return services;
    }

    private static void AddUseCases(IServiceCollection services)
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
        services.AddScoped<DeleteCustomerUseCase>();

        services.AddScoped<CreateMenuItemUseCase>();
        services.AddScoped<GetMenuItemsPaginatedUseCase>();
        services.AddScoped<GetMenuItemUseCase>();
        services.AddScoped<UpdateMenuItemUseCase>();
        services.AddScoped<DeleteMenuItemUseCase>();

        services.AddScoped<CreateOrderUseCase>();
        services.AddScoped<GetOrdersPaginatedUseCase>();
        services.AddScoped<GetOrderUseCase>();
        services.AddScoped<UpdateOrderUseCase>();
        services.AddScoped<DeleteOrderUseCase>();
    }

    private static void AddValidators(IServiceCollection services)
    {
        services.AddTransient<AbstractValidator<OrderRequest>, OrderRequestValidator>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<ICalculateOrderCostService, CalculateOrderCostService>();
    }
}
