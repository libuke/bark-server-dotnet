using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace DotAPNS.Extensions
{
    public class ApnsService : IApnsService
    {
        readonly ApnsJwtOptions _jwtOptions;
        readonly IHttpClientFactory _httpClientFactory;

        public ApnsService(IOptions<ApnsJwtOptions> jwtOptions, IHttpClientFactory httpClientFactory)
        {
            _jwtOptions = jwtOptions.Value;
            _httpClientFactory = httpClientFactory;
        }

        public IApnsClient CreateUsingCert(X509Certificate2 cert, bool useSandbox = false)
        {
            var httpClient = _httpClientFactory.CreateClient("dotAPNS_Cert");
            var client = ApnsClient.CreateUsingCustomHttpClient(httpClient, cert);

            if (useSandbox)
            {
                client.UseSandbox();
            }

            return client;
        }

        public IApnsClient CreateUsingCert(string pathToCert, bool useSandbox = false)
        {
            var cert = new X509Certificate2(pathToCert);
            return CreateUsingCert(cert, useSandbox);
        }

        public IApnsClient CreateUsingJwt(bool useSandbox = false)
        {
            var httpClient = _httpClientFactory.CreateClient("dotAPNS_Jwt");
            var client = ApnsClient.CreateUsingJwt(httpClient, _jwtOptions);

            if (useSandbox)
            {
                client.UseSandbox();
            }

            return client;
        }
    }
}
