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
using System.ServiceModel;

namespace ForeFuelSimulator
{
    public partial class Main : Form
    {
        static System.Windows.Forms.Timer MSRTimer = new System.Windows.Forms.Timer();

        private void MSRStuff()
        {
            MSRTimer.Tick += new EventHandler(MSRTimerEventProcessor);
            // Sets the timer interval to 2 seconds.
            MSRTimer.Interval = conf.HOAuthDelay;
        }
        private void MSRTimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            MSRTimer.Stop();
            //MessageBox.Show("Timer done");
            if (ProductCode == conf.Wash1Code || ProductCode == conf.Wash2Code)
            {
                AnimationPhase = 2;
                AnimationTimer.Start();
                WashPlayer.PlayLooping();
                (MainPage.Document.GetElementById("Plate")).InnerHtml = "Welcome " + dd.plate;
            }
            else if (true)//CheckAuth(cardtoauth, ActiveNozz, ref limit, ref type, ref plate, ref reason))
            {
                ps = PumpStatus.InUse;
                pup.ResetVol(conf.PumpAuthDelay);
                pup.SetStatus(ps, dd.limit, dd.type, dd.plate, conf.flowrate, -1);
                //AddToLogList(MsgLogType.Authorized, reason, 0, plate, limit, dd.cardnum);
                (MainPage.Document.GetElementById("Plate")).InnerHtml = "Welcome "+dd.plate;
                if (dd.type == "Money")
                    (MainPage.Document.GetElementById("Limit")).InnerHtml = dd.limit.ToString("0") + " " + conf.LimitPerMoneyText;
                else
                    (MainPage.Document.GetElementById("Limit")).InnerHtml = dd.limit.ToString("0") + " " + conf.LimitPerVolText;
            }
            else if (reason == "CPass required")
            {
                AddToLogList(MsgLogType.CheckCPass, reason, 0, plate, limit, cardtoauth);
                MSRTimer.Start();  //recheck in as few seconds
            }
            else if (reason == "CPass wait")
            {
                //AddToLogList(MsgLogType.CheckCPass, reason, 0, plate, limit, cardtoauth); //no second message
                if (CPassTry == conf.CPassRetry)
                {
                    reason = "No mobile confirmation";
                    if (statustouse != 0x30)
                        AddToLogList(MsgLogType.CannotAuth, reason, 0, plate, limit, gotmsg.carddata);
                    return;
                }
                CPassTry++;
                MSRTimer.Start();  //recheck in as few seconds
            }
            else
            {
                if (statustouse != 0x30)
                    AddToLogList(MsgLogType.CannotAuth, reason, 0, plate, limit, gotmsg.carddata);
            }

        }

        MSRStatusData msgSaved = null;

