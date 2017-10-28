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
    public enum PumpStatus
    {
        Idle,
        Call,
        Stopped,
        InUse,
        Ready,
        Payable,
        Error
    };
    
    public partial class Main : Form
    {
        Thread myThread = null;

        string fuellingcardnumber = "";

        PumpStatus ps = PumpStatus.Idle;

        Config conf = null;


        bool bLastCardFueled = false;
        bool bStoped = false;
        bool bHandle = false;

        delegate void ShowProgressDelegate(StatusData msg);
        delegate void ShowMSRProgressDelegate(MSRStatusData msg);

        delegate void ShowPumpProgressDelegate(PumpUpdateData data);

        PumpUpdater pup = null;

        int ActiveNozz = 0;

        public Logger myLog = new Logger("App.log");

        public StatusData laststatus = new StatusData();
        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

        static System.Windows.Forms.Timer AnimationTimer = new System.Windows.Forms.Timer();
        System.Media.SoundPlayer WashPlayer = new System.Media.SoundPlayer();
        

        string MobileRes = "None";
        int CPassTry = 0;

        Random rnd = new Random();
        int MsgID = 1;


        

        public string StationCode = "A1234";
        MSR msr = null;
        
        public Main()
        {
            InitializeComponent();
            
            //load the HTML
            MainPage.ObjectForScripting = new ScriptManager(this);
            string curDir = Directory.GetCurrentDirectory();
            var url = new Uri(String.Format("file:///{0}/{1}", curDir, "App.html"));
            MainPage.Navigate(url);

            myLog.Log("Application started : Version 1.2.0.0");
            //load configuration
            conf = new Config();


            laststatus.carddata = "";
            laststatus.status = -2;

            AnimationTimer.Tick += new EventHandler(AnimationTimerPorcessor);
            AnimationTimer.Interval = 1000; //1 seconds
            WashPlayer.SoundLocation = "Carwash.wav";

            myTimer.Tick += new EventHandler(TimerEventProcessor);
            // Sets the timer interval to 2 seconds.
            myTimer.Interval = conf.HOAuthDelay;

            this.Text = conf.AppTitle;

            MsgID = rnd.Next(1, 100000);

            if (conf.UseCPass)
                SendMobileTokenRequest();

            MSRStuff();

            
        }


        public void StartComm()
        {
            //pump updater
            ShowPumpProgressDelegate showPumpProgress = new ShowPumpProgressDelegate(ShowPumpProgress);
            pup = new PumpUpdater(this, showPumpProgress);
            pup.SetPPV(conf.PPV1, conf.PPV2);
            Thread t1 = new Thread(new ThreadStart(pup.Worker));
            //make them a daemon - prevent thread callback issues
            t1.IsBackground = true;
            t1.Start();

            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);
            Client client = new Client(conf.IP, conf.port, conf.Addr, this, showProgress);

            if (conf.UseWGT)
            {
                Thread t = new Thread(new ThreadStart(client.Worker));
                //make them a daemon - prevent thread callback issues
                t.IsBackground = true;
                t.Start();
            }

            //start MSR process
            ShowMSRProgressDelegate showMSRProgress = new ShowMSRProgressDelegate(ShowMSRProgress);
            msr = new MSR(this, showMSRProgress, conf);
            msr.SetInfo(SelectedStation,0);


            if (conf.UseMSR)
            {
                Thread t2 = new Thread(new ThreadStart(msr.Worker));
                //make them a daemon - prevent thread callback issues
                t2.IsBackground = true;
                t2.Start();
            }
        }

        public double lastvol = 0.0;
        public double lastamount = 0.0;

        static double val = 1.1;
        private void ShowPumpProgress(PumpUpdateData data)
        {
            Thread.BeginCriticalRegion();
            try
            {
                HtmlElement a;
                if (conf.UseWGT && bWGTInError)
                    return;

                lastvol = data.vol;
                lastamount = data.money;

                a = MainPage.Document.GetElementById("vol");
                a.InnerHtml = data.vol.ToString("00000.00");

                a = MainPage.Document.GetElementById("sale");
                a.InnerHtml = data.money.ToString("00000.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Thread.EndCriticalRegion();
            }
        }

        private bool IsNewFuelling(string carddata)
        {
            return true;
        }

        DeviceData dd = null;

        String dt = DateTime.Now.ToString();


        private bool CheckAuth(string cardnumber, int ActiveNozz, ref double limit,ref string type, ref string plate, ref string reason)
        {
            reason = "";
            string prod = "";
            if (ActiveNozz == 1)
                prod = conf.Product1;
            else
                prod = conf.Product2;
            if (cardnumber == "")
            {
                reason = "Not found";
                return false;
            }

            //serach DB list
            dd = conf.DB.Find(x => x.cardnum == cardnumber);
            if (dd != null && dd.bEnabled)
            {
                fuellingcardnumber = cardnumber;
                limit = dd.limit;
                plate = dd.plate;
                type = dd.type;
                if (dd.prod != prod)
                {
                    reason = "Wrong fuel type";
                    return false;
                }
                //check for CPass

                if (dd.bCPass )// && false)
                {
                    if (MobileRes == "None")
                    {
                        dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        SendMobileRequest(dd.cardnum,MsgID);
                        reason = "CPass required";
                        return false;
                    }
                    else if (MobileRes == "approve")
                    {
                        SendMobileConfirm(dd.cardnum, MsgID);
                        return true;
                    }
                    if (MobileRes == "waiting")  //no need for second message
                    {
                        SendMobilePollRequest(dd.cardnum,MsgID);
                        reason = "CPass wait";
                        return false;
                    }
                }
                return true;
            }
            if (dd != null && !dd.bEnabled)
            {
                reason = "Not allowed";
                return false;
            }
            reason = "Not found";
            return false;
        }

 

       
        public void DoSomething()
        {
            // Indicate success.
            MessageBox.Show("It worked!");
        }


        [ComVisible(true)]
        public class ScriptManager
        {
            // Variable to store the form of type Form1.
            private Main mForm;

            // Constructor.
            public ScriptManager(Main form)
            {
                // Save the form so it can be referenced later.
                mForm = form;
            }

            // This method can be called from JavaScript.
            public void MethodToCallFromScript()
            {
                // Call a method on the form.
                mForm.DoSomething();
            }

            public void Nozzle1Clicked()
            {
                mForm.Nozz1_Click();
            }

            public void Nozzle2Clicked()
            {
                mForm.Nozz2_Click();
            }

            //stations selection
            public void Station1Clicked()
            {
                mForm.Station1_Click();
            }

            public void Station2Clicked()
            {
                mForm.Station2_Click();
            }

            public void Station3Clicked()
            {
                mForm.Station3_Click();
            }

            public void Station4Clicked()
            {
                mForm.Station4_Click();
            }

            public void Wash1Clicked()
            {
                mForm.Wash1Clicked();
            }

            public void Wash2Clicked()
            {
                mForm.Wash2Clicked();
            }



            // This method can also be called from JavaScript.
            public void AnotherMethod(string message)
            {
                MessageBox.Show(message);
            }
        }


        private void UpdatePPVPrecent(double Discount)
        {
            HtmlElement a;

            a = MainPage.Document.GetElementById("FuelPrice1");
            a.InnerHtml = (conf.PPV1*(1-Discount/100)).ToString("0.00");

            a = MainPage.Document.GetElementById("FuelPrice2");
            a.InnerHtml = (conf.PPV2*(1-Discount/100)).ToString("0.00");

            pup.SetPPV(conf.PPV1 * (1 - Discount / 100), conf.PPV2* (1-Discount/100));
        }

        private void UpdatePPVAbs(double Discount)
        {
            HtmlElement a;

            a = MainPage.Document.GetElementById("FuelPrice1");
            a.InnerHtml = (conf.PPV1 - Discount).ToString("0.00");

            a = MainPage.Document.GetElementById("FuelPrice2");
            a.InnerHtml = (conf.PPV2 - Discount).ToString("0.00");

            pup.SetPPV(conf.PPV1 - Discount, conf.PPV2 - Discount);
        }

        private void MainPage_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //initalized all static var
            HtmlElement a;
            

            try
            {
                a = MainPage.Document.GetElementById("FuelName1");                
                a.InnerHtml = conf.Product1;

                a = MainPage.Document.GetElementById("FuelName2");
                a.InnerHtml = conf.Product2;

                a = MainPage.Document.GetElementById("FuelPrice1");
                a.InnerHtml = conf.PPV1.ToString("0.00");

                a = MainPage.Document.GetElementById("FuelPrice2");
                a.InnerHtml = conf.PPV2.ToString("0.00");


                a = MainPage.Document.GetElementById("VolText");
                a.InnerHtml = conf.Units;

                //the below is now hard coded in image
                //a = MainPage.Document.GetElementById("PlateText");
                //a.InnerHtml = conf.PlateText;

                //the below text is now hard coded in image
                //a = MainPage.Document.GetElementById("LimitText");
                //a.InnerHtml = conf.LimitText;



                //start comm
                StartComm();

            }
            catch
            {                                        
            }


        }

        static bool bMsg1Was = false;
        static bool bMsg2Was = false;

        static bool bWGTInError = true;


        static double limit = 0.0;
        static string plate = "";
        static string reason = "";
        static string type = "";

        static string cardtoauth = "";
        static int statustouse = 0x30;

        static StatusData gotmsg = null;

        static int AnimationPhase = -1;
        
        private void AnimationTimerPorcessor(Object myObject, EventArgs myEventArgs)
        {
            HtmlElement a =  MainPage.Document.GetElementById("WashAnimation");
            HtmlElement b =  MainPage.Document.GetElementById("WashTime");

            b.InnerHtml = (AnimationPhase).ToString("00") + "'";

            int p = AnimationPhase / 2;
            switch (p)
            {
                case 1:
                    a.SetAttribute("src", "Washing1.png"); 
                    break;
                case 2:
                    a.SetAttribute("src", "Washing2.png");
                    
                    break;
                case 3:
                    a.SetAttribute("src", "Washing3.png");
                   

                    break;
                case 4:
                    a.SetAttribute("src", "Washing4.png");
                    
                    break;
                case 5:
                    a.SetAttribute("src", "Washing5.png");
                    
                    break;
                case 6:
                    a.SetAttribute("src", "Washing6.png");
                   
                    break;
                case 7:
                    
                    AnimationPhase = -1;
                    AnimationTimer.Stop();
                    a.SetAttribute("src", "Washing1.png");
                    if (WashSelected == 1)
                    {
                        a = MainPage.Document.GetElementById("Wash1");
                        a.SetAttribute("src", "carwashbuttonOff.png");
                    }
                    else
                    {
                        a = MainPage.Document.GetElementById("Wash2");
                        a.SetAttribute("src", "carwashbuttonpremiumOff.png");
                    }
                    WashSelected = -1;
                    b.InnerHtml = "00'";
                    WashPlayer.Stop();
                    WashInProgress = false;
                    SendTransactionComplete();
                    AddToLogList(MsgLogType.TransEnded, "", 1, "", 0, "");  
                    break;

            }
            AnimationPhase++;
            
        }
       

        private  void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            myTimer.Stop();            
            //MessageBox.Show("Timer done");

            if (CheckAuth(cardtoauth, ActiveNozz, ref limit, ref type, ref plate, ref reason))
            {
                ps = PumpStatus.InUse;
                pup.ResetVol(conf.PumpAuthDelay);
                pup.SetStatus(ps, limit, dd.type, dd.plate, conf.flowrate, -1);
                AddToLogList(MsgLogType.Authorized, reason, 0, plate, limit, gotmsg.carddata);
                (MainPage.Document.GetElementById("Plate")).InnerHtml = "Welcome "+plate;
                if (type == "Money")
                    (MainPage.Document.GetElementById("Limit")).InnerHtml = limit.ToString("0") + " " + conf.LimitPerMoneyText;
                else
                    (MainPage.Document.GetElementById("Limit")).InnerHtml = limit.ToString("0") + " " + conf.LimitPerVolText;
            }
            else if (reason == "CPass required")
            {
                AddToLogList(MsgLogType.CheckCPass, reason, 0, plate, limit, cardtoauth);
                myTimer.Start();  //recheck in as few seconds
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
                myTimer.Start();  //recheck in as few seconds
            }
            else
            {
                if (statustouse != 0x30)
                    AddToLogList(MsgLogType.CannotAuth, reason, 0, plate, limit, gotmsg.carddata);
            }
            
        }

       


        private void ShowProgress(StatusData msg)
        {
            Thread.BeginCriticalRegion();
            gotmsg = msg;
            try
            {
                if (msg.status == -1)
                {
                    AddToLogList(MsgLogType.CommErr,"", 0, "", 0, msg.carddata);
                    bWGTInError = true;
                    pup.SetStatus(PumpStatus.Stopped);
                    return;
                }

                ActiveNozz = pup.GetNozz();

               
                //check that we were in error and update log
                if (bWGTInError)
                {
                    bWGTInError = false;
                    AddToLogList(MsgLogType.WGTConnected, "", 0, "", 0, "");
                }
                

               
                limit = 0.0;
                plate = "";
                reason = "";
                type = "";

                cardtoauth = "";
                statustouse = 0x30;

                if (ActiveNozz == 1)
                {
                    cardtoauth = msg.carddata;
                    statustouse = msg.status;
                }
                else if (ActiveNozz == 2)
                {
                    cardtoauth = msg.carddata2;
                    statustouse = msg.status2;
                }
                else  //no nozzle is active 
                {
                    return;
                }

                

                //check if anything changed
                if (cardtoauth == laststatus.carddata && statustouse == laststatus.status)
                    return; //no change 

                laststatus.carddata = cardtoauth;
                laststatus.status = statustouse;

                switch (ps)
                {
                    case PumpStatus.Idle:
                        //try authorize card
                        if (CheckAuth(cardtoauth,ActiveNozz, ref limit, ref type, ref plate,ref reason))
                        {
                            ps = PumpStatus.Ready;
                            pup.SetStatus(ps, limit, dd.type, dd.plate, conf.flowrate, -1);
                            AddToLogList(MsgLogType.Authorized, reason, 0, plate, limit, msg.carddata);
                            (MainPage.Document.GetElementById("Plate")).InnerHtml = "Welcome "+plate;
                            if (type == "Money")
                                (MainPage.Document.GetElementById("Limit")).InnerHtml = limit.ToString("0.00")+" "+conf.LimitPerMoneyText;
                            else
                                (MainPage.Document.GetElementById("Limit")).InnerHtml = limit.ToString("0.00") + " " + conf.LimitPerVolText;

                        }
                        else
                        {
                            if (statustouse == 0x30)
                            {
                                AddToLogList(MsgLogType.Disconnected, "", 0, "", 0, "");
                                return;
                            }
                            if (msg.status == -1)
                            {
                                AddToLogList(MsgLogType.CommErr,"", 0, plate, limit, msg.carddata);
                                return;
                            }
                            AddToLogList(MsgLogType.CannotAuth, reason, 0, plate, 0, msg.carddata);
                        }
                        break;
                    case PumpStatus.Call:
                        if (cardtoauth != "")
                        {
                            MobileRes = "None";
                            CPassTry = 0;
                            MsgID++;
                            AddToLogList(MsgLogType.CheckAuth, "", 0, "", 0, cardtoauth);
                            myTimer.Start();
                        }
                        /*
                        if (CheckAuth(cardtoauth,ActiveNozz, ref limit, ref type, ref plate,ref reason))
                        {
                            ps = PumpStatus.InUse;                            
                            pup.ResetVol();
                            pup.SetStatus(ps, limit, dd.type, dd.plate, conf.flowrate, -1);
                            AddToLogList(MsgLogType.Authorized, reason, 0, plate, limit, msg.carddata);
                            (MainPage.Document.GetElementById("Plate")).InnerHtml = plate;
                            if (type == "Money")
                                (MainPage.Document.GetElementById("Limit")).InnerHtml = limit.ToString("0.00") + " " + conf.LimitPerMoneyText;
                            else
                                (MainPage.Document.GetElementById("Limit")).InnerHtml = limit.ToString("0.00") + " " + conf.LimitPerVolText;
                        }
                        else
                        {
                            if (statustouse != 0x30)
                                AddToLogList(MsgLogType.CannotAuth, reason, 0, plate, limit, msg.carddata);
                        }
                         */
                        break;
                    case PumpStatus.InUse:
                        //check if we are 'out'
                        if (statustouse == 0x30)
                        {
                            ps = PumpStatus.Stopped;
                            pup.SetStatus(ps);
                            //TODO ActivityList.Items.Insert(0, "Nozzle is out - suspending fuelling " + msg.carddata);                            
                            AddToLogList(MsgLogType.Suspending, reason, 0, plate, limit, msg.carddata);
                            break;
                        }
                        //check that we did not got other card
                        if (cardtoauth != fuellingcardnumber)
                        {
                            //stop fuelling
                            ps = PumpStatus.Stopped;
                            pup.SetStatus(ps);
                            //TODO ActivityList.Items.Insert(0, "Got another card - suspending fuelling " + msg.carddata);                            
                            AddToLogList(MsgLogType.OtherCard, "", 0, plate, limit, msg.carddata);
                        }
                        break;
                    case PumpStatus.Stopped:
                        //check if we can resume
                        if (cardtoauth == fuellingcardnumber)
                        {
                            ps = PumpStatus.InUse;
                            pup.SetStatus(ps);                                                     
                            AddToLogList(MsgLogType.Resuming,  reason, 0, plate, limit, cardtoauth);
                        }
                        else if (cardtoauth != "")
                        {
                           // AddToLogList(MsgLogType.OtherCard, reason, 0, plate, limit, cardtoauth);
                        }
                        break;
                    case PumpStatus.Ready:
                        //if we lost the connection cancle the auth
                        if (statustouse == 0x30)
                        {
                            ps = PumpStatus.Idle;
                            pup.SetStatus(ps, 0);
                            AddToLogList(MsgLogType.CancelAuth, reason, 0, plate, limit, cardtoauth);
                        }
                        //if we got a different card - need to recheck
                        if (statustouse == 0x34 && fuellingcardnumber != cardtoauth)
                        {
                            if (CheckAuth(cardtoauth,ActiveNozz, ref limit, ref type,ref plate, ref reason))
                            {
                                ps = PumpStatus.Ready;
                                pup.SetStatus(ps, limit, dd.type, dd.plate, conf.flowrate, -1);
                                AddToLogList(MsgLogType.NewCard, reason, 0, plate, limit, cardtoauth);
                                (MainPage.Document.GetElementById("Plate")).InnerHtml = "Welcome "+plate;
                                if (type == "Money")
                                    (MainPage.Document.GetElementById("Limit")).InnerHtml = limit.ToString("0.00") + " " + conf.LimitPerMoneyText;
                                else
                                    (MainPage.Document.GetElementById("Limit")).InnerHtml = limit.ToString("0.00") + " " + conf.LimitPerVolText;
                            }
                            else
                            {
                                ps = PumpStatus.Idle;
                                pup.SetStatus(ps, 0);
                                AddToLogList(MsgLogType.CannotAuth, reason, 0, plate, limit, msg.carddata);
                            }
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

        public int ProductCode = 0;

        public void Nozz1_Click()
        {            
            if (pup.GetNozz() == 2)  //no nozz or this nozz
                return;  //cannot use two nozzles at same time
            ProductCode = conf.Prod1Code;
            msr.SetInfo(SelectedStation, ProductCode);
            HtmlElement a = MainPage.Document.GetElementById("Nozz1");
            switch (ps)
            {
                case PumpStatus.Idle:
                    //just lift nozzle
                    a.SetAttribute("src", "NozzUp.png");                    
                    AddToLogList(MsgLogType.NozzleUP, "", 1, "", 0, "");
                    if (MSRSuspendedAuth())
                    {
                        msgSaved = null; //clear for next
                    }
                    else
                    {
                        ps = PumpStatus.Call;
                        pup.SetStatus(ps, 1);
                    }
                    break;
                case PumpStatus.Call:
                    //put nozzle back
                    a.SetAttribute("src", "NozzDn.png");                    
                    AddToLogList(MsgLogType.NozzelDown,"", 1, "",0,""); 
                    ProductCode = 0;
                    msr.SetInfo(SelectedStation, ProductCode);       
                    ps = PumpStatus.Idle;
                    pup.SetStatus(ps, 0);                   
                    break;
                case PumpStatus.Ready:
                    //we can move to inuse
                    a.SetAttribute("src", "NozzUp.png");                    
                    AddToLogList(MsgLogType.StartRefuel,"", 1, "", 0, "");                            
                    ps = PumpStatus.InUse;
                    pup.ResetVol(conf.PumpAuthDelay);
                    pup.SetStatus(ps, 1);
                    break;
                case PumpStatus.InUse:
                    //stop fuelling;
                    a.SetAttribute("src", "NozzDn.png");                    
                    AddToLogList(MsgLogType.TransEnded, "", 1, "", 0, "");                            
                    ps = PumpStatus.Idle;
                    ProductCode = 0;
                    msr.SetInfo(SelectedStation, ProductCode);
                    MobileRes = "None";
                    CPassTry = 0;
                    
                    pup.SetStatus(ps, 0);
                    if (dd.bCPass)
                        SendMobileSummary(dd.cardnum, MsgID);
                    if (conf.UseMSR)
                    {
                        SendTransactionComplete();
                        
                    }
                    break;
                case PumpStatus.Stopped:
                    a.SetAttribute("src", "NozzDn.png");                    
                    AddToLogList(MsgLogType.TransEnded,"", 1, "", 0, "");                            
                    ps = PumpStatus.Idle;
                    ProductCode = 0;
                    msr.SetInfo(SelectedStation, ProductCode);
                    MobileRes = "None";
                    CPassTry = 0;
                    
                    pup.SetStatus(ps, 0);
                    if (dd.bCPass)
                        SendMobileSummary(dd.cardnum, MsgID);
                    break;
            }

        }

        public void Nozz2_Click()
        {
            if (pup.GetNozz() == 1)  //no nozz or this nozz
                return;  //cannot use two nozzles at same time
            ProductCode = conf.Prod2Code;
            msr.SetInfo(SelectedStation, ProductCode);
            HtmlElement a = MainPage.Document.GetElementById("Nozz2");
            switch (ps)
            {
                case PumpStatus.Idle:
                    //just lift nozzle
                    a.SetAttribute("src", "NozzUp.png");
                    AddToLogList(MsgLogType.NozzleUP, "", 2, "", 0, "");
                    if (MSRSuspendedAuth())
                    {
                        msgSaved = null; //clear for next
                    }
                    else
                    {
                        ps = PumpStatus.Call;
                        pup.SetStatus(ps, 2);
                    }
                    break;
                case PumpStatus.Call:
                    //put nozzle back
                    a.SetAttribute("src", "NozzDn.png");  
                    AddToLogList(MsgLogType.NozzelDown, "", 2, "", 0, "");
                    ProductCode = 0;
                    msr.SetInfo(SelectedStation, ProductCode);
                    ps = PumpStatus.Idle;
                    pup.SetStatus(ps, 0);
                    break;
                case PumpStatus.Ready:
                    //we can move to inuse
                    a.SetAttribute("src", "NozzUp.png");
                    AddToLogList(MsgLogType.StartRefuel,"", 2, "", 0, "");
                    ps = PumpStatus.InUse;
                    pup.ResetVol(conf.PumpAuthDelay);
                    pup.SetStatus(ps, 2);
                    break;
                case PumpStatus.InUse:
                    //stop fuelling;
                    a.SetAttribute("src", "NozzDn.png");  
                    AddToLogList(MsgLogType.TransEnded, "", 2, "", 0, "");
                    ps = PumpStatus.Idle;
                    ProductCode = 0;
                    msr.SetInfo(SelectedStation, ProductCode);
                    MobileRes = "None";
                    CPassTry = 0;                    
                    pup.SetStatus(ps, 0);
                    if (dd.bCPass)
                        SendMobileSummary(dd.cardnum, MsgID);
                    if (conf.UseMSR)
                    {
                        SendTransactionComplete();
            
                    }
                    break;
                case PumpStatus.Stopped:
                    a.SetAttribute("src", "NozzDn.png");  
                    AddToLogList(MsgLogType.TransEnded,"", 2, "", 0, "");
                    ps = PumpStatus.Idle;
                    ProductCode = 0;
                    msr.SetInfo(SelectedStation, ProductCode);
                    MobileRes = "None";
                    CPassTry = 0;                    
                    pup.SetStatus(ps, 0);
                    if (dd.bCPass)
                        SendMobileSummary(dd.cardnum, MsgID);
                    break;
            }
        }

        int WashSelected = -1;
        bool WashInProgress = false;

        public void Wash1Clicked()
        {
            HtmlElement a = null;
            if (pup.GetNozz() == 1 || pup.GetNozz() == 2)
                return;
            if (WashInProgress)
                return; 
            if (WashSelected == 1)
            {
                WashSelected = -1;
                a = MainPage.Document.GetElementById("Wash1");
                a.SetAttribute("src", "carwashbuttonOff.png");
                msr.SetInfo(SelectedStation, ProductCode);
                return;
            }
            WashSelected = 1;
            a = MainPage.Document.GetElementById("Wash1");
            a.SetAttribute("src", "carwashbuttonOn.png");
            a = MainPage.Document.GetElementById("Wash2");
            a.SetAttribute("src", "carwashbuttonpremiumOff.png");
            ProductCode = conf.Wash1Code;
            msr.SetInfo(SelectedStation, ProductCode);
            AddToLogList(MsgLogType.MSRWash1Selected, "", 0, "", 0, "");
 
        }

        public void Wash2Clicked()
        {
            HtmlElement a = null;
            if (pup.GetNozz() == 1 || pup.GetNozz() == 2)
                return;
            if (WashInProgress)
                return;
            if (WashSelected == 2)
            {
                WashSelected = -1;
                a = MainPage.Document.GetElementById("Wash2");
                a.SetAttribute("src", "carwashbuttonpremiumOff.png");
                msr.SetInfo(SelectedStation, ProductCode);
                return;
            }
            WashSelected = 2;
            a = MainPage.Document.GetElementById("Wash2");
            a.SetAttribute("src", "carwashbuttonpremiumOn.png");
            a = MainPage.Document.GetElementById("Wash1");
            a.SetAttribute("src", "carwashbuttonOff.png");
            ProductCode = conf.Wash2Code;
            msr.SetInfo(SelectedStation, ProductCode);
            AddToLogList(MsgLogType.MSRWash2Selected, "", 0, "", 0, "");
            
        }


        public int SelectedStation = 1;

        //stations
        public void Station1_Click()
        {
            if (SelectedStation == 1)
                return; //no reselect
            //unslected others
            HtmlElement a = MainPage.Document.GetElementById("Station2");
            a.SetAttribute("src", "Sitebutton2Off.png");
            a = MainPage.Document.GetElementById("Station3");
            a.SetAttribute("src", "Sitebutton3Off.png");
            a = MainPage.Document.GetElementById("Station4");
            a.SetAttribute("src", "Sitebutton4Off.png");

            //select this
            a = MainPage.Document.GetElementById("Station1");
            a.SetAttribute("src", "Sitebutton1On.png");
            SelectedStation = 1;
            msr.SetInfo(SelectedStation,ProductCode);
        }

        public void Station2_Click()
        {
            if (SelectedStation == 2)
                return; //no reselect
            //unslected others
            HtmlElement a = MainPage.Document.GetElementById("Station1");
            a.SetAttribute("src", "sitebutton1Off.png");
            a = MainPage.Document.GetElementById("Station3");
            a.SetAttribute("src", "Sitebutton3Off.png");
            a = MainPage.Document.GetElementById("Station4");
            a.SetAttribute("src", "Sitebutton4Off.png");

            //select this
            a = MainPage.Document.GetElementById("Station2");
            a.SetAttribute("src", "Sitebutton2On.png");
            SelectedStation = 2;
            msr.SetInfo(SelectedStation,ProductCode);
        }

        public void Station3_Click()
        {
            if (SelectedStation == 3)
                return; //no reselect
            //unslected others
            HtmlElement a = MainPage.Document.GetElementById("Station1");
            a.SetAttribute("src", "Sitebutton1Off.png");
            a = MainPage.Document.GetElementById("Station2");
            a.SetAttribute("src", "Sitebutton2Off.png");
            a = MainPage.Document.GetElementById("Station4");
            a.SetAttribute("src", "Sitebutton4Off.png");

            //select this
            a = MainPage.Document.GetElementById("Station3");
            a.SetAttribute("src", "Sitebutton3On.png");
            SelectedStation = 3;
            msr.SetInfo(SelectedStation,ProductCode);
        }

        public void Station4_Click()
        {
            if (SelectedStation == 4)
                return; //no reselect
            //unslected others
            HtmlElement a = MainPage.Document.GetElementById("Station1");
            a.SetAttribute("src", "Sitebutton1Off.png");
            a = MainPage.Document.GetElementById("Station2");
            a.SetAttribute("src", "Sitebutton2Off.png");
            a = MainPage.Document.GetElementById("Station3");
            a.SetAttribute("src", "Sitebutton3Off.png");

            //select this
            a = MainPage.Document.GetElementById("Station4");
            a.SetAttribute("src", "Sitebutton4On.png");
            SelectedStation = 3;
            msr.SetInfo(SelectedStation, ProductCode);
        }
        

        static bool bInError = false;

        MsgLogType lasttype = MsgLogType.CommErr;

        public void AddToLogList(MsgLogType type,string reason,int nozz,string plate, double limit, string cardnum)
        {
            //move all strings one line down
            HtmlElement l1,l2,l3,l4,l5;
            string line = "";
            string pic = "Empty.png";

            if (bInError && type == MsgLogType.CommErr)
                return;

            if (lasttype == type)
                return;
            lasttype = type;
            if (type != MsgLogType.CommErr)
                bInError = false;
            switch (type)
            {
                case MsgLogType.NozzleUP:
                    line = "Nozzle "+nozz.ToString()+ " is up.";
                    pic = "LogNozzUp.png";
                break;
                case MsgLogType.NozzelDown:
                    line = "Nozzle " + nozz.ToString() + " is down.";
                    pic = "PicNozzDown.png";
                break;
                case MsgLogType.CheckAuth:
                    line = "Authorizing on Head Office";
                    pic = "Authorizing_HO.png";
                break;
                case MsgLogType.CheckCPass:
                    line = "Request mobile confirmation";
                    pic = "phoneAuth.png";
                break;
                case MsgLogType.Authorized:
                    line = "Authorized " + plate;
                    pic = "Authorized.png";
                break;
                case MsgLogType.CannotAuth:
                    line = "Cannot authorize "+plate + " " + reason;  
                    pic = "Not_Authorized.png";
                break;
                case MsgLogType.CancelAuth:
                    line = "Disconnecet cancel authorization";
                break;
                case MsgLogType.Disconnected:
                    line = "Disconnected";
                break;
                case MsgLogType.NewCard:
                    line = "Authorized new card "+plate;
                break;
                case MsgLogType.OtherCard:
                    line = "Other card detected - suspending.";
                    pic = "Suspend.png";
                break;
                case MsgLogType.Resuming:
                    line = "Resume fuelling";
                    pic = "Resume.png";
                break;
                case MsgLogType.StartRefuel:
                    line = "Handle up - start fuelling.";
                break;
                case MsgLogType.Suspending:
                    line = "Disconnected suspending fuelling.";
                    pic = "Suspend.png";
                break;
                case MsgLogType.TransEnded:
                    line = "Transaction ended.";
                    pic = "PicNozzDown.png";
                    //clear plate & limits
                    MainPage.Document.GetElementById("Limit").InnerHtml = "";
                    MainPage.Document.GetElementById("Plate").InnerHtml = "";
                    MainPage.Document.GetElementById("Discount").InnerHtml = "";
                break;
                case MsgLogType.CommErr:                
                    line = "Communication error with WGT.";
                    pic = "Disconnected_WGT.png";
                    bInError = true;
                break;
                case MsgLogType.WGTConnected:
                    line = "Connected to WGT.";
                    pic = "Connected_WGT.png";
                break;

                //MSR part

                case MsgLogType.MSRCommError:
                    line = "Communication error with Host.";
                    
                bInError = true;
                break;
                case MsgLogType.MSRRead:
                    line = "Swiped card <" + cardnum + ">.";
                    //pic = "";
                break;
                case MsgLogType.MSRRequestAuth:
                    line = "Requesting authorization";
                    pic = "Authorizing_HO.png";
                break;
                case MsgLogType.MSRBadRead:
                    line = "Swiped card - error reading card.";
                    //pic = "";
                break;
                case MsgLogType.MSRAuth:
                    line = "Card was authorized.";
                    pic = "Authorized.png";
                break;
                case MsgLogType.MSRReject:
                    line = reason;
                    pic = "Not_Authorized.png";
                break;
                case MsgLogType.MSRWrongProduct:
                    line = "Product not allowed";
                    //pic = "";
                break;
                case MsgLogType.MSRSSelectProduct:
                    line = "Select Product.";
                break;
                case MsgLogType.MSRWash1Selected:
                    line = "Wash standard selected.";
                break;
                case MsgLogType.MSRWash2Selected:
                    line = "Wash premium selected.";
                break;

            }


            //push line
            l5 = MainPage.Document.GetElementById("LogText5");
            l4 = MainPage.Document.GetElementById("LogText4");
            l3 = MainPage.Document.GetElementById("LogText3");
            l2 = MainPage.Document.GetElementById("LogText2");
            l1 = MainPage.Document.GetElementById("LogText1");

            l5.InnerHtml = l4.InnerHtml;
            l4.InnerHtml = l3.InnerHtml;
            l3.InnerHtml = l2.InnerHtml;
            l2.InnerHtml = l1.InnerHtml;
            l1.InnerHtml = line;


            //push pic
            l5 = MainPage.Document.GetElementById("LogPic5");
            l4 = MainPage.Document.GetElementById("LogPic4");
            l3 = MainPage.Document.GetElementById("LogPic3");
            l2 = MainPage.Document.GetElementById("LogPic2");
            l1 = MainPage.Document.GetElementById("LogPic1");

            l5.SetAttribute("src", l4.GetAttribute("src"));
            l4.SetAttribute("src", l3.GetAttribute("src"));
            l3.SetAttribute("src", l2.GetAttribute("src"));
            l2.SetAttribute("src", l1.GetAttribute("src"));
            l1.SetAttribute("src", pic);


            myLog.Log(line + " ("+cardnum+")");

        }

        public char KeyCodeToKey(Keys KeyCode)
        {
            char key = ' ';
            if (KeyCode == Keys.D0)
                key = '0';
            if (KeyCode == Keys.D1)
                key = '1';
            if (KeyCode == Keys.D2)
                key = '2';
            if (KeyCode == Keys.D3)
                key = '3';
            if (KeyCode == Keys.D4)
                key = '4';
            if (KeyCode == Keys.D5)
                key = '5';
            if (KeyCode == Keys.D6)
                key = '6';
            if (KeyCode == Keys.D7)
                key = '7';
            if (KeyCode == Keys.D8)
                key = '8';
            if (KeyCode == Keys.D9)
                key = '9';
            if (KeyCode.ToString() == "Oemplus")
                key = '=';

            return key;
        }



        private bool _skipOnce = false;        

        private void MainPage_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (_skipOnce)
                {
                    _skipOnce = false;
                    return;
                }
                if (e.Control)
                    return;
                _skipOnce = true;
                char k = KeyCodeToKey(e.KeyCode);
                if (e.KeyCode == Keys.Return)
                {

                    msr.myLog.Log("Got Enter: card is: " + msr.card);
                    //MessageBox.Show(card);

                    int ep = msr.card.IndexOf('=');
                    if (ep != -1)
                    {
                        string c = msr.card.Substring(0, msr.card.IndexOf('='));
                        //notify GUI
                        msr.myLog.Log("Found card: " + c);
                        msr.Card = msr.card;
                        msr.FoundCard = true;
                        //Thread.Sleep(5000);                         
                    }
                    else
                    {
                        msr.myLog.Log("Found card: " + msr.card);
                        msr.Card = msr.card;
                        msr.FoundCard = true;
                    }
                    msr.card = "";
                }
                else
                    msr.card += k.ToString();
                //e.Handled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}

