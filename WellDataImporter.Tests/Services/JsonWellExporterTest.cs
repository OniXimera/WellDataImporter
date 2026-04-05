using Moq;
using WellDataImporter.Models;
using WellDataImporter.Services;

namespace WellDataImporter.Tests.Services
{
    public class JsonWellExporterTest
    {
        public JsonWellExporterTest()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _service = new JsonWellExporter(_fileSystemMock.Object);
        }

        [Fact]
        public async Task ExportAsync_EmptySummaries_WritesEmptyArray()
        {
            var mockStream = new MemoryStream();
            _fileSystemMock.Setup(f => f.Create(It.IsAny<string>())).Returns(mockStream);

            await _service.ExportAsync(Enumerable.Empty<WellSummary>(), "empty.json");

            _fileSystemMock.Verify(f => f.Create("empty.json"), Times.Once);
            var bytes = mockStream.ToArray();
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            Assert.Contains("[]", json.Trim());
        }

        private readonly Mock<IFileSystem> _fileSystemMock;
        private readonly JsonWellExporter _service;
    }
}
