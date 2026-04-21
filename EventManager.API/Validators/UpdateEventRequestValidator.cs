using EventManager.Application.DTOs.Event;
using FluentValidation;

namespace EventManager.API.Validators;

public sealed class UpdateEventRequestValidator : AbstractValidator<UpdateEventRequest>
{
    public UpdateEventRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del evento es requerido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del evento es requerido.")
            .MaximumLength(200).WithMessage("El nombre no puede superar los 200 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("La descripción no puede superar los 2000 caracteres.");

        RuleFor(x => x.Location)
            .MaximumLength(300).WithMessage("La ubicación no puede superar los 300 caracteres.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("La fecha de inicio es requerida.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.");

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("La capacidad máxima debe ser mayor a cero.");

        RuleFor(x => x.ParkingCapacity)
            .GreaterThanOrEqualTo(0).WithMessage("La capacidad de parqueadero no puede ser negativa.")
            .Must((request, capacity) => !request.HasParking || capacity > 0)
            .When(x => x.ParkingCapacity.HasValue)
            .WithMessage("La capacidad de parqueadero debe ser mayor a cero cuando el parqueadero está habilitado.");

        RuleFor(x => x.ParkingCapacity)
            .Null()
            .When(x => !x.HasParking)
            .WithMessage("No se pueden asignar cupos de parqueadero si el evento no tiene parqueadero habilitado.");
    }
}
