using Microsoft.IdentityModel.Protocols.Configuration;
using Waiter.Domain.Constants;

namespace Waiter.API;

/// <summary>
///
/// </summary>
public static class ValidateSettings
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="configuration"></param>
    public static void Validate(IConfiguration configuration)
    {
        Validate(configuration, ApplicationSettings.DatabaseConnectionString);
        Validate(configuration, ApplicationSettings.JwtKey);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="key"></param>
    /// <exception cref="InvalidConfigurationException"></exception>
    private static void Validate(IConfiguration configuration, string key)
    {
        if (string.IsNullOrWhiteSpace(configuration[key]))
            throw new InvalidConfigurationException(key);
    }
}
