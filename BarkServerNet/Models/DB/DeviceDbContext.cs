using System;
using Microsoft.EntityFrameworkCore;
namespace BarkServerNet
{
    public class DeviceDbContext : DbContext
    {
        public DeviceDbContext(DbContextOptions<DeviceDbContext> options)
            : base(options)
        {
        }

        DbSet<Device>? _devices;
        public DbSet<Device> Devices
        {
            get => _devices ?? throw new InvalidOperationException("Uninitialized " + nameof(Devices));
            set => _devices = value;
        }
    }
}
