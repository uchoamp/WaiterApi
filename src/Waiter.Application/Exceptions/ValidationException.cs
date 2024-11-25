namespace Waiter.Application.Exceptions
{
    public record ValidationItem(string Code, string Description) { }

    public class ValidationException : Exception
    {
        public ValidationItem[] Errors { get; }

        public ValidationException(params ValidationItem[] errors)
            : base("Validation errors occurred")
        {
            Errors = errors;
        }
    }
}
