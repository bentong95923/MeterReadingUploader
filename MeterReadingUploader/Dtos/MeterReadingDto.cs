namespace MeterReadingUploader.Dtos
{
    public class MeterReadingDto
    {
        public int AccountId { get; set; }
        public DateTime DateTime { get; set; }
        public string ReadValue { get; set; }
    }
}
