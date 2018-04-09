using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class WarningImplementer : IWarnings
    {
        private List<Warnings> warnings_list = new List<Warnings>(); //set of warnings make error file
        private List<string> warnings = new List<string>(); //list of files which contains warnings
        private Warnings warning = new Warnings();

        public void CreateWarningFile(string delimiter, string nameOfWarningFile, string pathOfWarningFile, List<string> all_CimToDmsReport_files,List<Statistics> statistics_list)
        {
            List<string> warning_file_list = new List<string>();

            foreach (string cimToDmsReportfile in all_CimToDmsReport_files)
            {
                string[] lines = File.ReadAllLines(cimToDmsReportfile);

                FileInfo currentFile = new FileInfo(cimToDmsReportfile);

                warning_file_list = ParseWarningFile(currentFile);

                string circuitNameFromCurrentWarningInFile = (lines[1].Split(':')[1]).Split(' ')[1];


                foreach (Statistics stat in statistics_list)
                {
                    if (stat.Circuit == circuitNameFromCurrentWarningInFile)
                    {
                        for (int i = 0; i < warning_file_list.Count(); i++)
                        {
                            warning.Circuit = (lines[1].Split(':')[1]).Split(' ')[1];
                            warning.File = currentFile.Name;
                            warning.Date = currentFile.CreationTime;
                            warning.FileState = stat.State;
                            warning.LogDirectory = stat.LogDirectory;
                            warning.FileContent = warning_file_list[i];
                            warnings_list.Add(warning);

                            WriteToWarningCSV(warnings_list, delimiter, nameOfWarningFile, pathOfWarningFile);
                        }
                        warning_file_list = null;
                        break;
                    }

                }
            }
        }

        public void WriteToWarningCSV(List<Warnings> warnings_list, string delimiter, string nameOfWarningFile, string pathOfWarningFile)
        {
            using (StreamWriter theWriter = new StreamWriter(pathOfWarningFile + "/" + nameOfWarningFile + ".csv"))
            {
                theWriter.WriteLine("Circuit" + delimiter + "File Content" + delimiter + "File" + delimiter + "Date" + delimiter + "File State" + delimiter + "Log Directory");

                foreach (Warnings warning_1 in warnings_list)
                {
                    theWriter.Write(warning_1.Circuit + delimiter);
                    theWriter.Write(warning_1.FileContent + delimiter);
                    theWriter.Write(warning_1.File + delimiter);
                    theWriter.Write(warning_1.Date + delimiter);
                    theWriter.Write(warning_1.FileState + delimiter);
                    theWriter.WriteLine(warning.LogDirectory);
                }
                warning = new Warnings();
            }
        }

        public List<string> ParseWarningFile(FileInfo currentFile)
        {
            warnings = new List<string>();
            string[] lines = File.ReadAllLines(currentFile.FullName);
            string warningDeclaration = null;

            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Contains("Warning with code:"))
                {
                    warningDeclaration = lines[i].Split(' ')[3];

                    for (int j = i + 1; j < lines.Count(); j++)
                    {

                        if (lines[j].Contains('-'))
                        {
                            string warningContent = (lines[j]).Split('\t')[1];
                            warnings.Add(warningDeclaration + ": " + warningContent);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
            return warnings;
        }

       
    }
}