        public bool MSRSuspendedAuth()
        {
            if (msgSaved !=null)
            {
                //check for correct nozzle
                bool found = false;
                double discount = 0;
                string discounttype = "";
                foreach (MyProductItem item in msgSaved.ProductsList)
                {
                    if (item.Code == ProductCode)
                    {
                        discount = item.Discount;
                        discounttype = item.DiscountType;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    AddToLogList(MsgLogType.MSRWrongProduct, "", 0, "", 0, "");
                    return false;
                }
                            
                dd = new DeviceData();
                dd.bCPass = false;
                dd.cardnum = msgSaved.carddata;
                dd.limit = msgSaved.Limit;
                dd.plate = msgSaved.DriverName;
                dd.type = "Money";
                MobileRes = "None";
                CPassTry = 0;
                MsgID++;
                //find the product discount
                found = false;
                MyProductItem itemfound = null;
                foreach (MyProductItem item in msgSaved.ProductsList)
                {
                    if (item.Code == ProductCode)
                    {
                        found = true;
                        itemfound = item;
                        break;
                    }
                }
                if (found)
                {
                    //check limit type
                    if (itemfound.DiscountType == "%")
                        UpdatePPVPrecent(itemfound.Discount);
                    else
                        UpdatePPVAbs(itemfound.Discount);

                }
                MSRTimer.Start();
                return true;
            }
            return false;
        }

        private void ShowMSRProgress(MSRStatusData msg)
        {
            Thread.BeginCriticalRegion();

            try
            {
                if (msg.status == -1)
                {
                    AddToLogList(MsgLogType.MSRCommError, "", 0, "", 0, msg.carddata);
                    return;
                }


                if ((ProductCode == conf.Wash1Code || ProductCode == conf.Wash2Code) && msg.msg == MsgLogType.MSRWrongProduct) //we are in wash?
                {
                    if (ProductCode == conf.Wash1Code)
                        Wash1Clicked(); //simulate relese
                    else
                        Wash2Clicked();                    
                }

                if (msg.msg != MsgLogType.MSRAuth) //just update the status
                {
                    AddToLogList(msg.msg, msg.ErrorDesc, 1, msg.DriverName, msg.Limit, msg.carddata);
                    msgSaved = null;
                    return; 
                }

                if (ProductCode == conf.Wash1Code || ProductCode == conf.Wash2Code) //we are in wash?
                {
                    AnimationPhase = 2;
                    WashInProgress = true;
                    AnimationTimer.Start();
                    WashPlayer.PlayLooping();
                    (MainPage.Document.GetElementById("Plate")).InnerHtml = "Welcome " + msg.DriverName;
                    return;
                }

                //we got authorization check if pump is ready
                
                switch (ps)
                {
                    case PumpStatus.Idle:
                            AddToLogList(msg.msg, msg.ErrorDesc, 1, msg.DriverName, msg.Limit, msg.carddata);
                            AddToLogList(MsgLogType.MSRSSelectProduct, "", 0, "", 0, "");
                            //save auth data
                            msgSaved = msg;
                        
                        break;
                    case PumpStatus.Call:
                        AddToLogList(msg.msg, msg.ErrorDesc, 1, msg.DriverName, msg.Limit, msg.carddata);
                        if (msg.msg == MsgLogType.MSRAuth)
                        {
                            dd = new DeviceData();
                            dd.bCPass = false;
                            dd.cardnum = msg.carddata;
                            dd.limit = msg.Limit;
                            dd.plate = msg.DriverName;
                            dd.type = msg.LimitType;
                            dd.bCPass = msg.CPassRequired;                            
                            MobileRes = "None";
                            CPassTry = 0;
                            MsgID++;
                            //find the product discount
                            bool found = false;
                            MyProductItem itemfound = null;
                            foreach (MyProductItem item in msg.ProductsList)
                            {
                                if (item.Code == ProductCode)
                                {
                                    found = true;
                                    itemfound = item;
                                    break;
                                }
                            }
                            if (found)
                            {
                                //check limit type
                                if (itemfound.DiscountType == "%")
                                {
                                    UpdatePPVPrecent(itemfound.Discount);
                                    (MainPage.Document.GetElementById("Discount")).InnerHtml = "Discount " + itemfound.Discount.ToString("00")+"%";
                                }
                                else
                                {
                                    UpdatePPVAbs(itemfound.Discount);
                                    (MainPage.Document.GetElementById("Discount")).InnerHtml = "Discount " + itemfound.Discount.ToString("00") + "c";
                                }

                            }
                            
                            MSRTimer.Start();
                        }

                        break;
                }


            }
            catch (Exception ex)
            {
                //  MessageBox.Show(ex.Message);
            }
            finally
            {
                Thread.EndCriticalRegion();
            }
        }


        public void SendTransactionComplete()
        {
            
            LoyaltyService.TransactionCompleteResult res = new LoyaltyService.TransactionCompleteResult();
            var myBinding = new BasicHttpBinding();
            myBinding.Security.Mode = BasicHttpSecurityMode.None;
            var myEndpointAddress = new EndpointAddress(conf.MSRService);
            LoyaltyService.LoyaltyServiceClient s = new LoyaltyService.LoyaltyServiceClient(myBinding, myEndpointAddress);

            try
            {
                  
                res = s.TransactionComplete(msr.lastReference, lastamount, lastvol, msr.ProductCode,DateTime.Now);
            }

            catch (Exception ex)
            {
                myLog.Log("Cannot report transaction - server down.");
            }
        }
    }
}
