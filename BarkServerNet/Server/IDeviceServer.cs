namespace BarkServerNet;

public interface IDeviceServer
{
    Task<string?> GetDeviceTokenAsync(string deviceKey);

    Task<string> RegisterDeviceAsync(string? key, string deviceToken);
}

