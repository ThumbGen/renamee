using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using renamee.Client.Components;
using renamee.Shared.Helpers;
using renamee.Shared.Hubs;
using renamee.Shared.Models;
using renamee.Shared.Services;

namespace renamee.Client.Pages
{
    public partial class Jobs : ComponentBase, IDisposable
    {
        [Inject]
        private IDialogService DialogService { get; set; }
        [Inject]
        private ISnackbar Snackbar { get; set; }
        [Inject]
        private Client Client { get; set; }
        [Inject]
        private JobsFactory JobsFactory { get; set; }
        [Inject]
        private HubConnection HubConnection { get; set; }

        private List<IJob> jobs;

        protected override Task OnParametersSetAsync()
        {
            HubConnection.On<JobDto>(nameof(IJobsHub.JobUpdated), newJobData =>
            {
                var found = jobs.FirstOrDefault(x => x.JobId == newJobData.JobId);
                if (found != null)
                {
                    found.AssignFrom(newJobData);
                    StateHasChanged();
                }
            });
            
            return base.OnParametersSetAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            await Refresh();
        }

        private async Task Refresh()
        {
            try
            {
                jobs = (await Client.GetJobsAsync())
                    .OrderBy(dto => dto.Name)
                    .Select(dto =>
                    {
                        var j = JobsFactory.Get<IJob>();
                        j.FromDto(dto);
                        return j;
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Could not load the jobs. {ex.Message}", Severity.Error);
                jobs = new List<IJob>();
            }
        }

        private async Task OnIsEnabledChanged(IJob job)
        {
            await SetIsEnabled(job, !job.IsEnabled);
        }

        private async Task SetIsEnabled(IJob job, bool value)
        {
            job.IsEnabled = value;
            try
            {
                await Client.UpsertJobAsync(job.ToDto());
                var str = job.IsEnabled ? "enabled" : "disabled";
                Snackbar.Add($"Job was {str} successfully.", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Could not update the job. {ex.Message}", Severity.Error);
            }
        }

        private async Task Edit(IJob job)
        {
            var orgIsEnabled = job.IsEnabled;
            try
            {
                // disable job to avoid problems while editing
                await SetIsEnabled(job, false);

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
                        await Client.UpsertJobAsync(job.ToDto());
                        Snackbar.Add($"Job was updated successfully.", Severity.Success);
                    }
                    catch (Exception ex)
                    {
                        Snackbar.Add($"Could not save the job. {ex.Message}", Severity.Error);
                    }
                }
            }
            finally
            {
                await SetIsEnabled(job, orgIsEnabled);
            }

            // refresh always (or store original job, for the cancel case)
            await Refresh();
        }

        private async Task Add()
        {
            var job = JobsFactory.Get<IJob>();
            job.Name = string.Empty;

            var parameters = new DialogParameters { ["Job"] = job };
            var dialog = DialogService.Show<JobDialog>("Add job", parameters, new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Large
            });
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                try
                {
                    await Client.CreateJobAsync(job.ToDto());
                    Snackbar.Add($"Job was added successfully.", Severity.Success);
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Could not add the job. {ex.Message}", Severity.Error);
                }
            }

            await Refresh();
        }

        private async Task Delete(IJob job)
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
                    await Client.DeleteJobAsync(job.JobId);
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

        private async Task Reset(IJob job)
        {
            bool? result = await DialogService.ShowMessageBox
               (
                   "Confirmation",
                   $"Are you sure you want to reset the '{job.Name}' job?",
                   yesText: "Reset", cancelText: "Cancel");

            if (result.HasValue && result.Value)
            {
                // do reset
                try
                {
                    await Client.ResetJobAsync(job.JobId);
                    Snackbar.Add($"Job was reset successfully.", Severity.Success);
                    await Refresh();
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Could not reset the job. {ex.Message}", Severity.Error);
                }
            }
            StateHasChanged();
        }

        public void Dispose()
        {
            HubConnection.Remove(nameof(IJobsHub.JobUpdated));
        }
    }
}
