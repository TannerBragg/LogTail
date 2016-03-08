using System;
using System.IO;
using System.Timers;

namespace LogReader
{
    public class LogTail
    {
        /// <summary>
        /// Delegate containing a line string
        /// </summary>
        /// <param name="line"></param>
        public delegate void OutputLineHandler(string line);

        /// <summary>
        /// Opens our file
        /// </summary>
        private readonly FileStream fileStream;

        /// <summary>
        /// Timer to read the log file.
        /// </summary>
        private readonly Timer readTimer;

        /// <summary>
        /// Reads the lines from our fileStream
        /// </summary>
        private readonly StreamReader streamReader;

        /// <summary>
        /// Constructor to open a filestream with read access to a file while allowing the log to still be written
        /// </summary>
        /// <param name="logPath"></param>
        public LogTail(string logPath)
        {
            try
            {
                this.readTimer = new Timer(250);

                // Open the file stream...
                this.fileStream = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Seek to the end of the file
                this.fileStream.Seek(0, SeekOrigin.End);

                // Open the stream reader...
                this.streamReader = new StreamReader(this.fileStream);

                // Get to the end of the log file... (Old way of seeking the end of the file)
                //this.streamReader.ReadToEnd();

                this.IsReading = false;

                this.readTimer.Elapsed += this.ReadTimerElapsed;
                this.readTimer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Is the log file currently being read line by line
        /// </summary>
        private bool IsReading { get; set; }

        /// <summary>
        /// Read the log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // If we are already reading the lines in the file...
            if (this.IsReading)
            {
                // Return so we don't double read.
                return;
            }

            string line = string.Empty;

            // While we are not at the end of the file...
            while ((line = this.streamReader.ReadLine()) != null)
            {
                // Send the line back to the main form
                this.OutputLine(line);
            }

            // We're done reading.  So set the flag so we can read again.
            this.IsReading = false;
        }

        /// <summary>
        /// Event to fire a line string back to the main form class
        /// </summary>
        public event OutputLineHandler OutputLine;
    }
}