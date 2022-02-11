using Microsoft.Extensions.Options;
using renamee.Server.Options;

namespace renamee.Server.Services
{
    public class BackgroundProcessorService : BackgroundService
    {
        private readonly ProcessorOptions processorOptions;
        private readonly IProcessorService processorService;

        public BackgroundProcessorService(IOptions<ProcessorOptions> processorOptions, IProcessorService processorService)
        {
            this.processorOptions = processorOptions.Value;
            this.processorService = processorService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                await processorService.Process();

                await Task.Delay(TimeSpan.FromMinutes(/*processorOptions.IntervalMinutes*/2), stoppingToken);
            }
        }
    }
}
