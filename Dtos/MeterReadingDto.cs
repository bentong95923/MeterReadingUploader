using CsvHelper.Configuration;

namespace MeterReadingUploader.Dtos
{
    public class MeterReadingDto
    {
        //public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime DateTime { get; set; }
        public int ReadValue { get; set; }
    }

    public sealed class MeterReadingDtoMap : ClassMap<MeterReadingDto>
    {
        public MeterReadingDtoMap()
        {
            Map(m => m.AccountId).Name("AccountId");
            Map(m => m.DateTime).TypeConverterOption.Format("dd/MM/yyyy HH:mm").Name("MeterReadingDateTime");
            Map(m => m.ReadValue).Name("MeterReadValue");
        }
    }
}
