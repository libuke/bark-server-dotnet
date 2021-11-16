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
        return _context.Devices.FirstOrDefault(x => x.DeviceKey == deviceKey)?.DeviceToken;
    }

    public async Task<string> RegisterDeviceAsync(string? key, string deviceToken)
    {
        // If the deviceKey is empty or the corresponding deviceToken cannot be obtained from the database,
        // it is considered as a new device registration
        if (string.IsNullOrWhiteSpace(key) || GetDeviceToken(key) == null)
        {
            // Generate a new UUID as the deviceKey when a new device register
            key = Guid.NewGuid().ToString("N");

            var device = new Device { DeviceKey = key, DeviceToken = deviceToken };
            _context.Add(device);
        }
        else
        {
            var device = _context.Devices.FirstOrDefault(x => x.DeviceKey == key);
            device!.DeviceToken = deviceToken;
        }
        await _context.SaveChangesAsync();

        return key;
    }
}
