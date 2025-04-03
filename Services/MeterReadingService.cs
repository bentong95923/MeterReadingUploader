using MeterReadingUploader.Dtos;
using MeterReadingUploader.Mappers;
using MeterReadingUploader.Persistence.Context;
using System.Linq;
using System.Text.RegularExpressions;

namespace MeterReadingUploader.Services
{
    public class MeterReadingService : IMeterReadingService
    {
        private readonly AppDbContext _dbContext;

        public MeterReadingService(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public (int successful, int failed) Validate(List<MeterReadingDto> meterReadings)
        {
            // Rule 1: Cannot have the same entry twice
            var duplicateCountsByGroup = meterReadings
                .GroupBy(m => new { m.AccountId, m.DateTime, m.ReadValue })
                .Where(g => g.Count() > 1)
                .ToDictionary(g => g.Key, g => g.Count());

            var invalidReadingCount = duplicateCountsByGroup.Values.Sum();

            var validReadings = meterReadings
                .GroupBy(m => new { m.AccountId, m.DateTime, m.ReadValue })
                .Where(g => g.Count() == 1)
                .Select(g => g.First())
                .ToList();

            // Rule 2: A meter reading must associated with an Account Id to be deemed valid
            // Assume the provided Ids are integers
            var existingAccountIds = _dbContext.CustomerAccounts.Select(a => a.Id);
            invalidReadingCount += validReadings.Where(m => !existingAccountIds.Contains(m.AccountId)).Count();
            validReadings = validReadings.Where(m => existingAccountIds.Contains(m.AccountId)).ToList();

            // Rule 3: Reading values should be in the format NNNNN
            var regex = new Regex(@"^\d{5}$");
            invalidReadingCount += validReadings.Where(m => !regex.IsMatch(m.ReadValue)).Count();
            validReadings = validReadings.Where(m => regex.IsMatch(m.ReadValue)).ToList();

            // Rule 4: New reading shouldn't be older than the existing reading
            // Assume the list is sorted by date time in ascending order    
            var invalidAccountIds = new HashSet<int>();

            foreach (var group in validReadings.GroupBy(m => m.AccountId))
            {
                MeterReadingDto? previousReading = null;
                foreach (var reading in group.OrderBy(m => m.DateTime))
                {
                    if (previousReading != null && reading.DateTime > previousReading.DateTime)
                    {
                        invalidAccountIds.Add(group.Key);
                        break;
                    }
                    previousReading = reading;
                }
            }

            var invalidChronologicalReadings = validReadings.Where(m => invalidAccountIds.Contains(m.AccountId)).ToList();
            invalidReadingCount += invalidChronologicalReadings.Count;
            validReadings = validReadings.Where(m => !invalidAccountIds.Contains(m.AccountId)).ToList();

            return (validReadings.Count, invalidReadingCount);
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
