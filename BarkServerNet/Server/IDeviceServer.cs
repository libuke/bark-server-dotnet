using System.Threading.Tasks;

namespace BarkServerNet
{
    public interface IDeviceServer
    {
        Task<int> AddDevice(Device device);

        Device? GetDevice(string deviceKey);

        Task<Device?> RegisterDevice(string deviceToken);
    }
}
