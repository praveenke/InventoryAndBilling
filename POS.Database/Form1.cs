using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using POS.Database;
namespace POS.Database
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string LatestTranSQL = "Select ID from Transaction1 where InvoiceNumber=7";
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
            con.Open();
            OleDbCommand cmd = new OleDbCommand(LatestTranSQL, con);
            OleDbDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
                MessageBox.Show(Convert.ToInt64(dr[0]).ToString());
        }
    }
}