using System;
using System.Collections;
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
    public partial class frmKitchenView : Form
    {
        public frmKitchenView()
        {
            InitializeComponent();
        }

        private void frmKitchenView_Load(object sender, EventArgs e)
        {
            GetOrder();
            MainClass.ApplyTheme(this, ThemeManager.CurrentTheme);
            ThemeManager.ThemeChanged += OnThemeChanged;
        }
        private void OnThemeChanged(string newTheme)
        {
            MainClass.ApplyTheme(this, newTheme);
        }

        private void GetOrder()
        {
            flowLayoutPanel1.Controls.Clear();
            string qr1 = @"select* from tblMain where status in  ('Pending', 'Hold') ";
            SqlCommand cmd1 = new SqlCommand(qr1, MainClass.con);
            DataTable dt1 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd1);
            da.Fill(dt1);

            FlowLayoutPanel p1;

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                p1 = new FlowLayoutPanel();
                p1.AutoSize = true;
                p1.Width = 230;
                p1.Height = 350;
                p1.FlowDirection = FlowDirection.TopDown;
                p1.BorderStyle = BorderStyle.FixedSingle;
                p1.Margin = new Padding(10, 10, 10, 10);
                

                FlowLayoutPanel p2 = new FlowLayoutPanel();
                p2 = new FlowLayoutPanel();
                p2.BackColor = Color.FromArgb(50, 55, 89);
                p2.AutoSize = true;
                p2.Width = 230;
                p2.Height = 350;
                p2.FlowDirection = FlowDirection.TopDown;
                p2.BorderStyle = BorderStyle.FixedSingle;
                p2.Margin = new Padding(0, 0, 0, 0);
                

                Label lb1 = new Label(); // TableName
                lb1.ForeColor = Color.White;
                lb1.Margin = new Padding(10, 10, 3, 0);
                lb1.AutoSize = true;

                Label lb2 = new Label(); // WaiterName
                lb2.ForeColor = Color.White;
                lb2.Margin = new Padding(10, 5, 3, 0);
                lb2.AutoSize = true;

                Label lb3 = new Label(); // ordertime
                lb3.ForeColor = Color.White;
                lb3.Margin = new Padding(10, 5, 3, 0);
                lb3.AutoSize = true;

                Label lb4 = new Label(); // orderType
                lb4.ForeColor = Color.White;

                lb4.Margin = new Padding(10, 5, 3,10);
                lb4.AutoSize = true;

                lb1.Text = "Table: " + dt1.Rows[i]["TableName"].ToString();
                lb2.Text = "Waiter Name: " + dt1.Rows[i]["WaiterName"].ToString();
                lb3.Text = "Order Time: " + dt1.Rows[i]["aTime"].ToString();
                lb4.Text = "Order Type: " + dt1.Rows[i]["orderType"].ToString();

                p2.Controls.Add(lb1);
                p2.Controls.Add(lb2);
                p2.Controls.Add(lb3);
                p2.Controls.Add(lb4);

                p1.Controls.Add(p2);

                // them sp
                int mid = 0;
                mid = Convert.ToInt32(dt1.Rows[i]["MainID"].ToString());

                string qry2 = @"select* from tblMain m inner join tblDetails d on m.MainID = d.MainID
                                                       inner join products p on p.pID = d.proID 
                                                       where m.MainID = " + mid + "";
                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                DataTable dt2 = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                da2.Fill(dt2);

                for (int j = 0; j < dt2.Rows.Count; j++)
                {
                    Label lb5 = new Label();
                    if (ThemeManager.CurrentTheme == "Light")
                    {
                        lb5.ForeColor = Color.Black;
                    }
                    else
                    {
                        lb5.ForeColor = Color.White;
                    }
                    lb5.Margin = new Padding(10, 5, 3, 0);
                    lb5.AutoSize = true;

                    int no = j + 1;
                    lb5.Text = "" + no + " " + dt2.Rows[j]["pName"].ToString() + " " + dt2.Rows[j]["qty"].ToString();
                    p1.Controls.Add(lb5);

                }

                // them button thay doi order

                Button b = new Button();
                b.Size = new Size(100, 35);
                b.BackColor = Color.FromArgb(249, 89, 89);
                b.Margin = new Padding(30, 5, 3, 10);
                b.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                b.Text = "Complete";
                b.ForeColor = Color.FromArgb(227, 227, 227);

                b.Tag = dt1.Rows[i]["MainID"].ToString();

                b.Click += new EventHandler(b_Click);
                p1.Controls.Add(b);

                flowLayoutPanel1.Controls.Add(p1);
            }
        }
        private void b_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag.ToString());

            DialogResult result = MessageBox.Show("Are you sure you ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string qry = @"update tblMain set status = 'Complete' where MainID = @ID";
                Hashtable ht = new Hashtable();
                ht.Add("@ID", id);

                if (MainClass.SQL(qry, ht) > 0)
                {
                    frmSave frm = new frmSave();
                    frm.ShowDialog();
                }

                GetOrder();
            }

        }
    }
}
