using System.Text.Json.Serialization;

namespace BarkServerNet
{
    public class CommonResponse
    {
        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("timestamp")]
        public long? Timestamp { get; set; }

        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DeviceInfo? DeviceInfo { get; set; }
    }

    public class DeviceInfo
    {
        [JsonPropertyName("key")]
        public string? DeviceKey { get; set; }

        [JsonPropertyName("device_device_token")]
        public string? DeviceToken { get; set; }
    }
}
