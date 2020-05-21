using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DuplicateFinder;
using NSubstitute;
using Xunit;

namespace DuplicateFinderTests
{
    public class DuplicateFinderTests
    {
        private readonly ICompareService _compareService;
        private readonly IFileWalker _fileWalker;

        private readonly IFileSystemService _fileSystemService;
        private readonly IFileDetailService _fileDetailService;
        private readonly IDuplicateFinder  _duplicateFinder;
        private readonly IConfigService _configService;

        public DuplicateFinderTests()
        {
            _fileWalker = Substitute.For<IFileWalker>();
            _compareService = Substitute.For<ICompareService>();
            _fileSystemService = Substitute.For<IFileSystemService>();
            _fileDetailService = Substitute.For<IFileDetailService>();
            _configService = Substitute.For<IConfigService>();

            _duplicateFinder =
                new DuplicateFinder.DuplicateFinder(_compareService, _fileWalker, _fileSystemService, _configService);
        }

        [Fact]
        public async Task Execute()
        {
            // Given
            const string path = "/";
            var args = new [] {path};

            _fileSystemService.DirectoryExists(path).Returns(true);

            // When
            await _duplicateFinder.Execute(args);

            // Then
            _configService.Received(1).SetFilterExtension(Arg.Any<string[]>());
            await _fileWalker.Received(1).RecursePath(path);
        }

        [Fact]
        public async Task ExecuteAll()
        {
            // Given
            const string path1 = "/home";
            const string path2 = "/tmp";
            var paths = new[] { path1 ,path2 };

            _fileSystemService.GetLogicalDrives().Returns(paths);
            _fileSystemService.DirectoryExists(path1).Returns(true);
            _fileSystemService.DirectoryExists(path2).Returns(true);

            // When
            await _duplicateFinder.Execute(null);

            // Then
            _configService.Received(1).SetFilterExtension(Arg.Any<string[]>());
            await _fileWalker.Received(1).RecursePath(path1);
            await _fileWalker.Received(1).RecursePath(path2);
        }


        [Fact]
        public void DisplayDuplicates()
        {
            // Given
            const string file1 = "file1";
            const string file2 = "file2";
            const string file3 = "file3";

            var duplicates = new List<FileDetail>
            {
                new FileDetail(_fileSystemService, _fileDetailService, file1) {HasDuplicates = true},
                new FileDetail(_fileSystemService, _fileDetailService, file2) {HasDuplicates = false},
                new FileDetail(_fileSystemService, _fileDetailService, file3) {HasDuplicates = true}
            };

            _fileDetailService.GetSha256(file1).Returns("SHA");
            _fileDetailService.GetSha256(file2).Returns("OTHER SHA");
            _fileDetailService.GetSha256(file3).Returns("SHA");

            const string expectedOutput = @"SHA : ""file3""";
            _compareService.GetFilesWithDuplicates().Returns(duplicates);

            // When
            _duplicateFinder.DisplayDuplicates();
            var actual = Output.LastMessage;

            // Then
            Assert.True(actual.Equals(expectedOutput));
        }

        [Fact]
        public async Task SearchSinglePath()
        {
            // Given
             const string path = "/";

            _fileSystemService.DirectoryExists(path).Returns(true);
            // When
            await _duplicateFinder.SearchSinglePath(path);

            // Then
            await _fileWalker.Received(1).RecursePath(path);
        }

        [Fact]
        public async Task SearchSinglePath_NotExistingPath()
        {
            // Given
             const string path = "/does not exists";

            _fileSystemService.DirectoryExists(path).Returns(false);

            // When
            await _duplicateFinder.SearchSinglePath(path);

            // Then
            await _fileWalker.Received(0).RecursePath(path);
        }

        [Fact]
        public async Task SearchAll()
        {
            // Given
            const string path1 = "/";
            const string path2 = "/tmp";

            _compareService.MarkIfDuplicate(Arg.Any<string>()).Returns(Task.CompletedTask);
            _fileWalker.RecursePath(Arg.Any<string>()).Returns(Task.CompletedTask);

            _fileSystemService.GetLogicalDrives().Returns(new[] {path1, path2});

            _fileSystemService.DirectoryExists(path1).Returns(true);
            _fileSystemService.DirectoryExists(path2).Returns(true);

            // When
            await _duplicateFinder.SearchAll();

            // Then
            await _fileWalker.Received(1).RecursePath(path1);
            await _fileWalker.Received(1).RecursePath(path2);
        }
    }
}
