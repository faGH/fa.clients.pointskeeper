using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using FrostAura.Clients.PointsKeeper.Components.Models;
using FrostAura.Clients.PointsKeeper.Components.Input;

namespace FrostAura.Clients.PointsKeeper.Pages
{
	public partial class Dashboard : ComponentBase
    {
        private List<FormPropertyEffect> formPropertyEffects = new List<FormPropertyEffect>();
        private List<Point>? points;

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
            //await JsRuntime.InvokeAsync<object>("setTimeout", new object[] { "() => window.location.reload()", 1000 * 15});
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

