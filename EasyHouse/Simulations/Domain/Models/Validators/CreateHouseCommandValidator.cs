using EasyHouse.Simulations.Domain.Models.Comands;
using FluentValidation;

namespace EasyHouse.Simulations.Domain.Models.Validators;

public class CreateHouseCommandValidator : AbstractValidator<CreateHouseCommand>
{
    public CreateHouseCommandValidator()
    {
        RuleFor(x => x.Proyecto).NotEmpty().MaximumLength(150);
        RuleFor(x => x.CodigoInmueble).NotEmpty().MaximumLength(50);
        RuleFor(x => x.AreaTotal).GreaterThan(0);
        RuleFor(x => x.Ubicacion).NotEmpty();
        RuleFor(x => x.Precio).GreaterThan(0);
    }
}