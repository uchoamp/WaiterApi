using Waiter.Application.Exceptions;
using Waiter.Application.Models.MenuItems;
using Waiter.Application.UseCases.MenuItems;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UnitTests.UseCases.MenuItems;

public class CreateMenuItemUseCaseTest
{
    private readonly CreateMenuItemUseCase _sut;
    private readonly Mock<IMenuItemRepository> _mockMenuItemRepository;
    private readonly MenuItemRequest _validMenuItem;

    public CreateMenuItemUseCaseTest()
    {
        _mockMenuItemRepository = new Mock<IMenuItemRepository>();

        _sut = new CreateMenuItemUseCase(_mockMenuItemRepository.Object);

        _validMenuItem = new MenuItemRequest("Item T1", 50);
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenUserRequestIsInvalid()
    {
        var user = _validMenuItem with { Name = "" };

        var exceptionAssert = await _sut.Invoking(x => x.Create(user))
            .Should()
            .ThrowExactlyAsync<ApplicationValidationException>();

        var errorCodes = exceptionAssert.Which.Errors.Select(x => x.Code);

        errorCodes.Should().NotBeEmpty();
        errorCodes.Should().Contain(new string[] { "NameRequired", });
    }

    [Fact]
    public async Task ShouldReturnUserReponseWhenIsSuccessfullyCreated()
    {
        var result = await _sut.Create(_validMenuItem);

        result.Name.Should().Be(_validMenuItem.Name);
        result.PriceCents.Should().Be(_validMenuItem.PriceCents);

        _mockMenuItemRepository.Verify(
            x =>
                x.CreateAsync(
                    It.Is<MenuItem>(c =>
                        c.Name == _validMenuItem.Name && c.PriceCents == _validMenuItem.PriceCents
                    )
                ),
            Times.Once
        );

        _mockMenuItemRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
