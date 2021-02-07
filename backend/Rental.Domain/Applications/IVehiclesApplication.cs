using Rental.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Domain.Applications
{
    public interface IVehiclesApplication
    {
        Task<VehicleDto> CreateVehicleAsync(VehicleDto vehicleDto, CancellationToken cancellationToken);
        Task<MakeDto> CreateMakeAsync(MakeDto makeDto, CancellationToken cancellationToken);
        Task<ModelDto> CreateModelAsync(ModelDto modelDto, CancellationToken cancellationToken);
        Task<VehicleDto> GetVehicleAsync(string plate, CancellationToken cancellationToken);
    }
}
