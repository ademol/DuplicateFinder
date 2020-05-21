using System;
using System.IO;
using System.Threading.Tasks;
using DuplicateFinder;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace DuplicateFinderTests
{
    public class FileWalkerTests
    {
        private readonly ICompareService _compareService;
        private readonly FileWalker _fileWalker;
        private readonly IConfigService _configService;
        private readonly IFileSystemService _fileSystemService;

        public FileWalkerTests()
        {
            _compareService = Substitute.For<ICompareService>();
            _configService = Substitute.For<IConfigService>();
            _fileSystemService = Substitute.For<IFileSystemService>();
            _fileWalker = new FileWalker(_compareService, _configService, _fileSystemService);
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

            _fileSystemService.GetDirectories(rootPath).Returns(rootSubDirs);
            _fileSystemService.GetDirectories(homePath).Returns(homeSubDirs);

            // When
            await _fileWalker.RecursePath(rootPath);

            // Then
            _fileSystemService.Received(1).GetDirectories(rootPath);
            _fileSystemService.Received(1).GetDirectories(homePath);
            _fileSystemService.Received(1).GetDirectories(rootPath);
            _fileSystemService.Received(1).GetDirectories(homePath);
        }

        [Fact]
        public async Task RecursePath_CallsGetFiles()
        {
            // Given
            const string rootPath = "/";
            const string homePath = "/home";
            const string userPath = "/home/user1";

            var rootSubDirs = new[] {homePath, "/etc", "/tmp"};
            var homeSubDirs = new[] {userPath};

            _fileSystemService.GetDirectories(rootPath).Returns(rootSubDirs);
            _fileSystemService.GetDirectories(homePath).Returns(homeSubDirs);

            const string rootFile = "/vmlinuz";
            const string userFileA = "/home/user1/.profile";
            const string userFileB = "/home/user1/Readme.txt";

            var filesInRootPath = new[] {rootFile};
            var filesInUserPath = new[] {userFileA, userFileB};

            _fileSystemService.GetFiles(rootPath).Returns(filesInRootPath);
            _fileSystemService.GetFiles(homePath).Returns(filesInUserPath);

            // When
            await _fileWalker.RecursePath(rootPath);

            // Then
            _fileSystemService.Received(1).GetFiles(rootPath);
            _fileSystemService.Received(1).GetFiles(homePath);
        }

        [Fact]
        public async Task RecursePath_MarkIfDuplicate()
        {
            // Given
            const string rootPath = "/";

            const string file1 = "/file1";
            const string file2 = "/file2";
            const string file3 = "/file3";

            var filesInRootPath = new[] {file1, file2, file3};

            _fileSystemService.GetFiles(rootPath).Returns(filesInRootPath);

            // When
            await _fileWalker.RecursePath(rootPath);

            // Then
            await _compareService.Received(1).MarkIfDuplicate(file1);
            await _compareService.Received(1).MarkIfDuplicate(file2);
            await _compareService.Received(1).MarkIfDuplicate(file3);
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
            _fileSystemService.Received(isSpecial ? 0 : 1).GetDirectories(Arg.Any<string>());
        }

        [Theory]
        [InlineData(".jpg", "/a.jpg", false)]
        [InlineData(".jpg", "/b.txt", true)]
        public async Task RecursePath_FilterFileByExtension(string extensionFilter, string file, bool filterFile)
        {
            // Given
            _fileSystemService.GetFiles("/").Returns(new[] {file});
            _configService.GetFilterExtension().Returns(new[] {extensionFilter});

            // When
            await _fileWalker.RecursePath("/");

            // Then
            await _compareService.Received(filterFile ? 0 : 1).MarkIfDuplicate(Arg.Is<string>(f => f.Equals(file)));
        }


        [Fact]
        public async Task RecursePath_GetDirectories_OutputException()
        {
            const string path = "/unAuthorizedPath";
            const string expectedOutput = "Get Dirs: Write was called";

            Output.MessageOverride(m => expectedOutput);

            // Given
            var expected = new UnauthorizedAccessException();
            _fileSystemService.GetDirectories(path).Throws(expected);

            var origOutWriter = Console.Out;

            await using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                // When
                await _fileWalker.RecursePath(path);

                var actual = sw.ToString().Replace("\n", "").Replace("\r", "");

                // Then
                Assert.True(actual.Equals(expectedOutput));
            }

            Console.SetOut(origOutWriter);
        }

        [Fact]
        public async Task RecursePath_GetFiles_OutputException()
        {
            const string path = "/unAuthorizedPath";

            const string expectedOutput = "GetFiles: Write was called";

            Output.MessageOverride(m => expectedOutput);

            // Given
            var expected = new UnauthorizedAccessException();
            _fileSystemService.GetFiles(path).Throws(expected);

            var origOutWriter = Console.Out;

            await using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                // When
                await _fileWalker.RecursePath(path);

                var actual = sw.ToString().Replace("\n", "").Replace("\r", "");

                // Then
                Assert.True(actual.Equals(expectedOutput));
            }

            Console.SetOut(origOutWriter);
        }
    }
}
