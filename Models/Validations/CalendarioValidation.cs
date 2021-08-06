using FluentValidation;
using SistemaDeControleDeTCCs.Models;
using System;

namespace SistemaDeControleDeTCCs.Models.Validations
{
    public class CalendarioValidator : AbstractValidator<Calendario>
    {
        public CalendarioValidator()
        {
  
            RuleFor(c => c.DataInicio)
            .NotEmpty().WithMessage("Data é obrigatório!");

            RuleFor(c => c.DataFim)
            .NotEmpty().WithMessage("Data é obrigatório!")
            .GreaterThan(m => m.DataInicio).WithMessage("Data final precisa ser mainor que a data inicial");

        }
    }
}
