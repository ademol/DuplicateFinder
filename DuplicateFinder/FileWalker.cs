using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    public interface IFileWalker
    {
        public Task RecursePath(string path);
    }

    public class FileWalker : IFileWalker
    {
        private readonly ICompareService _compareService;
        private readonly IConfigService _configService;
        private readonly IFileSystemService _fileSystemService;

        public FileWalker(ICompareService compareService,
            IConfigService configService,
            IFileSystemService fileSystemService)
        {
            _compareService = compareService;
            _configService = configService;
            _fileSystemService = fileSystemService;
        }


        public async Task RecursePath(string path)
        {
            if (IsSpecialPath(path))
            {
                Output.Write($"[{path}] special: skipping");
                return;
            }

            try
            {
                var subDirs = _fileSystemService.GetDirectories(path);
                foreach (var dir in subDirs)
                {
                    await RecursePath(dir);
                }
            }
            catch (Exception e)
            {
                Output.Write($"[{Thread.CurrentThread.ManagedThreadId}] {e.Message}");
            }

            try
            {
                var files = _fileSystemService.GetFiles(path).ToList();
                foreach (var file in files.Where(FilterByExtensions))
                {
                    await _compareService.MarkIfDuplicate(file);
                }
            }
            catch (Exception e)
            {
                Output.Write($"[{Thread.CurrentThread.ManagedThreadId}] {e.Message}");
            }
        }

        private static bool IsSpecialPath(string path)
        {
            var linuxSystemPaths = new List<string> {"/sys", "/proc", "/run", "/dev"};

            var specialPaths = linuxSystemPaths;

            return specialPaths.Contains(path);
        }

        private bool FilterByExtensions(string fileName)
        {
            var filterExtension = _configService.GetFilterExtension();

            var enumerable = filterExtension as string[] ?? filterExtension.ToArray();
            if (!enumerable.Any()) return true;

            var extension = Path.GetExtension(fileName).ToLower();

            return enumerable.Select(x => x.ToLower()).Contains(extension);
        }
    }
}
