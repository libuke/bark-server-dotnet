using Microsoft.Extensions.Configuration;
using System;

namespace DotAPNS.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Shorthand for GetSection("ApnsJwtOptions")[name].
        /// </summary>
        /// <param name="name">The ApnsJwtOptions string key.</param>
        /// <returns> The IConfiguration.</returns>
        public static IConfiguration GetApnsJwtOptions(this IConfiguration configuration, string name)
        {
            var section = configuration.GetSection($"ApnsJwtOptions:{name}");

            if (section.Exists())
            {
                return section;
            }
            else
            {
               throw new ArgumentNullException("Value cannot be null. (Parameter 'ApnsJwtOptions')");
            }
        }
    }
}