using EventManager.Application.DTOs.Registration;
using FluentValidation;

namespace EventManager.API.Validators;

public sealed class RegisterToEventRequestValidator : AbstractValidator<RegisterToEventRequest>
{
    public RegisterToEventRequestValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("El identificador del evento es requerido.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("El identificador del usuario es requerido.");
    }
}
