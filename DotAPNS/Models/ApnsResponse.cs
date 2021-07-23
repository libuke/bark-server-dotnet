using System.Text.Json.Serialization;

namespace DotAPNS
{
    public class ApnsResponse
    {
        public ApnsResponseReason? Reason { get; }
        public string? ReasonString { get; }
        public bool IsSuccessful { get; }

        [JsonConstructor]
        public ApnsResponse(ApnsResponseReason? reason, string? reasonString, bool isSuccessful)
        {
            Reason = reason;
            ReasonString = reasonString;
            IsSuccessful = isSuccessful;
        }

        public static ApnsResponse Successful() => new(ApnsResponseReason.Success, null, true);

        public static ApnsResponse Error(ApnsResponseReason? reason, string? reasonString) => new(reason, reasonString, false);
    }
}