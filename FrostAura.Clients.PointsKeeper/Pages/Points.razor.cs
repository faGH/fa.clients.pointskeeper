using System;
using System.Net.NetworkInformation;
using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.AspNetCore.Components;
using FrostAura.Clients.PointsKeeper.Data;
using FrostAura.Clients.PointsKeeper.Components.Enums.DynamicForm;
using Microsoft.EntityFrameworkCore;
using FrostAura.Clients.PointsKeeper.Components.Models;
using FrostAura.Clients.PointsKeeper.Components.Input;
using System.Numerics;

namespace FrostAura.Clients.PointsKeeper.Pages
{
	public partial class Points : ComponentBase
    {
        private List<Player>? players;
        private Point? newPoint;
        private List<FormPropertyEffect> formPropertyEffects = new List<FormPropertyEffect>();

        protected override void OnInitialized()
        {
            players = dbContext
                .Players
                .Where(p => !p.Deleted)
                .OrderBy(p => p.Name)
                .ToList();
            var basePlayers = players
                .Select(p => (BaseNamedEntity)p)
                .ToList();

            formPropertyEffects.Clear();
            formPropertyEffects.Add(new EntitySelectFormPropertyEffect<int, SelectInputCustom<int>>("PlayerId", basePlayers));
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
            var wasLastPointsCapturedIdentical = lastCapturedPoints?.Count == validPoint.Count &&
                lastCapturedPoints?.PlayerId == validPoint.PlayerId;

            if(wasLastPointsCapturedIdentical)
            {
                var player = await dbContext.Players.SingleAsync(p => p.Id == validPoint.PlayerId);
                bool confirmed = await JsRuntime.InvokeAsync<bool>("confirm", new[] { $"Are you sure you want to add {validPoint.Count} points for player '{player.Name}' twice in a row?" });

                if (!confirmed)
                {
                    newPoint.Count = 0;
                    return;
                }
            }

            await dbContext.Points.AddAsync(validPoint);
            await dbContext.SaveChangesAsync();
            OnInitialized();
        }
    }
}

