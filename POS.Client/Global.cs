using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using POS.Database;
namespace POS
{
    public partial class Global : Form
    {
        public Global()
        {
            InitializeComponent();
        }

        private void Global_Load(object sender, EventArgs e)
        {
            BindGrid();
        }
        void BindGrid()
        {
            string connectionString = DatabaseConnectionSettings.GetMDBConnectionString();
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = connectionString;
            DataSet ds = new DataSet();
            try
            {
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT Name,Value1,IsActive FROM Setup Order by Name", con);
                int recs = adp.Fill(ds);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = ds.Tables[0].DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)


                    con.Close();

                ds = null;
            }
        }
        private void SaveGlobal()
        {
            OleDbConnection con = new OleDbConnection();
            OleDbCommand cmd = new OleDbCommand();
            con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
            try
            {
                int RecordsAffected;
                con.Open();
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    string SQL=string.Empty;
                    cmd=new OleDbCommand("select count(*) from SetUp where Name='" + dataGridView1.Rows[i].Cells[0].Value + "'", con);
                    
                    int DuplicateCount = Convert.ToInt32(cmd.ExecuteScalar());
                    if (DuplicateCount > 0)
                        SQL = "Update Setup set Value1='" + dataGridView1.Rows[i].Cells[1].Value + "', UpdateDate='" + DateTime.Now + "',IsActive=" + dataGridView1.Rows[i].Cells[2].Value + " Where Name='" + dataGridView1.Rows[i].Cells[0].Value + "'";
                    else
                        SQL = "Insert into Setup(Name,Value1,CreateDate,UpdateDate,CreatedBy,IsActive) values('" + dataGridView1.Rows[i].Cells[0].Value + "','" + dataGridView1.Rows[i].Cells[1].Value + "','" + DateTime.Now + "','" + DateTime.Now + "','Staff'," + dataGridView1.Rows[i].Cells[2].Value + ")";
                    cmd = new OleDbCommand(SQL, con);
                    cmd.CommandType = CommandType.Text;
                    RecordsAffected = cmd.ExecuteNonQuery();                    
                }                
            }
            catch (Exception ex)
            {
                //            MessageBox.Show(TranSQL);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveGlobal();
        }
    }
}