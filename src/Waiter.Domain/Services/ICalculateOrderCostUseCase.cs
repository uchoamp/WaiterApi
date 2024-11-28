using Waiter.Domain.Models;

namespace Waiter.Domains.Services;

public interface ICalculateOrderCostService
{
    Task<int> Calculate(OrderItem[] items);
}
