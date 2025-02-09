
using System.Data;
using FluentValidation;
using ScaffoldDeneme.Models;

namespace ScaffoldDeneme.Validators
{
    public class StudentValidator : AbstractValidator<Student>
    {
        public StudentValidator()
        {

            RuleSet("Personal Informations", () =>
            {
                RuleFor(x => x.Name)
                                   .NotEmpty()
                                   .MinimumLength(3)
                                   .WithMessage("Name is required.");

                RuleFor(x => x.SurName)
                        .NotEmpty()
                        .NotNull()
                        .MaximumLength(30)
                        .WithMessage("Surname is required");
            });


            RuleFor(x => x.BirthDate)
                    .Must(date => DateTime.TryParse(date.ToString(), out _))
                    .WithMessage("Invalid birthdate format.");

        }
    }
}
