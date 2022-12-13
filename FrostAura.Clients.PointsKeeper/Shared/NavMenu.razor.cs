using Microsoft.AspNetCore.Components;
using FrostAura.Clients.PointsKeeper.Data;
using FrostAura.Clients.PointsKeeper.Components.Models;

namespace FrostAura.Clients.PointsKeeper.Shared
{
	public partial class NavMenu : ComponentBase
	{
        [Inject]
        public PointsKeeperDbContext DbContext { get; set; }
        private List<CarouselScene> carouselScenes;
        private bool collapseNavMenu = true;
        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected override void OnInitialized()
        {
            carouselScenes = DbContext
                .Donors
                .Where(d => !d.Deleted)
                .Select(d => new CarouselScene { Name = d.Name, BackgroundUrl = d.Logo })
                .ToList();
        }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}

