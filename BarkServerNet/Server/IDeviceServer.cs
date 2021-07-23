using System.Threading.Tasks;

namespace BarkServerNet
{
    public interface IDeviceServer
    {
        Task<int> AddDevice(Device device);

        Device? GetDevice(string deviceToken);

        Task<int> UpdateDevice(Device device);

        Task<string?> RegisterDevice(string deviceToken);
    }
}
