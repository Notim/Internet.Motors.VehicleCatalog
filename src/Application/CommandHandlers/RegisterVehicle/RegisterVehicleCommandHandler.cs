using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using FluentValidation;
using MediatR;
using Notim.Outputs;
using Notim.Outputs.FluentValidation;

namespace Application.CommandHandlers.RegisterVehicle
{
    public class RegisterVehicleCommandHandler : IRequestHandler<RegisterVehicleCommand, Output>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IValidator<RegisterVehicleCommand> _validator;

        public RegisterVehicleCommandHandler(
            IVehicleRepository vehicleRepository,
            IValidator<RegisterVehicleCommand> validator
        )
        {
            _vehicleRepository = vehicleRepository;
            _validator = validator;
        }

        public async Task<Output> Handle(RegisterVehicleCommand request, CancellationToken cancellationToken)
        {
            var output = new Output();

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (validationResult.IsValid is false)
            {
                output.AddValidationResult(validationResult);
                return output;
            }

            var vehicle = request.MapToDomainVehicle();

            var vehicleId = await _vehicleRepository.InsertVehicleAsync(vehicle);

            output.AddMessage($"vehicle added with id: {vehicleId}");

            return output;
        }

    }
}