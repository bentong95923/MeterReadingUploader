using CsvHelper;
using MeterReadingUploader.Dtos;
using MeterReadingUploader.Mappers;
using MeterReadingUploader.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace MeterReadingUploader.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MeterReadingController : ControllerBase
    {
        private readonly IMeterReadingService _meterReadingsService;

        public MeterReadingController(IMeterReadingService meterReadingsService)
        {
            _meterReadingsService = meterReadingsService;
        }

        [HttpPost]
        [Route("meter-reading-uploads")]
        [ProducesResponseType(typeof(MeterReadingResponse), 200)]
        [ProducesResponseType(typeof(MeterReadingResponse), 400)]
        [Produces("application/json")]
        public IActionResult Upload(IFormFile? file)
        {
            if (file == null)
            {
                return BadRequest(new MeterReadingResponse()
                {
                    Message = "No file was uploaded.",
                    Success = false
                });
            }
            var readings = new List<MeterReadingDto>();
            // Read the uploaded csv file
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // Parse the csv file into a list of MeterReadingDto objects
                    csv.Context.RegisterClassMap<MeterReadingDtoCsvHelperMapper>();
                    try
                    {
                        readings = csv.GetRecords<MeterReadingDto>().ToList();
                    }
                    catch (Exception)
                    {
                        return BadRequest(new MeterReadingResponse()
                        {
                            Message = $"The file cannot be processed. Please ensure the content of the file is in the correct format, or have at least one filled entry in the CSV file.",
                            Success = false
                        });
                    }
                }
            }
            // Validate the records
            (var validCount, var invalidCount) = _meterReadingsService.Validate(readings);
            if (invalidCount > 0)
            {
                return BadRequest(new MeterReadingResponse()
                {
                    Message = $"Processed {readings.Count} reading(s), where {validCount} valid and {invalidCount} invalid.",
                    Success = false
                });
            }
            // Store in the database
            _meterReadingsService.StoreReadings(readings);
            return Ok(new MeterReadingResponse()
            {
                Message = $"Successfully uploaded and processed {readings.Count} reading(s).",
                Success = true
            });
        }
    }
}
