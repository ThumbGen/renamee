using FluentValidation;
using renamee.Shared.Helpers;
using renamee.Shared.Models;

namespace renamee.Shared.Validators
{
    public class JobOptionsValidator : AbstractValidator<JobOptions>
    {
        public JobOptionsValidator()
        {
            RuleFor(options => options)
                .Must(options => !string.IsNullOrEmpty(options.SourceFolder))
                //.Must(options => Exist(options.SourceFolder))
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
            try
            {
                if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(dest))
                {
                    return false;
                }
                var parentUri = new Uri(source.EnsureDirectoryEnding());
                var childUri = new Uri(dest.EnsureDirectoryEnding());
                if (parentUri == childUri || childUri.IsBaseOf(parentUri))
                {
                    return false;
                }
                return true;
            }
            catch { return false; }
        }

        protected static bool Exist(string folder)
        {
            return Directory.Exists(folder);
        }
    }
}
