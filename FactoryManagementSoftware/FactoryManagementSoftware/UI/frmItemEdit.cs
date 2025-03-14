﻿using System;
using System.Data;
using FactoryManagementSoftware.BLL;
using FactoryManagementSoftware.DAL;
using System.Windows.Forms;
using FactoryManagementSoftware.Module;

namespace FactoryManagementSoftware.UI
{
    public partial class frmItemEdit : Form
    {
        public frmItemEdit()
        {
            InitializeComponent();

            loadItemCategoryData();
            loadMaterialTypeData();
            loadMasterBatchData();
        }

        public frmItemEdit(string itemCat)
        {
            InitializeComponent();

            loadItemCategoryData();
            loadMaterialTypeData();
            loadMasterBatchData();

            cmbCat.Text = itemCat;
        }

        public frmItemEdit(itemBLL u)
        {
            InitializeComponent();

            loadItemCategoryData();
            loadMaterialTypeData();
            loadMasterBatchData();
            InitialData(u);
            inputDisable();
        }

        #region OBJECT DECLARE

        itemBLL u = new itemBLL();
        itemDAL dalItem = new itemDAL();

        itemCatDAL dalItemCat = new itemCatDAL();

        materialBLL uMaterial = new materialBLL();
        materialDAL dalMaterial = new materialDAL();

        Tool tool = new Tool();

        #endregion

        #region LOAD DATA
        private void loadItemCategoryData()
        {
            DataTable dtItemCat = dalItemCat.Select();
            DataTable distinctTable = dtItemCat.DefaultView.ToTable(true, "item_cat_name");
            distinctTable.DefaultView.Sort = "item_cat_name ASC";
            cmbCat.DataSource = distinctTable;
            cmbCat.DisplayMember = "item_cat_name";
            cmbCat.SelectedIndex = -1;

            cbZeroCost.Visible = false;
        }

        private void loadMaterialTypeData()
        {
            DataTable dt = dalMaterial.catSearch("RAW Material");
            DataTable distinctTable = dt.DefaultView.ToTable(true, "material_code");
            distinctTable.DefaultView.Sort = "material_code ASC";
            cmbMaterialType.DataSource = distinctTable;
            cmbMaterialType.DisplayMember = "material_code";
            cmbMaterialType.SelectedIndex = -1;
        }

        private void loadMasterBatchData()
        {
            tool.loadMasterBatchToComboBox(cmbMasterBatch);
        }

        #endregion

        #region VALIDATION
        private bool Validation()
        {

            if (string.IsNullOrEmpty(cmbCat.Text))
            {

                errorProvider3.SetError(cmbCat, "Item Category Required");
            }

            if (string.IsNullOrEmpty(txtItemCode.Text))
            {

                errorProvider1.SetError(txtItemCode, "Item Code Required");
            }

            if (string.IsNullOrEmpty(txtItemName.Text))
            {

                errorProvider2.SetError(txtItemName, "Item Name Required");
            }

            bool result = false;
            if (!string.IsNullOrEmpty(txtItemCode.Text) && !string.IsNullOrEmpty(txtItemName.Text) && !string.IsNullOrEmpty(cmbCat.Text))
            {
                result = true;
            }

            return result;

        }

        private void InitialData()
        {
            cmbCat.SelectedIndex = -1;
            txtItemCode.Clear();
            txtItemName.Clear();
            cmbMaterialType.SelectedIndex = -1;
            cmbMasterBatch.SelectedIndex = -1;
            txtColor.Clear();

            txtQuoTon.Clear();
            txtBestTon.Clear();
            txtProTon.Clear();

            txtQuoCT.Clear();
            txtProCTFrom.Clear();
            txtProCTTo.Clear();
            txtCapacity.Clear();
            txtQuoPWPcs.Clear();
            txtQuoRWPcs.Clear();
            txtProPWPcs.Clear();
            txtProRWPcs.Clear();

            txtProPWShot.Clear();
            txtProRWShot.Clear();
            txtProCooling.Clear();
            txtWastageAllowed.Clear();
        }

