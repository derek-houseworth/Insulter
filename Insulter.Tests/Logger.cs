using System.Text;

namespace Insulter.Tests
{

	public enum LoggerLinePrefix { None, Date, Time, DateAndTime }
    public class Logger
    {
        private const string DEFAULT_LOG_FILE_NAME = "./logger.log";
        private string? _fileName;
        public string FileName 
        { 
            get => _fileName ?? ""; 
            set 
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"log file name cannot be null or empty");
                }
                _fileName = value;
            }
        }
        public LoggerLinePrefix LinePrefix;

        public int BytesWritten { get; internal set; }
        public void Clear()
        {
            BytesWritten = 0;
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            Write("");
        }

        public void WriteLine(string text)
        { 
            Write(text + Environment.NewLine);
        }

        public void Write(string text)
        {
            using (var fileStream = File.OpenWrite(FileName))
            {
                try
                {
                    if (fileStream.CanWrite)
                    {
                        fileStream.Seek(0, SeekOrigin.End);
                        byte[] buffer = Encoding.ASCII.GetBytes(GetLinePrefix() + text);
                        BytesWritten += buffer.Length;
                        fileStream.Write(buffer,0, buffer.Length);
                    }
                    else
                    {
                        throw new Exception($"can't write to {FileName}");
                    }
                }
                catch (Exception ex)
                {
                    TestHelper.DebugWriteLine($"Logger: {ex.Message}", false);
                }
                finally
                {
                    fileStream.Flush();
                    fileStream.Close();
                }
            }
        }

        private string GetLinePrefix()
        {
            DateTime localTime = DateTime.Now.ToLocalTime();
            switch(LinePrefix)
            {
                case LoggerLinePrefix.None:
                    return "";
                case LoggerLinePrefix.Date:
                    return localTime.ToString("yyyy-MM-dd ");
                case LoggerLinePrefix.Time:
                    return localTime.ToString("H:mm:ss ");
                case LoggerLinePrefix.DateAndTime:
                    return localTime.ToString("yyyy-MM-dd H:mm:ss ");
                default:
                    return "";
            };
        }

        public Logger(string fileName = DEFAULT_LOG_FILE_NAME, bool replaceExistingFile = false) 
        {
            FileName = fileName;
            if (replaceExistingFile ||(!replaceExistingFile && !File.Exists(FileName)))
            {
                Clear();
            }
            LinePrefix = LoggerLinePrefix.None;
            BytesWritten = 0;
        } 
    }
}