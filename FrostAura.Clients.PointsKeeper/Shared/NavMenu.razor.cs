using Microsoft.AspNetCore.Components;
using FrostAura.Clients.PointsKeeper.Data;
using FrostAura.Clients.PointsKeeper.Components.Models;
using FrostAura.Clients.PointsKeeper.Interfaces;
using Microsoft.Extensions.Options;
using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.JSInterop;

namespace FrostAura.Clients.PointsKeeper.Shared
{
	public partial class NavMenu : ComponentBase
	{
        private const string AUTH_TOKEN_KEY = "AUTH_TOKEN";
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IOptions<ApplicationConfig> ApplicationConfigOptions { get; set; }
        [Inject]
        public PointsKeeperDbContext DbContext { get; set; }
        [Inject]
        public IClientDataStore ClientDataStore { get; set; }
        private List<CarouselScene> carouselScenes;
        private bool collapseNavMenu = true;
        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        private bool IsSignedIn = false;

        protected override void OnInitialized()
        {
            carouselScenes = DbContext
                .Donors
                .Where(d => !d.Deleted)
                .Select(d => new CarouselScene { Name = d.Name, BackgroundUrl = d.Logo })
                .ToList();

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var authPinCode = ApplicationConfigOptions.Value.AdminPinCode;
            var authToken = await ClientDataStore.GetAsync(AUTH_TOKEN_KEY, CancellationToken.None);

            IsSignedIn = authToken == authPinCode;

            StateHasChanged();

            await base.OnAfterRenderAsync(firstRender);
        }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        private async Task SignInAsync()
        {
            var providedPinCode = await JsRuntime.InvokeAsync<string>("prompt", "Please enter your admin pin code.");

            await ClientDataStore.SetAsync(AUTH_TOKEN_KEY, providedPinCode, CancellationToken.None);
            await OnAfterRenderAsync(false);
        }

        private async Task SignOutAsync()
        {
            await ClientDataStore.SetAsync(AUTH_TOKEN_KEY, string.Empty, CancellationToken.None);
            await OnAfterRenderAsync(false);
        }
    }
}

