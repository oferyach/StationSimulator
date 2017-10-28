using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Windows.Forms;

namespace ForeFuelSimulator
{
    public class DeviceData
    {
        public string cardnum;
        public double limit;
        public bool bEnabled;
        public string plate;
        public string type;
        public string prod;
        public bool bCPass;
    }
    public class Config
    {
        //config data
        public bool UseWGT;
        public string IP;
        public string AppTitle;
        public int port;
        public int Addr = 0x32;
        public double PPV1 = 1.0;
        public double PPV2 = 1.0;
        public string Product1 = "Prod1";
        public string Product2 = "Prod2";
        public int Prod1Code = 0;
        public int Prod2Code = 0;
        public int Wash1Code = 0;
        public int Wash2Code = 0;
        public string Units = "Units";
        public double flowrate = 60.0;
        public string LimitText = "Limit";
        public string PlateText = "Plate";
        public string LimitPerVolText = "Limit/Tnx";
        public string LimitPerMoneyText = "Money/Tnx";
        public int HOAuthDelay = 2000;
        public int PumpAuthDelay = 1500;
        public bool UseCPass = false;
        public string CPassServer = "";
        public string CPassKey = "";
        public int CPassRetry = 5;

        public int num_of_device = 0;

        public List<DeviceData> DB = new List<DeviceData>();

        //MSR section
        public bool UseMSR;
        public string MSRService;

        public string Station1Code = "1";
        public string Station2Code = "2";
        public string Station3Code = "3";
        public string Station4Code = "4";

        public Config()
        {
            //read config file
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("config.xml");

                if (xDoc != null)
                {
                    AppTitle = xDoc.SelectSingleNode("Configuration/General/AppTitle").InnerXml.ToString();
                    //WGT data
                    UseWGT = bool.Parse(xDoc.SelectSingleNode("Configuration/General/WGT/Use").InnerXml.ToString());
                    IP = xDoc.SelectSingleNode("Configuration/General/WGT/IP").InnerXml.ToString();
                    port = Int32.Parse(xDoc.SelectSingleNode("Configuration/General/WGT/Port").InnerXml.ToString());
                    Addr = Convert.ToInt32(xDoc.SelectSingleNode("Configuration/General/WGT/Addr").InnerXml.ToString(), 16);
                    PPV1 = Double.Parse(xDoc.SelectSingleNode("Configuration/General/PPV1").InnerXml.ToString());
                    PPV2 = Double.Parse(xDoc.SelectSingleNode("Configuration/General/PPV2").InnerXml.ToString());
                    flowrate = Double.Parse(xDoc.SelectSingleNode("Configuration/General/FlowRate").InnerXml.ToString());
                    PumpAuthDelay = Int32.Parse(xDoc.SelectSingleNode("Configuration/General/PumpAuthDelay").InnerXml.ToString());
                    HOAuthDelay = Int32.Parse(xDoc.SelectSingleNode("Configuration/General/HOAuthDelay").InnerXml.ToString());
                    Product1 = xDoc.SelectSingleNode("Configuration/General/Prod1").InnerXml.ToString();
                    Product2 = xDoc.SelectSingleNode("Configuration/General/Prod2").InnerXml.ToString();
                    Prod1Code = Int32.Parse(xDoc.SelectSingleNode("Configuration/General/Prod1Code").InnerXml.ToString());
                    Prod2Code = Int32.Parse(xDoc.SelectSingleNode("Configuration/General/Prod2Code").InnerXml.ToString());
                    Wash1Code = Int32.Parse(xDoc.SelectSingleNode("Configuration/General/Wash1Code").InnerXml.ToString());
                    Wash2Code = Int32.Parse(xDoc.SelectSingleNode("Configuration/General/Wash2Code").InnerXml.ToString());
                    Units = xDoc.SelectSingleNode("Configuration/General/Units").InnerXml.ToString();


                    //MSR section
                    UseMSR = bool.Parse(xDoc.SelectSingleNode("Configuration/General/MSR/Use").InnerXml.ToString());
                    MSRService = xDoc.SelectSingleNode("Configuration/General/MSR/Service").InnerXml.ToString();

                    Station1Code = xDoc.SelectSingleNode("Configuration/General/Station1Code").InnerXml.ToString();
                    Station2Code = xDoc.SelectSingleNode("Configuration/General/Station2Code").InnerXml.ToString();
                    Station3Code = xDoc.SelectSingleNode("Configuration/General/Station3Code").InnerXml.ToString();
                    Station4Code = xDoc.SelectSingleNode("Configuration/General/Station4Code").InnerXml.ToString();

                    LimitText = xDoc.SelectSingleNode("Configuration/General/LimitText").InnerXml.ToString();
                    PlateText = xDoc.SelectSingleNode("Configuration/General/PlateText").InnerXml.ToString();
                    LimitPerVolText = xDoc.SelectSingleNode("Configuration/General/LimitPerVolText").InnerXml.ToString();
                    LimitPerMoneyText = xDoc.SelectSingleNode("Configuration/General/LimitPerMoneyText").InnerXml.ToString();

                    //CPass server data 
                    UseCPass = bool.Parse(xDoc.SelectSingleNode("Configuration/General/CPass/Use").InnerXml.ToString());
                    CPassServer = xDoc.SelectSingleNode("Configuration/General/CPass/Server").InnerXml.ToString();
                    CPassKey = xDoc.SelectSingleNode("Configuration/General/CPass/Key").InnerXml.ToString();
                    CPassRetry = Int32.Parse(xDoc.SelectSingleNode("Configuration/General/CPass/Retry").InnerXml.ToString());

                    // devices data
                    //XmlNode node = xDoc.SelectSingleNode("Configuration/Devices").FirstChild.ChildNodes;
                    foreach (XmlNode node in xDoc.SelectSingleNode("Configuration/Devices").ChildNodes)
                    {
                        DeviceData dd = new DeviceData();
                        foreach (XmlNode node1 in node)
                        {

                            switch (node1.Name)
                            {
                                case "CardNum":
                                    dd.cardnum = node1.InnerXml.ToString();
                                    break;
                                case "Plate":
                                    dd.plate = node1.InnerXml.ToString();
                                    break;
                                case "Type":
                                    dd.type = node1.InnerXml.ToString();
                                    break;
                                case "Limit":
                                    dd.limit = Int32.Parse(node1.InnerXml.ToString());
                                    break;
                                case "Prod":
                                    dd.prod = node1.InnerXml.ToString();
                                    break;
                                case "Enabled":
                                    if (node1.InnerXml == "true")
                                        dd.bEnabled = true;
                                    else
                                        dd.bEnabled = false;
                                    break;
                                case "CPass":
                                    if (node1.InnerXml == "true")
                                        dd.bCPass = true;
                                    else
                                        dd.bCPass = false;
                                    break;

                            }
                        }
                        DB.Add(dd);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading configuration file. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}
