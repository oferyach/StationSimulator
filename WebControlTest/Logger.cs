using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace ForeFuelSimulator
{
    public class Logger
    {
        //static StreamWriter w = new StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\log.txt", true); //File.AppendText("log.txt");
        StreamWriter w = null;
        public static string datePatt = @"yyyy-MM-dd HH:mm:ss";
        public static string islog;
        static readonly Object locker = new Object();
        public Logger(string sName)
        {
            w = new StreamWriter(sName, true);
            try
            {
                XmlTextReader reader = new XmlTextReader(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\settings.xml");
                reader.Read();
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(reader);
                XmlNodeList name = xDoc.GetElementsByTagName("log");
                islog = name[0].InnerXml;
            }
            catch (System.Exception e)
            {
                islog = "yes";
            }
            Log("Log started.");
            /*
            try
            {
              if (!(File.Exists("log.txt")))
                // Create a file to write to.
                w = File.CreateText("log.txt");
              else
                w = File.AppendText("log.txt");
            }
            catch (System.Exception e)
            {
              islog = "no";
            }
            */

        }
        public void Log(String logMessage, String Dir, int ID)
        {
            try
            {
                if (islog != "yes")
                    return;
                lock (locker)
                {
                    w.WriteLine("{0} ({1}) {2}: {3}", DateTime.Now.ToString(datePatt), ID, Dir, logMessage);
                    w.Flush();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        public void Log(String logMessage)
        {
            try
            {
                lock (locker)
                {
                    this.Log(logMessage, "", 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
