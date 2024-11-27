using Waiter.Application.Models.Request;
using Waiter.Application.Security;
using Waiter.Application.Validators;

namespace Waiter.Application.UnitTests.Validators;

public class UpdateUserRequestValidatorTest
{
    private readonly UpdateUserRequestValidator _validator;
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly UpdateUserRequest _validUser;

    public UpdateUserRequestValidatorTest()
    {
        _mockIdentityService = new Mock<IIdentityService>();

        _validator = new UpdateUserRequestValidator(_mockIdentityService.Object);

        _validUser = new UpdateUserRequest(Guid.NewGuid(), "Marcos", "Uchoa", "marcos@email.com");

        _mockIdentityService
            .Setup(x => x.GetRolesAsync())
            .ReturnsAsync(new HashSet<string> { "admin" });
    }

    [Fact]
    public async Task ShouldBeValidIfUserIsValid()
    {
        var result = await _validator.ValidateAsync(_validUser);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, new[] { "FirstNameRequired" })]
    [InlineData("M", new[] { "FirstNameInvalid" })]
    public async Task ShouldValidatedFirstName(string firstName, string[] expectedCodes)
    {
        var user = _validUser with { FirstName = firstName };

        var result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain(expectedCodes);
    }

    [Theory]
    [InlineData(null, new[] { "LastNameRequired" })]
    [InlineData("A", new[] { "LastNameInvalid" })]
    public async Task ShouldValidatedLastName(string lastName, string[] expectedCodes)
    {
        var user = _validUser with { LastName = lastName };

        var result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain(expectedCodes);
    }

    [Theory]
    [InlineData(null, new[] { "EmailRequired" })]
    [InlineData("", new[] { "EmailRequired" })]
    [InlineData("notemail", new[] { "EmailInvalid" })]
    public async Task ShouldValidatedEmail(string email, string[] expectedCodes)
    {
        var user = _validUser with { Email = email };

        var result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain(expectedCodes);
    }

    [Fact]
    public async Task ShouldValidatedEmailIfEmailAlreadyRegistered()
    {
        var email = "teste@email.com";
        var user = _validUser with { Email = email };

        _mockIdentityService.Setup(x => x.GetUserIdWithEmail(email)).ReturnsAsync(Guid.NewGuid());

        var result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain(new[] { "EmailAlreadyRegistered" });
    }

    [Fact]
    public async Task ShouldBeValidIfEmailIsTheSameOnUpdate()
    {
        var userId = Guid.NewGuid();
        var email = "teste@email.com";
        var user = _validUser with { Email = email, Id = userId };

        _mockIdentityService.Setup(x => x.GetUserIdWithEmail(email)).ReturnsAsync(userId);

        var result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldValidatedId()
    {
        var user = _validUser with { Id = Guid.Empty };

        var result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain(new[] { "IdRequired" });

        user = _validUser with { Id = Guid.NewGuid() };

        result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeFalse();

        errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain(new[] { "UserNotFoundForId" });
    }
}
