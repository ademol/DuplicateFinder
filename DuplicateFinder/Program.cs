using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;

namespace DuplicateFinder
{
    internal static class Program
    {
        private static readonly IOutput Output = DuplicateFinder.Output.Instance;

        private static ICompareService _compareService;
        private static IFileSystem _fileSystem;
        private static IConfigService _configService;

        private static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<IFileWalker, FileWalker>()
                .AddSingleton<IFileSystem, FileSystem>()
                .AddSingleton<ICompareService, CompareService>()
                .AddSingleton<IConfigService, ConfigService>()
                .AddSingleton<IOutput, Output>()
                .BuildServiceProvider();

            _compareService = serviceProvider.GetService<ICompareService>();
            _fileSystem = serviceProvider.GetService<IFileSystem>();
            _configService = serviceProvider.GetService<IConfigService>();


            if (args?.Length > 0)
            {
                await SearchSinglePath(args[0]);
            }
            else
            {
                await SearchAll();
            }

            DisplayDuplicates();
            Environment.Exit(0);
        }


        private static void DisplayDuplicates()
        {
            var fileDetails = _compareService.GetFilesWithDuplicates().ToList();
            fileDetails.Sort((a, b) => string.Compare(a.Sha256, b.Sha256, StringComparison.Ordinal));

            var currentSha = "";
            foreach (var fileDetail in fileDetails)
            {
                if (currentSha != fileDetail.Sha256)
                {
                    Output.Write("");
                    currentSha = fileDetail.Sha256;
                }

                const char s = '"';
                Output.Write($"{fileDetail.Sha256} : {s}{fileDetail.FileName}{s}");
            }
        }

        private static async Task SearchSinglePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Output.Write($"[{path}] does not exists");
                return;
            }

            Output.Write($"[{path}] Task Added");

            var fw = new FileWalker(_compareService, _fileSystem, _configService);

            await fw.RecursePath(path);
            Output.Write($"[{path}] Task Done");
        }

        private static async Task SearchAll()
        {
            var drives = Directory.GetLogicalDrives().ToList();

            var tasks = drives.Select(async drive => { await SearchSinglePath(drive); });
            await Task.WhenAll(tasks);
        }
    }
}
