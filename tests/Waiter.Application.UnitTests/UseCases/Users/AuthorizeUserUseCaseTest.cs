using Waiter.Application.Exceptions;
using Waiter.Application.Models.Request;
using Waiter.Application.Models.Response;
using Waiter.Application.Security;
using Waiter.Application.UseCases.Users;

namespace Waiter.Application.UnitTests.UseCases.Users;

public class AuthorizeUserUseCaseTest
{
    private readonly AuthorizeUserUseCase _sut;
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly Mock<ITokenProvider> _mockTokenProvider;

    public AuthorizeUserUseCaseTest()
    {
        _mockIdentityService = new Mock<IIdentityService>();
        _mockTokenProvider = new Mock<ITokenProvider>();

        _sut = new AuthorizeUserUseCase(_mockTokenProvider.Object, _mockIdentityService.Object);
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenPasswordOrEmailInvalid()
    {
        var credentials = new UserCredentialResquest("teste@email.com", "password");

        _mockIdentityService
            .Setup(x => x.CheckPasswordAsync(credentials.Email, credentials.Password))
            .ReturnsAsync(false);

        await _sut.Invoking(x => x.AuthorizeUser(credentials))
            .Should()
            .ThrowExactlyAsync<ApplicationValidationException>();

        _mockIdentityService.Verify(
            x => x.CheckPasswordAsync(credentials.Email, credentials.Password),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldReturnAccessTokenWhenPasswordIsValid()
    {
        var credentials = new UserCredentialResquest("teste@email.com", "password");
        var user = new UserResponse(
            Guid.Empty,
            "Teste",
            "User",
            "teste@email.com",
            new[] { "admin" }
        );

        var accessToken = new AccessTokenResponse("token", DateTime.UtcNow);

        _mockIdentityService
            .Setup(x => x.CheckPasswordAsync(credentials.Email, credentials.Password))
            .ReturnsAsync(true);

        _mockIdentityService
            .Setup(x => x.GetUserByEmailAsync(credentials.Email))
            .ReturnsAsync(user);

        _mockTokenProvider
            .Setup(x => x.CreateAcessTokenAsync(user.Id, user.Roles))
            .Returns(accessToken);

        var result = await _sut.AuthorizeUser(credentials);

        _mockIdentityService.Verify(
            x => x.CheckPasswordAsync(credentials.Email, credentials.Password),
            Times.Once
        );

        _mockIdentityService.Verify(x => x.GetUserByEmailAsync(credentials.Email), Times.Once);
        _mockTokenProvider.Verify(x => x.CreateAcessTokenAsync(user.Id, user.Roles), Times.Once);

        result.Should().Be(accessToken);
    }
}
