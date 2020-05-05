using System;
using System.IO;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    internal static class Program
    {
        private static readonly IOutput Output = DuplicateFinder.Output.Instance;

        private static void Main(string[] args)
        {
            if (args?.Length > 0)
            {
                SearchSinglePath(args[0]);
                DisplayDuplicates();
                Environment.Exit(0);
            }

            SearchAll();
            DisplayDuplicates();
        }

        private static void DisplayDuplicates()
        {
            var fileDetails = CompareService.GetFilesWithDuplicates();
            
            var currentSha = "";
            foreach (var fileDetail in fileDetails)
            {
                if (currentSha != fileDetail.Sha256)
                {
                    Output.Write("");
                    currentSha = fileDetail.Sha256;
                }

                Output.Write($"[{fileDetail.Sha256}] : [{fileDetail.FileName}]");
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
            Parallel.ForEach(drives, SearchSinglePath);
        }
    }
}
