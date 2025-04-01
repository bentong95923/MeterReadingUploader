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

        public static MeterReading ToEntity(this MeterReadingDto meterReadingDto)
        {
            return new MeterReading
            {
                AccountId = meterReadingDto.AccountId,
                DateTime = meterReadingDto.DateTime,
                ReadValue = int.Parse(meterReadingDto.ReadValue)
            };
        }
    }
}
