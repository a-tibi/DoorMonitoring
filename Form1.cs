using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MarsDoorMonitoring
{
    public partial class Form1 : Form
    {
        CSettings Settings;
        List<Button> lstButton;
        List<CDoorStatus> lstDoorStatus;
        List<CDoorStatus> lstDoorStatusAlarmActive;
        CTwinCatInterface TCInterface;
        bool bBlinkingStateVariable;
        bool bConnectionError;
        int CurrentButtonTagClicked;
        Thread DataInitialUpdateThread;
        Thread DataUpdateThread;
        CEventLog EvtLog;

        bool bInit;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            bInit = true;

            CDoorStatus locDoorStatus = null;
            bConnectionError = false;
            bBlinkingStateVariable = false;
            CurrentButtonTagClicked = 0;
            lstDoorStatusAlarmActive = new List<CDoorStatus>();

            EvtLog = new CEventLog("Door Monitoring");

            lstButton = new List<Button>();
            lstButton.Add(button1);
            lstButton.Add(button2);
            lstButton.Add(button3);
            lstButton.Add(button4);
            lstButton.Add(button5);
            lstButton.Add(button6);
            lstButton.Add(button7);
            lstButton.Add(button8);
            lstButton.Add(button9);
            lstButton.Add(button10);
            lstButton.Add(button11);
            lstButton.Add(button12);
            lstButton.Add(button13);
            lstButton.Add(button14);
            lstButton.Add(button15);
            lstButton.Add(button16);
            lstButton.Add(button17);
            lstButton.Add(button18);
            lstButton.Add(button19);
            lstButton.Add(button20);
            lstButton.Add(button21);
            lstButton.Add(button22);
            lstButton.Add(button23);
            lstButton.Add(button24);
            lstButton.Add(button25);
            lstButton.Add(button26);
            lstButton.Add(button27);
            lstButton.Add(button28);
            lstButton.Add(button29);
            lstButton.Add(button30);
            lstButton.Add(button31);
            lstButton.Add(button32);
            lstButton.Add(button33);
            lstButton.Add(button34);
            lstButton.Add(button35);
            lstButton.Add(button36);
            lstButton.Add(button37);
            lstButton.Add(button38);
            lstButton.Add(button39);
            lstButton.Add(button40);
            lstButton.Add(button41);
            lstButton.Add(button42);
            lstButton.Add(button43);
            lstButton.Add(button44);
            lstButton.Add(button45);            

            lstDoorStatus = new List<CDoorStatus>();
            Settings = new CSettings("C:\\Program Files\\IA\\DoorMonitoring\\Settings.xml");

            if (Settings.UpdateSettings())
            {
                TCInterface = new CTwinCatInterface(Settings.sAMSNetID, Settings.iPortNo);

                foreach (Button btn in lstButton)
                {
                    toolTip1.SetToolTip(btn, Settings.GetDescription(Convert.ToInt32(btn.Tag)));
                    Settings.SetButton(Convert.ToInt32(btn.Tag), btn);
                }

                // Connect DoorStatus with DoorSettings
                foreach (CDoorSettings doorSettings in Settings.lstDoorSettings)
                {
                    locDoorStatus = new CDoorStatus();
                    locDoorStatus.iDoorNumber = doorSettings.iPLC_ID;
                    locDoorStatus.iDoorID = doorSettings.iDoorID;
                    locDoorStatus.SetTwinCatInterface(TCInterface);
                    locDoorStatus.LinkedButton = doorSettings.LinkedButton;
                    lstDoorStatus.Add(locDoorStatus);
                }
                
                tmrFlashing.Interval = 1000;
                tmrFlashing.Enabled = true;

                DataInitialUpdateThread = new Thread(new ThreadStart(DataInitialUpdateThreadLoop));
                DataInitialUpdateThread.Start();
            }
            else
            {
                MessageBox.Show("Error while starting application. Settings file might be missing.");

            }            
            
        }

        private void DataInitialUpdateThreadLoop()
        {
            while (bInit)
            {
                int cntDoorStatus = 0;
                bConnectionError = false;
                foreach (CDoorStatus doorSts in lstDoorStatus)
                {
                    if (!bConnectionError)
                    {

                        doorSts.UpdateDoorStatus();
                    }



                    if (!doorSts.bConnectionError)
                    {
                        cntDoorStatus++;
                    }
                    else
                    {
                        bConnectionError = true;
                    }
                }
                if (cntDoorStatus >= lstDoorStatus.Count)
                {
                    bInit = false;                    
                }
                Thread.Sleep(500);
            }// end while         
            DataUpdateThread = new Thread(new ThreadStart(DataUpdateThreadLoop));
            DataUpdateThread.Start();      
        }

        private void DataUpdateThreadLoop()
        {
            while (!bInit)
            {                
                UInt32 uiDoorStatusChanged1 = TCInterface.GetDWord(".dwDoorStatusChanged1");
                UInt32 uiDoorStatusChanged2 = TCInterface.GetDWord(".dwDoorStatusChanged2");

                Int32 iOne = 1;
                CDoorStatus locDoorStatus = null;
                bConnectionError = false;
                for (int iDoorNo = 1; iDoorNo <= 45; iDoorNo++)
                {
                    Int32 iNumberOfDoorBit = iDoorNo - 1;
                    Int32 iDoorBit = iOne << iNumberOfDoorBit;
                    UInt32 uiDoorBit = (UInt32)iDoorBit;

                    if ((uiDoorStatusChanged1 & uiDoorBit) == 1)    //  Door i status has been changed
                    {
                        locDoorStatus = GetDoorStatusbyPlcID(iDoorNo);
                        if (locDoorStatus != null)
                        {
                            if (!bConnectionError)
                            {

                                locDoorStatus.UpdateDoorStatus();

                                uiDoorStatusChanged1 = uiDoorStatusChanged1 & ~uiDoorBit;   //  reset the Door Changed Flag

                            }

                            if (locDoorStatus.bConnectionError) bConnectionError = true;

                        }
                    }
                }

                // Update PLC variables -> write to PLC
                TCInterface.SetDWord(".dwDoorStatusChanged1", uiDoorStatusChanged1);
                TCInterface.SetDWord(".dwDoorStatusChanged2", uiDoorStatusChanged2);

                // bBlinkingStateVariable = !bBlinkingStateVariable;                                
                Thread.Sleep(500);
            }// end while                           
        }


        /* the old DataUpdateThreadLoop()
         private void DataUpdateThreadLoop()
        {
            while (true)
            {
                bConnectionError = false;
                foreach (CDoorStatus doorSts in lstDoorStatus)
                {
                    if (!bConnectionError)
                    {

                        doorSts.UpdateDoorStatus();
                    }

                    

                    if (doorSts.bConnectionError) bConnectionError = true;
                }                

                // bBlinkingStateVariable = !bBlinkingStateVariable;
                Thread.Sleep(500);
            }

        } 
         */

        private CDoorStatus GetDoorStatus(int iTagID)
        {
            CDoorStatus retDoorStatus = null;

            foreach (CDoorStatus doorStatus in lstDoorStatus)
            {
                if (doorStatus.iDoorID == iTagID)
                {
                    retDoorStatus = doorStatus;
                }

            }

            return retDoorStatus;
        }


        private CDoorStatus GetDoorStatusbyPlcID(int iPlcID)
        {
            CDoorStatus retDoorStatus = null;

            foreach (CDoorStatus doorStatus in lstDoorStatus)
            {
                if (doorStatus.iDoorNumber == iPlcID)
                {
                    retDoorStatus = doorStatus;
                }

            }

            return retDoorStatus;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void acnowledgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CDoorStatus locDoorStatus = null;
            locDoorStatus = GetDoorStatus(CurrentButtonTagClicked);

            if (locDoorStatus != null)
            {
                locDoorStatus.AcknowledgeAlarm();
                locDoorStatus.UpdateDoorStatus();
                //logAlarm(locDoorStatus);
                CurrentButtonTagClicked = 0;
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CDoorStatus locDoorStatus = null;
            locDoorStatus = GetDoorStatus(CurrentButtonTagClicked);

            if (locDoorStatus != null)
            {
                locDoorStatus.ClearAlarm();
                locDoorStatus.UpdateDoorStatus();
                //logAlarm(locDoorStatus);
                CurrentButtonTagClicked = 0;
            }
        }

        private void DisplayPopUpMenu(object sender, MouseEventArgs e)
        {
            Button btn;
            CDoorStatus locDoorStatus = null;

            btn = (Button)sender;
            if (e.Button == MouseButtons.Right)
            {
                CurrentButtonTagClicked = Convert.ToInt32(btn.Tag);
                locDoorStatus = GetDoorStatus(CurrentButtonTagClicked);
                if (locDoorStatus != null)
                {
                    locDoorStatus.UpdateDoorStatus();
                    if (locDoorStatus.AlarmActive())
                    {
                        acnowledgeToolStripMenuItem.Enabled = true;
                        if (locDoorStatus.IsAlarmClearable())
                        {
                            clearToolStripMenuItem.Enabled = true;
                        }
                        else
                        {
                            clearToolStripMenuItem.Enabled = false;
                        }
                        contextMenuStrip1.Show(this, new Point(TabFloorPlan.Bounds.X + btn.Location.X, TabFloorPlan.Bounds.Y + btn.Location.Y));
                    }
                    else
                    {
                        CurrentButtonTagClicked = 0;
                    }
                }
                else
                {
                    CurrentButtonTagClicked = 0;
                }
                
            }
        }


        /* the old logAlarm(CDoorStatus DrSts)
         private void logAlarm(CDoorStatus DrSts)
        {
            String sLogMessage = "";

            sLogMessage = DateTime.Now.ToString() + " : Door " + DrSts.iDoorID.ToString();
            if (DrSts.DoorOpen())
            {
                sLogMessage = sLogMessage + " openned,";
            }
            else
            {
                sLogMessage = sLogMessage + " closed,";
            }

            if (!DrSts.AlarmAcknowledged())
            {
                sLogMessage = sLogMessage + " Not acknowledged,";
            }
            else
            {
                sLogMessage = sLogMessage + " acknowledged,";
            }

            if (!DrSts.AlarmCleared())
            {
                sLogMessage = sLogMessage + " not cleared.";
            }
            else
            {
                sLogMessage = sLogMessage + " cleared.";
            }

            listBox1.Items.Add(sLogMessage);
            EvtLog.LogWarningEvent(sLogMessage);

            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.ShowBalloonTip(10000, "Door State Notification", sLogMessage, ToolTipIcon.Warning);
            }
        } 
         */
        private void logAlarm()
        {

            String sLogMessage = "";
            string sSpace = " ";
            byte bStatus;

            bool bPLCWritingToLog = TCInterface.GetBool(".bWritingToLog");
            if (!bPLCWritingToLog)
            {
                byte uiLatsLog = TCInterface.GetInt8(".iNextLog");
                byte uiCurrentLog; 

                // Read the Logs till the iNextLog

                for (uiCurrentLog = 1; uiCurrentLog < uiLatsLog; uiCurrentLog++)
                {
                    sLogMessage = "";
                    sLogMessage = TCInterface.GetDateTime(".aLogs[" + uiCurrentLog + "].dtLogTime").ToString();                     // write: Date & Time                    
                    sLogMessage = sLogMessage + ": Door";                                                                           // write: Door
                    sLogMessage = sLogMessage + sSpace;                                                                             // write: space
                    sLogMessage = sLogMessage + TCInterface.GetInt8(".aLogs[" + uiCurrentLog + "].bDoorID").ToString();           // write: Door_ID
                    sLogMessage = sLogMessage + sSpace;                                                                             // write: space

                    bStatus = TCInterface.GetInt8(".aLogs[" + uiCurrentLog + "].bStatus");                                          // get the Door's Status

                    if ((bStatus & 8) != 0)                                                                                         //  bStatus[3] == 1 -> Sensor is out of coverage
                    {
                        sLogMessage = sLogMessage + " is out of coverage, it was";
                    }

                    if ((bStatus & 1) != 0)                                                                                         // bStatus[0] == 1 -> the Door is opened 
                    {
                           sLogMessage = sLogMessage + "openned,";                                                                  // write: opened,

                    }
                    else                                                                                                            // door is not opened closed
                    {
                        sLogMessage = sLogMessage + " closed,";
                    }

                    if ((bStatus & 2) != 0)                                                                                         // bStatus[1] == 1 -> Alarm is Not Acknowledged
                    {
                        sLogMessage = sLogMessage + " Not acknowledged,";

                    }
                    else
                    {
                        sLogMessage = sLogMessage + " acknowledged,";
                    }

                    if ((bStatus & 4) != 0)                                                                                         //  bStatus[2] == 1 -> Alarm is Not Cleared
                    {
                        sLogMessage = sLogMessage + " not cleared.";
                    }
                    else
                    {
                        sLogMessage = sLogMessage + " cleared.";
                    }                                        

                    listBox1.Items.Add(sLogMessage);
                    EvtLog.LogWarningEvent(sLogMessage);

                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        notifyIcon1.ShowBalloonTip(10000, "Door State Notification", sLogMessage, ToolTipIcon.Warning);
                    }
                }   //  End For 

                // Update iNextLog -> write to PLC
                TCInterface.SetByte(".iNextLog", 0);
            }
        }

        private void tmrFlashing_Tick(object sender, EventArgs e)
        {

            if (bConnectionError)
            {
                btnSts.Text = "Connection Error";
                if (bBlinkingStateVariable)
                {
                    btnSts.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    btnSts.BackColor = System.Drawing.Color.White;
                }
                toolTip1.SetToolTip(btnSts, "Connection to controller is not established. Check network connection or that controller is powered up. Contact IA if problem persist.");
            }
            else
            {
                btnSts.Text = "Connected to PLC";
                btnSts.BackColor = System.Drawing.Color.Green;
                toolTip1.SetToolTip(btnSts, "Connection to controller established.");
            }

            bConnectionError = false;
            foreach (CDoorStatus doorSts in lstDoorStatus)
            {
                if (!bConnectionError)
                {
                    if (doorSts.LinkedButton != null)
                    {
                        if (doorSts.SensorOutOfCoverage())
                        {                            
                            if (bBlinkingStateVariable)
                            {
                                doorSts.LinkedButton.BackColor = System.Drawing.Color.Gray;
                            }
                            else
                            {
                                doorSts.LinkedButton.BackColor = doorSts.BtnColor;
                            }
                        }
                        else
                        {
                            if (doorSts.DoorOpen())
                            {
                                if (bBlinkingStateVariable)
                                {
                                    doorSts.LinkedButton.BackColor = System.Drawing.Color.White;
                                }
                                else
                                {
                                    doorSts.LinkedButton.BackColor = doorSts.BtnColor;
                                }
                            }
                            else
                            {
                                doorSts.LinkedButton.BackColor = doorSts.BtnColor;
                            }
                        }
                    }
                }

                if (doorSts.AlarmActive() && (!doorSts.AlarmAcknowledged()))
                {
                    logAlarm();
                    lstDoorStatusAlarmActive.Add(doorSts);
                    doorSts.bAlarmLogged = true;
                    if ((doorSts.iDoorID == 37) | (doorSts.iDoorID == 38) | (doorSts.iDoorID == 39))
                    {
                        TabFloorPlan.SelectedTab = TabFloorPlan.TabPages[1];
                    }
                    else
                    {
                        if ((doorSts.iDoorID == 29) | (doorSts.iDoorID == 41))
                        {
                            TabFloorPlan.SelectedTab = TabFloorPlan.TabPages[2];
                        }
                        else
                        {
                            TabFloorPlan.SelectedTab = TabFloorPlan.TabPages[0];
                        }
                    }
                }

                /*
                List<CDoorStatus> lstToRemove = new List<CDoorStatus>();

                foreach (CDoorStatus drSts in lstDoorStatusAlarmActive)
                {
                    if (!drSts.DoorOpen())
                    {
                        logAlarm(drSts);
                        lstToRemove.Add(drSts);

                    }
                }

                foreach (CDoorStatus ds in lstToRemove)
                {
                    lstDoorStatusAlarmActive.Remove(ds);
                }
                */
            }

            bBlinkingStateVariable = !bBlinkingStateVariable;
        }        

        private void btnClrLog_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            frmAbout formAbout = new frmAbout();
            formAbout.Show();
        }
                
    }
}
