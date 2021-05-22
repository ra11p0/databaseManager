using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace dataBaseManager
{
    public class sqlConnection
    {
        public string serverName { get; }
        public string databaseName { get; }
        public string userName { get; }
        private string _password;
        private SqlConnection _cnn;
        public sqlConnection(string serverName, string databaseName, string userName, string password)
        {
            this.serverName = serverName;
            this.databaseName = databaseName;
            this.userName = userName;
            _password = password;
        }
        public sqlConnection(SqlConnection cnn)
        {
            _cnn = cnn;
        }
        public bool establishConnection()
        {
            string connetionString = String.Format(@"Data Source={0}; Initial Catalog={1}; User ID={2}; Password={3}", serverName, databaseName, userName, _password);
            _cnn = new SqlConnection(connetionString);
            try
            {
                _cnn.Open();
            }
            catch (SqlException err)
            {
                MessageBox.Show("Error: " + err.Message);
                return false;
            }
            _cnn.Close();
            return true;
        }
        public List<String> getTableNames()
        {
            _cnn.Open();
            List<string> tableNames = new List<string>();
            foreach (DataRow row in _cnn.GetSchema("Tables").Rows) tableNames.Add((string)row[2]);
            _cnn.Close();
            return tableNames;
        }
        public DataSet getTable(string tableName)
        {
            _cnn.Open();
            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM \"" + tableName + "\"", _cnn);
            DataSet dataset = new DataSet();
            dataAdapter.Fill(dataset);
            _cnn.Close();
            return dataset;
        }
        public DataSet getQuery(string query)
        {
            _cnn.Open();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(query, _cnn);
            DataSet dataset = new DataSet();
            dataAdapter.Fill(dataset);
            _cnn.Close();
            return dataset;
        }
        public void sqlExecute(string command)
        {
            _cnn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand(command, _cnn);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException err)
            {
                MessageBox.Show("Error: " + err.Message + "\n in: " + command);
            }
            finally
            {
                _cnn.Close();
            }
        }
        public List<String>getColumnNames(String tableName)
        {
            List<string> columnNames = new List<string>();
            _cnn.Open();
            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('" + tableName + "') ", _cnn);
            DataSet dataset = new DataSet();
            dataAdapter.Fill(dataset);
            _cnn.Close();
            foreach(DataRow temp in dataset.Tables[0].Rows)
            {
                columnNames.Add(temp[0].ToString());
            }
            return columnNames;
        }
        public List<String> getKeys(String tableName)
        {
            List<string> columnNames = new List<string>();
            _cnn.Open();
            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID ('" + tableName + "') AND type = 'PK' OR type = 'F';", _cnn);
            DataSet dataset = new DataSet();
            dataAdapter.Fill(dataset);
            _cnn.Close();
            foreach (DataRow temp in dataset.Tables[0].Rows)
            {
                columnNames.Add(temp[0].ToString());
            }
            return columnNames;
        }
        public List<String> getRows(String tableName, String columnName)
        {
            List<string> values = new List<string>();
            _cnn.Open();
            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT "+columnName+" FROM "+ tableName +";", _cnn);
            DataSet dataset = new DataSet();
            dataAdapter.Fill(dataset);
            _cnn.Close();
            foreach (DataRow temp in dataset.Tables[0].Rows)
            {
                values.Add(temp[0].ToString());
            }
            return values;
        }
    }
}
