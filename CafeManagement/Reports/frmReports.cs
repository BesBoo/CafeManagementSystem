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
    public partial class frmReports : Form
    {
        public frmReports()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_report_Click(object sender, EventArgs e)
        {
            DateTime startDate = dateTimePicker1.Value.Date;
            DateTime endDate = dateTimePicker2.Value.Date;
            DateTime today = DateTime.Today;  
            dataGridView1.Visible = true;
            chart1.Visible = true;
            label1.Visible = true;
            lblTotal.Visible =true;

            if (endDate > today)
            {
                MessageBox.Show("End date cannot be later than today. Please choose another date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateTimePicker2.Value = today;  
                return; 
            }
            else if( startDate > today)
            {
                MessageBox.Show("End date cannot be later than today. Please choose another date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateTimePicker1.Value = today;
                return;
            }

            

            string query = @"SELECT 
                                ROW_NUMBER() OVER (ORDER BY SUM(d.qty) DESC) AS SNo, 
                                p.pName AS Products, 
                                SUM(d.qty) AS Qty, 
                                d.price AS Price, 
                                SUM(d.qty * d.price) AS Total
                            FROM tblDetails d
                            INNER JOIN tblMain m ON d.MainID = m.MainID
                            INNER JOIN products p ON d.proID = p.pID
                            WHERE 
                            CAST(m.aDate AS DATE) BETWEEN @StartDate AND @EndDate and status IN ('Paid')
                            GROUP BY p.pName, d.price
                            ORDER BY Total DESC";

            try
            {
                using (SqlConnection con = new SqlConnection(MainClass.con_string))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;


                        decimal totalRevenue = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            totalRevenue += Convert.ToDecimal(row["Total"]);
                        }
                        lblTotal.Text = totalRevenue.ToString("C");

                        DataView dv = new DataView(dt);
                        dv.Sort = "SNo ASC";  
                        dataGridView1.DataSource = dv;

                        chart1.Series.Clear();  

                        var series = new System.Windows.Forms.DataVisualization.Charting.Series
                        {
                            Name = "Revenue",
                            IsValueShownAsLabel = true,
                            ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column
                        };

                        foreach (DataRow row in dt.Rows)
                        {
                            string productName = row["Products"].ToString();
                            decimal total = Convert.ToDecimal(row["Total"]);
                            series.Points.AddXY(productName, total);
                        }

                        chart1.Series.Add(series);

      
                        chart1.ChartAreas[0].AxisX.Title = "Products";
                        chart1.ChartAreas[0].AxisY.Title = "Total Revenue";
                        chart1.Titles.Clear();
                        chart1.Titles.Add("Product Revenue Report");

                        chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false; 
                        chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

                        
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmReports_Load(object sender, EventArgs e)
        {

            
            chart1.Legends.Clear();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();


            DataGridViewTextBoxColumn colSNo = new DataGridViewTextBoxColumn();
            colSNo.HeaderText = "S#";
            colSNo.Name = "dgvSno";
            colSNo.DataPropertyName = "SNo";
            colSNo.Width = 50;
            dataGridView1.Columns.Add(colSNo);


            DataGridViewTextBoxColumn colProducts = new DataGridViewTextBoxColumn();
            colProducts.HeaderText = "Products";
            colProducts.Name = "dgvProducts";
            colProducts.DataPropertyName = "Products";
            colProducts.Width = 150;
            dataGridView1.Columns.Add(colProducts);

            DataGridViewTextBoxColumn colQty = new DataGridViewTextBoxColumn();
            colQty.HeaderText = "Qty";
            colQty.Name = "dgvQty";
            colQty.DataPropertyName = "Qty";
            colQty.Width = 100;
            dataGridView1.Columns.Add(colQty);
            


            DataGridViewTextBoxColumn colPrice = new DataGridViewTextBoxColumn();
            colPrice.HeaderText = "Price";
            colPrice.Name = "dgvPrice";
            colPrice.DataPropertyName = "Price";
            colPrice.Width = 100;
            dataGridView1.Columns.Add(colPrice);

            DataGridViewTextBoxColumn colTotal = new DataGridViewTextBoxColumn();
            colTotal.HeaderText = "Total";
            colTotal.Name = "dgvTotal";
            colTotal.DataPropertyName = "Total";
            colTotal.Width = 100;
            dataGridView1.Columns.Add(colTotal);

            dataGridView1.Visible = false;
            chart1.Visible = false;
            label1.Visible = false;
            lblTotal.Visible = false;

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
