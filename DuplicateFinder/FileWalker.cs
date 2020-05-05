using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuplicateFinder
{
    public class FileWalker
    {
        private readonly IOutput _output = Output.Instance;

        public static List<string> EnumerateFileSystems()
        {
            return Directory.GetLogicalDrives().ToList();
        }


        public static IEnumerable<string> EnumerateDirectories(string path)
        {
            IEnumerable<string> result = new List<string>();
            try
            {
                result = Directory.EnumerateDirectories(path);
            }
            catch (Exception e)
            {
                Output.Instance.Write($"[{e.Message}");
            }

            return result.ToList();
        }

        public static IEnumerable<string> EnumerateFiles(string path)
        {
            IEnumerable<string> result = new List<string>();
            try
            {
                result = Directory.EnumerateFiles(path);
            }
            catch (Exception e)
            {
                Output.Instance.Write($"[{e.Message}");
            }

            return result.ToList();
        }

        private static bool IsSpecialPath(string path)
        {
            var linuxSystemPaths = new List<string> {"/sys", "/proc", "/run", "/dev"};

            var specialPaths = linuxSystemPaths;

            return specialPaths.Contains(path);
        }

        private static bool FilterByExtensions(string fileName)
        {
            var extensionsToFilterOut = new List<string>
            {
                ".mp4", ".jpg", ".mp3", ".doc", ".webm", ".odg"
            };

            var extension = Path.GetExtension(fileName).ToLower();

            return extensionsToFilterOut.Select(x => x.ToLower()).Contains(extension);
        }


        public void RecurseDirectories(string path)
        {
            if (IsSpecialPath(path))
            {
                _output.Write($"[{path}] special: skipping");
                return;
            }

            var subDirs = EnumerateDirectories(path);
            foreach (var dir in subDirs)
            {
                RecurseDirectories(dir);
            }

            var files = EnumerateFiles(path);
            foreach (var file in files)
            {
                if (!FilterByExtensions(file))
                {
                    continue;
                }

                var fileInfo = new FileInfo(file);

                if (fileInfo.Length == 0) continue;

                try
                {
                    var fd = new FileDetail(file);
                    CompareService.CheckForDuplicates(fd);
                    CompareService.AddFile(fd);
                }
                catch (Exception e)
                {
                    _output.Write(e.Message);
                }
            }
        }
    }
}