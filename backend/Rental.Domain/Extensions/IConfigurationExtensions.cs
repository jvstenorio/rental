using Microsoft.Extensions.Configuration;

namespace Rental.Domain.Extensions
{
    public static class IConfigurationExtensions
    {
        public static int GetTimeoutInSeconds(this IConfiguration configuration) => int.Parse(configuration.GetSection("TIME_OUT").Value);
    }
}
