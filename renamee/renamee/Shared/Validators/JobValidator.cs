using FluentValidation;
using renamee.Shared.DTOs;

namespace renamee.Shared.Validators
{
    public class JobValidator : AbstractValidator<JobDto>
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