        private void InitialData(itemBLL u)
        {
            if (!string.IsNullOrEmpty(u.item_code))
            {
                cmbCat.Text = u.item_cat;
                txtItemCode.Text = u.item_code;
                txtItemName.Text = u.item_name;

                cmbMaterialType.Text = u.item_material;
                cmbMasterBatch.Text = u.item_mb;
                txtMBRate.Text = u.item_mb_rate.ToString();
                txtColor.Text = u.item_color;

                txtQuoTon.Text = u.item_quo_ton.ToString();
                txtBestTon.Text = u.item_best_ton.ToString();
                txtProTon.Text = u.item_pro_ton.ToString();

                txtQuoCT.Text = u.item_quo_ct.ToString();
                txtProCTFrom.Text = u.item_pro_ct_from.ToString();
                txtProCTTo.Text = u.item_pro_ct_to.ToString();
                txtCapacity.Text = u.item_capacity.ToString();
                txtQuoPWPcs.Text = u.item_quo_pw_pcs.ToString();
                txtQuoRWPcs.Text = u.item_quo_rw_pcs.ToString();
                txtProPWPcs.Text = u.item_pro_pw_pcs.ToString();
                txtProRWPcs.Text = u.item_pro_rw_pcs.ToString();

                txtProPWShot.Text = u.item_pro_pw_shot.ToString();
                txtProRWShot.Text = u.item_pro_rw_shot.ToString();
                txtProCooling.Text = u.item_pro_cooling.ToString();
                txtWastageAllowed.Text = u.item_wastage_allowed.ToString();

                if(u.item_assembly.ToString() != null)
                {
                    if(u.item_assembly == 1)
                    {
                        cbAssembly.Checked = true;
                    }
                    else
                    {
                        cbAssembly.Checked = false;
                    }
                }
                else
                {
                    cbAssembly.Checked = false;
                }

                if (u.item_production.ToString() != null)
                {
                    if (u.item_production == 1)
                    {
                        cbProduction.Checked = true;
                    }
                    else
                    {
                        cbProduction.Checked = false;
                    }
                }
                else
                {
                    cbProduction.Checked = false;
                }


                if(!dalItem.getCatName(u.item_code).Equals("Part"))
                {
                    if(dalMaterial.checkIfZeroCost(u.item_code))
                    {
                        cbZeroCost.Checked = true;
                    }
                    else
                    {
                        cbZeroCost.Checked = false;
                    }
                }
            }
            else
            {
                cmbCat.Text = u.item_cat;
                InitialData();
            }
        }

        private bool IfProductsExists(String productCode)
        {
            DataTable dt;
            if(cmbCat.Text.Equals("Part"))
            {
                dt = dalItem.Search(productCode);
            }
            else
            {
                dt = dalMaterial.codeSearch(productCode);
            }
            

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }
        #endregion

        #region INSERT/UPDATE DATA
        private void updateItem()
        {
            //Update data
            u.item_code = txtItemCode.Text;
            u.item_name = txtItemName.Text;

            u.item_material = cmbMaterialType.Text;
            u.item_mb = cmbMasterBatch.Text;
            u.item_mb_rate = Convert.ToSingle(txtMBRate.Text);
            u.item_color = txtColor.Text;

            u.item_quo_ton = tool.Int_TryParse(txtQuoTon.Text);
            u.item_best_ton = tool.Int_TryParse(txtBestTon.Text);
            u.item_pro_ton = tool.Int_TryParse(txtProTon.Text);

            u.item_quo_ct = tool.Int_TryParse(txtQuoCT.Text);
            u.item_pro_ct_from = tool.Int_TryParse(txtProCTFrom.Text);
            u.item_pro_ct_to = tool.Int_TryParse(txtProCTTo.Text);
            u.item_capacity = tool.Int_TryParse(txtCapacity.Text);

            u.item_quo_pw_pcs = tool.Float_TryParse(txtQuoPWPcs.Text);
            u.item_quo_rw_pcs = tool.Float_TryParse(txtQuoRWPcs.Text);
            u.item_pro_pw_pcs = tool.Float_TryParse(txtProPWPcs.Text);
            u.item_pro_rw_pcs = tool.Float_TryParse(txtProRWPcs.Text);

            u.item_pro_pw_shot = tool.Float_TryParse(txtProPWShot.Text);
            u.item_pro_rw_shot = tool.Float_TryParse(txtProRWShot.Text);
            u.item_pro_cooling = tool.Int_TryParse(txtProCooling.Text);
            u.item_wastage_allowed = tool.Float_TryParse(txtWastageAllowed.Text);

            if (cbAssembly.Checked)
            {
                u.item_assembly = 1;
            }
            else
            {
                u.item_assembly = 0;
            }

            if (cbProduction.Checked)
            {
                u.item_production = 1;
            }
            else
            {
                u.item_production = 0;
            }

            u.item_unit = txtUnit.Text;
            u.unit_to_pcs_rate = float.TryParse(txtPCSRate.Text, out float PcsRate) ? PcsRate : 1;

            u.item_updtd_date = DateTime.Now;
            u.item_updtd_by = MainDashboard.USER_ID;
            //Updating data into database
            //bool success = dalItem.Update(u);
            bool success = dalItem.NewUpdate(u);
            //if data is updated successfully then the value = true else false
            if (success == true)
            {
                //data updated successfully
                MessageBox.Show("Item successfully updated ");
                this.Close();
            }
            else
            {
                //failed to update user
                MessageBox.Show("Failed to updated item");
            }
        }

