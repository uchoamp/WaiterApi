using Waiter.Application.Exceptions;
using Waiter.Application.Models.Users;
using Waiter.Application.Security;
using Waiter.Application.UseCases.Users;

namespace Waiter.Application.UnitTests.UseCases.Users;

public class CreateUserUseCaseTest
{
    private readonly CreateUserUseCase _sut;
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly NewUserRequest _validUser;

    public CreateUserUseCaseTest()
    {
        _mockIdentityService = new Mock<IIdentityService>();

        _sut = new CreateUserUseCase(_mockIdentityService.Object);

        _validUser = new NewUserRequest(
            "Marcos",
            "Uchoa",
            "marcos@email.com",
            "86981732880",
            "Password123!",
            new[] { "admin" }
        );
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenUserRequestIsInvalid()
    {
        var user = _validUser with { Roles = new string[0] };

        var exceptionAssert = await _sut.Invoking(x => x.Create(user))
            .Should()
            .ThrowExactlyAsync<ApplicationValidationException>();

        var errorCodes = exceptionAssert.Which.Errors.Select(x => x.Code);

        errorCodes.Should().NotBeEmpty();
        errorCodes.Should().Contain(new string[] { "RolesValidRequired", });
    }

    [Fact]
    public async Task ShouldReturnUserReponseWhenIsSuccessfullyCreated()
    {
        var userResponse = new UserResponse(
            Guid.NewGuid(),
            _validUser.FirstName,
            _validUser.LastName,
            _validUser.Email,
            _validUser.PhoneNumber,
            _validUser.Roles
        );

        _mockIdentityService
            .Setup(x => x.GetRolesAsync())
            .ReturnsAsync(new HashSet<string> { "admin" });
        _mockIdentityService.Setup(x => x.CreateUserAsync(_validUser));
        _mockIdentityService
            .Setup(x => x.GetUserByEmailAsync(_validUser.Email))
            .ReturnsAsync(userResponse);

        var result = await _sut.Create(_validUser);

        _mockIdentityService.Verify(x => x.CreateUserAsync(_validUser), Times.Once);
        _mockIdentityService.Verify(x => x.GetUserByEmailAsync(_validUser.Email), Times.Once);

        result.Should().Be(userResponse);
    }
}
