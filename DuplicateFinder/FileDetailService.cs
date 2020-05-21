using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    public interface IFileDetailService
    {
        public Task<string> GetSha256(string filename);
    }

    public class FileDetailService : IFileDetailService
    {
        private readonly IFileSystemService _fileSystemService;

        public FileDetailService(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        public Task<string> GetSha256(string filename)
        {
            var bytes = GetHashSha256(filename);
            var hash = BytesToString(bytes);
            return Task.FromResult(hash);
        }

        private IEnumerable<byte> GetHashSha256(string filename)
        {
            using var stream = _fileSystemService.GetStream(filename);
            return SHA256.Create().ComputeHash(stream);
        }

        private static string BytesToString(IEnumerable<byte> bytes)
        {
            return bytes.Aggregate("", (current, b) => current + b.ToString("x2"));
        }
    }
}
