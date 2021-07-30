using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BarkServerNet
{
    public class Device
    {
        [Key]
        public string? DeviceKey { get; set; }

        public string? DeviceToken { get; set; }
    }
}
