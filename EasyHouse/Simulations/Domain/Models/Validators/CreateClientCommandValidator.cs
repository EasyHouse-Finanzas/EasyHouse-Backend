using EasyHouse.Simulations.Domain.Models.Comands;
using FluentValidation;

namespace EasyHouse.Simulations.Domain.Models.Validators;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DocumentNumber).NotEmpty().Length(8).WithMessage("DNI debe tener 8 caracteres.");
        RuleFor(x => x.BirthDate).NotEmpty();
        RuleFor(x => x.MonthlyIncome).GreaterThanOrEqualTo(0);

    }
}