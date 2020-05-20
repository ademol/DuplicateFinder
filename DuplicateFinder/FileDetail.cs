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
        }

        public string FileName { get; }

        private long FileSizeBackingField { get; set; }

        public long FileSize
        {
            get
            {
                if (FileSizeBackingField == 0)
                {
                    FileSizeBackingField = GetFileSize(FileName);
                }

                return FileSizeBackingField;
            }
            set => this.FileSizeBackingField = value;
        }

        public string Sha256LazyBackingField { get; set; }

        private string Sha256Lazy
        {
            get => Sha256LazyBackingField;
            set => Sha256LazyBackingField = value;
        }

        public string Sha256 => Sha256Lazy ??= (Sha256Lazy = _compareService.GetSha256(FileName));

        public bool HasDuplicates { get; set; }

        private long GetFileSize(string fileName)
        {
            return _fileSystem.FileInfo.FromFileName(fileName).Length;
        }
    }
}
