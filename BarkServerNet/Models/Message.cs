using System.ComponentModel.DataAnnotations;

namespace BarkServerNet
{
    public class Message
    {
        [Required]
        public string DeviceKey { get; set; }

        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public string Group { get; set; }
    }
}
