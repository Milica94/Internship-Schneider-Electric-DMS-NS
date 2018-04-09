using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IStatistics
    {
        string ParseStatisticsFile(FileInfo currentFileInfo, string delimiter, string nameOfStatisticFile, string pathOfStatisticFile);

        void WriteToStatisticCSV(string delimiter, string nameOfStatisticFile, string pathOfStatisticFile);

        void CreateStatisticFile(string delimiter, string nameOfStatisticFile, string pathOfStatisticFile, List<string> listOfAllTxtFiles);

    
    }
}
