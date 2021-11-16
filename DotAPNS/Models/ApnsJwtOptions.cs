using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DotAPNS;

public class ApnsJwtOptions
{
    readonly object _jwtRefreshLock = new();

    #region Property

    /// <summary>
    /// The 10-character Key ID you obtained from your developer account. See <a href="https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/establishing_a_token-based_connection_to_apns#2943371">Reference</a>.
    /// </summary>
    public string KeyID { get; }

    /// <summary>
    /// 10-character Team ID you use for developing your company's apps.
    /// </summary>
    public string TeamID { get; }

    public DateTime? IssuedAt { get; private set; }

    public string? Bearer { get; private set; }

    public ECDsa AuthKey { get; }

    #endregion

    public ApnsJwtOptions(string keyID, string teamID, ECDsa authKey)
    {
        KeyID = keyID ?? throw new ArgumentNullException(nameof(keyID));
        TeamID = teamID ?? throw new ArgumentNullException(nameof(teamID));
        AuthKey = authKey ?? throw new ArgumentNullException(nameof(authKey));
    }

    // AuthKeyFromFile loads a .p8 certificate from a local file and returns a
    public static ECDsa AuthKeyFromFile(string filePath)
    {
        var certContent = File.ReadAllText(filePath);
        return AuthKeyFromContent(certContent);
    }

    // AuthKeyFromContent loads a .p8 certificate from an in Content
    public static ECDsa AuthKeyFromContent(string certContent)
    {
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

        return ECDsa.Create(msEcp);
    }

    // GenerateIfExpired checks to see if the token is about to expire and
    // generates a new token.
    internal string GenerateIfExpired()
    {
        lock (_jwtRefreshLock)
        {
            if (IssuedAt > DateTime.UtcNow - TimeSpan.FromMinutes(20)) // refresh no more than once every 20 minutes
            {
                return Bearer!;
            }

            var now = DateTimeOffset.UtcNow;
            string header = JsonSerializer.Serialize((new { alg = "ES256", kid = KeyID }));
            string payload = JsonSerializer.Serialize(new { iss = TeamID, iat = now.ToUnixTimeSeconds() });
            string headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(header));
            string payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
            string unsignedJwtData = $"{headerBase64}.{payloadBase64}";
            byte[] signature = AuthKey!.SignData(Encoding.UTF8.GetBytes(unsignedJwtData), HashAlgorithmName.SHA256);
            IssuedAt = now.UtcDateTime;
            Bearer = $"{unsignedJwtData}.{Convert.ToBase64String(signature)}";

            return Bearer;
        }
    }
}
