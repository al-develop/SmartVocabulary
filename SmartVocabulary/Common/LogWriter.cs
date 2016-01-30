using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVocabulary.Common
{
    public sealed class LogWriter : IDisposable
    {
        #region Singleton
        private static readonly Lazy<LogWriter> _instance = new Lazy<LogWriter>(() => new LogWriter());

        public static LogWriter Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        #endregion


        StreamWriter logWriter;
        int fileCount = 1;
        int nfileCount = 1;
        static string logpath = String.Format("{0}//LOGS//", AppDomain.CurrentDomain.BaseDirectory);
        string logfilename = string.Empty;
        string logXmlfilename = string.Empty;
        long LogFileSize = 2097152;

        public void WriteLine(string Msg)
        {
            try
            {
                if (string.IsNullOrEmpty(logfilename))
                    logfilename = logpath + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".Log." + fileCount + ".txt";

                if (!(Directory.Exists(logpath)))
                    Directory.CreateDirectory(logpath);

                while (true)
                {
                    if (!File.Exists(logfilename))
                    {
                        logWriter = new StreamWriter(logfilename);
                        break;
                    }
                    else
                    {
                        if (LogFileSize == 0)
                            LogFileSize = 2097152;
                        FileInfo fi = new FileInfo(logfilename);
                        if (fi.Length > LogFileSize)
                        {
                            fileCount++;
                            logfilename = logpath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "_" + fileCount + ".txt";
                            continue;
                        }
                        else
                        {
                            logWriter = File.AppendText(logfilename);
                            break;
                        }
                    }
                }

                logWriter.WriteLine(DateTime.Now.ToString("g") + ": " + Msg);
                logWriter.Flush();
                logWriter.Close();
            }//End of Try
            catch (Exception ex)
            {
            }//End of catch
        }

        #region IDisposable Member

        public void Dispose()
        {            
            this.logWriter.Close();
            GC.Collect();
        }

        #endregion
    }
}
