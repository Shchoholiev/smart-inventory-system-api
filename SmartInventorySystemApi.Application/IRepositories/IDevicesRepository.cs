using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.IRepositories;

public interface IDevicesRepository : IBaseRepository<Device>
{
    Task<Device> UpdateAsync(Device device, CancellationToken cancellationToken);
}
