using Waiter.Application.Models.Orders;
using Waiter.Application.Validators.Orders;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UnitTests.Validators;

public class OrderRequestValidatorTest
{
    private readonly OrderRequestValidator _validator;

    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<IMenuItemRepository> _mockMenuItemRepository;

    private readonly OrderRequest _validRequest;

    public OrderRequestValidatorTest()
    {
        _validRequest = new OrderRequest(
            Guid.NewGuid(),
            new[]
            {
                new OrderItemRequest(Guid.NewGuid(), 1),
                new OrderItemRequest(Guid.NewGuid(), 2)
            }
        );

        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockMenuItemRepository = new Mock<IMenuItemRepository>();

        _validator = new OrderRequestValidator(
            _mockMenuItemRepository.Object,
            _mockCustomerRepository.Object
        );

        _mockCustomerRepository
            .Setup(x => x.ExistsEntity(_validRequest.CustomerId))
            .ReturnsAsync(true);

        foreach (var item in _validRequest.Items)
            _mockMenuItemRepository.Setup(x => x.ExistsEntity(item.ItemId)).ReturnsAsync(true);
    }

    [Fact]
    public async Task ShouldBeValid()
    {
        var result = await _validator.ValidateAsync(_validRequest);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldValidatedCustomerId()
    {
        _mockCustomerRepository
            .Setup(x => x.ExistsEntity(_validRequest.CustomerId))
            .ReturnsAsync(false);

        var result = await _validator.ValidateAsync(_validRequest);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain("CustomerNotFound");
    }

    [Fact]
    public async Task ShouldValidatedItemsNull()
    {
        var request = _validRequest with { Items = null };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain("ItemsRequired");
    }

    [Fact]
    public async Task ShouldValidatedItemsEmpty()
    {
        var request = _validRequest with { Items = new OrderItemRequest[0] };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain("ItemsAtLeastOne");
    }

    [Fact]
    public async Task ShouldValidatedItemsDuplication()
    {
        var request = _validRequest with
        {
            Items = new OrderItemRequest[]
            {
                new OrderItemRequest(Guid.Empty, 1),
                new OrderItemRequest(Guid.Empty, 2)
            }
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain("ItemsMustBeUnique");
    }

    [Fact]
    public async Task ShouldValidatedItemQuantity()
    {
        var request = _validRequest with
        {
            Items = new[] { new OrderItemRequest(Guid.NewGuid(), 0) }
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain("ItemQuantityZero");
    }

    [Fact]
    public async Task ShouldValidatedItemNotFound()
    {
        var request = _validRequest with
        {
            Items = new[]
            {
                new OrderItemRequest(Guid.NewGuid(), 2),
                new OrderItemRequest(Guid.NewGuid(), 2),
                new OrderItemRequest(Guid.NewGuid(), 0)
            }
        };

        foreach (var item in request.Items)
            _mockMenuItemRepository
                .Setup(x => x.ExistsEntity(item.ItemId))
                .ReturnsAsync(item.Quantity == 0);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain("ItemNotFound");
    }
}
