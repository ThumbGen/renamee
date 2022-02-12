using FluentValidation;
using renamee.Shared.Helpers;
using renamee.Shared.Validators;
using System.Linq;

namespace renamee.Shared.Models
{
    public class Job
    {
        private readonly IValidator<Job> jobValidator = new JobValidator();

        public JobOptions Options { get; } = new JobOptions();

        public Guid JobId { get; internal set; } = Guid.NewGuid();

        public string Name { get; set; } = "unknown";

        public DateTimeOffset LastExecutedOn { get; private set; } = DateTimeOffset.MinValue;

        public Job()
        {
            
        }

        public async Task Run()
        {
            var validationResult = await jobValidator.ValidateAsync(this);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // collect files from SourceFolder, apply the FormatPattern and move them to DestinationFolder

            var entries = Directory
                .GetFileSystemEntries(Options.SourceFolder, "*", SearchOption.AllDirectories)
                .Where(file => MediaInformation.MediaExtensions.Contains(Path.GetExtension(file).ToUpperInvariant()));

            //var root = new DirectoryInfo(Options.SourceFolder);
            //var directories = new[] { root }.Concat(root.GetDirectories("*", SearchOption.AllDirectories));


        }
    }
}
