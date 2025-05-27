using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;
namespace WowGMSBackend.DBContext
{
    public class WowDbContext : DbContext
    {
        public WowDbContext(DbContextOptions<WowDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer(@"Data Source=mssql14.unoeuro.com;Initial Catalog=mam015_zealand_dk_db_wowgms;User ID=mam015_zealand_dk;Password=34RGdnB5zpcyEm2Hr6Ax;TrustServerCertificate=true");
            }
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<BossRoster> BossRosters { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<BossKill> BossKills { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BossRoster>().HasKey(e => e.RosterId);
            

            modelBuilder.Entity<Application>()
                .Property(a => a.ServerName)
                .HasConversion<string>();

            modelBuilder.Entity<Application>()
                .Property(a => a.Class)
                .HasConversion<string>();

            modelBuilder.Entity<Application>()
                .Property(a => a.Role)
                .HasConversion<string>();
            modelBuilder.Entity<BossKill>()
                .HasOne(bk => bk.Application)
                .WithMany(a => a.BossKills)
                .HasForeignKey(bk => bk.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BossKill>()
                .HasOne(bk => bk.Character)
                .WithMany(c => c.BossKills)
                .HasForeignKey(bk => bk.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}