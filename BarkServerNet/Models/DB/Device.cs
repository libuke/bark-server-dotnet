using System.ComponentModel.DataAnnotations;

namespace BarkServerNet
{
    public class Device
    {
        [Key]
        public string DeviceToken { get; set; }


        public string DeviceKey { get; set; }
    }
}
