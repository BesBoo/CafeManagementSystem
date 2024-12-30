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

namespace CafeManagement.Model
{
    public partial class frmTableSelect : Form
    {
        public frmTableSelect()
        {
            InitializeComponent();
            if (!this.IsHandleCreated) // Đảm bảo chỉ gán một lần
            {
                this.Load += frmTableSelect_Load;
            }
        }
        public string TableName;

        private void frmTableSelect_Load(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            string qry = "select* from tables";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                Button b = new Button
                {
                    Text = row["tName"].ToString(),
                    Width = 150,
                    Height = 50,
                    BackColor = Convert.ToBoolean(row["isAvailable"]) ? Color.FromArgb(241, 85, 126) : Color.Silver,
                    Enabled = Convert.ToBoolean(row["isAvailable"])
                };

                b.Click += b_Click;
                flowLayoutPanel1.Controls.Add(b);
            }
        }
        private void b_Click(object sender, EventArgs e)
        {
            TableName = (sender as Button).Text.ToString();
            this.Close();


        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
