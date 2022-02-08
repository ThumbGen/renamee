namespace renamee.Shared.Models
{
    public interface IJob
    {
        public JobOptions Options { get; set; }

        Task Run();
    }
}
