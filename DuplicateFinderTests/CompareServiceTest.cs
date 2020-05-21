using System.Linq;
using DuplicateFinder;
using NSubstitute;
using Xunit;

namespace DuplicateFinderTests
{
    public class CompareServiceTest
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IFileDetailService _fileDetailService;

        public CompareServiceTest()
        {
            _fileSystemService = Substitute.For<IFileSystemService>();
            _fileDetailService = Substitute.For<IFileDetailService>();
        }


        [Fact]
        public void GetFilesWithDuplicates()
        {
            // Given
            var compareService = new CompareService(_fileSystemService, _fileDetailService);
            const string file = "filename1";
            const string fileSameContent = "filename2";
            const string fileOtherContent = "filename3";
            const string sameSha = "same Sha256 value";
            const string differentSha = "different Sha256 value";

            _fileDetailService.GetSha256(file).Returns(sameSha);
            _fileDetailService.GetSha256(fileSameContent).Returns(sameSha);
            _fileDetailService.GetSha256(fileOtherContent).Returns(differentSha);

            compareService.MarkIfDuplicate(file);
            compareService.MarkIfDuplicate(fileSameContent);
            compareService.MarkIfDuplicate(fileOtherContent);

            // When
            var fileDetails = compareService.GetFilesWithDuplicates().ToList();

            // Then
            Assert.True(compareService.FileDetails.Count == 3);
            Assert.True(fileDetails.Count(f => f.HasDuplicates) == 2);
        }


        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, false, false)]
        [InlineData(false, false, false)]
        [InlineData(false, true, false)] // note: impossible, file with different length cannot have same Sha256
        public void MarkIfDuplicate(bool hasSameFileLength, bool hasSameSha256, bool expected)
        {
            // Given
            const string fileA = "check against this file";
            const string fileB = "file to check";
            const long length = 42;
            const long differentLength = 12;
            const string sha256 = "same Sha256 value";
            const string differentSha256 = "different Sha256 value";
            var compareService = new CompareService(_fileSystemService, _fileDetailService);

            _fileSystemService.GetFileLength(fileA).Returns(length);
            _fileSystemService.GetFileLength(fileB).Returns(hasSameFileLength ? length : differentLength);

            _fileDetailService.GetSha256(fileA).Returns(sha256);
            _fileDetailService.GetSha256(fileB).Returns(hasSameSha256 ? sha256 : differentSha256);

            // When
            compareService.MarkIfDuplicate(fileA);
            compareService.MarkIfDuplicate(fileB);
            var actual = compareService.FileDetails.All(fd => fd.HasDuplicates);

            // Then
            Assert.Equal(expected, actual);
        }
    }
}
