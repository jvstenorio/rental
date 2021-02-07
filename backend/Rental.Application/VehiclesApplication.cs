using AutoMapper;
using Rental.Domain.Applications;
using Rental.Domain.Entities;
using Rental.Domain.Models;
using Rental.Domain.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Application
{
    public class VehiclesApplication : IVehiclesApplication
    {
        private readonly IVehiclesRepository _vehiclesRepository;
        private readonly IMakesRepository _makesRepository;
        private readonly IModelsRepository _modelsRepository;
        private readonly IMapper _mapper;

        public VehiclesApplication(
            IVehiclesRepository vehiclesRepository,
            IMakesRepository makesRepository,
            IModelsRepository modelsRepository,
            IMapper mapper)
        {
            _vehiclesRepository = vehiclesRepository;
            _makesRepository = makesRepository;
            _modelsRepository = modelsRepository;
            _mapper = mapper;
        }

        public async Task<MakeDto> CreateMakeAsync(MakeDto makeDto, CancellationToken cancellationToken)
        {
            var make = _mapper.Map<Make>(makeDto);
            make.Identifier = Make.GetIdentifier(makeDto.Name);
            await _makesRepository.AddAsync(make, cancellationToken);

            return makeDto;
        }

        public async Task<ModelDto> CreateModelAsync(ModelDto modelDto, CancellationToken cancellationToken)
        {
            var model = _mapper.Map<Model>(modelDto);
            model.Identifier = Model.GetIdentifier(modelDto.Name);
            await _modelsRepository.AddAsync(model, cancellationToken);

            return modelDto;
        }

        public async Task<VehicleDto> CreateVehicleAsync(VehicleDto vehicleDto, CancellationToken cancellationToken)
        {
            await ValidateVehicleAsync(vehicleDto,cancellationToken);

            var vehicle = _mapper.Map<Vehicle>(vehicleDto);
            vehicle.Identifier = Vehicle.GetIdentifier(vehicleDto.Plate);
            await _vehiclesRepository.AddAsync(vehicle, cancellationToken);

            return vehicleDto;
        }

        private async Task ValidateVehicleAsync(VehicleDto vehicleDto, CancellationToken cancellationToken)
        {

            var makeTask = _makesRepository.GetByIdentifierAsync(Make.GetIdentifier(vehicleDto.Make), cancellationToken);
            var modelTask = _modelsRepository.GetByIdentifierAsync(Model.GetIdentifier(vehicleDto.Model), cancellationToken);
            var vehicleTask = _vehiclesRepository.GetByIdentifierAsync(Vehicle.GetIdentifier(vehicleDto.Plate), cancellationToken);
            await Task.WhenAll(makeTask, modelTask, vehicleTask);

            if (makeTask.Result == null) 
            {
                throw new ValidationException("Invalid make");
            }
            if (modelTask.Result == null)
            {
                throw new ValidationException("Invalid model");
            }
            if (vehicleTask.Result != null)
            {
                throw new ValidationException("Vehicle already exists");
            }
        }
    }
}
