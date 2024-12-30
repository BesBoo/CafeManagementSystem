using CafeManagement.Model;
using CafeManagement.Reports;
using CafeManagement.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeManagement
{
    public partial class frmMain : Form
    {
        private Color lightDefaultBackColor = Color.FromArgb(200, 225, 221);
        private Color lightActiveBackColor = Color.FromArgb(193, 255, 246);
        private Color lightForeColor = Color.Black;

        private Color darkDefaultBackColor = Color.FromArgb(69, 93, 122);
        private Color darkActiveBackColor = Color.FromArgb(249, 89, 89);
        private Color darkForeColor = Color.White;

       
        public frmMain()
        {
            InitializeComponent();
           
            ThemeManager.ThemeChanged += ApplyTheme;
        }
        
        public void AddControls(Form f)
        {
            CenterPanel.Controls.Clear();
            f.Dock = DockStyle.Fill;
            f.TopLevel = false;
            CenterPanel.Controls.Add(f);
            f.Show();
        }
        private void ResetButtonColors()
        {
            Color defaultBackColor = ThemeManager.CurrentTheme == "Light" ? lightDefaultBackColor : darkDefaultBackColor;
            Color foreColor = ThemeManager.CurrentTheme == "Light" ? lightForeColor : darkForeColor;
            ResetButtonColorsRecursive(this, defaultBackColor, foreColor);
        }

        private void ResetButtonColorsRecursive(Control parent, Color defaultBackColor, Color foreColor)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = defaultBackColor;
                    btn.ForeColor = foreColor;
                }
                else if (ctrl.HasChildren)
                {
                    ResetButtonColorsRecursive(ctrl, defaultBackColor, foreColor); 
                }
            }
        }

        private void ApplyTheme(string theme)
        {
            ResetButtonColors();
        }

        private void ChangeButtonAppearance(Button btn)
        {
            ResetButtonColors(); 
            Color activeBackColor = ThemeManager.CurrentTheme == "Light" ? lightActiveBackColor : darkActiveBackColor;
            btn.BackColor = activeBackColor;
        }
        private void btn_home_Click(object sender, EventArgs e)
        {
            ChangeButtonAppearance(btn_home);
            AddControls(new frmHome());
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_cate_Click(object sender, EventArgs e)
        {
            ChangeButtonAppearance(btn_cate);
            AddControls(new frmCategoryView());
        }

        private void btn_table_Click(object sender, EventArgs e)
        {
            ChangeButtonAppearance(btn_table);
            AddControls(new frmTableView());
        }

        private void btn_staff_Click(object sender, EventArgs e)
        {
            ChangeButtonAppearance(btn_staff);
            AddControls(new frmStaffView());
        }

        private void btn_pos_Click(object sender, EventArgs e)
        {
            ChangeButtonAppearance(btn_pos);
            frmPOS frm = new frmPOS();
            frm.Show();
        }

        private void btn_product_Click(object sender, EventArgs e)
        {
            ChangeButtonAppearance(btn_product);
            AddControls(new frmProductView());
        }

        private void btn_kitchen_Click(object sender, EventArgs e)
        {
            ChangeButtonAppearance(btn_kitchen);
            AddControls(new frmKitchenView());

        }

        private void btn_setting_Click(object sender, EventArgs e)
        {
            ChangeButtonAppearance(btn_setting);
            AddControls(new frmSet());

        }

        private void btn_Report_Click(object sender, EventArgs e)
        {
            ChangeButtonAppearance(btn_Report);
            AddControls(new frmReportMain());
        }
        public void SetUserLabel(string uName)
        {
            lblUser.Text = "Welcome, " + uName;  
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            SetUserLabel(MainClass.USER);

           

        }

       
        
    }
}
