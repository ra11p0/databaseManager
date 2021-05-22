using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace dataBaseManager
{
    public partial class dbManager : Form
    {
        TabControl tables = new TabControl();
        MenuStrip menu = new MenuStrip();
        ToolStripMenuItem database, addTable, removeTable, renameTable;
        ToolStripMenuItem table, addColumn, removeColumn, renameColumn, addForeignKey, dropKey;
        ToolStripMenuItem records, insertRecord, editRecord, dropRecord;
        ToolStripMenuItem executeQuery, joinTables;
        sqlConnection _connection;
        public dbManager(sqlConnection connection)
        {
            InitializeComponent();
            this.Text = connection.serverName + "/" + connection.databaseName;
            _connection = connection;

            database = new ToolStripMenuItem();
            database.Text = "Database";

            addTable = new ToolStripMenuItem();
            addTable.Text = "Add table";
            addTable.Click += (sender, args) => {
                string newTableName = DialogPrompt.ShowStringDialog("New table name:", "New table");
                if (newTableName.Length == 0) return;
                _connection.sqlExecute("create table \"" + newTableName + " \"(" + newTableName + "ID INT NOT NULL IDENTITY(1,1), PRIMARY KEY (" + newTableName + "ID));");
                refresh(tables.SelectedIndex + 1);
            };

            removeTable = new ToolStripMenuItem();
            removeTable.Click += (object sender, EventArgs args) => { 
                _connection.sqlExecute("drop table \"" + tables.SelectedTab.Text + "\";");
                refresh(-1); 
            };
            removeTable.Text = "Drop current table";
            renameTable = new ToolStripMenuItem();
            renameTable.Text = "Rename current table";
            renameTable.Click += (sender, args) => {
                string newTableName = DialogPrompt.ShowStringDialog("New table name:", "Rename table");
                if (newTableName.Length == 0) return;
                _connection.sqlExecute("EXEC sp_rename '" + tables.SelectedTab.Text + "', '" + newTableName + "';");
                refresh(tables.SelectedIndex);
            };

            database.DropDownItems.AddRange(new ToolStripItem[] { addTable, removeTable, renameTable});

            table = new ToolStripMenuItem();
            table.Text = "Table";

            addColumn = new ToolStripMenuItem();
            addColumn.Text = "Add column";
            addColumn.Click += (sender, args) => {
                string query = DialogPrompt.ShowCreateColumnDialog("Specify new column:", "New column");
                if (query.Length == 0) return;
                _connection.sqlExecute("alter table \"" + tables.SelectedTab.Text + "\" add " + query);
                refresh(tables.SelectedIndex);
            };

            removeColumn = new ToolStripMenuItem();
            removeColumn.Text = "Drop column";
            removeColumn.Click += (sender, args) => { string column = DialogPrompt.ShowRemoveColumnDialog("Drop column", "Drop column", _connection.getColumnNames(tables.SelectedTab.Text));
                if (column.Length == 0) return;
                _connection.sqlExecute("alter table \"" + tables.SelectedTab.Text + "\" drop column " + column);
                refresh(tables.SelectedIndex);
            };

            renameColumn = new ToolStripMenuItem();
            renameColumn.Text = "Rename column";
            renameColumn.Click += (object sender, EventArgs args) => {
                string query = DialogPrompt.ShowRenameColumnDialog("Rename column", "Rename column", _connection.getColumnNames(tables.SelectedTab.Text));
                if (query.Length == 0) return;
                _connection.sqlExecute("sp_rename '" + tables.SelectedTab.Text + query);
                refresh(tables.SelectedIndex);
            };

            addForeignKey = new ToolStripMenuItem();
            addForeignKey.Text = "Add foreign key";
            addForeignKey.Click += (sender, args) => {
                string query = DialogPrompt.ShowAddForeignKeyDialog("Add foreign key", "Add foreign key", tables.SelectedTab.Text, _connection);
                if (query.Length == 0) return;
                _connection.sqlExecute("alter table " + tables.SelectedTab.Text + query);
                refresh(tables.SelectedIndex);
            };

            dropKey = new ToolStripMenuItem();
            dropKey.Text = "Drop key";
            dropKey.Click += (sender, args) => {
                string constraint = DialogPrompt.ShowRemoveKeyDialog("Drop key", "Drop key", _connection.getKeys(tables.SelectedTab.Text));
                if (constraint.Length == 0) return;
                _connection.sqlExecute("alter table \"" + tables.SelectedTab.Text + "\" drop constraint " + constraint);
                refresh(tables.SelectedIndex);
            };

            table.DropDownItems.AddRange(new ToolStripItem[] { addColumn, removeColumn, renameColumn, addForeignKey, dropKey });

            records = new ToolStripMenuItem();
            records.Text = "Records";

            insertRecord = new ToolStripMenuItem();
            insertRecord.Text = "Insert into";
            insertRecord.Click += (sender, args) => {
                string query = DialogPrompt.ShowInsertIntoDialog("Insert into " + tables.SelectedTab.Text, "Insert into " + tables.SelectedTab.Text, _connection.getColumnNames(tables.SelectedTab.Text));
                if (query == "") return;
                _connection.sqlExecute("insert into " + tables.SelectedTab.Text + " values (" + query + ");");
                refresh(tables.SelectedIndex);
            };

            dropRecord = new ToolStripMenuItem();
            dropRecord.Text = "Drop records";
            dropRecord.Click += (sender, args) => {
                string query = DialogPrompt.ShowDropRecordDialog("Drop records", "Drop records", tables.SelectedTab.Text, _connection);
                if (query == "") return;
                _connection.sqlExecute(query);
                refresh(tables.SelectedIndex);            
            };

            editRecord = new ToolStripMenuItem();
            editRecord.Text = "Update record";
            editRecord.Click += (sender, args) => {
                string query = DialogPrompt.ShowUpdateDialog("Update records", "Update records", tables.SelectedTab.Text, _connection);
                if (query == "") return;
                _connection.sqlExecute(query);
                refresh(tables.SelectedIndex);
            };

            records.DropDownItems.AddRange(new ToolStripItem[] { insertRecord, editRecord, dropRecord });

            executeQuery = new ToolStripMenuItem();
            executeQuery.Text = "Execute query";
            executeQuery.Click += (sender, args) => {
                string query = DialogPrompt.ShowStringDialog("Execute query", "Execute query:");
                if (query == "") return;
                _connection.sqlExecute(query);
                refresh(0);
            };

            joinTables = new ToolStripMenuItem();
            joinTables.Text = "Join tables";
            joinTables.Click += (sender, args) => {
                string query = DialogPrompt.ShowJoinDialog("Join tables", "Join tables", _connection);
                if (query == "") return;
                Form viewer = new Form();
                DataGridView tempGrid = new DataGridView();
                tempGrid.Dock = DockStyle.Fill;
                tempGrid.ReadOnly = true;
                tempGrid.DataSource = _connection.getQuery(query).Tables[0];
                viewer.Size = new Size(800, 600);
                viewer.Controls.Add(tempGrid);
                viewer.Show();
            };

            menu.Items.Add(database);
            menu.Items.Add(table);
            menu.Items.Add(records);
            menu.Items.Add(joinTables);
            menu.Items.Add(executeQuery);
            tables.Dock = DockStyle.Fill;
            this.Controls.Add(tables);
            this.Controls.Add(menu);
            refresh(-1);
        }

        void refresh(int tabIndex)
        {
            this.SuspendLayout();
            tables.Controls.Clear();
            foreach (string tableName in _connection.getTableNames())
            {
                TabPage tempTabPage = new TabPage();
                DataGridView tempDataGridView = new DataGridView();
                tempTabPage.Text = tableName;
                tempTabPage.Name = tableName;
                tempDataGridView.Dock = DockStyle.Fill;
                tempDataGridView.DataSource = _connection.getTable(tableName).Tables[0];
                tempDataGridView.ReadOnly = true;
                tempTabPage.Controls.Add(tempDataGridView);
                tables.Controls.Add(tempTabPage);
            }
            if (!(tabIndex < 0)) tables.SelectTab(tabIndex);
            if (tables.Controls.Count == 0)
            {
                table.Enabled = false;
                renameTable.Enabled = false;
                removeTable.Enabled = false;
                records.Enabled = false;
            }
            else
            {
                table.Enabled = true;
                renameTable.Enabled = true;
                removeTable.Enabled = true;
                records.Enabled = true;
            }
            this.ResumeLayout();
        }
    }
}
