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
    public partial class frmStaffView : Form
    {
        public frmStaffView()
        {
            InitializeComponent();
            this.Shown += frmStaffView_Shown;

            
        }

        //private void frmStaffView_Load(object sender, EventArgs e)
        //{
        //    GetData();
        //}

        private void frmStaffView_Shown(object sender, EventArgs e)
        {
            GetData();
            MainClass.ApplyTheme(this, ThemeManager.CurrentTheme);
            ThemeManager.ThemeChanged += OnThemeChanged;
        }
        private void OnThemeChanged(string newTheme)
        {
            MainClass.ApplyTheme(this, newTheme);
        }
        public void GetData()
        {
            string qry = "select staffID, sName, sPhone, sRole, sAddress from staff where sRole !='Fired' and sName like '%" + txtSearch.Text + "%' ";
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvName);
            lb.Items.Add(dgvPhone);
            lb.Items.Add(dgvRole);
            lb.Items.Add(dgvAddress);

            MainClass.LoadData(qry, dataGridView1, lb);

        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            MainClass.BlurBackground(new Model.frmStaffAdd());
            GetData();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.OwningColumn.Name == "dgvedit")
            {
                frmStaffAdd frm = new frmStaffAdd();
                frm.id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["dgvid"].Value);
                frm.txtName.Text = Convert.ToString(dataGridView1.CurrentRow.Cells["dgvName"].Value);
                frm.cbRole.Tag = Convert.ToString(dataGridView1.CurrentRow.Cells["dgvRole"].Value);
                frm.txtPhone.Text = Convert.ToString(dataGridView1.CurrentRow.Cells["dgvPhone"].Value);
                frm.txtAddress.Text = Convert.ToString(dataGridView1.CurrentRow.Cells["dgvAddress"].Value);

                MainClass.BlurBackground(frm);

                GetData();
            }
            else if (dataGridView1.CurrentCell.OwningColumn.Name == "dgvdel")
            {
                DialogResult result = MessageBox.Show("Are you wanna delete ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["dgvid"].Value);
                    string qry = "delete from staff where staffID = " + id + "";
                    Hashtable ht = new Hashtable();
                    MainClass.SQL(qry, ht);

                    MessageBox.Show("Delete successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GetData();
                }
            }
            else if (dataGridView1.CurrentCell.OwningColumn.Name == "dgvFired")
            {
                DialogResult result = MessageBox.Show("Are you wanna fired ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) {
                    int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["dgvid"].Value);
                    string qry = "update staff set sRole = 'Fired' where staffID = @id";
                    Hashtable ht = new Hashtable
                    {
                        {"@id", id}
                    };

                    if (MainClass.SQL(qry, ht) > 0)
                    {
                        MessageBox.Show("Staff was 'Fired'.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        GetData();
                    }
                }
                
            }
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            GetData();
        }
    }
}
