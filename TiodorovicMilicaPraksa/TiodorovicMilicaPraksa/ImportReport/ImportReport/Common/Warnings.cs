using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Common
{
    public class Warnings
    {
        private string circuit;
        private string fileContent;
        private string file;
        private DateTime date;
        private string fileState;
        private string logDirectory;


        public string Circuit
        {
            get { return circuit; }
            set { circuit = value; }
        }
        public string FileContent
        {
            get { return fileContent; }
            set { fileContent = value; }
        }
        public string File
        {
            get { return file; }
            set { file = value; }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public string FileState
        {
            get { return fileState; }
            set { fileState = value; }
        }
        public string LogDirectory
        {
            get { return logDirectory; }
            set { logDirectory = value; }
        }




     

    


      



    }
}
