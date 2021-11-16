using DotAPNS.Exceptions;
using DotAPNS.Models;
using DotAPNS.Responses;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace DotAPNS;

public class ApnsClient : IApnsClient
{
    #region Field

    internal const string HostDevelopment = "https://api.sandbox.push.apple.com";
    internal const string HostProduction = "https://api.push.apple.com";

    readonly ApnsJwtOptions? _jwtOptions;

    /// <summary>
    /// True if certificate provided can only be used for 'voip' type pushes, false otherwise.
    /// </summary>
    readonly bool _isVoipCert;

    readonly HttpClient _http;
    readonly bool _useCert;

    bool _useSandbox;
    bool _useBackupPort;

    #endregion

    #region Constructor

    ApnsClient(HttpClient http, X509Certificate cert)
    {
        _http = http;
        var split = cert.Subject.Split(new[] { "0.9.2342.19200300.100.1.1=" }, StringSplitOptions.RemoveEmptyEntries);
        if (split.Length != 2)
        {
            // On Linux .NET Core cert.Subject prints `userId=xxx` instead of `0.9.2342.19200300.100.1.1=xxx`
            split = cert.Subject.Split(new[] { "userId=" }, StringSplitOptions.RemoveEmptyEntries);
        }
        if (split.Length != 2)
        {
            // if subject prints `uid=xxx` instead of `0.9.2342.19200300.100.1.1=xxx`
            split = cert.Subject.Split(new[] { "uid=" }, StringSplitOptions.RemoveEmptyEntries);
        }

        if (split.Length != 2)
        {
            throw new InvalidOperationException("Provided certificate does not appear to be a valid APNs certificate.");
        }

        string topic = split[1];
        _isVoipCert = topic.EndsWith(".voip");
        _useCert = true;
    }

    ApnsClient(HttpClient http, ApnsJwtOptions jwtOptions)
    {
        _http = http;
        _jwtOptions = jwtOptions;
    }

    public static ApnsClient CreateUsingJwt(HttpClient http, ApnsJwtOptions jwtOptions)
    {
        http = http ?? throw new ArgumentNullException(nameof(http));
        jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));

        return new ApnsClient(http, jwtOptions);
    }

    public static ApnsClient CreateUsingCert(X509Certificate2 cert)
    {
        cert = cert ?? throw new ArgumentNullException(nameof(cert));

        var handler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
        };

        handler.ClientCertificates.Add(cert);
        var client = new HttpClient(handler);
        return CreateUsingCustomHttpClient(client, cert);
    }

    public static ApnsClient CreateUsingCustomHttpClient(HttpClient httpClient, X509Certificate2 cert)
    {
        httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        cert = cert ?? throw new ArgumentNullException(nameof(cert));

        var apns = new ApnsClient(httpClient, cert);
        return apns;
    }

    public static ApnsClient CreateUsingCert(string pathToCert, string? certPassword = null)
    {
        if (string.IsNullOrWhiteSpace(pathToCert))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(pathToCert));
        }

        var cert = new X509Certificate2(pathToCert, certPassword);
        return CreateUsingCert(cert);
    }

    #endregion

    #region Method
    public async Task<ApnsResponse> SendAsync(Notification notification, Payload payload, CancellationToken ct = default)
    {
        notification = notification ?? throw new ArgumentNullException(nameof(notification));

        if (_useCert)
        {
            if (_isVoipCert && notification.PushType != ApplePushType.Voip)
            {
                throw new InvalidOperationException("Provided certificate can only be used to send 'voip' type pushes.");
            }
        }

        string url = $"{(_useSandbox ? HostDevelopment : HostProduction)}{(_useBackupPort ? ":2197" : ":443")}/3/device/{notification.DeviceToken ?? notification.Topic}";
        var req = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Version = new Version(2, 0)
        };

        if (!string.IsNullOrWhiteSpace(notification.Topic))
        {
            req.Headers.Add("apns-topic", notification.Topic);
        }

        if (!string.IsNullOrWhiteSpace(notification.ApnsID))
        {
            req.Headers.Add("apns-id", notification.ApnsID);
        }

        if (!string.IsNullOrWhiteSpace(notification.CollapseID))
        {
            req.Headers.Add("apns-collapse-id", notification.CollapseID);
        }

        if (notification.Priority > 0)
        {
            req.Headers.Add("apns-priority", notification.Priority.ToString());
        }

        if (notification.Expiration.HasValue && notification.Expiration != DateTimeOffset.MinValue)
        {
            req.Headers.Add("apns-expiration", notification.Expiration.Value.ToUnixTimeSeconds().ToString());
        }

        req.Headers.Add("apns-push-type", notification.PushType.ToString().ToLowerInvariant());

        if (!_useCert)
        {
            req.Headers.Authorization = new AuthenticationHeaderValue("bearer", _jwtOptions!.GenerateIfExpired());
        }

        req.Content = payload?.MarshalJsonContent();

        HttpResponseMessage resp;
        try
        {
            resp = await _http.SendAsync(req, ct).ConfigureAwait(false);
        }
        catch (HttpRequestException ex) when (
            (Environment.OSVersion.Platform is PlatformID.Win32NT &&
            ex.InnerException is AuthenticationException { InnerException: Win32Exception { NativeErrorCode: -2146893016 } }) ||
            (Environment.OSVersion.Platform is PlatformID.Unix &&
            ex.InnerException is IOException { InnerException: IOException { InnerException: IOException { InnerException: { InnerException: { HResult: 336151573 } } } } }))
        {
            throw new ApnsCertificateExpiredException(innerException: ex);
        }

        var respContent = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        // Process status codes specified by APNs documentation
        // https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/handling_notification_responses_from_apns
        var statusCode = (int)resp.StatusCode;

        // Push has been successfully sent. This is the only code indicating a success as per documentation.
        if (statusCode == 200)
        {
            return ApnsResponse.Successful();
        }

        // something went wrong
        // check for payload 
        // {"reason":"DeviceTokenNotForTopic"}
        // {"reason":"Unregistered","timestamp":1454948015990}

        ApnsErrorResponsePayload? errorPayload;
        try
        {
            errorPayload = JsonSerializer.Deserialize<ApnsErrorResponsePayload>(respContent);
        }
        catch (JsonException)
        {
            return ApnsResponse.Error(ApnsResponseReason.Unknown,
                $"Status: {statusCode}, reason: {respContent ?? "not specified"}.");
        }

        return ApnsResponse.Error(errorPayload?.Reason, errorPayload?.ReasonRaw);
    }

    public ApnsClient UseSandbox()
    {
        _useSandbox = true;
        return this;
    }

    /// <summary>
    /// Use port 2197 instead of 443 to connect to the APNs server.
    /// You might use this port to allow APNs traffic through your firewall but to block other HTTPS traffic.
    /// </summary>
    /// <returns></returns>
    public ApnsClient UseBackupPort()
    {
        _useBackupPort = true;
        return this;
    }

    #endregion
}