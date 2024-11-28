using FluentValidation;
using Waiter.Application.Models.Customers;
using Waiter.Domain.Repositories;

namespace Waiter.Application.Validators.Customers
{
    public class CustomerRequestValidator : AbstractValidator<CustomerRequest>
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerRequestValidator(Guid id, ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .WithErrorCode("FirstNameRequired")
                .MinimumLength(2)
                .WithMessage("First name must be at least 2 characters.")
                .WithErrorCode("FirstNameInvalid");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .WithErrorCode("LastNameRequired")
                .MinimumLength(2)
                .WithMessage("Last name must be at least 2 characters.")
                .WithErrorCode("LastNameInvalid");

            RuleFor(x => x.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .WithErrorCode("PhoneNumberRequired")
                .Matches(@"^\(?[1-9]\d\)? ?9\d{4}-?\d{4}$")
                .WithMessage("Phone number informed is not valid.")
                .WithErrorCode("PhoneNumberInvalid")
                .MustAsync(
                    async (phoneNumber, cancellationToken) =>
                    {
                        var customerIdExists = await _customerRepository.FindIdWithPhoneNumber(
                            phoneNumber.RemoveMask()
                        );

                        return customerIdExists == Guid.Empty || customerIdExists == id;
                    }
                )
                .WithMessage("Phone number already registered for another customer.")
                .WithErrorCode("PhoneNumberAlreadyRegistered");
        }
    }
}
