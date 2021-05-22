using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace dataBaseManager
{
    public static class DialogPrompt
    {
        public static string ShowStringDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text, Width = 300};
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 90, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { textBox.Text = textBox.Text.Trim();  prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
        public static string ShowCreateColumnDialog(string text, string caption)
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            TextBox name = new TextBox();
            TextBox length = new TextBox();
            ComboBox constraint = new ComboBox();
            ComboBox type = new ComboBox();
            string result = "";
            layout.Size = new System.Drawing.Size(840, 30);
            layout.RowCount = 1;
            layout.ColumnCount = 8;
            layout.Location = new System.Drawing.Point(0, 30);

            

            type.Items.Add("VARCHAR");
            type.Items.Add("TEXT");
            type.Items.Add("INT");
            type.Items.Add("FLOAT");
            type.Items.Add("DATE");

            type.TextChanged += (object sender, EventArgs args) => {
                if (type.SelectedItem != null && (type.SelectedItem.ToString() == "DATE" || type.SelectedItem.ToString() == "TEXT"))
                {
                    length.Text = "";
                    length.Enabled = false;
                }
                else length.Enabled = true;
            };

            constraint.Items.Add("NOT NULL");
            constraint.Items.Add("NOT NULL UNIQUE");
            constraint.Items.Add("UNIQUE");

            Form prompt = new Form()
            {
                Width = 840,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            layout.Controls.AddRange(new Control[] { 
                new Label() {Text = "Name:" },
                name, 
                new Label(){ Text = "Type: "}, 
                type, 
                new Label(){ Text = "Size: "},
                length, 
                new Label(){ Text = "Constraint"}, 
                constraint
            });
            foreach(Control control in layout.Controls)
            {
                control.Width = 90;
                if (control is Label) ((Label)control).TextAlign = ContentAlignment.MiddleCenter;
            }
            constraint.Width = 130;
            prompt.Controls.Add(new Label() { Text = text, Width = 600,  Location = new System.Drawing.Point(0, 0)});
            prompt.Controls.Add(layout);
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 90, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => {
                if (type.Text == "VARCHAR" && length.Text == "")
                {
                    MessageBox.Show("Empty size field!");
                    return;
                }
                else if (!length.Text.All(c => char.IsDigit(c)))
                {
                    MessageBox.Show("Size field should contain only digits!");
                    return;
                }
                result = length.Text == "" ? string.Format("{0} {1} {2};", name.Text, type.Text, constraint.Text) : string.Format("{0} {1}({2}) {3};", name.Text, type.Text, length.Text, constraint.Text);
                prompt.Close(); 
            };
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? result : "";
        }
        public static string ShowRemoveColumnDialog(string text, string caption, List<String> columnNames)
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            ComboBox columns = new ComboBox();
            string result = "";
            layout.Size = new System.Drawing.Size(400, 30);
            layout.RowCount = 1;
            layout.ColumnCount = 2;
            layout.Location = new System.Drawing.Point(0, 30);
            columns.Items.AddRange(columnNames.ToArray());
            layout.Controls.Add(new Label() { Text = "Select column to drop: ", TextAlign = ContentAlignment.MiddleCenter, Width = 150});
            layout.Controls.Add(columns);


            Form prompt = new Form()
            {
                Width = 500,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            prompt.Controls.Add(new Label() { Text = text, Width = 400, Location = new System.Drawing.Point(0, 0) });
            prompt.Controls.Add(layout);
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 90, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => {
                result = columns.Text;
                prompt.Close();
            };
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? result : "";
        }
        public static string ShowRenameColumnDialog(string text, string caption, List<String> columnNames)
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            ComboBox columns = new ComboBox();
            TextBox newName = new TextBox();
            string result = "";
            layout.Size = new System.Drawing.Size(600, 30);
            layout.RowCount = 1;
            layout.ColumnCount = 4;
            layout.Location = new System.Drawing.Point(0, 30);
            columns.Items.AddRange(columnNames.ToArray());
            layout.Controls.Add(new Label() { Text = "Select column to rename: ", TextAlign = ContentAlignment.MiddleCenter, Width = 150 });
            layout.Controls.Add(columns);
            layout.Controls.Add(new Label() { Text = "New column name: ", TextAlign = ContentAlignment.MiddleCenter, Width = 150 });
            layout.Controls.Add(newName);

            Form prompt = new Form()
            {
                Width = 600,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            prompt.Controls.Add(new Label() { Text = text, Width = 600, Location = new System.Drawing.Point(0, 0) });
            prompt.Controls.Add(layout);
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 90, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => {
                result = string.Format(".{0}', '{1}', 'COLUMN';", columns.Text, newName.Text);
                prompt.Close();
            };
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? result : "";
        }
        public static string ShowAddForeignKeyDialog(string text, string caption, string currentTable, sqlConnection _cnn)
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            ComboBox tables = new ComboBox();
            ComboBox columns = new ComboBox();
            string result = "";
            layout.Size = new System.Drawing.Size(600, 30);
            layout.RowCount = 1;
            layout.ColumnCount = 4;
            layout.Location = new System.Drawing.Point(0, 30);
            tables.Items.AddRange(_cnn.getTableNames().ToArray());
            tables.TextChanged += (object sender, EventArgs args) => {
                columns.Items.Clear();
                columns.Items.AddRange(_cnn.getColumnNames(tables.Text).ToArray());
            };
            layout.Controls.Add(new Label() { Text = "Add reference to: ", TextAlign = ContentAlignment.MiddleCenter, Width = 150 });
            layout.Controls.Add(tables);
            layout.Controls.Add(new Label() { Text = "by field: ", TextAlign = ContentAlignment.MiddleCenter, Width = 150 });
            layout.Controls.Add(columns);

            Form prompt = new Form()
            {
                Width = 600,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            prompt.Controls.Add(new Label() { Text = text, Width = 600, Location = new System.Drawing.Point(0, 0) });
            prompt.Controls.Add(layout);
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 90, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => {
                _cnn.sqlExecute("alter table \"" + currentTable + "\" add \"" + columns.Text + "\" INT ;" );
                result = string.Format(" add constraint FK_{0}_{2} foreign key ({0}) references {1}({0});", columns.Text, tables.Text, currentTable);
                prompt.Close();
            };
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;
            
            return prompt.ShowDialog() == DialogResult.OK ? result : "";
        }
        public static string ShowRemoveKeyDialog(string text, string caption, List<String> keyNames)
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            ComboBox keys = new ComboBox();
            string result = "";
            layout.Size = new System.Drawing.Size(400, 30);
            layout.RowCount = 1;
            layout.ColumnCount = 2;
            layout.Location = new System.Drawing.Point(0, 30);
            keys.Items.AddRange(keyNames.ToArray());
            keys.Width = 200;
            layout.Controls.Add(new Label() { Text = "Select key to drop: ", TextAlign = ContentAlignment.MiddleCenter, Width = 150 });
            layout.Controls.Add(keys);
            Form prompt = new Form()
            {
                Width = 500,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            prompt.Controls.Add(new Label() { Text = text, Width = 400, Location = new System.Drawing.Point(0, 0) });
            prompt.Controls.Add(layout);
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 90, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => {
                result = keys.Text;
                prompt.Close();
            };
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? result : "";
        }
        public static string ShowInsertIntoDialog(string text, string caption, List<String> columnNames)
        {
            DataGridView record = new DataGridView();
            string result = "";
            record.AllowUserToAddRows = false;
            record.Location = new Point(0, 30);
            record.Width = 800;
            record.Height = 60;
            columnNames.RemoveAt(0);
            foreach (string column in columnNames)
            {
                record.Columns.Add(column, column);
            }
            try
            {
                record.Rows.Add();
            }
            catch
            {
                MessageBox.Show("Table seems to have no columns.");
                return "";
            }
            Form prompt = new Form()
            {
                Width = 800,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            prompt.Controls.Add(new Label() { Text = text, Width = 400, Location = new System.Drawing.Point(0, 0) });
            prompt.Controls.Add(record);
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 90, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => {
                foreach(DataGridViewCell cell in record.Rows[0].Cells)
                {
                    if (cell.Value == null) result = result.Length == 0 ? "''" : result + ", ''";
                    else result = result.Length == 0 ? "'" + cell.Value.ToString() + "'" : result + ", '" + cell.Value.ToString() + "'";
                }
                prompt.Close();
            };
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? result : "";
        }
        public static string ShowDropRecordDialog(string text, string caption, string currentTable, sqlConnection _cnn)
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            ComboBox values = new ComboBox();
            ComboBox columns = new ComboBox();
            string result = "";
            layout.Size = new System.Drawing.Size(600, 30);
            layout.RowCount = 1;
            layout.ColumnCount = 4;
            layout.Location = new System.Drawing.Point(0, 30);
            columns.Items.AddRange(_cnn.getColumnNames(currentTable).ToArray());
            columns.TextChanged += (object sender, EventArgs args) => {
                values.Items.Clear();
                values.Items.AddRange(_cnn.getRows(currentTable, columns.Text).ToArray());
            };
            layout.Controls.Add(new Label() { Text = "Drop where: ", TextAlign = ContentAlignment.MiddleCenter, Width = 150 });
            layout.Controls.Add(columns);
            layout.Controls.Add(new Label() { Text = " is like: ", TextAlign = ContentAlignment.MiddleCenter, Width = 150 });
            layout.Controls.Add(values);

            Form prompt = new Form()
            {
                Width = 600,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            prompt.Controls.Add(new Label() { Text = text, Width = 600, Location = new System.Drawing.Point(0, 0) });
            prompt.Controls.Add(layout);
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 90, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => {
                result = string.Format("DELETE FROM {0} WHERE {1} LIKE '{2}';", currentTable, columns.Text, values.Text);
                prompt.Close();
            };
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? result : "";
        }
        public static string ShowUpdateDialog(string text, string caption, string currentTable, sqlConnection _cnn)
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            ComboBox values = new ComboBox();
            ComboBox columns = new ComboBox();
            ComboBox valueCondition = new ComboBox();
            ComboBox columnCondition = new ComboBox();
            string result = "";
            layout.Size = new System.Drawing.Size(600, 60);
            layout.RowCount = 2;
            layout.ColumnCount = 4;
            layout.Location = new System.Drawing.Point(0, 30);
            columns.Items.AddRange(_cnn.getColumnNames(currentTable).ToArray());
            columnCondition.Items.AddRange(_cnn.getColumnNames(currentTable).ToArray());
            columns.TextChanged += (object sender, EventArgs args) => {
                values.Items.Clear();
                values.Items.AddRange(_cnn.getRows(currentTable, columns.Text).ToArray());
            };
            columnCondition.TextChanged += (object sender, EventArgs args) => {
                valueCondition.Items.Clear();
                valueCondition.Items.AddRange(_cnn.getRows(currentTable, columnCondition.Text).ToArray());
            };
            layout.Controls.Add(new Label() { Text = "Set: ", TextAlign = ContentAlignment.MiddleCenter, Width = 150 });
            layout.Controls.Add(columns);
            layout.Controls.Add(new Label() { Text = " to: ", TextAlign = ContentAlignment.MiddleCenter, Width = 150 });
            layout.Controls.Add(values);
            layout.Controls.Add(new Label() { Text = "Where: ", TextAlign = ContentAlignment.MiddleCenter, Width = 150 });
            layout.Controls.Add(columnCondition);
            layout.Controls.Add(new Label() { Text = " is like: ", TextAlign = ContentAlignment.MiddleCenter, Width = 150 });
            layout.Controls.Add(valueCondition);

            Form prompt = new Form()
            {
                Width = 600,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            prompt.Controls.Add(new Label() { Text = text, Width = 600, Location = new System.Drawing.Point(0, 0) });
            prompt.Controls.Add(layout);
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 90, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => {
                result = string.Format("UPDATE {0} SET {1}='{2}' WHERE {3}='{4}';", currentTable, columns.Text, values.Text, columnCondition.Text, valueCondition.Text);
                prompt.Close();
            };
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? result : "";
        }
        public static string ShowJoinDialog(string text, string caption, sqlConnection _cnn)
        {
            List<string> columns = new List<string>();
            TableLayoutPanel layout = new TableLayoutPanel();
            TableLayoutPanel settingsLayout = new TableLayoutPanel();
            ComboBox table1 = new ComboBox();
            ComboBox table2 = new ComboBox();
            ComboBox join1 = new ComboBox();
            ComboBox join2 = new ComboBox();
            ComboBox condition = new ComboBox();
            ComboBox value = new ComboBox();
            CheckedListBox table1Cols = new CheckedListBox();
            CheckedListBox table2Cols = new CheckedListBox();
            string result = "";
            settingsLayout.Size = new Size(520, 100);
            settingsLayout.RowCount = 2;
            settingsLayout.ColumnCount = 4;
            settingsLayout.Location = new Point(0, 230);
            layout.Size = new System.Drawing.Size(520, 200);
            layout.RowCount = 4;
            layout.ColumnCount = 2;
            layout.Location = new System.Drawing.Point(130, 30);

            table1.Items.AddRange(_cnn.getTableNames().ToArray());
            table1.TextChanged += (sender, args) => {
                join1.Items.Clear();
                table1Cols.Items.Clear();
                table1Cols.Items.AddRange(_cnn.getColumnNames(table1.Text).ToArray());
            };
            table2.Items.AddRange(_cnn.getTableNames().ToArray());
            table2.TextChanged += (sender, args) => {
                join2.Items.Clear();
                table2Cols.Items.Clear();
                table2Cols.Items.AddRange(_cnn.getColumnNames(table2.Text).ToArray());
            };

            table1Cols.MouseLeave += (sender, args) => {
                join1.Items.Clear();
                condition.Items.Clear();
                foreach (string item in table1Cols.CheckedItems)
                {
                    join1.Items.Add(item);
                    condition.Items.Add(table1.Text.Trim() + "." + item);
                }
                foreach (string item in table2Cols.CheckedItems)
                {
                    condition.Items.Add(table2.Text.Trim() + "." + item);
                }
            };
            table2Cols.MouseLeave += (sender, args) => {
                join2.Items.Clear();
                condition.Items.Clear();
                foreach (string item in table2Cols.CheckedItems)
                {
                    join2.Items.Add(item);
                    condition.Items.Add(table2.Text.Trim() + "."+ item);
                }
                foreach (string item in table1Cols.CheckedItems)
                {
                    condition.Items.Add(table1.Text.Trim() + "." + item);
                }
            };
            condition.TextChanged += (sender, args) => {
                string fixedColumnName;
                string fixedTableName;
                try
                {
                    fixedColumnName = condition.Text.Substring(condition.Text.IndexOf('.') + 1, condition.Text.Length - condition.Text.IndexOf('.') - 1);
                    fixedTableName = condition.Text.Substring(0, condition.Text.IndexOf('.'));
                    value.Items.Clear();
                    value.Items.AddRange(_cnn.getRows(fixedTableName, fixedColumnName).ToArray());
                }
                catch
                {
                    MessageBox.Show("Invalid value in condition column name field.");
                }
            };


            layout.Controls.Add(new Label() { Text = "First table:", TextAlign = ContentAlignment.MiddleCenter});
            layout.Controls.Add(new Label() { Text = "Second table", TextAlign = ContentAlignment.MiddleCenter});
            layout.Controls.Add(table1);
            layout.Controls.Add(table2);
            layout.Controls.Add(table1Cols);
            layout.Controls.Add(table2Cols);
            settingsLayout.Controls.Add(new Label() { Text = "On: ", TextAlign = ContentAlignment.MiddleCenter });
            settingsLayout.Controls.Add(join1);
            settingsLayout.Controls.Add(new Label() { Text = " = ", TextAlign = ContentAlignment.MiddleCenter });
            settingsLayout.Controls.Add(join2);
            settingsLayout.Controls.Add(new Label() { Text = "Where", TextAlign = ContentAlignment.MiddleCenter });
            settingsLayout.Controls.Add(condition);
            settingsLayout.Controls.Add(new Label() { Text = " like ", TextAlign = ContentAlignment.MiddleCenter });
            settingsLayout.Controls.Add(value);


            Form prompt = new Form()
            {
                Width = 520,
                Height = 450,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            prompt.Controls.Add(new Label() { Text = text, Width = 600, Location = new System.Drawing.Point(0, 0) });
            prompt.Controls.Add(layout);
            prompt.Controls.Add(settingsLayout);
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 350, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => {
                foreach (string item in condition.Items)
                {

                    result = result == "" ? item.Trim() + " AS '" + item.Trim() + "'" : result + ", " + item.Trim() + " AS '" + item.Trim() + "'";
                }
                result = "select " + result + string.Format(" from {0} full outer join {1} on {0}.{2}={1}.{3}", table1.Text.Trim(), table2.Text.Trim(), join1.Text.Trim(), join2.Text.Trim());
                if (value.Text != "") result = result + " where " + condition.Text.Trim() + " like '" + value.Text.Trim() + "';";
                else result = result + ";";
                prompt.Close();
            };
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? result : "";
        }
    }
}

