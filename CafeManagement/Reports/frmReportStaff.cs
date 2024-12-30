using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeManagement.Reports
{
    public partial class frmReportStaff : Form
    {
        private static readonly string connectionString = @"Data Source=DUCCKY\SQLEXPRESS;Initial Catalog=projectLTCSDL;Integrated Security=True;MultipleActiveResultSets=True;";
        public frmReportStaff()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_report_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            LoadStaffReport();
        }
        public void LoadStaffReport()
        {
            dataGridView1.Rows.Clear(); 
            string query = @"SELECT staffID, sName, sPhone, sRole, 
                                    CASE WHEN sRole = 'Waiter' THEN (SELECT COUNT(*) FROM tblMain WHERE WaiterName = sName) ELSE 0 END AS SL 
                                    FROM staff 
                                    WHERE sRole != 'Fired'
                                    ORDER BY CASE sRole 
                                        WHEN 'Manager' THEN 1 
                                        WHEN 'Waiter' THEN 2 
                                        WHEN 'Cashier' THEN 3 
                                        WHEN 'Driver' THEN 4 
                                        ELSE 5 END";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int sno = 0;

                        while (reader.Read())
                        {
                            sno++;
                            string staffID = reader["staffID"].ToString();
                            string name = reader["sName"].ToString();
                            string phone = reader["sPhone"].ToString();
                            string role = reader["sRole"].ToString();
                            int orderCount = 0;

                            if (role == "Waiter")
                            {
                                Image img = Properties.Resources.detail;

                                string countQuery = "select count(*) from tblMain where WaiterName = @WaiterName and status = 'Paid'";
                                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                                {
                                    countCmd.Parameters.AddWithValue("@WaiterName", name);
                                    orderCount = (int)countCmd.ExecuteScalar();
                                }
                                dataGridView1.Rows.Add(sno, name, phone, role, orderCount, img);

                            }
                            else
                            {
                                dataGridView1.Rows.Add(sno, name, phone, role);
                            }
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns["Details"].Index)
            {
                
                string role = dataGridView1.Rows[e.RowIndex].Cells["Role"].Value.ToString();
                if (role == "Waiter")
                {
                    string waiterName = dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString();
                    dataGridView2.Visible = true;
                    LoadOrderDetails(waiterName); 
                }
                else
                {
                    MessageBox.Show("Only Waiters have order details.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void LoadOrderDetails(string waiterName)
        {
            string query = @"
                            SELECT m.TableName, p.pName AS ProductName, d.price, d.qty, d.amount AS Total
                            FROM tblMain m
                            INNER JOIN tblDetails d ON m.MainID = d.MainID
                            INNER JOIN products p ON d.proID = p.pID
                            WHERE m.WaiterName = @WaiterName and m.status = 'Paid'";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@WaiterName", waiterName);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            dataGridView2.Rows.Clear();

                            while (reader.Read())
                            {
                                string tableName = reader["TableName"].ToString();
                                string productName = reader["ProductName"].ToString();
                                decimal price = Convert.ToDecimal(reader["price"]);
                                int qty = Convert.ToInt32(reader["qty"]);
                                decimal total = Convert.ToDecimal(reader["Total"]);

                                dataGridView2.Rows.Add(tableName, productName, price, qty, total);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmReportStaff_Load(object sender, EventArgs e)
        {
            

            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "S.No";
            dataGridView1.Columns[1].Name = "Name";
            dataGridView1.Columns[2].Name = "Phone";
            dataGridView1.Columns[3].Name = "Role";
            dataGridView1.Columns[4].Name = "Order Count";

            DataGridViewImageColumn imgColumn = new DataGridViewImageColumn();
            imgColumn.Name = "Details";
            imgColumn.HeaderText = "";
            imgColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imgColumn.DefaultCellStyle.NullValue = null;
            dataGridView1.Columns.Add(imgColumn);
            dataGridView1.Columns["Details"].Width = 60;

            dataGridView2.ColumnCount = 5;
            dataGridView2.Columns[0].Name = "TableName";
            dataGridView2.Columns[1].Name = "ProductName";
            dataGridView2.Columns[2].Name = "Price";
            dataGridView2.Columns[3].Name = "Qty";
            dataGridView2.Columns[4].Name = "Total";

            dataGridView1.Visible = false;
            dataGridView2.Visible = false;

            btn_report.MouseEnter += btn_report_MouseEnter;
            btn_report.MouseLeave += btn_report_MouseLeave;
            button1.MouseEnter += button1_MouseEnter;
            button1.MouseLeave += button1_MouseLeave;
        }

        private void btn_report_MouseEnter(object sender, EventArgs e)
        {
            btn_report.BackColor = Color.Green;
            btn_report.ForeColor = Color.White;
        }

        private void btn_report_MouseLeave(object sender, EventArgs e)
        {
            btn_report.BackColor = Color.FromArgb(225, 225, 225);
            btn_report.ForeColor = Color.Black;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.Red;
            button1.ForeColor = Color.White;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(225, 225, 225);
            button1.ForeColor = Color.Black;
        }
    }
}
