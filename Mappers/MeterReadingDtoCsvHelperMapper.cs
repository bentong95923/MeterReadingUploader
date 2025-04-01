using CsvHelper.Configuration;
using MeterReadingUploader.Dtos;

namespace MeterReadingUploader.Mappers
{
    // CsvHelper mapping class
    public class MeterReadingDtoCsvHelperMapper : ClassMap<MeterReadingDto>
    {
        public MeterReadingDtoCsvHelperMapper()
        {
            Map(m => m.AccountId).Name("AccountId");
            Map(m => m.DateTime).TypeConverterOption.Format("dd/MM/yyyy HH:mm").Name("MeterReadingDateTime");
            Map(m => m.ReadValue).Name("MeterReadValue");
        }
    }
}
