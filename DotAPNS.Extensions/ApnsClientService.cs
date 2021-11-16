using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace DotAPNS.Extensions;

public class ApnsClientService : IApnsClientService
{
    readonly ApnsStrings _apnsStrings;
    readonly ApnsJwtOptions _jwtOptions;
    readonly IHttpClientFactory _httpClientFactory;

    public ApnsClientService(IOptions<ApnsStrings> apnsStrings, IHttpClientFactory httpClientFactory)
    {
        _apnsStrings = apnsStrings.Value;
        _httpClientFactory = httpClientFactory;

        _jwtOptions = new ApnsJwtOptions(_apnsStrings.KeyID, _apnsStrings.TeamID,
            ApnsJwtOptions.AuthKeyFromFile(_apnsStrings.FilePath));
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