        private void insertItem()
        {
            //Add data
            u.item_cat = cmbCat.Text;

            u.item_code = txtItemCode.Text;
            u.item_name = txtItemName.Text;

            u.item_material = cmbMaterialType.Text;
            u.item_mb = cmbMasterBatch.Text;
            u.item_mb_rate = Convert.ToSingle(txtMBRate.Text);
            u.item_color = txtColor.Text;

            u.item_quo_ton = tool.Int_TryParse(txtQuoTon.Text);
            u.item_best_ton = tool.Int_TryParse(txtBestTon.Text);
            u.item_pro_ton = tool.Int_TryParse(txtProTon.Text);

            u.item_quo_ct = tool.Int_TryParse(txtQuoCT.Text);
            u.item_pro_ct_from = tool.Int_TryParse(txtProCTFrom.Text);
            u.item_pro_ct_to = tool.Int_TryParse(txtProCTTo.Text);
            u.item_capacity = tool.Int_TryParse(txtCapacity.Text);

            u.item_quo_pw_pcs = tool.Float_TryParse(txtQuoPWPcs.Text);
            u.item_quo_rw_pcs = tool.Float_TryParse(txtQuoRWPcs.Text);
            u.item_pro_pw_pcs = tool.Float_TryParse(txtProPWPcs.Text);
            u.item_pro_rw_pcs = tool.Float_TryParse(txtProRWPcs.Text);

            u.item_pro_pw_shot = tool.Float_TryParse(txtProPWShot.Text);
            u.item_pro_rw_shot = tool.Float_TryParse(txtProRWShot.Text);
            u.item_pro_cooling = tool.Int_TryParse(txtProCooling.Text);

            if(float.TryParse(txtWastageAllowed.Text,out float i))
            {
                if(i <= 0)
                {
                    u.item_wastage_allowed = 0.05f;
                }
                else
                {
                    u.item_wastage_allowed = i;
                }
            }
            else
            {
                u.item_wastage_allowed = 0.05f;
            }

            if(cbAssembly.Checked)
            {
                u.item_assembly = 1;
            }
            else
            {
                u.item_assembly = 0;
            }

            if (cbProduction.Checked)
            {
                u.item_production = 1;
            }
            else
            {
                u.item_production = 0;
            }


            u.item_added_date = DateTime.Now;
            u.item_added_by = MainDashboard.USER_ID;

            //Inserting Data into Database
            bool success = dalItem.NewInsert(u);
            //If the data is successfully inserted then the value of success will be true else false
            if (success == true)
            {
                //Data Successfully Inserted
                //MessageBox.Show("Item successfully created");
                this.Close();
            }
            else
            {
                //Failed to insert data
                MessageBox.Show("Failed to add new item");
            }
        }

        private void updateMaterial()
        {
            //Update data
            uMaterial.material_cat = cmbCat.Text;
            uMaterial.material_code = txtItemCode.Text;
            uMaterial.material_name = txtItemName.Text;

            if (cbZeroCost.Checked)
            {
                uMaterial.material_zero_cost = 1;
            }
            else
            {
                uMaterial.material_zero_cost = 0;
            }

            bool success = dalMaterial.Update(uMaterial);
            if (success == true)
            {
                //data updated successfully
                //MessageBox.Show("Material successfully updated ");
                updateItem();
                this.Close();
            }
            else
            {
                //failed to update user
                MessageBox.Show("Failed to updated material");
            }
        }

