
using FluentValidation;
using renamee.Shared.Helpers;
using renamee.Shared.Validators;
using System.Linq;

namespace renamee.Shared.Models
{
    public class Job : IJob
    {
        private readonly JobOptionsValidator jobOptionsValidator;

        public JobOptions Options { get; set; } = new JobOptions();

        public Guid Id { get; internal set; } = Guid.NewGuid();

        public Job(JobOptionsValidator jobOptionsValidator)
        {
            this.jobOptionsValidator = jobOptionsValidator;
        }


        public async Task Run()
        {
            if (Options == null) throw new ArgumentNullException(nameof(Options));
            var validationResult = await jobOptionsValidator.ValidateAsync(Options);
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
