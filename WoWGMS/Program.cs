using Microsoft.EntityFrameworkCore;
using WoWGMS.Repository;
using WoWGMS.Service;
using WowGMSBackend.Model;
using WowGMSBackend.Service;
using WowGMSBackend.DBContext;
using WowGMSBackend.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Admin/Login";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});
// Raider IO API
builder.Services.AddHttpClient("RaiderIO", client =>
{
    client.BaseAddress = new Uri("https://raider.io");
});
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IMemberService, MemberService>();
builder.Services.AddSingleton<MemberRepo>();
builder.Services.AddSingleton<ApplicationRepo>();
builder.Services.AddSingleton<IRosterRepository, BossRosterRepo>();
builder.Services.AddHostedService<BossKillCheckerService>();
builder.Services.AddDbContext<WowDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("WowGMSBackend")
    )
);

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();

app.Run();

