using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entities.DbModels;

namespace DatabaseService.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Veritabanı tabloları tanımlama
        public DbSet<Company> Companies { get; set; }
        public DbSet<TcmbExchangeRate> TcmbExchangeRates { get; set; }
        
        // Fluent api ile karışık ayarları yapıyoruz 
        // Örn: Birleşik key tanımlama
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // birleşik anahtar tanımı 
            modelBuilder.Entity<TcmbExchangeRate>()
                .HasKey(e => new { e.Date, e.CurrencyCode, e.Type });
        }
    }
}
