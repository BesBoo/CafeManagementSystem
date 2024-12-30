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
    public partial class frmReportCate : Form
    {
        public frmReportCate()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmReportCate_Load(object sender, EventArgs e)
        {
           dataGridView1.Visible = false;
        }

        

        private void btn_report_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            LoadAllProductsByCategory();
        }

        private void LoadAllProductsByCategory()
        {
            string qry = @"
        SELECT 
            c.catName AS CategoryName,
            p.pName AS ProductName,
            SUM(d.qty) AS QuantitySold,
            SUM(d.amount) / SUM(d.qty) AS UnitPrice
        FROM 
            tblDetails d
        INNER JOIN 
            products p ON d.proID = p.pID
        INNER JOIN 
            category c ON p.CategoryID = c.catID
        GROUP BY 
            c.catName, p.pName
        HAVING 
            SUM(d.qty) > 0  
        ORDER BY 
            c.catName";

            try
            {
                SqlCommand cmd = new SqlCommand(qry, MainClass.con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.Rows.Clear();

                //string currentCategory = null;
                int sno = 1;

                List<string> addedCategories = new List<string>();

                foreach (DataRow row in dt.Rows)
                {
                    string categoryName = row["CategoryName"].ToString();

                    if (!addedCategories.Contains(categoryName))
                    {
                       
                        addedCategories.Add(categoryName);

                       
                        dataGridView1.Rows.Add("", categoryName, "", "");
                        int categoryRowIndex = dataGridView1.Rows.Add("", categoryName, "", "");
                        dataGridView1.Rows[categoryRowIndex].DefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
                        dataGridView1.Rows[categoryRowIndex].DefaultCellStyle.ForeColor = Color.Black;
                        dataGridView1.Rows[categoryRowIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                        dataGridView1.Rows[categoryRowIndex].DefaultCellStyle.BackColor = Color.White;
                        dataGridView1.Rows[categoryRowIndex].DividerHeight = 0;

                        sno = 1;
                    }


                    dataGridView1.Rows.Add(
                        sno++,
                        row["ProductName"],
                        row["QuantitySold"],
                        Convert.ToDouble(row["UnitPrice"]).ToString("F2")
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

    }
}
