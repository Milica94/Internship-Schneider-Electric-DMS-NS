using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Common;

namespace ReadAndParse
{
    class Program
    {
        static void Main(string[] args)
        {
            StatisticImplementer statisticsImplementer = new StatisticImplementer();
            SummaryImplementer summaryImplementer = new SummaryImplementer();
            WarningImplementer warningImplementer = new WarningImplementer();
            ErrorImplementer errorImplementer = new ErrorImplementer();
            List<string> _txtfiles_list = new List<string>();
            Reader read = new Reader();
            string startPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\")) + "Reports";
            int userInput = 0;


            do
            {

                Console.WriteLine("~~~~~~~ MENU ~~~~~~~\n");
                Console.WriteLine("1. Read .txt files");
                Console.WriteLine("2. Create .csv files");
                Console.WriteLine("3. Exit");
                Console.WriteLine("\nSelect Your Option: ");

                var result = Console.ReadLine();
                userInput = Convert.ToInt32(result);

                switch (userInput)
                {
                    case 1:
                        try
                        {
                            _txtfiles_list = read.GetFilesFromDirectory(Directory.GetDirectories(startPath), Directory.GetFiles(startPath));
                            Console.WriteLine("\nText files are successfully readed!\n");
                        }
                        catch
                        {
                            Console.WriteLine("\nWrong format!\n");
                        }

                        break;

                    case 2:
                        int input;
                        do
                        {
                            Console.WriteLine("\n1. Create files");
                            Console.WriteLine("\n2. Exit");


                            var res = Console.ReadLine();
                            input = Convert.ToInt32(res);
                            switch (input)
                            {
                                case 1:
                                    Console.WriteLine("\nInsert name of statistic file:");
                                    var nameOfStatisticFile = Console.ReadLine();

                                    Console.WriteLine("\nInsert name of summary file:");
                                    var nameOfSummaryFile = Console.ReadLine();

                                    Console.WriteLine("\nInsert name of error file:");
                                    var nameOfErrorFile = Console.ReadLine();

                                    Console.WriteLine("\nInsert name of warning file:");
                                    var nameOfWarningFile = Console.ReadLine();

                                    Console.WriteLine("\nInsert separator:");
                                    var delimiter = Console.ReadLine();

                                    Console.WriteLine("\nCurrent location [ " + Directory.GetCurrentDirectory() + " ]");

                                    Console.WriteLine("\nDo you want change location? [Y/N]");
                                    var replay = Console.ReadLine();

                                 

                                    if (replay == "Y")
                                    {
                                        Console.WriteLine("\n1. Select path of directory where you want to save file:");
                                        string saveFileDirectory = Console.ReadLine();
                                        DirectoryInfo _directoryInfo = new DirectoryInfo(saveFileDirectory);

                                        if (!_directoryInfo.Exists)
                                        {
                                            Console.WriteLine("Directory does not exist.");
                                        }
                                        else
                                        {
                                            try
                                            {


                                                List<string> cimToDmsFiles_list = read.GetCIMToDMSFiles(_txtfiles_list);

                                                statisticsImplementer.CreateStatisticFile(delimiter, nameOfStatisticFile, _directoryInfo.FullName, _txtfiles_list);
                                                Console.WriteLine("\n" + nameOfStatisticFile + ".csv succesfully created!");

                                                summaryImplementer.CreateSummaryFile(delimiter, nameOfSummaryFile, _directoryInfo.FullName, nameOfStatisticFile);
                                                Console.WriteLine("\n" + nameOfSummaryFile + ".csv je succesfully created!");

                                                List<Common.Statistics> listOfStatistics = GetStatisticList(nameOfStatisticFile, delimiter);

                                                errorImplementer.CreateErrorCSVFile(delimiter, nameOfErrorFile, _directoryInfo.FullName, cimToDmsFiles_list, listOfStatistics);
                                                Console.WriteLine("\n" + nameOfErrorFile + ".csv je succesfully created!");

                                                warningImplementer.CreateWarningFile(delimiter, nameOfWarningFile, _directoryInfo.FullName, cimToDmsFiles_list, listOfStatistics);
                                                Console.WriteLine("\n" + nameOfWarningFile + ".csv je succesfully created!");
                                                cimToDmsFiles_list = new List<string>();

                                            }
                                            catch (TypeInitializationException tie)
                                            {
                                                Console.WriteLine(tie.InnerException);
                                            }

                                        }

                                    }
                                    else if (replay == "N")
                                    {
                                        DirectoryInfo _directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
                                        try
                                        {

                                            List<string> cimToDmsFiles_list = read.GetCIMToDMSFiles(_txtfiles_list);

                                            statisticsImplementer.CreateStatisticFile(delimiter, nameOfStatisticFile, _directoryInfo.FullName, _txtfiles_list);
                                            Console.WriteLine("\n" + nameOfStatisticFile + ".csv succesfully created!");

                                            summaryImplementer.CreateSummaryFile(delimiter, nameOfSummaryFile, _directoryInfo.FullName, nameOfStatisticFile);
                                            Console.WriteLine("\n" + nameOfSummaryFile + ".csv je succesfully created!");

                                        
                                            List<Common.Statistics> listOfStatistics = GetStatisticList(nameOfStatisticFile, delimiter);

                                            errorImplementer.CreateErrorCSVFile(delimiter, nameOfErrorFile, _directoryInfo.FullName, cimToDmsFiles_list, listOfStatistics);
                                            Console.WriteLine("\n" + nameOfErrorFile + ".csv je succesfully created!");

                                            warningImplementer.CreateWarningFile(delimiter, nameOfWarningFile, _directoryInfo.FullName, cimToDmsFiles_list, listOfStatistics);
                                            Console.WriteLine("\n" + nameOfWarningFile + ".csv je succesfully created!");
                                            Console.WriteLine("\n1. Files successfully saved!");

                                            cimToDmsFiles_list = new List<string>();

                                        }
                                        catch (TypeInitializationException tie)
                                        {
                                            Console.WriteLine(tie.InnerException);
                                        }
                                    }
                                    break;
                            }
                        } while (input != 2);
                        break;

                }

            } while (userInput != 3);


        }

        private static List<Common.Statistics> GetStatisticList(string statisticFileName, string delimiter)
        {
            List<Common.Statistics> statistic_list = new List<Common.Statistics>();
            try
            {
                StreamReader reader = new StreamReader(statisticFileName + ".csv");
                reader.ReadLine();

                while (reader.Peek() != -1)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(delimiter[0]);
                    Common.Statistics s = new Common.Statistics();
                    s.Circuit = values[0];
                    s.ErrorCount = Int32.Parse(values[1]);
                    s.WarningCount = Int32.Parse(values[2]);
                    s.State = values[3];
                    s.Date = DateTime.Parse(values[4]);
                    s.LogDirectory = values[5];

                    statistic_list.Add(s);

                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Could not read file from disk. Original error: " + ex.Message);

            }
            return statistic_list;
        }
        
    }
}
