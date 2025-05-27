using Microsoft.EntityFrameworkCore;
using WowGMSBackend.Repository;
using WowGMSBackend.Service;
using WowGMSBackend.Model;
using WowGMSBackend.DBContext;
using WowGMSBackend.Interfaces;

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
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IRosterService, RosterService>();


builder.Services.AddScoped<IBossKillRepo, BossKillRepo>();
builder.Services.AddScoped<IBossKillService, BossKillService>();

builder.Services.AddScoped<IMemberRepo, MemberRepo>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<ICharacterRepo, CharacterRepo>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IMemberService, MemberService>();

builder.Services.AddScoped<IRosterRepository, RosterRepository>();
builder.Services.AddHostedService<BossKillCheckerService>();

builder.Services.AddScoped<IApplicationRepo, ApplicationRepo>();

builder.Services.AddScoped(typeof(IDBService<>), typeof(DbGenericService<>));
builder.Services.AddDbContext<WowDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("WowGMSBackend")
    )
);

var app = builder.Build();
// SEED TEST ADMIN USER
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WowDbContext>();

    if (!db.Members.Any(m => m.Name == "admin"))
    {
        db.Members.Add(new Member
        {
            Name = "admin",
            Password = "password123", // hash if needed
            Rank = Rank.Officer
        });
        db.SaveChanges();
    }
}
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

