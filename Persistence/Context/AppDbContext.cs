using MeterReadingUploader.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingUploader.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<MeterReading> MeterReadings { get; set; }

        public virtual DbSet<CustomerAccount> CustomerAccounts { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("MeterReadingUploaderInMemoryDb");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed data as in "Test_Accounts.csv" file
            modelBuilder.Entity<CustomerAccount>().HasData(
                new CustomerAccount { Id = 2344, FirstName = "Tommy", LastName = "Test" },
                new CustomerAccount { Id = 2233, FirstName = "Barry", LastName = "Test" },
                new CustomerAccount { Id = 8766, FirstName = "Sally", LastName = "Test" },
                new CustomerAccount { Id = 2345, FirstName = "Jerry", LastName = "Test" },
                new CustomerAccount { Id = 2346, FirstName = "Ollie", LastName = "Test" },
                new CustomerAccount { Id = 2347, FirstName = "Tara", LastName = "Test" },
                new CustomerAccount { Id = 2348, FirstName = "Tammy", LastName = "Test" },
                new CustomerAccount { Id = 2349, FirstName = "Simon", LastName = "Test" },
                new CustomerAccount { Id = 2350, FirstName = "Colin", LastName = "Test" },
                new CustomerAccount { Id = 2351, FirstName = "Gladys", LastName = "Test" },
                new CustomerAccount { Id = 2352, FirstName = "Greg", LastName = "Test" },
                new CustomerAccount { Id = 2353, FirstName = "Tony", LastName = "Test" },
                new CustomerAccount { Id = 2355, FirstName = "Arthur", LastName = "Test" },
                new CustomerAccount { Id = 2356, FirstName = "Craig", LastName = "Test" },
                new CustomerAccount { Id = 6776, FirstName = "Laura", LastName = "Test" },
                new CustomerAccount { Id = 4534, FirstName = "JOSH", LastName = "TEST" },
                new CustomerAccount { Id = 1234, FirstName = "Freya", LastName = "Test" },
                new CustomerAccount { Id = 1239, FirstName = "Noddy", LastName = "Test" },
                new CustomerAccount { Id = 1240, FirstName = "Archie", LastName = "Test" },
                new CustomerAccount { Id = 1241, FirstName = "Lara", LastName = "Test" },
                new CustomerAccount { Id = 1242, FirstName = "Tim", LastName = "Test" },
                new CustomerAccount { Id = 1243, FirstName = "Graham", LastName = "Test" },
                new CustomerAccount { Id = 1244, FirstName = "Tony", LastName = "Test" },
                new CustomerAccount { Id = 1245, FirstName = "Neville", LastName = "Test" },
                new CustomerAccount { Id = 1246, FirstName = "Jo", LastName = "Test" },
                new CustomerAccount { Id = 1247, FirstName = "Jim", LastName = "Test" },
                new CustomerAccount { Id = 1248, FirstName = "Pam", LastName = "Test" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
