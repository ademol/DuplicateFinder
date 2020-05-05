using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace DuplicateFinder
{
    public static class CompareService
    {
        private static readonly SHA256 Sha256 = SHA256.Create();

        public static readonly Dictionary<string, string> DuplicateFileHashes = new Dictionary<string, string>();

        private static readonly List<FileDetail> FileDetails = new List<FileDetail>();

        private static readonly IOutput Output = DuplicateFinder.Output.Instance;

        public static void AddFile(FileDetail file)
        {
            FileDetails.Add(file);
        }
        
        public static string GetSha256(string filename)
        {
            var bytes = GetHashSha256(filename);
            var hash = BytesToString(bytes);
            return hash;
        }

        private static IEnumerable<byte> GetHashSha256(string filename)
        {
            using var stream = File.OpenRead(filename);
            return Sha256.ComputeHash(stream);
        }

        private static string BytesToString(IEnumerable<byte> bytes)
        {
            return bytes.Aggregate("", (current, b) => current + b.ToString("x2"));
        }

        public static bool IsSame(FileDetail newFile)
        {
            var isSame = false;
            foreach (var file in FileDetails.Where(file => SizeMatches(file, newFile))
                .Where(file => Sha256Matches(file, newFile)))
            {
                Output.Write($"Collision: [{newFile.FileName}] [{file.FileName}]");
                isSame = true;
                DuplicateFileHashes.TryAdd(file.FileName, file.Sha256);
                DuplicateFileHashes.Add(newFile.FileName, newFile.Sha256);
            }

            return isSame;
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