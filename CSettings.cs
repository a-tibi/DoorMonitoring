using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace MarsDoorMonitoring
{
    class CDoorSettings
    {
        public int iDoorID;
        public int iPLC_ID;
        public string sDescription;
        public Button LinkedButton;

    }

    class CSettings
    {
        
        private bool _bFileMissing;
        private string _sFileName;
        private CEventLog _cEventLog;

        public List<CDoorSettings> lstDoorSettings;
        public string sAMSNetID;
        public int iPortNo;



        public CSettings(string sFileName)
        {
            lstDoorSettings = new List<CDoorSettings>();
            _sFileName = sFileName;
            try
            {
                if (File.Exists(_sFileName))
                {
                    _bFileMissing = false;
                }
                else
                {
                    _cEventLog.LogErrorEvent("XML settings file is missing.");
                    _bFileMissing = true;
                }
                
            }
            catch
            {
                _cEventLog.LogErrorEvent("Exception caught while creating instance of XML reader. Check presence of xml settings file.");
                _bFileMissing = true;
            }
        }

        public string GetDescription(int iID)
        {
            string sRet="";

            foreach(CDoorSettings doorSettings in lstDoorSettings)
            {
                if (doorSettings.iDoorID == iID) sRet = doorSettings.sDescription;
            }

            return sRet;
        }

        public void SetButton(int iID,Button linkedBtn)
        {
            

            foreach (CDoorSettings doorSettings in lstDoorSettings)
            {
                if (doorSettings.iDoorID == iID) doorSettings.LinkedButton = linkedBtn;
            }

           
        }

        public bool UpdateSettings()
        {
            bool bRet = false;

            if (!_bFileMissing)
            {
                XmlTextReader XMLReader;
                string sValue;
                string sElemtName = "";
                CDoorSettings locDoorSettings = new CDoorSettings();

                XMLReader = new XmlTextReader(_sFileName);

                while (XMLReader.Read())
                {
                    switch (XMLReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            sElemtName = XMLReader.Name;
                            break;
                        case XmlNodeType.Text:
                            sValue = XMLReader.Value;
                            if (sElemtName == "AMS_NET_ID")
                                sAMSNetID = sValue;
                            if (sElemtName == "PORT_NO")
                                iPortNo = Convert.ToInt32(sValue);
                            if (sElemtName == "DOOR_ID")
                            {
                                locDoorSettings = new CDoorSettings();
                                locDoorSettings.iDoorID = Convert.ToInt32(sValue);
                            }
                            if (sElemtName == "PLC_DOOR_ID")
                            {
                                locDoorSettings.iPLC_ID = Convert.ToInt32(sValue);
                            }
                            if (sElemtName == "DOOR_DESCRIPTION")
                            {
                                locDoorSettings.sDescription = sValue;
                                lstDoorSettings.Add(locDoorSettings);
                                bRet = true;
                            }
                            break;
                    }
                }


            }

            return bRet;
        }
    }
}
