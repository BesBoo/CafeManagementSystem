using CafeManagement.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeManagement
{
    public class MainClass
    {
        public static readonly string con_string = @"Data Source=DUCCKY\SQLEXPRESS;Initial Catalog=projectLTCSDL;Integrated Security=True;";

        public static SqlConnection con = new SqlConnection(con_string);

    public static bool isvalidUser(string user, string pass)
        {

            

            bool isValid = false;
            string checkQry = @"SELECT * FROM users WHERE username = @username";
            SqlCommand cmdCheck = new SqlCommand(checkQry, con);
            cmdCheck.Parameters.AddWithValue("@username", user);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmdCheck);
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
               
                bool isDisabled = Convert.ToBoolean(dt.Rows[0]["isDisabled"]);
                if (isDisabled)
                {
                    MessageBox.Show("Your account was disable .", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                
                string storedPassword = dt.Rows[0]["upass"].ToString();
                if (storedPassword == pass)
                {
                    isValid = true;

                    
                    string resetQry = @"UPDATE users SET failedAttempts = 0 WHERE username = @username";
                    SqlCommand cmdReset = new SqlCommand(resetQry, con);
                    cmdReset.Parameters.AddWithValue("@username", user);

                    if (con.State == ConnectionState.Closed) con.Open();
                    cmdReset.ExecuteNonQuery();
                    con.Close();

                    USER = dt.Rows[0]["uName"].ToString(); 
                }
                else
                {
                    
                    int failedAttempts = Convert.ToInt32(dt.Rows[0]["failedAttempts"]) + 1;

                    string updateQry = @"UPDATE users SET failedAttempts = @failedAttempts WHERE username = @username";
                    SqlCommand cmdUpdate = new SqlCommand(updateQry, con);
                    cmdUpdate.Parameters.AddWithValue("@failedAttempts", failedAttempts);
                    cmdUpdate.Parameters.AddWithValue("@username", user);

                    if (con.State == ConnectionState.Closed) con.Open();
                    cmdUpdate.ExecuteNonQuery();
                    con.Close();

                  
                    if (failedAttempts >= 3)
                    {
                        string disableQry = @"UPDATE users SET isDisabled = 1 WHERE username = @username";
                        SqlCommand cmdDisable = new SqlCommand(disableQry, con);
                        cmdDisable.Parameters.AddWithValue("@username", user);

                        if (con.State == ConnectionState.Closed) con.Open();
                        cmdDisable.ExecuteNonQuery();
                        con.Close();

                        MessageBox.Show("Your account was disable.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        frmLoginFailed frm=new frmLoginFailed();
                        frm.ShowDialog();
                        MessageBox.Show($"You have {3 - failedAttempts} times.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter complete information.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return isValid;

        }



        public static string user;
        public static string USER
        {
            get { return user; }
            set { user = value; }
        }

        public static string GetuserRole(string username)
        {
            string role = string.Empty;

            try
            {
                string sql = "select uRole from users where username = @username";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue ("@username", username);

                if(con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                role = cmd.ExecuteScalar()?.ToString();
                con.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: { ex.Message}","Notification",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return role;
        }

       

        public static int SQL(string qry, Hashtable ht)
        {
            int res = 0;
            try
            {
                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.CommandType = CommandType.Text;

                foreach (DictionaryEntry item in ht)
                {
                    cmd.Parameters.AddWithValue(item.Key.ToString(), item.Value);
                }
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                res = cmd.ExecuteNonQuery();
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
                con.Close();
            }
            return res;



        }
        public static void LoadData(string qry, DataGridView gv, ListBox lb)
        {

            gv.CellFormatting += new DataGridViewCellFormattingEventHandler(gv_CellFormatting);


            try
            {
                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                for (int i = 0; i < lb.Items.Count; i++)
                {
                    string colNam1 = ((DataGridViewColumn)lb.Items[i]).Name;
                    gv.Columns[colNam1].DataPropertyName = dt.Columns[i].ToString();
                }

                gv.DataSource = dt;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
                con.Close();
            }



        }

        private static void gv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            DataGridView gv = sender as DataGridView;
            if (gv == null) return;

            int count = 0;

            foreach (DataGridViewRow row in gv.Rows)
            {
                count++;
                row.Cells[0].Value = count;
            }
        }

        public static void BlurBackground(Form Model)
        {
            Form BackGround = new Form();
            using (Model)
            {
                BackGround.StartPosition = FormStartPosition.Manual;
                BackGround.FormBorderStyle = FormBorderStyle.None;
                BackGround.Opacity = 0.5d;
                BackGround.BackColor = Color.Black;            
                BackGround.ShowInTaskbar = false;
                BackGround.Show();
                Model.Owner = BackGround;
                Model.ShowDialog(BackGround);
                BackGround.Dispose();
            }
        }

        public static void CBFill(string qry, ComboBox cb)
        {
            SqlCommand cmd = new SqlCommand(qry, con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            cb.DisplayMember = "name";
            cb.ValueMember = "id";
            cb.DataSource = dt;
            cb.SelectedIndex = -1;
        }
        public static object SQLScalar(string qry, Hashtable ht)
        {
            object result = null;
            try
            {
                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.CommandType = CommandType.Text;

                foreach (DictionaryEntry item in ht)
                {
                    cmd.Parameters.AddWithValue(item.Key.ToString(), item.Value);
                }

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                result = cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return result;
        }

        // Toggle Mode
        public static void ApplyTheme(Form form, string mode)
        {
            Color backColorPrimary; // màu nền
            Color backColorSecondary; // màu panel
            Color buttonColor; // màu button
            Color foreColor; // màu chữ
            Color dataGridViewBackColor; 
            Color dataGridViewForeColor;
            Color textBoxBackColor;
            Color textBoxForeColor;

            if (mode == "Light")
            {
                backColorPrimary = Color.FromArgb(200, 225, 221);
                backColorSecondary = Color.FromArgb(211, 235, 242);
                buttonColor = Color.FromArgb(200, 225, 221);
                foreColor = Color.Black;

                dataGridViewBackColor = Color.White;
                dataGridViewForeColor = Color.Black;
                textBoxBackColor = Color.White;
                textBoxForeColor = Color.Black; 
            }
            else // Dark mode
            {
                backColorPrimary = Color.FromArgb(69, 93, 122);
                backColorSecondary = Color.FromArgb(70, 90, 100);
                buttonColor = Color.FromArgb(249, 89, 89);
                foreColor = Color.White;

                dataGridViewBackColor = Color.FromArgb(70, 90, 100);
                dataGridViewForeColor = Color.White;

                textBoxBackColor = Color.White;  
                textBoxForeColor = Color.Black;  
            }

            form.BackColor = backColorPrimary;

            foreach (Control control in form.Controls)
            {
                if (control is Button button)
                {
                    button.BackColor = buttonColor;
                    button.ForeColor = foreColor;
                }
                else if (control is Panel panel)
                {
                    panel.BackColor = backColorSecondary;

                    foreach (Control subControl in panel.Controls)
                    {
                        if (subControl is Label label)
                        {
                            label.ForeColor = foreColor;
                        }
                        else if (subControl is DataGridView dgv)
                        {
                            dgv.BackgroundColor = dataGridViewBackColor;
                            dgv.DefaultCellStyle.BackColor = dataGridViewBackColor;
                            dgv.DefaultCellStyle.ForeColor = dataGridViewForeColor;
                            dgv.ColumnHeadersDefaultCellStyle.BackColor = dataGridViewBackColor;
                            dgv.ColumnHeadersDefaultCellStyle.ForeColor = dataGridViewForeColor;
                            dgv.RowHeadersDefaultCellStyle.BackColor = dataGridViewBackColor;
                            dgv.RowHeadersDefaultCellStyle.ForeColor = dataGridViewForeColor;
                            dgv.EnableHeadersVisualStyles = false;
                        }
                        else if (subControl is TextBox textBox)
                        {
                            textBox.BackColor = textBoxBackColor;
                            textBox.ForeColor = textBoxForeColor;
                        }
                    }
                }
                else if (control is Label label)
                {
                    label.ForeColor = foreColor;
                }
                else if (control is DataGridView dgv)
                {
                    dgv.BackgroundColor = dataGridViewBackColor;
                    dgv.DefaultCellStyle.BackColor = dataGridViewBackColor;
                    dgv.DefaultCellStyle.ForeColor = dataGridViewForeColor;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = dataGridViewBackColor;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = dataGridViewForeColor;
                    dgv.RowHeadersDefaultCellStyle.BackColor = dataGridViewBackColor;
                    dgv.RowHeadersDefaultCellStyle.ForeColor = dataGridViewForeColor;
                    dgv.EnableHeadersVisualStyles = false;
                }
                else if (control is TextBox textBox)
                {
                    
                    textBox.BackColor = textBoxBackColor;
                    textBox.ForeColor = textBoxForeColor;
                }
                else
                {
                    control.BackColor = backColorPrimary;
                    control.ForeColor = foreColor;
                }
            }
            if (form is frmPOS frmPOSInstance)
            {
                frmPOSInstance.panel1.BackColor = mode == "Light" ? Color.FromArgb(200, 225, 221) : Color.FromArgb(69, 93, 122);
                frmPOSInstance.panel.BackColor = mode == "Light" ? Color.FromArgb(200, 225, 221) : Color.FromArgb(69, 93, 122);
                

                frmPOSInstance.ApplyThemeToCategoryButtons(mode);
            }

        }
    }
}
