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

        public async Task<int> UpdateDevice(Device device)
        {
            _context.Devices!.Update(device);
            return await _context.SaveChangesAsync();
        }

        public async Task<string?> RegisterDevice(string deviceToken)
        {
            int count;
            string deviceKey = Guid.NewGuid().ToString("N");
            var device = _context.Devices!.FirstOrDefault(x => x.DeviceToken == deviceToken);
            if (device != null)
            {
                device.DeviceKey = deviceKey;
                count = await UpdateDevice(device);
            }
            else
            {
                count = await AddDevice(new() { DeviceKey = deviceKey, DeviceToken = deviceToken });
            }

            if (count > 0)
            {
                return deviceKey;
            }
            else
            {
                return default;
            }
        }
    }
}
