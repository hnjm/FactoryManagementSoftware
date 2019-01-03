﻿using FactoryManagementSoftware.DAL;
using FactoryManagementSoftware.BLL;
using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using FactoryManagementSoftware.Module;
using System.Globalization;

namespace FactoryManagementSoftware.UI
{
    public partial class frmOrder : Form
    {
        
        public frmOrder()
        {
            InitializeComponent();
            userPermission = dalUser.getPermissionLevel(MainDashboard.USER_ID);

            if (userPermission >= MainDashboard.ACTION_LVL_TWO)
            {
                btnOrder.Show();
            }
            else
            {
                btnOrder.Hide();
            }

            bool result = dalMatUsed.Delete();

            if (!result)
            {
                MessageBox.Show("Failed to reset material used data");
            }
            else
            {
                loadOrderAlertData();
            }
                
        }


        #region variable declare

        private int selectedOrderID = -1;
        static public string finalOrderNumber;
        static public string receivedNumber;

        static public bool receivedReturn = false;
        static public bool orderApproved = false;

        private int userPermission = -1;

        readonly string headerIndex = "NO";
        readonly string headerCode = "CODE";
        readonly string headerName = "NAME";
        readonly string headerReadyStock = "READY STOCK";
        readonly string headerBalanceOne = "FORECAST 1 BALANCE";
        readonly string headerBalanceTwo = "FORECAST 2 BALANCE";
        readonly string headerBalanceThree = "FORECAST 3 BALANCE";

        DataGridViewAutoSizeColumnMode Fill = DataGridViewAutoSizeColumnMode.Fill;
        DataGridViewAutoSizeColumnMode DisplayedCells = DataGridViewAutoSizeColumnMode.DisplayedCells;
        #endregion

        #region create class object (database)
        custBLL uCust = new custBLL();
        custDAL dalCust = new custDAL();

        orderActionBLL uOrderAction = new orderActionBLL();
        orderActionDAL dalOrderAction = new orderActionDAL();
        ordBLL uOrd = new ordBLL();
        ordDAL dalOrd = new ordDAL();

        facBLL uFac = new facBLL();
        facDAL dalFac = new facDAL();

        itemBLL uItem = new itemBLL();
        itemDAL dalItem = new itemDAL();

        itemCatBLL uItemCat = new itemCatBLL();
        itemCatDAL dalItemCat = new itemCatDAL();

        trfCatBLL utrfCat = new trfCatBLL();
        trfCatDAL daltrfCat = new trfCatDAL();

        trfHistBLL utrfHist = new trfHistBLL();
        trfHistDAL daltrfHist = new trfHistDAL();

        facStockBLL uStock = new facStockBLL();
        facStockDAL dalStock = new facStockDAL();

        itemCustBLL uItemCust = new itemCustBLL();
        itemCustDAL dalItemCust = new itemCustDAL();

        joinBLL uJoin = new joinBLL();
        joinDAL dalJoin = new joinDAL();

        forecastBLL uForecast = new forecastBLL();
        forecastDAL dalForecast = new forecastDAL();

        materialUsedBLL uMatUsed = new materialUsedBLL();
        materialUsedDAL dalMatUsed = new materialUsedDAL();

        userDAL dalUser = new userDAL();

        Tool tool = new Tool();

        #endregion

        #region UI setting

