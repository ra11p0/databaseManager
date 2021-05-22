using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dataBaseManager
{
    public partial class Login : Form
    {
        TextBox server = new TextBox();
        TextBox login = new TextBox();
        TextBox password = new TextBox();
        Label serverLabel = new Label();
        Label loginLabel = new Label();
        Label passwordLabel = new Label();
        TableLayoutPanel layout = new TableLayoutPanel();
        Button submit = new Button();

        public Login()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Text = "Login";
            this.Size = new Size(200, 250);
            layout.Dock = DockStyle.Fill;
            serverLabel.Text = "Server name:";
            server.Text = "plenczewski.mssql.somee.com";
            loginLabel.Text = "User name:";
            login.Text = "sampleUser";
            passwordLabel.Text = "Password:";
            password.Text = "samplePassword";
            password.PasswordChar = '*';
            submit.Text = "Connect";
            submit.Click += new EventHandler(submitFunc);

            layout.Controls.Add(serverLabel);
            layout.Controls.Add(server);
            layout.Controls.Add(loginLabel);
            layout.Controls.Add(login);
            layout.Controls.Add(passwordLabel);
            layout.Controls.Add(password);
            layout.Controls.Add(submit);
            foreach(Control control in layout.Controls)
            {
                control.Dock = DockStyle.Fill;
            }
            this.Controls.Add(layout);
        }

        private void submitFunc(object sender, EventArgs args)
        {
            submit.Text = "Connecting...";
            submit.Enabled = false;
            this.Refresh();
            bool result;
            DatabaseSelect databaseSelector = new DatabaseSelect(server.Text, login.Text, password.Text, out result);
            databaseSelector.FormClosing += (object sender, FormClosingEventArgs args) => { this.Show(); };
            submit.Text = "Connect";
            submit.Enabled = true;
            if (result)
            {
                this.Hide();
            }
        }
    }
}
