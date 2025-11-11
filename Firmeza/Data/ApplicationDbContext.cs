using Firmeza.Data.Entities;
using Firmeza.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Firmeza.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<Person, IdentityRole<int>, int>(options)
    {
        public DbSet<Person> People { get; set; } 
        public DbSet<Product> Products { get; set; }
        public DbSet<Receipt> Receipts { get; set; } 
        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Person>()
                .ToTable("AspNetUsers")
                .HasDiscriminator<string>("UserType")
                .HasValue<Client>("Client")
                .HasValue<Admin>("Admin");

            modelBuilder.Entity<Person>()
                .HasIndex(p => p.Email)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Document)
                .IsUnique();

            modelBuilder.Entity<Receipt>()
                .HasOne(r => r.Client)
                .WithMany(c => c.Receipts)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict); 
            
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Receipt)
                .WithMany(r => r.SaleLines)
                .HasForeignKey(s => s.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Product)
                .WithMany() 
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict);     
        }
    }
}