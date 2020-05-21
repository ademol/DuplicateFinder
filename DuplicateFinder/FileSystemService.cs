using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;

namespace DuplicateFinder
{
    public interface IFileSystemService
    {
        long GetFileLength(string fileName);
        public IEnumerable<string> GetDirectories(string path);
        public IEnumerable<string> GetFiles(string path);
        public Stream GetStream(string filename);
        public IEnumerable<string> GetLogicalDrives();
        public bool DirectoryExists(string path);
    }

    public class FileSystemService : IFileSystemService
    {
        private readonly IFileSystem _fileSystem;

        public FileSystemService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        [ExcludeFromCodeCoverage]
        public long GetFileLength(string fileName)
        {
            return _fileSystem.FileInfo.FromFileName(fileName).Length;
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            return _fileSystem.Directory.GetDirectories(path);
        }

        public IEnumerable<string> GetLogicalDrives()
        {
            return _fileSystem.Directory.GetLogicalDrives();
        }

        public bool DirectoryExists(string path)
        {
            return _fileSystem.Directory.Exists(path);
        }

        public IEnumerable<string> GetFiles(string path)
        {
            return _fileSystem.Directory.GetFiles(path);
        }

        public Stream GetStream(string filename)
        {
            return _fileSystem.File.OpenRead(filename);
        }
    }
}
