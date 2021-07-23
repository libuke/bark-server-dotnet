using System;

namespace DotAPNS
{
    public class ApnsJwtOptions
    {

        string? _certFilePath;

        /// <summary>
        /// Path to a .p8 certificate containing a key to be used to encrypt JWT. If specified, <see cref="CertContent"/> must be null.
        /// </summary>
        public string CertFilePath
        {
            get => _certFilePath ?? CertContent ?? throw new InvalidOperationException("Either path to the certificate or certificate's contents must be provided, not both.");
            set => _certFilePath = value;
        }


        string? _certContent;

        /// <summary>
        /// Contents of a .p8 certificate containing a key to be used to encrypt JWT. Can include BEGIN/END headers, line breaks, etc. If specified, <see cref="CertContent"/> must be null.
        /// </summary>
        public string CertContent
        {
            get => _certContent ?? CertFilePath ?? throw new InvalidOperationException("Either path to the certificate or certificate's contents must be provided, not both.");
            set => _certContent = value;
        }

        string? _keyId;
        /// <summary>
        /// The 10-character Key ID you obtained from your developer account. See <a href="https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/establishing_a_token-based_connection_to_apns#2943371">Reference</a>.
        /// </summary>
        public string KeyId
        {
            get => _keyId ?? throw new ArgumentNullException(nameof(KeyId));
            set => _keyId = value;
        }

        string? _teamId;

        /// <summary>
        /// 10-character Team ID you use for developing your company's apps.
        /// </summary>
        public string TeamId
        {
            get => _teamId ?? throw new ArgumentNullException(nameof(TeamId));
            set => _teamId = value;
        }


        string? _bundleId;
        /// <summary>
        /// Your app's bundle ID.
        /// </summary>
        public string BundleId
        {
            get => _bundleId ?? throw new ArgumentNullException(nameof(BundleId));
            set => _bundleId = value;
        }
    }
}