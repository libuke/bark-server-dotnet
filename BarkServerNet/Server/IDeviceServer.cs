namespace BarkServerNet;

public interface IDeviceServer
{
    string? GetDeviceToken(string deviceKey);

    Task<string?> RegisterDeviceAsync(string deviceToken);
}

