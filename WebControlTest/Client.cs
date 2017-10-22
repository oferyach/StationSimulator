using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ForeFuelSimulator
{
    public class Client
    {
        private string _Host;
        private int _Port;
        private int _Addr;
        private bool bConnected = false;
        private TcpClient tcpClient;
        static ContainerControl m_sender = null;
        static Delegate m_senderDelegate = null;

        //private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public Logger myLog = new Logger("VITComm.log");

        private byte[] _Msg = new byte[255];

        int[] Nozzs = new int[2];

        public Client(string Host, int Port, int Addr, ContainerControl sender, Delegate senderDelegate)
        {
            m_sender = sender;
            m_senderDelegate = senderDelegate;
            _Host = Host;
            _Port = Port;
            _Addr = Addr;
        }



        private string GetString(byte[] input, int len)
        {
            string s = "";
            int i = 0;
            foreach (byte b in input)
            {
                s += b.ToString("X2") + " ";
                i++;
                if (i == len)
                    break;
            }
            return s;
        }

        public ushort so_CRC(byte[] buff, short len)
        {
            ulong crc = 0;
            ushort i, j;
            byte d, bit;
            for (i = 1; i < len; i++)
            {
                d = buff[i];
                for (j = 0; j < 8; j++)
                {
                    bit = (byte)((d & 0x80) >> 7);
                    d = (byte)(d << 1);
                    crc = (ulong)((crc << 1) + bit);
                    if ((crc & 0x10000) != 0)
                    {
                        crc &= 0xFFFF;
                        crc ^= 0x8811;
                    }
                }
            }
            return (ushort)crc;
        }

        private void MakePollMessage()
        {
            //FD 46 30 08 00 36 54 FE
            _Msg[0] = 0xFD;
            _Msg[1] = (byte)_Addr;
            _Msg[2] = 0x30;
            _Msg[3] = 0x01;
            _Msg[4] = 0x02;
            _Msg[5] = 0x30;
            _Msg[6] = 0x30;

            ushort crc = so_CRC(_Msg, 7);
            _Msg[7] = (byte)(crc & 0xFF);
            _Msg[8] = (byte)((crc & 0xFF00) >> 8);
            _Msg[9] = 0xFE;
        }

        private void MakeGetDataMessage(byte nozz)
        {
            //FD 32 30 22 02 30 31 BD 24 FE
            _Msg[0] = 0xFD;
            _Msg[1] = (byte)_Addr;
            _Msg[2] = 0x30;
            _Msg[3] = 0x22;
            _Msg[4] = 0x02;
            _Msg[5] = 0x30;
            _Msg[6] = (byte)(0x30+nozz);
            ushort crc = so_CRC(_Msg, 7);
            _Msg[7] = (byte)(crc & 0xFF);
            _Msg[8] = (byte)((crc & 0xFF00) >> 8);
            _Msg[9] = 0xFE;
        }

        AutoResetEvent connectDone = new AutoResetEvent(false);

        public void ConnectCallback1(IAsyncResult ar)
        {
            try
            {
                connectDone.Set();
                TcpClient s = (TcpClient)(ar.AsyncState);
                s.EndConnect(ar);
            }
            catch (Exception ex)
            {
                myLog.Log(ex.Message);
            }

        }

        private int Recv(TcpClient tcpClient, byte[] indata)
        {
            Stream stream = tcpClient.GetStream();
            stream.ReadTimeout = 1000;
            int r = 0;
            int count = 0;
            bool bGotStrat = false;
            bool bGotEnd = false;
            try
            {
                while (!bGotStrat)
                {
                    r = stream.Read(indata, 0, 1);
                    if (indata[0] == 0xFD)
                    {
                        bGotStrat = true;
                        count++;
                    }
                }
                while (!bGotEnd)
                {
                    r = stream.Read(indata, count, 1);
                    if (indata[count] == 0xFE)
                    {
                        bGotEnd = true;
                    }
                    count++;
                }

                return count;
            }
            catch (IOException iox)
            {
                return -1;
            }

        }


        public void Worker()
        {
            StatusData stat = new StatusData();            
            stat.status = -1; //error
            stat.carddata = "";
            int laststatus1 = 0x30; //no data
            stat.status2 = -1; //error
            stat.carddata2 = "";
            int laststatus2 = 0x30; //no data
            int count = 0;
            while (true)
            {
                if (tcpClient == null || !tcpClient.Connected)
                {
                    try
                    {
                        tcpClient = new TcpClient();                       
                        //tcpClient.BeginConnect(_Host, _Port, new AsyncCallback(ConnectCallback1), tcpClient);
                        tcpClient.Connect(_Host, _Port);
                        //if (connectDone.WaitOne())
                        {
                            /*
                            myLog.Log("network connection failed!");
                            bConnected = false;
                            stat.carddata = "";
                            stat.status = -1;
                            laststatus1 = -1; //no data
                            stat.carddata2 = "";
                            stat.status2 = -1;
                            laststatus2 = -1; //no data
                            m_sender.BeginInvoke(m_senderDelegate, stat);
                            continue;
                             */

                            bConnected = true;
                            stat.status = 0x30;
                            stat.status2 = 0x30;
                            m_sender.BeginInvoke(m_senderDelegate, stat);
                        }

                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine(e.Message);
                        tcpClient.Close();
                        tcpClient = null;
                        bConnected = false;
                        stat.status = -1;
                        laststatus1 = -1; //no data
                        stat.carddata = "";
                        stat.status2 = -1;
                        laststatus2 = -1; //no data
                        stat.carddata2 = "";
                        m_sender.BeginInvoke(m_senderDelegate, stat);
                        Thread.Sleep(5000);
                        continue;
                    }

                    catch (Exception exp)
                    {
                        myLog.Log(exp.Message);
                        tcpClient.Close();

                        tcpClient = null;
                        bConnected = false;
                        laststatus1 = -1; //no data
                        stat.status = -1;
                        stat.carddata = "";
                        laststatus2 = -1; //no data
                        stat.status2 = -1;
                        stat.carddata2 = "";
                        m_sender.BeginInvoke(m_senderDelegate, stat);
                        Thread.Sleep(5000);
                        continue;
                    }
                } //!bConnected
                try
                {
                    Stream stream = tcpClient.GetStream();
                    stream.ReadTimeout = 5000;
                    MakePollMessage();

                    myLog.Log("SEND: " + GetString(_Msg, 10));
                    stream.Write(_Msg, 0, 10);

                    if (((count = Recv(tcpClient, _Msg)) == -1) || _Msg[3]!=0x01)
                    {
                        if (count == -1)
                            myLog.Log("RECV: Error in read: ");
                        else
                        {
                            laststatus1 = laststatus2 = -1; // 0x00; //reset
                            myLog.Log("RECV: Error in read: " + GetString(_Msg, count));
                        }
                        continue;
                    }

                    myLog.Log("RECV: " + GetString(_Msg, count));


                    int Chann2Stat = _Msg[8];

                    //check status of the 1st channel - we check on same data - will get on second poll
                    if (((_Msg[7] == 0x35 || _Msg[7] == 0x34) && (laststatus1 == 0x30 || laststatus1 == -1))
                        || (_Msg[7] == 0x34 && laststatus1 == 0x35) || (_Msg[7]==0x35 && laststatus1 ==0x34)) //only if we did not have any thing before
                    {
                        laststatus1 = _Msg[7]; //save it
                        MakeGetDataMessage(1);
                        myLog.Log("SEND: " + GetString(_Msg, 10));
                        stream.Write(_Msg, 0, 10);

                        if (((count = Recv(tcpClient, _Msg)) == -1) || _Msg[3]!=0x22)
                        {
                            tcpClient.Close();
                            if (count == -1)
                                myLog.Log("Error in read");
                            else
                                myLog.Log("RECV: Error in read: "+ GetString(_Msg,count));
                            continue;
                        }
                        myLog.Log("RECV: " + GetString(_Msg, count));

                        int i1 = System.Text.Encoding.Default.GetString(_Msg).IndexOf("?;");
                        int i2 = System.Text.Encoding.Default.GetString(_Msg).IndexOf("=");
                        if (i2 <= i1)
                        {
                            if (i2 == 0xc) //other type?
                            {
                                stat.carddata =
                                System.Text.Encoding.Default.GetString(_Msg).Substring(i2 + 1, 16);
                            }
                            else
                            {
                                myLog.Log("Bad card format = did not find track2 card number.");
                                stat.carddata = "";    //no data
                            }
                        }
                        else 
                        {
                            stat.carddata =
                            System.Text.Encoding.Default.GetString(_Msg).Substring(i1 + 2, i2 - i1 - 2);
                        }


                        stat.status = laststatus1;

                    }
                    else
                    {
                       // myLog.Log("What did we got");
                    }

                    if ((laststatus1 == 0x35 || laststatus1 == 0x34) && _Msg[7] == 0x30) //no data after we had some
                    {
                        laststatus1 = 0x30;
                        stat.status = laststatus1;
                        stat.carddata = "";
                        stat.nozz = 1;
                    }

                    //second chan
                    //check status of the 1st channel - we check on same data - will get on second poll
                    if (((Chann2Stat == 0x35 || Chann2Stat == 0x34) && (laststatus2 == 0x30 || laststatus2 == -1))
                        || (Chann2Stat == 0x34 && laststatus2 == 0x35)) //only if we did not have any thing before
                    {
                        laststatus2 = Chann2Stat; //save it
                        MakeGetDataMessage(2);
                        myLog.Log("SEND: " + GetString(_Msg, 10));
                        stream.Write(_Msg, 0, 10);

                        if (((count = Recv(tcpClient, _Msg)) == -1) || _Msg[3]!=0x22)
                        {
                            if (count == -1)
                                myLog.Log("Error in read");
                            else
                                myLog.Log("RECV: Error in read: "+ GetString(_Msg,count));
                            continue;
                        }
                        myLog.Log("RECV: " + GetString(_Msg, count));

                        int i1 = System.Text.Encoding.Default.GetString(_Msg).IndexOf("?;");
                        int i2 = System.Text.Encoding.Default.GetString(_Msg).IndexOf("=");
                        if (i2 <= i1)
                        {
                            myLog.Log("Bad card format = did not find track2 card number.");
                            stat.carddata2 = "";    //no data
                        }
                        else
                        {
                            stat.carddata2 =
                            System.Text.Encoding.Default.GetString(_Msg).Substring(i1 + 2, i2 - i1 - 2);
                        }


                        stat.status2 = laststatus2;

                    }

                    if ((laststatus2 == 0x35 || laststatus2 == 0x34) && Chann2Stat == 0x30) //no data after we had some
                    {
                        laststatus2 = 0x30;
                        stat.status2 = laststatus2;
                        stat.carddata2 = "";
                        stat.nozz2 = 2;
                    }
                    //second chan


                    m_sender.BeginInvoke(m_senderDelegate, stat);

                    Thread.Sleep(200);
                }
                catch (Exception exp)
                {
                    myLog.Log(exp.Message);
                    tcpClient.Close();
                    tcpClient = null;
                    bConnected = false;
                    laststatus1 = -1; //no data
                    stat.status = -1;
                    stat.carddata = "";
                    laststatus2 = -1; //no data
                    stat.status2 = -1;
                    stat.carddata2 = "";
                    m_sender.BeginInvoke(m_senderDelegate, stat);
                    Thread.Sleep(5000);
                    continue;
                }
            }
        }
    }
}
