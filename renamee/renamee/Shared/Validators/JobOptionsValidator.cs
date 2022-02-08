using FluentValidation;
using renamee.Shared.Models;

namespace renamee.Shared.Validators
{
    public class JobOptionsValidator : AbstractValidator<JobOptions>
    {
        public JobOptionsValidator()
        {
            RuleFor(options => options.SourceFolder)
                .NotNull()
                .Must(Exist)
                .WithMessage("Please provide a valid source folder");
            
            RuleFor(options => options.DestinationFolder)
                .NotNull()
                .Must(Exist)
                .WithMessage("Please provide a valid destination folder");

            RuleFor(options => options)
                .Must(options => DestinationShouldNotBeUnderSourceFolder(options.SourceFolder, options.DestinationFolder))
                .WithMessage("The destination folder cannot be under source folder");
            
            RuleFor(options => options.FormatPattern)
                .NotNull()
                .SetValidator(o => new FormatPatternValidator())
                .WithMessage("Please provide a valid format pattern");
        }

        private static bool DestinationShouldNotBeUnderSourceFolder(string source, string dest)
        {
            var parentUri = new Uri(source);
            var childUri = new Uri(dest);
            if (parentUri == childUri || childUri.IsBaseOf(parentUri))
            {
                return false;
            }
            return false;
        }

        protected static bool Exist(string folder)
        {
            return Directory.Exists(folder);
        }
    }
}
