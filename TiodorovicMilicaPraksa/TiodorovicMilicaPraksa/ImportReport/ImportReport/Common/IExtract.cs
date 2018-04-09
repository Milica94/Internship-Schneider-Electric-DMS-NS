using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IExtract
    {
         List<Errors> ExtractErrorsFromCSV();

         List<Warnings> ExtractWarningsFromCSV();

         List<Statistics> ExtractStatisticsFromCSV();

         List<Summary> ExtractSummaryFromCSV();
    }
}
