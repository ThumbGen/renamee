using Microsoft.AspNetCore.SignalR;
using renamee.Shared.Models;

namespace renamee.Shared.Hubs
{
    public interface IJobsHub
    {
        Task JobUpdated(JobDto job);
    }

    public class JobsHub : Hub<IJobsHub>
    {

    }
}
