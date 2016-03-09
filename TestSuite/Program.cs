namespace TestSuite
{
    using System;
    using System.IO;
    using System.Threading;

    using LogReader;

    /// <summary>
    /// The main Program.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The file name.
        /// </summary>
        private const string FileName = "testLog.txt";

        /// <summary>
        /// The new line.
        /// </summary>
        private const string NewLine = "This is a new line! ";

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (!File.Exists(FileName))
            {
                File.Create(FileName);
            }

            var tail = new LogTail(FileName);
            tail.Start();
            ThreadPool.QueueUserWorkItem(
                delegate
                    {
                        long x = 0;

                        while (true)
                        {
                            using (var sw = File.AppendText(FileName))
                            {
                                sw.WriteLine(NewLine + x);
                                x++;
                            }
                        }
                    });

            tail.OutputLine += Console.WriteLine;

            Console.ReadLine();
        }
    }
}