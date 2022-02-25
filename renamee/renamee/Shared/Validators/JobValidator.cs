using FluentValidation;
using renamee.Shared.Models;

namespace renamee.Shared.Validators
{
    public class JobValidator : AbstractValidator<Job>
    {
        public JobValidator()
        {
            RuleFor(job => job.Name)
                .NotEmpty()
                .WithMessage("Please provide a valid job name");

            RuleFor(job => job.Options).SetValidator(new JobOptionsValidator());
        }
    }
}
