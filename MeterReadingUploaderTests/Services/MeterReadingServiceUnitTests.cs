using MeterReadingUploader.Dtos;
using MeterReadingUploader.Persistence.Context;
using MeterReadingUploader.Persistence.Entities;
using MeterReadingUploader.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MeterReadingUploaderTests.Services
{
    // Testing the MeterReadingService class
    public class MeterReadingServiceUnitTests
    {
        // The Validate() method always return 2 counts: successful and failed readings counts.
        // Four validation rules are applied to the readings:
        // Rule 1: Cannot have the same entry twice
        // Rule 2: A meter reading must associated with an Account Id to be deemed valid - Assume the provided Ids are integers
        // Rule 3: Reading values should be in the format NNNNN
        // Rule 4: New reading shouldn't be older than the existing reading - Assume the list is sorted by date time in ascending order 

        // This test checks if the Validate() method returns the correct counts when the meter readings are valid
        // for duplicate readings. The method should return 0 successful counts and 2 failed counts as there
        // are 2 duplicate readings in the list.
        // Since there are invalid entries, the DbContext.SaveChanges() method should not be called.
        [Fact]
        public void Validate_DuplicateReadings_ReturnsCorrectCounts()
        {
            // Arrange
            var (mockDbContext, sut) = GetSystemUnderTest();
            var dateTime = DateTime.UtcNow;
            // Existing customer accounts
            var customerAccounts = new List<CustomerAccount>
            {
                new()
                {
                    Id = 1,
                    FirstName = "TestFirstName",
                    LastName = "TestLastName"
                }
            }.AsQueryable();
            // Meter readings with duplicate readings
            var meterReadings = new List<MeterReadingDto>
            {
                new() // Duplicate readings
                {
                    AccountId = 1,
                    DateTime = dateTime,
                    ReadValue = "12345"
                },
                new() // Duplicate readings
                {
                    AccountId = 1,
                    DateTime = dateTime,
                    ReadValue = "12345"
                }
            };

            var mockDbSetCustomerAccount = new Mock<DbSet<CustomerAccount>>();
            SetupMockDbSet(mockDbSetCustomerAccount, customerAccounts);
            mockDbContext.Setup(m => m.CustomerAccounts).Returns(mockDbSetCustomerAccount.Object);

            // Act
            (var successfulCount, var failedCount) = sut.Validate(meterReadings);

            // Assert
            Assert.Equal(0, successfulCount);
            Assert.Equal(2, failedCount);
            mockDbContext.Verify(db => db.SaveChanges(), Times.Never);
        }

        // This test checks if the Validate() method returns the correct counts when the meter readings are valid
        // i.e. invalid account id. The method should return 0 successful counts and 1 failed count as there
        // are 1 invalid reading in the list.
        // Since there are invalid entries, the DbContext.SaveChanges() method should not be called.
        [Fact]
        public void Validate_InvalidAccountId_ReturnsCorrectCounts()
        {
            // Arrange
            var (mockDbContext, sut) = GetSystemUnderTest();
            // Existing customer accounts
            var customerAccounts = new List<CustomerAccount>
            {
                new()
                {
                    Id = 1,
                    FirstName = "TestFirstName",
                    LastName = "TestLastName"
                }
            }.AsQueryable();
            // Meter readings with invalid account id
            var meterReadings = new List<MeterReadingDto>
            {
                new()
                {
                    AccountId = 9999, // Invalid AccountId
                    DateTime = DateTime.Now,
                    ReadValue = "45678"
                }
            };
            var mockDbSetCustomerAccount = new Mock<DbSet<CustomerAccount>>();
            SetupMockDbSet(mockDbSetCustomerAccount, customerAccounts);
            mockDbContext.Setup(m => m.CustomerAccounts).Returns(mockDbSetCustomerAccount.Object);

            // Act
            (var successfulCount, var failedCount) = sut.Validate(meterReadings);

            // Assert
            Assert.Equal(0, successfulCount);
            Assert.Equal(1, failedCount);
            mockDbContext.Verify(db => db.SaveChanges(), Times.Never);
        }

        // This test checks if the Validate() method returns the correct counts when the reading values are invalid
        // i.e. not in the format NNNNN. The method should return 0 successful counts and 1 failed count as there
        // is 1 invalid reading value in the list.
        // Since there are invalid entries, the DbContext.SaveChanges() method should not be called.
        [Fact]
        public void Validate_InvalidReadingValue_ReturnsCorrectCounts()
        {
            // Arrange
            var (mockDbContext, sut) = GetSystemUnderTest();
            // Existing customer accounts
            var customerAccounts = new List<CustomerAccount>
            {
                new()
                {
                    Id = 1,
                    FirstName = "TestFirstName",
                    LastName = "TestLastName"
                }
            }.AsQueryable();
            // Meter readings with invalid reading value
            var meterReadings = new List<MeterReadingDto>
            {
                new()
                {
                    AccountId = 1,
                    DateTime = DateTime.Now,
                    ReadValue = "1234" // Invalid reading value
                }
            };
            var mockDbSetCustomerAccount = new Mock<DbSet<CustomerAccount>>();
            SetupMockDbSet(mockDbSetCustomerAccount, customerAccounts);
            mockDbContext.Setup(m => m.CustomerAccounts).Returns(mockDbSetCustomerAccount.Object);

            // Act
            (var successfulCount, var failedCount) = sut.Validate(meterReadings);

            // Assert
            Assert.Equal(0, successfulCount);
            Assert.Equal(1, failedCount);
            mockDbContext.Verify(db => db.SaveChanges(), Times.Never);
        }

        // This test checks if the Validate() method returns the correct counts when there are multiple invalid entries
        // The method should return 0 successful counts and 4 failed counts as there are no valid entries and
        // 4 invalid entries in the list.
        // Since there are invalid entries, the DbContext.SaveChanges() method should not be called.
        [Fact]
        public void Validate_MultipleInvalidEntries_ReturnCorrectCounts()
        {
            // Arrange
            var (mockDbContext, sut) = GetSystemUnderTest();
            // Existing customer accounts
            var customerAccounts = new List<CustomerAccount>
            {
                new()
                {
                    Id = 1,
                    FirstName = "TestFirstName",
                    LastName = "TestLastName"
                }
            }.AsQueryable();
            // Meter readings with invalid entries
            var meterReadings = new List<MeterReadingDto>
            {
                new()
                {
                    AccountId = 1,
                    DateTime = DateTime.Now,
                    ReadValue = "1234" // Invalid reading value
                },
                new()
                {
                    AccountId = 9999, // Invalid AccountId
                    DateTime = DateTime.Now.AddMinutes(10),
                    ReadValue = "45678"
                },
                new() // Duplicate reading
                {
                    AccountId = 1,
                    DateTime = new DateTime(2025, 1, 1, 7, 20, 10),
                    ReadValue = "88888"
                },
                new() // Duplicate reading
                {
                    AccountId = 1,
                    DateTime = new DateTime(2025, 1, 1, 7, 20, 10),
                    ReadValue = "88888"
                }
            };
            var mockDbSetCustomerAccount = new Mock<DbSet<CustomerAccount>>();
            SetupMockDbSet(mockDbSetCustomerAccount, customerAccounts);
            mockDbContext.Setup(m => m.CustomerAccounts).Returns(mockDbSetCustomerAccount.Object);

            // Act
            (var successfulCount, var failedCount) = sut.Validate(meterReadings);

            // Assert
            Assert.Equal(0, successfulCount);
            Assert.Equal(4, failedCount);
            mockDbContext.Verify(db => db.SaveChanges(), Times.Never);
        }

        // This test checks if the Validate() method returns the correct counts when there are valid and invalid entries
        // The method should return 2 successful counts and 3 failed counts as there are 2 valid entries and
        // 3 invalid entries in the list.
        // Since there are invalid entries, the DbContext.SaveChanges() method should not be called.
        [Fact]
        public void Validate_ValidMixedWithInvalidEntries_ReturnsCorrectCounts()
        {
            // Arrange
            var (mockDbContext, sut) = GetSystemUnderTest();
            // Existing customer accounts
            var customerAccounts = new List<CustomerAccount>
            {
                new()
                {
                    Id = 1,
                    FirstName = "TestFirstName",
                    LastName = "TestLastName"
                },
                new()
                {
                    Id = 2,
                    FirstName = "TestFirstName2",
                    LastName = "TestLastName2"
                }
            }.AsQueryable();
            // Meter readings with valid and invalid entries
            var meterReadings = new List<MeterReadingDto>
            {
                new() // Valid reading
                {
                    AccountId = 1,
                    DateTime = DateTime.Now,
                    ReadValue = "12345"
                },
                new()
                {
                    AccountId = 1,
                    DateTime = DateTime.Now,
                    ReadValue = "1235" // Invalid reading value
                },
                new() // Duplicate reading
                {
                    AccountId = 1,
                    DateTime = new DateTime(2025, 1, 2, 7, 20, 10),
                    ReadValue = "66666"
                },
                new() // Duplicate reading
                {
                    AccountId = 1,
                    DateTime = new DateTime(2025, 1, 2, 7, 20, 10),
                    ReadValue = "66666"
                }
                ,
                new() // Valid reading
                {
                    AccountId = 2,
                    DateTime = new DateTime(2025, 1, 2, 7, 40, 10),
                    ReadValue = "10001"
                }
            };
            var mockDbSetCustomerAccount = new Mock<DbSet<CustomerAccount>>();
            SetupMockDbSet(mockDbSetCustomerAccount, customerAccounts);
            mockDbContext.Setup(m => m.CustomerAccounts).Returns(mockDbSetCustomerAccount.Object);
            // Act
            (var successfulCount, var failedCount) = sut.Validate(meterReadings);
            // Assert
            Assert.Equal(2, successfulCount);
            Assert.Equal(3, failedCount);
            mockDbContext.Verify(db => db.SaveChanges(), Times.Never);
        }

        // This test checks if the Validate() method returns the correct counts when the meter readings are valid
        // but not in chronological order per AccountId. The method should return 1 successful count and 3 failed counts as there
        // are 1 valid entry 1 invalid entry with invalid AccountId and 2 entries not in chronological order in the list.
        // Since there are invalid entries, the DbContext.SaveChanges() method should not be called.
        [Fact]
        public void Validate_ValidWithInvalidMeterReadingsNotInChronologicalOrderPerAccountId_ReturnsCorrectCounts()
        {
            // Arrange
            var (mockDbContext, sut) = GetSystemUnderTest();
            // Existing customer accounts
            var customerAccounts = new List<CustomerAccount>
            {
                new()
                {
                    Id = 1,
                    FirstName = "TestFirstName",
                    LastName = "TestLastName"
                },
                new()
                {
                    Id = 3,
                    FirstName = "TestFirstName3",
                    LastName = "TestLastName3"
                }
            }.AsQueryable();
            // Meter readings mixed with invalid entries and valid entries but not in chronological order per AccountId
            var meterReadings = new List<MeterReadingDto>
            {
                new() // Valid reading
                {
                    AccountId = 3,
                    DateTime = new DateTime(2025, 1, 1, 8, 20, 10),
                    ReadValue = "00005"
                },
                new() // Reading not in chronological order for AccountId 1
                {
                    AccountId = 1,
                    DateTime = new DateTime(2025, 1, 3, 7, 20, 10),
                    ReadValue = "12346"
                },
                new()
                {
                    AccountId = 2, // Invalid AccountId
                    DateTime = new DateTime(2025, 1, 2, 7, 20, 10),
                    ReadValue = "12345"
                },
                new() // Reading not in chronological order for AccountId 1
                {
                    AccountId = 1,
                    DateTime = new DateTime(2025, 1, 2, 7, 20, 10),
                    ReadValue = "12300"
                }
            };
            var mockDbSetCustomerAccount = new Mock<DbSet<CustomerAccount>>();
            SetupMockDbSet(mockDbSetCustomerAccount, customerAccounts);
            mockDbContext.Setup(m => m.CustomerAccounts).Returns(mockDbSetCustomerAccount.Object);

            // Act
            (var successfulCount, var failedCount) = sut.Validate(meterReadings);

            // Assert
            Assert.Equal(1, successfulCount);
            Assert.Equal(3, failedCount);
            mockDbContext.Verify(db => db.SaveChanges(), Times.Never);
        }

        // This test checks if the StoreReadings() method stores the valid meter readings in the database
        // and returns the correct counts. There are 2 valid readings in the list hence should call the
        // DbContext.SaveChanges() method and return 2 successful counts and 0 failed counts.
        [Fact]
        public void StoreReadings_ValidMeterReadings_StoresInDatabaseAndReturnsCorrectCounts()
        {
            // Arrange
            var (mockDbContext, sut) = GetSystemUnderTest();
            // Existing customer accounts
            var customerAccounts = new List<CustomerAccount>
            {
                new()
                {
                    Id = 1,
                    FirstName = "TestFirstName",
                    LastName = "TestLastName"
                },
                new()
                {
                    Id = 10,
                    FirstName = "TestFirstName10",
                    LastName = "TestLastName10"
                }
            }.AsQueryable();
            // Valid meter readings
            var meterReadings = new List<MeterReadingDto>
            {
                new()
                {
                    AccountId = 1,
                    DateTime = DateTime.Now,
                    ReadValue = "12345"
                },
                new()
                {
                    AccountId = 10,
                    DateTime = DateTime.Now.AddMinutes(10),
                    ReadValue = "22222"
                }
            };

            var mockDbSetCustomerAccount = new Mock<DbSet<CustomerAccount>>();
            var mockMeterReadingsDbSet = new Mock<DbSet<MeterReading>>();
            SetupMockDbSet(mockDbSetCustomerAccount, customerAccounts);
            mockDbContext.Setup(m => m.CustomerAccounts).Returns(mockDbSetCustomerAccount.Object);
            mockDbContext.Setup(m => m.MeterReadings).Returns(mockMeterReadingsDbSet.Object);

            // Act
            sut.StoreReadings(meterReadings);

            // Assert
            mockMeterReadingsDbSet.Verify(db => db.AddRange(It.IsAny<IEnumerable<MeterReading>>()), Times.Once);
            mockDbContext.Verify(db => db.SaveChanges(), Times.Once);
        }

        private void SetupMockDbSet<T>(Mock<DbSet<T>> mockDbSet, IQueryable<T> data) where T : class
        {
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        private (Mock<AppDbContext> mockDbContext, MeterReadingService meterReadingService) GetSystemUnderTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            var mockDbContext = new Mock<AppDbContext>(options);
            var meterReadingService = new MeterReadingService(mockDbContext.Object);
            return (mockDbContext, meterReadingService);
        }
    }
}