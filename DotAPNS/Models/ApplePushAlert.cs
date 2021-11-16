using DotAPNS.Responses;
using System.Text.Json.Serialization;

namespace DotAPNS.Models;

public enum EInterruptionLevel
{
    Active,
    Passive,
    Critical,
    TimeSensitive
}

//// A byte array containing the JSON-encoded payload of this push notification.
//// Refer to "The Remote Notification Payload" section in the Apple Local and
//// Remote Notification Programming Guide for more info.
public class Payload
{
    #region Property

    [JsonPropertyName("alert")]
    public ApplePushLocalizedAlert? Alert { get; private set; }

    [JsonPropertyName("badge")]
    public int? Badge { get; private set; }

    [JsonPropertyName("category")]
    public string? Category { get; private set; }

    [JsonPropertyName("content-available")]
    public int? ContentAvailable { get; private set; }

    [JsonPropertyName("interruption-level")]
    public string? InterruptionLevel { get; private set; }

    [JsonPropertyName("mutable-content")]
    public int? MutableContent { get; private set; }

    [JsonPropertyName("relevance-score")]
    public float? RelevanceScore { get; private set; }

    [JsonPropertyName("sound")]
    public string? Sound { get; private set; }

    [JsonPropertyName("thread-id")]
    public string? ThreadID { get; private set; }

    [JsonPropertyName("url-args")]
    public string[]? URLArgs { get; private set; }

    [JsonIgnore]
    public Dictionary<string, object> Content { get; private set; } = new Dictionary<string, object>();

    #endregion

    #region Alert dictionary

    // Alert sets the aps alert on the payload.
    // This will display a notification alert message to the user.
    //
    //	{"aps":{"alert":alert}}`
    public Payload AddAlert(ApplePushLocalizedAlert alert)
    {
        Alert = alert;
        return this;
    }

    // Badge sets the aps badge on the payload.
    // This will display a numeric badge on the app icon.
    //
    //	{"aps":{"badge":b}}
    public Payload AddBadge(int? b)
    {
        Badge = b;
        return this;
    }

    // ZeroBadge sets the aps badge on the payload to 0.
    // This will clear the badge on the app icon.
    //
    //	{"aps":{"badge":0}}
    public Payload ZeroBadge()
    {
        Badge = 0;
        return this;
    }

    // UnsetBadge removes the badge attribute from the payload.
    // This will leave the badge on the app icon unchanged.
    // If you wish to clear the app icon badge, use ZeroBadge() instead.
    //
    //	{"aps":{}}
    public Payload UnsetBadge()
    {
        Badge = null;
        return this;
    }

    // Category sets the aps category on the payload.
    // This is a string value that represents the identifier property of the
    // UIMutableUserNotificationCategory object you created to define custom actions.
    //
    //	{"aps":{"category":category}}
    public Payload AddCategory(string? category)
    {
        Category = category;
        return this;
    }

    // ContentAvailable sets the aps content-available on the payload to 1.
    // This will indicate to the app that there is new content available to download
    // and launch the app in the background.
    //
    //	{"aps":{"content-available":1}}
    public Payload AddContentAvailable()
    {
        ContentAvailable = 1;
        return this;
    }

    // InterruptionLevel defines the value for the payload aps interruption-level
    // This is to indicate the importance and delivery timing of a notification.
    // (Using InterruptionLevelCritical requires an approved entitlement from Apple.)
    // See: https://developer.apple.com/documentation/usernotifications/unnotificationinterruptionlevel/
    //
    // {"aps":{"interruption-level":passive}}
    public Payload AddInterruptionLevel(EInterruptionLevel level)
    {
        InterruptionLevel = level switch
        {
            EInterruptionLevel.Passive => "passive",
            EInterruptionLevel.Active => "active",
            EInterruptionLevel.TimeSensitive => "time-sensitive",
            EInterruptionLevel.Critical => "critical",
            _ => throw new NotImplementedException(),
        };
        return this;
    }

    public Payload AddInterruptionLevel(string? levelStr)
    {
        var level = levelStr?.ToLower() switch
        {
            "passive" => EInterruptionLevel.Passive,
            "critical" => EInterruptionLevel.Critical,
            "timesensitive" => EInterruptionLevel.TimeSensitive,
            _ => EInterruptionLevel.Active
        };

        return AddInterruptionLevel(level);
    }

    // MutableContent sets the aps mutable-content on the payload to 1.
    // This will indicate to the to the system to call your Notification Service
    // extension to mutate or replace the notification's content.
    //
    //	{"aps":{"mutable-content":1}}
    public Payload AddMutableContent()
    {
        MutableContent = 1;
        return this;
    }

    // The relevance score, a number between 0 and 1,
    // that the system uses to sort the notifications from your app.
    // The highest score gets featured in the notification summary.
    // See https://developer.apple.com/documentation/usernotifications/unnotificationcontent/3821031-relevancescore.
    //
    //	{"aps":{"relevance-score":0.1}}
    public Payload AddRelevanceScore(float? b)
    {
        RelevanceScore = b;
        return this;
    }

    // Unsets the relevance score
    // that the system uses to sort the notifications from your app.
    // The highest score gets featured in the notification summary.
    // See https://developer.apple.com/documentation/usernotifications/unnotificationcontent/3821031-relevancescore.
    //
    //	{"aps":{"relevance-score":0.1}}
    public Payload UnsetRelevanceScore()
    {
        RelevanceScore = null;
        return this;
    }

    // Sound sets the aps sound on the payload.
    // This will play a sound from the app bundle, or the default sound otherwise.
    //
    //	{"aps":{"sound":sound}}
    public Payload AddSound(string? sound)
    {
        Sound = sound;
        return this;
    }

    // ThreadID sets the aps thread id on the payload.
    // This is for the purpose of updating the contents of a View Controller in a
    // Notification Content app extension when a new notification arrives. If a
    // new notification arrives whose thread-id value matches the thread-id of the
    // notification already being displayed, the didReceiveNotification method
    // is called.
    //
    //	{"aps":{"thread-id":id}}
    public Payload AddThreadID(string? threadID)
    {
        ThreadID = threadID;
        return this;
    }

    // URLArgs sets the aps category on the payload.
    // This specifies an array of values that are paired with the placeholders
    // inside the urlFormatString value of your website.json file.
    // See Apple Notification Programming Guide for Websites.
    //
    //	{"aps":{"url-args":urlArgs}}
    public Payload AddURLArgs(string[]? urlArgs)
    {
        URLArgs = urlArgs;
        return this;
    }

    // Custom payload

    // Custom sets a custom key and value on the payload.
    // This will add custom key/value data to the notification payload at root level.
    //
    //	{"aps":{}, key:value}
    public Payload Custom(string key, object val)
    {
        Content[key] = val;
        return this;
    }

    // Mdm sets the mdm on the payload.
    // This is for Apple Mobile Device Management (mdm) payloads.
    //
    //	{"aps":{}:"mdm":mdm}
    public Payload Mdm(string? mdm)
    {
        Content["mdm"] = mdm!;
        return this;
    }

    #endregion

    #region Method

    // MarshalJSON converts the notification payload to JSON.
    public PayloadJsonContent MarshalJsonContent()
    {
        var payloadDic = new Dictionary<string, object>
        {
            ["aps"] = this
        };

        if (Content != null)
        {
            foreach (var custom in Content)
            {
                if (custom.Value != null)
                {
                    payloadDic[custom.Key] = custom.Value;
                }
            }
        }

        return new PayloadJsonContent(payloadDic);
    }

    #endregion
}
