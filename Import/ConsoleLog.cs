using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;

namespace Import
{
  public class ConsoleLog
  {
     #region vars
        NameValueCollection appSettings = ConfigurationManager.AppSettings;
        StreamWriter _logFile;
        List<AuditLog> _auditLog = new List<AuditLog>();
        private HousingSAModel _context = new HousingSAModel();
        public string line = "";
        private DateTime startTime;
      #endregion

    public ConsoleLog ()
    {
    }

    #region AppConfig
    public string GetFromConfig (string section)
    {
      return appSettings.Get(section);
    }
    #endregion

    #region Logging
    public void EndLog ()
    {
        DateTime endTime = DateTime.Now;
        TimeSpan runTime = endTime - startTime;
        AuditLog auditLog = new AuditLog();
        auditLog.LogTime = DateTime.Now;
        auditLog.Line = string.Format("Running time - {0}", runTime);
        _auditLog.Add(auditLog);

        try
        {
            if (line.Equals("Y"))
            {
                _logFile.WriteLine("Running time - {0}", runTime);
                _logFile.Close();
            }
        }
        catch (Exception e)
        {
            _logFile.WriteLine(e);
            _logFile.Close();
        }

    }

    public void BeginLog()
    {
            string logFileName = GetFromConfig("LogFilePath");
            if (!File.Exists(logFileName))
            {
                FileStream fs = System.IO.File.Create(logFileName);
                fs.Close();
            }
            _logFile = File.AppendText(logFileName);

            _logFile.Write("\r\nHousing Extract : ");
            _logFile.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            startTime = DateTime.Now;
    }

    public void Log (string logMessage)
    {
      DateTime logTime = DateTime.Now;

        //Write to Console
      System.Console.WriteLine("{0}: {1}", logTime, logMessage);

        //Write to AuditLog table
      AuditLog auditLog = new AuditLog();
      auditLog.LogTime = logTime;
      auditLog.Line = logMessage;
      _auditLog.Add(auditLog);

        //Write to log file
    _logFile.WriteLine("{0}: {1}", logTime, logMessage);
    // Update the underlying file.
    _logFile.Flush();
    }

    public void PopulateLog ()
    {
      foreach (AuditLog item in _auditLog)
      {
         _context.AuditLogs.Add(item);
      }
        _context.SaveChanges();
    }
      
    #endregion

  }

}