        private void insertMaterial()
        {
            //Add data
            uMaterial.material_cat = cmbCat.Text;
            uMaterial.material_code = txtItemCode.Text;
            uMaterial.material_name = txtItemName.Text;

            if (cbZeroCost.Checked)
            {
                uMaterial.material_zero_cost = 1;
            }
            else
            {
                uMaterial.material_zero_cost = 0;
            }

            bool success = dalMaterial.Insert(uMaterial);
            //If the data is successfully inserted then the value of success will be true else false
            if (success == true)
            {
                //Data Successfully Inserted
                //MessageBox.Show("Material successfully created");
                insertItem();
                this.Close();
            }
            else
            {
                //Failed to insert data
                dalMaterial.Delete(uMaterial);
                MessageBox.Show("Failed to add new material");
            }
        }
        #endregion

        #region BUTTON ACTION
        private void btnSave_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor; // change cursor to hourglass type
            if (Validation())
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure want to insert data to database?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    if (IfProductsExists(txtItemCode.Text))
                    {
                        if(cmbCat.Text.Equals("Part"))
                        {
                            updateItem();
                        }
                        else
                        {
                            updateMaterial();
                        }   
                    }
                    else
                    {
                        if (cmbCat.Text.Equals("Part"))
                        {
                            insertItem();
                        }
                        else
                        {
                            insertMaterial();
                        }
                    }
                }
            }
            Cursor = Cursors.Arrow; // change cursor to normal type
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region TEXT CHANGE/ INDEX CHANGE
        private void cmbCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cmbCat.Text))
            {
                errorProvider3.Clear();
                if(!cmbCat.Text.Equals("Part"))
                {
                    cbZeroCost.Visible = true;
                }
                else
                {
                    cbZeroCost.Visible = false;
                }
            }
        }

        private void txtItemCode_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtItemCode.Text))
            {
                errorProvider1.Clear();
            }

            if (cmbCat.Text.Equals("RAW Material") || cmbCat.Text.Equals("Master Batch"))
            {
                txtItemName.Text = txtItemCode.Text;
            }
        }

        private void txtItemName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtItemName.Text))
            {
                errorProvider2.Clear();
            }
        }
        #endregion

        #region KEY PRESS

        private void txtPartWeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void txtRunnerWeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void txtMcTon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void textBox12_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        #endregion

        private void inputDisable()
        {
            cmbCat.Enabled = false;
            txtItemCode.Enabled = false;
            txtItemName.Enabled = true;

            cmbMaterialType.Enabled = true;
            cmbMasterBatch.Enabled = true;
            txtColor.Enabled = true;
            txtQuoTon.Enabled = true;
            txtBestTon.Enabled = true;
            txtProTon.Enabled = true;
            txtQuoCT.Enabled = true;
            txtProCTFrom.Enabled = true;
            txtProCTTo.Enabled = true;
            txtCapacity.Enabled = true;
            txtQuoPWPcs.Enabled = true;
            txtQuoRWPcs.Enabled = true;
            txtProPWPcs.Enabled = true;
            txtProRWPcs.Enabled = true;
            txtProPWShot.Enabled = true;
            txtProRWShot.Enabled = true;
            txtProCooling.Enabled = true;
            txtWastageAllowed.Enabled = true;

            if (string.IsNullOrEmpty(cmbCat.Text))
            {
                cmbCat.Enabled = true;
                txtItemCode.Enabled = true;
            }
            else if (!cmbCat.Text.Equals("Part"))
            {
                cmbMaterialType.Enabled = false;
                cmbMasterBatch.Enabled = false;
                txtColor.Enabled = false;
                txtQuoTon.Enabled = false;
                txtBestTon.Enabled = false;
                txtProTon.Enabled = false;
                txtQuoCT.Enabled = false;
                txtProCTFrom.Enabled = false;
                txtProCTTo.Enabled = false;
                txtCapacity.Enabled = false;
                txtQuoPWPcs.Enabled = false;
                txtQuoRWPcs.Enabled = false;
                txtProPWPcs.Enabled = false;
                txtProRWPcs.Enabled = false;
                txtProPWShot.Enabled = false;
                txtProRWShot.Enabled = false;
                txtProCooling.Enabled = false;
                txtWastageAllowed.Enabled = true;
            }
        }

        private void cbPigment_CheckedChanged(object sender, EventArgs e)
        {
            if(cbPigment.Checked)
            {
                cbMB.Checked = false;
                tool.loadPigmentToComboBox(cmbMasterBatch);
            }

        }

        private void cbMB_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMB.Checked)
            {
                cbPigment.Checked = false;
                tool.loadMasterBatchToComboBox(cmbMasterBatch);
            }
        }

        private void tbMBRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void frmItemEdit_Load(object sender, EventArgs e)
        {

        }
    }
}
