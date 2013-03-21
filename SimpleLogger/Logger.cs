﻿using System;
using System.IO;
using System.Windows.Forms;

namespace SimpleLogger
{
    /// <summary>
    /// This is an extremely simple logger for diagnostic purposes - this is a far stretch from an example of a great logging class
    /// but is serves the purpose nicely with a minimum of overhead.  
    /// All log files are created with timestamps in their name to keep them unique in the temporary folder defined by the OS which
    /// is different depending on which version of Windows you happen to be running on.
    /// It's a static class and the stream file is cleaned up automatically when the application shuts down.
    /// This class is clearly NOT thread safe!  That's beyond the scope of this example.
    /// </summary>
    public static class Logger 
    {
        public static bool DebugEnabled { get; set; }
        
        private static string _fileName;
        private static StreamWriter _swLog;
        private static object _lock = new Object(); //used to make sure only one thread at a time is writing to the file

        //Class constructor - creates a new file name with date in the name to make it unique and opens the stream file writer.
        //The file is opened for append so if there happens to be a file with that name (not likely) it will simply tack onto the 
        //end if it.
        static Logger()
        {
            //generate a new log file name in the OS's temporary output folder.
            string strDate = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
            _fileName = string.Format("{0}RESTFastStart_Log_{1}.txt", Path.GetTempPath(), strDate);

            //by default debug is off unless the user explicitly turns it on
            DebugEnabled = false;

            //open the log file stream and leave it open for the duration of the application's life.
            try
            {
                _swLog = new StreamWriter(_fileName, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Could not open log file in {0}, error={1}", _fileName, ex.Message));
                _swLog = null;
            }

        }


        /// <summary>
        /// log a line of output prepended with the date and time.
        /// </summary>
        /// <param name="pOutput">
        /// String to output to the log file.
        /// </param>
        /// <param name="pDebugOutput">
        /// Optional paramter - if passed in as TRUE the log file will only be written to if the logging class DebugOutput is also
        /// set to TRUE, otherwise it's skipped.
        /// </param>
        public static void Log(string pOutput, bool pDebugOutput = false)
        {
            //if there was some sort of error opening the stream writer than bail out here.
            if (_swLog==null)
             {
                 return;
             }

            //if the logging request is for debug output but debug is not enabled, skip the log request
            if (pDebugOutput && DebugEnabled==false)
            {
                return;
            }

            //make sure only one thread at a time is writing to the output file.
            lock (_lock)
            {
                //don't include the timestamp prefix for debug output
                if (pDebugOutput)
                {
                    _swLog.WriteLine(pOutput);
                }
                else
                {
                    string strDate = DateTime.Now.ToString("[yyyy_MM_dd hh:mm:ss] ");
                    _swLog.WriteLine(strDate + pOutput);
                }
            }
        }


        /// <summary>
        /// Returns the current log file path.  Useful if you want to pop it open for a view.
        /// </summary>
        /// <returns>
        /// Fully qualified path to the currently open log file.
        /// </returns>
        public static string GetCurrentLogFilePath()
        {
            return _fileName;
        }

        /// <summary>
        /// Force the log file stream to flush its contents to the file system.  Usueful if you want to view the contents
        /// of the file without closing it first.
        /// </summary>
        public static void FlushLog()
        {
          	if (_swLog != null)
            {
              	_swLog.Flush();
            }
        }


        /// <summary>
        /// Close the logging stream if it's open prior to exiting
        /// </summary>
        public static void StopLogging()
        {
            if (_swLog!=null)
            {
                Log("Exiting application.");
                _swLog.Close();
                _swLog.Dispose();
            }
        }

    }
}