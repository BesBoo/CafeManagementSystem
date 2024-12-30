using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeManagement.Reports
{
    public partial class frmReportMain : Form
    {
        public frmReportMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmReports frm = new frmReports();
            frm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmReportStaff frm2 = new frmReportStaff();
            frm2.ShowDialog();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            frmReportCate frm3 = new frmReportCate();
            frm3.ShowDialog();
        }
        private void OnThemeChanged(string newTheme)
        {
            MainClass.ApplyTheme(this, newTheme);

        }

        private void frmReportMain_Load(object sender, EventArgs e)
        {
            MainClass.ApplyTheme(this, ThemeManager.CurrentTheme);
            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        
    }
}
