using MeterReadingUploader.Dtos;
using MeterReadingUploader.Mappers;
using MeterReadingUploader.Persistence.Context;
using System.Text.RegularExpressions;

namespace MeterReadingUploader.Services
{
    public class MeterReadingService
    {
        private readonly AppDbContext _dbContext;

        public MeterReadingService(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public (int successful, int failed) Validate(List<MeterReadingDto> meterReadings)
        {
            var invalidReadings = new List<MeterReadingDto>();
            var validReadings = new List<MeterReadingDto>();
            // Rule 1: Cannot have the same entry twice
            var duplicatesReadings = meterReadings
                .GroupBy(m => new { m.AccountId, m.DateTime, m.ReadValue })
                .Where(g => g.Count() > 1)
                .Select(g => g.FirstOrDefault())
                .ToList();
            invalidReadings.AddRange(duplicatesReadings);
            validReadings.AddRange(meterReadings.Except(duplicatesReadings));

            // Rule 2: A meter reading must associated with an Account Id to be deemed valid
            // Assume the provided Ids are integers
            var existingAccountIds = _dbContext.CustomerAccounts.Select(a => a.Id);
            var invalidAccountReadings = validReadings.Where(m => !existingAccountIds.Contains(m.AccountId)).ToList();
            invalidReadings.AddRange(invalidAccountReadings);
            validReadings = validReadings.Except(invalidAccountReadings).ToList();

            // Rule 3: Reading values should be in the format NNNNN
            var regex = new Regex(@"^\d{5}$");
            var invalidReadValuesReadings = validReadings.Where(m => !regex.IsMatch(m.ReadValue)).ToList();
            invalidReadings.AddRange(invalidReadValuesReadings);
            validReadings = validReadings.Except(invalidReadValuesReadings).ToList();
            return (validReadings.Count, invalidReadings.Count);
        }

        public void StoreReadings(List<MeterReadingDto> meterReadings)
        {
            // Store the readings in the database
            // Assume the database context has been injected
            _dbContext.MeterReadings.AddRange(meterReadings.Select(m => m.ToEntity()));
            _dbContext.SaveChanges();
        }
    }
}
