using System.Threading;
using System.Threading.Tasks;

namespace DotAPNS
{
    public interface IApnsClient
    {
        /// <exception cref="HttpRequestException">Exception occured during connection to an APNs service.</exception>
        /// <exception cref="ApnsCertificateExpiredException">APNs certificate used to connect to an APNs service is expired and needs to be renewed.</exception>
        Task<ApnsResponse> SendAsync(ApplePush push, CancellationToken ct = default);
    }
}
