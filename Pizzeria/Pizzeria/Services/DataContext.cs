using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;

namespace Pizzeria.Services
{
    // La classe DataContext estende DbContext e rappresenta il contesto del database
    public class DataContext : DbContext
    {
        // Il costruttore accetta le opzioni per configurare il contesto
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // Definisco le tabelle del database tramite DbSet
        public DbSet<Articolo> Articoli { get; set; }
        public DbSet<Ingrediente> Ingredienti { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UsersRole> UsersRoles { get; set; }
        public DbSet<Ordine> Ordini { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        // Configuro il modello di creazione del database tramite OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurazione specifica per l'entità Articolo
            modelBuilder.Entity<Articolo>(entity =>
            {
                entity.Property(e => e.PrezzoVendita)
                      .HasColumnType("decimal(18,2)"); // Definisco il tipo di colonna per PrezzoVendita

                entity.Property(e => e.Foto)
                      .HasColumnType("varbinary(max)"); // Definisco il tipo di colonna per Foto
            });

            // Configuro la tabella UsersRole con una chiave composta
            modelBuilder.Entity<UsersRole>()
                .ToTable("RoleUser") // Definisco il nome della tabella
                .HasKey(ur => new { ur.UsersId, ur.RolesId }); // Definisco la chiave primaria composta

            // Definisco le relazioni tra UsersRole, User e Role
            modelBuilder.Entity<UsersRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UsersRoles)
                .HasForeignKey(ur => ur.UsersId);

            modelBuilder.Entity<UsersRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UsersRoles)
                .HasForeignKey(ur => ur.RolesId);

            // Configuro la tabella Ordine
            modelBuilder.Entity<Ordine>().ToTable("Ordine");

            // Configuro la tabella OrderItem e le sue relazioni
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
