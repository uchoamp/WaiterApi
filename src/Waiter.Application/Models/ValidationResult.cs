using System.Text.Json.Serialization;
using Waiter.Application.Exceptions;

namespace Waiter.Application.Models
{
    public class ValidationResult
    {
        /// <summary>
        /// Validation errors
        /// </summary>
        [JsonPropertyName("errors")]
        public IDictionary<string, string> Errors { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="validationException"></param>
        public ValidationResult(ValidationException validationException)
        {
            Errors = new Dictionary<string, string>();
            foreach (var error in validationException.Errors)
            {
                Errors[error.Code] = error.Description;
            }
        }
    }
}
