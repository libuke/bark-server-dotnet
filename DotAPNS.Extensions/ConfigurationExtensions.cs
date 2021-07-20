using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            => configuration.GetSection($"ApnsJwtOptions:{name}");
    }
}
