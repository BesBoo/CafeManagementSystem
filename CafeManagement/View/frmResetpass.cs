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

namespace CafeManagement.View
{
    public partial class frmResetpass : Form
    {
        public frmResetpass()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            txtUser.TabIndex = 0;
            txtPass.TabIndex = 1;
            txtConfirmpass.TabIndex = 2;
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text.Trim();
            string newPass = txtPass.Text.Trim();
            string confirmPass = txtConfirmpass.Text.Trim();

            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmPass))
            {
                MessageBox.Show("Please enter complete information.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Confirmation password does not match.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

           
            string qry = @"SELECT * FROM users WHERE username = @username";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            cmd.Parameters.AddWithValue("@username", username);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Username does not exist.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUser.Clear();
                txtPass.Clear();
                txtConfirmpass.Clear();
                txtUser.Focus();
                return;
            }

            
            if (Convert.ToBoolean(dt.Rows[0]["isDisabled"]) == true)
            {
                MessageBox.Show("This account has been disabled. Password cannot be changed.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUser.Clear();
                txtPass.Clear();
                txtConfirmpass.Clear();
                txtUser.Focus();
                return;
            }

        
            string updateQry = @"UPDATE users SET upass = @newPass, failedAttempts = 0, isDisabled = 0 WHERE username = @username";
            SqlCommand updateCmd = new SqlCommand(updateQry, MainClass.con);
            updateCmd.Parameters.AddWithValue("@newPass", newPass);
            updateCmd.Parameters.AddWithValue("@username", username);

            try
            {
                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                updateCmd.ExecuteNonQuery();
                MessageBox.Show("Password reset successful.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (MainClass.con.State == ConnectionState.Open) MainClass.con.Close();
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
