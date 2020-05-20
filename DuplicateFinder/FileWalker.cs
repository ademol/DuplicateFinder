using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    public interface IFileWalker
    {
        public Task RecursePath(string path);
        public string[] GetDirectories(string path);
        public string[] GetFiles(string path);
    }

    public class FileWalker : IFileWalker
    {
        private readonly ICompareService _compareService;
        private readonly IFileSystem _fileSystem;
        private readonly IOutput _output = Output.Instance;
        private readonly IConfigService _configService;

        public FileWalker(ICompareService compareService, IFileSystem fileSystem, IConfigService configService)
        {
            _compareService = compareService;
            _fileSystem = fileSystem;
            _configService = configService;
        }

        public string[] GetDirectories(string path)
        {
            return _fileSystem.Directory.GetDirectories(path);
        }

        public string[] GetFiles(string path)
        {
            return _fileSystem.Directory.GetFiles(path);
        }

        public async Task RecursePath(string path)
        {
            if (IsSpecialPath(path))
            {
                _output.Write($"[{path}] special: skipping");
                return;
            }

            try
            {
                var subDirs = _fileSystem.Directory.GetDirectories(path);
                foreach (var dir in subDirs)
                {
                    await RecursePath(dir);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                //_output.Write(e.Message
            }
            catch (Exception e)
            {
                _output.Write(e.Message + e.StackTrace);
            }

            var files = new List<string>();
            try
            {
                files = _fileSystem.Directory.GetFiles(path)?.ToList();
            }
            catch (Exception e)
            {
               // _output.Write(e.Message);
            }


            foreach (var file in files)
            {
                if (!FilterByExtensions(file))
                {
                    continue;
                }

                var fileDetail = new FileDetail(_compareService, _fileSystem, file);
                _compareService.MarkIfDuplicate(fileDetail);
                _compareService.AddFile(fileDetail);
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
