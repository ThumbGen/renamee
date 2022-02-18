using Microsoft.AspNetCore.Components;
using renamee.Shared.DTOs;
using System.Net.Http.Json;

namespace renamee.Client.Pages
{
    public partial class Jobs: ComponentBase
    {
        [Inject]
        private HttpClient? Client { get; set; }
        private JobDto[]? jobs;

        protected override async Task OnInitializedAsync()
        {
            jobs = Client == null ? null : await Client.GetFromJsonAsync<JobDto[]>("api/jobs");
            jobs[1].IsEnabled = true;
        }
    }
}
