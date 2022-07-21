using System;
using System.Collections.Generic;
using System.Text;
using TwinCAT.Ads;
using System.Threading;

namespace MarsDoorMonitoring
{
    class CTwinCatInterface
    {        
        //public members
        public bool bConnectionError;

        private Mutex _ThreadMutex;

        private struct _strTIMESTRUCT
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

        //Constructor
        public CTwinCatInterface(string sNetID, int iPortNo)
        {
            try
            {
                _sNetID = sNetID;
                _iSvrPort = iPortNo;
                _cEventLog = new CEventLog("TC_Int_" + _sNetID);
                _cEventLog.LogInfoEvent("TC_Int_" + _sNetID + " created.");
                _cTcAds = new TcAdsClient();
                //_cEventLog.LogInfoEvent("TwinCAT interface on address " + _sNetID + " port " + _iSvrPort.ToString() + " created");
                _ThreadMutex = new Mutex();
            }
            catch
            {
                _cEventLog.LogErrorEvent("TwinCAT ADS dll might be missing.");
            }
        }
       
        public void SetBool(string sVarName, bool bValue)
        {
            int iHandle = 0;
            Connect();
            
            try
            {

                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);

                    _cTcAds.WriteAny(iHandle, bValue);

                    _cTcAds.DeleteVariableHandle(iHandle);
                    
                }
                else
                {
                    _cEventLog.LogErrorEvent("Attempting to write var name: " + sVarName + " to an unconnected server.");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to write var name: " + sVarName + ".");
            }

            DisConnect();

        }
        
        public bool GetBool(string sVarName)
        {
            int iHandle = 0;
            bool bValue = false;
           
            Connect();

            try
            {
                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);

                    bValue = (bool)_cTcAds.ReadAny(iHandle, typeof(bool));

                    _cTcAds.DeleteVariableHandle(iHandle);

                    
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
            return bValue;

        }
        
        public string GetDateTime(string sVarName)
        {
            int iHandle = 0;
            _strTIMESTRUCT strValue = new _strTIMESTRUCT();
            string sValue = "";
            //_ThreadMutex.WaitOne();
            Connect();

            try
            {
                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);

                    strValue = (_strTIMESTRUCT)_cTcAds.ReadAny(iHandle, typeof(_strTIMESTRUCT));

                    _cTcAds.DeleteVariableHandle(iHandle);


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
            //_ThreadMutex.ReleaseMutex();
            sValue = strValue.wMonth.ToString() + "/" + strValue.wDay.ToString() + "/" + strValue.wYear.ToString() + " " + strValue.wHour.ToString() + ":" + strValue.wMinute.ToString() + ":" + strValue.wSecond.ToString() + ":" + strValue.wMilliseconds.ToString();          
            return sValue;

        }

        public string GetString(string sVarName)
        {
            string sRet = "";
            int iHandle = 0;
            Connect();

            try
            {

                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);
                    sRet = (string)_cTcAds.ReadAny(iHandle, typeof(string));
                    _cTcAds.DeleteVariableHandle(iHandle);


                }
                else
                {
                    _cEventLog.LogErrorEvent("Attempting to write var name: " + sVarName + " to an unconnected server.");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to write var name: " + sVarName + ".");
            }
            DisConnect();

