using System;

namespace DuplicateFinder
{
    public static class Output
    {
        private static Func<string, string> _func;

        private static readonly object ConsoleWriterLock = new object();

        public static string LastMessage { get; private set; }

        static Output()
        {
            _func = message => message;
        }

        public static void MessageOverride(Func<string, string> func)
        {
            lock (_func)
            {
                _func = func;
            }
        }

        public static void Write(string message)
        {
            lock (ConsoleWriterLock)
            {
                Console.WriteLine($"{_func.Invoke(message)}");
                LastMessage = message;
            }
        }
    }
}
