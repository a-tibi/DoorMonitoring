using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MarsDoorMonitoring
{
    class CEventLog
    {
        public CEventLog(string sSource)
        {
            _sSource = sSource;
            _sLog = "Door_Monitoring";
        }

        public void LogInfoEvent(string sMessage)
        {
            if (!EventLog.SourceExists(_sSource))
                EventLog.CreateEventSource(_sSource, _sLog);
            EventLog.WriteEntry(_sSource, sMessage, EventLogEntryType.Information, 1);

        }

        public void LogErrorEvent(string sMessage)
        {
            if (!EventLog.SourceExists(_sSource))
                EventLog.CreateEventSource(_sSource, _sLog);

            EventLog.WriteEntry(_sSource, sMessage, EventLogEntryType.Error, 5);


        }

        public void LogWarningEvent(string sMessage)
        {
            if (!EventLog.SourceExists(_sSource))
                EventLog.CreateEventSource(_sSource, _sLog);

            EventLog.WriteEntry(_sSource, sMessage, EventLogEntryType.Warning, 1);


        }

        private string _sSource;
        private string _sLog;
        
    }
}
