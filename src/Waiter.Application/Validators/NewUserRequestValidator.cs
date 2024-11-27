using FluentValidation;
using Waiter.Application.Models.Users;
using Waiter.Application.Security;

namespace Waiter.Application.Validators
{
    public class NewUserRequestValidator : AbstractValidator<NewUserRequest>
    {
        private readonly IIdentityService _identityService;

        public NewUserRequestValidator(IIdentityService identityService)
        {
            _identityService = identityService;

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

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .WithErrorCode("PasswordRequired")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.")
                .WithErrorCode("PasswordAtLeast8")
                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .WithErrorCode("PasswordUppercaseRequired")
                .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .WithErrorCode("PasswordLowercaseRequired")
                .Matches("[0-9]")
                .WithMessage("Password must contain at least one number.")
                .WithErrorCode("PasswordNumberRequired")
                .Matches("[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one special character.")
                .WithErrorCode("PasswordNoAlphaNumericRequired");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .WithErrorCode("EmailRequired")
                .EmailAddress()
                .WithMessage("Email addresss informed is not valid.")
                .WithErrorCode("EmailInvalid")
                .MustAsync(
                    async (email, cancellationToken) =>
                    {
                        var userIdExists = await _identityService.GetUserIdWithEmail(email);

                        if (userIdExists == null)
                            return true;

                        return false;
                    }
                )
                .WithMessage("Email already registered.")
                .WithErrorCode("EmailAlreadyRegistered")
                .When(x => !string.IsNullOrWhiteSpace(x.Email), ApplyConditionTo.CurrentValidator);

            RuleFor(x => x.Roles)
                .NotNull()
                .WithMessage("At least one valid role is required.")
                .WithErrorCode("RolesRequired")
                .MustAsync(
                    async (roles, cancellationToken) =>
                    {
                        if (roles.Length > 0)
                        {
                            var rolesAvailable = await _identityService.GetRolesAsync();

                            if (Array.TrueForAll(roles, x => rolesAvailable.Contains(x)))
                                return true;
                        }

                        return false;
                    }
                )
                .WithMessage("At least one valid role is required or invalid role is informed.")
                .WithErrorCode("RolesValidRequired")
                .When(x => x.Roles != null, ApplyConditionTo.CurrentValidator);
        }
    }
}
