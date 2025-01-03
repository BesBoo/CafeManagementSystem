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
    public partial class frmProductView : Form
    {
        public frmProductView()
        {
            InitializeComponent();
            //this.Load += frmProductView_Load;
        }

        

        private void frmProductView_Load(object sender, EventArgs e)
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

                if(role == "waiter")
                {
                    btn_Add.Visible = false;
                    if(dataGridView1.Columns.Contains("dgvedit")){
                        dataGridView1.Columns["dgvedit"].Visible = false;  
                    }

                    
                    if(dataGridView1.Columns.Contains("dgvdel")){
                        dataGridView1.Columns["dgvdel"].Visible = false;  
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public void GetData()
        {
            

            try
            {
                string qry = "SELECT pID, pName, pPrice, CategoryID, c.catName, pStock FROM products p INNER JOIN category c ON c.catID = p.CategoryID WHERE pName LIKE '%" + txtSearch.Text + "%'";
                ListBox lb = new ListBox();
                lb.Items.Add(dgvid);
                lb.Items.Add(dgvName);
                lb.Items.Add(dgvPrice);
                lb.Items.Add(dgvcatID);
                lb.Items.Add(dgvcat);
                lb.Items.Add(dgvStock);



                MainClass.LoadData(qry, dataGridView1, lb);

                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("No data found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            MainClass.BlurBackground(new Model.frmProductAdd());
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
                frmProductAdd frm = new frmProductAdd();
                frm.id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["dgvid"].Value);
                frm.cID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["dgvcatID"].Value);


                MainClass.BlurBackground(frm);

                GetData();
            }
            if (dataGridView1.CurrentCell.OwningColumn.Name == "dgvdel")
            {
                DialogResult result = MessageBox.Show("Are you sure you ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["dgvid"].Value);
                    string qry = "delete from products where pID = " + id + "";
                    Hashtable ht = new Hashtable();
                    MainClass.SQL(qry, ht);

                    MessageBox.Show("Delete successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GetData();
                }
            }
        }
    }
}
