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
    public partial class NewProduct : Form
    {
        OleDbConnection con=null ;
        
        
        
        public NewProduct()
        {
            InitializeComponent();
        }

        private void InventoryNewItem_Load(object sender, EventArgs e)
        {
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
            OleDbDataAdapter adp = new OleDbDataAdapter("Select ID, UnitName from Units", con);
            DataSet ds = new DataSet();
            int recs = adp.Fill(ds);
            //listBox1.Items.Insert(0, "---select---");
           // listBox1.Items.Add("---select----");
            listBox1.DataSource = ds.Tables[0].DefaultView;
            DataRow dr = ds.Tables[0].NewRow();
            dr[0] = "0";
            dr[1] = "---select----";
            ds.Tables[0].Rows.InsertAt(dr,0);
            //dr[new DataColumn()];
            //ds.Tables[0].Rows.InsertAt();
            listBox1.DisplayMember = "UnitName";
            listBox1.ValueMember = "ID";
            listBox1.SelectedIndex=0;
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            bool frmstatus = false;
            
            try
            {
                con=new OleDbConnection();
                
                con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
                con.Open();
                frmstatus = true;
                //check to make ProductCode unique
                OleDbCommand cmd = new OleDbCommand("select count(ProductCode) from Product where ProductCode='" + textBox1.Text + "'", con);
                int DuplicateCount = Convert.ToInt32(cmd.ExecuteScalar());
                int recCount = 0;
                int ProductID = 0;
                string strIsActive=string.Empty;
                if (IsActive.CheckState == CheckState.Checked)
                    strIsActive="Yes";
                else
                    strIsActive="No";

                if (DuplicateCount == 0)
                {
                    cmd = new OleDbCommand("Insert into Product(ProductCode,ProductName,ProductBuyPrice,ProductSellPrice,UnitID,IsActive)" +
                        "values('" + textBox1.Text + "','" + textBox2.Text + "'," + textBox3.Text + "," + textBox4.Text + ",'" + listBox1.SelectedValue + "','" + strIsActive + "')", con);
                    recCount = cmd.ExecuteNonQuery();

                    //if number of records inserted is one and if there is a stock in unit entered insert into inventory table
                    if ((recCount == 1) && !(String.IsNullOrEmpty(textBox4.Text)))
                    {
                        cmd = new OleDbCommand("select ID from Product where ProductCode='" + textBox1.Text + "'", con);
                        ProductID = Convert.ToInt32(cmd.ExecuteScalar());
                        cmd = new OleDbCommand("Insert into Inventory(ProductID,InStock)" +
                            "values(" + ProductID + "," + textBox5.Text + ")", con);
                        recCount = cmd.ExecuteNonQuery();
                    }
                    con.Close();
                    //ClearFields();
                    
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
                if (con.State==ConnectionState.Open)
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