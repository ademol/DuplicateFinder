using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    internal static class Program
    {
        private static readonly IOutput Output = DuplicateFinder.Output.Instance;

        private static void Main(string[] args)
        {
            if (args?.Count() > 0)
            {
                var path = args[0];
                SearchSinglePath(args[0]);
                DisplayDuplicates();
                Environment.Exit(0);
            }

            SearchAll();
            DisplayDuplicates();
        }

        private static void DisplayDuplicates()
        {
            var fileNames = CompareService.DuplicateFileHashes.Keys.ToList();
            fileNames.Sort();

            var output = new List<string>();
            foreach (var fileName in fileNames)
            {
                output.Add($"[{CompareService.DuplicateFileHashes[fileName]}] : [{fileName}]");
            }

            output.Sort();
            foreach (var s in output)
            {
                Output.Write(s);
            }
        }

        private static void SearchSinglePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Output.Write($"[{path}] does not exists");
                return;
            }
            
            Output.Write($"[{path}] Task Added");

            var fw = new FileWalker();
            fw.RecurseDirectories(path);
            Output.Write($"[{path}] Task Done");
        }

        private static void SearchAll()
        {
            var drives = FileWalker.EnumerateFileSystems();

            var tasks = new List<Task>();

            Parallel.ForEach(drives, SearchSinglePath);
        }
    }
}
