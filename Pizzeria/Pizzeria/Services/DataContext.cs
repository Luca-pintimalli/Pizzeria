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
        public DbSet<UsersRole> UsersRoles { get; set; }
        public DbSet<Ordine> Ordini { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Articolo>(entity =>
            {
                entity.Property(e => e.PrezzoVendita)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Foto)
                      .HasColumnType("varbinary(max)");
            });

            modelBuilder.Entity<UsersRole>()
                .ToTable("RoleUser")
                .HasKey(ur => new { ur.UsersId, ur.RolesId });

            modelBuilder.Entity<UsersRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UsersRoles)
                .HasForeignKey(ur => ur.UsersId);

            modelBuilder.Entity<UsersRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UsersRoles)
                .HasForeignKey(ur => ur.RolesId);

            modelBuilder.Entity<Ordine>().ToTable("Ordine");
            modelBuilder.Entity<OrderItem>().ToTable("OrderItem")
                .HasOne(oi => oi.Articoli)
                .WithMany()
                .HasForeignKey("ArticoliId");

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Ordini)
                .WithMany(o => o.Items)
                .HasForeignKey("OrdiniId");
        }
    }
}
