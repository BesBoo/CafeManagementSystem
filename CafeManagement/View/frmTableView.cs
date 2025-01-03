using CafeManagement.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeManagement.View
{
    public partial class frmTableView : Form
    {
        public frmTableView()
        {
            InitializeComponent();
        }

        private void frmTableView_Load(object sender, EventArgs e)
        {
            GetRole();
            GetData();
            MainClass.ApplyTheme(this, ThemeManager.CurrentTheme);
            ThemeManager.ThemeChanged += OnThemeChanged;
        }
        private void OnThemeChanged(string newTheme)
        {
            MainClass.ApplyTheme(this, newTheme);
        }

        private void GetRole()
        {
            try
            {
                string role = MainClass.GetuserRole(MainClass.USER);

                if (role == "waiter")
                {
                    btn_Add.Visible = false;

                    dataGridView1.Columns["dgvedit"].Visible = false;
                    dataGridView1.Columns["dgvdel"].Visible = false;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public void GetData()
        {
            string qry = "select * from tables where tName like '%" + txtSearch.Text + "%' ";
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvName);

            MainClass.LoadData(qry, dataGridView1, lb);

            dataGridView1.Columns["isAvailable"].HeaderText = "";
            dataGridView1.Columns["isAvailable"].Width = 100;

        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            
            MainClass.BlurBackground(new frmTableAdd());
            GetData();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            GetData();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.OwningColumn.Name == "dgvedit")
            {
                frmTableAdd frm = new frmTableAdd();
                frm.id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["dgvid"].Value);
                frm.txtName.Text = Convert.ToString(dataGridView1.CurrentRow.Cells["dgvName"].Value);
                frm.ShowDialog();
                GetData();
            }
            if (dataGridView1.CurrentCell.OwningColumn.Name == "dgvdel")
            {
                DialogResult result = MessageBox.Show("Are you sure you ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["dgvid"].Value);
                    string qry = "delete from tables where tID = " + id + "";
                    Hashtable ht = new Hashtable();
                    MainClass.SQL(qry, ht);
                    MessageBox.Show("Delete successfully!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GetData();
                }
            }
        }
    }
}
