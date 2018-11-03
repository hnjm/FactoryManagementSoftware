﻿using FactoryManagementSoftware.BLL;
using FactoryManagementSoftware.DAL;
using FactoryManagementSoftware.UI;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace FactoryManagementSoftware
{
    public partial class frmItem : Form
    {
        
        itemBLL uItem = new itemBLL();
        itemDAL dalItem = new itemDAL();

        materialDAL dalMaterial = new materialDAL();

        itemCatDAL dALItemCat = new itemCatDAL();

        private bool formLoaded = false;
        private int currentRowIndex;
        static public string currentItemCode;
        static public string currentItemName;
        static public string currentItemColor;
        static public string currentItemCat;
        static public string currentItemPartWeight;
        static public string currentItemRunnerWeight;
        static public string currentMaterial;
        static public string currentMB;

        #region Load or Reset Form

        public frmItem()
        {
            InitializeComponent();
        }

        private void frmItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainDashboard.itemFormOpen = false;
        }

        private void frmItem_Load(object sender, EventArgs e)
        {
            loadItemCategoryData();
            resetForm();
            cmbCat.SelectedIndex = -1;
            formLoaded = true;
        }

        private void loadItemData()
        {  
            DataTable dt = dalItem.Select();
            dgvItem.Rows.Clear();
            
                foreach (DataRow item in dt.Rows)
                {
                    float partf = 0;
                    float runnerf = 0;
                    int n = dgvItem.Rows.Add();

                    if (!string.IsNullOrEmpty(item["item_part_weight"].ToString()))
                    {
                        partf = Convert.ToSingle(item["item_part_weight"]);
                    }

                    if (!string.IsNullOrEmpty(item["item_runner_weight"].ToString()))
                    {
                        runnerf = Convert.ToSingle(item["item_runner_weight"]);
                    }

                    dgvItem.Rows[n].Cells["Category"].Value = item["item_cat"].ToString();
                    dgvItem.Rows[n].Cells["item_color"].Value = item["item_color"].ToString();
                    dgvItem.Rows[n].Cells["item_part_weight"].Value = partf.ToString("0.00");
                    dgvItem.Rows[n].Cells["item_runner_weight"].Value = runnerf.ToString("0.00");
                    dgvItem.Rows[n].Cells["dgvcItemCode"].Value = item["item_code"].ToString();
                    dgvItem.Rows[n].Cells["dgvcItemName"].Value = item["item_name"].ToString();
                    dgvItem.Rows[n].Cells["item_material"].Value = dalItem.getMaterialName(item["item_material"].ToString());
                    dgvItem.Rows[n].Cells["item_mb"].Value = dalItem.getMBName(item["item_mb"].ToString());
                    dgvItem.Rows[n].Cells["item_mc"].Value = item["item_mc"].ToString();
                    dgvItem.Rows[n].Cells["dgvcQty"].Value = item["item_qty"].ToString();
                    dgvItem.Rows[n].Cells["dgvcOrd"].Value = item["item_ord"].ToString();
                }
                listPaint();
          
            if(dt.Rows.Count <= 0 && formLoaded)
            {
                MessageBox.Show("no data under this record");
            }        
        }

        private void loadMaterialData()
        {
            string category = cmbCat.Text;
            DataTable dt = dalMaterial.catSearch(category);

            dgvItem.Rows.Clear();
            foreach (DataRow item in dt.Rows)
            {
                int n = dgvItem.Rows.Add();
                dgvItem.Rows[n].Cells["Category"].Value = item["material_cat"].ToString();
                dgvItem.Rows[n].Cells["dgvcItemCode"].Value = item["material_code"].ToString();
                dgvItem.Rows[n].Cells["dgvcItemName"].Value = item["material_name"].ToString();
            }
            listPaint();
         
            if (dt.Rows.Count <= 0 && formLoaded)
            {
                MessageBox.Show("no data under this record");
            }
            
        }

        private void loadData()
        {
            if (!string.IsNullOrEmpty(cmbCat.Text))
            {
                if (cmbCat.Text.Equals("Part"))
                {
                    loadItemData();
                }
                else
                {
                    loadMaterialData();
                }
            }
        }

        private void loadItemCategoryData()
        {
            DataTable dtItemCat = dALItemCat.Select();

            DataTable distinctTable = dtItemCat.DefaultView.ToTable(true, "item_cat_name");

            distinctTable.DefaultView.Sort = "item_cat_name ASC";    
            cmbCat.DataSource = distinctTable;
            cmbCat.DisplayMember = "item_cat_name";

        }

        private void resetForm()
        {           
            currentRowIndex = -1;
            currentItemCode = null;
            currentItemName = null;
            currentItemCat = null;
            currentItemColor = null;
            currentItemPartWeight = null;
            currentItemRunnerWeight = null;
            currentMaterial = null;
            currentMB = null;
            loadData();
        }

        #endregion

        #region Function: Insert/Delete/Reset/Search

        private void dgvItem_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            currentRowIndex = e.RowIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmItemEdit frm = new frmItemEdit();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();//Item Edit

            resetForm();
        }

        private void btnDelete2_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor; // change cursor to hourglass type
            if (dgvItem.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure want to delete?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    uItem.item_code = dgvItem.Rows[currentRowIndex].Cells["dgvcItemCode"].Value.ToString(); ;

                    bool success = dalItem.Delete(uItem);

                    if (success == true)
                    {
                        //item deleted successfully
                        MessageBox.Show("Item deleted successfully");
                        resetForm();
                    }
                    else
                    {
                        //Failed to delete item
                        MessageBox.Show("Failed to delete item");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a data");
            }
            Cursor = Cursors.Arrow; // change cursor to normal type
        }

        private void dgvItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            currentRowIndex = e.RowIndex;

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvItem.SelectedRows.Count > 0)
            {
                if (cmbCat.Text.Equals("Part"))
                {
                    currentItemCat = dgvItem.Rows[currentRowIndex].Cells["Category"].Value.ToString();
                    currentItemCode = dgvItem.Rows[currentRowIndex].Cells["dgvcItemCode"].Value.ToString();
                    currentItemName = dgvItem.Rows[currentRowIndex].Cells["dgvcItemName"].Value.ToString();
                    currentItemColor = dgvItem.Rows[currentRowIndex].Cells["item_color"].Value.ToString();
                    currentItemPartWeight = Convert.ToSingle(dgvItem.Rows[currentRowIndex].Cells["item_part_weight"].Value).ToString("0.00");
                    currentItemRunnerWeight = Convert.ToSingle(dgvItem.Rows[currentRowIndex].Cells["item_runner_weight"].Value).ToString("0.00");
                    currentMaterial = dgvItem.Rows[currentRowIndex].Cells["item_material"].Value.ToString();
                    currentMB = dgvItem.Rows[currentRowIndex].Cells["item_mb"].Value.ToString();

                    frmItemEdit frm = new frmItemEdit();
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.ShowDialog();//Item Edit

                    resetForm(); 
                }
                else
                {
                    currentItemCat = dgvItem.Rows[currentRowIndex].Cells["Category"].Value.ToString();
                    currentItemCode = dgvItem.Rows[currentRowIndex].Cells["dgvcItemCode"].Value.ToString();
                    currentItemName = dgvItem.Rows[currentRowIndex].Cells["dgvcItemName"].Value.ToString();
                   
                    frmItemEdit frm = new frmItemEdit();
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.ShowDialog();//Item Edit

                    resetForm();
                }
            }
            else
            {
                MessageBox.Show("Please select a data");
            }

        }

        private void txtItemSearch_TextChanged(object sender, EventArgs e)
        {
            DataTable dt;
            string keywords = txtItemSearch.Text;
            string category = cmbCat.Text;
            

           
            if (keywords != null && category != null)
            {
                if (cmbCat.Text.Equals("ALL"))
                {
                    dt = dalItem.Search(keywords);
                }
                else
                {
                    dt = dalItem.catItemSearch(keywords, category);
                }

                dgvItem.Rows.Clear();
                foreach (DataRow item in dt.Rows)
                {
                    float partf = 0;
                    float runnerf = 0;
                    int n = dgvItem.Rows.Add();
                    if (!string.IsNullOrEmpty(item["item_part_weight"].ToString()))
                    {
                        partf = Convert.ToSingle(item["item_part_weight"]);
                    }
                    if (!string.IsNullOrEmpty(item["item_runner_weight"].ToString()))
                    {
                        partf = Convert.ToSingle(item["item_runner_weight"]);
                    }

                    dgvItem.Rows[n].Cells["Category"].Value = item["item_cat"].ToString();
                    dgvItem.Rows[n].Cells["dgvcItemCode"].Value = item["item_code"].ToString();
                    dgvItem.Rows[n].Cells["dgvcItemName"].Value = item["item_name"].ToString();
                    dgvItem.Rows[n].Cells["item_color"].Value = item["item_color"].ToString();
                    dgvItem.Rows[n].Cells["item_material"].Value = item["item_material"].ToString();
                    dgvItem.Rows[n].Cells["item_mb"].Value = item["item_mb"].ToString();
                    dgvItem.Rows[n].Cells["item_mc"].Value = item["item_mc"].ToString();
                    dgvItem.Rows[n].Cells["item_part_weight"].Value =partf.ToString("0.00");
                    dgvItem.Rows[n].Cells["item_runner_weight"].Value = runnerf.ToString("0.00");
                    dgvItem.Rows[n].Cells["dgvcQty"].Value = item["item_qty"].ToString();
                    dgvItem.Rows[n].Cells["dgvcOrd"].Value = item["item_ord"].ToString();
                }

            }
            else
            {
                //show all item from the database
                loadData();
            }

            bool rowColorChange = true;
            foreach (DataGridViewRow row in dgvItem.Rows)
            {
                int n = row.Index;
                if (rowColorChange)
                {
                    dgvItem.Rows[n].DefaultCellStyle.BackColor = Control.DefaultBackColor;
                    rowColorChange = false;
                }
                else
                {
                    dgvItem.Rows[n].DefaultCellStyle.BackColor = Color.White;
                    rowColorChange = true;
                }
            }
            dgvItem.ClearSelection();
        }

        private void cmbCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (formLoaded)
            {
                loadData();
            }
           
        }


        #endregion

  

        private void listPaint()
        {
            bool rowColorChange = true;
            foreach (DataGridViewRow row in dgvItem.Rows)
            {
                int n = row.Index;
                if (rowColorChange)
                {
                    dgvItem.Rows[n].DefaultCellStyle.BackColor = Control.DefaultBackColor;
                    rowColorChange = false;
                }
                else
                {
                    dgvItem.Rows[n].DefaultCellStyle.BackColor = Color.White;
                    rowColorChange = true;
                }
            }
            dgvItem.ClearSelection();
        }

        private void dgvItem_Sorted(object sender, EventArgs e)
        {
            
            listPaint();
        }
    }

}
