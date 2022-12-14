using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.AspNetCore.Components;
using FrostAura.Clients.PointsKeeper.Components.Models;

namespace FrostAura.Clients.PointsKeeper.Pages
{
	public partial class Teams : ComponentBase
    {
        private const int MAX_TEAMS_COUNT = 2;
        private List<Team>? teams;
        private Team? newTeam;
        private Team? editTeam;
        private List<FormPropertyEffect> formPropertyEffects = new List<FormPropertyEffect>();

        protected override void OnInitialized()
        {
            formPropertyEffects.Add(new ImagePickerFormPropertyEffect(nameof(Team.Logo)));
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

            if(dbContext.Teams.Count(t => !t.Deleted) >= MAX_TEAMS_COUNT)
            {
                OnInitialized();
                await JsRuntime.InvokeAsync<bool>("alert", new[] { $"The max of {MAX_TEAMS_COUNT} teams already exist." });
                return;
            }

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

        private string GetImageSrc(Team team)
        {
            var placeholder = "https://via.placeholder.com/256x256";

            if (string.IsNullOrWhiteSpace(team.Logo)) return placeholder;

            return team.Logo;
        }
    }
}

