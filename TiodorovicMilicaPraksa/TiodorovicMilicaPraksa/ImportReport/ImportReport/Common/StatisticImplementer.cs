using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class StatisticImplementer : IStatistics
    {

        private List<Statistics> statistics_list = new List<Statistics>();
        private Statistics statistics = new Statistics();

        
        public void CreateStatisticFile(string delimiter, string nameOfStatisticFile, string pathOfStatisticFile, List<string> listOfAllTxtFiles)
        {
            foreach (string file in listOfAllTxtFiles)
            {
                FileInfo currentFileInfo = new FileInfo(file);

                string[] lines = File.ReadAllLines(currentFileInfo.FullName);

                foreach (string line in lines) //detecting the file according to the content
                {
                    if (line.Contains("Parsing CIM extract file.") || line.Contains("Number of reported errors:"))// SummaryReport.txt
                    {
                        
                        ParseStatisticsFile(currentFileInfo, delimiter, nameOfStatisticFile, pathOfStatisticFile);
                    }
                }

            }
        }

        public string ParseStatisticsFile(FileInfo currentFileInfo, string delimiter, string nameOfStatisticFile, string pathOfStatisticFile)
        {
            //1. Detect file
            //2. Extract the information needed to create a statistic file

            string[] lines = File.ReadAllLines(currentFileInfo.FullName);

            foreach (string line in lines)
            {
                if (line.Contains("Number of reported errors:"))
                {
                    string[] part = line.Split(':');
                    string[] part2 = part[1].Split(' ');
                    statistics.ErrorCount = Int32.Parse(part2[1]);
                }
                else if (line.Contains("Parsing CIM extract file."))
                {
                    if (statistics.ErrorCount > 0 && lines.Count() > 2)
                    {
                        statistics.State = "Invalid Changeset";
                    }
                    else if (statistics.ErrorCount == 0 && lines.Count() > 2)
                    {
                        statistics.State = "Pending Changeset";
                    }
                    else if (statistics.ErrorCount == 0 && lines.Count() == 2)
                    {
                        statistics.State = "Pending Extract";
                    }
                    else if (statistics.ErrorCount > 0 && lines.Count() == 2)
                    {
                        statistics.State = "Invalid Extract";
                    }

                    statistics_list.Add(statistics);

                    WriteToStatisticCSV(delimiter, nameOfStatisticFile, pathOfStatisticFile);
                    break;

                }
                else if (line.Contains("Circuit name"))
                {
                    string[] part = line.Split(':');
                    string[] part2 = part[1].Split(' ');
                    statistics.Circuit = part2[1];
                }
                else if (line.Contains("Number of reported warnings:"))
                {
                    string[] part = line.Split(':');
                    string[] part2 = part[1].Split(' ');
                    statistics.WarningCount = Int32.Parse(part2[1]);
                    statistics.Date = currentFileInfo.CreationTime;
                    statistics.LogDirectory = "Reports_for_" + statistics.State + "_" + statistics.Circuit + "_created at_" + statistics.Date;
                    break;
                }


            }
            return nameOfStatisticFile;
        }

        public void WriteToStatisticCSV(string delimiter, string nameOfStatisticFile, string pathOfStatisticFile)
        {

            using (StreamWriter theWriter = new StreamWriter(pathOfStatisticFile + "\\" + nameOfStatisticFile + ".csv"))
            {
                theWriter.WriteLine("Circuit name" + delimiter + "ErrorCount" + delimiter + "WarningCount" + delimiter + "State" + delimiter + "Date" + delimiter + "LogDirectory");
                foreach (Statistics stat in statistics_list)
                {
                    theWriter.Write(stat.Circuit + delimiter);
                    theWriter.Write(stat.ErrorCount + delimiter);
                    theWriter.Write(stat.WarningCount + delimiter);
                    theWriter.Write(stat.State + delimiter);
                    theWriter.Write(stat.Date + delimiter);
                    theWriter.WriteLine(stat.LogDirectory);
                }

                statistics = new Statistics();
            }
        }

       
    }
}
