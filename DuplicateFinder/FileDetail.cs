using System.IO.Abstractions;

namespace DuplicateFinder
{
    public class FileDetail
    {
        private readonly ICompareService _compareService;
        private readonly IFileSystem _fileSystem;

        public FileDetail(ICompareService compareService, IFileSystem fileSystem, string fileName)
        {
            _compareService = compareService;
            _fileSystem = fileSystem;
            FileName = fileName;
            FileSize = GetFileSize(fileName);
        }

        public string FileName { get; }
        public long FileSize { get; }

        private string Sha256LazyBackStore { get; set; }

        private string Sha256Lazy
        {
            get => Sha256LazyBackStore;
            set => Sha256LazyBackStore = value;
        }

        public string Sha256 => Sha256Lazy ??= (Sha256Lazy = _compareService.GetSha256(FileName));

        public bool HasDuplicates { get; set; }

        private long GetFileSize(string fileName)
        {
            return _fileSystem.FileInfo.FromFileName(fileName).Length;
        }
    }
}
