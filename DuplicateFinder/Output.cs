using System;
using System.Threading;

namespace DuplicateFinder
{
    public interface IOutput
    {
        public void Write(string message);
    }

    public sealed class Output : IOutput
    {
        private static readonly Lazy<Output> Lazy = new Lazy<Output>(() => new Output());

        public static Output Instance => Lazy.Value;

        private static readonly object ConsoleWriterLock = new object();

        private Output()
        {
        }

        public void Write(string message)
        {
            lock (ConsoleWriterLock)
            {
                Console.WriteLine($"{message}");
            }
        }
    }
}
