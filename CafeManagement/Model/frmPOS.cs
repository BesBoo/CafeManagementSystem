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
    public partial class frmPOS : Form
    {
        
        public frmPOS()
        {
            InitializeComponent();
        }

        public int MainID = 0;
        public string OrderType = "";
        public int driverID = 0;
        public string customerName = "";
        public string customerPhone = "";

        private void ResetButtonColors()
        {
            Color defaultColor = Color.FromArgb(69, 93, 122);

            btn_new.BackColor = defaultColor;
            btn_hold.BackColor = defaultColor;
            btn_bill.BackColor = defaultColor;
            btn_KOT.BackColor = defaultColor;
            btn_Delivery.BackColor = defaultColor;
            btn_Takeaway.BackColor = defaultColor;
            btn_Din.BackColor = defaultColor;
            btn_checkout.BackColor = defaultColor;
        }

        private void ApplyTheme(string theme)
        {
            Color backColorPrimary;
            Color backColorSecondary;
            Color buttonColor;
            Color foreColor;

            if (theme == "Light")
            {
                backColorPrimary = Color.White;
                backColorSecondary = Color.Silver;
                buttonColor = Color.FromArgb(200, 225, 221);
                foreColor = Color.Black;
            }
            else // Dark mode
            {
                backColorPrimary = Color.FromArgb(69, 93, 122);
                backColorSecondary = Color.FromArgb(70, 90, 100);
                buttonColor = Color.FromArgb(249, 89, 89);
                foreColor = Color.White;
            }

            // Áp dụng màu cho form và các control
            this.BackColor = backColorPrimary;

            foreach (Control control in this.Controls)
            {
                if (control is Button button)
                {
                    button.BackColor = buttonColor;
                    button.ForeColor = foreColor;
                }
                else if (control is Panel panel)
                {
                    panel.BackColor = backColorSecondary;
                }
                else
                {
                    control.BackColor = backColorPrimary;
                    control.ForeColor = foreColor;
                }
            }
        }
        public void ApplyThemeToCategoryButtons(string mode)
        {
            Color backColor;
            Color foreColor;

            if (mode == "Light")
            {
                backColor = Color.FromArgb(200, 225, 221);
                foreColor = Color.Black;
            }
            else 
            {
                backColor = Color.FromArgb(69, 93, 122);
                foreColor = Color.FromArgb(227, 227, 227);
            }

            foreach (Control control in CategoryPanel.Controls)
            {
                if (control is Button button)
                {
                    button.BackColor = backColor;
                    button.ForeColor = foreColor;
                }
            }

            if (lastClickedButton != null)
            {
                lastClickedButton.BackColor = (mode == "Light") ? Color.Blue : Color.FromArgb(249, 89, 89);
            }
        }
        private void frmPOS_Load(object sender, EventArgs e)
        {
            dataGridView1.BorderStyle = BorderStyle.FixedSingle;
            AddCategory();

            ProductPanel.Controls.Clear();
            LoadProducts();

            MainClass.ApplyTheme(this, ThemeManager.CurrentTheme);
            ThemeManager.ThemeChanged += OnThemeChanged;
        }
        private void OnThemeChanged(string newTheme)
        {
            MainClass.ApplyTheme(this, newTheme);
        }


        private void AddCategory()
        {

            string qry = "select * from category";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            CategoryPanel.Controls.Clear();

            if (dt.Rows.Count > 0) 
            {
                foreach (DataRow row in dt.Rows)
                {
                    Button b = new Button();
                    

                    if(ThemeManager.CurrentTheme == "Dark")
                    {
                        b.Size = new Size(146, 41);
                        b.Text = row["catName"].ToString();
                        b.BackColor = Color.FromArgb(69, 93, 122);
                        b.ForeColor = Color.FromArgb(227, 227, 227);

                        b.Click += new EventHandler(b_Click);
                    }
                    else
                    {
                        b.Size = new Size(146, 41);
                        b.Text = row["catName"].ToString();
                        b.BackColor = Color.FromArgb(200, 225, 221);
                        b.ForeColor = Color.Black;

                        b.Click += new EventHandler(b_Click);
                    }


                    CategoryPanel.Controls.Add(b);
                }


            }
        }
        private string lastClickedCategory = "";
        private Button lastClickedButton = null;
        private void b_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            if (ThemeManager.CurrentTheme == "Light")
            {
                if (lastClickedButton != null)
                {
                    lastClickedButton.BackColor = Color.FromArgb(200, 225, 221); 
                    lastClickedButton.ForeColor = Color.Black;
                }
                b.BackColor = Color.FromArgb(69, 93, 122);
                b.ForeColor = Color.White;
                lastClickedButton = b;
            }
            else if (ThemeManager.CurrentTheme == "Dark")
            {
                if (lastClickedButton != null)
                {
                    lastClickedButton.BackColor = Color.FromArgb(69, 93, 122); // xanh dden
                }

                b.BackColor = Color.FromArgb(249, 89, 89); // cam
                lastClickedButton = b;
            }
            

            if (b.Text == lastClickedCategory)
            {
                if(ThemeManager.CurrentTheme == "Light")
                {
                    txtSearch.Text = "";
                    lastClickedCategory = "";
                    foreach (var item in ProductPanel.Controls)
                    {
                        var pro = (ucProduct)item;
                        pro.Visible = true;
                    }
                    b.BackColor = Color.FromArgb(200, 225, 221);
                    b.ForeColor = Color.Black;
                    lastClickedButton = null;
                }
                else
                {
                    txtSearch.Text = "";
                    lastClickedCategory = "";
                    foreach (var item in ProductPanel.Controls)
                    {
                        var pro = (ucProduct)item;
                        pro.Visible = true;
                    }
                    b.BackColor = Color.FromArgb(69, 93, 122);
                    lastClickedButton = null;
                }
                
            }
            else
            {
                lastClickedCategory = b.Text;
                foreach (var item in ProductPanel.Controls)
                {
                    var pro = (ucProduct)item;
                    pro.Visible = pro.PCategory.ToLower().Contains(b.Text.Trim().ToLower());
                }
            }
        }

        private int rowIndex = 1;
        private void AddItems(string id, String proID, string name, string cat, string price, Image pimage)
        {
            

            var w = new ucProduct()
            {
                PName = name,
                PPrice = price,
                PCategory = cat,
                PImage = pimage,
                id = Convert.ToInt32(proID)
            };

            ProductPanel.Controls.Add(w);

            w.onSelect += (ss, ee) =>
            {
                var wdg = (ucProduct)ss;
                bool productFound = false;
                
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    if (Convert.ToInt32(item.Cells["dgvproID"].Value) == wdg.id)
                    {
                        int qty = Convert.ToInt32(item.Cells["dgvQty"].Value) + 1;
                        item.Cells["dgvQty"].Value = qty;
                        item.Cells["dgvAmount"].Value = qty * double.Parse(item.Cells["dgvPrice"].Value.ToString());

                        productFound = true;
                        break;
                    }
                }
                if (!productFound)
                {
                    // thêm sp vào datagridview           
                    dataGridView1.Rows.Add(new object[] { rowIndex++, 0, wdg.id, wdg.PName, 1, wdg.PPrice, double.Parse(wdg.PPrice) });
                    UpdateRowNumbers();
                }
                GetTotal();
            };
        }
        private void UpdateStock(int productId, int newStock)
        {
            string qry = "UPDATE products SET pStock = @stock WHERE pID = @id";
            Hashtable ht = new Hashtable();
            ht.Add("@id", productId);
            ht.Add("@stock", newStock);
            MainClass.SQL(qry, ht);
        }

        // lay sp tu database
        private void LoadProducts()
        {
            string qry = "select * from products inner join category on catID = CategoryID";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow item in dt.Rows)
            {
                Byte[] imagearray = (byte[])item["pImage"];
                byte[] imagebytearray = imagearray;

                AddItems("0", item["pID"].ToString(), item["pName"].ToString(), item["catName"].ToString(), item["pPrice"].ToString(), Image.FromStream(new MemoryStream(imagearray)));
            }

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            foreach (var item in ProductPanel.Controls)
            {
                var pro = (ucProduct)item;
                pro.Visible = pro.PName.ToLower().Contains(txtSearch.Text.Trim());
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

        private void GetTotal()
        {
            double total = 0;
            lblTotal.Text = "";
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (item.Cells["dgvAmount"].Value != null)
                {
                    double amount;
                    if (double.TryParse(item.Cells["dgvAmount"].Value.ToString(), out amount))
                    {
                        total += amount;
                    }
                }

            }
            lblTotal.Text = total.ToString("N2");
        }

        private void btn_new_Click(object sender, EventArgs e)
        {
            ResetButtonColors();
            btn_new.BackColor = Color.FromArgb(249, 89, 89);
            btn_new.ForeColor = Color.FromArgb(227, 227, 227);
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            dataGridView1.Rows.Clear();
            rowIndex = 1;
            MainID = 0;
            lblTotal.Text = "0.0";
        }

        private void btn_Delivery_Click(object sender, EventArgs e)
        {
            ResetButtonColors();
            btn_Delivery.BackColor = Color.FromArgb(249, 89, 89);
            btn_Delivery.ForeColor = Color.FromArgb(227, 227, 227);
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            OrderType = "Delivery";

            frmAddCustomer frm = new frmAddCustomer();
            frm.mainID = MainID;
            frm.orderType = OrderType;
            MainClass.BlurBackground(frm);

            if (frm.txtName.Text != "")
            {
                driverID = frm.driverID;
                lblDriverName.Text = "Customer Name: " + frm.txtName.Text + "; Phone: " + frm.txtPhone.Text + "; Driver: " + frm.cbDriver.Text;
                lblDriverName.Visible = true;
                customerName = frm.txtName.Text;
                customerPhone = frm.txtPhone.Text;

            }
        }

        private void btn_Takeaway_Click(object sender, EventArgs e)
        {
            ResetButtonColors();
            btn_Takeaway.BackColor = Color.FromArgb(249, 89, 89);
            btn_Takeaway.ForeColor = Color.FromArgb(227, 227, 227);
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            OrderType = "Take Away";

            frmAddCustomer frm = new frmAddCustomer();
            frm.mainID = MainID;
            frm.orderType = OrderType;
            MainClass.BlurBackground(frm);

            if (frm.txtName.Text != "")
            {
                driverID = frm.driverID;
                lblDriverName.Text = "Customer Name: " + frm.txtName.Text + "; Phone: " + frm.txtPhone.Text;
                lblDriverName.Visible = true;
                customerName = frm.txtName.Text;
                customerPhone = frm.txtPhone.Text;

            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Din_Click(object sender, EventArgs e)
        {
            ResetButtonColors();
            btn_Din.BackColor = Color.FromArgb(249, 89, 89);
            btn_Din.ForeColor = Color.FromArgb(227, 227, 227);
            OrderType = "Din In";
            lblDriverName.Visible = false;
            frmTableSelect frm = new frmTableSelect();
            MainClass.BlurBackground(frm);
            if (frm.TableName != "")
            {
                lblTable.Text = frm.TableName;
                lblTable.Visible = true;
            }
            else
            {
                lblTable.Text = "";
                lblTable.Visible = false;
            }

            frmWaiterSelect frm2 = new frmWaiterSelect();
            MainClass.BlurBackground(frm2);
            if (frm2.waiterName != "")
            {
                lblWaiter.Text = frm2.waiterName;
                lblWaiter.Visible = true;
            }
            else
            {
                lblWaiter.Text = "";
                lblWaiter.Visible = false;
            }
        }

        private void btn_KOT_Click(object sender, EventArgs e)
        {
            ResetButtonColors();
            btn_KOT.BackColor = Color.FromArgb(249, 89, 89);
            btn_KOT.ForeColor = Color.FromArgb(227, 227, 227);
            if (dataGridView1.Rows.Count == 0 || (dataGridView1.Rows.Count == 1 && dataGridView1.Rows[0].IsNewRow))
            {
                MessageBox.Show("No products selected. Please add products.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(MainClass.con_string))
                {
                    con.Open();
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        
                        try
                        {
                            string qry1, qry2;

                            if (MainID == 0)
                            {
                                qry1 = @"INSERT INTO tblMain (aDate, aTime, TableName, WaiterName, status, orderType, total, received, change, driverID, CustName, CustPhone) 
                                VALUES (@aDate, @aTime, @TableName, @WaiterName, @status, @orderType, @total, @received, @change, @driverID, @CustName, @CustPhone);
                                SELECT SCOPE_IDENTITY()";
                            }
                            else
                            {
                                qry1 = @"UPDATE tblMain SET status = @status, total = @total, received = @received, change = @change WHERE MainID = @ID";
                            }

                            SqlCommand cmd1 = new SqlCommand(qry1, con, transaction);
                            cmd1.Parameters.AddWithValue("@ID", MainID);
                            cmd1.Parameters.AddWithValue("@aDate", Convert.ToDateTime(DateTime.Now.Date));
                            cmd1.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
                            cmd1.Parameters.AddWithValue("@TableName", lblTable.Text);
                            cmd1.Parameters.AddWithValue("@WaiterName", lblWaiter.Text);
                            cmd1.Parameters.AddWithValue("@status", "Pending");
                            cmd1.Parameters.AddWithValue("@orderType", OrderType);
                            cmd1.Parameters.AddWithValue("@total", Convert.ToDouble(lblTotal.Text));
                            cmd1.Parameters.AddWithValue("@received", Convert.ToDouble(0));
                            cmd1.Parameters.AddWithValue("@change", Convert.ToDouble(0));
                            cmd1.Parameters.AddWithValue("@driverID", driverID);
                            cmd1.Parameters.AddWithValue("@CustName", customerName);
                            cmd1.Parameters.AddWithValue("@CustPhone", customerPhone);

                            if (MainID == 0)
                            {
                                MainID = Convert.ToInt32(cmd1.ExecuteScalar());
                            }
                            else
                            {
                                cmd1.ExecuteNonQuery();
                            }

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (row.IsNewRow || row.Cells["dgvproID"].Value == null) continue;

                                int productId = Convert.ToInt32(row.Cells["dgvproID"].Value);
                                int orderQty = Convert.ToInt32(row.Cells["dgvQty"].Value);

                                // Kiểm tra số lượng trong kho
                                string qryStockCheck = "SELECT pStock FROM products WHERE pID = @id";
                                SqlCommand cmdStockCheck = new SqlCommand(qryStockCheck, con, transaction);
                                cmdStockCheck.Parameters.AddWithValue("@id", productId);

                                int currentStock = Convert.ToInt32(cmdStockCheck.ExecuteScalar());

                                if (orderQty > currentStock)
                                {
                                    transaction.Rollback();
                                    MessageBox.Show(
                                        $"Product {row.Cells["dgvPName"].Value} not enough raw materials in stock.\nQuantity: {currentStock}, Number of requests: {orderQty}.",
                                        "Warning",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning
                                    );
                                    return;
                                }

                                // Cập nhật tblDetails
                                int detailID = Convert.ToInt32(row.Cells["dgvid"].Value);
                                qry2 = detailID == 0
                                    ? @"INSERT INTO tblDetails (MainID, proID, qty, price, amount) 
                                       VALUES (@MainID, @proID, @qty, @price, @amount)"
                                    : @"UPDATE tblDetails SET qty = @qty, price = @price, amount = @amount WHERE DetailID = @ID";

                                SqlCommand cmd2 = new SqlCommand(qry2, con, transaction);
                                cmd2.Parameters.AddWithValue("@ID", detailID);
                                cmd2.Parameters.AddWithValue("@MainID", MainID);
                                cmd2.Parameters.AddWithValue("@proID", productId);
                                cmd2.Parameters.AddWithValue("@qty", orderQty);
                                cmd2.Parameters.AddWithValue("@price", Convert.ToDouble(row.Cells["dgvPrice"].Value));
                                cmd2.Parameters.AddWithValue("@amount", Convert.ToDouble(row.Cells["dgvAmount"].Value));
                                cmd2.ExecuteNonQuery();

                                // Cập nhật kho
                                string qryStockUpdate = "UPDATE products SET pStock = pStock - @qty WHERE pID = @id";
                                SqlCommand cmdStockUpdate = new SqlCommand(qryStockUpdate, con, transaction);
                                cmdStockUpdate.Parameters.AddWithValue("@qty", orderQty);
                                cmdStockUpdate.Parameters.AddWithValue("@id", productId);
                                cmdStockUpdate.ExecuteNonQuery();
                            }

                            UpdateTableStatus(con, transaction, lblTable.Text, false);
                            transaction.Commit();

                            MessageBox.Show("Saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            MainID = 0;
                            dataGridView1.Rows.Clear();
                            lblTable.Text = "";
                            lblWaiter.Text = "";
                            lblTable.Visible = false;
                            lblWaiter.Visible = false;
                            lblTotal.Text = "0.0";
                            lblDriverName.Text = "";
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Insufficient raw materials in stock", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateTableStatus(SqlConnection con, SqlTransaction transaction, string tableName, bool isAvailable)
        {
            string qry = @"UPDATE tables SET isAvailable = @isAvailable WHERE tName = @tableName";
            SqlCommand cmd = new SqlCommand(qry, con, transaction);
            cmd.Parameters.AddWithValue("@isAvailable", isAvailable ? 1 : 0);
            cmd.Parameters.AddWithValue("@tableName", tableName);
            cmd.ExecuteNonQuery();
        }
        private void MarkTableAsAvailable(string tableName)
        {
            using (SqlConnection con = new SqlConnection(MainClass.con_string))
            {
                string qry = @"UPDATE tables SET isAvailable = 1 WHERE tName = @tableName";
                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.Parameters.AddWithValue("@tableName", tableName);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public int id = 0;

        private void btn_bill_Click(object sender, EventArgs e)
        {
            ResetButtonColors();
            btn_bill.BackColor = Color.FromArgb(249, 89, 89);
            btn_bill.ForeColor = Color.FromArgb(227, 227, 227);

            frmBillList frm = new frmBillList();
            MainClass.BlurBackground(frm);

            if (frm.MainID > 0)
            {
                id = frm.MainID;
                MainID = frm.MainID;
                LoadEntries();
            }

        }
        private void LoadEntries()
        {

            try
            {
                string qry = @"SELECT m.MainID, m.TableName, m.WaiterName, m.orderType, 
                              d.DetailID, d.proID, p.pName, d.qty, d.price, d.amount 
                       FROM tblMain m 
                       INNER JOIN tblDetails d ON m.MainID = d.MainID
                       INNER JOIN products p ON p.pID = d.proID 
                       WHERE m.MainID = @MainID";

                SqlCommand cmd2 = new SqlCommand(qry, MainClass.con);
                cmd2.Parameters.AddWithValue("@MainID", id);
                DataTable dt2 = new DataTable();
                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                da2.Fill(dt2);

                dataGridView1.Rows.Clear();

                int rowNumber = 1;

                if (dt2.Rows.Count > 0)
                {
                    lblTable.Text = dt2.Rows[0]["TableName"].ToString();
                    lblWaiter.Text = dt2.Rows[0]["WaiterName"].ToString();

                    string orderType = dt2.Rows[0]["orderType"].ToString();
                    lblTable.Visible = orderType != "Delivery" && orderType != "Take away";
                    lblWaiter.Visible = lblTable.Visible;
                }
                else
                {
                    MessageBox.Show("No data found for this order.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }



                foreach (DataRow item in dt2.Rows)
                {
                    string detailid = item["DetailID"].ToString();
                    string proName = item["pName"].ToString();
                    string proid = item["proID"].ToString();
                    string qty = item["qty"].ToString();
                    string price = item["price"].ToString();
                    string amount = item["amount"].ToString();

                    object[] obj = { rowNumber++, detailid, proid, proName, qty, price, amount };
                    dataGridView1.Rows.Add(obj);
                }
                GetTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading entries: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        
        private void btn_checkout_Click(object sender, EventArgs e)
        {

            

            if (MainID <= 0)
            {
                MessageBox.Show("No order selected for checkout.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            frmCheckout frm = new frmCheckout
            {
                MainID = MainID,
                amt = Convert.ToDouble(lblTotal.Text)
            };
            frm.ShowDialog();
            frmPOS posForm = new frmPOS();
            posForm.MarkTableAsAvailable(lblTable.Text);
            MainID = 0;
            dataGridView1.Rows.Clear();
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblTotal.Text = "0.0";
        }

        private void btn_hold_Click(object sender, EventArgs e)
        {
            ResetButtonColors();
            btn_hold.BackColor = Color.FromArgb(249, 89, 89);
            btn_hold.ForeColor = Color.FromArgb(227, 227, 227);
            string qry1 = ""; // Main table
            string qry2 = ""; // detail table

            int detailID = 0;

            if (OrderType == "") 
            {
                MessageBox.Show(" Please select order type ");
                return;
            }

            if (MainID == 0) // insert
            {
                qry1 = @"insert into tblMain values(@aDate,@aTime,@TableName,@WaiterName,@status,@orderType,@total,@received,@change,@driverID,@CustName,@CustPhone);
                        select SCOPE_IDENTITY()";
            }

            else // update
            {
                qry1 = @"update tblMain set status = @status, total = @total, received = @received, change =@change where MainID = @ID";

            }



            SqlCommand cmd = new SqlCommand(qry1, MainClass.con);
            cmd.Parameters.AddWithValue("@ID", MainID);
            cmd.Parameters.AddWithValue("@aDate", Convert.ToDateTime(DateTime.Now.Date));
            cmd.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmd.Parameters.AddWithValue("@TableName", lblTable.Text);
            cmd.Parameters.AddWithValue("@WaiterName", lblWaiter.Text);
            cmd.Parameters.AddWithValue("@status", "Hold");
            cmd.Parameters.AddWithValue("@orderType", OrderType);
            cmd.Parameters.AddWithValue("@total", Convert.ToDouble(lblTotal.Text)); // chỉ save data cho kitchen , đc update khi thanh toán received
            cmd.Parameters.AddWithValue("@received", Convert.ToDouble(0));
            cmd.Parameters.AddWithValue("@change", Convert.ToDouble(0));
            cmd.Parameters.AddWithValue("@driverID", driverID);
            cmd.Parameters.AddWithValue("@CustName", customerName);
            cmd.Parameters.AddWithValue("@CustPhone", customerPhone);

            if (MainClass.con.State == ConnectionState.Closed)
            {
                MainClass.con.Open();
            }
            if (MainID == 0)
            {
                MainID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                cmd.ExecuteNonQuery();
            }
            if (MainClass.con.State == ConnectionState.Open)
            {
                MainClass.con.Close();
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                detailID = Convert.ToInt32(row.Cells["dgvid"].Value);

                if (detailID == 0)
                {
                    qry2 = @"insert into tblDetails values (@MainID,@proID,@qty,@price,@amount)";
                }
                else
                {
                    qry2 = @"update tblDetails set proID = @proID, qty = @qty, price = @price, amount = @amount where DetailID = @ID";
                }

                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                cmd2.Parameters.AddWithValue("@ID", detailID);
                cmd2.Parameters.AddWithValue("@MainID", MainID);
                cmd2.Parameters.AddWithValue("@proID", Convert.ToInt32(row.Cells["dgvproID"].Value));
                cmd2.Parameters.AddWithValue("@qty", Convert.ToInt32(row.Cells["dgvQty"].Value));
                cmd2.Parameters.AddWithValue("@price", Convert.ToDouble(row.Cells["dgvPrice"].Value));
                cmd2.Parameters.AddWithValue("@amount", Convert.ToDouble(row.Cells["dgvAmount"].Value));

                if (MainClass.con.State == ConnectionState.Closed)
                {
                    MainClass.con.Open();
                }

                cmd2.ExecuteNonQuery();

                if (MainClass.con.State == ConnectionState.Open)
                {
                    MainClass.con.Close();
                }

                MessageBox.Show("Saved successfully");
                MainID = 0;
                detailID = 0;
                dataGridView1.Rows.Clear();

                lblTable.Text = "";
                lblWaiter.Text = "";
                lblTable.Visible = false;
                lblWaiter.Visible = false;
                lblTotal.Text = "0.0";
                lblDriverName.Text = "";
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "dgvqty")
            {
                try
                {

                    if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count)
                        return;

                    DataGridViewRow currentRow = dataGridView1.Rows[e.RowIndex];
                    int qty = Convert.ToInt32(currentRow.Cells["dgvqty"].Value);

                    if (qty < 0)
                    {
                        MessageBox.Show("Số lượng không thể nhỏ hơn 0. Vui lòng nhập lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        currentRow.Cells["dgvqty"].Value = 1;

                        return;
                    }

                    if (qty == 0)
                    {
                     
                        dataGridView1.Rows.RemoveAt(e.RowIndex);
                        UpdateRowNumbers();
                        
                        GetTotal();
                        return;
                    }

                   
                    double price = Convert.ToDouble(currentRow.Cells["dgvPrice"].Value); 
                    double amount = qty * price; 
                    currentRow.Cells["dgvAmount"].Value = amount;

           
                    GetTotal();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Giá trị nhập không hợp lệ. Vui lòng nhập một số nguyên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (ArgumentOutOfRangeException)
                {
               
                    MessageBox.Show("Không thể xóa dòng vì chỉ mục không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception)
                {
                    MessageBox.Show("Pls choose your favorite drink");
                }
            }
        }
        private void UpdateRowNumbers()
        {
            int rowNumber = 1;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue; 
                row.Cells["dgvSno"].Value = rowNumber; 
                rowNumber++;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
