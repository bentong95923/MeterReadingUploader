namespace MeterReadingUploader.Persistence.Entities
{
    public class MeterReading
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime DateAndTime { get; set; }
        public int ReadValue { get; set; }
    }
}
