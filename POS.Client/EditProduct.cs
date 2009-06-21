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
    public partial class EditProduct : Form
    {
        OleDbConnection con = null;
        string SelectedProduct = string.Empty;
        public EditProduct()
        {
            InitializeComponent();
        }
        private void InventoryEditItem_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(Inventory.SelectedProduct);
            SelectedProduct = Inventory.SelectedProduct;
            Inventory.SelectedProduct = string.Empty;
            LoadProductDetails();

        }
        private void LoadProductDetails()
        {
            con = new OleDbConnection();
            con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
            
            OleDbCommand cmd = new OleDbCommand("SELECT Product.ProductCode, Product.ProductName, Product.ProductBuyPrice, Product.ProductSellPrice, Units.UnitName ,Inventory.InStock, Product.IsActive FROM (Units INNER JOIN Product ON Units.ID = Product.UnitID) INNER JOIN Inventory ON Product.ID = Inventory.ProductID where ProductCode='" + SelectedProduct + "'", con);
            try
            {
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    textBox1.Text = dr["ProductCode"].ToString();
                    textBox2.Text = dr["ProductName"].ToString();
                    textBox3.Text = dr["ProductBuyPrice"].ToString();
                    textBox4.Text = dr["ProductSellPrice"].ToString();
                    LoadUnits(dr["UnitName"].ToString());
                    textBox5.Text = dr["InStock"].ToString();
                    if (dr["IsActive"].ToString().ToUpper()=="TRUE")
                        IsActive.CheckState = CheckState.Checked;

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);                
            }
            finally
            {
                if (con.State==ConnectionState.Open)
                {
                    con.Close();                
                   
                }                
            }

        }
        private void LoadUnits(string UnitName)
        {
            con = new OleDbConnection();
            con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
            OleDbDataAdapter adp = new OleDbDataAdapter("Select ID, UnitName from Units", con);
            try
            {
                DataSet ds = new DataSet();
                int recs = adp.Fill(ds);
                //listBox1.Items.Insert(0, "---select---");
                // listBox1.Items.Add("---select----");
                listBox1.DataSource = ds.Tables[0].DefaultView;
                DataRow dr = ds.Tables[0].NewRow();
                dr[0] = "0";
                dr[1] = "---select----";
                ds.Tables[0].Rows.InsertAt(dr, 0);
                //dr[new DataColumn()];
                //ds.Tables[0].Rows.InsertAt();
                listBox1.DisplayMember = "UnitName";
                listBox1.ValueMember = "ID";
                listBox1.SelectedIndex = listBox1.FindStringExact(UnitName);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);                
            }
            finally
            {
                if (con.State==ConnectionState.Open)
                {
                    con.Close();                
                   
                }                
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            bool frmstatus = false;

            try
            {
                con = new OleDbConnection();

                con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
                con.Open();
                frmstatus = true;
                //check to make ProductCode unique
                OleDbCommand cmd = new OleDbCommand("select count(ProductCode) from Product where ProductCode='" + textBox1.Text + "' and  ProductCode<>'" + SelectedProduct + "'", con);
                int DuplicateCount = Convert.ToInt32(cmd.ExecuteScalar());
                int recCount = 0;
                int ProductID = 0;
                string strIsActive = string.Empty;
                if (IsActive.CheckState == CheckState.Checked)
                    strIsActive = "TRUE";
                else
                    strIsActive = "FALSE";
                if (DuplicateCount == 0)
                {
                    cmd = new OleDbCommand("Update Product set ProductCode='" + textBox1.Text + "',ProductName='" + textBox2.Text + "',ProductBuyPrice=" + textBox3.Text + ",ProductSellPrice=" + textBox4.Text + ",UnitID='" + listBox1.SelectedValue + "', IsActive=" + strIsActive + " where ProductCode='" + SelectedProduct + "'", con);
                    recCount = cmd.ExecuteNonQuery();
                    //if number of records inserted is one and if there is a stock in unit entered insert into inventory table
                    if ((recCount == 1) && !(String.IsNullOrEmpty(textBox4.Text)))
                    {
                        cmd = new OleDbCommand("select ID from Product where ProductCode='" + textBox1.Text + "'", con);
                        ProductID = Convert.ToInt32(cmd.ExecuteScalar());
                        cmd = new OleDbCommand("Update Inventory set InStock=" + textBox5.Text + " where ProductID=" + ProductID, con);
                        recCount = cmd.ExecuteNonQuery();
                    }
                    con.Close();                

                }
                else
                {
                    MessageBox.Show("This Product Code " + textBox1.Text + "already exists please try a new Product Code");
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);                
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    frmstatus = false;
                }
                if (frmstatus == true)
                {
                    this.Close();
                }                
            }
        }
        void ClearFields()
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            textBox5.Text = string.Empty;
            listBox1.SelectedIndex = 0;
        }
    }
}