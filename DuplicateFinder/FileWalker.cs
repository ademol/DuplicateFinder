using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace DuplicateFinder
{
    public interface IFileWalker
    {
        //public void RecurseDirectories(string path);
        public string[] GetDirectories(string path);
    }

    public class FileWalker : IFileWalker
    {
        private readonly ICompareService _compareService;
        private readonly IFileSystem _fileSystem;
        private readonly IOutput _output = Output.Instance;

        public FileWalker(ICompareService compareService, IFileSystem fileSystem)
        {
            _compareService = compareService;
            _fileSystem = fileSystem;
        }


        public string[] GetDirectories(string path)
        {
            return _fileSystem.Directory.GetDirectories(path);
        }

        public string[] GetFiles(string path)
        {
            return _fileSystem.Directory.GetFiles(path);
        }



//
//
//
//
//
//
//         private static bool IsSpecialPath(string path)
//         {
//             var linuxSystemPaths = new List<string> {"/sys", "/proc", "/run", "/dev"};
//
//             var specialPaths = linuxSystemPaths;
//
//             return specialPaths.Contains(path);
//         }
//
//         private static bool FilterByExtensions(string fileName)
//         {
//             var extensionsToFilterOut = new List<string>
//             {
//                 ".mp4", ".jpg", ".mp3", ".doc", ".webm", ".odg"
//             };
//
//             var extension = Path.GetExtension(fileName).ToLower();
//
//             return extensionsToFilterOut.Select(x => x.ToLower()).Contains(extension);
//         }
//
//
//         public void RecurseDirectories(string path)
//         {
//             Console.WriteLine(path);
//             if (IsSpecialPath(path))
//             {
//                 _output.Write($"[{path}] special: skipping");
//                 return;
//             }
//
//             try
//             {
//                 var subDirs = _fileSystem.Directory.GetDirectories(path);
//                 foreach (var dir in subDirs)
//                 {
//                     RecurseDirectories(dir);
//                 }
//             }
//             catch (Exception e)
//             {
//                 _output.Write(e.Message);
//             }
//
//             var files = new List<string>();
//             try {
//                  files = _fileSystem.Directory.GetFiles(path).ToList();
//             }
//             catch (Exception e)
//             {
//                 _output.Write(e.Message);
//             }
//
//             foreach (var file in files)
//             {
//                 // if (!FilterByExtensions(file))
//                 // {
//                 //     continue;
//                 // }
//
// //                var fileInfo = new FileInfo(file);
//                 var fileInfo = _fileSystem.FileInfo.FromFileName(file);
//
//                 try
//                 {
//               //      if (fileInfo.Length == 0) continue;
//
//                     var fd = new FileDetail(file);
//                     _compareService.CheckForDuplicates(fd);
//                     _compareService.AddFile(fd);
//                 }
//                 catch (Exception e)
//                 {
//                     _output.Write(e.Message);
//                 }
//             }
//         }

    }
}
