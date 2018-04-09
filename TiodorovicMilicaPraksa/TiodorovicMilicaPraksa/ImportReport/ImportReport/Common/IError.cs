using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IError
    {
        List<string> ParseErrorFile(FileInfo currentFile);

        void WriteToErrorCSV(List<Errors> errors_list, string nameOfErrorFile, string pathOfErrorFile, string delimiter);

        void CreateErrorCSVFile(string delimiter, string nameOfErrorFile, string pathOfErrorFile, List<string> all_CimToDmsReport_files, List<Statistics> statistics_list);
    }
}
