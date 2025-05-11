using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using WoW.Model;
using Microsoft.EntityFrameworkCore;


namespace WowGMSBackend.WowDBContext
{
    public class WowDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=mssql14.unoeuro.com;Initial Catalog=mam015_zealand_dk_db_wowgms;User ID=mam015_zealand_dk;Password=34RGdnB5zpcyEm2Hr6Ax; TrustServerCertificate=true");
        }
        public DbSet<Application> Applications { get; set; }
        public DbSet<BossRoster> BossRosters { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BossRoster>().HasKey(e => e.RosterId);
            modelBuilder.Entity<Character>().HasKey(c => c.CharacterName);
            //modelBuilder.Entity<Character>().HasNoKey(); //Mangler løsning uden key
        }
    }
}

