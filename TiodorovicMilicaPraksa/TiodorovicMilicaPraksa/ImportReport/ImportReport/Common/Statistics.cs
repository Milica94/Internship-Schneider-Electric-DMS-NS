using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Statistics
    {
        private string circuit;
        private int errorCount;
        private int warningCount;
        private string state;
        private DateTime date;
        private string logDirectory;
       
        public string Circuit
        {
            get { return circuit; }
            set { circuit = value; }
        }
        public int ErrorCount
        {
            get { return errorCount; }
            set { errorCount = value; }
        }
        public int WarningCount
        {
            get { return warningCount; }
            set { warningCount = value; }
        }
        public string State
        {
            get { return state; }
            set { state = value; }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
        public string LogDirectory
        {
            get { return logDirectory; }
            set { logDirectory = value; }
        }


       


     

      


     


     

    }
}
