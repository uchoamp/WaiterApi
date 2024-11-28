using Waiter.Application.Exceptions;
using Waiter.Application.Models.Orders;
using Waiter.Application.UseCases.Orders;
using Waiter.Application.Validators.Orders;
using Waiter.Domain.Enums;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;
using Waiter.Domains.Services;

namespace Waiter.Application.UnitTests.UseCases.Orders;

public class CreateOrderUseCaseTest
{
    private readonly CreateOrderUseCase _sut;

    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<ICalculateOrderCostService> _mockCalculateOrderCostService;

    private readonly OrderRequest _validRequest;

    public CreateOrderUseCaseTest()
    {
        var mockMenuItemRepository = new Mock<IMenuItemRepository>();
        var mockCustomerRepositor = new Mock<ICustomerRepository>();

        var validator = new OrderRequestValidator(
            mockMenuItemRepository.Object,
            mockCustomerRepositor.Object
        );

        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockCalculateOrderCostService = new Mock<ICalculateOrderCostService>();

        _sut = new CreateOrderUseCase(
            _mockOrderRepository.Object,
            _mockCalculateOrderCostService.Object,
            validator
        );

        _validRequest = new OrderRequest(
            Guid.NewGuid(),
            new[] { new OrderItemRequest(Guid.NewGuid(), 2) }
        );

        mockMenuItemRepository.Setup(x => x.ExistsEntity(It.IsAny<Guid>())).ReturnsAsync(true);
        mockCustomerRepositor.Setup(x => x.ExistsEntity(It.IsAny<Guid>())).ReturnsAsync(true);
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenUserRequestIsInvalid()
    {
        var request = _validRequest with { Items = new OrderItemRequest[0] };

        var exceptionAssert = await _sut.Invoking(x => x.Create(request))
            .Should()
            .ThrowExactlyAsync<ApplicationValidationException>();

        var errorCodes = exceptionAssert.Which.Errors.Select(x => x.Code);

        errorCodes.Should().NotBeEmpty();
        errorCodes.Should().Contain(new string[] { "ItemsAtLeastOne", });
    }

    [Fact]
    public async Task ShouldReturnUserReponseWhenIsSuccessfullyCreated()
    {
        var orderCostExpected = 100;
        var idExpected = Guid.NewGuid();

        _mockCalculateOrderCostService
            .Setup(x => x.Calculate(It.IsAny<OrderItem[]>()))
            .ReturnsAsync(orderCostExpected);

        _mockOrderRepository
            .Setup(x => x.CreateAsync(It.IsAny<Order>()))
            .Callback(
                (Order order) =>
                {
                    order.Id = idExpected;
                    order.CreatedAt = DateTime.UtcNow;
                    order.Customer = new Customer
                    {
                        FirstName = "Foo",
                        LastName = "Bar",
                        PhoneNumber = "TESTE"
                    };

                    var count = 1;
                    foreach (var orderItem in order.Items)
                    {
                        orderItem.Item = new MenuItem
                        {
                            Name = "Item " + (count++).ToString(),
                            PriceCents = 10
                        };
                    }
                }
            );

        var result = await _sut.Create(_validRequest);

        result.TotalPriceCents.Should().Be(orderCostExpected);
        result.Status.Should().Be(OrderStatus.Pending);
        result.StatusDescription.Should().Be(OrderStatus.Pending.ToString());

        result.CustomerName.Should().NotBeNullOrEmpty();
        result
            .Items.Should()
            .AllSatisfy(
                (orderItem) =>
                {
                    orderItem.Quantity.Should().BeGreaterThan(0);
                    orderItem.ItemName.Should().NotBeNullOrEmpty();
                    orderItem.ItemId.Should().NotBeEmpty();
                }
            );

        _mockOrderRepository.Verify(
            x =>
                x.CreateAsync(
                    It.Is<Order>(t =>
                        t.CustomerId == _validRequest.CustomerId
                        && t.Items.Count == _validRequest.Items.Length
                    )
                ),
            Times.Once
        );

        _mockOrderRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        _mockOrderRepository.Verify(
            x => x.RefreshAsync(It.Is<Order>(t => t.Id == idExpected)),
            Times.Once
        );
    }
}
