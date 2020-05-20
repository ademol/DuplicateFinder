using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

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

        public readonly List<FileDetail> FileDetails = new List<FileDetail>();

        private static readonly IOutput Output = DuplicateFinder.Output.Instance;

        private static int sha256Count;

        public CompareService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void AddFile(FileDetail fileDetail)
        {
            lock (FileDetails)
            {
                FileDetails.Add(fileDetail);
            }
        }

        public IEnumerable<FileDetail> GetFilesWithDuplicates()
        {
            return FileDetails.Where(s => s.HasDuplicates).ToList();
        }

        public string GetSha256(string filename)
        {
            sha256Count++;
            var identifier = $"[{Thread.CurrentThread.ManagedThreadId}:{sha256Count}]";
            Output.Write($"{identifier} {filename}: determining SHA256");
            var bytes = GetHashSha256(filename);
            var hash = BytesToString(bytes);
            Output.Write($"{identifier} {filename} SHa256 => [{hash}]");
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
            List<FileDetail> listCopy;
            lock (FileDetails)
            {
                listCopy = FileDetails.ToList();
            }

            //Console.WriteLine($"checking {newFile.FileName} {newFile.FileSize} {newFile.Sha256LazyBackingField}");
            foreach (var file in listCopy
                .Where(file => SizeMatches(file, newFile))
                .Where(file => Sha256Matches(file, newFile)))
            {
                newFile.HasDuplicates = true;
                file.HasDuplicates = true;
                Output.Write(
                    $"Collision: [{newFile.FileName}] [{file.FileName}]  [{file.FileSize}][{file.Sha256}][{file.HasDuplicates}]");
            }
        }

        private static bool Sha256Matches(FileDetail file, FileDetail newFile)
        {
            return file.Sha256 == newFile.Sha256;
        }

        private static bool SizeMatches(FileDetail file, FileDetail newFile)
        {
            if (file.FileSize == newFile.FileSize)
            {
                Output.Write($"[{Thread.CurrentThread.ManagedThreadId}] Size matches for {newFile.FileName} => {file.FileName}  [{file.FileSize}]");
            }

            return file.FileSize == newFile.FileSize;
        }
    }
}
