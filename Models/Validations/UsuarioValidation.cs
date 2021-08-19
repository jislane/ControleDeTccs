using FluentValidation;
using SistemaDeControleDeTCCs.Areas.Identity.Pages.Account;
using SistemaDeControleDeTCCs.Models;
using System;

namespace SistemaDeControleDeTCCs.Models.Validations
{
    public class UserValidator : AbstractValidator<Usuario>
    {
        public UserValidator()
        {
            RuleFor(c => c.Email)
            .EmailAddress().WithMessage("Digite um e-mail válido!");
        }
    }
}
