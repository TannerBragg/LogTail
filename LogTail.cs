namespace LogReader
{
    using System;
    using System.IO;
    using System.Threading;

    public class LogTail : IDisposable
    {
        /// <summary>
        ///     Reads the lines from our fileStream
        /// </summary>
        private readonly StreamReader streamReader;

        /// <summary>
        ///     Constructor to use a file and seek to the end of that file to start reading the tail of a file.
        /// </summary>
        /// <param name="logPath">The path the the file you wish to open.</param>
        public LogTail(string logPath)
            : this(new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
        }

        /// <summary>
        ///     Constructor to use a fileStream and seek to the end of that file stream to start reading the tail of a file.
        /// </summary>
        /// <param name="fileStream"></param>
        public LogTail(FileStream fileStream)
        {
            // Seek to the end of the file
            fileStream.Seek(0, SeekOrigin.End);

            // Open the stream reader...
            this.streamReader = new StreamReader(fileStream);
        }

        /// <summary>
        ///     Event to fire a line string back to the main form class
        /// </summary>
        public event Action<string> OutputLine = delegate { };

        /// <summary>
        ///     Is the log file currently being read line by line
        /// </summary>
        public bool IsReading { get; private set; }

        /// <summary>
        ///     Dispose of the object.
        /// </summary>
        public void Dispose()
        {
            if (this.streamReader != null)
            {
                this.streamReader.Close();
                this.streamReader.Dispose();
            }
        }

        /// <summary>
        ///     Start the log tail.
        /// </summary>
        public void Start()
        {
            ThreadPool.QueueUserWorkItem(
                delegate
                    {
                        // If we are not currently reading...
                        if (!this.IsReading)
                        {
                            // Set the flag that we are reading
                            this.IsReading = true;

                            // While we are allowed to read...
                            while (this.IsReading)
                            {
                                while (!this.streamReader.EndOfStream && this.IsReading)
                                {
                                    this.OutputLine?.Invoke(this.streamReader.ReadLine());
                                }

                                while (this.streamReader.EndOfStream && this.IsReading)
                                {
                                    Thread.Sleep(100);
                                }
                            }
                        }
                    });
        }

        /// <summary>
        ///     Stop the log tail.
        /// </summary>
        public void Stop()
        {
            this.IsReading = false;
        }
    }
}