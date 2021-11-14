using DotAPNS.Models;
using DotAPNS.Responses;

namespace DotAPNS;

public interface IApnsClient
{
    /// <exception cref="HttpRequestException">Exception occured during connection to an APNs service.</exception>
    /// <exception cref="ApnsCertificateExpiredException">APNs certificate used to connect to an APNs service is expired and needs to be renewed.</exception>
    Task<ApnsResponse> SendAsync(Notification notification, Payload payload, CancellationToken ct = default);
}
