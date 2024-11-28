using Waiter.Application.Services;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UnitTests.Services;

public class CalculateOrderCostServiceTest
{
    private readonly CalculateOrderCostService _sut;

    private readonly Mock<IMenuItemRepository> _mockMenuItemRepository;

    public CalculateOrderCostServiceTest()
    {
        _mockMenuItemRepository = new Mock<IMenuItemRepository>();
        _sut = new CalculateOrderCostService(_mockMenuItemRepository.Object);
    }

    [Fact]
    public async Task ShouldCalculateOrderCost()
    {
        var menuItems = new[]
        {
            new MenuItem
            {
                Name = "Item 1",
                PriceCents = 10,
                Id = Guid.NewGuid()
            },
            new MenuItem
            {
                Name = "Item 2",
                PriceCents = 50,
                Id = Guid.NewGuid()
            },
            new MenuItem
            {
                Name = "Item 3",
                PriceCents = 150,
                Id = Guid.NewGuid()
            }
        };

        var items = new[]
        {
            new OrderItem { ItemId = menuItems[0].Id, Quantity = 5 },
            new OrderItem { ItemId = menuItems[1].Id, Quantity = 2 },
        };

        _mockMenuItemRepository
            .Setup(x => x.GetEntitiesWithIds(It.IsAny<Guid[]>()))
            .ReturnsAsync(new List<MenuItem> { menuItems[0], menuItems[1] });

        var result = await _sut.Calculate(items);

        result.Should().Be(150);
    }
}
