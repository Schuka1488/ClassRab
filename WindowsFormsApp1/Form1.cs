using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        DataTable Curtable; 
        string id_selected_rows = "0"; 
        private static BindingSource bSource = new BindingSource(); 
        DBclass db = new DBclass(); 
        class DBclass 
        {
            MySqlConnection connection = new MySqlConnection("server=10.90.12.110;port=33333;username=st_3_20_21;password=15733563;database=is_3_20_st21_KURS");
            public void openConnection() 
            {
                if (connection.State == ConnectionState.Closed) 
                    connection.Open(); 
            }
            public void closeConnection() 
            {
                if (connection.State == ConnectionState.Open) 
                    connection.Close(); 
            }
            public MySqlConnection getConnection() 
            {
                return connection; 
            }
        }
        public Form1()
        {
             InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string FormatFix = FormatINDate.GGWP(dateTimePicker1.Value.ToString());
                db.openConnection();
                MySqlCommand cmd = new MySqlCommand($"INSERT INTO Pyaterochka(Name, Sell, Date) VALUES( '{textBox2.Text}', '{textBox3.Text}', \"{FormatFix}\")", db.getConnection());
                MessageBox.Show(cmd.ExecuteNonQuery() > 0 ? "Данные добавились" : "Данные  не добавились", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                string commandStr = "SELECT IDtovar, Name, Sell, Date FROM Pyaterochka";
                Curtable = FormatINDate.FormatTableMethod(commandStr, dataGridView1);
                db.closeConnection();
                RedandYellow();
            }
            catch
            {
                db.closeConnection(); // отключаем подключение к БД
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                db.openConnection();
                MySqlCommand cmd = new MySqlCommand($"DELETE FROM Pyaterochka WHERE IDtovar = {id_selected_rows}", db.getConnection());
                MessageBox.Show(cmd.ExecuteNonQuery() > 0 ? "Данные удалены" : "Данные  не удалены", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                db.closeConnection();
                RedandYellow();
            }
            catch
            {
                db.closeConnection(); // отключаем подключение к БД
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string commandStr = "SELECT IDtovar, Name, Sell, Date FROM Pyaterochka";
            Curtable = FormatINDate.FormatTableMethod(commandStr, dataGridView1);
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            RedandYellow();
        }
        public void GetSelectedIDString()
        {
            string index_selected_rows;
            index_selected_rows = dataGridView1.SelectedCells[0].RowIndex.ToString();
            id_selected_rows = dataGridView1.Rows[Convert.ToInt32(index_selected_rows)].Cells[0].Value.ToString();
        }
        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!e.RowIndex.Equals(-1) && !e.ColumnIndex.Equals(-1) && e.Button.Equals(MouseButtons.Right))
            {
                dataGridView1.CurrentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
                dataGridView1.CurrentCell.Selected = true;
                GetSelectedIDString();
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView1.CurrentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
            dataGridView1.CurrentRow.Selected = true;
            GetSelectedIDString();
        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.Text.Length < 0) return;
            Curtable.DefaultView.RowFilter = $"Name LIKE '%{textBox5.Text}%'";
            RedandYellow();
        }//
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (textBox6.Text.Length < 0) return;
            Curtable.DefaultView.RowFilter = $"Sell LIKE '%{textBox6.Text}%'";
            RedandYellow();
        }
        private void RedandYellow()
        {
            DateTime editDataTable = DateTime.Now.AddDays(1);
            foreach(DataGridViewRow r in dataGridView1.Rows)
            {
                DateTime date = Convert.ToDateTime(r.Cells[3].Value);
                if (date < DateTime.Now.AddDays(-1))
                {
                    r.DefaultCellStyle.BackColor = Color.Red;
                }
                else if (date > DateTime.Now.AddDays(-1) && date <= editDataTable)
                {
                    r.DefaultCellStyle.BackColor = Color.Yellow;
                }
            }
        }
         class FormatINDate
         {
            static DBclass DB = new DBclass();
            public static DataTable FormatTableMethod(string commandStr, DataGridView grid)
            {
                MySqlCommand cmd = new MySqlCommand(commandStr, DB.getConnection());
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                DataTable table = new DataTable();
                adapter.SelectCommand = cmd;
                adapter.Fill(table);
                bSource.DataSource = table;
                grid.DataSource = bSource;
                return table;
            }
            public static string GGWP(string cordate)
            {
                DateTime _date = DateTime.Parse(cordate);
                return $"{_date.Year}.{_date.Month}.{_date.Day}";
            }
         }
    }
}
