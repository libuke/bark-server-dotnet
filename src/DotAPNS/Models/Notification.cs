namespace DotAPNS.Models;

public enum Priority
{
    Low = 5,
    High = 10
}

public class Notification
{
    #region Field

    int? _priority;

    #endregion

    #region Property

    // An optional canonical UUID that identifies the notification. The
    // canonical form is 32 lowercase hexadecimal digits, displayed in five
    // groups separated by hyphens in the form 8-4-4-4-12. An example UUID is as
    // follows:
    //
    //  123e4567-e89b-12d3-a456-42665544000
    //
    // If you don't set this, a new UUID is created by APNs and returned in the
    // response.
    public string? ApnsID { get; set; }

    // A string which allows multiple notifications with the same collapse
    // identifier to be displayed to the user as a single notification. The
    // value should not exceed 64 bytes.
    public string? CollapseID { get; set; }

    // A string containing hexadecimal bytes of the device token for the target
    // device.
    public string? DeviceToken { get; }

    // The topic of the remote notification, which is typically the bundle ID
    // for your app. The certificate you create in the Apple Developer Member
    // Center must include the capability for this topic. If your certificate
    // includes multiple topics, you must specify a value for this header. If
    // you omit this header and your APNs certificate does not specify multiple
    // topics, the APNs server uses the certificate’s Subject as the default
    // topic.
    public string? Topic { get; }

    // An optional time at which the notification is no longer valid and can be
    // discarded by APNs. If this value is in the past, APNs treats the
    // notification as if it expires immediately and does not store the
    // notification or attempt to redeliver it. If this value is left as the
    // default (ie, Expiration.IsZero()) an expiration header will not added to
    // the http request.
    public DateTimeOffset? Expiration { get; set; }

    // The priority of the notification. Specify ether Priority.High (10) or
    // Priority.Low (5) If you don't set this, the APNs server will set the
    // priority to 10.
    public int Priority
    {
        get => _priority ?? (PushType == ApplePushType.Background ? 5 : 10);
        set
        {
            if (value < 0 || value > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(Priority), Priority, "Priority must be between 0 and 10.");
            }
            _priority = value;
        }
    }

    // The pushtype of the push notification. If this values is left as the
    // default an apns-push-type header with value 'alert' will be added to the
    // http request.
    public ApplePushType PushType { get; }

    #endregion

    #region Constructor

    public Notification(string deviceToken, string topic)
    {
        DeviceToken = deviceToken;
        Topic = topic;
    }

    #endregion
}
