using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    public interface ICompareService
    {
        public IEnumerable<FileDetail> GetFilesWithDuplicates();

        public Task MarkIfDuplicate(string filename);
    }

    public class CompareService : ICompareService
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IFileDetailService _fileDetailService;

        public readonly List<FileDetail> FileDetails = new List<FileDetail>();


        public CompareService(
            IFileSystemService fileSystemService,
            IFileDetailService fileDetailService
        )
        {
            _fileSystemService = fileSystemService;
            _fileDetailService = fileDetailService;
        }


        public IEnumerable<FileDetail> GetFilesWithDuplicates()
        {
            lock (FileDetails)
            {
                return FileDetails.Where(s => s.HasDuplicates).ToList();
            }
        }


        public Task MarkIfDuplicate(string newFile)
        {
            lock (FileDetails)
            {
                var newFileDetail = new FileDetail(_fileSystemService, _fileDetailService, newFile);

                foreach (var file in FileDetails
                    .Where(file => SizeMatches(file, newFileDetail))
                    .Where(file => Sha256Matches(file, newFileDetail)))
                {
                    newFileDetail.HasDuplicates = true;
                    file.HasDuplicates = true;
                    Output.Write($"[{Thread.CurrentThread.ManagedThreadId}] Collision: [{newFile}]");
                    break;
                }

                FileDetails.Add(newFileDetail);
            }

            return Task.CompletedTask;
        }

        private static bool Sha256Matches(FileDetail file, FileDetail newFile)
        {
            return file.Sha256Lazy == newFile.Sha256Lazy;
        }

        private static bool SizeMatches(FileDetail file, FileDetail newFile)
        {
            return file.FileSize == newFile.FileSize;
        }
    }
}
