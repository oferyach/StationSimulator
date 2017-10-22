using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace ForeFuelSimulator
{
    public class PumpUpdater
    {
        static ContainerControl m_sender = null;
        static Delegate m_senderDelegate = null;
        //double runvol = 0.0;
        //double runmoney = 0.0;
        double limit = 0.0;
        string type = "Volume";
        string plate = "";
        bool b888 = false;
        int count888 = 0;
        double dFlow = 60.00;
        double PPV1 = 1.0;
        double PPV2 = 1.0;
        static int nozz = 0;
        PumpStatus ps;

        public PumpUpdateData data = new PumpUpdateData();
        public PumpUpdater(ContainerControl sender, Delegate senderDelegate)
        {
            m_sender = sender;
            m_senderDelegate = senderDelegate;
            ps = PumpStatus.Idle;
        }


        public void SetPPV(double ppv1, double ppv2)
        {
            PPV1 = ppv1;
            PPV2 = ppv2;
        }

        public void SetStatus(PumpStatus ps1, double limit1, string type1, string plate1, double flow1, int nozz1)
        {
            ps = ps1;
            limit = limit1;
            plate = plate1;
            dFlow = flow1;
            type = type1;
            if (nozz1 != -1)
                nozz = nozz1;
        }

        public void SetUpdateData(bool init)
        {
            if (nozz == 1)
                data.PPV = PPV1;
            else
                data.PPV = PPV2;
            data.plate = plate;
            if (init)
            {
                data.vol = 88888.88;
                data.money = 88888.88;
                return;
            }
            //data.vol = runvol;
            if (nozz == 1)
                data.money = data.vol * PPV1;
            else
                data.money = data.vol * PPV2;
        }

        public void SetStatus(PumpStatus ps1, int nozz1)
        {
            ps = ps1;
            nozz = nozz1;
        }

        public void SetStatus(PumpStatus ps1)
        {
            ps = ps1;
            SetUpdateData(false);
        }
        bool bLimit = false;
        public void ResetVol(int pumpdelay)
        {
            //runvol = 0.0;            
            bLimit = false;
            b888 = true;
            count888 = pumpdelay / 100;  
        }

        public int GetNozz()
        {
            return nozz;
        }
        public void Worker()
        {
            bool bSoundOn = false;
            bool bLimitDone = false;


            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.SoundLocation = "fuel_pump_operating.wav";
            while (true)
            {
                if (b888 && count888 > 0)
                {
                    bLimitDone = false;
                    SetUpdateData(true);
                    m_sender.BeginInvoke(m_senderDelegate, data);
                    count888--;
                    if (count888 == 0)
                    {
                        b888 = false;
                        data.vol = 0.0;
                        data.money = 0.0;
                    }
                    Thread.Sleep(100);
                    continue;
                }
                if (ps == PumpStatus.InUse && !bLimit)
                {
                    data.vol += (dFlow / 600);
                    data.vol = Math.Round(data.vol, 2);
                    SetUpdateData(false);
                    double valuetocheck = 0.0;
                    if (type == "Money")
                        valuetocheck = data.money;
                    else
                        valuetocheck = data.vol;
                    if (valuetocheck > limit)
                    {
                        bLimit = true;
                        if (type == "Money")
                        {
                            data.money = limit;
                            //make sure no over flow - we calcualte display by vol
                            if (nozz == 1)
                                data.vol = data.money / PPV1;
                            else
                                data.vol = data.money / PPV2;
                        }
                        else
                            data.vol = limit;                        
                        if (bSoundOn)
                        {
                            player.Stop();
                            bSoundOn = false;
                        }
                    }

                    SetUpdateData(false);
                    
                    m_sender.BeginInvoke(m_senderDelegate, data);
                    if (!bSoundOn && !bLimit)
                    {
                        bSoundOn = true;
                        player.PlayLooping();
                        //player.Play();
                    }
                }
                else
                {
                    if (bSoundOn)
                    {
                        player.Stop();
                        bSoundOn = false;
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}
