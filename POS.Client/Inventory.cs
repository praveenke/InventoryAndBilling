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
    public partial class Inventory : Form
    {
        public static string SelectedProduct;
        public Inventory()
        {
            InitializeComponent();                   
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            NewProduct frmProduct = new NewProduct();
            frmProduct.FormClosed += new FormClosedEventHandler(frmProduct_FormClosed);
            
            frmProduct.Show();            
        }
        void frmProduct_FormClosed(object sender, FormClosedEventArgs e)
        {            
            BindGrid();
        }

        private void Inventory_Load(object sender, EventArgs e)
        {
            SetColumnStyles();
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
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT Product.ProductCode, Product.ProductName, Product.ProductSellPrice, Units.UnitName ,Inventory.InStock,Product.IsActive FROM (Units INNER JOIN Product ON Units.ID = Product.UnitID) INNER JOIN Inventory ON Product.ID = Inventory.ProductID", con);
                int recs = adp.Fill(ds);
                dataGrid1.DataSource = null;                
                dataGrid1.DataSource = ds.Tables[0].DefaultView; 
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

        private void dataGrid1_DoubleClick(object sender, System.EventArgs ne)
        {
//            MessageBox.Show(dataGrid1.CurrentCell.RowNumber.ToString());
            //Make the id column hidden and get that value to open edit popup
            //MessageBox.Show(dataGrid1[dataGrid1.CurrentCell].ToString());
            SelectedProduct = dataGrid1[dataGrid1.CurrentCell].ToString();
            EditProduct frmProduct = new EditProduct();
            frmProduct.FormClosed += new FormClosedEventHandler(frmProduct_FormClosed);
            frmProduct.Show();            

        }
        void SetColumnStyles()
        {
            DataGridTableStyle tblStyle = new DataGridTableStyle();
            tblStyle.MappingName = "Inventory";
            tblStyle.GridColumnStyles.Clear();
            DataGridTextBoxColumn Column = new DataGridTextBoxColumn();
            Column.Width = 20;
            tblStyle.GridColumnStyles.Add(Column);
            Column = new DataGridTextBoxColumn();
            Column.Width = 200;
            tblStyle.GridColumnStyles.Add(Column);
            Column = new DataGridTextBoxColumn();
            Column.Width = 20;
            tblStyle.GridColumnStyles.Add(Column);
            Column = new DataGridTextBoxColumn();
            Column.Width = 20;
            tblStyle.GridColumnStyles.Add(Column);
            Column = new DataGridTextBoxColumn();
            Column.Width = 20;
            tblStyle.GridColumnStyles.Add(Column);
            dataGrid1.TableStyles.Add(tblStyle);
        }

        private void dataGrid1_Navigate(object sender, NavigateEventArgs ne)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            SelectedProduct = dataGrid1[dataGrid1.CurrentCell].ToString();
            EditProduct frmProduct = new EditProduct();
            frmProduct.FormClosed += new FormClosedEventHandler(frmProduct_FormClosed);
            frmProduct.Show();   
        }
    }
}