using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;

namespace Pizzeria.Services
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Articolo> Articoli { get; set; }
        public DbSet<Ingrediente> Ingredienti { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Articolo>(entity =>
            {
                entity.Property(e => e.PrezzoVendita)
                      .HasColumnType("decimal(18,2)"); // Specifica il tipo di colonna e la precisione e la scala
            });
        }


      
    }
}
