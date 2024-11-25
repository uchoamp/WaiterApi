namespace Waiter.API.Exceptions
{
    /// <summary>
    /// Exception for validate appsettings
    /// </summary>
    public class InvalidConfigurationException : Exception
    {
        /// <summary>
        /// InvalidConfigurationException constructor
        /// </summary>
        /// <param name="key">Name appsettings Key</param>
        public InvalidConfigurationException(string key)
            : base($"{key} was not defined or is invalid.") { }
    }
}
