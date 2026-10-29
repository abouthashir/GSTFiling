
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

        public DbSet<Invoice> Invoice { get; set; }
    }
}
