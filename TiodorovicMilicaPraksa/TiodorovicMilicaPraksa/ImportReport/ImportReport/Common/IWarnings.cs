using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IWarnings
    {
        List<string> ParseWarningFile(FileInfo currentFile);

        void CreateWarningFile(string delimiter, string nameOfWarningFile, string pathOfWarningFile, List<string> all_CimToDmsReport_files, List<Statistics> statistics_list);   

        void WriteToWarningCSV(List<Warnings> warnings_list, string delimiter, string nameOfWarningFile, string pathOfWarningFile);

    }
}
