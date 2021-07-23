using System.ComponentModel.DataAnnotations;

namespace BarkServerNet
{
    #nullable disable
    public class Device
    {
        [Key]
        public string DeviceToken { get; set; }


        public string DeviceKey { get; set; }
    }
}
