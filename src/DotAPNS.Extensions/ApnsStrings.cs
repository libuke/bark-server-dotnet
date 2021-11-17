namespace DotAPNS.Extensions;

public class ApnsStrings
{
    string _keyID = string.Empty;
    string _teamID = string.Empty;
    string _topic = string.Empty;
    string _filePath = string.Empty;

    public string KeyID { get => _keyID; set => _keyID = value ?? throw new ArgumentNullException(nameof(KeyID)); }

    public string TeamID { get => _teamID; set => _teamID = value ?? throw new ArgumentNullException(nameof(TeamID)); }

    public string Topic { get => _topic; set => _topic = value ?? throw new ArgumentNullException(nameof(Topic)); }

    public string FilePath { get => _filePath; set => _filePath = value ?? throw new ArgumentNullException(nameof(FilePath)); }
}
