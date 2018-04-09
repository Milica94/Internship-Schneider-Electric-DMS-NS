using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface ISummary
    {
        void CreateSummaryFile(string delimiter, string nameOfSummaryFile, string pathOfSummaryFile,  string pathOfStatisticFile);

        List<string> ExtractStatesFromStatisticFile(string delimiter,  string pathOfStatisticFile);

        int GetNumberOfInvalidChangeset(string delimiter, string pathOfStatisticFile);

        int GetNumberOfInvalidExtract(string delimiter, string pathOfStatisticFile);

        int GetNumberOfPendingExtract(string delimiter,string pathOfStatisticFile);

        int GetNumberOfPendingChangeset(string delimiter, string pathOfStatisticFile);

        void WriteToSummaryCSV(List<Summary> summary_list, string pathOfSummaryFile, string nameOfSummaryFile, string delimiter);

    }
}
