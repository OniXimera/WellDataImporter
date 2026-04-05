using Moq;
using System.Windows;
using WellDataImporter.Services;

namespace WellDataImporter.Tests.Services
{
    public class CsvWellReaderTest
    {
        public CsvWellReaderTest()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _service = new CsvWellReader(_fileSystemMock.Object);
        }

        [Fact]
        public async Task ReadAsync_ValidData_ReturnsWellsWithoutErrors()
        {
            var csvLines = new[]
            {
                "A-001;82.10;55.20;0;10;Sandstone;0.18",
                "A-001;82.10;55.20;10;25;Limestone;0.07",
                "A-002;90.00;60.00;0;15;Shale;0.04"
            };

            _fileSystemMock.Setup(f => f.ReadAllLinesAsync(It.IsAny<string>()))
                          .ReturnsAsync(csvLines);

            var (wells, errors) = await _service.ReadAsync("test.csv");
            Assert.Empty(errors);
            Assert.Equal(2, wells.Count);
            var wellA001 = wells.First(w => w.WellId == "A-001");
            Assert.Equal(2, wellA001.Intervals.Count);
            Assert.Equal(25, wellA001.Intervals.Max(i => i.DepthTo));
        }

        [Theory]
        [InlineData("A-001;82.10;55.20;0;10;Sandstone;1.5", "ErrorPorosity01")]   // Porosity > 1
        [InlineData("A-001;82.10;55.20;15;10;Sandstone;0.2", "ErrorDepthFromDepthTo")] // DepthFrom >= DepthTo
        [InlineData("A-001;82.10;55.20;-5;10;Sandstone;0.2", "ErrorDepthFromZero")] // DepthFrom < 0
        [InlineData("A-001;82.10;55.20;0;10;;0.2", "ErrorNullRock")]               // ErrorNullRock
        public async Task ReadAsync_InvalidLine_AddsCorrectError(string invalidLine, string resourceKey)
        {
            _fileSystemMock.Setup(f => f.ReadAllLinesAsync(It.IsAny<string>()))
                          .ReturnsAsync(new[] { invalidLine });

            var (wells, errors) = await _service.ReadAsync("test.csv");
            Assert.Single(errors);
            string expectedMessage = Resources.Resources.ResourceManager.GetString(resourceKey);
            Assert.NotEmpty(errors[0].ErrorMessage);
            Assert.Contains(expectedMessage, errors[0].ErrorMessage);
            Assert.Empty(wells);
        }

        [Fact]
        public async Task ReadAsync_OverlappingIntervals_RemovesWholeWellAndAddsError()
        {
            var csvLines = new[]
            {
                "A-001;82.10;55.20;0;20;Sandstone;0.18",
                "A-001;82.10;55.20;15;25;Limestone;0.07"
            };

            _fileSystemMock.Setup(f => f.ReadAllLinesAsync(It.IsAny<string>()))
                          .ReturnsAsync(csvLines);

            var (wells, errors) = await _service.ReadAsync("test.csv");
            Assert.Single(errors);
            Assert.Contains(Resources.Resources.ErrorInterval, errors[0].ErrorMessage);
            Assert.Empty(wells);
        }

        [Fact]
        public async Task ReadAsync_FileReadException_ReturnsError()
        {
            _fileSystemMock.Setup(f => f.ReadAllLinesAsync(It.IsAny<string>()))
                          .ThrowsAsync(new IOException("Disk error"));

            var (wells, errors) = await _service.ReadAsync("test.csv");
            Assert.Single(errors);
            Assert.Equal(0, errors[0].LineNumber);
            Assert.Contains(Resources.Resources.StatusReadError, errors[0].ErrorMessage);
            Assert.Empty(wells);
        }

        [Fact]
        public async Task ReadAsync_EmptyFileOrOnlyHeader_ReturnsEmptyWells()
        {
            _fileSystemMock.Setup(f => f.ReadAllLinesAsync(It.IsAny<string>()))
                          .ReturnsAsync(new[] { "WellId;X;Y;DepthFrom;DepthTo;Rock;Porosity" });

            var (wells, errors) = await _service.ReadAsync("test.csv");
            Assert.Empty(errors);
            Assert.Empty(wells);
        }

        private readonly Mock<IFileSystem> _fileSystemMock;
        private readonly CsvWellReader _service;
    }
}
