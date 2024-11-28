using Waiter.Application.Exceptions;
using Waiter.Application.Models.Users;
using Waiter.Application.Security;
using Waiter.Application.UseCases.Users;

namespace Waiter.Application.UnitTests.UseCases.Users;

public class UpdateUserUseCaseTest
{
    private readonly UpdateUserUseCase _sut;
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly UpdateUserRequest _validUser;

    public UpdateUserUseCaseTest()
    {
        _mockIdentityService = new Mock<IIdentityService>();

        _mockIdentityService
            .Setup(x => x.GetRolesAsync())
            .ReturnsAsync(new HashSet<string> { "admin" });

        _sut = new UpdateUserUseCase(_mockIdentityService.Object);

        _validUser = new UpdateUserRequest(
            Guid.NewGuid(),
            "Marcos",
            "Uchoa",
            "86981732880",
            "marcos@email.com"
        );
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenUserRequestIsInvalid()
    {
        var user = _validUser with { FirstName = "" };

        var exceptionAssert = await _sut.Invoking(x => x.Update(user))
            .Should()
            .ThrowExactlyAsync<ApplicationValidationException>();

        var errorCodes = exceptionAssert.Which.Errors.Select(x => x.Code);

        errorCodes.Should().NotBeEmpty();
        errorCodes.Should().Contain(new string[] { "FirstNameRequired", });
    }

    [Fact]
    public async Task ShouldReturnUserReponseWhenIsSuccessfullyUdate()
    {
        var userResponse = new UserResponse(
            _validUser.Id,
            _validUser.FirstName,
            _validUser.LastName,
            _validUser.Email,
            _validUser.PhoneNumber,
            new[] { "admin" }
        );

        _mockIdentityService.Setup(x => x.UpdateUserAsync(_validUser));
        _mockIdentityService.Setup(x => x.GetUserAsync(_validUser.Id)).ReturnsAsync(userResponse);

        var result = await _sut.Update(_validUser);

        _mockIdentityService.Verify(x => x.UpdateUserAsync(_validUser), Times.Once);
        _mockIdentityService.Verify(x => x.GetUserAsync(_validUser.Id), Times.Exactly(2));

        result.Should().Be(userResponse);
    }
}
