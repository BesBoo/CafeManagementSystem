using CafeManagement.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeManagement
{
    public partial class frmLogin : Form
    {
        private string uName = string.Empty;
        
        public frmLogin()
        {
            InitializeComponent();
            txtUser.TabIndex = 0;
            txtPass.TabIndex = 1;
            btn_login.TabIndex = 2;
            txtUser.Focus();

            
            txtPass.UseSystemPasswordChar = true; // Mặc định ẩn mật khẩu

            // tạo hiệu ứng hover
            btn_Close.MouseEnter += btn_exit_MouseEnter;
            btn_Close.MouseLeave += btn_exit_MouseLeave;
            btn_login.MouseEnter += btn_login_MouseEnter;
            btn_login.MouseLeave += btn_login_MouseLeave;

            // tạo border radius
            btn_Close.Paint += btn_exit_Paint;
            btn_login.Paint += btn_login_Paint;

            p.Paint += p_Paint;
            pictureBox2.BackColor = Color.Transparent;
            

            label1.BackColor = Color.Transparent;
            label2.BackColor = Color.Transparent;
            label3.BackColor = Color.Transparent;

            txtUser.KeyDown += txtUser_KeyDown;
            txtPass.KeyDown += txtPass_KeyDown;

            this.FormBorderStyle = FormBorderStyle.None;
            this.Region = CreateRoundedRegion(30);
        }
        private void frmLogin_Load(object sender, EventArgs e)
        {
            this.Region = CreateRoundedRegion(20);

        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            //if (MainClass.isvalidUser(txtUser.Text, txtPass.Text) == false)
            //{
            //    frmLoginFailed frmFailed = new frmLoginFailed();
            //    frmFailed.FormClosed += (s, args) =>
            //    {
            //        ClearTextBoxes();
            //    };
            //    frmFailed.ShowDialog();
            //    return;
            //}
            //else
            //{
            //    uName = txtUser.Text;
            //    MainClass.USER = uName;

            //    this.Hide();
            //    frmMain frm = new frmMain();
            //    frm.SetUserLabel(uName);
            //    frm.Show();
            //}

            if (MainClass.isvalidUser(txtUser.Text, txtPass.Text))
            {
                uName = txtUser.Text;
                MainClass.USER = uName;

                this.Hide();
                frmMain frm = new frmMain();
                frm.SetUserLabel(uName);
                frm.Show();
            }
            else
            {
                ClearTextBoxes(); 
            }
        }
        private void ClearTextBoxes()
        {
            txtUser.Clear();
            txtPass.Clear();
            txtUser.Focus();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPass.Focus();
            }
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_login.PerformClick();
            }
        }
        private void btn_exit_MouseEnter(object sender, EventArgs e)
        {
            btn_Close.BackColor = Color.Red;
            btn_Close.ForeColor = Color.White;
        }

        private void btn_exit_MouseLeave(object sender, EventArgs e)
        {
            btn_Close.BackColor = Color.FromArgb(225, 225, 225);
            btn_Close.ForeColor = Color.Black;
        }


        private void btn_login_MouseEnter(object sender, EventArgs e)
        {
            btn_login.BackColor = Color.Green;
            btn_login.ForeColor = Color.White;
        }

        private void btn_login_MouseLeave(object sender, EventArgs e)
        {
            btn_login.BackColor = Color.FromArgb(225, 225, 225);
            btn_login.ForeColor = Color.Black;
        }


        private void DrawRoundedButton(Button btn, PaintEventArgs e)
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            int radius = 20;
            Rectangle rect = new Rectangle(0, 0, btn.Width, btn.Height);
            graphicsPath.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            graphicsPath.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
            graphicsPath.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
            graphicsPath.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
            graphicsPath.CloseAllFigures();

            btn.Region = new Region(graphicsPath);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.FillPath(new SolidBrush(btn.BackColor), graphicsPath);


            TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, rect, btn.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }


        private void btn_login_Paint(object sender, PaintEventArgs e)
        {
            DrawRoundedButton(btn_login, e);
        }
        private void btn_exit_Paint(object sender, PaintEventArgs e)
        {
            DrawRoundedButton(btn_Close, e);
        }

        private void p_Paint(object sender, PaintEventArgs e)
        {
            Color colorStart = Color.FromArgb(255, ColorTranslator.FromHtml("#D6E9F4"));
            Color colorEnd = Color.FromArgb(255, ColorTranslator.FromHtml("#A1C9D5"));


            using (LinearGradientBrush brush = new LinearGradientBrush(p.ClientRectangle, colorStart, colorEnd, 45F))
            {
                e.Graphics.FillRectangle(brush, p.ClientRectangle);
            }
        }

        private Region CreateRoundedRegion(int radius)
        {
            int smoothingOffset = 2;
            int effectiveRadius = radius - smoothingOffset;

            GraphicsPath path = new GraphicsPath();
            path.AddArc(new Rectangle(0, 0, effectiveRadius * 2, effectiveRadius * 2), 180, 90);
            path.AddLine(effectiveRadius, 0, this.Width - effectiveRadius, 0);
            path.AddArc(new Rectangle(this.Width - effectiveRadius * 2, 0, effectiveRadius * 2, effectiveRadius * 2), 270, 90);
            path.AddLine(this.Width, effectiveRadius, this.Width, this.Height - effectiveRadius);

            path.AddArc(new Rectangle(this.Width - effectiveRadius * 2, this.Height - effectiveRadius * 2, effectiveRadius * 2, effectiveRadius * 2), 0, 90);
            path.AddLine(this.Width - effectiveRadius, this.Height, effectiveRadius, this.Height);
            path.AddArc(new Rectangle(0, this.Height - effectiveRadius * 2, effectiveRadius * 2, effectiveRadius * 2), 90, 90);
            path.AddLine(0, this.Height - effectiveRadius, 0, effectiveRadius);

            path.CloseFigure();


            using (Bitmap bitmap = new Bitmap(this.Width, this.Height))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(new Pen(Color.Transparent, smoothingOffset), path);
            }

            return new Region(path);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        }

        

        private void txtPass_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void label3_Click(object sender, EventArgs e)
        {
            frmResetpass frm = new frmResetpass();
            frm.ShowDialog();
        }
    }
}
