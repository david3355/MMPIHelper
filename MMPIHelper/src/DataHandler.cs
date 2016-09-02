using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace MMPIHelper
{
    public delegate void MessageHandler(string Message);

    public abstract class DataHandler
    {
        protected DataHandler(string fileName, MessageHandler MessageHandler)
        {
            messageHandler = MessageHandler;
            try
            {
                database = SQLiteHelper.GetSQLiteHelper();
                string basepath = System.AppDomain.CurrentDomain.BaseDirectory;
                string path = basepath + fileName;
                if (!File.Exists(path))
                {
                    database.NewFile(path);
                    CreateDatabaseStructure();
                    Init();
                }
                else database.OpenFile(path);
                //messageHandler("Database connection succesful");
            }
            catch (Exception e)
            {
                database = null;
                messageHandler("Database connection failed due to: " + e.Message);
            }
        }

        protected static SQLiteHelper database;
        protected static DataHandler self;
        protected static MessageHandler messageHandler;

        public abstract void CreateDatabaseStructure();

        public virtual void Init()
        {
            // Optional hook method
        }

    }

    public class MMPIDataHandler : DataHandler
    {
        protected MMPIDataHandler(string fileName, MessageHandler MessageHandler)
            : base(fileName, MessageHandler)
        {
        }

        public static MMPIDataHandler GetDataHandler(string FileName, MessageHandler MessageHandler)
        {
            if (self == null) self = new MMPIDataHandler(FileName, MessageHandler);
            return self as MMPIDataHandler;
        }
        public static MMPIDataHandler GetDataHandler()
        {
            return self as MMPIDataHandler;
        }

        public override void CreateDatabaseStructure()
        {
            String q = String.Empty;
            try
            {
                q = "CREATE TABLE mmpidata (id INTEGER, number INTEGER, PRIMARY KEY(id))";
                database.ExecuteNonQuery(q);
            }
            catch (Exception e)
            {
                messageHandler(String.Format("Error while executing command [{0}]. Error message: {1}", q, e.Message));
            }
        }

        public override void Init()
        {
            InitMMPIData();
        }

        public void InitMMPIData()
        {
            string q = String.Empty;
            try
            {
                for (int i = 1; i <= 566; i++)
                {
                    q = String.Format("INSERT INTO mmpidata (id, number) VALUES ('{0}', '0')", i);
                    database.ExecuteNonQuery(q);
                }
            }
            catch (Exception e)
            {
                messageHandler(String.Format("Error while executing command [{0}]. Error message: {1}", q, e.Message));
            }
        }

        public void SaveNewNumber(int Index, int Number)
        {
            string q = String.Format("UPDATE mmpidata SET number='{0}' WHERE id='{1}'", Number, Index);
            try
            {
                database.ExecuteNonQuery(q);
            }
            catch (Exception e)
            {
                messageHandler(String.Format("Error while executing command [{0}]. Error message: {1}", q, e.Message));
            }
        }

        public void EraseData()
        {
            string q = String.Format("UPDATE mmpidata SET number='0' WHERE 1=1");
            try
            {
                database.ExecuteNonQuery(q);
            }
            catch (Exception e)
            {
                messageHandler(String.Format("Error while executing command [{0}]. Error message: {1}", q, e.Message));
            }
        }

        public List<MMPIData> GetSavedData()
        {
            List<MMPIData> data = new List<MMPIData>();
            try
            {
                string q = "SELECT * FROM mmpidata";
                DataTable dt = database.ExecuteDataQuery(q);
                Data2List(dt, data);
                return data;
            }
            catch (Exception e)
            {
                messageHandler("Query failed due to: " + e.Message);
                return null;
            }

        }

        void Data2List(DataTable data, List<MMPIData> list)
        {
            foreach (DataRow row in data.Rows)
            {
                MMPIData mmpi = new MMPIData(Convert.ToInt32(row["id"]), Convert.ToInt32(row["number"]));
                list.Add(mmpi);
            }
        }

    }
}
