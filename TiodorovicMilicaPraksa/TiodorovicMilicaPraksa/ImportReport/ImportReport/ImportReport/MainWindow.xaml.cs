using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Windows.Controls.DataVisualization.Charting;
using System.Collections;
using Common;

namespace ImportReport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region
        private bool validateSeparatorTextBox, validateStatisticsTextBox, validateSummaryTextBox, validateErrorTextBox, validateWarningTextBox;
        StatisticImplementer statisticsImplementer = new StatisticImplementer();
        SummaryImplementer summaryImplementer = new SummaryImplementer();
        WarningImplementer warningImplementer = new WarningImplementer();
        ErrorImplementer errorImplementer = new ErrorImplementer();
        Reader read = new Reader();
        private List<string> all_files = new List<string>();
        private int numberOfClickOnDatabaseButton = 0;
        public List<Statistics> statistics_list1 = new List<Statistics>();
        public List<Statistics> statistics_list = new List<Statistics>();
        public FileInfo oldNameDatabase;
        #endregion

        #region Methods

        public MainWindow()
        {
            InitializeComponent();   
        }

        private List<Common.Errors> ExtractErrorsFromCSV()
        {
            List<Errors> errors_list = new List<Errors>();

                try
                {
                    StreamReader reader = new StreamReader(tb_errorFileName.Text + ".csv");
                    reader.ReadLine();

                    while (reader.Peek() != -1)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(tb_separator.Text[0]);

                        Errors errors = new Errors();
                        errors.Circuit = values[0];
                        errors.FileContent = values[1];
                        errors.File = values[2];
                        errors.Date = DateTime.Parse(values[3]);
                        errors.FileState = values[4];
                        errors.LogDirectory = values[5];
                        errors_list.Add(errors);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Could not read file from disk. Original error: " + ex.Message);

                }
            

            return errors_list;
        }

        private List<Warnings> ExtractWarningsFromCSV()
        {
            List<Warnings> warning_list = new List<Warnings>();

            try
            {
                StreamReader reader = new StreamReader(tb_warningFileName.Text + ".csv");
                reader.ReadLine();

                while (reader.Peek() != -1)
                {

                    while (reader.Peek() != -1)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(tb_separator.Text[0]);
                        Warnings warning = new Warnings();
                        warning.Circuit = values[0];
                        warning.FileContent = values[1];
                        warning.File = values[2];
                        warning.Date = DateTime.Parse(values[3]);
                        warning.FileState = values[4];
                        warning.LogDirectory = values[5];
                        warning_list.Add(warning);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Could not read file from disk. Original error: " + ex.Message);

            }


            return warning_list;
        }

        private List<Statistics> ExtractStatisticsFromCSV()
        {
            List<Statistics> statistic_list = new List<Statistics>();
            try
            {
                StreamReader reader = new StreamReader(tb_statisticFileName.Text + ".csv");
                reader.ReadLine();

                while (reader.Peek() != -1)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(tb_separator.Text[0]);
                    Statistics s = new Statistics();
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

        private List<Summary> ExtractSummaryFromCSV()
        {
            List<Summary> summary_list = new List<Summary>();
            try
            {
                StreamReader reader = new StreamReader(tb_summaryFileName.Text + ".csv");
                reader.ReadLine();

                while (reader.Peek() != -1)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(tb_separator.Text[0]);
                    Summary summary = new Summary();
                    summary.Category = values[0];
                    summary.Count = Int32.Parse(values[1]);
                    summary_list.Add(summary);
                }

               
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
            return summary_list;
        }

        private void SaveToDatabase(FileInfo fileName,int i)
        {

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            List<Statistics> statistic_list = ExtractStatisticsFromCSV();
            List<Summary> summary_list = ExtractSummaryFromCSV();
            List<Errors> error_list = ExtractErrorsFromCSV();
            List<Warnings> warning_list = ExtractWarningsFromCSV();

            SQLiteConnection sqlite_connection;
            SQLiteCommand sqlite_command;
            string databasebName = fileName.FullName;
            sqlite_connection = new SQLiteConnection("Data Source="+ databasebName + ";Version=3;New=True;Compress=True;");
            sqlite_command = new SQLiteCommand("begin", sqlite_connection);
            sqlite_connection.Open();
            sqlite_command = sqlite_connection.CreateCommand();

            if (numberOfClickOnDatabaseButton == 1)
            {
                try
                {
                    //Statistics
                    sqlite_command.CommandText = "CREATE TABLE Statistics (CircuitName varchar(100), ErrorCount integer, WarningCount integer,State varchar(100),Date DATETIME,LogDirectory varchar(100),TimeStamp DATETIME);";
                    sqlite_command.ExecuteNonQuery();

                    DateTime _dateTime = DateTime.Now;

                    using (SQLiteTransaction mytransaction = sqlite_connection.BeginTransaction())
                    {
                        using (SQLiteCommand mycommand = new SQLiteCommand(sqlite_connection))
                        {
                            SQLiteParameter myparam = new SQLiteParameter();

                            for (int n = 0; n < statistic_list.Count; n++)
                            {
                                mycommand.CommandText = "INSERT INTO Statistics (CircuitName,ErrorCount,WarningCount,State,Date,LogDirectory,TimeStamp) VALUES (@CircuitName, @ErrorCount,@WarningCount,@State, @Date, @LogDirectory,@TimeStamp);";
                                mycommand.Parameters.Add(myparam);
                                mycommand.Parameters.AddWithValue("@TimeStamp", @_dateTime);
                                mycommand.Parameters.AddWithValue("@CircuitName", @statistic_list[n].Circuit);
                                mycommand.Parameters.AddWithValue("@ErrorCount", @statistic_list[n].ErrorCount);
                                mycommand.Parameters.AddWithValue("@WarningCount", @statistic_list[n].WarningCount);
                                mycommand.Parameters.AddWithValue("@State", @statistic_list[n].State);
                                mycommand.Parameters.AddWithValue("@Date", @statistic_list[n].Date);
                                mycommand.Parameters.AddWithValue("@LogDirectory", @statistic_list[n].LogDirectory);
                                mycommand.ExecuteNonQuery();
                            }
                        }
                        mytransaction.Commit();
                    }


                    //Summary
                    sqlite_command.CommandText = "CREATE TABLE Summary (Category varchar(100), Count integer,TimeStamp DATETIME);";
                    sqlite_command.ExecuteNonQuery();

                    using (SQLiteTransaction mytransaction = sqlite_connection.BeginTransaction())
                    {
                        using (SQLiteCommand mycommand = new SQLiteCommand(sqlite_connection))
                        {
                            SQLiteParameter myparam = new SQLiteParameter();

                            for (int n = 0; n < summary_list.Count; n++)
                            {
                                mycommand.CommandText = "INSERT INTO Summary (Category,Count,TimeStamp) VALUES (@Category, @Count,@TimeStamp);";
                                mycommand.Parameters.Add(myparam);
                                mycommand.Parameters.AddWithValue("@TimeStamp", @_dateTime);
                                mycommand.Parameters.AddWithValue("@Category", @summary_list[n].Category);
                                mycommand.Parameters.AddWithValue("@Count", @summary_list[n].Count);
                                mycommand.ExecuteNonQuery();
                            }
                        }
                        mytransaction.Commit();
                    }


                    //Errors
                    sqlite_command.CommandText = "CREATE TABLE Errors (CircuitName varchar(100), FileContent varchar(100), File varchar(100),Date DATETIME,FileState varchar(100),LogDirectory varchar(100),TimeStamp DATETIME);";
                    sqlite_command.ExecuteNonQuery();


                    using (SQLiteTransaction mytransaction = sqlite_connection.BeginTransaction())
                    {
                        using (SQLiteCommand mycommand = new SQLiteCommand(sqlite_connection))
                        {
                            SQLiteParameter myparam = new SQLiteParameter();

                            for (int n = 0; n < error_list.Count; n++)
                            {
                                mycommand.CommandText = "INSERT INTO Errors (CircuitName,FileContent,File,Date,FileState,LogDirectory,TimeStamp) VALUES (@CircuitName, @FileContent,@File,@Date, @FileState, @LogDirectory,@TimeStamp);";
                                mycommand.Parameters.Add(myparam);
                                mycommand.Parameters.AddWithValue("@TimeStamp", @_dateTime);
                                mycommand.Parameters.AddWithValue("@CircuitName", @error_list[n].Circuit);
                                mycommand.Parameters.AddWithValue("@FileContent", @error_list[n].FileContent);
                                mycommand.Parameters.AddWithValue("@File", @error_list[n].File);
                                mycommand.Parameters.AddWithValue("@Date", @error_list[n].Date);
                                mycommand.Parameters.AddWithValue("@FileState", @error_list[n].FileState);
                                mycommand.Parameters.AddWithValue("@LogDirectory", @error_list[n].LogDirectory);
                                mycommand.ExecuteNonQuery();
                            }
                        }
                        mytransaction.Commit();
                    }



                    //Warnings
                    sqlite_command.CommandText = "CREATE TABLE Warnings(CircuitName varchar(100), FileContent varchar(100), File varchar(100),Date DATETIME,FileState varchar(100),LogDirectory varchar(100),TimeStamp DATETIME);";
                    sqlite_command.ExecuteNonQuery();

                    using (SQLiteTransaction mytransaction = sqlite_connection.BeginTransaction())
                    {
                        using (SQLiteCommand mycommand = new SQLiteCommand(sqlite_connection))
                        {
                            SQLiteParameter myparam = new SQLiteParameter();
                            for (int n = 0; n < warning_list.Count; n++)
                            {

                                mycommand.CommandText = "INSERT INTO Warnings (CircuitName,FileContent,File,Date,FileState,LogDirectory,TimeStamp) VALUES (@CircuitName, @FileContent,@File,@Date, @FileState, @LogDirectory,@TimeStamp);";
                                mycommand.Parameters.Add(myparam);
                                mycommand.Parameters.AddWithValue("@TimeStamp", @_dateTime);
                                mycommand.Parameters.AddWithValue("@CircuitName", @warning_list[n].Circuit);
                                mycommand.Parameters.AddWithValue("@FileContent", @warning_list[n].FileContent);
                                mycommand.Parameters.AddWithValue("@File", @warning_list[n].File);
                                mycommand.Parameters.AddWithValue("@Date", @warning_list[n].Date);
                                mycommand.Parameters.AddWithValue("@FileState", @warning_list[n].FileState);
                                mycommand.Parameters.AddWithValue("@LogDirectory", @warning_list[n].LogDirectory);
                                mycommand.ExecuteNonQuery();
                            }
                        }
                        mytransaction.Commit();
                    }

                    sqlite_command = new SQLiteCommand("end", sqlite_connection);
                    System.Windows.MessageBox.Show("Saved to database successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    sqlite_connection.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);

                }
            }
            else
            {
                string databasebName1 = fileName.FullName;
                sqlite_connection = new SQLiteConnection("Data Source=" + databasebName1 + ";Version=3;New=True;Compress=True;");
                sqlite_command = new SQLiteCommand("begin", sqlite_connection);
                sqlite_connection.Open();
                sqlite_command = sqlite_connection.CreateCommand();

                    try
                {
                    DateTime _dateTime = DateTime.Now;

                using (SQLiteTransaction mytransaction = sqlite_connection.BeginTransaction())
                {
                    using (SQLiteCommand mycommand = new SQLiteCommand(sqlite_connection))
                    {
                        SQLiteParameter myparam = new SQLiteParameter();

                        for (int n = 0; n < statistic_list.Count; n++)
                        {
                            mycommand.CommandText = "INSERT INTO Statistics (CircuitName,ErrorCount,WarningCount,State,Date,LogDirectory,TimeStamp) VALUES (@CircuitName, @ErrorCount,@WarningCount,@State, @Date, @LogDirectory,@TimeStamp);";
                            mycommand.Parameters.Add(myparam);
                            mycommand.Parameters.AddWithValue("@TimeStamp", @_dateTime);
                            mycommand.Parameters.AddWithValue("@CircuitName", @statistic_list[n].Circuit);
                            mycommand.Parameters.AddWithValue("@ErrorCount", @statistic_list[n].ErrorCount);
                            mycommand.Parameters.AddWithValue("@WarningCount", @statistic_list[n].WarningCount);
                            mycommand.Parameters.AddWithValue("@State", @statistic_list[n].State);
                            mycommand.Parameters.AddWithValue("@Date", @statistic_list[n].Date);
                            mycommand.Parameters.AddWithValue("@LogDirectory", @statistic_list[n].LogDirectory);
                            mycommand.ExecuteNonQuery();
                        }
                    }
                    mytransaction.Commit();
                }


                //Summary


                using (SQLiteTransaction mytransaction = sqlite_connection.BeginTransaction())
                {
                    using (SQLiteCommand mycommand = new SQLiteCommand(sqlite_connection))
                    {
                        SQLiteParameter myparam = new SQLiteParameter();

                        for (int n = 0; n < summary_list.Count; n++)
                        {
                            mycommand.CommandText = "INSERT INTO Summary (Category,Count,TimeStamp) VALUES (@Category, @Count,@TimeStamp);";
                            mycommand.Parameters.Add(myparam);
                            mycommand.Parameters.AddWithValue("@TimeStamp", @_dateTime);
                            mycommand.Parameters.AddWithValue("@Category", @summary_list[n].Category);
                            mycommand.Parameters.AddWithValue("@Count", @summary_list[n].Count);
                            mycommand.ExecuteNonQuery();
                        }
                    }
                    mytransaction.Commit();
                }


                //Errors

                using (SQLiteTransaction mytransaction = sqlite_connection.BeginTransaction())
                {
                    using (SQLiteCommand mycommand = new SQLiteCommand(sqlite_connection))
                    {
                        SQLiteParameter myparam = new SQLiteParameter();

                        for (int n = 0; n < error_list.Count; n++)
                        {
                            mycommand.CommandText = "INSERT INTO Errors (CircuitName,FileContent,File,Date,FileState,LogDirectory,TimeStamp) VALUES (@CircuitName, @FileContent,@File,@Date, @FileState, @LogDirectory,@TimeStamp);";
                            mycommand.Parameters.Add(myparam);
                            mycommand.Parameters.AddWithValue("@TimeStamp", @_dateTime);
                            mycommand.Parameters.AddWithValue("@CircuitName", @error_list[n].Circuit);
                            mycommand.Parameters.AddWithValue("@FileContent", @error_list[n].FileContent);
                            mycommand.Parameters.AddWithValue("@File", @error_list[n].File);
                            mycommand.Parameters.AddWithValue("@Date", @error_list[n].Date);
                            mycommand.Parameters.AddWithValue("@FileState", @error_list[n].FileState);
                            mycommand.Parameters.AddWithValue("@LogDirectory", @error_list[n].LogDirectory);
                            mycommand.ExecuteNonQuery();
                        }
                    }
                    mytransaction.Commit();
                }

                //Warnings

                using (SQLiteTransaction mytransaction = sqlite_connection.BeginTransaction())
                {
                    using (SQLiteCommand mycommand = new SQLiteCommand(sqlite_connection))
                    {
                        SQLiteParameter myparam = new SQLiteParameter();
                        for (int n = 0; n < warning_list.Count; n++)
                        {

                            mycommand.CommandText = "INSERT INTO Warnings (CircuitName,FileContent,File,Date,FileState,LogDirectory,TimeStamp) VALUES (@CircuitName, @FileContent,@File,@Date, @FileState, @LogDirectory,@TimeStamp);";
                            mycommand.Parameters.Add(myparam);
                            mycommand.Parameters.AddWithValue("@TimeStamp", @_dateTime);
                            mycommand.Parameters.AddWithValue("@CircuitName", @warning_list[n].Circuit);
                            mycommand.Parameters.AddWithValue("@FileContent", @warning_list[n].FileContent);
                            mycommand.Parameters.AddWithValue("@File", @warning_list[n].File);
                            mycommand.Parameters.AddWithValue("@Date", @warning_list[n].Date);
                            mycommand.Parameters.AddWithValue("@FileState", @warning_list[n].FileState);
                            mycommand.Parameters.AddWithValue("@LogDirectory", @warning_list[n].LogDirectory);
                            mycommand.ExecuteNonQuery();
                        }
                    }
                    mytransaction.Commit();
                }

                sqlite_command = new SQLiteCommand("end", sqlite_connection);
                System.Windows.MessageBox.Show("Saved to database successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    sqlite_connection.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);

            }
            }


        }
        #endregion

        #region Events (button, datagrid, textbox)

        private void btn_loadCSV_Click(object sender, RoutedEventArgs e)
        {
            this.dataGridStatistics.ItemsSource = ExtractStatisticsFromCSV();
            this.dataGridSummary.ItemsSource = ExtractSummaryFromCSV();
            this.dataGridError.ItemsSource = ExtractErrorsFromCSV();
            this.dataGridWarning.ItemsSource = ExtractWarningsFromCSV();



            ((PieSeries)Chart.Series[0]).ItemsSource =
                 new KeyValuePair<string, int>[]{
                        new KeyValuePair<string, int>("Error Count", ExtractErrorsFromCSV().Count),
                        new KeyValuePair<string, int>("Warning Count", ExtractWarningsFromCSV().Count),
               };

            btn_saveToDatabase.IsEnabled = true;
        }

        private void btn_loadTxt_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            
            FolderBrowserDialog folderBrowsingDialog = new FolderBrowserDialog();

            if (folderBrowsingDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if(!folderBrowsingDialog.SelectedPath.Contains("Reports"))
                {
                    System.Windows.Forms.MessageBox.Show("This directory does not contain the appropriate files. ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    all_files = read.GetFilesFromDirectory(Directory.GetDirectories(folderBrowsingDialog.SelectedPath), Directory.GetFiles(folderBrowsingDialog.SelectedPath));
                    btn_save.IsEnabled = true;
                    labelPath.Content = folderBrowsingDialog.SelectedPath;
                }
               
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            List<string> CIMToDMS_files = new List<string>();

            CIMToDMS_files = read.GetCIMToDMSFiles(all_files);

            if (tb_separator.Text == "")
            {
                lb_separatorValidate.Visibility = Visibility.Visible;
                lb_separatorValidate.Content = "Field must be filled.";
                validateSeparatorTextBox = false;
            }
             if (tb_errorFileName.Text == "")
            {
                lb_errorTextBoxValidator.Visibility = Visibility.Visible;
                lb_errorTextBoxValidator.Content = "Field must be filled.";
                validateErrorTextBox = false;
            }
             if(tb_statisticFileName.Text == "")
            {
                lb_statisticTextBoxValidator.Visibility = Visibility.Visible;
                lb_statisticTextBoxValidator.Content = "Field must be filled.";
                validateStatisticsTextBox = false;
            }
             if(tb_summaryFileName.Text == "")
            {
                lb_summaryTextBoxValidator.Visibility = Visibility.Visible;
                lb_summaryTextBoxValidator.Content = "Field must be filled.";
                validateSummaryTextBox = false;
            }
             if(tb_warningFileName.Text == "")
            {
                lb_warningTextBoxValidator.Visibility = Visibility.Visible;
                lb_warningTextBoxValidator.Content = "Field must be filled.";
                validateWarningTextBox = false;
            }

            if (validateSeparatorTextBox == true && validateStatisticsTextBox == true && validateSummaryTextBox == true && validateErrorTextBox == true && validateWarningTextBox == true)
            {
                FileInfo statisticFilePath = new FileInfo(tb_statisticFileName.Text);

                try
                {
                    statisticsImplementer.CreateStatisticFile(tb_separator.Text, tb_statisticFileName.Text, AppDomain.CurrentDomain.BaseDirectory, all_files);
                    summaryImplementer.CreateSummaryFile(tb_separator.Text, tb_summaryFileName.Text, AppDomain.CurrentDomain.BaseDirectory, statisticFilePath.FullName);
                    errorImplementer.CreateErrorCSVFile(tb_separator.Text, tb_errorFileName.Text, AppDomain.CurrentDomain.BaseDirectory, CIMToDMS_files, ExtractStatisticsFromCSV());
                    warningImplementer.CreateWarningFile(tb_separator.Text, tb_warningFileName.Text, AppDomain.CurrentDomain.BaseDirectory, CIMToDMS_files, ExtractStatisticsFromCSV());

                    
                    System.Windows.Forms.MessageBox.Show("Files successfully saved on location: " + AppDomain.CurrentDomain.BaseDirectory, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    btn_loadCSV.IsEnabled = true;
                    btn_save.IsEnabled = false;
                    
                }
                catch (TypeInitializationException tie)
                {
                    Console.WriteLine(tie.InnerException);
                }
             }
           
            
        }

        private void btn_saveToDatabase_Click(object sender, RoutedEventArgs e)
        {
            numberOfClickOnDatabaseButton++;

            if (numberOfClickOnDatabaseButton == 1)
            {
                //First insert into database

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Filter = "database file (*.db)|*.db|All files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FileInfo fileName = new FileInfo(saveFileDialog.FileName);
                    oldNameDatabase = fileName;
                    SaveToDatabase(fileName, numberOfClickOnDatabaseButton);
                }
            }
            else
            {
                //Is not the first insert in the database

                SaveToDatabase(oldNameDatabase, numberOfClickOnDatabaseButton);
            }
        }

        private void btn_openChart_Click(object sender, RoutedEventArgs e)
        {
           statistics_list1 = new List<Statistics>();

            IList items = dataGridStatistics.SelectedItems;
            foreach (object item in items)
            {
                statistics_list1.Add(item as Statistics);
            }

            Chart chartWindow = new Chart(statistics_list1);
            chartWindow.Show();
        }

        private void dataGridStatistics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((Object)dataGridStatistics.SelectedItem != null)
            {
                Statistics statistic = (Statistics)dataGridStatistics.SelectedItem;

                CircuitNameChart.Title = "Circuit Name: " + statistic.Circuit;
                ((PieSeries)CircuitNameChart.Series[0]).ItemsSource =
                new KeyValuePair<string, int>[]{
                new KeyValuePair<string, int>("Error Count", statistic.ErrorCount),
                new KeyValuePair<string, int>("Warning Count", statistic.WarningCount),
                };
            }

            if (dataGridStatistics.SelectedItems.Count > 1)
            {
                btn_openChart.IsEnabled = true;
            }
            else
            {
                btn_openChart.IsEnabled = false;
            }
        }

        private void tb_separator_TextChanged(object sender, TextChangedEventArgs e)
        {


            if (tb_separator.Text == "")
            {
                lb_separatorValidate.Visibility = Visibility.Visible;
                lb_separatorValidate.Content = "Field must be filled.";
                lb_separatorValidate.BorderBrush = Brushes.Red;
                validateSeparatorTextBox = false;
            }
            else
            {
                if (tb_separator.Text.Length > 1)
                {
                    lb_separatorValidate.Visibility = Visibility.Visible;
                    lb_separatorValidate.Content = "Insert one character.";
                    lb_separatorValidate.BorderBrush = Brushes.Red;
                    validateSeparatorTextBox = false;
                }
                else
                {

                    if (tb_separator.Text == "," || tb_separator.Text == " " || tb_separator.Text == "/" || tb_separator.Text == "-" || tb_separator.Text == "." || Regex.IsMatch(tb_separator.Text, @"^[a-zA-Z0-9]*$") == true || tb_separator.Text == ":")
                    {
                        lb_separatorValidate.Visibility = Visibility.Visible;
                        lb_separatorValidate.Content = "Character is not allowed.";
                        lb_separatorValidate.BorderBrush = Brushes.Red;
                        validateSeparatorTextBox = false;
                    }
                    else
                    {
                        lb_separatorValidate.Visibility = Visibility.Hidden;
                        lb_separatorValidate.Content = null;
                        lb_separatorValidate.ToolTip = null;
                        validateSeparatorTextBox = true;
                    }
                }
            }

        }

        private void tb_summaryFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb_summaryFileName.Text == "")
            {
                lb_summaryTextBoxValidator.Visibility = Visibility.Visible;
                lb_summaryTextBoxValidator.Content = "Field must be filled.";
                lb_summaryTextBoxValidator.BorderBrush = Brushes.Red;
                validateSummaryTextBox = false;
            }
            else
            {
                lb_summaryTextBoxValidator.Visibility = Visibility.Hidden;
                lb_summaryTextBoxValidator.Content = null;
                lb_summaryTextBoxValidator.ToolTip = null;
                validateSummaryTextBox = true;
            }
        }

        private void tb_warningFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb_warningFileName.Text == "")
            {
                lb_warningTextBoxValidator.Visibility = Visibility.Visible;
                lb_warningTextBoxValidator.Content = "Field must be filled.";
                lb_warningTextBoxValidator.BorderBrush = Brushes.Red;
                validateWarningTextBox = false;
            }
            else
            {
                lb_warningTextBoxValidator.Visibility = Visibility.Hidden;
                lb_warningTextBoxValidator.Content = null;
                lb_warningTextBoxValidator.ToolTip = null;
                validateWarningTextBox = true;
            }
        }

        private void tb_errorFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb_errorFileName.Text == "")
            {
                lb_errorTextBoxValidator.Visibility = Visibility.Visible;
                lb_errorTextBoxValidator.Content = "Field must be filled.";
                lb_errorTextBoxValidator.BorderBrush = Brushes.Red;
                validateErrorTextBox = false;
            }
            else
            {
                lb_errorTextBoxValidator.Visibility = Visibility.Hidden;
                lb_errorTextBoxValidator.Content = null;
                lb_errorTextBoxValidator.ToolTip = null;
                validateErrorTextBox = true;
            }
        }

        private void tb_statisticFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb_statisticFileName.Text == "")
            {
                lb_statisticTextBoxValidator.Visibility = Visibility.Visible;
                lb_statisticTextBoxValidator.Content = "Field must be filled.";
                lb_statisticTextBoxValidator.BorderBrush = Brushes.Red;
                validateStatisticsTextBox = false;
            }
            else
            {
                lb_statisticTextBoxValidator.Visibility = Visibility.Hidden;
                lb_statisticTextBoxValidator.Content = null;
                lb_statisticTextBoxValidator.ToolTip = null;
                validateStatisticsTextBox = true;
            }
        }

        #endregion


    }
}