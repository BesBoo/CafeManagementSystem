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
using System.Xml.Linq;

namespace CafeManagement.Model
{
    public partial class frmCategoryAdd : Form
    {
        public frmCategoryAdd()
        {
            InitializeComponent();
        }
        public int id = 0;

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Pls insert category name.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

  
            string checkQuery = "select count(*) from category where catName = @Name";
            Hashtable htCheck = new Hashtable();
            htCheck.Add("@Name", txtName.Text);

            int exists = Convert.ToInt32(MainClass.SQLScalar(checkQuery, htCheck));

            if (exists > 0)
            {
                MessageBox.Show("Category was exist.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Clear();
                txtName.Focus();
                return;
            }

            string qry = id == 0
                ? "insert into category (catName) values (@Name)"
                : "update category set catName = @Name where catID = @id";

            Hashtable ht = new Hashtable();
            ht.Add("@id", id);
            ht.Add("@Name", txtName.Text);

            if (MainClass.SQL(qry, ht) > 0)
            {
                frmSave frm = new frmSave();
                frm.ShowDialog();
                id = 0;
                txtName.Clear();
                txtName.Focus();
            }
        }

        private void frmCategoryAdd_Load(object sender, EventArgs e)
        {
            
            
        }
        

    }
}
