
namespace renamee.Shared.Services
{
    public class JobsFactory
    {
        private readonly IServiceProvider serviceProvider;

        public JobsFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public TJob Get<TJob>()
        {
            return (TJob)serviceProvider.GetService(typeof(TJob));
        }
    }
}
