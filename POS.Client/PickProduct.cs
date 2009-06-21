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
    public partial class PickProduct : Form
    {
        public PickProduct()
        {
            InitializeComponent();
        }
        string _productCode = string.Empty;
        string _productName = string.Empty;
        double _price = 0;
        public string ProductCode
        {
            get
            {
                return _productCode;
            }
            set
            {
                _productCode = value;
            }
        }
        public string ProductName
        {
            get
            {
                return _productName;
            }
            set
            {
                _productName = value;
            }
        }
        public double Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
            }
        }
        private void PickProduct_Load(object sender, EventArgs e)
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
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT Product.ProductCode, Product.ProductName, Product.ProductSellPrice, Units.UnitName ,Inventory.InStock FROM (Units INNER JOIN Product ON Units.ID = Product.UnitID) INNER JOIN Inventory ON Product.ID = Inventory.ProductID", con);
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
        private void dataGrid1_DoubleClick(object sender, EventArgs ne)
        {            
            ProductCode = dataGrid1[dataGrid1.CurrentRowIndex,0].ToString();
            ProductName = dataGrid1[dataGrid1.CurrentRowIndex, 1].ToString();
            Price = Convert.ToDouble(dataGrid1[dataGrid1.CurrentRowIndex, 2]);
            this.Close();
        }
    }
}