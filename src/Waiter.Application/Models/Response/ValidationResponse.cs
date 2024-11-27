using Waiter.Application.Exceptions;

namespace Waiter.Application.Models.Response
{
    public class ValidationResponse
    {
        /// <summary>
        /// Validation errors
        /// </summary>
        public IDictionary<string, string> Errors { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="validationException"></param>
        public ValidationResponse(ApplicationValidationException validationException)
        {
            Errors = new Dictionary<string, string>();
            foreach (var error in validationException.Errors)
            {
                Errors[error.Code] = error.Description;
            }
        }
    }
}
