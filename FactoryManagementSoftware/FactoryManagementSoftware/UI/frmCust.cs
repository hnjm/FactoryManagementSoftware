﻿using FactoryManagementSoftware.BLL;
using FactoryManagementSoftware.DAL;
using FactoryManagementSoftware.UI;
using System;
using System.Data;
using System.Windows.Forms;

namespace CusttoryManagementSoftware.UI
{
    public partial class frmCust : Form
    {
        public frmCust()
        {
            InitializeComponent();
        }

        custBLL uCust = new custBLL();
        custDAL dalCust = new custDAL();

        #region Load or Reset Form

        private void frmCust_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainDashboard.custFormOpen = false;
        }

        private void frmCust_Load(object sender, EventArgs e)
        {
            resetForm();
        }

        private void loadData()
        {
            DataTable dt = dalCust.Select();
            dgvCust.Rows.Clear();
            foreach (DataRow item in dt.Rows)
            {
                int n = dgvCust.Rows.Add();
                dgvCust.Rows[n].Cells["dgvcCustID"].Value = item["cust_id"].ToString();
                dgvCust.Rows[n].Cells["dgvcCustName"].Value = item["cust_name"].ToString();
            }
        }

        private void resetForm()
        {
            txtCustID.Clear();
            txtCustName.Clear();
            btnInsert.Text = "ADD";
            btnDelete.Hide();
            loadData();
            this.ActiveControl = txtCustName;
        }

        #endregion

        #region Data Grid View

        private void dgvCust_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //get the index of particular row
            int rowIndex = e.RowIndex;
            txtCustID.Text = dgvCust.Rows[rowIndex].Cells["dgvcCustID"].Value.ToString();
            txtCustName.Text = dgvCust.Rows[rowIndex].Cells["dgvcCustName"].Value.ToString();

            btnInsert.Text = "UPDATE";
            btnDelete.Show();

        }

        #endregion

        #region data validation

        private bool Validation()
        {
            bool result = false;
            if (string.IsNullOrEmpty(txtCustName.Text))
            {

                errorProvider2.SetError(txtCustName, "Customer name Required");
            }
            else
            {
                result = true;
            }

            return result;

        }

        private bool IfCustExists(String custID)
        {
            if (string.IsNullOrEmpty(txtCustID.Text))
            {
                return false;
            }
            else
            {
                DataTable dt = dalCust.idSearch(custID);
                if (dt.Rows.Count > 0)
                    return true;
                else
                    return false;
            }

        }

        private void txtCustName_TextChanged(object sender, EventArgs e)
        {
            errorProvider2.Clear();
        }

        #endregion

        #region Function: Insert/Delete/Reset/Search

        private void btnInsert_Click(object sender, EventArgs e)
        {

            if (Validation())
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure want to insert data to database?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    if (IfCustExists(txtCustID.Text))
                    {

                        //Update data
                        uCust.cust_id = Convert.ToInt32(txtCustID.Text);
                        uCust.cust_name = txtCustName.Text;
                        uCust.cust_updtd_date = DateTime.Now;
                        uCust.cust_updtd_by = 0;

                        //Updating data into database
                        bool success = dalCust.Update(uCust);

                        //if data is updated successfully then the value = true else false
                        if (success == true)
                        {
                            //data updated successfully
                            MessageBox.Show("Customer successfully updated ");
                            resetForm();
                        }
                        else
                        {
                            //failed to update user
                            MessageBox.Show("Failed to updated customer");
                        }
                    }
                    else
                    {
                        //Add data
                        uCust.cust_name = txtCustName.Text;
                        uCust.cust_added_date = DateTime.Now;
                        uCust.cust_added_by = 1;

                        //Inserting Data into Database
                        bool success = dalCust.Insert(uCust);
                        //If the data is successfully inserted then the value of success will be true else false
                        if (success == true)
                        {
                            //Data Successfully Inserted
                            MessageBox.Show("Customer successfully created");
                            resetForm();
                        }
                        else
                        {
                            //Failed to insert data
                            MessageBox.Show("Failed to add new customer");
                        }
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure want to delete?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                uCust.cust_id = Convert.ToInt32(txtCustID.Text);

                bool success = dalCust.Delete(uCust);

                if (success == true)
                {
                    //item deleted successfully
                    MessageBox.Show("Customer deleted successfully");
                    resetForm();
                }
                else
                {
                    //Failed to delete item
                    MessageBox.Show("Failed to delete customer");
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure want to reset?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                resetForm();
            }
        }

        private void txtCustSearch_TextChanged(object sender, EventArgs e)
        {
            //get keyword from text box
            string keywords = txtCustSearch.Text;

            //check if the keywords has value or not
            if (keywords != null)
            {
                //show user based on keywords
                DataTable dt = dalCust.Search(keywords);
                //dgvItem.DataSource = dt;
                dgvCust.Rows.Clear();
                foreach (DataRow cust in dt.Rows)
                {
                    int n = dgvCust.Rows.Add();
                    dgvCust.Rows[n].Cells["dgvcCustID"].Value = cust["cust_id"].ToString();
                    dgvCust.Rows[n].Cells["dgvcCustName"].Value = cust["cust_name"].ToString();
                }

            }
            else
            {
                //show all item from the database
                loadData();
            }
        }

        #endregion
    }
}
