using System.IO;
using System.IO.Abstractions;
using System.Text;
using DeepEqual.Syntax;
using DuplicateFinder;
using NSubstitute;
using Xunit;

namespace DuplicateFinderTests
{
    public class FileSystemServiceTests
    {
        private readonly IFileSystem _fileSystem;

        public FileSystemServiceTests()
        {
            _fileSystem = Substitute.For<IFileSystem>();
        }

        [Fact]
        public void GetGetLogicalDrives()
        {
            // Given
            var fileSystemService = new FileSystemService(_fileSystem);
            var expectedDirectories = new[] { "/","/tmp" };
            _fileSystem.Directory.GetLogicalDrives().Returns(expectedDirectories);

            // When
            var actual = fileSystemService.GetLogicalDrives();

            // Then
            actual.ShouldDeepEqual(expectedDirectories);
        }

        [Fact]
        public void DirectoryExists()
        {
            // Given
            var fileSystemService = new FileSystemService(_fileSystem);
            const string path1 = "/";
            const string path2 = "/nope";

            _fileSystem.Directory.Exists(path1).Returns(true);
            _fileSystem.Directory.Exists(path2).Returns(false);

            // When
            var actual1 = fileSystemService.DirectoryExists(path1);
            var actual2 = fileSystemService.DirectoryExists(path2);

            // Then
            Assert.True(actual1);
            Assert.False(actual2);
        }

        [Theory]
        [InlineData("/", new[] {"/tmp", "/etc", "/home"})]
        [InlineData("/tmp", null)]
        public void GetDirectories(string path, string[] expectedDirectories)
        {
            // Given
            var fileSystemService = new FileSystemService(_fileSystem);
            _fileSystem.Directory.GetDirectories(path).Returns(expectedDirectories);

            // When
            var actual = fileSystemService.GetDirectories(path);

            // Then
            actual.ShouldDeepEqual(expectedDirectories);
        }


        [Theory]
        [InlineData("/", new[] {"/vmlinuz"})]
        [InlineData("/tmp", null)]
        public void GetFiles(string path, string[] expectedFiles)
        {
            // Given
            var fileSystemService = new FileSystemService(_fileSystem);
            _fileSystem.Directory.GetFiles(path).Returns(expectedFiles);

            // When
            var actual = fileSystemService.GetFiles(path);

            // Then
            actual.ShouldDeepEqual(expectedFiles);
        }

        [Fact]
        public void GetStream()
        {
            // Given
            var sut = new FileSystemService(_fileSystem);

            const string file = "file.txt";
            const string fileContent = "file content here";

            var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            _fileSystem.File.OpenRead(file).Returns(expectedStream);

            // When
            var actual = sut.GetStream(file);

            // Then
            Assert.Equal(expectedStream, actual);
        }
    }
}
