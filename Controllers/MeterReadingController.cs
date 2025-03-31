using CsvHelper;
using MeterReadingUploader.Dtos;
using MeterReadingUploader.Persistence.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace MeterReadingUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeterReadingController : ControllerBase
    {
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
            return Ok("hi");
        }
    }
}
