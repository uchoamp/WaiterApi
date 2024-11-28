using Waiter.Application.Exceptions;
using Waiter.Application.Models.Orders;
using Waiter.Application.UseCases.Orders;
using Waiter.Application.Validators.Orders;
using Waiter.Domain.Enums;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;
using Waiter.Domains.Services;

namespace Waiter.Application.UnitTests.UseCases.Orders;

public class UpdateOrderUseCaseTest
{
    private readonly UpdateOrderUseCase _sut;

    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<ICalculateOrderCostService> _mockCalculateOrderCostService;

    private readonly Order _databaseOrder;
    private readonly MenuItem[] _databaseMenuItems;

    public UpdateOrderUseCaseTest()
    {
        _databaseMenuItems = new[]
        {
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Item 1",
                PriceCents = 10
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Item 2",
                PriceCents = 20
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Item 3",
                PriceCents = 40
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Item 4",
                PriceCents = 120
            }
        };

        var mockMenuItemRepository = new Mock<IMenuItemRepository>();
        var mockCustomerRepositor = new Mock<ICustomerRepository>();

        var validator = new OrderRequestValidator(
            mockMenuItemRepository.Object,
            mockCustomerRepositor.Object
        );

        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockCalculateOrderCostService = new Mock<ICalculateOrderCostService>();

        _sut = new UpdateOrderUseCase(
            _mockOrderRepository.Object,
            _mockCalculateOrderCostService.Object,
            validator
        );

        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        _databaseOrder = new Order
        {
            Id = orderId,
            CustomerId = customerId,
            Status = OrderStatus.Cooking,
            Customer = new Customer
            {
                Id = customerId,
                FirstName = "Foo",
                LastName = "Bar",
                PhoneNumber = "TESTE"
            },
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    OrderId = orderId,
                    ItemId = _databaseMenuItems[0].Id,
                    Item = _databaseMenuItems[0],
                    Quantity = 1
                },
                new OrderItem
                {
                    OrderId = orderId,
                    ItemId = _databaseMenuItems[3].Id,
                    Item = _databaseMenuItems[3],
                    Quantity = 1
                },
            },
            TotalPriceCents = 130
        };

        mockMenuItemRepository.Setup(x => x.ExistsEntity(It.IsAny<Guid>())).ReturnsAsync(true);
        mockCustomerRepositor.Setup(x => x.ExistsEntity(It.IsAny<Guid>())).ReturnsAsync(true);

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(_databaseOrder.Id))
            .ReturnsAsync(_databaseOrder);

        _mockOrderRepository.Setup(x => x.ExistsEntity(_databaseOrder.Id)).ReturnsAsync(true);
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenUserRequestIsInvalid()
    {
        var request = new OrderRequest(Guid.NewGuid(), new OrderItemRequest[0]);

        var exceptionAssert = await _sut.Invoking(x => x.Update(_databaseOrder.Id, request))
            .Should()
            .ThrowExactlyAsync<ApplicationValidationException>();

        var errorCodes = exceptionAssert.Which.Errors.Select(x => x.Code);

        errorCodes.Should().NotBeEmpty();
        errorCodes.Should().Contain(new string[] { "ItemsAtLeastOne", });
    }

    [Fact]
    public async Task ShouldThrowResourceNotFoundWhenEntityNotExists()
    {
        var request = new OrderRequest(Guid.NewGuid(), new OrderItemRequest[0]);

        _mockOrderRepository.Setup(x => x.ExistsEntity(_databaseOrder.Id)).ReturnsAsync(false);

        var exceptionAssert = await _sut.Invoking(x => x.Update(_databaseOrder.Id, request))
            .Should()
            .ThrowExactlyAsync<ResourceNotFoundException>();

        exceptionAssert.WithMessage("Order*");
    }

    [Fact]
    public async Task ShouldReturnUserReponseWhenIsSuccessfullyCreated()
    {
        var orderCostExpected = 170;

        var request = new OrderRequest(
            Guid.NewGuid(),
            new[]
            {
                new OrderItemRequest(_databaseMenuItems[0].Id, 2),
                new OrderItemRequest(_databaseMenuItems[2].Id, 3),
                new OrderItemRequest(_databaseMenuItems[1].Id, 1)
            }
        );

        _mockOrderRepository
            .Setup(x => x.RefreshAsync(_databaseOrder))
            .Callback(
                (Order order) =>
                {
                    foreach (var orderItem in order.Items)
                    {
                        orderItem.Item = _databaseMenuItems.First(x => x.Id == orderItem.ItemId);
                    }
                }
            );

        _mockCalculateOrderCostService
            .Setup(x => x.Calculate(It.IsAny<OrderItem[]>()))
            .ReturnsAsync(orderCostExpected);

        var result = await _sut.Update(_databaseOrder.Id, request);

        result.CustomerId.Should().Be(request.CustomerId);
        result.TotalPriceCents.Should().Be(orderCostExpected);
        result.Status.Should().Be(_databaseOrder.Status);
        result.StatusDescription.Should().Be(_databaseOrder.Status.ToString());
        result.CustomerName.Should().NotBeNullOrEmpty();

        result.Items.Should().HaveCount(3);
        result
            .Items.Should()
            .AllSatisfy(
                (orderItem) =>
                {
                    orderItem.ItemId.Should().NotBe(_databaseMenuItems[3].Id);
                    orderItem.Quantity.Should().BeGreaterThan(0);
                    orderItem.ItemName.Should().NotBeNullOrEmpty();
                    orderItem.ItemId.Should().NotBeEmpty();
                }
            );

        _mockOrderRepository.Verify(x => x.Update(_databaseOrder), Times.Once);

        _mockOrderRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        _mockOrderRepository.Verify(x => x.RefreshAsync(_databaseOrder), Times.Once);
    }
}
