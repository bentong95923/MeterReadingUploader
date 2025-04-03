using MeterReadingUploader.Dtos;
using MeterReadingUploader.Persistence.Entities;

namespace MeterReadingUploader.Mappers
{
    public static class MeterReadingMappingExtensions
    {
        public static MeterReadingDto ToDto(this MeterReading meterReading)
        {
            return new MeterReadingDto
            {
                AccountId = meterReading.AccountId,
                DateTime = meterReading.DateTime,
                ReadValue = $"{meterReading.ReadValue}"
            };
        }

        // We are assuming that the AccountId and DateTime are not null,
        // and that the ReadValue is a string of 5 digits
        public static MeterReading ToEntity(this MeterReadingDto meterReadingDto)
        {
            return new MeterReading
            {
                Id = Guid.NewGuid(),
                AccountId = meterReadingDto.AccountId,
                DateTime = meterReadingDto.DateTime,
                ReadValue = int.Parse(meterReadingDto.ReadValue)
            };
        }
    }
}
