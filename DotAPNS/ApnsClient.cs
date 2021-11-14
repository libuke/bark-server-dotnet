using DotAPNS.Exceptions;
using DotAPNS.Models;
using DotAPNS.Responses;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System.ComponentModel;
using System.Dynamic;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace DotAPNS;

public class ApnsClient : IApnsClient
{
    #region Field

    internal const string HostDevelopment = "https://api.sandbox.push.apple.com";
    internal const string HostProduction = "https://api.push.apple.com";

    readonly ECDsa? _key;
    readonly string? _keyId;
    readonly string? _teamId;

    readonly HttpClient _http;
    readonly bool _useCert;

    /// <summary>
    /// True if certificate provided can only be used for 'voip' type pushes, false otherwise.
    /// </summary>
    readonly bool _isVoipCert;
    readonly string _bundleId;

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
        _bundleId = split[1].Replace(".voip", "");
        _useCert = true;
    }

    ApnsClient(HttpClient http, ECDsa key, string keyId, string teamId, string bundleId)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _key = key ?? throw new ArgumentNullException(nameof(key));

        _keyId = keyId ?? throw new ArgumentNullException(nameof(keyId),
            $"Make sure {nameof(ApnsJwtOptions)}.{nameof(ApnsJwtOptions.KeyId)} is set to a non-null value.");

        _teamId = teamId ?? throw new ArgumentNullException(nameof(teamId),
            $"Make sure {nameof(ApnsJwtOptions)}.{nameof(ApnsJwtOptions.TeamId)} is set to a non-null value.");

        _bundleId = bundleId ?? throw new ArgumentNullException(nameof(bundleId),
            $"Make sure {nameof(ApnsJwtOptions)}.{nameof(ApnsJwtOptions.BundleId)} is set to a non-null value.");
    }

    public static ApnsClient CreateUsingJwt(HttpClient http, ApnsJwtOptions options)
    {
        if (http == null) throw new ArgumentNullException(nameof(http));
        if (options == null) throw new ArgumentNullException(nameof(options));

        string certContent;
        if (options.CertFilePath != null)
        {
            certContent = File.ReadAllText(options.CertFilePath);
        }
        else if (options.CertContent != null)
        {
            certContent = options.CertContent;
        }
        else
        {
            throw new ArgumentException("Either certificate file path or certificate contents must be provided.", nameof(options));
        }

        certContent = certContent.Replace("\r", "").Replace("\n", "")
            .Replace("-----BEGIN PRIVATE KEY-----", "").Replace("-----END PRIVATE KEY-----", "");

        certContent = $"-----BEGIN PRIVATE KEY-----\n{certContent}\n-----END PRIVATE KEY-----";
        var ecPrivateKeyParameters = (ECPrivateKeyParameters)new PemReader(new StringReader(certContent)).ReadObject();
        // See https://github.com/dotnet/core/issues/2037#issuecomment-436340605 as to why we calculate q ourselves
        // TL;DR: we don't have Q coords in ecPrivateKeyParameters, only G ones. They won't work.
        var q = ecPrivateKeyParameters.Parameters.G.Multiply(ecPrivateKeyParameters.D).Normalize();
        var d = ecPrivateKeyParameters.D.ToByteArrayUnsigned();
        var msEcp = new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            Q = { X = q.AffineXCoord.GetEncoded(), Y = q.AffineYCoord.GetEncoded() },
            D = d
        };
        var key = ECDsa.Create(msEcp);
        return new ApnsClient(http, key, options.KeyId, options.TeamId, options.BundleId);
    }

    public static ApnsClient CreateUsingCert(X509Certificate2 cert)
    {
        if (cert == null)
        {
            throw new ArgumentNullException(nameof(cert));
        }

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
        if (httpClient == null)
        {
            throw new ArgumentNullException(nameof(httpClient));
        }
        if (cert == null)
        {
            throw new ArgumentNullException(nameof(cert));
        }

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
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

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
        req.Headers.Add("apns-priority", notification.Priority.ToString());
        req.Headers.Add("apns-push-type", notification.PushType.ToString().ToLowerInvariant());
        req.Headers.Add("apns-topic", GetTopic(notification.PushType));

        if (!_useCert)
        {
            req.Headers.Authorization = new AuthenticationHeaderValue("bearer", GetOrGenerateJwt());
        }

        if (notification.Expiration.HasValue)
        {
            var exp = notification.Expiration.Value;
            if (exp == DateTimeOffset.MinValue)
            {
                req.Headers.Add("apns-expiration", "0");
            }
            else
            {
                req.Headers.Add("apns-expiration", exp.ToUnixTimeSeconds().ToString());
            }
        }
        if (!string.IsNullOrEmpty(notification.CollapseID))
        {
            req.Headers.Add("apns-collapse-id", notification.CollapseID);
        }

        var payloadDic = new Dictionary<string, object>
        {
            ["aps"] = payload
        };

        if (payload?.Content != null)
        {
            foreach (var custom in payload.Content)
            {
                if (custom.Value != null)
                {
                    payloadDic[custom.Key] = custom.Value;
                }
            }
        }

        req.Content = new JsonContent(payloadDic);

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

    string GetTopic(ApplePushType pushType)
    {
        return pushType switch
        {
            ApplePushType.Background or ApplePushType.Alert => _bundleId,
            ApplePushType.Voip => _bundleId + ".voip",
            _ => throw new ArgumentOutOfRangeException(nameof(pushType), pushType, null),
        };
    }

    string GetOrGenerateJwt()
    {
        string header = JsonSerializer.Serialize((new { alg = "ES256", kid = _keyId }));
        string payload = JsonSerializer.Serialize(new { iss = _teamId, iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
        string headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(header));
        string payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
        string unsignedJwtData = $"{headerBase64}.{payloadBase64}";
        byte[] signature = _key!.SignData(Encoding.UTF8.GetBytes(unsignedJwtData), HashAlgorithmName.SHA256);

        return $"{unsignedJwtData}.{Convert.ToBase64String(signature)}";
    }

    #endregion
}