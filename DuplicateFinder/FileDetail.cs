namespace DuplicateFinder
{
    public class FileDetail
    {
        public string FileName { get;}
        public long FileSize { get; }

        private string Sha256Lazy { get; set; }

        public string Sha256 => Sha256Lazy ??= (Sha256Lazy = CompareService.GetSha256(FileName));

        public bool HasDuplicates { get; set; }
        
        public FileDetail(string fileName)
        {
            FileName = fileName;
            FileSize = new System.IO.FileInfo(fileName).Length;
        }
    }
}