using MeterReadingUploader.Persistence.Context;
using MeterReadingUploader.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingUploaderTests.Persistence.Context
{
    // These tests are run against the AppDbContext class to just make sure
    // the database context is created and configured correctly ensuring
    // the database is seeded with the correct data.
    // Note that in-memory database is used.
    public class AppDbContextUnitTests
    {
        private DbContextOptions<AppDbContext> GetInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        // This test is used to check if the database context is created and configured correctly.
        // It should be able to add and retrieve Customer Accounts according to what
        // is seeded in the database.
        [Fact]
        public void ShouldAddAndGetCustomerAccounts()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            using (var context = new AppDbContext(options))
            {
                var customerAccount = new CustomerAccount { Id = 1, FirstName = "John", LastName = "Doe" };
                context.CustomerAccounts.Add(customerAccount);
                context.SaveChanges();
            }

            // Act
            using (var context = new AppDbContext(options))
            {
                var customerAccount = context.CustomerAccounts.FirstOrDefault(c => c.Id == 1);

                // Assert
                Assert.NotNull(customerAccount);
                Assert.Equal("John", customerAccount.FirstName);
                Assert.Equal("Doe", customerAccount.LastName);
            }
        }

        // This test is used to check if the database context is created and configured correctly.
        // It should be able to add and retrieve Meter Readings according to what
        // is stored in the database.
        [Fact]
        public void ShouldAddAndGetMeterReadings()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var dateTime = DateTime.Now;
            using (var context = new AppDbContext(options))
            {
                var meterReading = new MeterReading { Id = Guid.NewGuid(), AccountId = 1, DateTime = dateTime, ReadValue = 74589 };
                context.MeterReadings.Add(meterReading);
                context.SaveChanges();
            }

            // Act
            using (var context = new AppDbContext(options))
            {
                var meterReading = context.MeterReadings.FirstOrDefault(m => m.AccountId == 1);

                // Assert
                Assert.NotNull(meterReading);
                Assert.Equal(74589, meterReading.ReadValue);
                Assert.Equal(dateTime, meterReading.DateTime);
            }
        }
    }
}
