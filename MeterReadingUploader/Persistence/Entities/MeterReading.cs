﻿namespace MeterReadingUploader.Persistence.Entities
{
    public class MeterReading
    {
        public Guid Id { get; set; }
        public int AccountId { get; set; }
        public DateTime DateTime { get; set; }
        public int ReadValue { get; set; }
    }
}
