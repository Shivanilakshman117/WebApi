using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApi.Utilities
{
    public class ExceptionLog
    {

        public static void Logger(Exception Ex)
        {

            string logfile = "C:\\Users\\Shivani.Lakshman\\Documents\\Impact Phase 2\\EmployeeManagementSystem\\MyLogFile.txt";
            if (!File.Exists(logfile))
            {
                using (StreamWriter sw = File.CreateText(logfile))
                {
                    sw.WriteLine("___________________________________________________________________");
                    sw.WriteLine("\n\n" + DateTime.Now.ToString() + "\n\n" + Ex.GetType().FullName + "\n\n" + Ex.Message + "\n" + Ex.StackTrace + "\n" + DateTime.Now.ToLongDateString());
                }
            }
            using (StreamWriter sw = File.AppendText(logfile))
            {
                sw.WriteLine("___________________________________________________________________");
                sw.WriteLine("\n\n" + DateTime.Now.ToString() + "\n\n" + Ex.GetType().FullName + "\n\n" + Ex.Message + "\n" + Ex.StackTrace + "\n" + DateTime.Now.ToLongDateString());
            }
        }
    }
}