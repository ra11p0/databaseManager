using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace dataBaseManager
{
    public partial class DatabaseSelect : Form
    {
        SqlConnection _cnn;
        Button submit = new Button();
        Button newDatabase = new Button();
        Button dropDatabase = new Button();
        ListBox databases = new ListBox();
        string _serverName, _userName, _password;
        public DatabaseSelect(string serverName, string userName, string password, out bool res)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Text = serverName;
            _serverName = serverName;
            _userName = userName;
            _password = password;
            this.Size = new Size(200, 500);
            string connetionString = String.Format(@"Data Source={0}; User ID={1}; Password={2}", serverName, userName, password);
            _cnn = new SqlConnection(connetionString);
            try
            {
                _cnn.Open();
            }
            catch (SqlException err)
            {
                MessageBox.Show("Error: " + err.Message);
                res = false;
                return;
            }
            this.Show();
            using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", _cnn))
            {
                using (IDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        databases.Items.Add(dr[0].ToString());
                    }
                }
            }
            newDatabase.Text = "New database";
            newDatabase.Location = new Point(0, 300);
            newDatabase.Height = 50;
            newDatabase.Width = 183;
            newDatabase.Click += (sender, args) => {
                string name = DialogPrompt.ShowStringDialog("Name of new database: ", "New database");
                if (name == "") return;
                _cnn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("create database " + name, _cnn);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException err)
                {
                    MessageBox.Show("Error: " + err.Message);
                }
                databases.Items.Clear();
                using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", _cnn))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            databases.Items.Add(dr[0].ToString());
                        }
                    }
                }
                _cnn.Close();
            };
            dropDatabase.Text = "Drop database";
            dropDatabase.Location = new Point(0, 350);
            dropDatabase.Height = 50;
            dropDatabase.Width = 183;
            dropDatabase.Click += (sender, args) => {
                string name = DialogPrompt.ShowStringDialog("Name of database to drop: ", "Drop database");
                if (name == "") return;
                _cnn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("drop database " + name, _cnn);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException err)
                {
                    MessageBox.Show("Error: " + err.Message);
                }
                databases.Items.Clear();
                using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", _cnn))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            databases.Items.Add(dr[0].ToString());
                        }
                    }
                }
                _cnn.Close();
            };
            databases.SelectedItem = "plenczewski";
            databases.Height = 300;
            databases.Width = 180;
            databases.Dock = DockStyle.Top;
            submit.Height = 50;
            submit.Text = "Connect";
            submit.Dock = DockStyle.Bottom;
            submit.Click += new EventHandler(openDbManager);
            this.Controls.Add(databases);
            this.Controls.Add(newDatabase);
            this.Controls.Add(dropDatabase);
            this.Controls.Add(submit);
            _cnn.Close();
            res = true;
        }

        private void openDbManager(object sender, EventArgs e)
        {
            if (databases.SelectedItem == null) return;
            submit.Text = "Connecting...";
            submit.Enabled = false;
            this.Refresh();
            sqlConnection conection = new sqlConnection(_serverName, databases.SelectedItem.ToString(), _userName, _password);
            if (conection.establishConnection())
            {
                this.Hide();
                dbManager manager = new dbManager(conection);
                manager.Show();
                manager.FormClosing += (object sender, FormClosingEventArgs args) => { this.Show(); };
            }
            submit.Text = "Connect";
            submit.Enabled = true;
        }
    }
}
