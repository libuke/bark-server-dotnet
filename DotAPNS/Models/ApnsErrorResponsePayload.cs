using System;
using System.Text.Json.Serialization;

namespace DotAPNS
{
    public class ApnsErrorResponsePayload
    {
        [JsonIgnore]
        public ApnsResponseReason Reason =>
            Enum.TryParse<ApnsResponseReason>(ReasonRaw, out var value)
            ? value : ApnsResponseReason.Unknown;

        [JsonPropertyName("reason")]
        public string? ReasonRaw { get; set; }

        [JsonConverter(typeof(UnixTimestampMillisecondsJsonConverter))] // timestamp is in milliseconds (https://openradar.appspot.com/24548417)
        public DateTimeOffset? Timestamp { get; set; }
    }
}