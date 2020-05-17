using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Cryptography;

namespace DuplicateFinder
{
    public interface ICompareService
    {
        public void AddFile(FileDetail fileDetail);
        public IEnumerable<FileDetail> GetFilesWithDuplicates();

        public string GetSha256(string filename);

        public void MarkIfDuplicate(FileDetail newFile);
    }

    public class CompareService : ICompareService
    {
        private readonly IFileSystem _fileSystem;
        private static readonly SHA256 Sha256 = SHA256.Create();

        private static readonly List<FileDetail> FileDetails = new List<FileDetail>();

        private static readonly IOutput Output = DuplicateFinder.Output.Instance;

        public CompareService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void AddFile(FileDetail fileDetail)
        {
                FileDetails.Add(fileDetail);
        }

        public IEnumerable<FileDetail> GetFilesWithDuplicates()
        {
            return FileDetails.Where(s => s.HasDuplicates).ToList();
        }

        public string GetSha256(string filename)
        {
            var bytes = GetHashSha256(filename);
            var hash = BytesToString(bytes);
            return hash;
        }

        private IEnumerable<byte> GetHashSha256(string filename)
        {
            using var stream = _fileSystem.File.OpenRead(filename);
            return Sha256.ComputeHash(stream);
        }

        private static string BytesToString(IEnumerable<byte> bytes)
        {
            return bytes.Aggregate("", (current, b) => current + b.ToString("x2"));
        }

        public void MarkIfDuplicate(FileDetail newFile)
        {
            foreach (var file in FileDetails
                .Where(file => SizeMatches(file, newFile))
                .Where(file => Sha256Matches(file, newFile)))
            {
                Output.Write($"Collision: [{newFile.FileName}] [{file.FileName}]");
                newFile.HasDuplicates = true;
                file.HasDuplicates = true;
            }
        }

        private static bool Sha256Matches(FileDetail file, FileDetail newFile)
        {
            return file.Sha256 == newFile.Sha256;
        }

        private static bool SizeMatches(FileDetail file, FileDetail newFile)
        {
            return file.FileSize == newFile.FileSize;
        }
    }
}
