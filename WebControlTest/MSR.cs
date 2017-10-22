using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;


namespace ForeFuelSimulator
{
    public class MSR
    {
        static ContainerControl m_sender = null;
        static Delegate m_senderDelegate = null;

        //private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public Logger myLog = new Logger("MSR.log");


        public string lastReference = "";

        
        MSRStatusData stat = new MSRStatusData();

        Config conf = null;

        int SelectedStation;

         public MSR(ContainerControl sender, Delegate senderDelegate,Config Conf)
        {
            m_sender = sender;
            m_senderDelegate = senderDelegate;
            conf = Conf;
         
        }

         public string card = "";
         public string Card = "";

         public bool FoundCard = false;

         public int ProductCode = 0;

        public void SetInfo(int SelectedStation,int ProductCode)
        {
            this.SelectedStation = SelectedStation;
            this.ProductCode = ProductCode;
        }


        string GetStationCode()
        {
            string code = "";

            switch (SelectedStation)
            {
                case 1: code = conf.Station1Code; break;
                case 2: code = conf.Station2Code; break;
                case 3: code = conf.Station3Code; break;
            }

            return code;

        }

        

         public void Worker()
         {
             string c = "";
             while (true)
             {
                 try
                 {
                     Thread.Sleep(5);
                     if (FoundCard)
                     {
                         myLog.Log("Invoked found card");
                         FoundCard = false;
                         int ep = Card.IndexOf('=');
                         if (ep != -1)
                         {
                             c = Card.Substring(0, Card.IndexOf('='));
                         }
                         else
                             c = Card;

                        stat.carddata = c;
                        myLog.Log("Valid card, update GUI: " + c);
                        stat.msg = MsgLogType.MSRRead;
                        m_sender.BeginInvoke(m_senderDelegate, stat);

                            
                        Thread.Sleep(50);
                        stat.msg = MsgLogType.MSRRequestAuth;
                        m_sender.BeginInvoke(m_senderDelegate, stat);
                        Thread.Sleep(1000);

                        myLog.Log("Going to request auth: " + c);
                        //get auth
                        LoyaltyService.LoyaltyServiceClient s = new LoyaltyService.LoyaltyServiceClient();
                        LoyaltyService.AuthResult res = new LoyaltyService.AuthResult();
                        try
                        {
                            res = s.GetAuth(c, GetStationCode());
                        }
                        catch (Exception ex)
                        {
                            myLog.Log("Error GetAuth - server down.");
                            stat.msg = MsgLogType.CommErr;
                            m_sender.BeginInvoke(m_senderDelegate, stat);
                            continue;
                        }
                        //MessageBox.Show(res.DriverName + " " + res.Limit.ToString() + " " + res.ProductsCode[0].ToString());
                        myLog.Log("Got auth replay: " + res.Allowed.ToString() + ":" + res.DriverName+":"+res.ErrorDesc);
                        stat.ErrorDesc = res.ErrorDesc;
                        stat.carddata = c;
                        stat.ProductsCode = res.ProductsCode;
                        stat.Discount = res.Discount;
                        stat.Reference = res.Reference;
                        if (res.Allowed)
                        {
                            //check correct product
                            bool found = false;
                            foreach (int code in res.ProductsCode)
                            {
                                if (code == ProductCode)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found && ProductCode!=0)
                            {
                                stat.msg = MsgLogType.MSRWrongProduct;
                            }
                            else
                            {
                                stat.msg = MsgLogType.MSRAuth;
                                lastReference = res.Reference;
                            }
                            
                        }
                        else
                            stat.msg = MsgLogType.MSRReject;
                        stat.DriverName = res.DriverName;
                        stat.Limit = res.Limit;
                        m_sender.BeginInvoke(m_senderDelegate, stat);
                     }
                 }
                 catch( Exception ex)
                 {
                     MessageBox.Show(ex.Message);
                 }
             }
         }

         
    }
}
