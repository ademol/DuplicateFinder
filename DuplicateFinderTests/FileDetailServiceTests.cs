using System.IO;
using System.Text;
using DuplicateFinder;
using NSubstitute;
using Xunit;

namespace DuplicateFinderTests
{
    public class FileDetailServiceTests
    {
        private readonly IFileSystemService _fileSystemService;

        public FileDetailServiceTests()
        {
            _fileSystemService = Substitute.For<IFileSystemService>();
        }


        [Fact]
        public void GetSha256()
        {
            // Given
            const string fileName = "fileName";
            var fileDetailService = new FileDetailService(_fileSystemService);

            const string fileContent = "bla";
            const string expectedSha256 = "4df3c3f68fcc83b27e9d42c90431a72499f17875c81a599b566c9889b9696703";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            _fileSystemService.GetStream(fileName).Returns(stream);

            // When
            var actualSha256 = fileDetailService.GetSha256(fileName).Result;

            // Then
            Assert.Equal(expectedSha256, actualSha256);
        }
    }
}
