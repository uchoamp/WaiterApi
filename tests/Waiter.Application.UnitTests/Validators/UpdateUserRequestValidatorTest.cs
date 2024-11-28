using Waiter.Application.Models.Users;
using Waiter.Application.Security;
using Waiter.Application.Validators.Users;

namespace Waiter.Application.UnitTests.Validators;

public class UpdateUserRequestValidatorTest
{
    private readonly UpdateUserRequestValidator _validator;
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly UpdateUserRequest _validUser;

    public UpdateUserRequestValidatorTest()
    {
        _validUser = new UpdateUserRequest(
            Guid.NewGuid(),
            "Marcos",
            "Uchoa",
            "86981732880",
            "marcos@email.com"
        );

        _mockIdentityService = new Mock<IIdentityService>();

        _mockIdentityService
            .Setup(x => x.GetUserAsync(_validUser.Id))
            .ReturnsAsync(
                new UserResponse(
                    _validUser.Id,
                    _validUser.FirstName,
                    _validUser.LastName,
                    _validUser.Email,
                    _validUser.PhoneNumber,
                    new string[0]
                )
            );

        _validator = new UpdateUserRequestValidator(_mockIdentityService.Object);

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
        var email = "teste@email.com";
        var user = _validUser with { Email = email };

        _mockIdentityService.Setup(x => x.GetUserIdWithEmail(email)).ReturnsAsync(_validUser.Id);

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

    [Theory]
    [InlineData(null, new[] { "PhoneNumberRequired" })]
    [InlineData("", new[] { "PhoneNumberRequired" })]
    [InlineData("1234", new[] { "PhoneNumberInvalid" })]
    [InlineData("869817328800", new[] { "PhoneNumberInvalid" })]
    [InlineData("06981732880", new[] { "PhoneNumberInvalid" })]
    [InlineData("8601732880", new[] { "PhoneNumberInvalid" })]
    [InlineData("+55 (86) 98173-2880", new[] { "PhoneNumberInvalid" })]
    public async Task ShouldValidatedPhoneNumber(string phoneNumber, string[] expectedCodes)
    {
        var user = _validUser with { PhoneNumber = phoneNumber };

        var result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain(expectedCodes);
    }
}
