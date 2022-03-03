using FluentValidation;
using Microsoft.Extensions.Logging;
using renamee.Shared.Helpers;
using renamee.Shared.Interfaces;
using renamee.Shared.Validators;

namespace renamee.Shared.Models
{
    public interface IRunnableJob : IJob
    {
        Task Run();
    }

    public class RunnableJob : Job, IRunnableJob
    {
        private readonly IValidator<Job> jobValidator = new JobValidator();
        private readonly IReverseGeocoder reverseGeocoder;

        public RunnableJob(ILogger<RunnableJob> logger, IReverseGeocoder reverseGeocoder) : base(logger)
        {
            this.reverseGeocoder = reverseGeocoder;
        }

        public async Task Run()
        {
            if (IsRunning)
            {
                logger.LogWarning($"Job '{Name}' already running.");
                return;
            }

            IsRunning = true;
            try
            {
                var validationResult = await jobValidator.ValidateAsync(this);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }

                // collect files from SourceFolder, apply the FormatPattern and move them to DestinationFolder

                var files = Directory
                    .GetFileSystemEntries(Options.SourceFolder, "*", SearchOption.AllDirectories)
                    .Where(file => MediaInformation.MediaExtensions.Contains(Path.GetExtension(file).ToUpperInvariant()))
                    .Where(file => new FileInfo(file).LastWriteTimeUtc > this.LastProcessedFileModifiedOn)
                    .ToList(); // important, as enumerating will be affected by the changing 'where' condition

                //TODO introduce parallelism and process more files in parallel
                foreach (var file in files)
                {
                    await ProcessFile(file);
                }
            }
            finally
            {
                IsRunning = false;
            }
        }

        private async Task<GeocodingData> ReverseGeocode(string filePath)
        {
            if (reverseGeocoder == null) return new GeocodingData(string.Empty, string.Empty);

            var (latitude, longitude) = GPSHelper.GetCoordinates(filePath);

            return await reverseGeocoder.Resolve(latitude, longitude);
        }

        private async Task ProcessFile(string filePath)
        {
            try
            {
                var date = FileDateHelper.GetFileDate(filePath);
                var filename = Path.GetFileName(filePath);
                var extension = Path.GetExtension(filePath);

                // perform reverse geocoding only if the pattern contains City and/or Country
                var geocodingData = (Options.FormatPattern.Contains(FormatParser.City) || Options.FormatPattern.Contains(FormatParser.Country)) ? await ReverseGeocode(filePath) : null;

                // if the date is valid and can be parsed correctly...
                if (date.HasValue && !date.Value.Equals(default) && FormatParser.TryParse(date.Value, this.Options.FormatPattern, filename, out string finalSegments, geocodingData))
                {
                    var coercedFinalSegments = finalSegments.Replace('|', '/');
                    logger.LogDebug($"Processing '{filename}' into '{coercedFinalSegments + extension}'");

                    var targetPath = Path.Combine(Options.DestinationFolder, coercedFinalSegments + extension);

                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));

                    switch (ActionType)
                    {
                        default:
                        case JobActionType.Simulate:
                            logger.LogInformation($"Simulation. Source: {filename} Target: {targetPath}");
                            break;
                        case JobActionType.Copy:
                            File.Copy(filePath, targetPath, true);
                            LastProcessedFileModifiedOn = new FileInfo(filePath).LastWriteTime;
                            break;
                        case JobActionType.Move:
                            File.Move(filePath, targetPath, true);
                            LastProcessedFileModifiedOn = new FileInfo(filePath).LastWriteTime;
                            break;
                    }

                    LastExecutedOn = DateTimeOffset.UtcNow;
                }
                else
                {
                    logger.LogError($"No date found. Could not process {filePath}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Can't process {filePath}");
            }
        }
    }
}
