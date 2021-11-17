using System.Text.Json.Serialization;

namespace DotAPNS.Models;

public class ApplePushLocalizedAlert
{
    #region Property

    [JsonPropertyName("action")]
    public string? Action { get; private set; }

    [JsonPropertyName("action-loc-key")]
    public string? ActionLocKey { get; private set; }

    [JsonPropertyName("body")]
    public string? Body { get; private set; }

    [JsonPropertyName("launch-image")]
    public string? LaunchImage { get; private set; }

    [JsonPropertyName("loc-args")]
    public string[]? LocArgs { get; private set; }

    [JsonPropertyName("loc-key")]
    public string? LocKey { get; private set; }

    [JsonPropertyName("title")]
    public string? Title { get; private set; }

    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; private set; }

    [JsonPropertyName("title-loc-args")]
    public string[]? TitleLocArgs { get; private set; }

    [JsonPropertyName("title-loc-key")]
    public string? TitleLocKey { get; private set; }

    [JsonPropertyName("summary-arg")]
    public string? SummaryArg { get; private set; }

    [JsonPropertyName("summary-arg-count")]
    public int? SummaryArgCount { get; private set; }

    #endregion

    #region Alert dictionary

    // AlertAction sets the aps alert action on the payload.
    // This is the label of the action button, if the user sets the notifications
    // to appear as alerts. This label should be succinct, such as “Details” or
    // “Read more”. If omitted, the default value is “Show”.
    //
    //	{"aps":{"alert":{"action":action}}}
    public ApplePushLocalizedAlert AlertAction(string? action)
    {
        Action = action;
        return this;
    }

    // AlertActionLocKey sets the aps alert action localization key on the payload.
    // This is the the string used as a key to get a localized string in the current
    // localization to use for the notfication right button’s title instead of
    // “View”. See Localized Formatted Strings in Apple documentation for more
    // information.
    //
    //	{"aps":{"alert":{"action-loc-key":key}}}
    public ApplePushLocalizedAlert AlertActionLocKey(string? key)
    {
        ActionLocKey = key;
        return this;
    }

    // AlertBody sets the aps alert body on the payload.
    // This is the text of the alert message.
    //
    //	{"aps":{"alert":{"body":body}}}
    public ApplePushLocalizedAlert AlertBody(string? body)
    {
        Body = body;
        return this;
    }

    // AlertLaunchImage sets the aps launch image on the payload.
    // This is the filename of an image file in the app bundle. The image is used
    // as the launch image when users tap the action button or move the action
    // slider.
    //
    //	{"aps":{"alert":{"launch-image":image}}}
    public ApplePushLocalizedAlert AlertLaunchImage(string? image)
    {
        LaunchImage = image;
        return this;
    }

    // AlertLocArgs sets the aps alert localization args on the payload.
    // These are the variable string values to appear in place of the format
    // specifiers in loc-key. See Localized Formatted Strings in Apple
    // documentation for more information.
    //
    //  {"aps":{"alert":{"loc-args":args}}}
    public ApplePushLocalizedAlert AlertLocArgs(string[]? args)
    {
        LocArgs = args;
        return this;
    }

    // AlertLocKey sets the aps alert localization key on the payload.
    // This is the key to an alert-message string in the Localizable.strings file
    // for the current localization. See Localized Formatted Strings in Apple
    // documentation for more information.
    //
    //	{"aps":{"alert":{"loc-key":key}}}
    public ApplePushLocalizedAlert AlertLocKey(string? key)
    {
        LocKey = key;
        return this;
    }

    // AlertTitle sets the aps alert title on the payload.
    // This will display a short string describing the purpose of the notification.
    // Apple Watch & Safari display this string as part of the notification interface.
    //
    //	{"aps":{"alert":{"title":title}}}
    public ApplePushLocalizedAlert AlertTitle(string? title)
    {
        Title = title;
        return this;
    }

    // AlertSubtitle sets the aps alert subtitle on the payload.
    // This will display a short string describing the purpose of the notification.
    // Apple Watch & Safari display this string as part of the notification interface.
    //
    //	{"aps":{"alert":{"subtitle":"subtitle"}}}
    public ApplePushLocalizedAlert AlertSubtitle(string? subtitle)
    {
        Subtitle = subtitle;
        return this;
    }

    // AlertTitleLocArgs sets the aps alert title localization args on the payload.
    // These are the variable string values to appear in place of the format
    // specifiers in title-loc-key. See Localized Formatted Strings in Apple
    // documentation for more information.
    //
    //	{"aps":{"alert":{"title-loc-args":args}}}
    public ApplePushLocalizedAlert AlertTitleLocArgs(string[]? args)
    {
        TitleLocArgs = args;
        return this;
    }

    // AlertTitleLocKey sets the aps alert title localization key on the payload.
    // This is the key to a title string in the Localizable.strings file for the
    // current localization. See Localized Formatted Strings in Apple documentation
    // for more information.
    //
    //	{"aps":{"alert":{"title-loc-key":key}}}
    public ApplePushLocalizedAlert AlertTitleLocKey(string? key)
    {
        TitleLocKey = key;
        return this;
    }

    // AlertSummaryArg sets the aps alert summary arg key on the payload.
    // This is the string that is used as a key to fill in an argument
    // at the bottom of a notification to provide more context, such as
    // a name associated with the sender of the notification.
    //
    //	{"aps":{"alert":{"summary-arg":key}}}
    public ApplePushLocalizedAlert AlertSummaryArg(string? key)
    {
        SummaryArg = key;
        return this;
    }

    // AlertSummaryArgCount sets the aps alert summary arg count key on the payload.
    // This integer sets a custom "weight" on the notification, effectively
    // allowing a notification to be viewed internally as two. For example if
    // a notification encompasses 3 messages, you can set it to 3.
    //
    //	{"aps":{"alert":{"summary-arg-count":key}}}
    public ApplePushLocalizedAlert AlertSummaryArgCount(int? key)
    {
        SummaryArgCount = key;
        return this;
    }

    #endregion
}
