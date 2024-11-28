using Waiter.Application.Models.Users;
using Waiter.Application.Security;
using Waiter.Application.Validators.Users;

namespace Waiter.Application.UnitTests.Validators;

public class UserRequestValidatorTest
{
    private readonly NewUserRequestValidator _validator;
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly NewUserRequest _validUser;

    public UserRequestValidatorTest()
    {
        _mockIdentityService = new Mock<IIdentityService>();

        _validator = new NewUserRequestValidator(_mockIdentityService.Object);

        _validUser = new NewUserRequest(
            "Marcos",
            "Uchoa",
            "marcos@email.com",
            "86981372880",
            "Password123!",
            new[] { "admin" }
        );

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

    [Theory]
    [InlineData(null, new[] { "PasswordRequired" })]
    [InlineData("", new[] { "PasswordRequired" })]
    [InlineData("Pass12!", new[] { "PasswordAtLeast8" })]
    [InlineData("pass123!", new[] { "PasswordUppercaseRequired" })]
    [InlineData("PASS1234", new[] { "PasswordLowercaseRequired" })]
    [InlineData("PASSword!", new[] { "PasswordNumberRequired" })]
    [InlineData("Pass12345", new[] { "PasswordNoAlphaNumericRequired" })]
    public async Task ShouldValidatedPasword(string password, string[] expectedCodes)
    {
        var user = _validUser with { Password = password };

        var result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain(expectedCodes);
    }

    [Theory]
    [InlineData(null, new[] { "admin" }, new[] { "RolesRequired" })]
    [InlineData(new string[0], new[] { "admin" }, new[] { "RolesValidRequired" })]
    [InlineData(new[] { "no-admin", "admin" }, new[] { "admin" }, new[] { "RolesValidRequired" })]
    public async Task ShouldValidatedRoles(
        string[] roles,
        string[] databaseRoles,
        string[] expectedCodes
    )
    {
        _mockIdentityService.Setup(x => x.GetRolesAsync()).ReturnsAsync(databaseRoles.ToHashSet());

        var user = _validUser with { Roles = roles };

        var result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain(expectedCodes);
    }

    [Theory]
    [InlineData(null, new[] { "PhoneNumberRequired" })]
    [InlineData("", new[] { "PhoneNumberRequired" })]
    [InlineData("1234", new[] { "PhoneNumberInvalid" })]
    [InlineData("869817328800", new[] { "PhoneNumberInvalid" })]
    [InlineData("06981732880", new[] { "PhoneNumberInvalid" })]
    [InlineData("8601732880", new[] { "PhoneNumberInvalid" })]
    public async Task ShouldValidatedPhoneNumber(string phoneNumber, string[] expectedCodes)
    {
        var user = _validUser with { PhoneNumber = phoneNumber };

        var result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain(expectedCodes);
    }
}
