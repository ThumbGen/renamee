using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace renamee.Client.Shared
{
    public partial class MainLayout: LayoutComponentBase
    {
        private bool isDrawerOpen = true;
        private bool isDarkMode = false;
        private MudThemeProvider mudThemeProvider;

        public MudTheme MyCustomTheme = new MudTheme();

        //public MudTheme MyCustomTheme = new MudTheme()
        //{
        //    Palette = new Palette()
        //    {
        //        Primary = Colors.Blue.Default,
        //        Secondary = Colors.Green.Accent4,
        //        AppbarBackground = Colors.Red.Default,
        //    },
        //    PaletteDark = new Palette()
        //    {
        //        Primary = Colors.Blue.Lighten1
        //    },

        //    LayoutProperties = new LayoutProperties()
        //    {
        //        DrawerWidthLeft = "260px",
        //        DrawerWidthRight = "300px"
        //    }
        //};

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isDarkMode = await mudThemeProvider.GetSystemPreference();
                StateHasChanged();
            }
        }

        private void ToggleDarkMode()
        {
            isDarkMode = !isDarkMode;
        }

        private void DrawerToggle()
        {
            isDrawerOpen = !isDrawerOpen;
        }

        public void Dispose()
        {
        }
    }
}
