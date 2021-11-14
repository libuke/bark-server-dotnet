using DotAPNS;
using DotAPNS.Models;
using DotAPNS.Responses;
using System.ComponentModel;
using System.Reflection;

namespace BarkServerNet.Apns;

public static class ApnsExtensions
{
    public static async Task<ApnsResponse> PushAlertAsync(this IApnsClient apnsClient, string deviceToken, Message message)
    {
        if (string.IsNullOrWhiteSpace(deviceToken))
        {
            throw new ArgumentNullException(nameof(deviceToken), $"Make sure {nameof(deviceToken)} is set to a non-null value.");
        }

        var notification = new Notification(deviceToken, ApplePushType.Alert)
        {
            Expiration = DateTimeOffset.UtcNow.AddDays(1)
        };

        #region Alert

        var alert = new ApplePushLocalizedAlert();

        alert.AlertBody(message.Body!);
        alert.AlertTitle(message.Title);

        #endregion

        #region Payload

        var payload = new Payload();

        payload.AddAlert(alert);
        payload.AddSound(message.Sound);
        payload.AddThreadID(message.Group!);

        payload.AddCategory(message.Category!);
        payload.AddInterruptionLevel(message.Level);

        if ((message.IsArchive ?? 1) == 1)
        {
            payload.AddMutableContent();
        }

        foreach (var property in message.GetType().GetProperties())
        {
            if (property.GetCustomAttribute<DisplayNameAttribute>() is DisplayNameAttribute display)
            {
                payload.Custom(display.DisplayName, property.GetValue(message)!);
            }
        }

        #endregion

        return await apnsClient.SendAsync(notification, payload);
    }
}
