using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using Project1;
using NLog;

namespace Common
{
  public class ConsoleLog
  {
     #region vars
        //NameValueCollection appSettings = ConfigurationManager.AppSettings;
        StreamWriter _logFile;
        List<AuditLog> _auditLog = new List<AuditLog>();
        private FAASModel _context = new FAASModel();
        public string line = "";
        private DateTime startTime;
        
        //Nlog
        private static Logger logger = LogManager.GetCurrentClassLogger();
      #endregion

    public ConsoleLog ()
    {
    }

    #region Logging
    public void Log (string logMessage)
    {
      //Write to AuditLog table
      AuditLog auditLog = new AuditLog();
      auditLog.LogTime = DateTime.Now;
      auditLog.Line = logMessage;
      _auditLog.Add(auditLog);

    //Write to log file
    logger.Info("{0}:", logMessage);
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
