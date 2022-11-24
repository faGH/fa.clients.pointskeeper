using FrostAura.Clients.PointsKeeper.Components.Interfaces.Resources;
using FrostAura.Clients.PointsKeeper.Components.Services.Resources;
using FrostAura.Clients.PointsKeeper.Data;
using FrostAura.Clients.PointsKeeper.Components.Models;
using FrostAura.Clients.PointsKeeper.Data.Extensions;
using Microsoft.AspNetCore.Components.Forms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<IContentService, EmbeddedContentService>();
builder.Services.AddFrostAuraResources(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseFrostAuraResources<Program>();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
