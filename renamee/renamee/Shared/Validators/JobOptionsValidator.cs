using FluentValidation;
using renamee.Shared.DTOs;

namespace renamee.Shared.Validators
{
    public class JobOptionsValidator : AbstractValidator<JobOptionsDto>
    {
        public JobOptionsValidator()
        {
            RuleFor(options => options)
                .Must(options => !string.IsNullOrEmpty(options.SourceFolder))
                .Must(options => Exist(options.SourceFolder))
                .WithMessage("Please provide a valid source folder")
                .Must(options => !string.IsNullOrEmpty(options.DestinationFolder))
                //.Must(options => Exist(options.DestinationFolder))
                .WithMessage("Please provide a valid destination folder")
                .Must(options => DestinationIsNotUnderSourceFolder(options.SourceFolder, options.DestinationFolder))
                .WithMessage("The destination folder cannot be under source folder");
            
            RuleFor(options => options.FormatPattern)
                .NotNull()
                .SetValidator(o => new FormatPatternValidator())
                .WithMessage("Please provide a valid format pattern");
        }

        private static bool DestinationIsNotUnderSourceFolder(string source, string dest)
        {
            if(string.IsNullOrEmpty(source) || string.IsNullOrEmpty(dest))
            {
                return false;
            }
            var parentUri = new Uri(source);
            var childUri = new Uri(dest);
            if (parentUri == childUri || childUri.IsBaseOf(parentUri))
            {
                return false;
            }
            return true;
        }

        protected static bool Exist(string folder)
        {
            return Directory.Exists(folder);
        }
    }
}
