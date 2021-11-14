using System.ComponentModel;

namespace BarkServerNet;

public class Message
{
    public string? Title { get; set; }

    public string? Body { get; set; }

    string? _sound;
    public string? Sound
    {
        get => $"{_sound}.caf";
        set { _sound =  value ?? "default" ; }
    }

    public string? Category { get; set; } = "myNotificationCategory";

    public int? Badge { get; set; }

    public int? IsArchive { get; set; }

    public string? Level { get; set; }

    [DisplayName("group")]
    public string? Group { get; set; }

    [DisplayName("url")]
    public string? Url { get; set; }

    [DisplayName("icon")]
    public string? Icon { get; set; }

    [DisplayName("autoCopy")]
    public string? AutoCopy { get; set; }

    [DisplayName("copy")]
    public string? Copy { get; set; }
}
