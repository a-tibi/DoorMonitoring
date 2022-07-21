using System;
using System.Collections.Generic;
using System.Text;
using TwinCAT.Ads;
using System.Threading;

namespace MarsDoorMonitoring
{
    class CLog
    {        
        private struct strTIMESTRUCT
        {
            public UInt16 wYear;
            public UInt16 wMonth;
            public UInt16 wDayOfWeek;
            public UInt16 wDay;
            public UInt16 wHour;
            public UInt16 wMinute;
            public UInt16 wSecond;
            public UInt16 wMilliseconds;
        }
        private struct _strLog
        {
            public strTIMESTRUCT _strDateTime;
            public byte _bDoorID;
            public byte _bStatus;
        }

        //Constructor
        public CLog()
        {

        }

        private TcAdsClient _cTcAds;
        private int _iSvrPort;
        private string _sNetID;
        CEventLog _cEventLog;

        private void Connect()
        {

            try
            {

                _cTcAds.Connect(_sNetID, _iSvrPort);
                if (_cTcAds.IsConnected)
                {
                    //_cEventLog.LogInfoEvent("Connected to " + _sNetID + ", to port " + _iSvrPort + ".");
                }
                else
                {
                    _cEventLog.LogErrorEvent("Error while attempting to connect to " + _sNetID + ", to port " + _iSvrPort + ".");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to connect to " + _sNetID + ", to port " + _iSvrPort + ".");
            }
        }

        private void DisConnect()
        {
            _cTcAds.Dispose();
        }

        public bool GetLog(string sVarName)
        {
            int iHandle = 0;            
            _strLog strLog = new _strLog();
            bool ret = false;
            Connect();

            try
            {
                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);

                    strLog = (_strLog)_cTcAds.ReadAny(iHandle, typeof(_strLog));

                    _cTcAds.DeleteVariableHandle(iHandle);

                    ret = true;
                }
                else
                {                    
                    _cEventLog.LogErrorEvent("Attempting to read var name: " + sVarName + " to an unconnected server.");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to read var name: " + sVarName + ".");
            }

            DisConnect();                        
            return ret;
        }
        
    }
}