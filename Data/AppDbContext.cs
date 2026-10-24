using GSTFiling.Models;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Existing database tables
        public DbSet<AboutUs> AboutUs { get; set; }

        public DbSet<Centre> Centres { get; set; }

        public DbSet<Consultant> Consultants { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerType> CustomerTypes { get; set; }

        public DbSet<District> Districts { get; set; }

        public DbSet<GstFiling> GstFilings { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        // Master Data tables
        public DbSet<FinancialYear> FinancialYears { get; set; }

        public DbSet<FilingMonths> FilingMonths { get; set; }

        public DbSet<GstTaxRates> GstTaxRates { get; set; }

        public DbSet<InvoiceClassification>
            InvoiceClassifications
        { get; set; }

        // Configure decimal precision
        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GstTaxRates>()
                .Property(x => x.Rate)
                .HasPrecision(5, 2);
        }
    }
}