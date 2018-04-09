using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ErrorImplementer : IError
    {
        public List<string> errorsFile = new List<string>(); //list of files which contains errors
        public List<Errors> errors_list = new List<Errors>(); //set of errors make error file
        public Errors error = new Errors();


        #region Error

        public void CreateErrorCSVFile(string delimiter, string nameOfErrorFile, string pathOfErrorFile, List<string> all_CimToDmsReport_files, List<Statistics> statistics_list)
        {
            List<string> errorFile_list = new List<string>();

            foreach (string cimToDmsReportfile in all_CimToDmsReport_files)
            {
                string[] lines = File.ReadAllLines(cimToDmsReportfile);

                FileInfo currentFile = new FileInfo(cimToDmsReportfile);

                errorFile_list = ParseErrorFile(currentFile); //detect error in Report file

                string circuitNameFromCurrentErrorInFile = (lines[1].Split(':')[1]).Split(' ')[1];

                foreach (Statistics statistic in statistics_list)
                {
                    if (statistic.Circuit == circuitNameFromCurrentErrorInFile)
                    {

                        for (int i = 0; i < errorFile_list.Count(); i++)
                        {
                            error.Circuit = circuitNameFromCurrentErrorInFile;
                            error.File = currentFile.Name;
                            error.Date = currentFile.CreationTime;
                            error.FileState = statistic.State;
                            error.LogDirectory = statistic.LogDirectory;
                            error.FileContent = errorFile_list[i];
                            errors_list.Add(error);

                            WriteToErrorCSV(errors_list, nameOfErrorFile, pathOfErrorFile, delimiter);
                        }
                        errorFile_list = null;
                        break;
                    }

                }
            }
        }

        public void WriteToErrorCSV(List<Errors> errors_list, string nameOfErrorFile, string pathOfErrorFile, string delimiter)
        {
            using (StreamWriter theWriter = new StreamWriter(pathOfErrorFile + "/" + nameOfErrorFile + ".csv"))
            {
                theWriter.WriteLine("Circuit" + delimiter + "File Content" + delimiter + "File" + delimiter + "Date" + delimiter + "File State" + delimiter + "Log Directory");

                foreach (Errors error_1 in errors_list)
                {
                    theWriter.Write(error_1.Circuit + delimiter);
                    theWriter.Write(error_1.FileContent + delimiter);
                    theWriter.Write(error_1.File + delimiter);
                    theWriter.Write(error_1.Date + delimiter);
                    theWriter.Write(error_1.FileState + delimiter);
                    theWriter.WriteLine(error_1.LogDirectory);
                }
                error = new Errors();
            }
        }

        public List<string> ParseErrorFile(FileInfo currentFile) //detect errors in current file
        {
            errorsFile = new List<string>();

            string[] lines = File.ReadAllLines(currentFile.FullName);

            string errorDeclaration = null;

            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Contains("Error with code:"))
                {
                    errorDeclaration = lines[i].Split(' ')[3];

                    for (int j = i + 1; j < lines.Count(); j++)
                    {

                        if (lines[j].StartsWith("\t -"))
                        {
                            string errorContent = (lines[j]).Split('\t')[1];
                            errorsFile.Add(errorDeclaration + ": " + errorContent);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
            return errorsFile;
        }

        #endregion
    }
}
