using EventManager.Application.DTOs.Event;
using FluentValidation;

namespace EventManager.API.Validators;

public sealed class ChangeEventStatusRequestValidator : AbstractValidator<ChangeEventStatusRequest>
{
    public ChangeEventStatusRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del evento es requerido.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("El estado del evento no es válido.");
    }
}
