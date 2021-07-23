using System;

namespace BarkServerNet
{
    public class MessageHead
    {
        string? _deviceKey;
        public string DeviceKey
        {
            get => _deviceKey ?? throw new InvalidOperationException($"Uninitialized {nameof(DeviceKey)}");
            set => _deviceKey = value;
        }

        public string? Title { get; set; }

        public string? Body { get; set; }

        string? _sound;
        public string? Sound
        {
            get => _sound;
            set { _sound = value + ".caf"; }
        }
    }
}
