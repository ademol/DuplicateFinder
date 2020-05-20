using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using DeepEqual.Syntax;
using DuplicateFinder;
using NSubstitute;
using Xunit;

namespace DuplicateFinderTests
{
    public class CompareServiceTest
    {
        private readonly IFileSystem _fileSystem;

        public CompareServiceTest()
        {
            _fileSystem = Substitute.For<IFileSystem>();
        }

        [Fact]
        public void AddFile()
        {
            // Given
            var compareService = new CompareService(_fileSystem);

            // When
            compareService.AddFile(new FileDetail(compareService, _fileSystem, "filename" ));
            // Then
            Assert.True(compareService.FileDetails.Count == 1);
        }


        [Fact]
        public void GetFilesWithDuplicates()
        {
            // Given
            var compareService = new CompareService(_fileSystem);

            var fileDetail1 = new FileDetail(compareService, _fileSystem, "filename1");
            var fileDetail2 = new FileDetail(compareService, _fileSystem, "filename2");
            var fileDetail3 = new FileDetail(compareService, _fileSystem, "filename3");

            fileDetail1.HasDuplicates = true;
            fileDetail2.HasDuplicates = false;
            fileDetail3.HasDuplicates = true;

            compareService.AddFile(fileDetail1);
            compareService.AddFile(fileDetail2);
            compareService.AddFile(fileDetail3);

            var expected = new List<FileDetail> {fileDetail1, fileDetail3};


            // When
            var actual = compareService.GetFilesWithDuplicates();


            // Then
            actual.ShouldDeepEqual(expected);
        }


        [Fact]
        public void GetSha256()
        {
            // Given
            const string server = "serverName";
            var compareService = new CompareService(_fileSystem);

            const string fileContent = "bla";
            const string expectedSha256 = "4df3c3f68fcc83b27e9d42c90431a72499f17875c81a599b566c9889b9696703";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            _fileSystem.File.OpenRead(server).Returns(stream);

            // When
            var actualSha256 =compareService.GetSha256(server);

            // Then
            Assert.Equal(expectedSha256, actualSha256);
        }

        [Fact]
        public void MarkIfDuplicate()
        {
            // Given
            var compareService = new CompareService(_fileSystem);
            const string file = "filename1";
            const string fileSameContent = "filename2";
            const string anotherFile = "filename3";


            var fileDetail1 = new FileDetail(compareService, _fileSystem, file);
            var fileDetail2 = new FileDetail(compareService, _fileSystem, fileSameContent);
            var fileDetail3 = new FileDetail(compareService, _fileSystem, anotherFile);

            fileDetail1.FileSize = 42;
            fileDetail2.FileSize = 42;
            fileDetail3.FileSize = 12;

            const string fileContent = "bla";
            const string otherContent = "something else";

            var streamContent = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            var sameStreamContent = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            var otherStreamContent = new MemoryStream(Encoding.UTF8.GetBytes(otherContent));
            _fileSystem.File.OpenRead(file).Returns(streamContent);
            _fileSystem.File.OpenRead(fileSameContent).Returns(sameStreamContent);
            _fileSystem.File.OpenRead(anotherFile).Returns(otherStreamContent);

            compareService.AddFile(fileDetail1);
            compareService.AddFile(fileDetail2);
            compareService.AddFile(fileDetail3);


            // When
            compareService.MarkIfDuplicate(fileDetail1);

            // Then
            Assert.True(fileDetail1.HasDuplicates);
            Assert.True(fileDetail2.HasDuplicates);
            Assert.True(! fileDetail3.HasDuplicates);
        }
    }
}
