using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotAPNS.Extensions
{
    public static class ApnsServiceExtensions
    {
        public static IServiceCollection AddApns<T>(this IServiceCollection services, int httpTimeout, Action<ApnsOptionsBuilder<T>> optionsAction) where T : class
        {
            services.AddHttpClient("dotAPNS_Jwt", x => 
            { 
                x.Timeout = TimeSpan.FromSeconds(httpTimeout);
            });
            services.AddHttpClient("dotAPNS_Cert")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (m, x, c, s) => true
                });
            optionsAction?.Invoke(new ApnsOptionsBuilder<T>(services));
            services.AddSingleton<IApnsService, ApnsService>();
            return services;
        }
    }

    public class ApnsOptionsBuilder<T> where T : class
    {
        readonly IServiceCollection _services;
        public ApnsOptionsBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public ApnsOptionsBuilder<T> UseApnsJwt(IConfiguration section)
        {
            _services.Configure<T>(section);
            return this;
        }
    }
}
