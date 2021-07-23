using System;

namespace BarkServerNet
{
    public class Message
    {
        MessageHead? _head;
        public MessageHead Head
        {
            get => _head ?? throw new InvalidOperationException($"Uninitialized {nameof(Head)}");
            set => _head = value;
        }

        public MessageExtra? Extra { get; set; }
    }
}
