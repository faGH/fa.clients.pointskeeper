using System;
using System.Net.NetworkInformation;
using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.AspNetCore.Components;
using FrostAura.Clients.PointsKeeper.Data;
using FrostAura.Clients.PointsKeeper.Components.Enums.DynamicForm;

namespace FrostAura.Clients.PointsKeeper.Pages
{
	public partial class Teams : ComponentBase
    {
        private List<Team>? teams;
        private Team? newTeam;
        private Team? editTeam;

        protected override void OnInitialized()
        {
            teams = dbContext
                .Teams
                .Where(t => !t.Deleted)
                .OrderBy(t => t.Name)
                .ToList();
            newTeam = null;
            editTeam = null;
        }

        private void FocusOnCreateNewTeam()
        {
            editTeam = null;
            newTeam = new Team();
        }

        private async Task OnAddTeamAsync(Team validNewTeam)
        {
            // See if another team with this name doesn't already exist. If so, use it instead.
            Team? existingTeam = dbContext
                .Teams
                .FirstOrDefault(t => t.Name.ToLower() == validNewTeam.Name.ToLower());

            if(existingTeam == default)
            {
                await dbContext.Teams.AddAsync(validNewTeam);
            }
            else
            {
                existingTeam.Deleted = false;
                dbContext.Update(existingTeam);
            }

            await dbContext.SaveChangesAsync();
            OnInitialized();
        }

        private void FocusOnEditTeam(Team teamToEdit)
        {
            editTeam = teamToEdit;
            newTeam = null;
        }

        private async Task OnEditTeamAsync(Team validTeamToEdit)
        {
            dbContext.Update(validTeamToEdit);
            await dbContext.SaveChangesAsync();
            OnInitialized();
        }

        private async Task OnRemoveTeamAsync(Team teamToRemove)
        {
            bool confirmed = await JsRuntime.InvokeAsync<bool>("confirm", new[] { $"Are you sure you want to remove team '{teamToRemove.Name}' ?" });

            if (confirmed)
            {
                teamToRemove.Deleted = true;
                dbContext.Update(teamToRemove);
                await dbContext.SaveChangesAsync();
                OnInitialized();
            }
        }
    }
}

