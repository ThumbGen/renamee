using Microsoft.AspNetCore.Components;
using MudBlazor;
using renamee.Shared.Models;

namespace renamee.Client.Pages
{
    public partial class Job : ComponentBase
    {
        [Inject]
        private ISnackbar Snackbar { get; set; }
        [Inject]
        private Client Client { get; set; }

        private async Task Save(JobDto job)
        {
            try
            {
                await Client.ApiJobsPutAsync(job);
            }
            catch (ApiException ex)
            {
                Snackbar.Add($"Could not save the current job. {ex.Message}", Severity.Warning);
            }
        }
    }
}
