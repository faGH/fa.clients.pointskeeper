﻿using System;
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
	public partial class Dashboard : ComponentBase
    {
        private List<Team>? teams;
        private List<FormPropertyEffect> formPropertyEffects = new List<FormPropertyEffect>();

        protected override void OnInitialized()
        {
            teams = dbContext
                .Teams
                .Include(t => t.Players)
                .ThenInclude(p => p.Points)
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
            //await JsRuntime.InvokeAsync<object>("setTimeout", new object[] { "() => window.location.reload()", 1000 * 15});
            await base.OnAfterRenderAsync(firstRender);
        }

        private int GetTotalDonationsPerPoint()
        {
            return dbContext
                .Donors
                .Sum(d => d.Amount);
        }

        private int GetTotalPoints()
        {
            return dbContext
                .Points
                .Sum(p => p.Count);
        }
    }
}

