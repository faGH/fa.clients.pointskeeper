using System;
using System.Net.NetworkInformation;
using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.AspNetCore.Components;
using FrostAura.Clients.PointsKeeper.Data;
using FrostAura.Clients.PointsKeeper.Components.Enums.DynamicForm;
using Microsoft.EntityFrameworkCore;

namespace FrostAura.Clients.PointsKeeper.Pages
{
	public partial class Donors : ComponentBase
    {
        private List<Donor>? donors;
        private Donor? newDonor;
        private Donor? editDonor;

        protected override void OnInitialized()
        {
            donors = dbContext
                .Donors
                .Where(t => !t.Deleted)
                .OrderBy(t => t.Name)
                .ToList();
            newDonor = null;
            editDonor = null;
        }

        private void FocusOnCreateNewDonor()
        {
            editDonor = null;
            newDonor = new Donor();
        }

        private async Task OnAddDonorAsync(Donor validNewDonor)
        {
            // See if another donor with this name doesn't already exist. If so, use it instead.
            Donor? existingDonor = dbContext
                .Donors
                .FirstOrDefault(t => t.Name.ToLower() == validNewDonor.Name.ToLower());

            if(existingDonor == default)
            {
                await dbContext.Donors.AddAsync(validNewDonor);
            }
            else
            {
                existingDonor.Deleted = false;
                dbContext.Update(existingDonor);
            }

            await dbContext.SaveChangesAsync();
            OnInitialized();
        }

        private void FocusOnEditDonor(Donor donorToEdit)
        {
            editDonor = donorToEdit;
            newDonor = null;
        }

        private async Task OnEditDonorAsync(Donor validDonorToEdit)
        {
            dbContext.Update(validDonorToEdit);
            await dbContext.SaveChangesAsync();
            OnInitialized();
        }

        private async Task OnRemoveDonorAsync(Donor donorToRemove)
        {
            bool confirmed = await JsRuntime.InvokeAsync<bool>("confirm", new[] { $"Are you sure you want to remove donor '{donorToRemove.Name}' ?" });

            if (confirmed)
            {
                donorToRemove.Deleted = true;
                dbContext.Update(donorToRemove);
                await dbContext.SaveChangesAsync();
                OnInitialized();
            }
        }

        private int GetTotalPointsToConsider(bool isOnceOffDonor = false)
        {
            if (isOnceOffDonor) return 1;

            return dbContext
                .Points
                .Include(p => p.Player1)
                .Include(p => p.Player2)
                .Where(p => !p.Deleted && !p.Player1.Deleted && !p.Player2.Deleted)
                .Sum(p => p.Player1Score + p.Player2Score);
        }
    }
}

