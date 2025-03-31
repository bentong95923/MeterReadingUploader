using CsvHelper;
using MeterReadingUploader.Dtos;
using MeterReadingUploader.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace MeterReadingUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeterReadingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MeterReadingController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        [HttpPost]
        [Route("meter-reading-uploads")]
        public IActionResult Upload(IFormFile file)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<MeterReadingDtoMap>();
                    var records = csv.GetRecords<MeterReadingDto>().ToList();
                    // Save records to database
                }
            }
            var customer = _context.CustomerAccounts.ToList();
            return Ok("hi");
        }
    }
}
