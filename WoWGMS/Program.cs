using Microsoft.EntityFrameworkCore;
using WowGMSBackend.Repository;
using WowGMSBackend.Service;
using WowGMSBackend.Model;
using WowGMSBackend.DBContext;

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
builder.Services.AddScoped<MemberRepo>();


// Add services to the container.
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IRosterService, RosterService>();



builder.Services.AddScoped<MemberRepo>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<CharacterRepo>();
builder.Services.AddScoped<MemberService>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<MemberRepo>();
builder.Services.AddScoped<IRosterRepository, BossRosterRepo>();
builder.Services.AddHostedService<BossKillCheckerService>();

builder.Services.AddScoped<ApplicationRepo>();

builder.Services.AddScoped(typeof(IDBService<>), typeof(DbGenericService<>));
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

