using MeterReadingUploader.Dtos;
using MeterReadingUploader.Mappers;
using MeterReadingUploader.Persistence.Entities;

namespace MeterReadingUploaderTests.Extensions
{
    // There are two assumptions when creating unit tests, due to the assumption to the requirement
    // that all properties are required to be provided:
    // 1. MeterReading: all properties provided are not null when creating an instance
    // 2. MeterReadingDto: all properties provided are not null when creating an instance
    // Hence, we do not expect any null checks in the ToDto() and ToEntity() methods,
    // as all properties are expected to be provided.
    public class MeterReadingMappingExtensionsUnitTests
    {
        // This test is to ensure that the ToDto() method returns a valid MeterReadingDto object
        [Fact]
        public void ToDto_ValidMeterReading_ReturnsDto()
        {
            // Arrange
            var meterReading = new MeterReading
            {
                AccountId = 1234,
                DateTime = DateTime.Now,
                ReadValue = 12345
            };

            // Act
            var meterReadingDto = meterReading.ToDto();

            // Assert
            Assert.Equal(meterReading.AccountId, meterReadingDto.AccountId);
            Assert.Equal(meterReading.DateTime, meterReadingDto.DateTime);
            Assert.Equal($"{meterReading.ReadValue}", meterReadingDto.ReadValue);
        }

        // This test is to ensure that the ToEntity() method returns a valid MeterReading entity
        [Fact]
        public void ToEntity_ValidMeterReadingDto_ReturnsEntity()
        {
            // Arrange
            var meterReadingDto = new MeterReadingDto
            {
                AccountId = 1234,
                DateTime = DateTime.Now,
                ReadValue = "12345"
            };

            // Act
            var meterReading = meterReadingDto.ToEntity();

            // Assert
            Assert.Equal(meterReadingDto.AccountId, meterReading.AccountId);
            Assert.Equal(meterReadingDto.DateTime, meterReading.DateTime);
            Assert.Equal(int.Parse(meterReadingDto.ReadValue), meterReading.ReadValue);
            Assert.NotEqual(Guid.Empty, meterReading.Id);
        }
    }
}
