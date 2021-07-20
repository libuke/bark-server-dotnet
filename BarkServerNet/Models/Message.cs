using System.ComponentModel.DataAnnotations;

namespace BarkServerNet
{
    public class Message
    {
        [Required]
        public string DeviceKey { get; set; }

        [Required]
        public string Title { get; set; }

        public string Body { get; set; } = "";

        public string Group { get; set; }
    }
}
