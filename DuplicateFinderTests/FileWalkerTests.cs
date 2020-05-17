using System.IO.Abstractions;
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


        // [Fact]
        // public void RecursePath()
        // {
        //     // Given
        //     const string rootPath = "/";
        //     const string homePath = "/home";
        //     const string userPath = "/home/user1";
        //
        //     var rootSubDirs = new[] {homePath, "/etc", "/tmp"};
        //     var homeSubDirs = new[] {userPath};
        //
        //     _fileSystem.Directory.GetDirectories(rootPath).Returns(rootSubDirs);
        //     _fileSystem.Directory.GetDirectories(homePath).Returns(homeSubDirs);
        //
        //     var filesInRootPath = new string[] {"/vmlinuz"};
        //     var filesInUserPath = new string[] {"/home/user1/.profile", "/home/user1/Readme.txt"};
        //
        //     _fileSystem.Directory.GetFiles(rootPath).Returns(filesInRootPath);
        //     _fileSystem.Directory.GetFiles(userPath).Returns(filesInUserPath);
        //
        //     // when
        //     var actual = RecursePath(rootPath);
        // }

        // [Fact]
        // public void RecursePath()
        // {
        //     // Given
        //     var rootPaths = new[] {"/home", "/tmp"};
        //     const string rootFile = "/vmlinuz";
        //     var rootFiles = new[] {rootFile};
        //     const string homeDir = "/home/user";
        //     var homeDirs = new[] {homeDir};
        //     const string userFileA = "/home/user/.profile";
        //     const string userFileB = "/home/user/userfile";
        //     var userHomeDirFiles = new[] {userFileA, userFileB};
        //
        //     _fileSystem.Directory.GetDirectories("/").Returns(rootPaths);
        //     _fileSystem.Directory.GetDirectories("/home").Returns(homeDirs);
        //
        //     _fileSystem.Directory.GetFiles("/").Returns(rootFiles);
        //     _fileSystem.Directory.GetFiles("/home/user").Returns(userHomeDirFiles);
        //
        //     _fileSystem.FileInfo.FromFileName(userFileA).Returns(new FileInfoWrapper(_fileSystem, new FileInfo(userFileA)));
        //     _fileSystem.FileInfo.FromFileName(userFileB).Returns(new FileInfoWrapper(_fileSystem, new FileInfo(userFileB)));
        //
        //     // When
        //     _fileWalker.RecurseDirectories("/");
        //
        //     // Then
        //
        //     _compareService.Received(1).AddFile(Arg.Is<FileDetail>(s => s.FileName.Equals(rootFile)));
        //     _compareService.Received(1).AddFile(Arg.Is<FileDetail>(s => s.FileName.Equals(userFileA)));
        //     _compareService.Received(1).AddFile(Arg.Is<FileDetail>(s => s.FileName.Equals(userFileB)));
        // }
    }
}
