namespace Waiter.Application.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException()
            : base("Resource not found.") { }

        public ResourceNotFoundException(string resourceName)
            : base($"{resourceName} not found.") { }
    }
}
