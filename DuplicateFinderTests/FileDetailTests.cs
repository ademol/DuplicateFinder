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
        public void Sha256Lazy_SingleTimeCalculate()
        {
            // Given
            const string expected = "sha256string";
            const string fileName = "/a.txt";
            var fileDetailService = Substitute.For<IFileDetailService>();
            var fileSystemService = Substitute.For<IFileSystemService>();

            fileDetailService.GetSha256(fileName).Returns(expected);
            _fileSystem.FileInfo.FromFileName(fileName).Length.Returns(42);

            var fileDetail = new FileDetail(fileSystemService, fileDetailService, fileName);

            // When
            var actual = fileDetail.Sha256Lazy;
            var actualAgain = fileDetail.Sha256Lazy;

            // Then
            Assert.True(actual.Equals(expected));
            Assert.True(actualAgain.Equals(expected));
            fileDetailService.Received(1).GetSha256(fileName);
        }

        [Fact]
        public void FileSizeBackingField_SingleTimeCalculate()
        {
            const long expected = 42;
            const string fileName = "/a.txt";
            var fileDetailService = Substitute.For<IFileDetailService>();
            var fileSystemService = Substitute.For<IFileSystemService>();

            fileSystemService.GetFileLength(fileName).Returns(expected);
            _fileSystem.FileInfo.FromFileName(fileName).Length.Returns(42);

            var fileDetail = new FileDetail(fileSystemService, fileDetailService, fileName);

            // When
            var actual = fileDetail.FileSize;
            var actualAgain = fileDetail.FileSize;

            // Then
            Assert.True(actual.Equals(expected));
            Assert.True(actualAgain.Equals(expected));
            fileSystemService.Received(1).GetFileLength(fileName);
        }
    }
}
