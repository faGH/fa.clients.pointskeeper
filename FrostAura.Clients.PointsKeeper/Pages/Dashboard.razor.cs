using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using FrostAura.Clients.PointsKeeper.Components.Models;
using FrostAura.Clients.PointsKeeper.Components.Input;
using Microsoft.JSInterop;
using Microsoft.Extensions.Options;

namespace FrostAura.Clients.PointsKeeper.Pages
{
	public partial class Dashboard : ComponentBase
    {
        private const int POINTS_TO_DISPLAY_IN_LADDER = 15;
        [Inject]
        public IOptions<ApplicationConfig> ConfigOptions { get; set; }
        [Parameter]
        public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(15);
        private List<FormPropertyEffect> formPropertyEffects = new List<FormPropertyEffect>();
        private List<Point>? points;
        private List<Team>? teams;

        [JSInvokable]
        public Task RefreshDashboardAsync()
        {
            OnInitialized();
            StateHasChanged();

            return Task.CompletedTask;
        }

        private int GetPointsForTeam(int teamIndex)
        {
            if (teamIndex > teams.Count) return 0;

            var team = teams[teamIndex];
            var allPlayer1TeamPoints = points
                .Where(p => p.Player1.TeamId == team.Id)
                .Sum(p => p.Player1Score);
            var allPlayer2TeamPoints = points
                .Where(p => p.Player2.TeamId == team.Id)
                .Sum(p => p.Player2Score);

            return allPlayer1TeamPoints + allPlayer2TeamPoints;
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
            teams = dbContext
                .Teams
                .Where(t => !t.Deleted)
                .OrderBy(t => t.Name)
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

        private double GetTotal()
        {
            return Math.Round(GetTotalDonationsPerPoint() * GetTotalPoints() + GetTotalOnceOffDonations(), 2);
        }

        private int GetProgressPercentage()
        {
            var total = GetTotal();
            var target = ConfigOptions.Value.MonitaryTarget;
            var delta = total / target * 100;

            return (int)delta;
        }
    }
}

