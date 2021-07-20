using System.Net.Http;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;

namespace DotAPNS.Extensions
{
    public class ApnsService : IApnsService
    {
        readonly ApnsJwtOptions _jwtOptions;
        readonly IHttpClientFactory _httpClientFactory;

        readonly ConcurrentDictionary<string, ApnsClient> _cachedCertClients = new(); // key is cert thumbprint and sandbox prefix
        readonly ConcurrentDictionary<string, ApnsClient> _cachedJwtClients = new(); // key is bundle id and sandbox prefix

        public ApnsService(IOptions<ApnsJwtOptions> jwtOptions, IHttpClientFactory httpClientFactory)
        {
            _jwtOptions = jwtOptions.Value;
            _httpClientFactory = httpClientFactory;
        }

        public IApnsClient CreateUsingCert(X509Certificate2 cert, bool useSandbox = false)
        {
            string clientCacheId = (useSandbox ? "s_" : "") + cert.Thumbprint;
            ApnsClient client;

            if (_cachedCertClients.ContainsKey(clientCacheId))
            {
                client = _cachedCertClients[clientCacheId];
            }
            else
            {
                var httpClient = _httpClientFactory.CreateClient("dotAPNS_Cert");
                client = _cachedCertClients.GetOrAdd(clientCacheId, _ =>
                ApnsClient.CreateUsingCustomHttpClient(httpClient, cert));
            }

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
            string clientCacheId = (useSandbox ? "s_" : "") + _jwtOptions.BundleId;
            ApnsClient client;

            if (_cachedJwtClients.ContainsKey(clientCacheId))
            {
                client = _cachedJwtClients[clientCacheId];
            }
            else
            {
                var httpClient = _httpClientFactory.CreateClient("dotAPNS_Jwt");
                client = _cachedJwtClients.GetOrAdd(clientCacheId, _ =>
                    ApnsClient.CreateUsingJwt(httpClient, _jwtOptions));
            }

            if (useSandbox)
            {
                client.UseSandbox();
            }

            return client;
        }
    }
}
