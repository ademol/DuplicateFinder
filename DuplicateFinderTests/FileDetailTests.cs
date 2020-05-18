using System.IO.Abstractions;
using DuplicateFinder;
using NSubstitute;
using Xunit;

namespace DuplicateFinderTests
{
    public class FileDetailTests
    {
        private readonly IFileSystem _fileSystem;

        public FileDetailTests()
        {
            _fileSystem = Substitute.For<IFileSystem>();
        }

        [Fact]
        public void Sha256Lazy_SingleTimeCalculateSha()
        {
            // Given
            const string expected = "sha256string";
            const string fileName = "/a.txt";
            var compareService = Substitute.For<ICompareService>();

            compareService.GetSha256(fileName).Returns(expected);
            _fileSystem.FileInfo.FromFileName(fileName).Length.Returns(42);

            var fileDetail = new FileDetail(compareService, _fileSystem, fileName);

            // When
            var actual = fileDetail.Sha256;
            var actualAgain = fileDetail.Sha256;

            // Then
            Assert.True(actual.Equals(expected));
            Assert.True(actualAgain.Equals(expected));
            compareService.Received(1).GetSha256(fileName);
        }
    }
}
