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

namespace CafeManagement.View
{
    public partial class frmBillView : Form
    {
        public int MainID { get; set; }
        public frmBillView(int mainID)
        {
            InitializeComponent();
            MainID = mainID;
        }

        private void frmBillView_Load(object sender, EventArgs e)
        {
            
            ConfigureDataGridView();
            CustomizeDataGridView();
            LoadBillDetails();
        }
        private void CustomizeDataGridView()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView1.BackgroundColor = Color.White;

            // Tùy chỉnh header
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None; 
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black; 
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize; 

            // Tùy chỉnh dòng dữ liệu
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White; 
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 


            dataGridView1.RowHeadersVisible = false; 
            dataGridView1.GridColor = Color.White;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
        private void ConfigureDataGridView()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("ProductName", "Product Name");
            dataGridView1.Columns["ProductName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns.Add("Quantity", "Quantity");
            dataGridView1.Columns["Quantity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns.Add("Price", "Price");
            dataGridView1.Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns.Add("Total", "Total");
            dataGridView1.Columns["Total"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
        private void LoadBillDetails()
        {
            try
            {
                string qryMain = "select * from tblMain where MainID = @MainID";
                string qryDetails = @"
                    select p.pName AS ProductName, d.qty AS Quantity, d.price AS Price, d.amount AS Total
                    from tblDetails d
                    inner join products p on d.proID = p.pID
                    where d.MainID = @MainID";

                using (SqlConnection con = new SqlConnection(MainClass.con_string))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(qryMain, con))
                    {
                        cmd.Parameters.AddWithValue("@MainID", MainID);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                lblDate.Text = Convert.ToDateTime(dr["aDate"]).ToString("yyyy-MM-dd");
                                lblTime.Text = dr["aTime"].ToString();
                                lblOrderType.Text = dr["orderType"].ToString();

                                if (lblOrderType.Text == "Take Away" || lblOrderType.Text == "Delivery")
                                {
                                    lblName.Text = dr["CustName"].ToString();
                                    lblPhone.Text = dr["CustPhone"].ToString();
                                    lblName.Visible = true;
                                    lblPhone.Visible = true;
                                    lblTable.Visible = false;
                                    labelTable.Visible = false;
                                }
                                else
                                {
                                    lblTable.Text = dr["TableName"].ToString();
                                    lblTable.Visible = true;
                                    lblName.Visible = false;
                                    labelCustname.Visible = false;
                                    lblPhone.Visible = false;
                                    labelPhone.Visible = false;
                                }

                                lblStatus.Text = dr["status"].ToString();
                                lblTotal.Text = $"{dr["total"]} $";
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(qryDetails, con))
                    {
                        cmd.Parameters.AddWithValue("@MainID", MainID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            dataGridView1.Rows.Add(
                                row["ProductName"],
                                row["Quantity"],
                                Convert.ToDouble(row["Price"]).ToString("C"),
                                Convert.ToDouble(row["Total"]).ToString("C")
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bill details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
