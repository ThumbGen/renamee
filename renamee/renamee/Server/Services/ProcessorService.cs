namespace renamee.Server.Services
{
    public interface IProcessorService
    {
        Task Process();
    }

    public class ProcessorService : IProcessorService
    {
        public async Task Process()
        {
            Console.WriteLine("Processing");
        }
    }
}
