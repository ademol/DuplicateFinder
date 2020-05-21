using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;

namespace DuplicateFinder
{
    internal static class Program
    {
        private static ICompareService _compareService;
        private static IConfigService _configService;
        private static IFileWalker _fileWalker;
        private static IFileSystemService _fileSystemService;

        [ExcludeFromCodeCoverage]
        private static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<IFileWalker, FileWalker>()
                .AddSingleton<ICompareService, CompareService>()
                .AddTransient<IFileSystemService, FileSystemService>()
                .AddTransient<IFileDetailService, FileDetailService>()
                .AddSingleton<IFileSystem, FileSystem>()
                .AddSingleton<IConfigService, ConfigService>()
                .AddSingleton<IDuplicateFinder, DuplicateFinder>()
                .BuildServiceProvider();

            _configService = serviceProvider.GetService<IConfigService>();
            _fileWalker = serviceProvider.GetService<IFileWalker>();
            _compareService = serviceProvider.GetService<ICompareService>();
            _fileSystemService = serviceProvider.GetService<IFileSystemService>();

            var duplicateFinder = new DuplicateFinder(_compareService, _fileWalker, _fileSystemService, _configService);

            await duplicateFinder.Execute(args);

            Environment.Exit(0);
        }
    }
}
