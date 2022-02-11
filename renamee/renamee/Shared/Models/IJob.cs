namespace renamee.Shared.Models
{
    public interface IJob
    {
        Guid Id { get; }
        public JobOptions Options { get; set; }

        Task Run();
    }
}
