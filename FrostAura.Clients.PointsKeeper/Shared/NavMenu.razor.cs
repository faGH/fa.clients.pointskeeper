using Microsoft.AspNetCore.Components;
using FrostAura.Clients.PointsKeeper.Data;
using FrostAura.Clients.PointsKeeper.Components.Models;
using FrostAura.Clients.PointsKeeper.Interfaces;
using Microsoft.Extensions.Options;
using FrostAura.Clients.PointsKeeper.Shared.Models;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Polly;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;
using System;
using System.Text;
using System.Collections.Generic;

namespace FrostAura.Clients.PointsKeeper.Shared
{
	public partial class NavMenu : ComponentBase
	{
        private const string AUTH_TOKEN_KEY = "AUTH_TOKEN";
        [Inject]
        public IOptions<ApplicationConfig> AplicationConfigOptions { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IOptions<ApplicationConfig> ApplicationConfigOptions { get; set; }
        [Inject]
        public PointsKeeperDbContext DbContext { get; set; }
        [Inject]
        public IClientDataStore ClientDataStore { get; set; }
        private List<CarouselScene> carouselScenes;
        private List<Team>? teams;
        private bool collapseNavMenu = true;
        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        private bool IsSignedIn = false;

        protected override void OnInitialized()
        {
            carouselScenes = DbContext
                .Donors
                .Where(d => !d.Deleted)
                .Select(d => new CarouselScene { Name = d.Name, BackgroundUrl = d.Logo })
                .ToList();
            teams = DbContext
                .Teams
                .Where(t => !t.Deleted)
                .OrderBy(t => t.Name)
                .ToList();

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var authPinCode = ApplicationConfigOptions.Value.AdminPinCode;
            var authToken = await ClientDataStore.GetAsync(AUTH_TOKEN_KEY, CancellationToken.None);

            IsSignedIn = authToken == authPinCode;

            StateHasChanged();

            await base.OnAfterRenderAsync(firstRender);
        }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        private string GetMainLogoImgSrc()
        {
            var baseUrl = ApplicationConfigOptions.Value.AppBaseUrl;
            var mainLogoUrl = $"{baseUrl}/img/main_logo.png";

            return mainLogoUrl;
        }

        private async Task SignInAsync()
        {
            var providedPinCode = await JsRuntime.InvokeAsync<string>("prompt", "Please enter your admin pin code.");

            await ClientDataStore.SetAsync(AUTH_TOKEN_KEY, providedPinCode, CancellationToken.None);
            await OnAfterRenderAsync(false);
        }

        private async Task SignOutAsync()
        {
            await ClientDataStore.SetAsync(AUTH_TOKEN_KEY, string.Empty, CancellationToken.None);
            await OnAfterRenderAsync(false);
        }

        private async Task OnMainLogoChangedAsync(InputFileChangeEventArgs args)
        {
            if (!IsSignedIn) return;

            var imagesDirectoryName = "wwwroot/img";
            var baseUrl = Path.Combine(ApplicationConfigOptions.Value.AppBaseUrl, imagesDirectoryName);
            var directory = Path.Combine(Directory.GetCurrentDirectory(), imagesDirectoryName);
            var filename = $"{directory}/main_logo.png";

            await using FileStream fs = new(filename, FileMode.Create);
            await args.File.OpenReadStream(maxAllowedSize: int.MaxValue).CopyToAsync(fs);
            await JsRuntime.InvokeVoidAsync("eval", "window.location.reload()");
        }

        private async Task ExportDataAsync()
        {
            var subDirName = "wwwroot/docs";
            var baseUrl = Path.Combine(AplicationConfigOptions.Value.AppBaseUrl, subDirName);
            var directory = Path.Combine(Directory.GetCurrentDirectory(), subDirName);
            var points_filename = $"{directory}/points.csv";
            var donations_filename = $"{directory}/donations.csv";

            ExportPointsData(points_filename);
            ExportDonationsData(donations_filename);

            var points_url = $"{baseUrl}/points.csv".Replace("wwwroot/", string.Empty);
            var donations_url = $"{baseUrl}/donations.csv".Replace("wwwroot/", string.Empty);

            await JsRuntime.InvokeVoidAsync("eval", $"window.open('{points_url}', '_blank');");
            await JsRuntime.InvokeVoidAsync("eval", $"window.open('{donations_url}', '_blank');");
        }

        private void ExportPointsData(string fileName)
        {
            var csv = new StringBuilder();
            var points = GroupPointsByTeam(DbContext
                .Points
                .Include(p => p.Player1)
                .Include(p => p.Player2)
                .Where(p => !p.Deleted)
                .OrderByDescending(p => p.TimeStamp)
                .ToList());

            csv.AppendLine(string.Format("{0},{1},{2},{3},{4}",
                "WCC Player",
                "WCC Points",
                "Crusaders Player",
                "Crusaders Points",
                "Time Captured"));

            foreach (var point in points)
            {
                csv.AppendLine(string.Format("{0},{1},{2},{3},{4}",
                point.Player1.Name,
                point.Player1Score,
                point.Player2.Name,
                point.Player2Score,
                point.TimeStamp.ToString("H:mm:ss MMMM dd yyyy")));
            }

            csv.AppendLine(string.Format("{0},{1},{2},{3},{4}",
                string.Empty,
                points.Sum(p => p.Player1Score),
                string.Empty,
                points.Sum(p => p.Player2Score),
                string.Empty));
            File.WriteAllText(fileName, csv.ToString());
        }

        private void ExportDonationsData(string fileName)
        {
            var csv = new StringBuilder();
            var donations = DbContext
                .Donors
                .Where(d => !d.Deleted)
                .OrderBy(d => d.Name);
            var totalPoints = GetTotalPointsToConsider();

            csv.AppendLine(string.Format("{0},{1},{2},{3}",
                "Name",
                "Amount",
                "Is Once-Off",
                $"Dues (For {totalPoints} Points)"));

            foreach (var donation in donations)
            {
                csv.AppendLine(string.Format("{0},{1},{2},{3}",
                donation.Name,
                $"{donation.Amount} (" + (donation.OnceOff ? "Once-Off" : "Per-Point"),
                donation.OnceOff,
                Math.Round(GetTotalPointsToConsider(donation.OnceOff) * donation.Amount, 2)
                ));
            }

            csv.AppendLine(string.Format("{0},{1},{2},{3}",
                string.Empty,
                GetTotal(),
                string.Empty,
                string.Empty));
            File.WriteAllText(fileName, csv.ToString());
        }

        private double GetTotalDonationsPerPoint()
        {
            return DbContext
                .Donors
                .Where(d => !d.OnceOff && !d.Deleted)
                .Sum(d => d.Amount);
        }

        private double GetTotalOnceOffDonations()
        {
            return DbContext
                .Donors
                .Where(d => d.OnceOff && !d.Deleted)
                .Sum(d => d.Amount);
        }

        private int GetTotalPoints()
        {
            return DbContext
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

        private int GetTotalPointsToConsider(bool isOnceOffDonor = false)
        {
            if (isOnceOffDonor) return 1;

            return DbContext
                .Points
                .Include(p => p.Player1)
                .Include(p => p.Player2)
                .Where(p => !p.Deleted && !p.Player1.Deleted && !p.Player2.Deleted)
                .Sum(p => p.Player1Score + p.Player2Score);
        }

        private IEnumerable<Point> GroupPointsByTeam(IEnumerable<Point> points)
        {
            var team1Id = teams[1].Id;
            var team2Id = teams[0].Id;
            var groupedPoint = new Point();

            foreach (var point in points)
            {
                var swoppingRequired = point.Player1.TeamId != team1Id;

                if (!swoppingRequired) yield return point;
                else yield return new Point
                {
                    Player1 = point.Player2,
                    Player1Id = point.Player2Id,
                    Player1Score = point.Player2Score,
                    Player2 = point.Player1,
                    Player2Id = point.Player1Id,
                    Player2Score = point.Player1Score,
                    TimeStamp = point.TimeStamp,
                    Id = point.Id
                };
            }
        }
    }
}

