using Microsoft.EntityFrameworkCore;
using SamuraiApp.Domain;

namespace SamuraiApp.Data
{
    public class SamuraiContext : DbContext
    {
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Clan> Clans { get; set; }

        public DbSet<Battle> Battles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=.\sqlexpress;Database=SamuraiAppData;Trusted_Connection=True;");           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SamuraiBattle>().HasKey(s => new { s.SamuraiId, s.BattleId });
            // as there is no db set declared for horse entity to prevent client code 
            // from interacting with horses directly the table will be defaulted to Horse (singular), 
            // use ToTable to affect the name to Horses (plural).            
            modelBuilder.Entity<Horse>().ToTable("Horses");
        }



    }
}
