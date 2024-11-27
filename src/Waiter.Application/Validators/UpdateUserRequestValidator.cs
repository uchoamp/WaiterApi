using FluentValidation;
using Waiter.Application.Models.Users;
using Waiter.Application.Security;

namespace Waiter.Application.Validators
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        private readonly IIdentityService _identityService;

        public UpdateUserRequestValidator(IIdentityService identityService)
        {
            _identityService = identityService;

            RuleFor(x => x.Id)
                .NotEqual(Guid.Empty)
                .WithMessage("Id is required.")
                .WithErrorCode("IdRequired")
                .MustAsync(
                    async (userId, cancellationToken) =>
                    {
                        var user = await _identityService.GetUserAsync(userId);
                        return user != null;
                    }
                )
                .WithMessage("User not found.")
                .WithErrorCode("UserNotFoundForId")
                .When(x => x.Id != Guid.Empty, ApplyConditionTo.CurrentValidator);

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
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .WithErrorCode("PhoneNumberRequired")
                .Matches(@"^\+? ?(?:55)? ?\(?[1-9]\d\)? ?(?:(?:9\d{4})|(?:[1-9]\d{3}))-?\d{4}$")
                .WithMessage("Phone number informed is not valid.")
                .WithErrorCode("PhoneNumberInvalid");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .WithErrorCode("EmailRequired")
                .EmailAddress()
                .WithMessage("Email addresss informed is not valid.")
                .WithErrorCode("EmailInvalid")
                .MustAsync(
                    async (userRequest, email, cancellationToken) =>
                    {
                        var userIdExists = await _identityService.GetUserIdWithEmail(email);

                        if (userIdExists == null || userIdExists == userRequest.Id)
                            return true;

                        return false;
                    }
                )
                .WithMessage("Email already registered.")
                .WithErrorCode("EmailAlreadyRegistered")
                .When(x => !string.IsNullOrWhiteSpace(x.Email), ApplyConditionTo.CurrentValidator);
        }
    }
}