        private void createOrderAlertDatagridview()
        {
            DataGridView dgv = dgvOrderAlert;
            dgv.Columns.Clear();

            tool.AddTextBoxColumns(dgv, headerIndex, headerIndex, DisplayedCells);
            tool.AddTextBoxColumns(dgv, headerCode, dalItem.ItemCode, Fill);
            tool.AddTextBoxColumns(dgv, headerName, dalItem.ItemName, Fill);
            tool.AddTextBoxColumns(dgv, headerReadyStock, dalItem.ItemQty, DisplayedCells);
            tool.AddTextBoxColumns(dgv, headerBalanceOne, headerBalanceOne, DisplayedCells);
            tool.AddTextBoxColumns(dgv, headerBalanceTwo, headerBalanceTwo, DisplayedCells);
            tool.AddTextBoxColumns(dgv, headerBalanceThree, headerBalanceThree, DisplayedCells);

            dgv.Columns[dalItem.ItemQty].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[headerBalanceOne].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[headerBalanceTwo].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[headerBalanceThree].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        #endregion

        #region Load or Reset Form

        private DataTable AddDuplicates(DataTable dt)
        {

            if (dt.Rows.Count > 0)
            {
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        break;
                    }
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (dt.Rows[i]["item_code"].ToString() == dt.Rows[j]["item_code"].ToString())
                        {
                            float readyStock = dalItem.getStockQty(dt.Rows[i]["item_code"].ToString());

                            float jQty = Convert.ToSingle(dt.Rows[j]["quantity_order"].ToString());
                            float iQty = Convert.ToSingle(dt.Rows[i]["quantity_order"].ToString());

                            float stillOne = readyStock - jQty;
                            float stllTwo = readyStock - iQty;

                            float actualBalance = readyStock - stillOne - stllTwo;
                            dt.Rows[j]["quantity_order"] = actualBalance.ToString();
                            dt.Rows[i].Delete();
                            break;
                        }
                    }
                }
                dt.AcceptChanges();

            }
            return dt;
        }

        private DataTable RemoveDuplicates(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        break;
                    }
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (dt.Rows[i]["item_code"].ToString() == dt.Rows[j]["item_code"].ToString())
                        {
                            dt.Rows[i].Delete();
                            break;
                        }
                    }
                }
                dt.AcceptChanges();

            }
            return dt;
        }

        private void frmOrder_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainDashboard.ordFormOpen = false;
        }

        private void frmOrder_Load(object sender, EventArgs e)
        {
            resetForm();
            cmbStatusSearch.SelectedIndex = 0;
        }

        private void resetForm()
        {
            loadOrderRecord();
            txtOrdSearch.Clear();
        }
        
        private void loadOrderRecord()
        {

            string keywords = txtOrdSearch.Text;

            //check if the keywords has value or not
            if (keywords != null)
            {
                string statusSearch = cmbStatusSearch.Text;
                DataTable dt = dalOrd.Search(keywords);
                dt.DefaultView.Sort = "ord_added_date DESC";
                DataTable sortedDt = dt.DefaultView.ToTable();
                dgvOrd.Rows.Clear();

                foreach (DataRow ord in sortedDt.Rows)
                {
                    if (statusSearch.Equals("ALL") || ord["ord_status"].ToString().Equals(statusSearch))
                    {
                        int n = dgvOrd.Rows.Add();
                        dgvOrd.Rows[n].Cells["ord_id"].Value = ord["ord_id"].ToString();
                        dgvOrd.Rows[n].Cells["ord_added_date"].Value = ord["ord_added_date"].ToString();
                        dgvOrd.Rows[n].Cells["ord_required_date"].Value = Convert.ToDateTime(ord["ord_required_date"]).ToString("dd/MM/yyyy"); ;
                        dgvOrd.Rows[n].Cells["ord_item_code"].Value = ord["ord_item_code"].ToString();
                        dgvOrd.Rows[n].Cells["item_name"].Value = ord["item_name"].ToString();
                        dgvOrd.Rows[n].Cells["ord_qty"].Value = ord["ord_qty"].ToString();
                        dgvOrd.Rows[n].Cells["ord_pending"].Value = ord["ord_pending"].ToString();
                        dgvOrd.Rows[n].Cells["ord_received"].Value = ord["ord_received"].ToString();
                        dgvOrd.Rows[n].Cells["ord_unit"].Value = ord["ord_unit"].ToString();
                        dgvOrd.Rows[n].Cells["ord_status"].Value = ord["ord_status"].ToString();
                    }
                }
            }
            else
            {
                //show all item from the database
                loadOrderRecord();
            }
            listPaint(dgvOrd);

        }

        private void refreshOrderRecord(int orderID)
        {
            loadOrderRecord();
            dgvOrd.ClearSelection();
            if (selectedOrderID != -1)
            {
                foreach (DataGridViewRow row in dgvOrd.Rows)
                {
                    if (Convert.ToInt32(row.Cells["ord_id"].Value.ToString()).Equals(orderID))
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }
        }

        private void statusColorSet(DataGridView dgv, int rowIndex)
        {
            string value = dgv.Rows[rowIndex].Cells["ord_status"].Value.ToString();
            Color foreColor = this.dgvOrd.DefaultCellStyle.ForeColor;

            if (value.Equals("REQUESTING"))
            {
                foreColor = Color.FromArgb(244, 170, 66);
                //foreColor = Color.FromArgb(251, 188, 5);
            }
            else if (value.Equals("CANCELLED"))
            {
                foreColor = Color.FromArgb(234, 67, 53);
            }
            else if (value.Equals("PENDING"))
            {
                foreColor = Color.FromArgb(66, 133, 244);
            }
            else if (value.Equals("RECEIVED"))
            {
                foreColor = Color.FromArgb(52, 168, 83);
            }

            //dgv.Rows[rowIndex].Cells["ord_status"].Style = new DataGridViewCellStyle { ForeColor = SystemColors.Control,BackColor = foreColor };
            dgv.Rows[rowIndex].Cells["ord_status"].Style.ForeColor = foreColor;
        }

        private void listPaint(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.BackgroundColor = Color.White;

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            dgv.Columns["ord_qty"].DefaultCellStyle.BackColor = Color.FromArgb(232, 244, 66);
            dgv.Columns["ord_pending"].DefaultCellStyle.BackColor = Color.FromArgb(66, 191, 244);
            dgv.Columns["ord_received"].DefaultCellStyle.BackColor = Color.FromArgb(66, 244, 161);

            dgv.Columns["ord_qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["ord_pending"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["ord_received"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dgv.RowTemplate.Height = 40;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                int n = row.Index;
                if(dgv == dgvOrd)
                {
                    statusColorSet(dgv, n);
                }
                
            }
            dgv.ClearSelection();
        }

        private DataTable getOrderStatusTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Status", typeof(string));
            dt.Rows.Add("Requesting");
            dt.Rows.Add("Cancelled");
            dt.Rows.Add("Approved");
            dt.Rows.Add("Received");
            return dt;
        }

        private void insertOrderAlertData()
        {
            string forecastOne = "forecast_one";
            string forecastTwo = "forecast_two";
            string forecastThree = "forecast_three";

            DataTable dt = dalItemCust.Select();

            dt = RemoveDuplicates(dt);
            if (dt.Rows.Count <= 0)
            {
                MessageBox.Show("no data under this record.");
            }
            else
            {
                int ForecastOneQty = 0;
                int ForecastTwoQty = 0;
                int ForecastThreeQty = 0;
                int outStock = 0;
                int readyStock = 0;
                int stillNeed = 0;
                int stillNeedTwo = 0;
                int stillNeedThree = 0;
                string itemCode;
                string currentMonth;

                int forecastIndex = 1;

                foreach (DataRow item in dt.Rows)
                {
                    readyStock = 0;
                    outStock = 0;
                    stillNeed = 0;
                    stillNeedTwo = 0;
                    stillNeedThree = 0;
                    ForecastOneQty = 0;
                    ForecastTwoQty = 0;
                    ForecastThreeQty = 0;

                    itemCode = item["item_code"].ToString();
                    currentMonth = item["forecast_current_month"].ToString();
                    currentMonth = DateTime.ParseExact(currentMonth, "MMMM", CultureInfo.CurrentCulture).Month.ToString();
                    string year = DateTime.Now.Year.ToString();

                    readyStock = Convert.ToInt32(dalItem.getStockQty(itemCode));

                    DataTable dt3 = daltrfHist.rangeItemToAllCustomerSearchByMonth(currentMonth, year, itemCode);

                    if (dt3.Rows.Count > 0)
                    {

                        foreach (DataRow outRecord in dt3.Rows)
                        {
                            if (outRecord["trf_result"].ToString().Equals("Passed"))
                            {
                                outStock += Convert.ToInt32(outRecord["trf_hist_qty"]);
                            }
                        }
                    }

                    DataTable dt4 = dalItemCust.itemCodeSearch(itemCode);

                    if (dt4.Rows.Count > 0)
                    {

                        foreach (DataRow outRecord in dt4.Rows)
                        {
                            ForecastOneQty += Convert.ToInt32(outRecord[forecastOne]);
                            ForecastTwoQty += Convert.ToInt32(outRecord[forecastTwo]);
                            ForecastThreeQty += Convert.ToInt32(outRecord[forecastThree]);
                        }
                    }

                    stillNeed = readyStock - ForecastOneQty + outStock;
                    stillNeedTwo = stillNeed - ForecastTwoQty;
                    stillNeedThree = stillNeedTwo - ForecastThreeQty;

                    if (tool.ifGotChild(itemCode))
                    {
                        DataTable dtJoin = dalJoin.parentCheck(itemCode);
                        foreach (DataRow Join in dtJoin.Rows)
                        {
                            uMatUsed.no = forecastIndex;
                            uMatUsed.item_code = Join["join_child_code"].ToString();

                            if (stillNeed < 0)
                            {
                                stillNeed *= -1;
                            }

                            if (stillNeedTwo < 0)
                            {
                                stillNeedTwo *= -1;
                            }

                            if (stillNeedThree < 0)
                            {
                                stillNeedThree *= -1;
                            }

                            uMatUsed.quantity_order = Convert.ToInt32(dalItem.getStockQty(uMatUsed.item_code)) - stillNeed;
                            uMatUsed.quantity_order_two = uMatUsed.quantity_order - stillNeedTwo;
                            uMatUsed.quantity_order_three = uMatUsed.quantity_order_two - stillNeedThree;

                            bool result = dalMatUsed.Insert(uMatUsed);
                            if (!result)
                            {
                                MessageBox.Show("failed to insert material used data");
                                return;
                            }
                            else
                            {
                                forecastIndex++;
                            }

                        }

                    }
                    else
                    {
                        uMatUsed.no = forecastIndex;
                        uMatUsed.item_code = itemCode;
                        uMatUsed.quantity_order = stillNeed;
                        uMatUsed.quantity_order_two = stillNeedTwo;
                        uMatUsed.quantity_order_three = stillNeedThree;

                        bool result = dalMatUsed.Insert(uMatUsed);
                        if (!result)
                        {
                            MessageBox.Show("failed to insert material used data");
                            return;
                        }
                        else
                        {
                            forecastIndex++;
                        }
                    }
                }
            }
            
        }

        private void loadOrderAlertData()
        {
            
            createOrderAlertDatagridview();
            insertOrderAlertData();

            int n = 0;
            int index = 1;
            string materialType = null;
            float wastagePercetage = 0;
            float itemWeight;

            int OrderQty = 0;
            int OrderQtyTwo = 0;
            int OrderQtyThree = 0;

            float totalMaterialUsed = 0;
            float totalMaterialUsedTwo = 0;
            float totalMaterialUsedThree = 0;

            float materialUsed = 0;
            float materialUsedTwo = 0;
            float materialUsedThree = 0;

            float wastageUsed = 0;
            float wastageUsedTwo = 0;
            float wastageUsedThree = 0;

           
            float readyStock = 0;
            float balanceOne = 0;
            float balanceTwo = 0;
            float balanceThree = 0;

            DataTable dt = dalMatUsed.Select();
            dt = AddDuplicates(dt);
            dt.DefaultView.Sort = "item_material ASC";
            dt = dt.DefaultView.ToTable();

            dgvOrderAlert.Rows.Clear();
            dgvOrderAlert.Refresh();
            foreach (DataRow item in dt.Rows)
            {
                materialUsed = 0;
                materialUsedTwo = 0;
                materialUsedThree = 0;

                wastageUsed = 0;
                wastageUsedTwo = 0;
                wastageUsedThree = 0;

                itemWeight = Convert.ToSingle(item["item_part_weight"].ToString());
                wastagePercetage = Convert.ToSingle(item["item_wastage_allowed"].ToString());

                OrderQty = Convert.ToInt32(item["quantity_order"].ToString());
                OrderQtyTwo = Convert.ToInt32(item["quantity_order_two"].ToString());
                OrderQtyThree = Convert.ToInt32(item["quantity_order_three"].ToString());

                if (OrderQty < 0 || OrderQtyTwo < 0 || OrderQtyThree < 0)
                {
                    if(OrderQty < 0)
                    {
                        materialUsed = OrderQty * itemWeight / 1000;
                        wastageUsed = materialUsed * wastagePercetage;
                    }

                    if (OrderQtyTwo < 0)
                    {
                        materialUsedTwo = OrderQtyTwo * itemWeight / 1000;
                        wastageUsedTwo = materialUsedTwo * wastagePercetage;
                    }

                    if (OrderQtyThree < 0)
                    {
                        materialUsedThree = OrderQtyThree * itemWeight / 1000;
                        wastageUsedThree = materialUsedThree * wastagePercetage;
                    }
                    

                    if (string.IsNullOrEmpty(materialType))
                    {
                        materialType = item["item_material"].ToString();

                        totalMaterialUsed = materialUsed + wastageUsed;
                        totalMaterialUsedTwo = materialUsedTwo + wastageUsedTwo;
                        totalMaterialUsedThree = materialUsedThree + wastageUsedThree;

                        readyStock = dalItem.getStockQty(materialType); 

                        n = dgvOrderAlert.Rows.Add();
                        dgvOrderAlert.Rows[n].Cells[headerIndex].Value = index;
                        dgvOrderAlert.Rows[n].Cells[dalItem.ItemCode].Value = item[dalItem.ItemMaterial].ToString();
                        dgvOrderAlert.Rows[n].Cells[dalItem.ItemQty].Value = readyStock;

                        index++;
                    }
                    else if (materialType.Equals(item["item_material"].ToString()))
                    {
                        //same data
                        totalMaterialUsed += materialUsed + wastageUsed;
                        totalMaterialUsedTwo += materialUsedTwo + wastageUsedTwo;
                        totalMaterialUsedThree += materialUsedThree + wastageUsedThree;
                    }
                    else
                    {
                        //first data


                        if (totalMaterialUsed < 0)
                        {
                            balanceOne = readyStock + totalMaterialUsed;
                        }
                        else
                        {
                            balanceOne = readyStock;
                        }

                        if (totalMaterialUsedTwo < 0)
                        {
                            balanceTwo = balanceOne + totalMaterialUsedTwo;
                        }
                        else
                        {
                            balanceTwo = balanceOne;
                        }

                        if (totalMaterialUsedThree < 0)
                        {
                            balanceThree = balanceTwo + totalMaterialUsedThree;
                        }
                        else
                        {
                            balanceThree = balanceTwo;
                        }

                        if (balanceOne < 0)
                        {
                            dgvOrderAlert.Rows[n].Cells[headerBalanceOne].Style.ForeColor = Color.Red;
                        }

                        if (balanceTwo < 0)
                        {
                            dgvOrderAlert.Rows[n].Cells[headerBalanceTwo].Style.ForeColor = Color.Red;
                        }

                        if (balanceThree < 0)
                        {
                            dgvOrderAlert.Rows[n].Cells[headerBalanceThree].Style.ForeColor = Color.Red;
                        }

                        dgvOrderAlert.Rows[n].Cells[headerBalanceOne].Value = balanceOne;
                        dgvOrderAlert.Rows[n].Cells[headerBalanceTwo].Value = balanceTwo;
                        dgvOrderAlert.Rows[n].Cells[headerBalanceThree].Value = balanceThree;

                        materialType = item["item_material"].ToString();

                        totalMaterialUsed = materialUsed + wastageUsed;
                        totalMaterialUsedTwo = materialUsedTwo + wastageUsedTwo;
                        totalMaterialUsedThree = materialUsedThree + wastageUsedThree;

                        readyStock = dalItem.getStockQty(materialType);

                        n = dgvOrderAlert.Rows.Add();
                        dgvOrderAlert.Rows[n].Cells[headerIndex].Value = index;
                        dgvOrderAlert.Rows[n].Cells[dalItem.ItemCode].Value = item[dalItem.ItemMaterial].ToString();
                        dgvOrderAlert.Rows[n].Cells[dalItem.ItemQty].Value = dalItem.getStockQty(materialType);
                        index++;

                    }
                }
                else
                {
                    materialUsed = 0;
                    materialUsedTwo = 0;
                    materialUsedThree = 0;

                    wastageUsed = 0;
                    wastageUsedTwo = 0;
                    wastageUsedThree = 0;

                }
            }

            // this is the last item
            totalMaterialUsed += materialUsed + wastageUsed;
            totalMaterialUsedTwo += materialUsedTwo + wastageUsedTwo;
            totalMaterialUsedThree += materialUsedThree + wastageUsedThree;

            if (totalMaterialUsed < 0)
            {
                balanceOne = readyStock + totalMaterialUsed;
            }
            else
            {
                balanceOne = readyStock;
            }

            if (totalMaterialUsedTwo < 0)
            {
                balanceTwo = balanceOne + totalMaterialUsedTwo;
            }
            else
            {
                balanceTwo = balanceOne;
            }

            if (totalMaterialUsedThree < 0)
            {
                balanceThree = balanceTwo + totalMaterialUsedThree;
            }
            else
            {
                balanceThree = balanceTwo;
            }

            if (balanceOne < 0)
            {
                dgvOrderAlert.Rows[n].Cells[headerBalanceOne].Style.ForeColor = Color.Red;
            }

            if (balanceTwo < 0)
            {
                dgvOrderAlert.Rows[n].Cells[headerBalanceTwo].Style.ForeColor = Color.Red;
            }

            if (balanceThree < 0)
            {
                dgvOrderAlert.Rows[n].Cells[headerBalanceThree].Style.ForeColor = Color.Red;
            }

            dgvOrderAlert.Rows[n].Cells[headerBalanceOne].Value = balanceOne;
            dgvOrderAlert.Rows[n].Cells[headerBalanceTwo].Value = balanceTwo;
            dgvOrderAlert.Rows[n].Cells[headerBalanceThree].Value = balanceThree;

            tool.listPaintGreyHeader(dgvOrderAlert);
            dgvOrderAlert.ClearSelection();
        }

        #endregion

        #region data Insert/Update/Search

        private void btnOrder_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor; // change cursor to hourglass type
            
            frmOrderRequest frm = new frmOrderRequest();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();//create new order

            if(frmOrderRequest.orderSuccess)
            {
                Cursor = Cursors.WaitCursor; // change cursor to hourglass type
                resetForm();
                frmOrderRequest.orderSuccess = false;
                Cursor = Cursors.Arrow; // change cursor to normal type
            }
            Cursor = Cursors.Arrow; // change cursor to normal type
        }

        private void txtOrdSearch_TextChanged(object sender, EventArgs e)
        {
            loadOrderRecord();
        }

        private void addOrderAction(string action, int orderID)
        {
            if(orderID < 0)//create a new action record after new order has created
            {
                //get the last record from tbl_ord
                DataTable lastRecord = dalOrd.lastRecordSelect();

                foreach (DataRow ord in lastRecord.Rows)
                {
                    uOrderAction.ord_id = Convert.ToInt32(ord["ord_id"]);
                    uOrderAction.added_date = Convert.ToDateTime(ord["ord_added_date"]);
                    uOrderAction.added_by = Convert.ToInt32(ord["ord_added_by"]); ;
                    uOrderAction.action = action;
                    uOrderAction.note = "";

                    if (!dalOrderAction.Insert(uOrderAction))
                    {
                        MessageBox.Show("Failed to create new action");
                    }
                }
            }
           else//action record exist,add another action
            {
                uOrderAction.ord_id = orderID;
                uOrderAction.added_date = DateTime.Now;
                uOrderAction.added_by = MainDashboard.USER_ID;
                uOrderAction.action = action;
                uOrderAction.note = "";

                if (!dalOrderAction.Insert(uOrderAction))
                {
                    MessageBox.Show("Failed to add new action");
                }
            }
        }

        #endregion

        #region order status change

        private void orderRequest(int orderID)
        {
            Cursor = Cursors.WaitCursor; // change cursor to hourglass type

            //get data from datagridview
            // item category, item code, item name, order id, 
            string id = "";
            string cat = "";
            string itemCode = "";
            string itemName = "";
            string qty = "";
            string unit = "";
            string date = "";
            DataTable dt = dalOrd.Select(orderID);

            if(dt.Rows.Count > 0)
            {
                foreach (DataRow ord in dt.Rows)
                {
                    id = ord["ord_id"].ToString();
                    cat = ord["item_cat"].ToString();
                    itemCode = ord["ord_item_code"].ToString();
                    itemName = ord["item_name"].ToString();
                    qty = ord["ord_qty"].ToString();
                    unit = ord["ord_unit"].ToString();
                    date = Convert.ToDateTime(ord["ord_required_date"]).ToString("dd/MM/yyyy");
                }
            }
            
            frmOrderRequest frm = new frmOrderRequest(id, cat, itemCode, itemName, qty, unit, date);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();//order request

            if (frmOrderRequest.orderSuccess)
            {
                Cursor = Cursors.WaitCursor; // change cursor to hourglass type
                frmOrderRequest.orderSuccess = false;
                resetForm();
                Cursor = Cursors.Arrow; // change cursor to normal type
            }

            refreshOrderRecord(selectedOrderID);

            Cursor = Cursors.Arrow; // change cursor to normal type
        }

        private void orderAprove(int rowIndex, int orderID)
        {
            if (userPermission >= MainDashboard.ACTION_LVL_THREE)
            {
                //get data from datagridview
                string itemCode = dgvOrd.Rows[rowIndex].Cells["ord_item_code"].Value.ToString();
                string itemName = dgvOrd.Rows[rowIndex].Cells["item_name"].Value.ToString();
                string requiredDate = dgvOrd.Rows[rowIndex].Cells["ord_required_date"].Value.ToString();
                string qty = dgvOrd.Rows[rowIndex].Cells["ord_qty"].Value.ToString();
                string unit = dgvOrd.Rows[rowIndex].Cells["ord_unit"].Value.ToString();

                //approve form
                frmOrderApprove frm = new frmOrderApprove(orderID.ToString(), requiredDate, itemName, itemCode, qty, unit);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();

                if (orderApproved)//if order approved from approve form, then change order status from requesting to pending
                {
                    dalItem.orderAdd(itemCode, finalOrderNumber);//add order qty to item
                    refreshOrderRecord(selectedOrderID);
                    orderApproved = false;
                } 
            }
            else
            {
                MessageBox.Show("Action denied. Please admin for this action.");
            }
        }

        private void orderReceive(int rowIndex, int orderID)
        {
            //get data from datagridview
            string itemCode = dgvOrd.Rows[rowIndex].Cells["ord_item_code"].Value.ToString();
            string itemName = dgvOrd.Rows[rowIndex].Cells["item_name"].Value.ToString();
            string requiredDate = dgvOrd.Rows[rowIndex].Cells["ord_required_date"].Value.ToString();
            string qty = dgvOrd.Rows[rowIndex].Cells["ord_qty"].Value.ToString();
            string received = dgvOrd.Rows[rowIndex].Cells["ord_received"].Value.ToString();
            string unit = dgvOrd.Rows[rowIndex].Cells["ord_unit"].Value.ToString();

            frmOrderReceive frm = new frmOrderReceive(orderID,itemCode,itemName,Convert.ToSingle(qty),Convert.ToSingle(received), unit);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();//stock in

            refreshOrderRecord(selectedOrderID);
        }

        private void orderCancel(int rowIndex, int orderID, string presentStatus)
        {
            Cursor = Cursors.WaitCursor; // change cursor to hourglass type

            DialogResult dialogResult = MessageBox.Show("Are you sure want to cancel this order?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                string itemCode = dgvOrd.Rows[rowIndex].Cells["ord_item_code"].Value.ToString();
                string itemName = dgvOrd.Rows[rowIndex].Cells["item_name"].Value.ToString();
                string requiredDate = dgvOrd.Rows[rowIndex].Cells["ord_required_date"].Value.ToString();
                string qty = dgvOrd.Rows[rowIndex].Cells["ord_qty"].Value.ToString();
                string unit = dgvOrd.Rows[rowIndex].Cells["ord_unit"].Value.ToString();
                float received = Convert.ToSingle(dgvOrd.Rows[rowIndex].Cells["ord_received"].Value);
                string pending = dgvOrd.Rows[rowIndex].Cells["ord_pending"].Value.ToString();

                if (received > 0)//if have received record under this order ,then need to return this item from stock before cancel this order
                {
                    dialogResult = MessageBox.Show(@"There is an existing received record under this order. Please undo this record before canceling.", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                    if (dialogResult == DialogResult.OK)
                    {
                        frmOrderActionHistory frm = new frmOrderActionHistory(orderID);
                        frm.StartPosition = FormStartPosition.CenterScreen;
                        frm.ShowDialog();//return item from stock

       
                    }
                }
                else
                {
                    uOrd.ord_id = orderID;
                    uOrd.ord_required_date = Convert.ToDateTime(requiredDate);
                    uOrd.ord_qty = Convert.ToSingle(qty);
                    uOrd.ord_pending = 0;
                    uOrd.ord_received = 0;
                    uOrd.ord_updated_date = DateTime.Now;
                    uOrd.ord_updated_by = 0;
                    uOrd.ord_item_code = itemCode;
                    uOrd.ord_note = "";
                    uOrd.ord_unit = unit;
                    uOrd.ord_status = "CANCELLED";

                    if (dalOrd.Update(uOrd))
                    {
                        if (!presentStatus.Equals("REQUESTING"))
                        {
                            dalItem.orderSubtract(itemCode, pending); //subtract order qty
                        }

                        dalOrderAction.orderCancel(orderID, "");
                    }
                    
                }
            }
            refreshOrderRecord(selectedOrderID);
            Cursor = Cursors.Arrow; // change cursor to normal type
        }

        #endregion

        #region  click / selected index changed action

        //handle the row selection on right click
        private void dgvOrd_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            Cursor = Cursors.WaitCursor; // change cursor to hourglass type

            //handle the row selection on right click
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && userPermission >= MainDashboard.ACTION_LVL_TWO)
            {
                ContextMenuStrip my_menu = new ContextMenuStrip();
                dgvOrd.CurrentCell = dgvOrd.Rows[e.RowIndex].Cells[e.ColumnIndex];
                // Can leave these here - doesn't hurt
                dgvOrd.Rows[e.RowIndex].Selected = true;
                dgvOrd.Focus();
                int rowIndex = dgvOrd.CurrentCell.RowIndex;

                try
                {
                    string result = dgvOrd.Rows[rowIndex].Cells["ord_status"].Value.ToString();

                    if (result.Equals("REQUESTING"))
                    {
                        my_menu.Items.Add("Approve").Name = "Approve";
                        my_menu.Items.Add("Cancel").Name = "Cancel";
                    }
                    else if (result.Equals("CANCELLED"))
                    {
                        my_menu.Items.Add("Request").Name = "Request";
                    }
                    else if (result.Equals("PENDING"))
                    {
                        my_menu.Items.Add("Receive").Name = "Receive";
                        my_menu.Items.Add("Cancel").Name = "Cancel";
                    }
                    else if (result.Equals("RECEIVED"))
                    {
                        my_menu.Items.Add("Cancel").Name = "Cancel";
                    }

                    my_menu.Show(Cursor.Position.X, Cursor.Position.Y);
                    contextMenuStrip1 = my_menu;
                    my_menu.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_ItemClicked);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            Cursor = Cursors.Arrow; // change cursor to normal type
        }

        //handle what will happen when item clicked after right click on datagridview
        private void my_menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Cursor = Cursors.WaitCursor; // change cursor to hourglass type
            
            DataGridView dgv = dgvOrd;
            string itemClicked = e.ClickedItem.Name.ToString();
            int rowIndex = dgv.CurrentCell.RowIndex;
            int orderID = Convert.ToInt32(dgv.Rows[rowIndex].Cells["ord_id"].Value);
            string presentStatus = dgv.Rows[rowIndex].Cells["ord_status"].Value.ToString();

            contextMenuStrip1.Hide();
            if(itemClicked.Equals("Request"))
            {
                orderRequest(orderID);
            }
            else if (itemClicked.Equals("Approve"))
            {
                orderAprove(rowIndex, orderID);
            }
            else if (itemClicked.Equals("Cancel"))
            {
                orderCancel(rowIndex, orderID, presentStatus);
            }
            else if (itemClicked.Equals("Receive"))
            {
                orderReceive(rowIndex, orderID);
            }

            Cursor = Cursors.Arrow; // change cursor to normal type

        }

        //show record action record when double click on datagridview
        private void dgvOrd_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Cursor = Cursors.WaitCursor; // change cursor to hourglass type
            //MessageBox.Show("double click");
            int rowIndex = dgvOrd.CurrentCell.RowIndex;
            if (rowIndex >= 0)
            {
                int orderID = Convert.ToInt32(dgvOrd.Rows[rowIndex].Cells["ord_id"].Value);

                DataTable dt = dalOrderAction.Select(orderID);
                if (dt.Rows.Count > 0)
                {
                    frmOrderActionHistory frm = new frmOrderActionHistory(orderID);
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.ShowDialog();//Item Edit
                }
                else
                {
                    MessageBox.Show("No action record under this order yet.");
                }
            }

            if (receivedReturn)//if order approved from approve form, then change order status from requesting to pending
            {
                refreshOrderRecord(selectedOrderID);
                receivedReturn = false;
            }
            Cursor = Cursors.Arrow; // change cursor to normal type
        }

        //clear selection when mouse click on empty space
        private void dgvOrd_MouseClick(object sender, MouseEventArgs e)
        {
            var ht = dgvOrd.HitTest(e.X, e.Y);

            if (ht.Type == DataGridViewHitTestType.None)
            {
                //clicked on grey area
                dgvOrd.ClearSelection();
            }
        }
        
        //clear selection when mouse click on empty space
        private void frmOrder_Click(object sender, EventArgs e)
        {
            dgvOrd.ClearSelection();
            dgvOrderAlert.ClearSelection();
        }

        //input text search
        private void cmbStatusSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadOrderRecord();
        }

        //sort data according number
        private void dgvOrd_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            //if (e.Column.Index == 5 || e.Column.Index == 6 || e.Column.Index == 7)
            //{
            //    e.SortResult = float.Parse(e.CellValue1.ToString()).CompareTo(float.Parse(e.CellValue2.ToString()));
            //    e.Handled = true;//pass by the default sorting
            //}
            object tempObject1 = e.CellValue1;
            object tempObject2 = e.CellValue2;
            if (!(tempObject1 is null) && !(tempObject2 is null))
            {
                if (float.TryParse(tempObject1.ToString(), out float tmp) && float.TryParse(tempObject2.ToString(), out tmp))
                {
                    e.SortResult = float.Parse(tempObject1.ToString()).CompareTo(float.Parse(tempObject2.ToString()));
                    e.Handled = true;//pass by the default sorting
                }
            }
        }

        #endregion


    }
}
