using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    public interface IDuplicateFinder
    {
        public void DisplayDuplicates();
        public Task SearchSinglePath(string path);
        public Task SearchAll();
        public Task Execute(string[] args);
    }

    public class DuplicateFinder : IDuplicateFinder
    {
        private readonly ICompareService _compareService;
        private readonly IFileWalker _fileWalker;
        private readonly IFileSystemService _fileSystemServices;
        private readonly IConfigService _configService;

        public DuplicateFinder(ICompareService compareService, IFileWalker fileWalker, IFileSystemService fileSystemServices, IConfigService configService)
        {
            _compareService = compareService;
            _fileWalker = fileWalker;
            _fileSystemServices = fileSystemServices;
            _configService = configService;
        }

        public async Task Execute(string[] args)
        {
            _configService.SetFilterExtension(new[] {".jpg", ".mp3" });

            if (args?.Length > 0)
            {
                await SearchSinglePath(args[0]);
            }
            else
            {
                await SearchAll();
            }

            DisplayDuplicates();
        }

        public void DisplayDuplicates()
        {
            var fileDetails = _compareService.GetFilesWithDuplicates().ToList();
            fileDetails.Sort((a, b) => string.Compare(a.Sha256Lazy, b.Sha256Lazy, StringComparison.Ordinal));

            var currentSha = "";
            foreach (var fileDetail in fileDetails)
            {
                if (currentSha != fileDetail.Sha256Lazy)
                {
                    Output.Write("");
                    currentSha = fileDetail.Sha256Lazy;
                }

                const char s = '"';
                Output.Write($"{fileDetail.Sha256Lazy} : {s}{fileDetail.FileName}{s}");
            }
        }

        public async Task SearchSinglePath(string path)
        {
            if (!_fileSystemServices.DirectoryExists(path))
            {
                Output.Write($"[{path}] does not exists");
                return;
            }

            Output.Write($"[{path}] Task Added on thread {Thread.CurrentThread.ManagedThreadId}");

            await _fileWalker.RecursePath(path);
            Output.Write($"[{path}] Task Done");
        }

        public Task SearchAll()
        {
            var drives = _fileSystemServices.GetLogicalDrives().ToList();

            var taskList = new List<Task>();
            Parallel.ForEach(drives, (drive) => { taskList.Add(SearchSinglePath(drive)); });

            Task.WaitAll(taskList.ToArray());
            return Task.FromResult(Task.CompletedTask);
        }
    }
}
