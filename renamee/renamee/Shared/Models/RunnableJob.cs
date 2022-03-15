using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using renamee.Shared.Helpers;
using renamee.Shared.Hubs;
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
        private readonly IHubContext<JobsHub, IJobsHub> hubContext;

        public RunnableJob(ILogger<RunnableJob> logger, IReverseGeocoder reverseGeocoder, IHubContext<JobsHub,IJobsHub> hubContext) : base(logger)
        {
            this.reverseGeocoder = reverseGeocoder;
            this.hubContext = hubContext;
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
                await hubContext.Clients.All.JobUpdated(this.ToDto());
                var validationResult = await jobValidator.ValidateAsync(this);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }

                // collect files from SourceFolder, apply the FormatPattern and move them to DestinationFolder

                var files = Directory
                    .GetFileSystemEntries(Options.SourceFolder, "*", SearchOption.AllDirectories)
                    .Where(file => !file.ToUpperInvariant().Contains("@EADIR"))
                    .Where(file => MediaInformation.MediaExtensions.Contains(Path.GetExtension(file).ToUpperInvariant()))
                    .Where(file => new FileInfo(file).LastWriteTimeUtc > this.LastProcessedFileModifiedOn)
                    .ToList(); // important, as enumerating will be affected by the changing 'where' condition

                //TODO introduce parallelism and process more files in parallel
                foreach (var file in files)
                {
                    await ProcessFile(file);
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Processing job " + Name);
            }
            finally
            {
                LastExecutedOn = DateTimeOffset.UtcNow;
                IsRunning = false;
                await hubContext.Clients.All.JobUpdated(this.ToDto());
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

                    switch (ActionType)
                    {
                        default:
                        case JobActionType.Simulate:
                            logger.LogInformation($"Simulation. Source: {filename} Target: {targetPath}");
                            break;
                        case JobActionType.Copy:
                            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                            File.Copy(filePath, targetPath, true);
                            LastProcessedFileModifiedOn = new FileInfo(filePath).LastWriteTime;
                            break;
                        case JobActionType.Move:
                            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                            File.Move(filePath, targetPath, true);
                            LastProcessedFileModifiedOn = new FileInfo(filePath).LastWriteTime;
                            break;
                    }
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
