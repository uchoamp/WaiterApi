using Waiter.Application.Exceptions;
using Waiter.Application.Models.MenuItems;
using Waiter.Application.UseCases.MenuItems;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UnitTests.UseCases.MenuItems;

public class UpdateMenuItemUseCaseTest
{
    private readonly UpdateMenuItemUseCase _sut;
    private readonly Mock<IMenuItemRepository> _mockMenuItemRepository;
    private readonly MenuItemRequest _validMenuItem;
    private readonly Guid _id;

    public UpdateMenuItemUseCaseTest()
    {
        _mockMenuItemRepository = new Mock<IMenuItemRepository>();

        _sut = new UpdateMenuItemUseCase(_mockMenuItemRepository.Object);

        _validMenuItem = new MenuItemRequest("Teste T1", 150);
        _id = Guid.NewGuid();

        _mockMenuItemRepository.Setup(x => x.ExistsEntity(_id)).ReturnsAsync(true);
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenUserRequestIsInvalid()
    {
        var user = _validMenuItem with { PriceCents = 0 };

        var exceptionAssert = await _sut.Invoking(x => x.Update(_id, user))
            .Should()
            .ThrowExactlyAsync<ApplicationValidationException>();

        var errorCodes = exceptionAssert.Which.Errors.Select(x => x.Code);

        errorCodes.Should().NotBeEmpty();
        errorCodes.Should().Contain(new string[] { "PriceCentsZero", });
    }

    [Fact]
    public async Task ShouldThrowResourceNotFoundWhenEntityNotExists()
    {
        _mockMenuItemRepository.Setup(x => x.ExistsEntity(_id)).ReturnsAsync(false);

        var exceptionAssert = await _sut.Invoking(x => x.Update(_id, _validMenuItem))
            .Should()
            .ThrowExactlyAsync<ResourceNotFoundException>();

        exceptionAssert.WithMessage("MenuItem*");
    }

    [Fact]
    public async Task ShouldReturnUserReponseWhenIsSuccessfullyCreated()
    {
        var menuItemDatabase = new MenuItem
        {
            Id = _id,
            Name = "Teste T2",
            PriceCents = 10
        };

        _mockMenuItemRepository.Setup(x => x.GetByIdAsync(_id)).ReturnsAsync(menuItemDatabase);

        var result = await _sut.Update(_id, _validMenuItem);

        result.Should().NotBeNull();
        result.Name.Should().Be(menuItemDatabase.Name);
        result.PriceCents.Should().Be(menuItemDatabase.PriceCents);

        menuItemDatabase.Name.Should().Be(_validMenuItem.Name);
        menuItemDatabase.PriceCents.Should().Be(_validMenuItem.PriceCents);

        _mockMenuItemRepository.Verify(x => x.Update(menuItemDatabase), Times.Once);
        _mockMenuItemRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
