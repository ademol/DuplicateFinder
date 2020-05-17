namespace DuplicateFinder
{
    public class FileDetail
    {
        private readonly ICompareService _compareService;

        public FileDetail(ICompareService compareService, string fileName)
        {
            _compareService = compareService;
            FileName = fileName;
            FileSize = GetFileSize(fileName);
        }

        public string FileName { get;}
        public long FileSize { get; }

        private string Sha256LazyBackStore { get; set; }

        private string Sha256Lazy
        {
            get => Sha256LazyBackStore;
            set => Sha256LazyBackStore = value;
            //Sha256HasBeenDetermined = true;
        }

        public string Sha256 => Sha256Lazy ??= (Sha256Lazy = _compareService.GetSha256(FileName));

        public bool HasDuplicates { get; set; }
        //private bool Sha256HasBeenDetermined { get; set; }

        private static long GetFileSize(string fileName)
        {
           return new System.IO.FileInfo(fileName).Length;
        }
    }
}
