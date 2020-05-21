namespace DuplicateFinder
{
    public class FileDetail
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IFileDetailService _fileDetailService;

        public FileDetail(
            IFileSystemService fileSystemService,
            IFileDetailService fileDetailService,
            string fileName)
        {
            FileName = fileName;
            _fileDetailService = fileDetailService;
            _fileSystemService = fileSystemService;
        }

        public string FileName { get; }

        private long FileSizeBackingField { get; set; }

        public long FileSize
        {
            get
            {
                if (FileSizeBackingField == 0)
                {
                    FileSizeBackingField = _fileSystemService.GetFileLength(FileName);
                }

                return FileSizeBackingField;
            }
        }

        private string Sha256LazyBackingField { get; set; }

        public string Sha256Lazy
        {
            get
            {
                Sha256LazyBackingField ??= _fileDetailService.GetSha256(FileName).Result;
                return Sha256LazyBackingField;
            }
        }

        public bool HasDuplicates { get; set; }
    }
}
