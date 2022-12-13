using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using FrostAura.Clients.PointsKeeper.Components.Models;
using FrostAura.Clients.PointsKeeper.Components.Input;
using Microsoft.JSInterop;

namespace FrostAura.Clients.PointsKeeper.Pages
{
	public partial class Dashboard : ComponentBase
    {
        [Parameter]
        public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(15);
        private List<FormPropertyEffect> formPropertyEffects = new List<FormPropertyEffect>();
        private List<Point>? points;

        [JSInvokable]
        public Task RefreshDashboardAsync()
        {
            OnInitialized();
            StateHasChanged();

            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            points = dbContext
                .Points
                .Include(p => p.Player1)
                .Include(p => p.Player2)
                .Where(p => !p.Deleted)
                .OrderByDescending(p => p.TimeStamp)
                .ToList();
            var players = dbContext
                .Players
                .Where(p => !p.Deleted)
                .OrderBy(p => p.Name)
                .Select(p => (BaseNamedEntity)p)
                .ToList();
            formPropertyEffects.Clear();
            formPropertyEffects.Add(new EntitySelectFormPropertyEffect<int, SelectInputCustom<int>>("PlayerId", players));
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var thisJsReference = DotNetObjectReference.Create(this);
            var bootstrapCommand = @"var boostrapDashboard = (csharpObj) => {
                window.dashboard = csharpObj;
            }";
            var mainLoopCommand = @"(() => {
                window.dashboardsLoaded = window.dashboardsLoaded || false;

                if(window.dashboardsLoaded) return;

                setInterval(() => {
                    window.dashboard.invokeMethodAsync('" + nameof(RefreshDashboardAsync) + @"')
                }, " + Delay.TotalMilliseconds + @");

                window.dashboardsLoaded = true;
            })();";

            await JsRuntime.InvokeVoidAsync("eval", bootstrapCommand);
            await JsRuntime.InvokeVoidAsync("boostrapDashboard", thisJsReference);
            await JsRuntime.InvokeVoidAsync("eval", mainLoopCommand);
            await base.OnAfterRenderAsync(firstRender);
        }

        private double GetTotalDonationsPerPoint()
        {
            return dbContext
                .Donors
                .Where(d => !d.OnceOff && !d.Deleted)
                .Sum(d => d.Amount);
        }

        private double GetTotalOnceOffDonations()
        {
            return dbContext
                .Donors
                .Where(d => d.OnceOff && !d.Deleted)
                .Sum(d => d.Amount);
        }

        private int GetTotalPoints()
        {
            return dbContext
                .Points
                .Include(p => p.Player1)
                .Include(p => p.Player2)
                .Where(p => !p.Deleted && !p.Player1.Deleted && !p.Player2.Deleted)
                .Sum(p => p.Player1Score + p.Player2Score);
        }
    }
}

