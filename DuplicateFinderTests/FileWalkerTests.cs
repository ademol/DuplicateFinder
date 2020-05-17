using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using DuplicateFinder;
using NSubstitute;
using Xunit;

namespace DuplicateFinderTests
{
    public class FileWalkerTests
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICompareService _compareService;
        private readonly FileWalker _fileWalker;

        public FileWalkerTests()
        {
            _compareService = Substitute.For<ICompareService>();
            _fileSystem = Substitute.For<IFileSystem>();
            _fileWalker = new FileWalker(_compareService, _fileSystem);
        }


        [Theory]
        [InlineData("/", new [] { "/tmp", "/etc", "/home"})]
        [InlineData("/tmp", null)]
        public void GetDirectories(string path, string[] expectedDirectories)
        {
            // Given
            _fileSystem.Directory.GetDirectories(path).Returns(expectedDirectories);

            // When
           var actual = _fileWalker.GetDirectories(path);

            // Then
            actual.ShouldDeepEqual(expectedDirectories);
        }

        [Theory]
        [InlineData("/", new [] { "/vmlinuz"})]
        [InlineData("/tmp", null)]
        public void GetFiles(string path, string[] expectedFiles)
        {
            // Given
            _fileSystem.Directory.GetFiles(path).Returns(expectedFiles);

            // When
            var actual = _fileWalker.GetFiles(path);

            // Then
            actual.ShouldDeepEqual(expectedFiles);
        }


        [Fact]
        public async Task RecursePath_Directories()
        {
            // Given
            const string rootPath = "/";
            const string homePath = "/home";
            const string userPath = "/home/user1";

            var rootSubDirs = new[] {homePath, "/etc", "/tmp"};
            var homeSubDirs = new[] {userPath};

            _fileSystem.Directory.GetDirectories(rootPath).Returns(rootSubDirs);
            _fileSystem.Directory.GetDirectories(homePath).Returns(homeSubDirs);

            // When
             await _fileWalker.RecursePath(rootPath);

             // Then
             _fileSystem.Directory.Received(1).GetDirectories(rootPath);
             _fileSystem.Directory.Received(1).GetDirectories(homePath);
             _fileSystem.Directory.Received(1).GetDirectories(rootPath);
             _fileSystem.Directory.Received(1).GetDirectories(homePath);
        }

        [Fact]
        public async Task RecursePath_Files()
        {
            // Given
            const string rootPath = "/";
            const string homePath = "/home";
            const string userPath = "/home/user1";

            var rootSubDirs = new[] {homePath, "/etc", "/tmp"};
            var homeSubDirs = new[] {userPath};

            _fileSystem.Directory.GetDirectories(rootPath).Returns(rootSubDirs);
            _fileSystem.Directory.GetDirectories(homePath).Returns(homeSubDirs);

            const string rootFile = "/vmlinuz";
            const string userFileA = "/home/user1/.profile";
            const string userFileB = "/home/user1/Readme.txt";

            var filesInRootPath = new string[] {rootFile};
            var filesInUserPath = new string[] {userFileA, userFileB};

            _fileSystem.Directory.GetFiles(rootPath).Returns(filesInRootPath);
            _fileSystem.Directory.GetFiles(homePath).Returns(filesInUserPath);

            // When
            await _fileWalker.RecursePath(rootPath);

            // Then
            _fileSystem.Directory.Received(1).GetFiles(rootPath);
            _fileSystem.Directory.Received(1).GetFiles(homePath);
        }

        [Fact]
        public async Task RecursePath_AddFile()
        {
            // Given
            const string rootPath = "/";

            const string file1 = "/file1";
            const string file2 = "/file2";
            const string file3 = "/file3";

            var filesInRootPath = new string[] {file1, file2, file3};

            _fileSystem.Directory.GetFiles(rootPath).Returns(filesInRootPath);

            _fileSystem.FileInfo.FromFileName(file1).Returns(new FileInfoWrapper(_fileSystem, new FileInfo(file1)));
            _fileSystem.FileInfo.FromFileName(file2).Returns(new FileInfoWrapper(_fileSystem, new FileInfo(file2)));
            _fileSystem.FileInfo.FromFileName(file3).Returns(new FileInfoWrapper(_fileSystem, new FileInfo(file3)));

            // When
            await _fileWalker.RecursePath(rootPath);

            // Then
            _compareService.Received(1).AddFile(Arg.Is<FileDetail>(detail => detail.FileName.Equals(file1)));
            _compareService.Received(1).AddFile(Arg.Is<FileDetail>(detail => detail.FileName.Equals(file2)));
            _compareService.Received(1).AddFile(Arg.Is<FileDetail>(detail => detail.FileName.Equals(file3)));
        }

       [Theory]
       [InlineData("/proc", true)]
       [InlineData("/home", false)]
       public async Task RecursePath_SkipSpecialDirectories(string path, bool isSpecial)
        {
            // Given
            // When
            await _fileWalker.RecursePath(path);

            // Then
            _fileSystem.Directory.Received(isSpecial ? 0 : 1).GetDirectories(Arg.Any<string>());
        }
    }
}

