using System.Text.Json.Serialization;

namespace BarkServerNet
{
    public class DeviceInfo
    {
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("device_device_token")]
        public string? DeviceToken { get; set; }
    }
}
