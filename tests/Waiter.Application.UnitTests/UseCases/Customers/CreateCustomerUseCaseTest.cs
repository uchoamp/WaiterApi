using Waiter.Application.Exceptions;
using Waiter.Application.Models.Customers;
using Waiter.Application.UseCases.Customers;
using Waiter.Domain.Models;
using Waiter.Domain.Repositories;

namespace Waiter.Application.UnitTests.UseCases.Customers;

public class CreateCustomerUseCaseTest
{
    private readonly CreateCustomerUseCase _sut;
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly CustomerRequest _validCustomer;

    public CreateCustomerUseCaseTest()
    {
        _mockCustomerRepository = new Mock<ICustomerRepository>();

        _sut = new CreateCustomerUseCase(_mockCustomerRepository.Object);

        _validCustomer = new CustomerRequest("Marcos", "Uchoa", "86981732880");
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenUserRequestIsInvalid()
    {
        var user = _validCustomer with { FirstName = "" };

        var exceptionAssert = await _sut.Invoking(x => x.Create(user))
            .Should()
            .ThrowExactlyAsync<ApplicationValidationException>();

        var errorCodes = exceptionAssert.Which.Errors.Select(x => x.Code);

        errorCodes.Should().NotBeEmpty();
        errorCodes.Should().Contain(new string[] { "FirstNameRequired", });
    }

    [Fact]
    public async Task ShouldReturnUserReponseWhenIsSuccessfullyCreated()
    {
        var result = await _sut.Create(_validCustomer);

        result.FirstName.Should().Be(_validCustomer.FirstName);
        result.LastName.Should().Be(_validCustomer.LastName);
        result.PhoneNumber.Should().Be(_validCustomer.PhoneNumber.RemoveMask());

        _mockCustomerRepository.Verify(
            x =>
                x.CreateAsync(
                    It.Is<Customer>(c =>
                        c.FirstName == _validCustomer.FirstName
                        && c.LastName == _validCustomer.LastName
                        && c.PhoneNumber == _validCustomer.PhoneNumber.RemoveMask()
                    )
                ),
            Times.Once
        );

        _mockCustomerRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
