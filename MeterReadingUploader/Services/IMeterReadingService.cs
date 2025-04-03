using MeterReadingUploader.Dtos;

namespace MeterReadingUploader.Services
{
    public interface IMeterReadingService
    {
        (int successful, int failed) Validate(List<MeterReadingDto> meterReadings);

        void StoreReadings(List<MeterReadingDto> meterReadings);
    }
}
