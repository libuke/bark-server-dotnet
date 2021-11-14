namespace BarkServerNet;

public class DeviceServer : IDeviceServer
{
    readonly DeviceDbContext _context;

    public DeviceServer(DeviceDbContext context)
    {
        _context = context;
    }

    public string? GetDeviceToken(string deviceKey)
    {
        return _context.Devices!.FirstOrDefault(x => x.DeviceKey == deviceKey)?.DeviceToken;
    }

    public async Task<string?> RegisterDeviceAsync(string deviceToken)
    {
        string deviceKey = Guid.NewGuid().ToString("N");

        var device = new Device { DeviceKey = deviceKey, DeviceToken = deviceToken };
        _context.Add(device);

        return await _context.SaveChangesAsync() > 0 ? deviceKey : null;
    }
}
