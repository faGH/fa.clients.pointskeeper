using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using FrostAura.Clients.PointsKeeper.Components.Models;
using FrostAura.Clients.PointsKeeper.Components.Input;

namespace FrostAura.Clients.PointsKeeper.Pages
{
	public partial class Points : ComponentBase
    {
        private List<Player>? players;
        private Point? newPoint;
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
                .Take(5)
                .ToList();
            players = dbContext
                .Players
                .Where(p => !p.Deleted)
                .OrderBy(p => p.Name)
                .ToList();
            var basePlayers = players
                .Select(p => (BaseNamedEntity)p)
                .ToList();

            formPropertyEffects.Clear();
            formPropertyEffects.Add(new EntitySelectFormPropertyEffect<int, SelectInputCustom<int>>(nameof(Point.Player1Id), basePlayers));
            formPropertyEffects.Add(new EntitySelectFormPropertyEffect<int, SelectInputCustom<int>>(nameof(Point.Player2Id), basePlayers));
            newPoint = new Point();
        }

        private async Task OnResetPointsAsync()
        {
            bool confirmed = await JsRuntime.InvokeAsync<bool>("confirm", new[] { $"Are you sure you want to reset all points and restart the system? Players, teams and donors will be unaffected." });

            if (confirmed)
            {
                dbContext.Points.RemoveRange(dbContext.Points);
                await dbContext.SaveChangesAsync();
                OnInitialized();
            }
        }

        private async Task OnCapturePointAsync(Point validPoint)
        {
            // Savety check for adding the same points twice.
            var lastCapturedPoints = await dbContext
                .Points
                .OrderByDescending(p => p.TimeStamp)
                .FirstOrDefaultAsync();
            var wasLastPointsCapturedIdentical = lastCapturedPoints?.Player1Score == validPoint.Player1Score &&
                lastCapturedPoints?.Player1Id == validPoint.Player1Id &&
                lastCapturedPoints?.Player2Score == validPoint.Player2Score &&
                lastCapturedPoints?.Player2Id == validPoint.Player2Id;

            if(wasLastPointsCapturedIdentical)
            {
                bool confirmed = await JsRuntime.InvokeAsync<bool>("confirm", new[] { $"Are you sure you want to add the same points twice in a row?" });

                if (!confirmed)
                {
                    OnInitialized();
                    return;
                }
            }

            await dbContext.Points.AddAsync(validPoint);
            await dbContext.SaveChangesAsync();
            OnInitialized();
        }
    }
}

