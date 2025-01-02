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
    public partial class frmStaffAdd : Form
    {
        public int id = 0;
        public frmStaffAdd()
        {
            InitializeComponent();
            txtName.TabIndex = 0;
            txtPhone.TabIndex = 1;
        }
        
        private void btn_save_Click(object sender, EventArgs e)
        {
            
            string qry = "";

            if (id == 0)
            {
                qry = "insert into staff(sName, sPhone, sRole, sAddress) values (@Name, @phone, @role,@address)";
            }
            else
            {
                qry = "Update staff set sName = @Name, sPhone = @phone, sRole = @role, sAddress = @address where staffID = @id";
            }

            Hashtable ht = new Hashtable();
            ht.Add("@id", id);
            ht.Add("@Name", txtName.Text);
            ht.Add("@phone", txtPhone.Text);
            ht.Add("@role", cbRole.Text);
            ht.Add("@address",txtAddress.Text);

            if (MainClass.SQL(qry, ht) > 0)
            {
                frmSave frm = new frmSave();
                frm.ShowDialog();
                id = 0;
                txtName.Text = "";
                txtPhone.Text = "";
                txtAddress.Text = "";
                cbRole.SelectedIndex = -1;
                txtName.Focus();
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmStaffAdd_Load(object sender, EventArgs e)
        {
            List<string> roles = new List<string> { "Cashier","Waiter","Driver","Manager","Fired" };

            
            cbRole.DataSource = roles.Where(role => role != "Fired").ToList();

            if (!string.IsNullOrEmpty(cbRole.Tag?.ToString()))
            {
                cbRole.SelectedItem = cbRole.Tag.ToString();
            }

            
        } 
    }
}
