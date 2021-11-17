using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarkServerNet;

[Table("t_device")]
public class Device
{
    [Key]
    [Column("c_device_key")]
    public string? DeviceKey { get; set; }

    [Column("c_device_token")]
    public string? DeviceToken { get; set; }
}
