namespace Waiter.Application.Exceptions
{
    public record ValidationItem(string Code, string Description) { }

    public class ApplicationValidationException : Exception
    {
        public ValidationItem[] Errors { get; }

        public ApplicationValidationException(params ValidationItem[] errors)
            : base("Validation errors occurred")
        {
            Errors = errors;
        }

        public ApplicationValidationException(IEnumerable<ValidationItem> errors)
            : base("Validation errors occurred")
        {
            Errors = errors.ToArray();
        }

        public ApplicationValidationException(
            FluentValidation.Results.ValidationResult validationResult
        )
            : base("Validation errors occurred")
        {
            Errors = validationResult
                .Errors.Select(vf => new ValidationItem(vf.ErrorCode, vf.ErrorMessage))
                .ToArray();
        }
    }
}
