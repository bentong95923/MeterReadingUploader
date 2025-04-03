using MeterReadingUploader.Controllers;
using MeterReadingUploader.Dtos;
using MeterReadingUploader.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Text;

namespace MeterReadingUploaderTests.Controllers
{
    // Testing the MeterReadingController class
    public class MeterReadingControllerUnitTests
    {
        // This test is to ensure that the controller returns a BadRequest response
        // with readable error message when no file is uploaded
        [Fact]
        public void Upload_NoFileUploaded_ReturnsBadRequest()
        {
            // Arrange
            var (mockMeterReadingService, sut) = GetSystemUnderTest();

            // Act
            var objResult = sut.Upload(null) as ObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, objResult.StatusCode);
            var actionResult = objResult.Value as MeterReadingResponse;
            Assert.False(actionResult.Success);
            Assert.Equal("No file was uploaded.", actionResult.Message);
        }

        // This test is to ensure that the controller returns a BadRequest response
        // with readable error message when the CSV content is invalid.
        // The Validate() method is returned with 0 valid and 1 invalid count to
        // simulate an invalid CSV content
        [Fact]
        public void Upload_InvalidCsvContent_ReturnsBadRequest()
        {
            // Arrange
            // Invalid CSV content - MeterReadValue is not in the format NNNNN
            var someInvalidCsvContent = "AccountId,MeterReadingDateTime,MeterReadValue\r\n2351,22/04/2019 12:25,5\r\n";
            var (mockMeterReadingService, sut) = GetSystemUnderTest();
            var file = new Mock<IFormFile>();
            var msContent = new MemoryStream(Encoding.UTF8.GetBytes(someInvalidCsvContent));
            file.Setup(f => f.OpenReadStream()).Returns(msContent);
            // Set up to consider the CSV content is invalid
            mockMeterReadingService.Setup(mrs => mrs.Validate(It.IsAny<List<MeterReadingDto>>())).Returns((0, 1));

            // Act
            var objResult = sut.Upload(file.Object) as ObjectResult;

            // Assert
            Assert.Equal(objResult.StatusCode, (int)HttpStatusCode.BadRequest);
            var actionResult = objResult.Value as MeterReadingResponse;
            Assert.False(actionResult.Success);
            Assert.Equal($"Processed 1 reading(s), where 0 valid and 1 invalid.", actionResult.Message);
        }

        // This test is to ensure that the controller returns an OK response
        // with correct message for a valid CSV content.
        // The Validate() method is returned with 1 valid and 0 invalid count to
        // simulate a valid CSV content
        [Fact]
        public void Upload_ValidCsvContent_ReturnsOk()
        {
            // Arrange
            var validCsvContent = "AccountId,MeterReadingDateTime,MeterReadValue\r\n2351,22/04/2019 12:25,57579\r\n";
            var (mockMeterReadingService, sut) = GetSystemUnderTest();
            var file = new Mock<IFormFile>();
            var msContent = new MemoryStream(Encoding.UTF8.GetBytes(validCsvContent));
            file.Setup(f => f.OpenReadStream()).Returns(msContent);
            // Set up to consider the CSV content is valid
            mockMeterReadingService.Setup(mrs => mrs.Validate(It.IsAny<List<MeterReadingDto>>())).Returns((1, 0));

            // Act
            var objResult = sut.Upload(file.Object) as ObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, objResult.StatusCode);
            var actionResult = objResult.Value as MeterReadingResponse;
            Assert.True(actionResult.Success);
            Assert.Equal($"Successfully uploaded and processed 1 reading(s).", actionResult.Message);
        }

        // This test is to ensure that the controller returns a BadRequest response
        // with readable error message when the CSV content is unparsable.
        [Theory]
        [InlineData("AccountId,MeterReadingDateTime,MeterReadValue,\r\n,,,,,,,,\r\n,,,,,,,,,,,,,,,,")]
        [InlineData("AccasdfountId,MeterReafdsfasdsddingDateTime,MeterReadfwsddValue,\r\n,,,,,,,,\r\n,,,,,,,,,,,,,,,,\r\n")]
        [InlineData("somethingUnparsable")]
        public void Upload_UnparsableCsvContent_ReturnsBadRequest(string unparsableCsvContent)
        {
            // Arrange
            var (mockMeterReadingService, sut) = GetSystemUnderTest();
            var file = new Mock<IFormFile>();
            var msContent = new MemoryStream(Encoding.UTF8.GetBytes(unparsableCsvContent));
            file.Setup(f => f.OpenReadStream()).Returns(msContent);

            // Act
            var objResult = sut.Upload(file.Object) as ObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, objResult.StatusCode);
            var actionResult = objResult.Value as MeterReadingResponse;
            Assert.False(actionResult.Success);
            Assert.Equal("The file cannot be processed. Please ensure the content of the file is in the correct format, or have at least one filled entry in the CSV file.", actionResult.Message);
        }

        public (Mock<IMeterReadingService> mockMeterReadingService, MeterReadingController sut) GetSystemUnderTest()
        {
            var mockMeterReadingService = new Mock<IMeterReadingService>();
            var sut = new MeterReadingController(mockMeterReadingService.Object);
            return (mockMeterReadingService, sut);
        }
    }
}
