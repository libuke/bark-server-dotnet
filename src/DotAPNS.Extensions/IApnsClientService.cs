using System.Security.Cryptography.X509Certificates;

namespace DotAPNS.Extensions;

public interface IApnsClientService
{
    IApnsClient CreateUsingCert(X509Certificate2 cert, bool useSandbox = false);

    IApnsClient CreateUsingCert(string pathToCert, bool useSandbox = false);

    IApnsClient CreateUsingJwt(bool useSandbox = false);
}
