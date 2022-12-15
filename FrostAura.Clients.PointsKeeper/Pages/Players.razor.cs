using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using FrostAura.Clients.PointsKeeper.Components.Models;
using FrostAura.Clients.PointsKeeper.Components.Input;

namespace FrostAura.Clients.PointsKeeper.Pages
{
    public partial class Players : ComponentBase
    {
        private List<Player>? players;
        private Player? newPlayer;
        private Player? editPlayer;
        private List<FormPropertyEffect> formPropertyEffects = new List<FormPropertyEffect>();

        protected override void OnInitialized()
        {
            List<BaseNamedEntity> teams = dbContext
                .Teams
                .Where(t => !t.Deleted)
                .OrderBy(t => t.Name)
                .Select(t => (BaseNamedEntity)t)
                .ToList();
            formPropertyEffects.Clear();
            formPropertyEffects.Add(new EntitySelectFormPropertyEffect<int, SelectInputCustom<int>>("TeamId", teams));
            formPropertyEffects.Add(new ImagePickerFormPropertyEffect(nameof(Player.Logo)));
            players = dbContext
                .Players
                .Include(p => p.Team)
                .Where(p => !p.Deleted)
                .OrderBy(p => p.Name)
                .ToList();

            newPlayer = null;
            editPlayer = null;
        }

        private void FocusOnCreateNewPlayer()
        {
            editPlayer = null;
            newPlayer = new Player();
        }

        private async Task OnAddPlayerAsync(Player validNewPlayer)
        {
            // See if another player with this name doesn't already exist. If so, use it instead.
            Player? existingPlayer = dbContext
                .Players
                .FirstOrDefault(t => t.Name.ToLower() == validNewPlayer.Name.ToLower());

            if (existingPlayer == default)
            {
                await dbContext.Players.AddAsync(validNewPlayer);
            }
            else
            {
                existingPlayer.Deleted = false;
                dbContext.Update(existingPlayer);
            }

            await dbContext.SaveChangesAsync();
            OnInitialized();
        }

        private void FocusOnEditPlayer(Player playerToEdit)
        {
            editPlayer = playerToEdit;
            newPlayer = null;
        }

        private async Task OnEditPlayerAsync(Player validPlayerToEdit)
        {
            dbContext.Update(validPlayerToEdit);
            await dbContext.SaveChangesAsync();
            OnInitialized();
        }

        private async Task OnRemovePlayerAsync(Player playerToRemove)
        {
            bool confirmed = await JsRuntime.InvokeAsync<bool>("confirm", new[] { $"Are you sure you want to remove player '{playerToRemove.Name}' ?" });

            if (confirmed)
            {
                playerToRemove.Deleted = true;
                dbContext.Update(playerToRemove);
                await dbContext.SaveChangesAsync();
                OnInitialized();
            }
        }

        private string GetImageSrc(Player player)
        {
            var placeholder = "https://via.placeholder.com/256x256";

            if (string.IsNullOrWhiteSpace(player.Logo)) return placeholder;

            return player.Logo;
        }
    }
}
