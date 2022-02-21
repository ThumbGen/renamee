using Microsoft.AspNetCore.Components;
using MudBlazor;
using renamee.Client.Components;
using renamee.Shared.Models;

namespace renamee.Client.Pages
{
    public partial class Jobs : ComponentBase
    {
        [Inject]
        private IDialogService DialogService { get; set; }
        [Inject]
        private ISnackbar Snackbar { get; set; }
        [Inject]
        private Client Client { get; set; }

        private IEnumerable<JobDto>? jobs;

        protected override async Task OnInitializedAsync()
        {
            await Refresh();
        }

        private async Task Refresh()
        {
            try
            {
                jobs = (await Client.ApiJobsGetAsync()).OrderBy(x => x.Name);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Could not load the jobs. {ex.Message}", Severity.Error);
                jobs = new List<JobDto>();
            }
        }

        private async Task OnIsEnabledChanged(JobDto job)
        {
            job.IsEnabled = !job.IsEnabled;
            try
            {
                await Client.ApiJobsPutAsync(job);
                var str = job.IsEnabled ? "enabled" : "disabled";
                Snackbar.Add($"Job was {str} successfully.", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Could not update the job. {ex.Message}", Severity.Error);
            }
        }

        private async Task Edit(JobDto job)
        {
            var parameters = new DialogParameters { ["Job"] = job };
            var dialog = DialogService.Show<JobDialog>("New job", parameters, new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Large
            });
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                try
                {
                    await Client.ApiJobsPutAsync(job);
                    Snackbar.Add($"Job was updated successfully.", Severity.Success);
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Could not save the job. {ex.Message}", Severity.Error);
                }
            }
            // refresh always (or store original job, for the cancel case)
            await Refresh();
        }

        private async Task Add()
        {
            var job = new JobDto
            {
                JobId = Guid.NewGuid(),
                Name = string.Empty
            };
            var parameters = new DialogParameters { ["Job"] = job };
            var dialog = DialogService.Show<JobDialog>("Edit job", parameters, new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Large
            });
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                try
                {
                    await Client.ApiJobsPostAsync(job);
                    Snackbar.Add($"Job was added successfully.", Severity.Success);
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Could not add the job. {ex.Message}", Severity.Error);
                }
            }
            
            await Refresh();
        }

        private async Task Delete(JobDto job)
        {
            bool? result = await DialogService.ShowMessageBox
                (
                    "Confirmation",
                    $"Are you sure you want to delete the '{job.Name}' job?",
                    yesText: "Delete", cancelText: "Cancel");

            if (result.HasValue && result.Value)
            {
                // do delete
                try
                {
                    await Client.ApiJobsDeleteAsync(job.JobId);
                    Snackbar.Add($"Job was deleted successfully.", Severity.Success);
                    await Refresh();
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Could not delete the job. {ex.Message}", Severity.Error);
                }
            }
            StateHasChanged();
        }
    }
}
