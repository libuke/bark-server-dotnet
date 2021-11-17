using Microsoft.EntityFrameworkCore;

namespace BarkServerNet;

public class DeviceServer : IDeviceServer
{
    readonly DeviceDbContext _context;

    public DeviceServer(DeviceDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetDeviceTokenAsync(string deviceKey)
    {
        var result = await _context.Devices.FirstOrDefaultAsync(x => x.DeviceKey == deviceKey);
        return result?.DeviceToken;
    }

    public async Task<string> RegisterDeviceAsync(string? key, string deviceToken)
    {
        // If the deviceKey is empty or the corresponding deviceToken cannot be obtained from the database,
        // it is considered as a new device registration
        if (string.IsNullOrWhiteSpace(key) || await GetDeviceTokenAsync(key) == null)
        {
            // Generate a new UUID as the deviceKey when a new device register
            key = Guid.NewGuid().ToString("N");

            var device = new Device { DeviceKey = key, DeviceToken = deviceToken };
            _context.Add(device);
        }
        else
        {
            var device = await _context.Devices.FirstOrDefaultAsync(x => x.DeviceKey == key);
            device!.DeviceToken = deviceToken;
        }
        await _context.SaveChangesAsync();

        return key;
    }
}
