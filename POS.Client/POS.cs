using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using POS.Database;
using System.Drawing.Printing;
using System.Collections;
namespace POS{
    public partial class POS : Form
    {
        AutoCompleteStringCollection scAutoComplete;
        //private System.ComponentModel.Container components = null;
        private System.Windows.Forms.PrintDialog printDialog1=null;
        private System.Drawing.Printing.PrintDocument printDocument1 = null;
        private System.Windows.Forms.PageSetupDialog pageSetupDialog1 = null;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1 = null;
        private int linesPrinted;
        private string[] lines;
        public POS()
        {
            InitializeComponent();
        }
        private void POS_Load(object sender, EventArgs e)
        {
            //  textBox1.Text = DateTime.Now.ToShortDateString();
            TxtSaleDate.Text = DateTime.Now.ToString();
            //dataGridView1.CellLeave += new DataGridViewCellEventHandler(dataGridView1_CellContentDoubleClick);
            TxtInvoiceNo.Text = (GetLatestInvoiceNumber() + 1).ToString();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            monthCalendar1.Visible = true;
            monthCalendar1.Focus();
        }
        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            //textBox1.Text = monthCalendar1.SelectionStart.ToShortDateString();
            TxtSaleDate.Text = monthCalendar1.SelectionStart.AddHours(12).ToString();
            monthCalendar1.Visible = false;
        }
        PickProduct frmPickProduct;
        private void Pick_Click(object sender, EventArgs e)
        {
            frmPickProduct = new PickProduct();
            frmPickProduct.FormClosed += new FormClosedEventHandler(AddLineItemToGrid);
            frmPickProduct.Show();
        }
        public void AddLineItemToGrid(object sender, FormClosedEventArgs e)
        {
            int productCount = 1;
            int rowIndex = 0;
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value.ToString().ToLower() == frmPickProduct.ProductCode.ToLower())
                {
                    productCount = Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value) + 1;
                    rowIndex = i;
                    break;
                }
            }
            if (productCount == 1)
                dataGridView1.Rows.Add(new string[] { frmPickProduct.ProductCode, frmPickProduct.ProductName, "1", frmPickProduct.Price.ToString(), "", frmPickProduct.Price.ToString() });
            else
            {
                dataGridView1.Rows[rowIndex].Cells[2].Value = productCount.ToString();
                dataGridView1.Rows[rowIndex].Cells[5].Value = (frmPickProduct.Price * productCount).ToString();
            }
        }
        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 2)
                dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[5].Value = (Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value) * Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[3].Value) - Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value) * Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[3].Value) * (Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[4].Value) / 100)).ToString();
            if (dataGridView1.CurrentCell.ColumnIndex == 0)
                FillLineItem(dataGridView1);
            if (dataGridView1.CurrentCell.ColumnIndex == 4)
                dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[5].Value = (Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value) * Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[3].Value) - Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value) * Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[3].Value) * (Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[4].Value) / 100)).ToString();
        }
        void FillLineItem(DataGridView dataGridView1)
        {
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
            OleDbCommand cmd = new OleDbCommand("SELECT Product.ProductCode, Product.ProductName, Product.ProductSellPrice FROM (Units INNER JOIN Product ON Units.ID = Product.UnitID) INNER JOIN Inventory ON Product.ID = Inventory.ProductID where ProductCode='" + dataGridView1.CurrentCell.Value + "' and Product.IsActive=Yes and Inventory.InStock>0", con);
            try
            {
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //scAutoComplete.Add(dr["ProductCode"].ToString());
                    dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value = dr["ProductName"].ToString();
                    dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value = "1";
                    dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[3].Value = dr["ProductSellPrice"].ToString();
                    dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[5].Value = dr["ProductSellPrice"].ToString();
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

                }
            }
        }
        void LoadAutoCompleteDataSource(string ProductCode)
        {
            scAutoComplete = new AutoCompleteStringCollection();
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
            try
            {
                string sql = "SELECT Product.ProductCode FROM (Units INNER JOIN Product ON Units.ID = Product.UnitID) INNER JOIN Inventory ON Product.ID = Inventory.ProductID where ProductCode like " + "\"ProdctCodePlaceHolder%\"" + " and Product.IsActive=Yes and Inventory.InStock>0";
                sql = sql.Replace("ProdctCodePlaceHolder", ProductCode);
                OleDbCommand cmd = new OleDbCommand(sql, con);

                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    scAutoComplete.Add(dr["ProductCode"].ToString());
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
                }
            }
        }
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //MessageBox.Show(scAutoComplete.Count.ToString());
            if (dataGridView1.CurrentCellAddress.X == 0)
            {
                TextBox txt = e.Control as TextBox;

                LoadAutoCompleteDataSource(txt.Text);
                if (scAutoComplete != null)
                {


                    txt.AutoCompleteCustomSource = scAutoComplete;
                    txt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    txt.AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
            }

            //return false;
            if (dataGridView1.CurrentCellAddress.X == 4)
            {
                TextBox txtbox = e.Control as TextBox;
                if (txtbox.Text.Trim() != "")
                {
                    txtbox.KeyPress += new KeyPressEventHandler(txtbox_KeyPress);
                }
            }
        }
        void txtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txtbox = sender as TextBox;
            //MessageBox.Show((Convert.ToSingle(txtbox.Text)<=100).ToString());
            if (Convert.ToSingle(txtbox.Text + e.KeyChar.ToString()) > 100)
                e.Handled = true;
            if ((!(char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar))) && (Convert.ToSingle(txtbox.Text) > 100))
                e.Handled = true;
        }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Double Total = 0;
            if (e.ColumnIndex == 5)
            {
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    Total = Total + Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value);
                }
                TxtTotal.Text = Total.ToString();
            }
        }
        private long GetLatestInvoiceNumber()
        {
            long LatestInvoiceNumber = 0;
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
            try
            {
                OleDbCommand cmd = new OleDbCommand("SELECT  Value1 from SetUp where Name='LatestInvoiceNumber' and IsActive=Yes", con);
                con.Open();
                LatestInvoiceNumber = Convert.ToInt32(cmd.ExecuteScalar());
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
                }
            }
            return LatestInvoiceNumber;
        }
        private void UpdateLatestInvoiceNumber(string LatestInvoiceNumber)
        {
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
            try
            {
                OleDbCommand cmd = new OleDbCommand("Update SetUp set Value1='"+LatestInvoiceNumber+"' where Name='LatestInvoiceNumber' and IsActive=Yes", con);
                con.Open();
                Int32 RecordsAffected = cmd.ExecuteNonQuery();
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
                }
            }
        }
        private void Save_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to save invoice with out printing?", "Confirm", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No)
                return;            
            long TransactionID=SaveInvoice();            
            dataGridView1.Rows.Clear();
            TxtDiscount.Text = "";
            TxtTotal.Text = "";
            TxtInvoiceNo.Text = (GetLatestInvoiceNumber() + 1).ToString();
        }
        private long SaveInvoice()
        {
            long InvoiceNumber = 0;
            float Discount = 0;
            double Total = 0;
            long TrasactionID = 0;
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
            try
            {
                con.Open();
                InvoiceNumber = TxtInvoiceNo.Text.Trim() == "" ? 0 : Convert.ToInt64(TxtInvoiceNo.Text);
                Discount = TxtDiscount.Text.Trim() == "" ? 0 : Convert.ToInt64(TxtDiscount.Text);
                Total = TxtTotal.Text.Trim() == "" ? 0 : Convert.ToInt64(TxtTotal.Text);

                string TranSQL = "Execute TransactionInsert";
                OleDbCommand cmd = new OleDbCommand(TranSQL, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@pInvoiceNumber", System.Data.OleDb.OleDbType.BigInt).Value = InvoiceNumber;
                cmd.Parameters.Add("@pSaleDate", System.Data.OleDb.OleDbType.Date).Value = Convert.ToDateTime(TxtSaleDate.Text);
                cmd.Parameters.Add("@pDiscount", System.Data.OleDb.OleDbType.Single).Value = Discount;
                cmd.Parameters.Add("@pTotal", System.Data.OleDb.OleDbType.Double).Value = Total;
                cmd.Parameters.Add("@pCreateDate", System.Data.OleDb.OleDbType.Date).Value = DateTime.Now;
                cmd.Parameters.Add("@pUpdateDate", System.Data.OleDb.OleDbType.Date).Value = DateTime.Now;
                cmd.Parameters.Add("@pCreatedBy", System.Data.OleDb.OleDbType.VarWChar).Value = "staff";

                Int32 RecordsAffected = cmd.ExecuteNonQuery();
                if (RecordsAffected > 0)
                {
                    string LatestTranSQL = "Select ID from Transaction1 where InvoiceNumber=" + InvoiceNumber;
                    cmd = new OleDbCommand(LatestTranSQL, con);
                    OleDbDataReader dr=cmd.ExecuteReader();
                    while(dr.Read())
                        TrasactionID = Convert.ToInt64(dr[0]);
                }

                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    cmd = new OleDbCommand("select ID from Product where ProductCode=\"" + dataGridView1.Rows[i].Cells[0].Value + "\"", con);
                    Int64 ProductID = Convert.ToInt64(cmd.ExecuteScalar());
                    cmd = new OleDbCommand("select ID from Inventory where ProductID=" + ProductID, con);
                    Int64 InventoryID = Convert.ToInt64(cmd.ExecuteScalar());

                    string TranDetailSQL = "Execute TransactionDetailInsert";
                    cmd = new OleDbCommand(TranDetailSQL, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@pTrasactionID", System.Data.OleDb.OleDbType.BigInt).Value = TrasactionID;
                    cmd.Parameters.Add("@pProductID", System.Data.OleDb.OleDbType.BigInt).Value = ProductID;
                    cmd.Parameters.Add("@pInventoryID", System.Data.OleDb.OleDbType.BigInt).Value = InventoryID;
                    cmd.Parameters.Add("@pProductCode", System.Data.OleDb.OleDbType.VarWChar).Value = dataGridView1.Rows[i].Cells[0].Value;
                    cmd.Parameters.Add("@pDescription", System.Data.OleDb.OleDbType.VarWChar).Value = dataGridView1.Rows[i].Cells[1].Value;
                    cmd.Parameters.Add("@pQty", System.Data.OleDb.OleDbType.Single).Value = Convert.ToSingle(dataGridView1.Rows[i].Cells[2].Value);
                    cmd.Parameters.Add("@pPrice", System.Data.OleDb.OleDbType.Double).Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value);
                    cmd.Parameters.Add("@pDiscount", System.Data.OleDb.OleDbType.Single).Value = Convert.ToSingle(dataGridView1.Rows[i].Cells[4].Value);
                    cmd.Parameters.Add("@pLineTotal", System.Data.OleDb.OleDbType.Double).Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value);
                    cmd.Parameters.Add("@pCreateDate", System.Data.OleDb.OleDbType.Date).Value = DateTime.Now;
                    cmd.Parameters.Add("@pCreatedBy", System.Data.OleDb.OleDbType.VarWChar).Value = "staff";
                    RecordsAffected = cmd.ExecuteNonQuery();
                    RecordsAffected = UpdateProductInventory(InventoryID, Convert.ToSingle(dataGridView1.Rows[i].Cells[2].Value));
                }
                UpdateLatestInvoiceNumber(InvoiceNumber.ToString());
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
            return InvoiceNumber;
        }
        private int UpdateProductInventory(long InventoryID, Single Qty)
        {
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
            string UpdateSQL = "Update Inventory set InStock=InStock-"+Qty+" Where ID="+InventoryID;            
            int RecordsAffected = 0;
            try
            {
                OleDbCommand cmd = new OleDbCommand(UpdateSQL, con);
                //cmd.Parameters.Add("@InventoryID", System.Data.OleDb.OleDbType.BigInt).Value = InventoryID;
                cmd.CommandType = CommandType.Text;
                con.Open();
                RecordsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(TranSQL);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return RecordsAffected;
        }
        private void TxtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            //MessageBox.Show(Convert.ToInt32(e.KeyChar).ToString());
            if ((!char.IsNumber(e.KeyChar) && (e.KeyChar != Convert.ToChar(".")) && (Convert.ToInt32(e.KeyChar) != 8) && (Convert.ToInt32(e.KeyChar) != 13)) || (Convert.ToSingle(TxtDiscount.Text + (char.IsNumber(e.KeyChar) == true ? e.KeyChar.ToString() : "")) > 100))
                e.Handled = true;

            //MessageBox.Show((Convert.ToSingle(TxtDiscount.Text) <= 100).ToString());
            //if ((!char.IsNumber(e.KeyChar)) || !(char.IsControl(e.KeyChar)) || (Convert.ToSingle(TxtDiscount.Text) <= 100))
            //    e.Handled = true;
        }
        private void TxtDiscount_LostFocus(object sender, EventArgs e)
        {
            dataGridView1_CellValueChanged(new object(), new DataGridViewCellEventArgs(5, 0));
            if ((TxtDiscount.Text != "") & (TxtTotal.Text.Trim() != ""))
                TxtTotal.Text = (Convert.ToDouble(TxtTotal.Text) - Convert.ToDouble(TxtTotal.Text) * Convert.ToSingle(TxtDiscount.Text) / 100).ToString();
        }
        private void Print_Click(object sender, EventArgs e)
        {
            //richTextBox2.Text = "                               " +
            //    DateTime.Now.Month + "/" + DateTime.Now.Day + "/" +
            //    DateTime.Now.Year + "\r\n\r\n";
            //richTextBox2.AppendText("This is a greatly simplified Print " +
            //    "Document Method\r\n\r\n");
            //richTextBox2.AppendText("We can write text to a richTextBox, " +
            //    "or use Append Text," + "\r\n" + "or Concatenate a String, and " +
            //    "write that textBox. The " + "\r\n" + "richTextBox does not " +
            //    "even have to be visible. " + "\r\n\r\n" + "Because we use a " +
            //    "richTextBox it's physical dimensions are " + "\r\n" +
            //    "irrelevant. We can place it anywhere on our form, and set the " +
            //    "\r\n" + "Visible Property to false.\r\n\r\n");
            //richTextBox2.AppendText("This is the document we will print. The " +
            //    "rich TextBox serves " + "\r\n" + "as a Cache for our Report, " +
            //    "or any other text we wish to print.\r\n\r\n");
            //richTextBox2.AppendText("I have also included Print Setup " +
            //    "and Print Preview. ");
            long InvoiceNumber = SaveInvoice();
            //richTextBox2.Text = PreparePrintText(InvoiceNumber);
            richTextBox2.Text = PrepareTextForPrint(true, InvoiceNumber);
            dataGridView1.Rows.Clear();
            TxtDiscount.Text = "";
            TxtTotal.Text = "";
            TxtInvoiceNo.Text = (GetLatestInvoiceNumber() + 1).ToString();

            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }
        // OnBeginPrint 
        string PreparedText = string.Empty;
        private void OnBeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            char[] param = { '\n' };

            if (printDialog1.PrinterSettings.PrintRange == PrintRange.Selection)
            {
                lines = PreparedText.Split(param);
            }
            else
            {
                lines = PreparedText.Split(param);
            }

            int i = 0;
            char[] trimParam = { '\r' };
            foreach (string s in lines)
            {
                lines[i++] = s.TrimEnd(trimParam);
            }
        }
        // OnPrintPage
        private void OnPrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int x = e.MarginBounds.Left;
            int y = e.MarginBounds.Top;
            Brush brush = new SolidBrush(richTextBox2.ForeColor);

            while (linesPrinted < lines.Length)
            {
                //linesPrinted++;
                e.Graphics.DrawString(lines[linesPrinted++],
                    new Font("Lucida Console",10), brush, x, y);
                y += 15;
                if (y >= e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }
            }

            linesPrinted = 0;
            e.HasMorePages = false;
        }
        private void PrintPreview_Click(object sender, EventArgs e)
        {
            // Call Dialog Box
            //richTextBox2.Text = "                               " +
            //    DateTime.Now.Month + "/" + DateTime.Now.Day + "/" +
            //    DateTime.Now.Year + "\r\n\r\n";
            //richTextBox2.AppendText("This is a greatly simplified Print " +
            //    "Document Method\r\n\r\n");
            //richTextBox2.AppendText("We can write text to a richTextBox, " +
            //    "or use Append Text," + "\r\n" + "or Concatenate a String, and " +
            //    "write that textBox. The " + "\r\n" + "richTextBox does not " +
            //    "even have to be visible. " + "\r\n\r\n" + "Because we use a " +
            //    "richTextBox it's physical dimensions are " + "\r\n" +
            //    "irrelevant. We can place it anywhere on our form, and set the " +
            //    "\r\n" + "Visible Property to false.\r\n\r\n");
            //richTextBox2.AppendText("This is the document we will print. The " +
            //    "rich TextBox serves " + "\r\n" + "as a Cache for our Report, " +
            //    "or any other text we wish to print.\r\n\r\n");
            //richTextBox2.AppendText("I have also included Print Setup " +
            //    "and Print Preview. ");
            //MessageBox.Show(richTextBox2.Text);
            PreparedText = PrepareTextForPrint(false, Convert.ToInt64(TxtInvoiceNo.Text));
            printPreviewDialog1.ShowDialog();
        }
        private string PreparePrintText(long InvoiceNumber)
        {            
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
            string PrintText = string.Empty;
            try
            {
                con.Open();                
                string InvoiceSQL = "SELECT * FROM Transaction1 INNER JOIN TransactionDetail ON Transaction1.ID=TransactionDetail.TransactionID" +
                                 "WHERE Transaction1.Invoicenumber=" + InvoiceNumber;
                OleDbCommand cmd = new OleDbCommand(InvoiceSQL, con);
                cmd.CommandType = CommandType.Text;
                OleDbDataReader DR=cmd.ExecuteReader();
                bool IsHeader = true;
                string Footer = string.Empty;
                while(DR.Read())
                {
                    if(IsHeader==true)
                    {
                        PrintText = PrintText + "Bill No.:  " +DR["InvoiceNumber"].ToString();
                        PrintText = PrintText + "Date:  " + DR["SaleDate"].ToString();
                        Footer = Footer + "           Discount:  " + DR["Transaction1.Discount"].ToString();
                        Footer = Footer + "           Total:  " + DR["Transaction1.Total"].ToString();
                        IsHeader = false;
                    }
                    PrintText = PrintText + "\n" + DR["ProductCode"].ToString();
                    PrintText = PrintText + "  "+  DR["Description"].ToString();
                    PrintText = PrintText + "  " + DR["Price"].ToString();
                    PrintText = PrintText + "  " + DR["TransactionDetail.Discount"].ToString();
                    PrintText = PrintText + "  " + DR["TransactionDetail.LineTotal"].ToString();                    
                }
                PrintText = PrintText + Footer;                
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
            return PrintText;
        }
        private string PreparePreviewText()
        {            
            string PrintText = string.Empty;
            string Header = string.Empty;
            string Footer = string.Empty;                
            Header = Header + "Bill No.:  " + TxtInvoiceNo.Text + new String(Convert.ToChar(" "),25);
            Header = Header + "Date:  " + TxtSaleDate.Text;
            try
            {
                PrintText = PrintText + "\nProduct Code";
                PrintText = PrintText + new string(Convert.ToChar(" "),7) +"Description";
                PrintText = PrintText + new string(Convert.ToChar(" "),7) +"Qty";
                PrintText = PrintText + new string(Convert.ToChar(" "), 7) + "Price";
                PrintText = PrintText + new string(Convert.ToChar(" "), 7) + "Discount";
                PrintText = PrintText + new string(Convert.ToChar(" "), 7) +  "Line Total";

                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    //PrintText = "";
                    PrintText = PrintText + "\n" + dataGridView1.Rows[i].Cells[0].Value.ToString().ToUpper()+ new String(Convert.ToChar(" "),19-dataGridView1.Rows[i].Cells[0].Value.ToString().Length);
                    PrintText = PrintText + dataGridView1.Rows[i].Cells[1].Value.ToString() + new String(Convert.ToChar(" "),18-dataGridView1.Rows[i].Cells[1].Value.ToString().Length);
                    PrintText = PrintText + new String(Convert.ToChar(" "), 10 - dataGridView1.Rows[i].Cells[2].Value.ToString().Length) + dataGridView1.Rows[i].Cells[2].Value.ToString() ;
                    PrintText = PrintText + new String(Convert.ToChar(" "), 12 - dataGridView1.Rows[i].Cells[3].Value.ToString().Length) + dataGridView1.Rows[i].Cells[3].Value.ToString();
                    if(dataGridView1.Rows[i].Cells[4].Value == null)
                        PrintText = PrintText + new String(Convert.ToChar(" "), 15);
                    else
                        PrintText = PrintText +new String(Convert.ToChar(" "), 15 - dataGridView1.Rows[i].Cells[4].Value.ToString().Length)+ dataGridView1.Rows[i].Cells[4].Value.ToString();
                    PrintText = PrintText + new String(Convert.ToChar(" "), 10 - dataGridView1.Rows[i].Cells[5].Value.ToString().Length) + dataGridView1.Rows[i].Cells[5].Value.ToString() ;
                }
                Footer = Footer + "\n\n"+new String(Convert.ToChar(" "),69)+"Discount:  " + TxtDiscount.Text;
                Footer = Footer + "\n"+new String(Convert.ToChar(" "),69)+"   Total:  " + TxtTotal.Text;
                PrintText = Header + PrintText + Footer;
            }
            catch (Exception ex)
            {

            }
            return PrintText;
        }
        private string PrepareTextForPrint(bool FromDB,long InvoiceNumber)
        {
            string PrintText = string.Empty;
            string Header = string.Empty;
            string Footer = string.Empty;
            string SQL = string.Empty;

            DataSet DS = new DataSet();

            try
            {
                if (FromDB == true)
                {
                    OleDbConnection con = new OleDbConnection();
                    con.ConnectionString = DatabaseConnectionSettings.GetMDBConnectionString();
                    SQL = "SELECT InvoiceNumber,SaleDate,Product.ProductCode,Description,QTY,Price,TransactionDetail.Discount,LineTotal,Transaction1.discount,Total " +
                                "FROM (Transaction1 INNER JOIN TransactionDetail ON Transaction1.ID=TransactionDetail.TransactionID) INNER JOIN PRODUCT ON Product.ID=TransactionDetail.Productid " +
                                "WHERE Transaction1.InvoiceNumber=" + InvoiceNumber;
                    OleDbDataAdapter cmd = new OleDbDataAdapter(SQL, con);
                    cmd.Fill(DS);
                }
                else
                {
                    for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                    {
                        DataTable DT = new DataTable();
                        DS.Tables.Add(DT);
                        
                        DataColumn DC;
                        for (int j = 0; j < 9; j++)
                        {
                            DC = new DataColumn();
                            DS.Tables[0].Columns.Add(DC);
                        }
                        DataRow DR = DS.Tables[0].NewRow();
                        DR[2] = dataGridView1.Rows[i].Cells[0].Value.ToString();
                        DR[3] = dataGridView1.Rows[i].Cells[1].Value.ToString();
                        DR[4] = dataGridView1.Rows[i].Cells[2].Value.ToString();
                        DR[5] = dataGridView1.Rows[i].Cells[3].Value.ToString();
                        DR[6] = dataGridView1.Rows[i].Cells[4].Value == null ? "" : dataGridView1.Rows[i].Cells[4].Value.ToString();
                        DR[7] = dataGridView1.Rows[i].Cells[5].Value.ToString();
                        DS.Tables[0].Rows.Add(DR);
                    }
                }

                if (FromDB == true)
                {
                    Header = Header + "Bill No.:  " + DS.Tables[0].Rows[0][0].ToString() + new String(Convert.ToChar(" "), 25);
                    Header = Header + "Date:  " + DS.Tables[0].Rows[0][1].ToString();
                }
                else
                {
                    Header = Header + "Bill No.:  " + TxtInvoiceNo.Text + new String(Convert.ToChar(" "), 25);
                    Header = Header + "Date:  " + TxtSaleDate.Text;
                }

                PrintText = PrintText + "\nProduct Code";
                PrintText = PrintText + new string(Convert.ToChar(" "), 7) + "Description";
                PrintText = PrintText + new string(Convert.ToChar(" "), 7) + "Qty";
                PrintText = PrintText + new string(Convert.ToChar(" "), 7) + "Price";
                PrintText = PrintText + new string(Convert.ToChar(" "), 7) + "Discount";
                PrintText = PrintText + new string(Convert.ToChar(" "), 7) + "Line Total";

                for (int i = 0; i < DS.Tables[0].Rows.Count; i++)
                {

                    PrintText = PrintText + "\n" + DS.Tables[0].Rows[i][2].ToString().ToUpper() + new String(Convert.ToChar(" "), 19 - DS.Tables[0].Rows[i][2].ToString().Length);
                    PrintText = PrintText + DS.Tables[0].Rows[i][3].ToString() + new String(Convert.ToChar(" "), 18 - DS.Tables[0].Rows[i][3].ToString().Length);
                    PrintText = PrintText + new String(Convert.ToChar(" "), 10 - DS.Tables[0].Rows[i][4].ToString().Length) + DS.Tables[0].Rows[i][4].ToString();
                    PrintText = PrintText + new String(Convert.ToChar(" "), 12 - DS.Tables[0].Rows[i][5].ToString().Length) + DS.Tables[0].Rows[i][5].ToString();
                    if (DS.Tables[0].Rows[i][6] == null)
                        PrintText = PrintText + new String(Convert.ToChar(" "), 15);
                    else
                        PrintText = PrintText + new String(Convert.ToChar(" "), 15 - DS.Tables[0].Rows[i][6].ToString().Length) + DS.Tables[0].Rows[i][6].ToString();
                    PrintText = PrintText + new String(Convert.ToChar(" "), 10 - DS.Tables[0].Rows[i][7].ToString().Length) + DS.Tables[0].Rows[i][7].ToString();
                }
                if (FromDB == true)
                {
                    Footer = Footer + "\n\n" + new String(Convert.ToChar(" "), 69) + "Discount:  " + DS.Tables[0].Rows[0][8].ToString();
                    Footer = Footer + "\n" + new String(Convert.ToChar(" "), 69) + "   Total:  " + DS.Tables[0].Rows[0][9].ToString();
                }
                else
                {
                    Footer = Footer + "\n\n" + new String(Convert.ToChar(" "), 69) + "Discount:  " + TxtDiscount.Text;
                    Footer = Footer + "\n" + new String(Convert.ToChar(" "), 69) + "   Total:  " + TxtTotal.Text;
                }
                PrintText = Header + PrintText + Footer;
            }
            catch (Exception ex)
            {
                MessageBox.Show("");
            }
            finally
            {
                DS = null;
            }
            return PrintText;
        }        
    }
}