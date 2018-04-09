using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IRead
    {
        List<string> GetFilesFromDirectory(string[] directories, string[] files);

        List<string> GetCIMToDMSFiles(List<string> all_files);

    }
}
