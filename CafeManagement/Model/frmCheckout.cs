using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using iTextSharp;
using System.Threading.Tasks;
using System.Windows.Forms;
using CafeManagement.View;

namespace CafeManagement.Model
{
    public partial class frmCheckout : Form
    {
        public frmCheckout()
        {
            InitializeComponent();
        }

        public double amt;
        public int MainID = 0;
        private void txtReceived_TextChanged(object sender, EventArgs e)
        {
            double amt = 0;
            double receipt = 0;
            double change = 0;

            double.TryParse(txtBillAmount.Text, out amt);
            double.TryParse(txtReceived.Text, out receipt);

            change = Math.Abs(amt - receipt);

            txtChange.Text = change.ToString("F2");
        }
        
        private void btn_save_Click(object sender, EventArgs e)
        {
            string statusCheckQuery = "select status from tblMain where MainID = @id";
            string orderStatus = string.Empty;

            using (SqlConnection con = new SqlConnection(MainClass.con_string))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(statusCheckQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", MainID);
                    orderStatus = cmd.ExecuteScalar()?.ToString();
                }
            }

            if (orderStatus != "Complete")
            {
                MessageBox.Show("Your order is not completed yet.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string qry = @"update tblMain 
                   set total = @total, received = @rec, change = @change, status = 'Paid' 
                   where MainID = @id";

            Hashtable ht = new Hashtable
            {
                { "@id", MainID },
                { "@total", double.TryParse(txtBillAmount.Text, out double total) ? total : 0 },
                { "@rec", double.TryParse(txtReceived.Text, out double rec) ? rec : 0 },
                { "@change", double.TryParse(txtChange.Text, out double chg) ? chg : 0 }
            };



            if (MainClass.SQL(qry, ht) > 0)
            {
             
                ExportToExcel();
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to pay the order.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ExportToExcel()
        {
            frmPOS frm = (frmPOS)Application.OpenForms["frmPOS"];
            if (frm == null) return;

            string orderType = frm.OrderType;
            string fileName = $"Order_{MainID}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage excel = new ExcelPackage())
            {
                var ws = excel.Workbook.Worksheets.Add("OrderDetails");


                ws.Cells["A1"].Value = "Chi tiết hóa đơn";
                ws.Cells["A1"].Style.Font.Size = 14;
                ws.Cells["A1"].Style.Font.Bold = true;


                ws.Cells["A2"].Value = "Date & Time:";
                ws.Cells["B2"].Value = DateTime.Now.ToString("yyyy-MM-dd");
                ws.Cells["C2"].Value = DateTime.Now.ToShortTimeString();
                ws.Cells["A4"].Value = orderType == "Din In" ? "Table Name:" : "Customer Name:";
                ws.Cells["B4"].Value = orderType == "Din In" ? frm.lblTable.Text : frm.customerName;
                ws.Cells["A5"].Value = "Status:";
                ws.Cells["B5"].Value = "Paid";


                ws.Cells["A5"].Value = "Status";
                ws.Cells["B5"].Value = "Paid";


                ws.Cells["A7"].Value = "Product Name";
                ws.Cells["B7"].Value = "Quantity";
                ws.Cells["C7"].Value = "Price";
                ws.Cells["D7"].Value = "Total";
                ws.Cells["A7:D7"].Style.Font.Bold = true;
                ws.Cells["A7:D7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                int row = 8; 
                foreach (DataGridViewRow item in frm.dataGridView1.Rows)
                {
                    if (item.Cells["dgvproID"].Value != null)
                    {
                        ws.Cells[row, 1].Value = item.Cells["dgvName"].Value;
                        ws.Cells[row, 2].Value = item.Cells["dgvQty"].Value;
                        ws.Cells[row, 3].Value = item.Cells["dgvPrice"].Value;
                        ws.Cells[row, 4].Value = item.Cells["dgvAmount"].Value;
                        row++;
                    }
                }

                ws.Cells[row, 3].Value = "Total:";
                ws.Cells[row, 4].Value = frm.lblTotal.Text;
                ws.Cells[row, 3].Style.Font.Bold = true;
                ws.Cells[row, 4].Style.Font.Bold = true;
                ws.Cells.AutoFitColumns();

                string folderPath = @"D:\Hóa đơn";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, fileName);
                FileInfo excelFile = new FileInfo(filePath);
                excel.SaveAs(excelFile);

                MessageBox.Show("Payment successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                frmBillView frm1 = new frmBillView(MainID);
                frm1.ShowDialog();
            }
        }



        private void frmCheckout_Load(object sender, EventArgs e)
        {
            txtBillAmount.Text = amt.ToString();
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
