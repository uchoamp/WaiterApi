using Waiter.Application.Models.Customers;
using Waiter.Application.Validators.Customers;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UnitTests.Validators;

public class CustomerRequestValidatorTest
{
    private readonly CustomerRequestValidator _validator;

    private readonly Mock<ICustomerRepository> _mockCustomerRepository;

    private readonly CustomerRequest _validCustomer;

    public CustomerRequestValidatorTest()
    {
        _validCustomer = new CustomerRequest("Marcos", "Uchoa", "86981732880");

        _mockCustomerRepository = new Mock<ICustomerRepository>();

        _validator = new CustomerRequestValidator(Guid.Empty, _mockCustomerRepository.Object);
    }

    [Fact]
    public async Task ShouldBeValidIfUserIsValidNew()
    {
        var result = await _validator.ValidateAsync(_validCustomer);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, new[] { "FirstNameRequired" })]
    [InlineData("M", new[] { "FirstNameInvalid" })]
    public async Task ShouldValidatedFirstName(string firstName, string[] expectedCodes)
    {
        var user = _validCustomer with { FirstName = firstName };

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
        var user = _validCustomer with { LastName = lastName };

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
        var user = _validCustomer with { PhoneNumber = phoneNumber };

        var result = await _validator.ValidateAsync(user);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain(expectedCodes);
    }

    [Fact]
    public async Task ShouldValidatedPhoneNumerAlreadyRegistered()
    {
        _mockCustomerRepository
            .Setup(x => x.FindIdWithPhoneNumbe(_validCustomer.PhoneNumber))
            .ReturnsAsync(Guid.NewGuid());

        var result = await _validator.ValidateAsync(_validCustomer);

        result.IsValid.Should().BeFalse();

        var errorCodes = result.Errors.Select(x => x.ErrorCode);

        errorCodes.Should().Contain("PhoneNumberAlreadyRegistered");
    }

    [Fact]
    public async Task ShouldPhoneNumberValidWhenUpdateSameId()
    {
        var id = Guid.NewGuid();
        var validator = new CustomerRequestValidator(id, _mockCustomerRepository.Object);

        _mockCustomerRepository
            .Setup(x => x.FindIdWithPhoneNumbe(_validCustomer.PhoneNumber))
            .ReturnsAsync(id);

        var result = await validator.ValidateAsync(_validCustomer);

        result.IsValid.Should().BeTrue();
    }
}
