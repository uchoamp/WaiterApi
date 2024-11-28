using Waiter.Application.Exceptions;
using Waiter.Application.Models.Customers;
using Waiter.Application.UseCases.Customers;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UnitTests.UseCases.Customers;

public class UpdateCustomerUseCaseTest
{
    private readonly UpdateCustomerUseCase _sut;
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly CustomerRequest _validCustomer;
    private readonly Guid _id;

    public UpdateCustomerUseCaseTest()
    {
        _mockCustomerRepository = new Mock<ICustomerRepository>();

        _sut = new UpdateCustomerUseCase(_mockCustomerRepository.Object);

        _validCustomer = new CustomerRequest("Marcos", "Uchoa", "(86) 981732880");
        _id = Guid.NewGuid();

        _mockCustomerRepository.Setup(x => x.ExistsEntity(_id)).ReturnsAsync(true);
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenUserRequestIsInvalid()
    {
        var user = _validCustomer with { FirstName = "" };

        var exceptionAssert = await _sut.Invoking(x => x.Update(_id, user))
            .Should()
            .ThrowExactlyAsync<ApplicationValidationException>();

        var errorCodes = exceptionAssert.Which.Errors.Select(x => x.Code);

        errorCodes.Should().NotBeEmpty();
        errorCodes.Should().Contain(new string[] { "FirstNameRequired", });
    }

    [Fact]
    public async Task ShouldThrowResourceNotFoundWhenEntityNotExists()
    {
        _mockCustomerRepository.Setup(x => x.ExistsEntity(_id)).ReturnsAsync(false);

        var exceptionAssert = await _sut.Invoking(x => x.Update(_id, _validCustomer))
            .Should()
            .ThrowExactlyAsync<ResourceNotFoundException>();

        exceptionAssert.WithMessage("Customer*");
    }

    [Fact]
    public async Task ShouldReturnUserReponseWhenIsSuccessfullyCreated()
    {
        var customerDatabase = new Customer
        {
            Id = _id,
            FirstName = "First",
            LastName = "Last",
            PhoneNumber = "9898988989",
        };

        _mockCustomerRepository.Setup(x => x.GetByIdAsync(_id)).ReturnsAsync(customerDatabase);

        var result = await _sut.Update(_id, _validCustomer);

        result.Should().NotBeNull();
        result.FirstName.Should().Be(customerDatabase.FirstName);
        result.LastName.Should().Be(customerDatabase.LastName);
        result.PhoneNumber.Should().Be(customerDatabase.PhoneNumber);

        customerDatabase.FirstName.Should().Be(_validCustomer.FirstName);
        customerDatabase.LastName.Should().Be(_validCustomer.LastName);
        customerDatabase.PhoneNumber.Should().Be(_validCustomer.PhoneNumber.RemoveMask());

        _mockCustomerRepository.Verify(x => x.Update(customerDatabase), Times.Once);
        _mockCustomerRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
