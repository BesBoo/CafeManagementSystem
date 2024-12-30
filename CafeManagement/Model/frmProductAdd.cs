using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeManagement.Model
{
    public partial class frmProductAdd : Form
    {
        public frmProductAdd()
        {
            InitializeComponent();
            txtName.TabIndex = 0;
            txtPrice.TabIndex = 1;
            cbCat.TabIndex = 2;
            txtStock.TabIndex = 3;
        }
        public int id = 0;
        public int cID = 0;
        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmProductAdd_Load(object sender, EventArgs e)
        {
            string qry = "select catID 'id',catName 'name' from category ";
            MainClass.CBFill(qry, cbCat);

            if (cID > 0) // để update
            {
                cbCat.SelectedValue = cID;
            }

            if (id > 0)
            {
                ForUpDateLoadData();
            }
        }
        Byte[] imageByteArray;
        private void btn_browse_Click(object sender, EventArgs e)
        {
            

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images (*.jpg;*.png)|*.jpg;*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;
                string projectPath = Application.StartupPath;
                string destPath = Path.Combine(projectPath, "Images", Path.GetFileName(filePath));

                if (!Directory.Exists(Path.Combine(projectPath, "Images")))
                {
                    Directory.CreateDirectory(Path.Combine(projectPath, "Images"));
                }


                File.Copy(filePath, destPath, true);

                txtImage.Image = new Bitmap(destPath);
            }   
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            string qry = "";

            if (id == 0)
            {
                if (IsProductExist(txtName.Text.Trim()))
                {
                    MessageBox.Show("This product already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                qry = "insert into products (pName, pPrice, CategoryID, pImage, pStock) values (@Name, @price, @cat, @img, @stock)";
            }
            else
            {
                qry = "update products set pName = @Name, pPrice = @price, CategoryID = @cat, pImage = @img, pStock = @stock where pID = @id";
            }

            Image temp = new Bitmap(txtImage.Image);
            MemoryStream ms = new MemoryStream();
            temp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            imageByteArray = ms.ToArray();

            Hashtable ht = new Hashtable();
            ht.Add("@id", id);
            ht.Add("@Name", txtName.Text);
            ht.Add("@price", txtPrice.Text);
            ht.Add("@cat", Convert.ToInt32(cbCat.SelectedValue));
            ht.Add("@img", imageByteArray);
            ht.Add("@stock", Convert.ToInt32(txtStock.Text));

            if (MainClass.SQL(qry, ht) > 0)
            {
                frmSave frm = new frmSave();
                frm.ShowDialog();
                id = 0;
                cID = 0;
                txtName.Text = "";
                txtPrice.Text = "";
                txtStock.Text = "";
                cbCat.SelectedIndex = -1;
                txtName.Focus();
            }
        }
        private void ForUpDateLoadData()
        {
            string qry = @"select * from products where pid = " + id + "";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                txtName.Text = dt.Rows[0]["pName"].ToString();
                txtPrice.Text = dt.Rows[0]["pPrice"].ToString();
                txtStock.Text = dt.Rows[0]["pStock"].ToString();
                Byte[] imageArray = (byte[])dt.Rows[0]["pImage"];
                byte[] imageByteArray = imageArray;
                txtImage.Image = Image.FromStream(new MemoryStream(imageArray));
            }
        }

        private bool IsProductExist(string productName)
        {
            string qry = "select count(*) from products where LOWER(pName) = LOWER(@Name)";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            cmd.Parameters.AddWithValue("@Name", productName);

            if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            MainClass.con.Close();

            return count > 0;
        }
    }
}
