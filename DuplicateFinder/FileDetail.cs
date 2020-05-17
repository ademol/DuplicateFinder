namespace DuplicateFinder
{
    public class FileDetail
    {
        private readonly ICompareService _compareService;

        public FileDetail(ICompareService compareService)
        {
            this._compareService = compareService;
        }

        public string FileName { get;}
        public long FileSize { get; }

        private string Sha256LazyBackStore { get; set; }

        private string Sha256Lazy
        {
            get => Sha256LazyBackStore;
            set
            {
                Sha256LazyBackStore = value;
                Sha256HasBeenDetermined = true;
            }
        }

        public string Sha256 => Sha256Lazy ??= (Sha256Lazy = _compareService.GetSha256(FileName));

        public bool HasDuplicates { get; set; }
        public bool Sha256HasBeenDetermined { get; private set; }

        public FileDetail(string fileName)
        {
            FileName = fileName;
            //FileSize = new System.IO.FileInfo(fileName).Length;
        }
    }
}
