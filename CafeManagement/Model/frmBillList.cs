using System;
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
    public partial class frmBillList : Form
    {
        public frmBillList()
        {
            InitializeComponent();
        }
        public int MainID = 0;
        private void frmBillList_Load(object sender, EventArgs e)
        {
            LoadData();

            btn_Exit.MouseEnter += btn_Exit_MouseEnter;
            btn_Exit.MouseLeave += btn_Exit_MouseLeave;
        }
        private void LoadData()
        {
            try
            {
                string qry = "select  MainID, TableName, WaiterName, orderType, status, total from tblMain where status in ('Pending','Complete') ";
                ListBox lb = new ListBox();
                lb.Items.Add(dgvid);
                lb.Items.Add(dgvTable);
                lb.Items.Add(dgvWaiter);
                lb.Items.Add(dgvType);
                lb.Items.Add(dgvStatus);
                lb.Items.Add(dgvTotal);

                MainClass.LoadData(qry, dataGridView1, lb);

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int count = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                count++;
                row.Cells[0].Value = count;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.OwningColumn.Name == "dgvedit")
            {
                MainID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["dgvid"].Value);
                this.Close();


            }

            
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Exit_MouseEnter(object sender, EventArgs e)
        {
            btn_Exit.BackColor = Color.Red;
            btn_Exit.ForeColor = Color.White;
        }

        private void btn_Exit_MouseLeave(object sender, EventArgs e)
        {
            btn_Exit.BackColor = Color.FromArgb(225, 225, 225);
            btn_Exit.ForeColor = Color.Black;
        }
    }
}
