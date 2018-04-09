using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Reader : IRead
    {
        public List<string> files_list = new List<string>();
        public List<string> CIMToDMSTransformReportFile_list = new List<string>();


        public List<string> GetFilesFromDirectory(string[] directories, string[] files)
        {
            foreach (string pathOfDirectory in directories)
            {
                DirectoryInfo _directoryinfo = new DirectoryInfo(pathOfDirectory);
                try
                {
                    GetFilesFromDirectory(Directory.GetDirectories(pathOfDirectory), Directory.GetFiles(pathOfDirectory));
                }
                catch (Exception e)
                {
                    Debug.Write("Exception from GetFilesFromDirectory()->", e.Message.ToString());
                }

            }
            foreach (string pathOfFile in files)
            {
                FileInfo _fileinfo = new FileInfo(pathOfFile);
                files_list.Add(_fileinfo.FullName);
            }
            return files_list;

        }

        public List<string> GetCIMToDMSFiles(List<string> all_files)
        {
            foreach (string file in all_files)
            {
                string text = File.ReadAllText(file);
                if (text.Contains("Number of reported errors:"))
                {
                    CIMToDMSTransformReportFile_list.Add(file);
                }
            }
            return CIMToDMSTransformReportFile_list;
        }
    }
}
