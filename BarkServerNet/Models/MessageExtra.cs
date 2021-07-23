using System.ComponentModel;

namespace BarkServerNet
{
    public class MessageExtra
    {
        [DisplayName("group")]
        public string? Group { get; set; }

        [DisplayName("isArchive")]
        public string? IsArchive { get; set; }

        [DisplayName("url")]
        public string? Url { get; set; }
    }
}
