using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MarsDoorMonitoring
{
    class CDoorStatus
    {
        public int iDoorNumber;                 // PLC_ID
        public int iDoorID;                     // Door_ID = Button_Tag
        public Button LinkedButton;
        public System.Drawing.Color BtnColor;
        public bool bConnectionError;
        public bool bAlarmLogged;

        private bool _bDoorOpen;
        private bool _bAlarmNotAcknowledged;
        private bool _bAlarmNotCleared;
        private bool _bIsAlarmActive;
        private bool _bOutOfCoverage;

        public CDoorStatus()
        {
            bAlarmLogged = false;
            iDoorNumber = 0;
            _bDoorOpen = false;
            _bAlarmNotAcknowledged=false;
            _bAlarmNotCleared = false;
            _bIsAlarmActive = false;
            _bOutOfCoverage = false;
        }

        private CTwinCatInterface _TwinCatInterface;

        public void SetTwinCatInterface(CTwinCatInterface TCInterface)
        {
            _TwinCatInterface = TCInterface;
        }

        public void AcknowledgeAlarm()
        {
            byte bStatus = _TwinCatInterface.GetByte(".aDoors[" + iDoorNumber.ToString() + "].bStatus");

            bStatus = (byte)((int)bStatus & 253);   // bStatus[1] = 0

            _TwinCatInterface.SetByte(".aDoors[" + iDoorNumber.ToString() + "].bStatus", bStatus);

            bConnectionError = _TwinCatInterface.bConnectionError;

            bAlarmLogged = false;
        }

        public void ClearAlarm()
        {
            this.UpdateDoorStatus();

            if (this.IsAlarmClearable())
            {
                byte bStatus = _TwinCatInterface.GetByte(".aDoors[" + iDoorNumber.ToString() + "].bStatus");

                bStatus = (byte)((int)bStatus & 251);   // bStatus[2] = 0

                _TwinCatInterface.SetByte(".aDoors[" + iDoorNumber.ToString() + "].bStatus", bStatus);

                
            }
            
            bConnectionError = _TwinCatInterface.bConnectionError;
        }

        public bool IsAlarmClearable()
        {
            bool bRet = false;
            if ((!this._bAlarmNotAcknowledged) && (!this._bDoorOpen)) bRet = true;  //  if Alarm is Acknowledged and Door is Closed 

            return bRet;
        }

        public void UpdateDoorStatus()
        {
            byte bStatus = _TwinCatInterface.GetByte(".aDoors[" + iDoorNumber.ToString() + "].bStatus");

            if ((bStatus & 1) != 0)                                             // bStatus[0] == 1 -> the Door is opened 
            {
                this._bDoorOpen = true;

            }
            else                                                                // door is not opened closed
            {
                this._bDoorOpen = false;
            }

            if ((bStatus & 2) != 0)                                             // bStatus[1] == 1 -> Alarm is Not Acknowledged
            {
                this._bAlarmNotAcknowledged = true;
                
            }
            else
            {
                this._bAlarmNotAcknowledged = false;
            }

            if ((bStatus & 4) != 0)                                             //  bStatus[2] == 1 -> Alarm is Not Cleared
            {
                this._bAlarmNotCleared = true;
            }
            else
            {
                this._bAlarmNotCleared = false;
            }

            if ((bStatus & 8) != 0)                                             //  bStatus[3] == 1 -> Sensor is out of coverage
            {
                this._bOutOfCoverage = true;
            }
            else
            {
                this._bOutOfCoverage = false;
            }

            if(this._bOutOfCoverage)
            {
                this._bIsAlarmActive = true;
                BtnColor = System.Drawing.Color.Black;
            }
            else
            {
                if ((!this._bAlarmNotAcknowledged) && (!this._bAlarmNotCleared))    //  Alarm is Acknowledged & Cleared -> Button -> Green
                {
                    this._bIsAlarmActive = false;
                    BtnColor = System.Drawing.Color.LawnGreen;
                }
                else
                {
                    this._bIsAlarmActive = true;
                    if (this._bAlarmNotAcknowledged)                                //  Alarm is Not Acknowledged -> Button -> Red
                    {
                        BtnColor = System.Drawing.Color.Red;
                    }
                    else                                                            //  Alarm is Acknowledged -> Button -> Yellow
                    {
                        BtnColor = System.Drawing.Color.Yellow;
                    }

                }
            }

            bConnectionError = _TwinCatInterface.bConnectionError;
        }

        public bool DoorOpen()
        {
            return this._bDoorOpen; 

        }

        public bool AlarmActive()
        {
            return this._bIsAlarmActive;
        }

        public bool AlarmAcknowledged()
        {
            return !this._bAlarmNotAcknowledged;
        }

        public bool AlarmCleared()
        {
            return !this._bAlarmNotCleared;
        }

        public bool SensorOutOfCoverage()
        {
            return this._bOutOfCoverage;
        }

    }
}