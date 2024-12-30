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

namespace CafeManagement.Model
{
    public partial class frmTableAdd : Form
    {
        public frmTableAdd()
        {
            InitializeComponent();
        }

        public int id = 0;

        private void btn_save_Click(object sender, EventArgs e)
        {
            string qry = "";

            
            string checkQry = "select count(*) from tables where tName = @Name";
            Hashtable checkHt = new Hashtable();
            checkHt.Add("@Name", txtName.Text);

            int count = Convert.ToInt32(MainClass.SQLScalar(checkQry, checkHt) ?? 0); //nếu SQLScalar trả về null thì thay nó bằng 0
            if (count > 0 && id == 0)
            {
                MessageBox.Show("Table was exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

          
            if (id == 0)
            {
                qry = "insert into tables (tName) values (@Name)";
            }
            else
            {
                qry = "update tables set tName = @Name where tID = @id";
            }

            Hashtable ht = new Hashtable();
            ht.Add("@id", id);
            ht.Add("@Name", txtName.Text);

            if (MainClass.SQL(qry, ht) > 0)
            {
                frmSave frm = new frmSave();
                frm.ShowDialog();
                id = 0;
                txtName.Text = "";
                txtName.Focus();
            }
        }

      

       
        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
