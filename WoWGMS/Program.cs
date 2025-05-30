using Microsoft.EntityFrameworkCore;
using WowGMSBackend.Repository;
using WowGMSBackend.Service;
using WowGMSBackend.Model;
using WowGMSBackend.DBContext;
using WowGMSBackend.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configure cookie authentication
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Admin/Login";
    });

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// Configure Raider.IO API HTTP client
builder.Services.AddHttpClient("RaiderIO", client =>
{
    client.BaseAddress = new Uri("https://raider.io");
});

// Register application services and repositories
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IRosterService, RosterService>();
builder.Services.AddScoped<IBossKillRepo, BossKillRepo>();
builder.Services.AddScoped<IBossKillService, BossKillService>();
builder.Services.AddScoped<IMemberRepo, MemberRepo>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<ICharacterRepo, CharacterRepo>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IRosterRepository, RosterRepository>();
builder.Services.AddScoped<IApplicationRepo, ApplicationRepo>();
builder.Services.AddScoped(typeof(IDBService<>), typeof(DbGenericService<>));
builder.Services.AddHostedService<BossKillCheckerService>();

// Register Razor Pages
builder.Services.AddRazorPages();

// Configure database context
builder.Services.AddDbContext<WowDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("WowGMSBackend")
    )
);

var app = builder.Build();

// Seed test admin user if not existing
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WowDbContext>();

    if (!db.Members.Any(m => m.Name == "admin"))
    {
        db.Members.Add(new Member
        {
            Name = "admin",
            Password = "password123", // plaintext for now, hashing recommended
            Rank = Rank.Officer
        });
        db.SaveChanges();
    }
}

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // Enforce HTTPS Strict Transport Security
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
