using Microsoft.Extensions.Configuration;

namespace Rental.Domain.Extensions
{
    public static class IConfigurationExtensions
    {
        public static int GetTimeoutInSec(this IConfiguration configuration) => int.Parse(configuration.GetSection("TIME_OUT").Value);
        public static int GetSizeLimit(this IConfiguration configuration) => int.Parse(configuration.GetSection("CACHE_SIZE_LIMIT").Value);
        public static int GetAsoluteExpirationInSec(this IConfiguration configuration) => int.Parse(configuration.GetSection("CACHE_ABSOLUTE_EXPIRATION_IN_SEC").Value);
        public static int GetSlidingExpirationInSec(this IConfiguration configuration) => int.Parse(configuration.GetSection("CACHE_SLIDING_EXPIRATION_IN_SEC").Value);
    }
}
