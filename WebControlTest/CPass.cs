using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Collections;
using System.Web.Script.Serialization;

namespace ForeFuelSimulator
{
    public partial class Main : Form
    {
        //CPass
        HttpWebRequest request;

        string MobileAction = "";

        public string cPassKey = "";


        void SendMobileTokenRequest()
        {
            try
            {
                MobileAction = "Token";
                //request = (HttpWebRequest)HttpWebRequest.Create(conf.CPassServer + "/Api/Authorization/CpassAuth1Request");
                request = (HttpWebRequest)HttpWebRequest.Create("http://cpass2ndauth.azurewebsites.net/API/token");
                request.ContentType = "application/json; charset=utf-8";
                request.Accept = "application/json, text/javascript, */*";
                request.Method = "POST";
                request.Headers.Add("Authorization", "Bearer Gg5lvEloTlZZcifbG7IjQ2DvMdWLnD1AQ0Ip7NPhXAHFkr8OFDNK0EnQVl5jz0194H_kL981GaAV5jLDGJkYJ_xuqrfXraLzzWlZXopItVRM6WrsmNLuDrc20i6Z05cQ45MF13FgtlPmKqvhoX808fyOMo61cxESO3iJam1KbQvNiHd7qQRtF870FglgYehJ1WbJkEg0-8ADWUPei6-o7Licwz5sYZrLuX82X7rtpLHtlR8TAInHwLdYfcXi6J8XBHC39yA4a-OHm7iwLmfcAnVa7q6HgvjszW_p44d2CVrjqYioTA7KO4jh8eEGN6AV");  //was conf.CPassKey
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write("grant_type=client_credentials&client_id=696c8a38-cc39-4658-8351-e78f31d9e275&client_secret=IXWNkRpAhPEbEw8dzzg9oomQ0En3EPCO6rqacacdvMIlOdALUZaEk00Kiij6lejUDHW7nLrpvvZhm%2FF9PDu03WwEIDEGJcmMaXmlnbA9PlYrJd2HzBPkuX6%2B54Zq31Dkti5%2FvPZscjgeJINTNpxd9nP85u0Io9omnl3d3pqjj%2B%2B6pVWyXwccJtIMRCyVLHhnBwTLBoNb39DLfHrb5Glrrw%3D%3D");                                   
                }
                request.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
            }
            catch (Exception ex)
            {
                myLog.Log(ex.Message);
            }
        }

        //send request for mobile auth
        void SendMobileRequest(string DeviceID, int MsgID)
        {
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(conf.CPassServer + "/Api/Authorization/CpassAuth1Request");
                request.ContentType = "application/json; charset=utf-8";
                request.Accept = "application/json, text/javascript, */*";
                request.Method = "POST";
                request.Headers.Add("Authorization", "Bearer " + cPassKey);
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {

                    writer.Write(@"{""requestId"": """ +
                                 MsgID.ToString() +
                                 @""",""dateTime"": """ +
                                 dt +
                                 @""",""cpass_UTN"":""" +
                                 DeviceID +
                                 @""",""tagid"": """ +
                                 DeviceID +
                                 @""",""pumP_ID"": ""1"",""fuel_Type_code"": ""0"",""nR_ID"": ""101"",""odometer"": ""0"",""engine_Hours"": ""0""}");

                    /*
                    writer.Write(@"{""requestId"": """ +
                                 MsgID.ToString() +
                                 @""",""dateTime"": ""2017-07-02T17:18:07.829+03:00"",""cpass_UTN"":""" +
                                 DeviceID +
                                 @""",""tagid"": """ +
                                 DeviceID +
                                 @""",""pumP_ID"": ""0"",""fuel_Type_code"": ""0"",""nR_ID"": ""101"",""odometer"": ""0"",""engine_Hours"": ""0""}");
                     */
                }
                request.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
            }
            catch (Exception ex)
            {
                myLog.Log(ex.Message);
            }
        }

        void SendMobilePollRequest(string DeviceID, int MsgId)
        {
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(conf.CPassServer + "/Api/Authorization/CpassAuth1Poll");
                request.ContentType = "application/json; charset=utf-8";
                request.Accept = "application/json, text/javascript, */*";
                request.Method = "POST";
                request.Headers.Add("Authorization", "Bearer " + conf.CPassKey);
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(@"{""requestId"": """ +
                                 MsgID.ToString() +
                                 @""",""dateTime"": """ +
                                 dt +
                                 @""",""cpass_UTN"":""" +
                                 DeviceID +
                                 @""",""tagid"": """ +
                                 DeviceID +
                                 @""",""pumP_ID"": ""1"",""fuel_Type_code"": ""0"",""nR_ID"": ""101"",""odometer"": ""0"",""engine_Hours"": ""0""}");
                }
                request.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
            }
            catch (Exception ex)
            {
                myLog.Log(ex.Message);
            }
        }

        void SendMobileConfirm(string DeviceID, int MsgId)
        {
            try
            {

                request = (HttpWebRequest)HttpWebRequest.Create(conf.CPassServer + "/Api/Authorization/CpassAuth1Confirm");
                request.ContentType = "application/json; charset=utf-8";
                request.Accept = "application/json, text/javascript, */*";
                request.Method = "POST";
                request.Headers.Add("Authorization", "Bearer " + conf.CPassKey);
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(@"{""requestId"": """ +
                                 MsgID.ToString() +
                                 @""",""result"": ""OK""}");
                }
                request.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
            }
            catch (Exception ex)
            {
                myLog.Log(ex.Message);
            }
        }


        void SendMobileSummary(string DeviceID, int MsgID)
        {
            try
            {
                Thread.Sleep(1000);
                String dt = DateTime.Now.ToString();
                request = (HttpWebRequest)HttpWebRequest.Create(conf.CPassServer + "/Api/Authorization/CpassAuth1Summary");
                request.ContentType = "application/json; charset=utf-8";
                request.Accept = "application/json, text/javascript, */*";
                request.Method = "POST";
                request.Headers.Add("Authorization", "Bearer " + conf.CPassKey);
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(@"{""requestId"": """ +
                                 MsgID.ToString() +
                                 @""",""dateTime"": """ +
                                 dt +
                                 @""",""cpass_UTN"":""" +
                                 DeviceID +
                                 @""",""tagid"": """ +
                                 DeviceID +
                                 @""",""pumP_ID"": ""1"",""fuel_Type_code"": ""1"",""nR_ID"": ""101"",""odometer"": ""23420"",""engine_Hours"": ""0""" +
                                 @",""Fuel_type_name"":""" + "Diesel" +
                                 @""",""Volume"": """ + lastvol.ToString("0.00") +
                                 @""",""Amount"": """ + lastamount.ToString("0.00") +
                                 @""",""Price"": """ + pup.data.PPV.ToString("0.00") +
                                 @"""}");
                }
                request.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
            }
            catch (Exception ex)
            {
                myLog.Log(ex.Message);
            }
        }


        void FinishWebRequest(IAsyncResult result)
        {
            try
            {
                WebResponse response = request.EndGetResponse(result);

                Stream stream = response.GetResponseStream();

                string json = "";

                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        json += reader.ReadLine();
                    }
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dynamic item = serializer.Deserialize<object>(json);
                

                if (MobileAction == "Token")
                {
                    cPassKey = item["access_token"];
                }
                else
                    MobileRes = item["request_Status"];


                MobileAction = ""; //reset action

            }
            catch (Exception ex)
            {
                myLog.Log(ex.Message);
            }
        }

        //CPass
    }
}
