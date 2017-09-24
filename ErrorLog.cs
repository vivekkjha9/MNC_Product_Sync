using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AX_CRT_MAge_Connector
{
    class ErrorLog
    {
        private string sLogFormat;
        private string sErrorTime;

        public  ErrorLog()
        {
            //sLogFormat used to create log files format :
            // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message
            sLogFormat = DateTime.Now.ToShortDateString().ToString()+" "+DateTime.Now.ToLongTimeString().ToString()+" -- ";
            
            //this variable used to create log filename format "
            //for example filename : ErrorLogYYYYMMDD
            string sYear    = DateTime.Now.Year.ToString();
            string sMonth    = DateTime.Now.Month.ToString();
            string sDay    = DateTime.Now.Day.ToString();
            sErrorTime = sYear+sMonth+sDay;
        }

        public void LogError(string sPathName, string sErrMsg)
        {


            using (StreamWriter sw = (File.Exists(sPathName + "\\" + sErrorTime + ".log")) ? File.AppendText(sPathName + "\\" + sErrorTime + ".log") : File.CreateText(sPathName + "\\" + sErrorTime + ".log"))
            {

                sw.WriteLine(sLogFormat + sErrMsg); ;
                sw.Flush();
                sw.Close();

               
            }     
        }
    }
}
