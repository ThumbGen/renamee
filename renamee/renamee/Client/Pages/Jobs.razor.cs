using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using renamee.Shared.Models;
using System.Net.Http.Json;
using System.Linq;

namespace renamee.Client.Pages
{
    public partial class Jobs : ComponentBase
    {
        //[Inject]
        //private HttpClient Client { get; set; }
        [Inject]
        private IDialogService DialogService { get; set; }
        [Inject]
        private ISnackbar Snackbar { get; set; }
        [Inject]
        private Client Client { get; set; }
        private JobDto[]? jobs;
        private MudForm form;

        protected override async Task OnInitializedAsync()
        {
            jobs = Client == null ? null : (await Client.ApiJobsGetAsync()).ToArray();
            jobs[1].IsEnabled = true;
        }


        private void EnabledChanged(bool e)
        {

            //await Client.PutAsJsonAsync("api/jobs", job);
        }

        private async Task Delete(JobDto job)
        {
            bool? result = await DialogService.ShowMessageBox
                (
                    "Confirmation",
                    $"Are you sure you want to delete the '{job.Name}' job?",
                    yesText: "Yes, delete it!", cancelText: "No, cancel");

            if (result.HasValue && result.Value)
            {
                // do delete
                try
                {
                    await Client.ApiJobsDeleteAsync(job.JobId);
                }
                catch (ApiException ex)
                {
                    Snackbar.Add($"Could not delete the job. {ex.Message}", Severity.Warning);
                }
            }
            StateHasChanged();
        }
    }
}
