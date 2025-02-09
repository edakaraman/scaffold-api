
using System.Data;
using FluentValidation;
using ScaffoldDeneme.Models;

namespace ScaffoldDeneme.Validators
{
    public class NationalityValidator : AbstractValidator<Nationality>
    {
        public NationalityValidator()
        {
            RuleFor(x => x.Name)
             .NotEmpty()
             .NotNull()
             .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long.")
             .MaximumLength(50)
            .WithMessage("Name must be at most 50 characters long.");

        }
    }
}