            return (string)sRet;

        }

        public UInt16 GetWord(string sVarName)
        {
            ushort iRet = 0;
            int iHandle = 0;
            Connect();

            try
            {

                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);
                    iRet=(ushort)_cTcAds.ReadAny(iHandle,typeof(ushort));
                    _cTcAds.DeleteVariableHandle(iHandle);

                   
                }
                else
                {
                    _cEventLog.LogErrorEvent("Attempting to write var name: " + sVarName + " to an unconnected server.");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to write var name: " + sVarName + ".");
            }
            DisConnect();

            return (UInt16)iRet;

        }

        public UInt32 GetDWord(string sVarName)
        {
            uint iRet = 0;
            int iHandle = 0;
            Connect();

            try
            {

                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);
                    iRet = (uint)_cTcAds.ReadAny(iHandle, typeof(uint));
                    _cTcAds.DeleteVariableHandle(iHandle);


                }
                else
                {
                    _cEventLog.LogErrorEvent("Attempting to write var name: " + sVarName + " to an unconnected server.");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to write var name: " + sVarName + ".");
            }
            DisConnect();

            return (UInt32)iRet;

        }

        public long GetLong(string sVarName)
        {
            long iRet = 0;
            int iHandle = 0;
            Connect();

            try
            {

                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);
                    iRet = (long)_cTcAds.ReadAny(iHandle, typeof(long));
                    _cTcAds.DeleteVariableHandle(iHandle);
                    _cEventLog.LogInfoEvent("Reading var : " + sVarName + ", " + iRet.ToString());

                }
                else
                {
                    _cEventLog.LogErrorEvent("Attempting to write var name: " + sVarName + " to an unconnected server.");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to write var name: " + sVarName + ".");
            }
            DisConnect();

            return iRet;

        }

        public void SetInt(string sVarName, int uiValue)
        {
            int iHandle = 0;
            Connect();

            try
            {

                if (_cTcAds.IsConnected)
                {
                    _cEventLog.LogInfoEvent("Writing var : " + sVarName + ", " + uiValue.ToString());

                    iHandle = _cTcAds.CreateVariableHandle(sVarName);

                    

                    _cTcAds.WriteAny(iHandle, uiValue);

                    _cTcAds.DeleteVariableHandle(iHandle);

                    
                }
                else
                {
                    _cEventLog.LogErrorEvent("Attempting to write var name: " + sVarName + " to an unconnected server.");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to write var name: " + sVarName + "," + uiValue.ToString() + ".");
            }
            DisConnect();
        }

        public void SetWord(string sVarName,UInt16 uiValue)
        {
            int iHandle = 0;
            Connect();

            try
            {

                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);

                    _cTcAds.WriteAny(iHandle, uiValue);

                    _cTcAds.DeleteVariableHandle(iHandle);

                    _cEventLog.LogInfoEvent("Writing var : " + sVarName + ", " + uiValue.ToString());
                }
                else
                {
                    _cEventLog.LogErrorEvent("Attempting to write var name: " + sVarName + " to an unconnected server.");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to write var name: " + sVarName + ".");
            }
            DisConnect();
        }

        public void SetDWord(string sVarName, UInt32 uiValue)
        {
            int iHandle = 0;
            Connect();

            try
            {

                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);

                    _cTcAds.WriteAny(iHandle, uiValue);

                    _cTcAds.DeleteVariableHandle(iHandle);

                    _cEventLog.LogInfoEvent("Writing var : " + sVarName + ", " + uiValue.ToString());
                }
                else
                {
                    _cEventLog.LogErrorEvent("Attempting to write var name: " + sVarName + " to an unconnected server.");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to write var name: " + sVarName + ".");
            }
            DisConnect();
        }

        public void SetByte(string sVarName, byte bValue)
        {
            int iHandle = 0;

            _ThreadMutex.WaitOne();
            Connect();
           
            try
            {
                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);

                    _cTcAds.WriteAny(iHandle, bValue);

                    _cTcAds.DeleteVariableHandle(iHandle);
                    bConnectionError = false;
                   
                }
                else
                {
                    _cEventLog.LogErrorEvent("Attempting to write var name: " + sVarName + " to an unconnected server.");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to write var name: " + sVarName + ".");
                bConnectionError = true;
            }
            DisConnect();


            _ThreadMutex.ReleaseMutex();
        }

        public byte GetByte(string sVarName)
        {
            int iHandle = 0;
            byte bValue = 0;

            _ThreadMutex.WaitOne();

            Connect();

            try
            {
                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);

                    bValue = (byte)_cTcAds.ReadAny(iHandle, typeof(byte));

                    _cTcAds.DeleteVariableHandle(iHandle);
                    bConnectionError = false;
                    
                }
                else
                {
                    _cEventLog.LogErrorEvent("Attempting to read var name: " + sVarName + " to an unconnected server.");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to read var name: " + sVarName + ".");
                bConnectionError = true;
            }
            DisConnect();


            _ThreadMutex.ReleaseMutex();
            return bValue;

        }

        public byte GetInt8(string sVarName)    //  same as GetByte but without _ThreadMutex process
        {
            int iHandle = 0;
            byte bValue = 0;

            //_ThreadMutex.WaitOne();

            Connect();

            try
            {
                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);

                    bValue = (byte)_cTcAds.ReadAny(iHandle, typeof(byte));

                    _cTcAds.DeleteVariableHandle(iHandle);
                    bConnectionError = false;

                }
                else
                {
                    _cEventLog.LogErrorEvent("Attempting to read var name: " + sVarName + " to an unconnected server.");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to read var name: " + sVarName + ".");
                bConnectionError = true;
            }
            DisConnect();


            //_ThreadMutex.ReleaseMutex();
            return bValue;

        }

        public short GetCircuitByIdx(int iCircuit)
        {
            Connect();
            short iRet;
            long idxOffset;

            idxOffset = 20 + ((iCircuit - 1) * 2);

            iRet = (short)_cTcAds.ReadAny(61488, idxOffset, typeof(short));

            DisConnect();
            return iRet;
        }       

        public void SetCircuitByIdx(int iCircuit, short iCircuitValue)
        {
            Connect();

            long idxOffset;

            idxOffset = 20 + ((iCircuit - 1) * 2);

            _cTcAds.WriteAny(61488, idxOffset, iCircuitValue);

            DisConnect();
        }

        public void SwitchAllGroupOff()
        {
            Connect();

            long idxOffset;

            idxOffset = 256;

            _cTcAds.WriteAny(61488, idxOffset, true);

            DisConnect();


        }

        public void SwitchOFFGroupByIdx(int iGroupNo)
        {
            Connect();

            long idxOffset;

            idxOffset = 236 + (iGroupNo - 1);

            _cTcAds.WriteAny(61488, idxOffset, true);

            DisConnect();
        }

        public bool GetContactorByIndex(int iCtcNo)
        {
            Connect();
            bool bRet;
            long idxOffset;

            idxOffset = iCtcNo - 1;

            bRet = (bool)_cTcAds.ReadAny(61488, idxOffset, typeof(bool));

            DisConnect();
            return bRet;
        }

        public void SetContactorByIndex(int iCtcNo, bool bValue)
        {
            Connect();

            long idxOffset;

            if (bValue)
            {
                idxOffset = 80 + (iCtcNo - 1);
            }
            else
            {
                idxOffset = 100 + (iCtcNo - 1);
            }

            _cTcAds.WriteAny(61488, idxOffset, true);

            DisConnect();
        }
       
        public void SetSceneValueByIdx(int iSceneNo, int iCircuitNo, UInt16 iValue)
        {
            Connect();

            long idxOffset;

            idxOffset = (80 + ((iSceneNo - 1) * 34)) + ((iCircuitNo) * 2);

            _cTcAds.WriteAny(61488, idxOffset, iValue);

            DisConnect();

        }

        public void RecallSceneByIdx(int iSceneNo)
        {
            Connect();

            long idxOffset;

            idxOffset = 216 + (iSceneNo - 1);

            _cTcAds.WriteAny(61488, idxOffset, true);

            DisConnect();

        }

        public void SetSceneNbCircuitByIdx(int iSceneNo, byte iValue)
        {
            Connect();

            long idxOffset;

            idxOffset = 112 + ((iSceneNo - 1)*34);

            _cTcAds.WriteAny(61488, idxOffset, iValue);

            DisConnect();
        }

        public void SetScene(int iSceneIndex, int iCircuitIndex, int iDimmingValue)
        {
            //this.SetWord(".aScenes[" + iSceneIndex.ToString() + "].aCommandOutput[" + iCircuitIndex.ToString() + "]", (UInt16)iDimmingValue);
            this.SetSceneValueByIdx(iSceneIndex, iCircuitIndex, (UInt16)iDimmingValue);
        }
              
        public short GetShort(string sVarName)
        {
            int iHandle = 0;
            short iValue = 0;

            Connect();

            try
            {
                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);

                    iValue = (short)_cTcAds.ReadAny(iHandle, typeof(short));

                    _cTcAds.DeleteVariableHandle(iHandle);


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
            return iValue;

        }

        public void SetShort(string sVarName, short iValue)
        {
            int iHandle = 0;
            Connect();

            try
            {
                if (_cTcAds.IsConnected)
                {
                    iHandle = _cTcAds.CreateVariableHandle(sVarName);

                    _cTcAds.WriteAny(iHandle, iValue);

                    _cTcAds.DeleteVariableHandle(iHandle);


                }
                else
                {
                    _cEventLog.LogErrorEvent("Attempting to write var name: " + sVarName + " to an unconnected server.");
                }
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while attempting to write var name: " + sVarName + ".");
            }
            DisConnect();
        }

        //private members
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
       
    }
}
