using ChoETL;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace TestService
{
    public partial class Service1 : ServiceBase
    {
        System.Threading.Timer threadTimer;
        private static string conn  = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        GhostlyLiveEntities db = new GhostlyLiveEntities();
        //private static CSVProcess db = new CSVProcess();
      
        public Service1()
        {
            InitializeComponent();
        }
        //public void OnDebug()
        //{
        //    OnStart(null);
        //}

        protected override void OnStart(string[] args)
        {

            SelectRecord();
            //LoadCsvDataIntoSqlServer();
            //TimeSpan tsInterval = new TimeSpan(0, 2, 0); //2 minute interval.
            //threadTimer = new System.Threading.Timer(new TimerCallback(threadTimer_Elapsed)
            //    , null, tsInterval, tsInterval);


            //timer = new System.Timers.Timer();
            //timer.Interval = 10000;
            //timer.Elapsed += Timer_Elapsed;
            //timer.Enabled = true;
            //timer.Start();
        }

        protected override void OnStop()
        {
            threadTimer.Change(Timeout.Infinite, Timeout.Infinite);
            threadTimer.Dispose();
            threadTimer = null;
            //timer.Stop();
        }

        private void SelectRecord()
        {
            Guid Id = new Guid();
            string csv_file_path = "";
            string csvName = "";
            Guid ? OperatorLocationId = new Guid();
            Guid? OperatorId = new Guid();
            var statusTodo = db.ImportStatus.Where(x => x.status == "To do").FirstOrDefault();
            var statusDone = db.ImportStatus.Where(x => x.status == "Done").FirstOrDefault();
            var statusError = db.ImportStatus.Where(x => x.status == "Error").FirstOrDefault();
            
            var data = (from t1 in db.ImportProcesses
                        join t2 in db.ImportFileTypes on t1.FileTypeId equals t2.Id
                        select new { t2.FileName, t1.FilePath, t1.OperatorId, t1.OperatotLocationId, t1.FileStatusId, t1.ProcessId, t1.ErrorDescription }).
                        Where(x => x.FileStatusId == statusTodo.Id  && x.FilePath !=null).ToList();
            foreach (var item in data)
            {
                try
                {
                    csv_file_path = item.FilePath;
                    csvName = item.FileName;
                    Id = item.ProcessId;
                    OperatorLocationId = item.OperatotLocationId;
                    OperatorId = item.OperatorId;


                    if (!string.IsNullOrEmpty(csv_file_path))
                    {
                        var errorDescription = db.ImportProcesses.Where(x => x.ProcessId == Id).FirstOrDefault();
                        DataTable csvData = GetDataTabletFromCSVFile(csv_file_path, OperatorLocationId, OperatorId);
                        InsertDataIntoSQLServerUsingSQLBulkCopy(csvData, csvName);
                        errorDescription.ErrorDescription = "Done";
                        errorDescription.FileStatusId = statusDone.Id;
                        errorDescription.DateProcessed = DateTime.Now.Date;
                        db.Entry(errorDescription).State = EntityState.Modified;                      

                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    var errorDescription = db.ImportProcesses.Where(x => x.ProcessId == Id).FirstOrDefault();
                    errorDescription.ErrorDescription = ex.Message;                  
                    errorDescription.FileStatusId = statusError.Id;
                    errorDescription.DateProcessed = DateTime.Now.Date;
                    db.Entry(errorDescription).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        private static DataTable GetDataTabletFromCSVFile(string csv_file_path,Guid ? OperatorLocationId, Guid? OperatorId)
        {
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    var tempListCol = colFields.ToList();
                    tempListCol.Add("OperatorLocationId");
                    tempListCol.Add("OperatorId");
                    colFields = tempListCol.ToArray();                 
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;                        
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        var tempListField = fieldData.ToList();
                        tempListField.Add(OperatorLocationId.ToString());
                        tempListField.Add(OperatorId.ToString());
                        fieldData = tempListField.ToArray();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i].StartsWith("$"))
                            {
                                fieldData[i] = fieldData[i].Replace("$", "");
                            }
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            return csvData;
        }

        static void InsertDataIntoSQLServerUsingSQLBulkCopy(DataTable csvFileData,string csvName)
        {            
                using (SqlConnection dbConnection = new SqlConnection(conn))
                {
                    dbConnection.Open();

                    var tablename = csvName;

                    using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                    {
                        s.DestinationTableName = tablename;
                        foreach (var column in csvFileData.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());
                        s.WriteToServer(csvFileData);
                    }
                }  
        }
    }
}
