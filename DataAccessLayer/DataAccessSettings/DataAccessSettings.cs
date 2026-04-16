using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

public class DataAccessSettings
{
    public static string? ConnectionString { get; set; }


    public static void LogEvent(string Message, EventLogEntryType Type)
    {
        string SourceName = "ResturantSystem";


        if (OperatingSystem.IsWindows())
        {

            if (!EventLog.SourceExists(SourceName))
            {
                EventLog.CreateEventSource(SourceName, "Application");
            }

            EventLog.WriteEntry(SourceName, Message, Type);
        }
        else
        {
            string Folder = Path.Combine(AppContext.BaseDirectory + "Logs");

            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }
            File.AppendAllText(Path.Combine(Folder + "Errors.txt"), Message);
        }

    }
}
