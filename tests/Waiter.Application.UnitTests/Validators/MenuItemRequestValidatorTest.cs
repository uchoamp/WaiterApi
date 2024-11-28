using Waiter.Application.Models.MenuItems;
using Waiter.Application.Validators.MenuItems;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UnitTests.Validators;

public class MenuItemRequestValidatorTest
{
    private readonly MenuItemRequestValidator _validator;

    private readonly MenuItemRequest _validRequest;

    public MenuItemRequestValidatorTest()
    {
        _validRequest = new MenuItemRequest("Item T1", 50);

        _validator = new MenuItemRequestValidator();
    }

    [Fact]
    public async Task ShouldBeValidIfUserIsValidNew()
    {
        var result = await _validator.ValidateAsync(_validRequest);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, new[] { "NameRequired" })]
    [InlineData("M", new[] { "NameInvalid" })]
    public async Task ShouldValidatedName(string name, string[] expectedCodes)
    {
        var user = _validRequest with { Name = name };

        var result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain(expectedCodes);
    }

    [Theory]
    [InlineData(0, new[] { "PriceCentsZero" })]
    public async Task ShouldValidatedPrice(int priceCents, string[] expectedCodes)
    {
        var user = _validRequest with { PriceCents = priceCents };

        var result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain(expectedCodes);
    }
}
