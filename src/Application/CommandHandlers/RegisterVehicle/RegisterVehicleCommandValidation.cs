using FluentValidation;

namespace Application.CommandHandlers.RegisterVehicle;

public class RegisterVehicleCommandValidation : AbstractValidator<RegisterVehicleCommand>
{
    public RegisterVehicleCommandValidation()
    {
        RuleFor(command => command.CarName)
            .NotEmpty().WithMessage("Car name is required.")
            .MaximumLength(100).WithMessage("Car name cannot exceed 100 characters.");

        RuleFor(command => command.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .MaximumLength(50).WithMessage("Brand cannot exceed 50 characters.");

        RuleFor(command => command.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(50).WithMessage("Model cannot exceed 50 characters.");

        RuleFor(command => command.Year)
            .InclusiveBetween(1886, DateTime.Now.Year).WithMessage($"Year must be between 1886 and {DateTime.Now.Year}.");

        RuleFor(command => command.Color)
            .NotEmpty().WithMessage("Color is required.")
            .MaximumLength(30).WithMessage("Color cannot exceed 30 characters.");

        RuleFor(command => command.FuelType)
            .NotEmpty().WithMessage("Fuel type is required.")
            .MaximumLength(30).WithMessage("Fuel type cannot exceed 30 characters.");

        RuleFor(command => command.NumberOfDoors)
            .InclusiveBetween(2, 6).WithMessage("Number of doors must be between 2 and 6.");

        RuleFor(command => command.Mileage)
            .GreaterThanOrEqualTo(0).WithMessage("Mileage must be greater than or equal to 0.");

        RuleFor(command => command.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");
    }

}