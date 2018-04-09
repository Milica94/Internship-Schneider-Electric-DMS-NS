using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SummaryImplementer : ISummary
    {
        private List<string> all_states_list = new List<string>();
        private List<string> state_list = new List<string>();

        public void WriteToSummaryCSV(List<Summary> summary_list, string pathOfSummaryFile, string nameOfSummaryFile, string delimiter)
        {
            using (StreamWriter theWriter = new StreamWriter(pathOfSummaryFile + "/" + nameOfSummaryFile + ".csv"))
            {
                theWriter.WriteLine("Category" + delimiter + "Count");

                foreach (Summary summary in summary_list)
                {
                    theWriter.Write(summary.Category + delimiter);
                    theWriter.WriteLine(summary.Count);
                }

            }

        }

        public List<string> ExtractStatesFromStatisticFile(string delimiter, string pathOfStatisticFile)
        {
            try
            {
                state_list = new List<string>();
                string[] lines = File.ReadAllLines(pathOfStatisticFile +".csv");

                foreach (string status in lines)
                {
                    string[] part1 = status.Split(delimiter[0]);
                    state_list.Add(part1[3]);
                }
                return state_list;
            }
            catch (Exception e)
            {
                Debug.Write("EXCEPTION:", e.Message);
                return null;
            }
        }

        public int GetNumberOfInvalidChangeset(string delimiter, string pathOfStatisticFile)
        {
            int invalidChangeset = 0;
            all_states_list = ExtractStatesFromStatisticFile(delimiter,  pathOfStatisticFile);
            try
            {
                foreach (string state in all_states_list)
                {
                    if (state == "Invalid Changeset")
                    {
                        ++invalidChangeset;
                    }
                }
                all_states_list = null;
                return invalidChangeset;
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                return 0;
            }
        }

        public int GetNumberOfInvalidExtract(string delimiter, string pathOfStatisticFile)
        {
            int invalidExtract = 0;
            all_states_list = ExtractStatesFromStatisticFile(delimiter, pathOfStatisticFile);

            try
            {
                foreach (string state in all_states_list)
                {
                    if (state == "Invalid Extract")
                    {
                        ++invalidExtract;
                    }
                }
                all_states_list = null;
                return invalidExtract;
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                return 0;
            }
        }

        public int GetNumberOfPendingChangeset(string delimiter, string pathOfStatisticFile)
        {
            int pendingExtract = 0;

            all_states_list = ExtractStatesFromStatisticFile(delimiter,   pathOfStatisticFile);

            try
            {
                foreach (string state in all_states_list)
                {
                    if (state == "Pending Extract")
                    {
                        ++pendingExtract;
                    }
                }
                all_states_list = null;
                return pendingExtract;
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                return 0;
            }
        }

        public int GetNumberOfPendingExtract(string delimiter,  string pathOfStatisticFile)
        {
            int pendingChangeset = 0;
            all_states_list = ExtractStatesFromStatisticFile(delimiter,  pathOfStatisticFile);
            try
            {
                foreach (string state in all_states_list)
                {
                    if (state == "Pending Changeset")
                    {
                        ++pendingChangeset;
                    }
                }

                all_states_list = null;
                return pendingChangeset;
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                return 0;
            }
        }

        public void CreateSummaryFile(string delimiter, string nameOfSummaryFile, string pathOfSummaryFile,  string pathOfStatisticFile)
        {
            List<Summary> summary_list = new List<Summary>();

            summary_list.Add(new Summary() { Category = "Total Number Log Directories Processed", Count = GetNumberOfInvalidChangeset(delimiter,   pathOfStatisticFile) + GetNumberOfInvalidExtract(delimiter,   pathOfStatisticFile) + GetNumberOfPendingChangeset(delimiter,    pathOfStatisticFile) + GetNumberOfPendingExtract(delimiter,   pathOfStatisticFile) });
            summary_list.Add(new Summary() { Category = "Total Invalid Changesets", Count = GetNumberOfInvalidChangeset(delimiter,   pathOfStatisticFile) });
            summary_list.Add(new Summary() { Category = "Total Invalid Extracts", Count = GetNumberOfInvalidExtract(delimiter,    pathOfStatisticFile) });
            summary_list.Add(new Summary() { Category = "Total Pending Extracts", Count = GetNumberOfPendingExtract(delimiter,   pathOfStatisticFile) });
            summary_list.Add(new Summary() { Category = "Total Pending Changesets", Count = GetNumberOfPendingChangeset(delimiter,   pathOfStatisticFile) });

            WriteToSummaryCSV(summary_list, pathOfSummaryFile, nameOfSummaryFile, delimiter);
        }

      
    }
}
