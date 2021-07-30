using System;
using System.Linq;
using System.Threading.Tasks;

namespace BarkServerNet
{
    public class DeviceServer : IDeviceServer
    {
        readonly DeviceDbContext _context;

        public DeviceServer(DeviceDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddDevice(Device device)
        {
            _context.Add(device);
            return await _context.SaveChangesAsync();
        }

        public Device? GetDevice(string deviceKey)
        {
            return _context.Devices!.FirstOrDefault(x => x.DeviceKey == deviceKey);
        }

        public async Task<Device?> RegisterDevice(string deviceToken)
        {
            string deviceKey = Guid.NewGuid().ToString("N");

            var device = new Device { DeviceKey = deviceKey, DeviceToken = deviceToken };
            int count = await AddDevice(device);

            return count > 0 ? device : default;
        }
    }
}
