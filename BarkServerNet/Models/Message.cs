using System;
using DotAPNS;
using System.Reflection;
using System.ComponentModel;

namespace BarkServerNet
{
    public class Message
    {
        string _device = "";
        public string DeviceKey
        {
            get => _device;
            set => _device = value ?? throw new InvalidOperationException($"Uninitialized {nameof(DeviceKey)}");
        }

        string _title = "";
        public string Title
        {
            get => _title;
            set => _title = value ?? throw new InvalidOperationException($"Uninitialized {nameof(Title)}");
        }

        public string? Body { get; set; }

        string? _sound;
        public string? Sound
        {
            get => _sound;
            set { _sound = value + ".caf"; }
        }

        [DisplayName("group")]
        public string? Group { get; set; }

        [DisplayName("isArchive")]
        public string? IsArchive { get; set; }

        [DisplayName("url")]
        public string? Url { get; set; }

        public ApplePush CreatePush(string deviceToken)
        {
            deviceToken = deviceToken ?? throw new ArgumentNullException(nameof(deviceToken));

            var push = new ApplePush(ApplePushType.Alert)
                .AddMutableContent()
                .AddToken(deviceToken)
                .AddAlert(Title, Body ?? "");

            if (!string.IsNullOrWhiteSpace(Sound))
            {
                push.AddSound(Sound);
            }

            foreach (var property in GetType().GetProperties())
            {
                if (property.GetCustomAttribute<DisplayNameAttribute>() is DisplayNameAttribute display
                    && property.GetValue(this) is string value)
                {
                    push.AddCustomProperty(display.DisplayName, value);
                }
            }
            return push;
        }
    }
}
