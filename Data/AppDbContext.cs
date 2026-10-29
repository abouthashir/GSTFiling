
using GSTFiling.Models;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<AboutUs> AboutUs { get; set; }

        public DbSet<Centre> Centres { get; set; }

        public DbSet<Consultant> Consultants { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerType> CustomerTypes { get; set; }

        public DbSet<District> Districts { get; set; }

        public DbSet<GstFiling> GstFilings { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Invoices> Invoices { get; set; }

        public DbSet<Invoice> Invoice { get; set; }

        // Master data
        public DbSet<Year> Years { get; set; }

        public DbSet<Month> Months { get; set; }

        public DbSet<InvoiceType> InvoiceTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define primary keys for master data models
            modelBuilder.Entity<Year>()
                .HasKey(y => y.yid);

            modelBuilder.Entity<Month>()
                .HasKey(m => m.mid);

            modelBuilder.Entity<InvoiceType>()
                .HasKey(t => t.ctypid);

            // Map to existing database tables
            modelBuilder.Entity<Year>()
                .ToTable("tbl_year");

            modelBuilder.Entity<Month>()
                .ToTable("tbl_month");

            modelBuilder.Entity<InvoiceType>()
                .ToTable("tbl_ctype");
        }
    }
}
