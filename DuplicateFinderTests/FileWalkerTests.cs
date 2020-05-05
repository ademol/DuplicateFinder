using DuplicateFinder;
using Xunit;

namespace DuplicateFinderTests
{
    public class FileWalkerTests
    {
        [Fact]
        public void EnumerateFileSystems()
        {
            // Given
            // When
            var drives = FileWalker.EnumerateFileSystems();
            
            // Then
            Assert.NotNull(drives);
        }


        [Fact]
        public void EnumerateDirectories()
        {
            // Given
            var fileWalker = new FileWalker();
            
            // When
            var directories = FileWalker.EnumerateDirectories("C:\\");
            
            // Then
            Assert.NotNull(directories);
        }

        [Fact]
        public void EnumerateFiles()
        {
            // Given
            var fileWalker = new FileWalker();
            
            // When
            var files = FileWalker.EnumerateFiles("C:\\");
            
            // Then
            Assert.NotNull(files);
        }
    }
}