﻿using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FactoryManagementSoftware.BLL;
using FactoryManagementSoftware.DAL;
using FactoryManagementSoftware.Module;

namespace FactoryManagementSoftware.UI
{
    public partial class frmProductionRecordNewV2 : Form
    {
        public frmProductionRecordNewV2()
        {
            InitializeComponent();
            AutoScroll = true;
            userPermission = dalUser.getPermissionLevel(MainDashboard.USER_ID);
            txtPlanID.Visible = true;
            //if (userPermission >= MainDashboard.ACTION_LVL_FOUR)
            //{
            //    lblRemoveCompleted.Show();
            //    lblClearList.Show();
            //}
            //else
            //{
            //    lblRemoveCompleted.Hide();
            //    lblClearList.Hide();
            //}

            loadPackingData();
            CreateMeterReadingData();

            tool.DoubleBuffered(dgvItemList, true);
            tool.DoubleBuffered(dgvRecordHistory, true);
            tool.DoubleBuffered(dgvMeterReading, true);

            dt_ItemInfo = dalItem.Select();
            dt_JoinInfo = dalJoin.SelectAll();
            dt_Mac = dalMac.Select();

        }

        Text text = new Text();
        Tool tool = new Tool();

        itemDAL dalItem = new itemDAL();
        planningDAL dalPlan = new planningDAL();
        PlanningBLL uPlan = new PlanningBLL();
        MacDAL dalMac = new MacDAL();
        MacBLL uMac = new MacBLL();
        userDAL dalUser = new userDAL();
        joinDAL dalJoin = new joinDAL();
        joinBLL uJoin = new joinBLL();
        ProductionRecordDAL dalProRecord = new ProductionRecordDAL();
        ProductionRecordBLL uProRecord = new ProductionRecordBLL();
        trfHistDAL dalTrf = new trfHistDAL();

        DataTable dt_Plan;
        DataTable dt_ProductionRecord;
        DataTable dt_ItemInfo;
        DataTable dt_JoinInfo;
        DataTable dt_MultiPackaging;
        DataTable dt_Carton;
        DataTable dt_MultiParent;
        DataTable dt_Mac;
       // DataTable dt_Trf;

        int userPermission = -1;
        readonly private string string_NewSheet = "NEW";
        readonly private string string_Multi = "MULTI";
        readonly private string string_MultiPackaging = "MULTI PACKAGING";

        readonly private string header_Time = "TIME";
        readonly private string header_Operator = "OPERATOR";
        readonly private string header_MeterReading = "METER READING";
        readonly private string header_Hourly = "HOURLY";

        readonly private string header_ProLotNo = "PRO LOT NO";
        readonly private string header_Machine = "MAC.";
        readonly private string header_Factory = "FAC.";
        readonly private string header_PlanID = "PLAN ID";
        readonly private string header_PartName = "NAME";
        readonly private string header_PartCode = "CODE";
        readonly private string header_Status = "STATUS";
        readonly private string header_SheetID = "SHEET ID";
        readonly private string header_ProductionDate = "PRO. DATE";
        readonly private string header_Shift = "SHIFT";
        readonly private string header_ProducedQty = "PRODUCED QTY";
        readonly private string header_StockIn = "STOCK IN";
        readonly private string header_UpdatedDate = "UPDATED DATE";
        readonly private string header_UpdatedBy = "UPDATED BY";
        readonly private string header_TotalProduced = "TOTAL PRODUCED";
        readonly private string header_TotalStockIn = "TOTAL STOCK IN";

        readonly static public string header_PackagingCode = "CODE";
        readonly static public string header_PackagingName = "NAME";
        readonly static public string header_PackagingQty = "CARTON QTY";
        readonly static public string header_PackagingMax = "PART QTY PER BOX";
        readonly static public string header_PackagingStockOut = "STOCK OUT";

        readonly private string text_RemoveRecord = "Remove from this list.";
        readonly private string text_ChangePlanID = "Change Plan To";
        readonly private string string_CheckBy= "CHECK BY: ";
        readonly private string string_CheckDate = "DATE: ";

        readonly string header_Cat = "Cat";
        readonly string header_ProDate = "DATE";
        readonly string header_ItemCode = "ITEM CODE";
        readonly string header_From = "FROM";
        readonly string header_To = "TO";
        readonly string header_Qty = "QTY";

        private int macID = 0;
        private int lotNo = 0;

        private bool loaded = false;
        private bool addingNewSheet = false;
        private bool sheetLoaded = false;
        private bool dailyRecordLoaded = false;
        private bool stopCheckedChange = false;
        private bool dataSaved = true;
        private bool balanceOutEdited = false;
        private bool balanceInEdited = false;

        private DataTable NewPackagingTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(header_PackagingCode, typeof(string));
            dt.Columns.Add(header_PackagingName, typeof(string));
            dt.Columns.Add(header_PackagingMax, typeof(int));
            dt.Columns.Add(header_PackagingQty, typeof(int));

            return dt;
        }

        private DataTable NewCartonTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(header_PackagingMax, typeof(int));
            dt.Columns.Add(header_PackagingCode, typeof(string));
            dt.Columns.Add(header_PackagingName, typeof(string));
            dt.Columns.Add(header_PackagingQty, typeof(int));
            dt.Columns.Add(header_PackagingStockOut, typeof(bool));

            return dt;
        }


        private DataTable NewItemListTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(header_Machine, typeof(int));
            dt.Columns.Add(header_Factory, typeof(string));
            dt.Columns.Add(header_PartName, typeof(string));
            dt.Columns.Add(header_PartCode, typeof(string));
            dt.Columns.Add(header_Status, typeof(string));
            dt.Columns.Add(header_PlanID, typeof(int));

            return dt;
        }

        private DataTable NewStockInTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(header_Cat, typeof(string));
            dt.Columns.Add(header_ProDate, typeof(string));
            dt.Columns.Add(header_ItemCode, typeof(string));
            dt.Columns.Add(header_From, typeof(string));
            dt.Columns.Add(header_To, typeof(string));
            dt.Columns.Add(header_Qty, typeof(string));
            dt.Columns.Add(header_PlanID, typeof(string));
            dt.Columns.Add(header_Shift, typeof(string));

            return dt;
        }

        private DataTable NewSheetRecordTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(header_SheetID, typeof(int));
            dt.Columns.Add(header_ProductionDate, typeof(DateTime));
            dt.Columns.Add(header_ProLotNo, typeof(string));
            dt.Columns.Add(header_Shift, typeof(string));
            dt.Columns.Add(header_ProducedQty, typeof(double));
            dt.Columns.Add(header_StockIn, typeof(double));
            dt.Columns.Add(header_UpdatedDate, typeof(DateTime));
            dt.Columns.Add(header_UpdatedBy, typeof(string));
            dt.Columns.Add(header_TotalProduced, typeof(double));
            dt.Columns.Add(header_TotalStockIn, typeof(double));

            return dt;
        }

        private DataTable NewMeterReadingTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(header_Time, typeof(string));
            dt.Columns.Add(header_Operator, typeof(string));
            dt.Columns.Add(header_MeterReading, typeof(double));
            dt.Columns.Add(header_Hourly, typeof(double));

            return dt;
        }

        private void dgvMeterStyleEdit(DataGridView dgv)
        {
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 6F, FontStyle.Regular);
            dgv.RowsDefaultCellStyle.Font = new Font("Segoe UI", 8F, FontStyle.Regular);
            dgv.Columns[header_Time].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[header_Operator].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[header_MeterReading].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[header_Hourly].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgv.Columns[header_Operator].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgv.Columns[header_Operator].DefaultCellStyle.BackColor = SystemColors.Info;
            dgv.Columns[header_MeterReading].DefaultCellStyle.BackColor = SystemColors.Info;
        }

        private void dgvItemListStyleEdit(DataGridView dgv)
        {
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8F, FontStyle.Regular);
            dgv.RowsDefaultCellStyle.Font = new Font("Segoe UI", 8F, FontStyle.Regular);
            dgv.Columns[header_Machine].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[header_Factory].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[header_PlanID].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[header_Status].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[header_PartName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.Columns[header_PartCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        private void dgvSheetRecordStyleEdit(DataGridView dgv)
        {
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 6F, FontStyle.Regular);
            dgv.RowsDefaultCellStyle.Font = new Font("Segoe UI", 8F, FontStyle.Regular);
            dgv.Columns[header_SheetID].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[header_ProductionDate].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[header_Shift].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[header_ProducedQty].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[header_UpdatedDate].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[header_UpdatedBy].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns[header_TotalProduced].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns[header_StockIn].DefaultCellStyle.BackColor = SystemColors.Info;
        }

        private void ClearAllError()
        {
            errorProvider1.Clear();
            errorProvider2.Clear();
            errorProvider3.Clear();
            errorProvider4.Clear();
            errorProvider5.Clear();
            errorProvider6.Clear();
            errorProvider7.Clear();
            errorProvider8.Clear();
            errorProvider9.Clear();
            errorProvider10.Clear();
        }

        private bool Validation()
        {
            ClearAllError();
            bool passed = true;

            string date = dtpProDate.Value.ToString();

            if(string.IsNullOrEmpty(date))
            {
                errorProvider1.SetError(lblDate, "Please select a production's date");
                passed = false;
            }

            if(CheckIfDateOrShiftDuplicate())
            {
                errorProvider1.SetError(lblDate, "Please select a different date");
                errorProvider2.SetError(lblShift, "Please select a different shift");
                
                passed = false;
            }

            if(!cbMorning.Checked && !cbNight.Checked)
            {
                errorProvider2.SetError(lblShift, "Please select shift");
                passed = false;
            }

            if(string.IsNullOrEmpty(lblPartName.Text))
            {
                errorProvider3.SetError(lblPartNameHeader, "Please select a item on the left table");
                passed = false;
            }

            if (string.IsNullOrEmpty(lblPartCode.Text))
            {
                errorProvider4.SetError(lblPartCodeHeader, "Please select a item on the left table");
                passed = false;
            }

            if (string.IsNullOrEmpty(txtMeterStart.Text))
            {
                errorProvider5.SetError(lblMeterStartAt, "Please input meter start value");
                passed = false;
            }

            if (string.IsNullOrEmpty(txtBalanceOfLastShift.Text))
            {
                errorProvider6.SetError(lblBalanceLast, "Please input balance of last shift");
                passed = false;
            }

            if (string.IsNullOrEmpty(txtBalanceOfThisShift.Text))
            {
                errorProvider7.SetError(lblBalanceCurrent, "Please input balance of this shift");
                passed = false;
            }

            if (string.IsNullOrEmpty(txtFullBox.Text))
            {
                errorProvider8.SetError(lblFullCarton, "Please input full carton qty");
                passed = false;
            }

            if (string.IsNullOrEmpty(txtTotalProduce.Text))
            {
                errorProvider9.SetError(lblTotalStockIn, "Please input total stock in qty");
                passed = false;
            }

            if (string.IsNullOrEmpty(txtPackingMaxQty.Text))
            {
                errorProvider10.SetError(lblPacking, "Please input total qty per box");
                passed = false;
            }

            return passed;
        }

        private void AddNewSheetUI(bool newSheet)
        {
            dgvMeterReading.SuspendLayout();
            ClearData();

            if (newSheet)
            {
                if (dgvItemList.SelectedRows.Count > 0)
                {
                    tlpSheet.RowStyles[0] = new RowStyle(SizeType.Percent, 0);
                    tlpSheet.RowStyles[1] = new RowStyle(SizeType.Percent, 100);

                    btnNewSheet.Visible = false;
                    dgvMeterReading.ResumeLayout();
                }
                    
            }
            else
            {
                
                sheetLoaded = false;
                addingNewSheet = false;
                dgvRecordHistory.ClearSelection();
                tlpSheet.RowStyles[0] = new RowStyle(SizeType.Percent, 100);
                tlpSheet.RowStyles[1] = new RowStyle(SizeType.Percent, 0);
                btnNewSheet.Visible = true;
                dgvMeterReading.ResumeLayout();
            }
          
        }

        private void LoadItemListData()
        {
            dt_ProductionRecord = dalProRecord.Select();
            DataGridView dgv = dgvItemList;
            DataTable dt = NewItemListTable();

            dt_Plan = dalPlan.Select();

            foreach (DataRow row in dt_Plan.Rows)
            {
                bool match = true;

                #region Recording Filtering

                if(row[dalPlan.recording] != DBNull.Value)
                {
                    bool recording = Convert.ToBoolean(row[dalPlan.recording]);

                    if(!recording)
                    {
                        match = false;
                    }
                }
                else
                {
                    match = false;
                }

                #endregion

                if (match)
                {
                    DataRow dt_Row = dt.NewRow();

                    dt_Row[header_Machine] = row[dalPlan.machineID];
                    dt_Row[header_Factory] = row[dalMac.MacLocation];
                    dt_Row[header_PlanID] = row[dalPlan.planID];
                    dt_Row[header_PartName] = row[dalItem.ItemName];
                    dt_Row[header_PartCode] = row[dalItem.ItemCode];
                    dt_Row[header_Status] = row[dalPlan.planStatus];

                    dt.Rows.Add(dt_Row);
                }

            }

            dgv.DataSource = null;

            if (dt.Rows.Count > 0)
            {
                //if (userPermission >= MainDashboard.ACTION_LVL_FOUR)
                //{
                //    lblRemoveCompleted.Show();
                //    lblClearList.Show();
                //}

                dgv.DataSource = dt;
                dgvItemListStyleEdit(dgv);
                dgv.ClearSelection();
            }
            //else
            //{
            //    lblRemoveCompleted.Hide();
            //    lblClearList.Hide();
            //}

        }

        private void CheckIfStockIn()
        {
            if (dgvRecordHistory.DataSource != null)
            {
                DataTable dt = (DataTable)dgvRecordHistory.DataSource;
                
                dt.DefaultView.Sort = header_ProductionDate + " ASC";
                dt = dt.DefaultView.ToTable();

                DateTime previousDate = DateTime.MaxValue;
                int totalProducedQty = 0;
                int totalStockIn = 0;

                string itemCode = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PartCode].Value.ToString();
                string planID = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();
                
                DataTable transferData = dalTrf.codeLikeSearch(itemCode);

                foreach (DataRow row in dt.Rows)
                {
                    DateTime proDate = Convert.ToDateTime(row[header_ProductionDate].ToString());
                    int producedQty = int.TryParse(row[header_ProducedQty].ToString(), out producedQty) ? producedQty : 0;
                    

                    if(previousDate == DateTime.MaxValue)
                    {
                        previousDate = proDate;

                        totalProducedQty += producedQty;

                        

                    }
                    else if(previousDate == proDate)
                    {
                        totalProducedQty += producedQty;
                    }
                    else
                    {
                        //search transfer table
                        
                        
                        //change color
                        foreach(DataGridViewRow dgvRow in dgvRecordHistory.Rows)
                        {
                            if(previousDate == Convert.ToDateTime(dgvRow.Cells[header_ProductionDate].Value.ToString()))
                            {

                                //check if parent
                                string sheetID = dgvRow.Cells[header_SheetID].Value.ToString();

                                itemCode = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PartCode].Value.ToString();
                                transferData = dalTrf.codeLikeSearch(itemCode);

                                //foreach (DataRow rowPR in dt_ProductionRecord.Rows)
                                //{
                                //    if(sheetID == rowPR[dalProRecord.SheetID].ToString())
                                //    {
                                //        string parentCode = rowPR[dalProRecord.ParentCode].ToString();

                                //        if(!string.IsNullOrEmpty(parentCode))
                                //        {
                                //            itemCode = parentCode;
                                //            transferData = dalTrf.codeSearch(itemCode);
                                //        }
                                //        break;
                                //    }
                                //}

                                int trfQty = tool.TotalProductionStockInInOneDay(transferData, itemCode, previousDate, planID, dgvRow.Cells[header_Shift].Value.ToString());
                               
                                if (trfQty == -1)
                                {
                                    dgvRow.Cells[header_SheetID].Style.BackColor = Color.Pink;
                                }
                                else if (totalProducedQty == trfQty)
                                {
                                    totalStockIn += trfQty;
                                    dgvRow.Cells[header_SheetID].Style.BackColor = Color.LightGreen;
                                    dgvRow.Cells[header_StockIn].Value = trfQty;
                                }
                                else if (totalProducedQty != trfQty)
                                {
                                    totalStockIn += trfQty;
                                    dgvRow.Cells[header_SheetID].Style.BackColor = Color.LightBlue;
                                    dgvRow.Cells[header_StockIn].Value = trfQty;
                                }
                            }
                        }

                        previousDate = proDate;
                        totalProducedQty = producedQty;
                    }

                }

                //change last row color
                foreach (DataGridViewRow dgvRow in dgvRecordHistory.Rows)
                {
                    if (previousDate == Convert.ToDateTime(dgvRow.Cells[header_ProductionDate].Value.ToString()))
                    {
                        //check if parent
                        string sheetID = dgvRow.Cells[header_SheetID].Value.ToString();

                        itemCode = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PartCode].Value.ToString();
                        transferData = dalTrf.codeLikeSearch(itemCode);

                        //foreach (DataRow rowPR in dt_ProductionRecord.Rows)
                        //{
                        //    if (sheetID == rowPR[dalProRecord.SheetID].ToString())
                        //    {
                        //        string parentCode = rowPR[dalProRecord.ParentCode].ToString();

                        //        if (!string.IsNullOrEmpty(parentCode))
                        //        {
                        //            itemCode = parentCode;
                        //            transferData = dalTrf.codeSearch(itemCode);
                        //        }
                        //        break;
                        //    }
                        //}

                        int trfQty_ = tool.TotalProductionStockInInOneDay(transferData, itemCode, previousDate, planID, dgvRow.Cells[header_Shift].Value.ToString());

                        if (trfQty_ == -1)
                        {
                            dgvRow.Cells[header_SheetID].Style.BackColor = Color.Pink;
                        }
                        else if (totalProducedQty == trfQty_)
                        {
                            totalStockIn += trfQty_;
                            dgvRow.Cells[header_SheetID].Style.BackColor = Color.LightGreen;
                            dgvRow.Cells[header_StockIn].Value = trfQty_;
                            dgvRow.Cells[header_TotalStockIn].Value = totalStockIn;
                        }
                        else if (totalProducedQty != trfQty_)
                        {
                            totalStockIn += trfQty_;
                            dgvRow.Cells[header_SheetID].Style.BackColor = Color.LightBlue;
                            dgvRow.Cells[header_StockIn].Value = trfQty_;
                            dgvRow.Cells[header_TotalStockIn].Value = totalStockIn;
                        }
                    }
                }

               
                txtTotalStockInRecord.Text = totalStockIn.ToString();
            }

        }

        private void CheckIfBalanceClear()
        {
            if(balanceInEdited || balanceOutEdited)
            {
                int balanceInQty = int.TryParse(txtIn.Text, out balanceInQty) ? balanceInQty : 0;
                int balanceOutQty = int.TryParse(txtOut.Text, out balanceOutQty) ? balanceOutQty : 0;

                string itemCode = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PartCode].Value.ToString();
                string planID = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

                string parentCode = cmbParentList.Text;

                if (!string.IsNullOrEmpty(parentCode))
                {
                    itemCode = parentCode;
                }

                DataTable transferData = dalTrf.codeLikeSearch(itemCode);

                DateTime proDate = dtpProDate.Value;

                string shift = text.Shift_Morning;

                if(cbNight.Checked)
                {
                    shift = text.Shift_Night;
                }


                //check balance in
                if(balanceInEdited)
                {
                    var trfData = tool.GetPreviousBalanceClearRecord(transferData, itemCode, proDate, planID, shift, true);

                    int trfID = trfData.Item1;
                    int trfQty = trfData.Item2;

                    if(balanceInQty != trfQty)
                    {
                        //undo trfID
                        if (trfID != -1)
                            tool.changeTransferRecord("Undo", trfID);

                        //make a new transfer
                        //open in out edit page
                        if (balanceInQty > 0)
                            DirectInWithoutSave();
                    }
                 
                }

                //check balance Out
                if (balanceOutEdited)
                {
                    var trfData = tool.GetPreviousBalanceClearRecord(transferData, itemCode, proDate, planID, shift, false);

                    int trfID = trfData.Item1;
                    int trfQty = trfData.Item2;

                    if (balanceOutQty != trfQty)
                    {
                        //undo trfID
                        if(trfID != -1)
                            tool.changeTransferRecord("Undo", trfID);

                        //make a new transfer
                        //open in out edit page
                        if (balanceOutQty > 0)
                            DirectOutWithoutSave();
                    }
                   
                }


                balanceInEdited = false;
                balanceOutEdited = false;
            }
          

        }

        private void UndoPreviousRecordIfExist()
        {
            string planID = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

            DateTime proDate = dtpProDate.Value;

            DataTable dt_Trf = dalTrf.rangeTrfSearch(proDate.ToString("yyyy/MM/dd"), proDate.ToString("yyyy/MM/dd"));

            string shift = text.Shift_Morning;

            if (cbNight.Checked)
            {
                shift = text.Shift_Night;
            }

            dt_Trf.DefaultView.Sort = dalTrf.TrfDate + " DESC";
            dt_Trf = dt_Trf.DefaultView.ToTable();

            int trfTableCode = -1;

            foreach (DataRow row in dt_Trf.Rows)
            {
                string status = row[dalTrf.TrfResult].ToString();

                if (status == "Passed")
                {
                    DateTime trfDate = Convert.ToDateTime(row[dalTrf.TrfDate].ToString());

                    string productionInfo = row[dalTrf.TrfNote].ToString();
                    string _shift = "";
                    string _planID = "";
                    string balanceClearType = "";
                    bool startCopy = false;
                    bool IDCopied = false;
                    bool ShiftCopied = false;
                    bool StopIDCopy = false;

                    for (int i = 0; i < productionInfo.Length; i++)
                    {
                        if (productionInfo[i].ToString() == "P")
                        {
                            startCopy = true;
                        }
                        else if (ShiftCopied && (productionInfo[i].ToString() == "O" || productionInfo[i].ToString() == "B"))
                        {
                            startCopy = true;
                        }
                        else if (ShiftCopied && productionInfo[i].ToString() == "]")
                        {
                            startCopy = false;
                            StopIDCopy = true;
                        }

                        if (startCopy)
                        {

                            if (char.IsDigit(productionInfo[i]) && !StopIDCopy)
                            {
                                IDCopied = true;
                                _planID += productionInfo[i];
                            }
                            else if (IDCopied && i + 1 < productionInfo.Length)
                            {
                                _shift += productionInfo[i + 1];
                                IDCopied = false;
                                ShiftCopied = true;
                                startCopy = false;
                            }
                            else if (ShiftCopied)
                            {
                                balanceClearType += productionInfo[i];
                            }

                        }
                    }

                    if (_shift.ToUpper() == "M")
                    {
                        _shift = text.Shift_Morning;
                    }
                    else if (_shift.ToUpper() == "N")
                    {
                        _shift = text.Shift_Night;
                    }

                    bool dataMatch = shift == _shift && planID == _planID;

                    if (dataMatch)
                    {
                        trfTableCode = int.TryParse(row[dalTrf.TrfID].ToString(), out trfTableCode) ? trfTableCode : -1;

                        if (trfTableCode != -1)
                            tool.UndoTransferRecord(trfTableCode);

                    }
                }

            }

        }

        private bool CheckIfPreviousRecordExist()
        {
            string planID = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

            DateTime proDate = dtpProDate.Value;

            DataTable dt_Trf = dalTrf.rangeTrfSearch(proDate.ToString("yyyy/MM/dd"), proDate.ToString("yyyy/MM/dd"));

            string shift = text.Shift_Morning;

            if (cbNight.Checked)
            {
                shift = text.Shift_Night;
            }

            dt_Trf.DefaultView.Sort = dalTrf.TrfDate + " DESC";
            dt_Trf = dt_Trf.DefaultView.ToTable();

            foreach (DataRow row in dt_Trf.Rows)
            {
                string status = row[dalTrf.TrfResult].ToString();

                if (status == "Passed")
                {
                    DateTime trfDate = Convert.ToDateTime(row[dalTrf.TrfDate].ToString());

                    string productionInfo = row[dalTrf.TrfNote].ToString();
                    string _shift = "";
                    string _planID = "";
                    string balanceClearType = "";
                    bool startCopy = false;
                    bool IDCopied = false;
                    bool ShiftCopied = false;
                    bool StopIDCopy = false;

                    for (int i = 0; i < productionInfo.Length; i++)
                    {
                        if (productionInfo[i].ToString() == "P")
                        {
                            startCopy = true;
                        }
                        else if (ShiftCopied && (productionInfo[i].ToString() == "O" || productionInfo[i].ToString() == "B"))
                        {
                            startCopy = true;
                        }
                        else if (ShiftCopied && productionInfo[i].ToString() == "]")
                        {
                            startCopy = false;
                            StopIDCopy = true;
                        }

                        if (startCopy)
                        {

                            if (char.IsDigit(productionInfo[i]) && !StopIDCopy)
                            {
                                IDCopied = true;
                                _planID += productionInfo[i];
                            }
                            else if (IDCopied && i + 1 < productionInfo.Length)
                            {
                                _shift += productionInfo[i + 1];
                                IDCopied = false;
                                ShiftCopied = true;
                                startCopy = false;
                            }
                            else if (ShiftCopied)
                            {
                                balanceClearType += productionInfo[i];
                            }

                        }
                    }

                    if (_shift.ToUpper() == "M")
                    {
                        _shift = text.Shift_Morning;
                    }
                    else if (_shift.ToUpper() == "N")
                    {
                        _shift = text.Shift_Night;
                    }

                    bool dataMatch = shift == _shift && planID == _planID;

                    if (dataMatch)
                    {
                        return true;

                    }
                }

            }

            return false;

        }

        private bool CheckIfDateOrShiftDuplicate()
        {
            ClearAllError();
            bool ifDuplicateData = false;
            if (addingNewSheet)
            {
                
                DataTable dt = (DataTable)dgvRecordHistory.DataSource;

                DateTime selectedDate = dtpProDate.Value;
                string shift = null;

                if (cbMorning.Checked)
                {
                    shift = text.Shift_Morning;
                }
                else if (cbNight.Checked)
                {
                    shift = text.Shift_Night;
                }

                if (shift != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DateTime recordedDate = Convert.ToDateTime(row[header_ProductionDate].ToString());
                        string recordedShift = row[header_Shift].ToString();
                        string recordedLotNo = row[header_ProLotNo].ToString();



                        ifDuplicateData = recordedDate == selectedDate;
                        ifDuplicateData &= recordedShift == shift;

                        if (ifDuplicateData)
                        {
                            MessageBox.Show("There is a existing daily job sheet under your selected date and shift!");
                        }
                    }
                }
            }
           

            return ifDuplicateData;
        }

        private int GetLastestLotNoFromDGV()
        {
            int LastestLotNo = -1;

            DataTable dt = (DataTable)dgvRecordHistory.DataSource;

            foreach (DataRow row in dt.Rows)
            {
                string recordedLotNo = row[header_ProLotNo].ToString();

                int numberOnly = GetINTOnlyFromLotNo(recordedLotNo);

                if (numberOnly > LastestLotNo)
                {
                    LastestLotNo = numberOnly;
                }
            }

            return LastestLotNo;
        }

        private void LoadDailyRecord()
        {
            dailyRecordLoaded = false;

            int itemRow = -1;
            if (dgvItemList.CurrentCell != null)
            {
                itemRow = dgvItemList.CurrentCell.RowIndex;
            }
            
            if(itemRow >= 0)
            {
                //btnShowDailyRecord.Visible = true;
                string planID = dgvItemList.Rows[itemRow].Cells[header_PlanID].Value.ToString();
                string planStatus = dgvItemList.Rows[itemRow].Cells[header_Status].Value.ToString();
                macID = int.TryParse(dgvItemList.Rows[itemRow].Cells[header_Machine].Value.ToString(), out macID) ? macID : 0;

                DataTable dt = NewSheetRecordTable();
                DataRow dt_Row;
                
                int totalProducedQty = 0;
                foreach(DataRow row in dt_ProductionRecord.Rows)
                {
                    bool active = Convert.ToBoolean(row[dalProRecord.Active].ToString());
                    if(planID == row[dalProRecord.PlanID].ToString() && active)
                    {
                        dt_Row = dt.NewRow();

                        int produced = Convert.ToInt32(row[dalProRecord.TotalProduced].ToString());
                        int proLotNo = int.TryParse(row[dalProRecord.ProLotNo].ToString(), out proLotNo) ? proLotNo : -1;
                        totalProducedQty += produced;

                       

                        if(proLotNo != -1)
                        {
                            dt_Row[header_ProLotNo] = SetAlphabetToProLotNo(macID, proLotNo);
                        }

                        dt_Row[header_SheetID] = row[dalProRecord.SheetID].ToString();
                        dt_Row[header_ProductionDate] = row[dalProRecord.ProDate].ToString();
                        dt_Row[header_Shift] = row[dalProRecord.Shift].ToString();
                        dt_Row[header_ProducedQty] = produced;
                        dt_Row[header_UpdatedDate] = row[dalProRecord.UpdatedDate].ToString();
                        dt_Row[header_UpdatedBy] = dalUser.getUsername(Convert.ToInt32(row[dalProRecord.UpdatedBy].ToString()));

                        dt_Row[header_TotalProduced] = totalProducedQty;

                        if(dt.Rows.Count > 0)
                        dt.Rows[dt.Rows.Count - 1][header_TotalProduced] = DBNull.Value;

                        dt.Rows.Add(dt_Row);
                        
                        

                    }
                }

                txtTotalProducedRecord.Text = totalProducedQty.ToString();
                //update total produced
                uPlan.plan_id = Convert.ToInt32(planID);
                uPlan.plan_produced = totalProducedQty;
                if(!dalPlan.TotalProducedUpdate(uPlan))
                {
                    MessageBox.Show("Failed to update total produced qty!");
                }

                dgvRecordHistory.DataSource = null;

                dgvRecordHistory.DataSource = dt;
                dgvSheetRecordStyleEdit(dgvRecordHistory);
                dgvRecordHistory.ClearSelection();

                //load checked status
                LoadCheckedStatus(planID, planStatus);

                CheckIfStockIn();
            }
            else
            {
                //btnShowDailyRecord.Visible = false;
            }
           
        }

        private void LoadCheckedStatus(string planID, string planStatus)
        {
            if(planStatus == text.planning_status_completed)
            {
                
                DataTable dt = dalPlan.idSearch(planID);
                foreach (DataRow row in dt.Rows)
                {
                    if (planID == row[dalPlan.planID].ToString())
                    {
                        bool Checked = bool.TryParse(row[dalPlan.Checked].ToString(), out Checked) ? Checked : false;

                        cbChecked.Visible = true;
                        if (Checked)
                        {
                            stopCheckedChange = true;
                            cbChecked.Checked = true;
                            stopCheckedChange = false;

                            int checkedBy = int.TryParse(row[dalPlan.CheckedBy].ToString(), out checkedBy) ? checkedBy : -1;
                            string userName = dalUser.getUsername(checkedBy);
                            string checkedDate = row[dalPlan.CHeckedDate].ToString();

                            lblCheckBy.Text = string_CheckBy + userName;
                            lblCheckDate.Text = string_CheckDate + checkedDate;
                        }
                        else
                        {
                            stopCheckedChange = true;
                            cbChecked.Checked = false;
                            stopCheckedChange = false;

                            lblCheckBy.Text = "";
                            lblCheckDate.Text = "";
                        }

                        break;
                    }

                }
            }
            else
            {
                cbChecked.Visible = false;

                lblCheckBy.Text = "";
                lblCheckDate.Text = "";
            }
          
        }

        private void LoadSheetRecordHistoryData()
        {
            DataGridView dgv = dgvRecordHistory;
            DataTable dt = NewSheetRecordTable();

            dgv.DataSource = dt;
        }

        private void LoadParentList(string itemCode)
        {
            DataTable dt_Parent = GetParentList(itemCode);

            if (dt_Parent.Rows.Count > 0)
            {
                dt_Parent.DefaultView.Sort = "Parent ASC";

                cmbParentList.DataSource = dt_Parent;
                cmbParentList.DisplayMember = "Parent";
                cmbParentList.SelectedIndex = -1;
            }
        
            else
            {
                cmbParentList.Enabled = false;
            }
        }

        private DataTable GetParentList(string itemCode)
        {
            DataTable dtJoin = dalJoin.loadParentList(itemCode);
            DataTable dt = new DataTable();

            if (dtJoin.Rows.Count > 0)
            {
                cmbParentList.Enabled = true;
               
                dt.Columns.Add("Parent");

                foreach (DataRow row in dtJoin.Rows)
                {
                    bool duplicate = false;
                    string parentCode = row[dalJoin.JoinParent].ToString();

                    DataTable dt_GrandParentList = GetParentList(parentCode);

                    if (dt_GrandParentList.Rows.Count > 0)
                    {
                        dt.Merge(dt_GrandParentList);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row2 in dt.Rows)
                        {
                            string list_ParentCode = row2["Parent"].ToString();

                            if (list_ParentCode == parentCode)
                            {
                                duplicate = true;
                                break;
                            }
                        }
                    }

                    if (!duplicate)
                    {
                        dt.Rows.Add(parentCode);
                    }

                }

               
            }
           
            return dt;
        }

        private void LoadNewSheetData()
        {
            //if new
            //if row selected

            sheetLoaded = false;

            int selectedItem = dgvItemList.CurrentCell.RowIndex;

            if(selectedItem <= -1)
            {
                MessageBox.Show("Please select a item before adding new sheet.");
            }
            else
            {
                string itemName = dgvItemList.Rows[selectedItem].Cells[header_PartName].Value.ToString();
                string itemCode = dgvItemList.Rows[selectedItem].Cells[header_PartCode].Value.ToString();
                string planID = dgvItemList.Rows[selectedItem].Cells[header_PlanID].Value.ToString();

                LoadParentList(itemCode);

                int macID = Convert.ToInt32(dgvItemList.Rows[selectedItem].Cells[header_Machine].Value.ToString());

                lotNo = tool.GetNextLotNo(dt_Mac, macID);
                int lotNoFromDGV = GetLastestLotNoFromDGV();

                if(lotNoFromDGV != -1)
                {
                    lotNo = lotNoFromDGV;
                }
              
                //if(lotNoFromDGV != -1 && lotNo - lotNoFromDGV > 1)
                //{
                //    lotNo = lotNoFromDGV + 1;
                //}

                txtProLotNo.Text = SetAlphabetToProLotNo(macID, lotNo);
                lblCustomer.Text = "";
                lblPartName.Text = itemName;
                lblPartCode.Text = itemCode;

                foreach (DataRow row in dt_ItemInfo.Rows)
                {
                    if(row[dalItem.ItemCode].ToString().Equals(itemCode))
                    {
                        float partWeight = float.TryParse(row[dalItem.ItemProPWShot].ToString(), out float i) ? Convert.ToSingle(row[dalItem.ItemProPWShot].ToString()) : -1;
                        float runnerWeight = float.TryParse(row[dalItem.ItemProRWShot].ToString(), out float k) ? Convert.ToSingle(row[dalItem.ItemProRWShot].ToString()) : -1;

                        txtPlanID.Text = planID;
                        txtSheetID.Text = string_NewSheet;
                        lblPW.Text = partWeight.ToString("0.##");
                        lblRW.Text = runnerWeight.ToString("0.##");
                        lblCavity.Text = row[dalItem.ItemCavity] == DBNull.Value ? "" : row[dalItem.ItemCavity].ToString();
                        lblRawMat.Text = row[dalItem.ItemMaterial] == DBNull.Value ? "" : row[dalItem.ItemMaterial].ToString();
                        lblColorMat.Text = row[dalItem.ItemMBatch] == DBNull.Value ? "" : row[dalItem.ItemMBatch].ToString();

                        float colorRate = row[dalItem.ItemMBRate] == DBNull.Value ? 0 : Convert.ToSingle(row[dalItem.ItemMBRate]);
                        lblColorUsage.Text = (colorRate * 100).ToString("0.##");
                    }
                       
                }

                //GetPackagingDataForNewSheet(itemCode);

                LoadCartonSetting();

                #region GET PACKAGING DATA
                //get packaging data
                //foreach (DataRow row in dt_JoinInfo.Rows)
                //{
                //    string parentCode = row[dalJoin.JoinParent].ToString();

                //    if (parentCode == itemCode)
                //    {
                //        string childCode = row[dalJoin.JoinChild].ToString();

                //        foreach (DataRow rowItem in dt_ItemInfo.Rows)
                //        {
                //            if (rowItem[dalItem.ItemCode].ToString().Equals(childCode))
                //            {
                //                string cat = rowItem[dalItem.ItemCat].ToString();

                //                if (cat.Equals(text.Cat_Carton) || cat.Equals(text.Cat_Packaging))
                //                {
                //                    string packagingName = rowItem[dalItem.ItemName].ToString();
                //                    string packagingCode = rowItem[dalItem.ItemCode].ToString();
                //                    string packagingQty = row[dalJoin.JoinMax].ToString();

                //                    txtPackingMaxQty.Text = packagingQty;
                //                    cmbPackingName.Text = packagingName;
                //                    cmbPackingCode.Text = packagingCode;

                //                    break;
                //                }
                //            }
                //        }
                //    }

                //}
                #endregion

                #region OLD GET PACKAGING DATA
                ////get packaging data
                //foreach (DataRow row in dt_JoinInfo.Rows)
                //{
                //    string parentCode = row[dalJoin.JoinParent].ToString();

                //    if (parentCode == itemCode)
                //    {
                //        string childCode = row[dalJoin.JoinChild].ToString();

                //        foreach (DataRow rowItem in dt_ItemInfo.Rows)
                //        {
                //            if (rowItem[dalItem.ItemCode].ToString().Equals(childCode))
                //            {
                //                string cat = rowItem[dalItem.ItemCat].ToString();

                //                if (cat.Equals(text.Cat_Carton) || cat.Equals(text.Cat_Packaging))
                //                {
                //                    string packagingName = rowItem[dalItem.ItemName].ToString();
                //                    string packagingCode = rowItem[dalItem.ItemCode].ToString();
                //                    string packagingQty = row[dalJoin.JoinMax].ToString();

                //                    txtPackingMaxQty.Text = packagingQty;
                //                    cmbPackingName.Text = packagingName;
                //                    cmbPackingCode.Text = packagingCode;

                //                    break;
                //                }
                //            }
                //        }
                //    }

                //}
                #endregion

                txtOut.Text ="0";
                txtIn.Text = "0";

                //get balance of last shift
                txtBalanceOfLastShift.Text = GetLatestBalanceLeft(dalProRecord.Select(), planID).ToString();
            }

            sheetLoaded = true;

        }

        private void LoadCartonSetting()
        {
            //get sheet Id
            string itemCode = lblPartCode.Text;
            string sheetID = txtSheetID.Text;
            int MainCartonQty = 0;
            txtFullBox.Enabled = true;

            dt_Carton = null;

            if (string.IsNullOrEmpty(sheetID) || sheetID.Equals(string_NewSheet))
            {
                //new sheet
                //find main carton from item group and load to dt_carton
                foreach(DataRow row in dt_JoinInfo.Rows)
                {
                    string parentCode = row[dalJoin.JoinParent].ToString();

                    bool mainCarton = bool.TryParse(row[dalJoin.JoinMainCarton].ToString(), out mainCarton) ? mainCarton : false;
                    bool StockOut = bool.TryParse(row[dalJoin.JoinStockOut].ToString(), out StockOut) ? StockOut : true;

                    if (itemCode == parentCode && mainCarton)
                    {

                        MainCartonQty++;

                        //adddata to dt_carton
                        if (dt_Carton == null)
                        {
                            dt_Carton = NewCartonTable();
                        }

                        DataRow newRow = dt_Carton.NewRow();

                        string cartonCode = row[dalJoin.JoinChild].ToString();

                        int MaxQty = int.TryParse(row[dalJoin.JoinMax].ToString(), out MaxQty) ? MaxQty : 0;
                        int childQty = int.TryParse(row[dalJoin.JoinQty].ToString(), out childQty) ? childQty : 0;

                        newRow[header_PackagingCode] = cartonCode;
                        newRow[header_PackagingName] = tool.getItemName(cartonCode);
                        newRow[header_PackagingMax] = MaxQty;
                        newRow[header_PackagingQty] = childQty;
                        newRow[header_PackagingStockOut] = StockOut;

                        dt_Carton.Rows.Add(newRow);

                    }
                }
            }
            else
            {
                //old sheet
                //load carton data from carton db to dt_carton
                DataTable db_carton = dalProRecord.PackagingRecordSelect(int.TryParse(sheetID, out int i)? i:0);

                if(db_carton != null && db_carton.Rows.Count > 0)
                {
                    foreach(DataRow row in db_carton.Rows)
                    {

                        //adddata to dt_carton
                        if (dt_Carton == null)
                        {
                            dt_Carton = NewCartonTable();
                        }

                        DataRow newRow = dt_Carton.NewRow();

                        string cartonCode = row[dalProRecord.PackagingCode].ToString();

                        int MaxQty = int.TryParse(row[dalProRecord.PackagingMax].ToString(), out MaxQty) ? MaxQty : 0;
                        int childQty = int.TryParse(row[dalProRecord.PackagingQty].ToString(), out childQty) ? childQty : 0;

                        bool cartonStockOut = bool.TryParse(row[dalProRecord.PackagingStockOut].ToString(), out cartonStockOut) ? cartonStockOut : true;

                        newRow[header_PackagingCode] = cartonCode;
                        newRow[header_PackagingName] = tool.getItemName(cartonCode);
                        newRow[header_PackagingMax] = MaxQty;
                        newRow[header_PackagingQty] = childQty;
                        newRow[header_PackagingStockOut] = cartonStockOut;

                        dt_Carton.Rows.Add(newRow);
                    }
                }

            }

            string AlertMessage = "";

            if (dt_Carton != null &&  dt_Carton.Rows.Count > 0)
            {
                int totalStockIn = 0;
                int totalFullBox = 0;

                string name = dt_Carton.Rows[0][header_PackagingName].ToString();
                string code = dt_Carton.Rows[0][header_PartCode].ToString();

                int rowCount = dt_Carton.Rows.Count;

                if (rowCount >= 2)
                {
                    LoadPackingToCMB(string_MultiPackaging);
                    txtPackingMaxQty.Text = "MULTI";

                    txtFullBox.Enabled = false;
                  

                    foreach (DataRow row in dt_Carton.Rows)
                    {
                        int boxQty = int.TryParse(row[header_PackagingQty].ToString(), out boxQty) ? boxQty : 0;
                        int maxQty = int.TryParse(row[header_PackagingMax].ToString(), out maxQty) ? maxQty : 0;

                        totalFullBox += boxQty;

                        totalStockIn += boxQty * maxQty;
                    }


                    txtFullBox.Text = totalFullBox.ToString();

                }
                else if (rowCount == 1)
                {
                    loadPackingData();

                    cmbPackingName.Text = name;
                    cmbPackingCode.Text = code;

                    foreach (DataRow row in dt_Carton.Rows)
                    {
                        txtPackingMaxQty.Text = row[header_PackagingMax].ToString();
                        txtFullBox.Text = row[header_PackagingQty].ToString();
                    }


                }
            }
            else
            {
                txtPackingMaxQty.Text = "";
                cmbPackingName.SelectedIndex = -1;
                cmbPackingCode.SelectedIndex = -1;

                AlertMessage = "Carton data not found!\nPlease add a main carton to this item at:\n1. Item Group Edit";
            }

            //check if more than 1 main carton
            if(MainCartonQty > 1)
            {
                AlertMessage = "More than 1 main carton detected!\nYou can edit it at:\n1. Carton Setting\n2. Item Group Edit";
            }

            if(!string.IsNullOrEmpty(AlertMessage))
            MessageBox.Show(AlertMessage, "Carton Alert",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void GetPackagingDataForNewSheet(string itemCode)
        {
            DataTable dt = NewPackagingTable();

            //get packaging data
            foreach (DataRow row in dt_JoinInfo.Rows)
            {
                string parentCode = row[dalJoin.JoinParent].ToString();

                if (parentCode == itemCode)
                {
                    string childCode = row[dalJoin.JoinChild].ToString();

                    foreach (DataRow rowItem in dt_ItemInfo.Rows)
                    {
                        if (rowItem[dalItem.ItemCode].ToString().Equals(childCode))
                        {
                            string cat = rowItem[dalItem.ItemCat].ToString();
                            string packagingCode = rowItem[dalItem.ItemCode].ToString();
                            string packagingName = rowItem[dalItem.ItemName].ToString();
                            string packagingQty = int.TryParse(row[dalJoin.JoinMax].ToString(), out int i) ? i.ToString() : "1" ;


                            //ADD DATA TO PACKAGING TABLE
                            DataRow dt_Row = dt.NewRow();

                            int TotalBox = 0;

                            dt_Row[header_PackagingCode] = packagingCode;
                            dt_Row[header_PackagingName] = tool.getItemName(packagingCode);
                            dt_Row[header_PackagingMax] = packagingQty;
                            dt_Row[header_PackagingQty] = TotalBox;

                            dt.Rows.Add(dt_Row);

                            if ((cat.Equals(text.Cat_Carton) && (packagingCode.Contains("CTN") || packagingCode.Contains("CTR"))) || cat.Equals(text.Cat_Packaging))
                            {
                                txtPackingMaxQty.Text = packagingQty;
                                cmbPackingName.Text = packagingName;
                                cmbPackingCode.Text = packagingCode;
                            }
                        }
                    }
                }

            }

            dt_MultiPackaging = dt.Copy();

            if (dt_MultiPackaging.Rows.Count > 0)
            {
                string name = dt_MultiPackaging.Rows[0][header_PackagingName].ToString();
                string code = dt_MultiPackaging.Rows[0][header_PartCode].ToString();

                int rowCount = dt_MultiPackaging.Rows.Count;

                if (rowCount >= 2)
                {
                    LoadPackingToCMB(string_MultiPackaging);
                    txtPackingMaxQty.Text = "MULTI";

                    int totalStockIn = 0;
                    int totalFullBox = 0;

                    foreach (DataRow row in dt_MultiPackaging.Rows)
                    {
                        int boxQty = Convert.ToInt32(row[frmProPackaging.header_PackagingQty].ToString());
                        int maxQty = Convert.ToInt32(row[frmProPackaging.header_PackagingMax].ToString());

                        totalFullBox += boxQty;

                        totalStockIn += boxQty * maxQty;
                    }


                    txtFullBox.Text = totalFullBox.ToString();

                }

                else if (rowCount == 1)
                {
                    loadPackingData();

                    cmbPackingName.Text = name;
                    cmbPackingCode.Text = code;

                    foreach (DataRow row in dt_MultiPackaging.Rows)
                    {
                        txtPackingMaxQty.Text = row[header_PackagingMax].ToString();
                        txtFullBox.Text = row[header_PackagingQty].ToString();
                    }


                }
            }

        }

        private DateTime GetStatusUpdatedDate(string planID)
        {
            DateTime UpdatedDate = DateTime.MaxValue;

            foreach(DataRow row in dt_Plan.Rows)
            {
                if(planID == row[dalPlan.planID].ToString())
                {
                    //UpdatedDate = Convert.ToDateTime();
                    UpdatedDate = DateTime.TryParse(row[dalPlan.planUpdatedDate].ToString(), out UpdatedDate) ? UpdatedDate : DateTime.MaxValue;

                    return UpdatedDate;
                }
            }

            return UpdatedDate;
        }

        private void LoadPlanInfo()
        {
            int selectedItem = dgvItemList.CurrentCell.RowIndex;

            if (selectedItem <= -1)
            {
                MessageBox.Show("Please select a item before adding new sheet.");
            }
            else
            {
                string itemName = dgvItemList.Rows[selectedItem].Cells[header_PartName].Value.ToString();
                string itemCode = dgvItemList.Rows[selectedItem].Cells[header_PartCode].Value.ToString();
                string planID = dgvItemList.Rows[selectedItem].Cells[header_PlanID].Value.ToString();


                lblCustomer.Text = "";
                lblPartName.Text = itemName;
                lblPartCode.Text = itemCode;

                lblPlanUpdatedDate.Text = GetStatusUpdatedDate(planID).ToString();

                foreach (DataRow row in dt_ItemInfo.Rows)
                {
                    if (row[dalItem.ItemCode].ToString().Equals(itemCode))
                    {
                        float partWeight = float.TryParse(row[dalItem.ItemProPWShot].ToString(), out float i) ? Convert.ToSingle(row[dalItem.ItemProPWShot].ToString()) : -1;
                        float runnerWeight = float.TryParse(row[dalItem.ItemProRWShot].ToString(), out float k) ? Convert.ToSingle(row[dalItem.ItemProRWShot].ToString()) : -1;

                        txtPlanID.Text = planID;
                      
                        lblPW.Text = partWeight.ToString("0.##");
                        lblRW.Text = runnerWeight.ToString("0.##");
                        lblCavity.Text = row[dalItem.ItemCavity] == DBNull.Value ? "" : row[dalItem.ItemCavity].ToString();
                        lblRawMat.Text = row[dalItem.ItemMaterial] == DBNull.Value ? "" : row[dalItem.ItemMaterial].ToString();
                        lblColorMat.Text = row[dalItem.ItemMBatch] == DBNull.Value ? "" : row[dalItem.ItemMBatch].ToString();

                        float colorRate = row[dalItem.ItemMBRate] == DBNull.Value ? 0 : Convert.ToSingle(row[dalItem.ItemMBRate]);
                        lblColorUsage.Text = (colorRate * 100).ToString("0.##");
                    }

                }

               
            }

        }

        private void LoadExistingSheetData()
        {
            sheetLoaded = false;
            
            int selectedItem = dgvItemList.CurrentCell.RowIndex;
            int selectedDailyRecord = dgvRecordHistory.CurrentCell.RowIndex;
            if (selectedItem <= -1)
            {
                MessageBox.Show("Please select a item before adding new sheet.");
            }
            else
            {
                string itemName = dgvItemList.Rows[selectedItem].Cells[header_PartName].Value.ToString();
                string itemCode = dgvItemList.Rows[selectedItem].Cells[header_PartCode].Value.ToString();
                string planID = dgvItemList.Rows[selectedItem].Cells[header_PlanID].Value.ToString();
                string sheetID = dgvRecordHistory.Rows[selectedDailyRecord].Cells[header_SheetID].Value.ToString();
                string shift = dgvRecordHistory.Rows[selectedDailyRecord].Cells[header_Shift].Value.ToString();

                LoadParentList(itemCode);
                uProRecord.sheet_id = Convert.ToInt32(sheetID);
                DataTable dt_ProductionRecord = dalProRecord.ProductionRecordSelect(uProRecord);

                lblPartName.Text = itemName;
                lblPartCode.Text = itemCode;
                txtPlanID.Text = planID;
                txtSheetID.Text = sheetID;

                if (shift == "MORNING")
                {
                    cbMorning.Checked = true;
                }
                else if(shift == "NIGHT")
                {
                    cbNight.Checked = true;
                }

                foreach (DataRow row in dt_ProductionRecord.Rows)
                {
                    if (row[dalProRecord.SheetID].ToString().Equals(sheetID))
                    {
                        float partWeight = float.TryParse(row[dalPlan.planPW].ToString(), out float i) ? Convert.ToSingle(row[dalPlan.planPW].ToString()) : -1;
                        float runnerWeight = float.TryParse(row[dalPlan.planRW].ToString(), out float k) ? Convert.ToSingle(row[dalPlan.planRW].ToString()) : -1;

                        lblPW.Text = partWeight.ToString("0.##");
                        lblRW.Text = runnerWeight.ToString("0.##");

                        lblCavity.Text = row[dalPlan.planCavity] == DBNull.Value ? "" : row[dalPlan.planCavity].ToString();
                        lblRawMat.Text = row[dalPlan.materialCode] == DBNull.Value ? "" : row[dalPlan.materialCode].ToString();
                        lblColorMat.Text = row[dalPlan.colorMaterialCode] == DBNull.Value ? "" : row[dalPlan.colorMaterialCode].ToString();

                        
                        float colorRate = row[dalPlan.colorMaterialUsage] == DBNull.Value ? 0 : Convert.ToSingle(row[dalPlan.colorMaterialUsage]);
                        lblColorUsage.Text = colorRate.ToString("0.##");

                        dtpProDate.Value = Convert.ToDateTime(row[dalProRecord.ProDate].ToString());

                        int _ProLotNo = int.TryParse(row[dalProRecord.ProLotNo].ToString(), out _ProLotNo) ? _ProLotNo : -1;

                        if(_ProLotNo != -1)
                        {
                            txtProLotNo.Text = SetAlphabetToProLotNo(macID, _ProLotNo);
                        }
                        else
                        {
                            txtProLotNo.Text = row[dalProRecord.ProLotNo].ToString();
                        }
                        
                        txtRawMatLotNo.Text = row[dalProRecord.RawMatLotNo].ToString();
                        txtColorMatLotNo.Text = row[dalProRecord.ColorMatLotNo].ToString();
                        txtMeterStart.Text = row[dalProRecord.MeterStart].ToString();
                        txtBalanceOfLastShift.Text = row[dalProRecord.LastShiftBalance].ToString();
                        txtBalanceOfThisShift.Text = row[dalProRecord.CurrentShiftBalance].ToString();
                        txtIn.Text = row[dalProRecord.directIn].ToString();
                        txtOut.Text = row[dalProRecord.directOut].ToString();
                        txtFullBox.Text = row[dalProRecord.FullBox].ToString();
                        txtTotalProduce.Text = row[dalProRecord.TotalProduced].ToString();
                        cmbParentList.Text = row[dalProRecord.ParentCode].ToString();
                        txtNote.Text = row[dalProRecord.Note].ToString();

                        //cmbPackingName.Text = tool.getItemName(row[dalProRecord.PackagingCode].ToString());
                        //cmbPackingCode.Text = row[dalProRecord.PackagingCode].ToString();
                        //txtPackingMaxQty.Text = row[dalProRecord.PackagingQty].ToString();

                        //txtTotalReject.Text = row[dalProRecord.ProLotNo].ToString();
                        // txtRejectPercentage.Text = row[dalProRecord.ProLotNo].ToString();
                    }

                }

                LoadMeterReadingData(Convert.ToInt32(sheetID));
                CalculateHourlyShot();

                //get packaging data
                LoadPackagingList(Convert.ToInt32(sheetID));

                //foreach (DataRow row in dt_JoinInfo.Rows)
                //{
                //    string parentCode = row[dalJoin.JoinParent].ToString();

                //    if (parentCode == itemCode)
                //    {
                //        string childCode = row[dalJoin.JoinChild].ToString();

                //        foreach (DataRow rowItem in dt_ItemInfo.Rows)
                //        {
                //            if (rowItem[dalItem.ItemCode].ToString().Equals(childCode))
                //            {
                //                string cat = rowItem[dalItem.ItemCat].ToString();

                //                if (cat.Equals(text.Cat_Carton) || cat.Equals(text.Cat_Packaging))
                //                {
                //                    string packagingName = rowItem[dalItem.ItemName].ToString();
                //                    string packagingCode = rowItem[dalItem.ItemCode].ToString();
                //                    string packagingQty = row[dalJoin.JoinMax].ToString();

                //                    txtPackingMaxQty.Text = packagingQty;
                //                    cmbPackingName.Text = packagingName;
                //                    cmbPackingCode.Text = packagingCode;

                //                    break;
                //                }
                //            }
                //        }
                //    }

                //}

            }

            sheetLoaded = true;

        }

        private void NewLoadExistingSheetData()
        {
            sheetLoaded = false;

            int selectedItem = dgvItemList.CurrentCell.RowIndex;
            int selectedDailyRecord = dgvRecordHistory.CurrentCell.RowIndex;
            if (selectedItem <= -1)
            {
                MessageBox.Show("Please select a item before adding new sheet.");
            }
            else
            {
                string itemName = dgvItemList.Rows[selectedItem].Cells[header_PartName].Value.ToString();
                string itemCode = dgvItemList.Rows[selectedItem].Cells[header_PartCode].Value.ToString();
                string planID = dgvItemList.Rows[selectedItem].Cells[header_PlanID].Value.ToString();
                string sheetID = dgvRecordHistory.Rows[selectedDailyRecord].Cells[header_SheetID].Value.ToString();
                string shift = dgvRecordHistory.Rows[selectedDailyRecord].Cells[header_Shift].Value.ToString();

                LoadParentList(itemCode);
                uProRecord.sheet_id = Convert.ToInt32(sheetID);
                DataTable dt_ProductionRecord = dalProRecord.ProductionRecordSelect(uProRecord);

                lblPartName.Text = itemName;
                lblPartCode.Text = itemCode;
                txtPlanID.Text = planID;
                txtSheetID.Text = sheetID;

                if (shift == "MORNING")
                {
                    cbMorning.Checked = true;
                }
                else if (shift == "NIGHT")
                {
                    cbNight.Checked = true;
                }

                foreach (DataRow row in dt_ProductionRecord.Rows)
                {
                    if (row[dalProRecord.SheetID].ToString().Equals(sheetID))
                    {
                        float partWeight = float.TryParse(row[dalPlan.planPW].ToString(), out float i) ? Convert.ToSingle(row[dalPlan.planPW].ToString()) : -1;
                        float runnerWeight = float.TryParse(row[dalPlan.planRW].ToString(), out float k) ? Convert.ToSingle(row[dalPlan.planRW].ToString()) : -1;

                        lblPW.Text = partWeight.ToString("0.##");
                        lblRW.Text = runnerWeight.ToString("0.##");

                        lblCavity.Text = row[dalPlan.planCavity] == DBNull.Value ? "" : row[dalPlan.planCavity].ToString();
                        lblRawMat.Text = row[dalPlan.materialCode] == DBNull.Value ? "" : row[dalPlan.materialCode].ToString();
                        lblColorMat.Text = row[dalPlan.colorMaterialCode] == DBNull.Value ? "" : row[dalPlan.colorMaterialCode].ToString();


                        float colorRate = row[dalPlan.colorMaterialUsage] == DBNull.Value ? 0 : Convert.ToSingle(row[dalPlan.colorMaterialUsage]);
                        lblColorUsage.Text = colorRate.ToString("0.##");

                        dtpProDate.Value = Convert.ToDateTime(row[dalProRecord.ProDate].ToString());

                        int _ProLotNo = int.TryParse(row[dalProRecord.ProLotNo].ToString(), out _ProLotNo) ? _ProLotNo : -1;

                        if (_ProLotNo != -1)
                        {
                            txtProLotNo.Text = SetAlphabetToProLotNo(macID, _ProLotNo);
                        }
                        else
                        {
                            txtProLotNo.Text = row[dalProRecord.ProLotNo].ToString();
                        }

                        txtRawMatLotNo.Text = row[dalProRecord.RawMatLotNo].ToString();
                        txtColorMatLotNo.Text = row[dalProRecord.ColorMatLotNo].ToString();
                        txtMeterStart.Text = row[dalProRecord.MeterStart].ToString();
                        txtBalanceOfLastShift.Text = row[dalProRecord.LastShiftBalance].ToString();
                        txtBalanceOfThisShift.Text = row[dalProRecord.CurrentShiftBalance].ToString();
                        txtIn.Text = row[dalProRecord.directIn].ToString();
                        txtOut.Text = row[dalProRecord.directOut].ToString();
                        txtFullBox.Text = row[dalProRecord.FullBox].ToString();
                        txtTotalProduce.Text = row[dalProRecord.TotalProduced].ToString();
                        cmbParentList.Text = row[dalProRecord.ParentCode].ToString();
                        txtNote.Text = row[dalProRecord.Note].ToString();

                        //cmbPackingName.Text = tool.getItemName(row[dalProRecord.PackagingCode].ToString());
                        //cmbPackingCode.Text = row[dalProRecord.PackagingCode].ToString();
                        //txtPackingMaxQty.Text = row[dalProRecord.PackagingQty].ToString();

                        //txtTotalReject.Text = row[dalProRecord.ProLotNo].ToString();
                        // txtRejectPercentage.Text = row[dalProRecord.ProLotNo].ToString();
                    }

                }

                LoadMeterReadingData(Convert.ToInt32(sheetID));
                CalculateHourlyShot();

                //get packaging data
                //LoadPackagingList(Convert.ToInt32(sheetID));
                LoadCartonSetting();

                //foreach (DataRow row in dt_JoinInfo.Rows)
                //{
                //    string parentCode = row[dalJoin.JoinParent].ToString();

                //    if (parentCode == itemCode)
                //    {
                //        string childCode = row[dalJoin.JoinChild].ToString();

                //        foreach (DataRow rowItem in dt_ItemInfo.Rows)
                //        {
                //            if (rowItem[dalItem.ItemCode].ToString().Equals(childCode))
                //            {
                //                string cat = rowItem[dalItem.ItemCat].ToString();

                //                if (cat.Equals(text.Cat_Carton) || cat.Equals(text.Cat_Packaging))
                //                {
                //                    string packagingName = rowItem[dalItem.ItemName].ToString();
                //                    string packagingCode = rowItem[dalItem.ItemCode].ToString();
                //                    string packagingQty = row[dalJoin.JoinMax].ToString();

                //                    txtPackingMaxQty.Text = packagingQty;
                //                    cmbPackingName.Text = packagingName;
                //                    cmbPackingCode.Text = packagingCode;

                //                    break;
                //                }
                //            }
                //        }
                //    }

                //}

            }

            sheetLoaded = true;

        }

        private void LoadMeterReadingData(int sheedID)
        {
            CreateMeterReadingData();
            uProRecord.sheet_id = sheedID;
            DataTable dt = dalProRecord.MeterRecordSelect(uProRecord);
            DataTable dt_Meter = (DataTable)dgvMeterReading.DataSource;

            foreach(DataRow row in dt.Rows)
            {
                string timeFromDB = Convert.ToDateTime(row[dalProRecord.ProTime]).ToShortTimeString();

                foreach(DataRow dgvRow in dt_Meter.Rows)
                {
                    string timeFromDGV = Convert.ToDateTime(dgvRow[header_Time]).ToShortTimeString();

                    if(timeFromDB == timeFromDGV)
                    {
                        string proOperator = row[dalProRecord.ProOperator].ToString();
                        string proMeter = row[dalProRecord.ProMeterReading].ToString();

                        dgvRow[header_Operator] = proOperator;
                        dgvRow[header_MeterReading] = proMeter;

                        break;
                    }
                }
            }
        }

        private void CreateMeterReadingData()
        {
            DataTable dt = NewMeterReadingTable();
            DataRow dt_Row;

            if (cbMorning.Checked || cbNight.Checked)
            {
                dgvMeterReading.DataSource = null;

                bool MorningShift = true;

                if (cbNight.Checked)
                {
                    MorningShift = false;
                }

                if(MorningShift)
                {
                    DateTime dateNow = DateTime.Now;
                    DateTime date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 9, 0, 0);

                    for(int i = 0; i < 12; i++)
                    {
                        dt_Row = dt.NewRow();
                        int time = date.AddHours(i).Hour;

                        DateTime TimeSet = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, time, 0, 0);
                        dt_Row[header_Time] = TimeSet.ToShortTimeString();

                        //TO-DO: read record if exist

                        dt.Rows.Add(dt_Row);
                    }
                }
                else
                {
                    DateTime dateNow = DateTime.Now;
                    DateTime date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 21, 0, 0);

                    for (int i = 0; i < 12; i++)
                    {
                        dt_Row = dt.NewRow();
                        int time = date.AddHours(i).Hour;

                        DateTime TimeSet = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, time, 0, 0);
                        dt_Row[header_Time] = TimeSet.ToShortTimeString();

                        //TO-DO: read record if exist

                        dt.Rows.Add(dt_Row);
                    }
                }

                if(dt.Rows.Count > 0)
                {
                    dgvMeterReading.DataSource = dt;
                    dgvMeterStyleEdit(dgvMeterReading);
                }
            }
            else
            {
                errorProvider2.SetError(lblShift, "Please select shift");
            }
            
        }

        private void UpdatePackagingData()
        {
            if(sheetLoaded)
            {
                int qty = int.TryParse(txtPackingMaxQty.Text, out qty) ? qty : -1;
                string packagingName = cmbPackingName.Text;
                string packagingCode = cmbPackingCode.Text;

                if (qty != -1 && !string.IsNullOrEmpty(packagingName) && !string.IsNullOrEmpty(packagingCode) && packagingCode != string_MultiPackaging)
                {
                    string itemCode = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PartCode].Value.ToString();
                    string itemName = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PartName].Value.ToString();

                    string messageToUser = null;

                    messageToUser = "ITEM            : " + itemName + " (" + itemCode + ")\n";
                    messageToUser += "QTY              : " + qty + " PCS\n";
                    messageToUser += "PACKAGING : " + packagingName + " (" + packagingCode + ")\n\n";
                    messageToUser += "Do you want to update these packaging data to database?";

                    DialogResult dialogResult = MessageBox.Show(messageToUser, "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        uJoin.join_parent_code = itemCode;

                        uJoin.join_child_code = packagingCode;

                        uJoin.join_max = qty;
                        uJoin.join_min = qty;
                        uJoin.join_qty = 1;

                        DataTable dt_existCheck = dalJoin.existCheck(uJoin.join_parent_code, uJoin.join_child_code);

                        bool success = false;

                        if (dt_existCheck.Rows.Count > 0)
                        {
                            uJoin.join_updated_date = DateTime.Now;
                            uJoin.join_updated_by = MainDashboard.USER_ID;
                            //update data
                            success = dalJoin.UpdateWithMaxMin(uJoin);
                            //If the data is successfully inserted then the value of success will be true else false
                            if (success)
                            {
                                //Data Successfully Inserted
                                MessageBox.Show("Join updated.");
                                dt_JoinInfo = dalJoin.SelectAll();


                            }
                            else
                            {
                                //Failed to insert data
                                MessageBox.Show("Failed to update join");
                            }
                        }
                        else
                        {
                            uJoin.join_added_date = DateTime.Now;
                            uJoin.join_added_by = MainDashboard.USER_ID;
                            //insert new data
                            success = dalJoin.InsertWithMaxMin(uJoin);
                            //If the data is successfully inserted then the value of success will be true else false
                            if (success)
                            {
                                //Data Successfully Inserted
                                MessageBox.Show("Join successfully created");
                                dt_JoinInfo = dalJoin.SelectAll();
                                // this.Close();
                            }
                            else
                            {
                                //Failed to insert data
                                MessageBox.Show("Failed to add new join");
                            }
                        }
                    }
                }
            }
           
        }

        private void LoadPackagingList(int sheetID)
        {
            DataTable dt = NewPackagingTable();

            DataTable dt_PackagingFromDB = dalProRecord.PackagingRecordSelect(sheetID);

            foreach (DataRow row in dt_PackagingFromDB.Rows)
            {
                DataRow dt_Row = dt.NewRow();

                string packagingCode = row[dalProRecord.PackagingCode].ToString();

                int QtyPerBox = Convert.ToInt16(row[dalProRecord.PackagingMax].ToString());
                int TotalBox = Convert.ToInt16(row[dalProRecord.PackagingQty].ToString());

                dt_Row[header_PackagingCode] = packagingCode;
                dt_Row[header_PackagingName] = tool.getItemName(packagingCode);
                dt_Row[header_PackagingMax] = QtyPerBox;
                dt_Row[header_PackagingQty] = TotalBox;

                dt.Rows.Add(dt_Row);
            }

            dt_MultiPackaging = dt.Copy();

            if (dt_MultiPackaging.Rows.Count > 0)
            {
                string name = dt_MultiPackaging.Rows[0][header_PackagingName].ToString();
                string code = dt_MultiPackaging.Rows[0][header_PartCode].ToString();

                int rowCount = dt_MultiPackaging.Rows.Count;

                if (rowCount >= 2)
                {
                    LoadPackingToCMB(string_MultiPackaging);
                    txtPackingMaxQty.Text = "MULTI";

                    int totalStockIn = 0;
                    int totalFullBox = 0;

                    foreach (DataRow row in dt_MultiPackaging.Rows)
                    {
                        int boxQty = Convert.ToInt32(row[frmProPackaging.header_PackagingQty].ToString());
                        int maxQty = Convert.ToInt32(row[frmProPackaging.header_PackagingMax].ToString());

                        totalFullBox += boxQty;

                        totalStockIn += boxQty * maxQty;
                    }


                    txtFullBox.Text = totalFullBox.ToString();

                }
                else if (rowCount == 1)
                {
                    loadPackingData();

                    cmbPackingName.Text = name;
                    cmbPackingCode.Text = code;

                    foreach (DataRow row in dt_MultiPackaging.Rows)
                    {
                        txtPackingMaxQty.Text = row[header_PackagingMax].ToString();
                        txtFullBox.Text = row[header_PackagingQty].ToString();
                    }


                }
            }

        }

        private bool SaveSuccess()
        {
            dataSaved = false;
            bool success = false;

            if(Validation())
            {
                frmLoading.ShowLoadingScreen();

                string planID = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();
                int sheetID = int.TryParse(txtSheetID.Text, out sheetID)? sheetID : -1;

                string Shift = text.Shift_Night;
                string PackagingCode = cmbPackingCode.Text;
                int PackagingQty = int.TryParse(txtPackingMaxQty.Text, out PackagingQty)? PackagingQty : 0;
                int meterStart = Convert.ToInt32(txtMeterStart.Text);
                int meterEnd = meterStart;
                int lastShiftBalance = Convert.ToInt32(txtBalanceOfLastShift.Text);
                int thisShiftBalance = Convert.ToInt32(txtBalanceOfThisShift.Text);

                int directIn = int.TryParse(txtIn.Text, out directIn) ? directIn : 0;
                int directOut = int.TryParse(txtOut.Text, out directOut) ? directOut : 0;

                int fullBox = Convert.ToInt32(txtFullBox.Text);
                int TotalStockIn = Convert.ToInt32(txtTotalProduce.Text);
                int TotalReject = Convert.ToInt32(txtTotalReject.Text);
                double RejectPercentage = Convert.ToDouble(txtRejectPercentage.Text);
                string ParentStockIn = cmbParentList.Text;
                string note = txtNote.Text ;
                string itemCode = lblPartCode.Text;
                lotNo = GetINTOnlyFromLotNo(txtProLotNo.Text);

                DateTime updateTime = DateTime.Now;

                if (cbMorning.Checked)
                {
                    Shift = text.Shift_Morning;
                }

                uProRecord.active = true;
                uProRecord.plan_id = Convert.ToInt16(planID);
                uProRecord.production_date = dtpProDate.Value;
                uProRecord.shift = Shift;
                uProRecord.packaging_code = PackagingCode;
                uProRecord.packaging_qty = PackagingQty;
                uProRecord.production_lot_no = lotNo.ToString();
                uProRecord.raw_mat_lot_no = txtRawMatLotNo.Text;
                uProRecord.color_mat_lot_no = txtColorMatLotNo.Text;
                uProRecord.meter_start = meterStart;
                uProRecord.parent_code = ParentStockIn;
                uProRecord.note = note;
                uProRecord.directIn = directIn;
                uProRecord.directOut = directOut;

                foreach (DataGridViewRow row in dgvMeterReading.Rows)
                {
                    if(row.Cells[header_MeterReading].Value != DBNull.Value)
                    {
                        int i = 0;
                        if (int.TryParse(row.Cells[header_MeterReading].Value.ToString(), out i))
                        {
                            meterEnd = i;
                        }
                    }
                    
                }

                int userID = MainDashboard.USER_ID;
                uProRecord.meter_end = meterEnd;

                uProRecord.last_shift_balance = lastShiftBalance;
                uProRecord.current_shift_balance = thisShiftBalance;
                uProRecord.full_box = fullBox;
                uProRecord.total_produced = TotalStockIn;
                uProRecord.total_reject = TotalReject;
                uProRecord.reject_percentage = RejectPercentage;
                uProRecord.updated_date = updateTime;
                uProRecord.updated_by = userID;
                uProRecord.sheet_id = sheetID;

                success = false;

                if(addingNewSheet)
                {
                    success = dalProRecord.InsertProductionRecord(uProRecord);
                }
                else
                {
                    //update data
                    success = dalProRecord.ProductionRecordUpdate(uProRecord);
                }

                if(success)
                {
                    dataSaved = true;
                    
                    if (addingNewSheet)
                    {
                        //new sheet: sheet id + item
                        tool.historyRecord(text.AddDailyJobSheet, "Plan ID: "+ planID + "(" + itemCode + ")", updateTime, userID);
                    }
                    else
                    {
                        //edit/modify sheet
                        tool.historyRecord(text.EditDailyJobSheet, "Sheet ID: " + sheetID + "(" + itemCode + ")", updateTime, userID);
                    }

                    dt_ProductionRecord = dalProRecord.Select();
                    dgvItemList.Focus();
                    LoadDailyRecord();

                    if(uProRecord.sheet_id == -1)
                    {
                        uProRecord.sheet_id = dalProRecord.GetLastInsertedSheetID();
                    }

                    //remove old meter data
                    //remove data under same sheet id
                    dalProRecord.DeleteMeterData(uProRecord);

                    //insert new meter data
                    DataTable dt_Meter = (DataTable)dgvMeterReading.DataSource;

                    foreach(DataRow row in dt_Meter.Rows)
                    {
                        DateTime time = Convert.ToDateTime(row[header_Time].ToString());
                        string proOperator = row[header_Operator].ToString();
                        int meterReading = int.TryParse(row[header_MeterReading].ToString(), out meterReading) ? meterReading : -1;

                        uProRecord.time = time;
                        uProRecord.production_operator = proOperator;
                        uProRecord.meter_reading = meterReading;

                        if(meterReading != -1)
                        {
                            if (!dalProRecord.InsertSheetMeter(uProRecord))
                            {
                                break;
                            }
                        }
                        
                    }


                    //remove packaging data from db and insert new data
                    dalProRecord.DeletePackagingData(uProRecord);

                    if (dt_Carton != null)
                    {
                        //save packaging data
                        foreach(DataRow row in dt_Carton.Rows)
                        {
                            uProRecord.packaging_code = row[header_PackagingCode].ToString();
                            uProRecord.packaging_max = Convert.ToInt32(row[header_PackagingMax].ToString());
                            uProRecord.packaging_qty = Convert.ToInt32(row[header_PackagingQty].ToString());
                            uProRecord.packaging_stock_out = bool.TryParse(row[header_PackagingStockOut].ToString(), out bool stockOUt) ? stockOUt : true;

                            if (!dalProRecord.InsertProductionPackaging(uProRecord))
                            {
                                MessageBox.Show("Failed to save packaging data!");
                            }
                        }
                    }

                    //update lot no
                    if(lotNo > 0)
                    {
                        uMac.mac_id = macID;
                        uMac.mac_lot_no = lotNo;
                        uMac.mac_updated_date = updateTime;
                        uMac.mac_updated_by = MainDashboard.USER_ID;

                        if (!dalMac.UpdateLotNo(uMac))
                        {
                            MessageBox.Show("Failed to update machine lot no!");
                        }

                    }


                    
                    frmLoading.CloseForm();
                    MessageBox.Show("Data saved!");
                    //CheckIfBalanceClear();

                }
                else
                {
                    MessageBox.Show("Failed to save data");
                }
                //save meter data

            }
            //dt_Trf = dalTrf.Select();
            
            return success;
        }

        //private void SaveAndStock()
        //{
        //    bool OKToProcess = true;

        //    if(CheckIfPreviousRecordExist())
        //    {
        //        DialogResult dialogResult = MessageBox.Show("This action will UNDO all the previous record,\nare you sure you want to continue?", "Message",
        //                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //        if (dialogResult != DialogResult.Yes)
        //        {
        //            OKToProcess = false;
        //        }
        //    }

        //    if (OKToProcess && SaveSuccess())
        //    {
        //        UndoPreviousRecordIfExist();

        //        DataTable dt = NewStockInTable();
        //        DataRow dt_Row;

        //        //get totalStockIn qty from box qty and max pcs per box
        //        //check if one or multi packaging box

        //        int totalStockIn = 0;
        //        int directStockIn = int.TryParse(txtIn.Text, out directStockIn) ? directStockIn : 0;
        //        int directStockOut = int.TryParse(txtOut.Text, out directStockOut) ? directStockOut : 0;

        //        int selectedItem = dgvItemList.CurrentCell.RowIndex;

        //        string planStatus = dgvItemList.Rows[selectedItem].Cells[header_Status].Value.ToString();
        //        int balance = int.TryParse(txtBalanceOfThisShift.Text, out balance) ? balance : 0;

        //        if (dt_MultiPackaging != null && dt_MultiPackaging.Rows.Count >= 2)
        //        {
        //            int totalFullBox = 0;

        //            foreach (DataRow row in dt_MultiPackaging.Rows)
        //            {

        //                string packingCode = row[frmProPackaging.header_PackagingCode].ToString();

        //                string itemType = packingCode.Substring(0, 3);

        //                int boxQty = Convert.ToInt32(row[frmProPackaging.header_PackagingQty].ToString());
        //                int maxQty = Convert.ToInt32(row[frmProPackaging.header_PackagingMax].ToString());

        //                if (itemType == "CTN" || itemType == "CTR")
        //                {
        //                    totalFullBox += boxQty;

        //                    totalStockIn += boxQty * maxQty;
        //                }

        //                if (!string.IsNullOrEmpty(packingCode) && itemType == "CTN" && boxQty > 0)
        //                {
        //                    dt_Row = dt.NewRow();
        //                    dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
        //                    dt_Row[header_ItemCode] = packingCode;
        //                    dt_Row[header_Cat] = tool.getItemCat(packingCode);

        //                    dt_Row[header_From] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();
        //                    dt_Row[header_To] = text.Production;

        //                    dt_Row[header_Qty] = boxQty;
        //                    dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

        //                    if (cbMorning.Checked)
        //                    {
        //                        dt_Row[header_Shift] = "M";
        //                    }
        //                    else if (cbNight.Checked)
        //                    {
        //                        dt_Row[header_Shift] = "N";
        //                    }


        //                    dt.Rows.Add(dt_Row);
        //                }
        //                else if (!string.IsNullOrEmpty(packingCode))
        //                {
        //                    dt_Row = dt.NewRow();
        //                    dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
        //                    dt_Row[header_ItemCode] = packingCode;
        //                    dt_Row[header_Cat] = tool.getItemCat(packingCode);

        //                    dt_Row[header_From] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();
        //                    dt_Row[header_To] = text.Production;

        //                    dt_Row[header_Qty] = boxQty * maxQty;
        //                    dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

        //                    if (cbMorning.Checked)
        //                    {
        //                        dt_Row[header_Shift] = "M";
        //                    }
        //                    else if (cbNight.Checked)
        //                    {
        //                        dt_Row[header_Shift] = "N";
        //                    }


        //                    dt.Rows.Add(dt_Row);
        //                }

        //            }
        //        }
        //        else
        //        {

        //            int packingMaxQty = int.TryParse(txtPackingMaxQty.Text, out packingMaxQty) ? packingMaxQty : 0;

        //            int fullBoxQty = int.TryParse(txtFullBox.Text, out fullBoxQty) ? fullBoxQty : 0;

        //            string itemType = "";
        //            if (!string.IsNullOrEmpty(cmbPackingCode.Text))
        //            {
        //                itemType = cmbPackingCode.Text.Substring(0, 3);
        //            }

        //            totalStockIn += packingMaxQty * fullBoxQty;

        //            if (!string.IsNullOrEmpty(cmbPackingCode.Text) && itemType != "CTR" && fullBoxQty > 0)
        //            {
        //                dt_Row = dt.NewRow();
        //                dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
        //                dt_Row[header_ItemCode] = cmbPackingCode.Text;
        //                dt_Row[header_Cat] = tool.getItemCat(cmbPackingCode.Text);

        //                dt_Row[header_From] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();
        //                dt_Row[header_To] = text.Production;

        //                dt_Row[header_Qty] = fullBoxQty;
        //                dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

        //                if (cbMorning.Checked)
        //                {
        //                    dt_Row[header_Shift] = "M";
        //                }
        //                else if (cbNight.Checked)
        //                {
        //                    dt_Row[header_Shift] = "N";
        //                }

        //                dt.Rows.Add(dt_Row);
        //            }

        //        }

        //        if (!string.IsNullOrEmpty(lblPartCode.Text))
        //        {
        //            string ProductionOrAssembly = text.Production;
        //            //get itemCode
        //            //get join qty
        //            //get stock in qty
        //            string StockInItemCode = lblPartCode.Text;

        //            if (!string.IsNullOrEmpty(cmbParentList.Text))
        //            {
        //                if (totalStockIn > 0)
        //                {
        //                    dt_Row = dt.NewRow();
        //                    dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
        //                    dt_Row[header_ItemCode] = StockInItemCode;
        //                    dt_Row[header_Cat] = text.Cat_Part;

        //                    dt_Row[header_From] = text.Production;
        //                    dt_Row[header_To] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();

        //                    dt_Row[header_Qty] = totalStockIn;
        //                    dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

        //                    if (cbMorning.Checked)
        //                    {
        //                        dt_Row[header_Shift] = "M";
        //                    }
        //                    else if (cbNight.Checked)
        //                    {
        //                        dt_Row[header_Shift] = "N";
        //                    }

        //                    dt.Rows.Add(dt_Row);
        //                }

        //                if (directStockIn > 0)
        //                {
        //                    dt_Row = dt.NewRow();
        //                    dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
        //                    dt_Row[header_ItemCode] = StockInItemCode;
        //                    dt_Row[header_Cat] = text.Cat_Part;

        //                    dt_Row[header_From] = text.Production;
        //                    dt_Row[header_To] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();

        //                    dt_Row[header_Qty] = directStockIn;
        //                    dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

        //                    if (planStatus == text.planning_status_completed)
        //                    {
        //                        if (cbMorning.Checked)
        //                        {
        //                            dt_Row[header_Shift] = "MB";
        //                        }
        //                        else if (cbNight.Checked)
        //                        {
        //                            dt_Row[header_Shift] = "NB";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (cbMorning.Checked)
        //                        {
        //                            dt_Row[header_Shift] = "M";
        //                        }
        //                        else if (cbNight.Checked)
        //                        {
        //                            dt_Row[header_Shift] = "N";
        //                        }
        //                    }


        //                    dt.Rows.Add(dt_Row);
        //                }

        //                if (directStockOut > 0)
        //                {
        //                    dt_Row = dt.NewRow();
        //                    dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
        //                    dt_Row[header_ItemCode] = StockInItemCode;
        //                    dt_Row[header_Cat] = text.Cat_Part;

        //                    dt_Row[header_From] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();
        //                    dt_Row[header_To] = text.Other;

        //                    dt_Row[header_Qty] = directStockOut;
        //                    dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

        //                    if (cbMorning.Checked)
        //                    {
        //                        dt_Row[header_Shift] = "MBO";
        //                    }
        //                    else if (cbNight.Checked)
        //                    {
        //                        dt_Row[header_Shift] = "NBO";
        //                    }

        //                    dt.Rows.Add(dt_Row);
        //                }

        //                StockInItemCode = cmbParentList.Text;
        //                ProductionOrAssembly = text.Assembly;
        //                float joinQty = tool.getJoinQty(StockInItemCode, lblPartCode.Text);

        //                if (joinQty <= 0)
        //                {
        //                    joinQty = 1;
        //                }

        //                totalStockIn = totalStockIn / (int)joinQty;
        //            }


        //            if (totalStockIn > 0)
        //            {
        //                dt_Row = dt.NewRow();
        //                dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
        //                dt_Row[header_ItemCode] = StockInItemCode;
        //                dt_Row[header_Cat] = text.Cat_Part;

        //                dt_Row[header_From] = ProductionOrAssembly;
        //                dt_Row[header_To] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();

        //                dt_Row[header_Qty] = totalStockIn;
        //                dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

        //                if (cbMorning.Checked)
        //                {
        //                    dt_Row[header_Shift] = "M";
        //                }
        //                else if (cbNight.Checked)
        //                {
        //                    dt_Row[header_Shift] = "N";
        //                }

        //                dt.Rows.Add(dt_Row);
        //            }


        //            if (directStockIn > 0)
        //            {
        //                dt_Row = dt.NewRow();
        //                dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
        //                dt_Row[header_ItemCode] = StockInItemCode;
        //                dt_Row[header_Cat] = text.Cat_Part;

        //                dt_Row[header_From] = ProductionOrAssembly;
        //                dt_Row[header_To] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();

        //                dt_Row[header_Qty] = directStockIn;
        //                dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

        //                if (planStatus == text.planning_status_completed)
        //                {
        //                    if (cbMorning.Checked)
        //                    {
        //                        dt_Row[header_Shift] = "MB";
        //                    }
        //                    else if (cbNight.Checked)
        //                    {
        //                        dt_Row[header_Shift] = "NB";
        //                    }
        //                }
        //                else
        //                {
        //                    if (cbMorning.Checked)
        //                    {
        //                        dt_Row[header_Shift] = "M";
        //                    }
        //                    else if (cbNight.Checked)
        //                    {
        //                        dt_Row[header_Shift] = "N";
        //                    }
        //                }


        //                dt.Rows.Add(dt_Row);
        //            }

        //            if (directStockOut > 0)
        //            {
        //                dt_Row = dt.NewRow();
        //                dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
        //                dt_Row[header_ItemCode] = StockInItemCode;
        //                dt_Row[header_Cat] = text.Cat_Part;

        //                dt_Row[header_From] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();
        //                dt_Row[header_To] = text.Other;

        //                dt_Row[header_Qty] = directStockOut;
        //                dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

        //                if (cbMorning.Checked)
        //                {
        //                    dt_Row[header_Shift] = "MBO";
        //                }
        //                else if (cbNight.Checked)
        //                {
        //                    dt_Row[header_Shift] = "NBO";
        //                }

        //                dt.Rows.Add(dt_Row);
        //            }
        //        }

        //        #region process to in/out edit page

        //        frmInOutEdit frm = new frmInOutEdit(dt, true);
        //        frm.StartPosition = FormStartPosition.CenterScreen;
        //        frm.WindowState = FormWindowState.Normal;
        //        frm.ShowDialog();//Item Edit

        //        if (!frmInOutEdit.TrfSuccess)
        //        {
        //            MessageBox.Show("Transfer failed or cancelled!");
        //        }

        //        ResetInputField();
        //        #endregion
        //    }
        //}

        private void NewSaveAndStock()
        {
            bool OKToProcess = true;

            if (CheckIfPreviousRecordExist())
            {
                DialogResult dialogResult = MessageBox.Show("This action will UNDO all the previous record,\nare you sure you want to continue?", "Message",
                                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult != DialogResult.Yes)
                {
                    OKToProcess = false;
                }
            }

            if (OKToProcess && SaveSuccess())
            {
                UndoPreviousRecordIfExist();

                DataTable dt = NewStockInTable();
                DataRow dt_Row;

                //get totalStockIn qty from box qty and max pcs per box
                //check if one or multi packaging box

                int totalStockIn = 0;
                int directStockIn = int.TryParse(txtIn.Text, out directStockIn) ? directStockIn : 0;
                int directStockOut = int.TryParse(txtOut.Text, out directStockOut) ? directStockOut : 0;

                int selectedItem = dgvItemList.CurrentCell.RowIndex;

                string planStatus = dgvItemList.Rows[selectedItem].Cells[header_Status].Value.ToString();
                int balance = int.TryParse(txtBalanceOfThisShift.Text, out balance) ? balance : 0;

                if (dt_Carton != null && dt_Carton.Rows.Count >= 2)
                {
                    int totalFullBox = 0;

                    foreach (DataRow row in dt_Carton.Rows)
                    {
                        string packingCode = row[header_PackagingCode].ToString();

                        string itemType = packingCode.Substring(0, 3);

                        int boxQty = Convert.ToInt32(row[header_PackagingQty].ToString());
                        int maxQty = Convert.ToInt32(row[header_PackagingMax].ToString());

                        bool cartonStockOut = bool.TryParse(row[header_PackagingStockOut].ToString(), out cartonStockOut) ? cartonStockOut : true;

                        totalFullBox += boxQty;

                        totalStockIn += boxQty * maxQty;

                        if (!string.IsNullOrEmpty(packingCode) && boxQty > 0 && cartonStockOut)
                        {
                           

                            dt_Row = dt.NewRow();
                            dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
                            dt_Row[header_ItemCode] = packingCode;
                            dt_Row[header_Cat] = tool.getItemCat(packingCode);

                            dt_Row[header_From] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();
                            dt_Row[header_To] = text.Production;

                            dt_Row[header_Qty] = boxQty;
                            dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

                            if (cbMorning.Checked)
                            {
                                dt_Row[header_Shift] = "M";
                            }
                            else if (cbNight.Checked)
                            {
                                dt_Row[header_Shift] = "N";
                            }


                            dt.Rows.Add(dt_Row);
                        }

                    }
                }
                else
                {

                    int packingMaxQty = int.TryParse(txtPackingMaxQty.Text, out packingMaxQty) ? packingMaxQty : 0;

                    int fullBoxQty = int.TryParse(txtFullBox.Text, out fullBoxQty) ? fullBoxQty : 0;

                    bool cartonStockOut = true;

                    if (dt_Carton != null)
                    {
                        foreach (DataRow row in dt_Carton.Rows)
                        {
                            string packingCode = row[header_PackagingCode].ToString();

                            if (cmbPackingCode.Text.Equals(packingCode))
                                cartonStockOut = bool.TryParse(row[header_PackagingStockOut].ToString(), out cartonStockOut) ? cartonStockOut : true;
                        }
                    }
     
                    totalStockIn += packingMaxQty * fullBoxQty;

                    if (!string.IsNullOrEmpty(cmbPackingCode.Text) && fullBoxQty > 0 && cartonStockOut)
                    {
                        dt_Row = dt.NewRow();
                        dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
                        dt_Row[header_ItemCode] = cmbPackingCode.Text;
                        dt_Row[header_Cat] = tool.getItemCat(cmbPackingCode.Text);

                        dt_Row[header_From] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();
                        dt_Row[header_To] = text.Production;

                        dt_Row[header_Qty] = fullBoxQty;
                        dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

                        if (cbMorning.Checked)
                        {
                            dt_Row[header_Shift] = "M";
                        }
                        else if (cbNight.Checked)
                        {
                            dt_Row[header_Shift] = "N";
                        }

                        dt.Rows.Add(dt_Row);
                    }

                }

                if (!string.IsNullOrEmpty(lblPartCode.Text))
                {
                    string ProductionOrAssembly = text.Production;
                    //get itemCode
                    //get join qty
                    //get stock in qty
                    string StockInItemCode = lblPartCode.Text;

                    if (!string.IsNullOrEmpty(cmbParentList.Text))
                    {
                        if (totalStockIn > 0)
                        {
                            dt_Row = dt.NewRow();
                            dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
                            dt_Row[header_ItemCode] = StockInItemCode;
                            dt_Row[header_Cat] = text.Cat_Part;

                            dt_Row[header_From] = text.Production;
                            dt_Row[header_To] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();

                            dt_Row[header_Qty] = totalStockIn;
                            dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

                            if (cbMorning.Checked)
                            {
                                dt_Row[header_Shift] = "M";
                            }
                            else if (cbNight.Checked)
                            {
                                dt_Row[header_Shift] = "N";
                            }

                            dt.Rows.Add(dt_Row);
                        }

                        if (directStockIn > 0)
                        {
                            dt_Row = dt.NewRow();
                            dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
                            dt_Row[header_ItemCode] = StockInItemCode;
                            dt_Row[header_Cat] = text.Cat_Part;

                            dt_Row[header_From] = text.Production;
                            dt_Row[header_To] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();

                            dt_Row[header_Qty] = directStockIn;
                            dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

                            if (planStatus == text.planning_status_completed)
                            {
                                if (cbMorning.Checked)
                                {
                                    dt_Row[header_Shift] = "MB";
                                }
                                else if (cbNight.Checked)
                                {
                                    dt_Row[header_Shift] = "NB";
                                }
                            }
                            else
                            {
                                if (cbMorning.Checked)
                                {
                                    dt_Row[header_Shift] = "M";
                                }
                                else if (cbNight.Checked)
                                {
                                    dt_Row[header_Shift] = "N";
                                }
                            }


                            dt.Rows.Add(dt_Row);
                        }

                        if (directStockOut > 0)
                        {
                            dt_Row = dt.NewRow();
                            dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
                            dt_Row[header_ItemCode] = StockInItemCode;
                            dt_Row[header_Cat] = text.Cat_Part;

                            dt_Row[header_From] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();
                            dt_Row[header_To] = text.Other;

                            dt_Row[header_Qty] = directStockOut;
                            dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

                            if (cbMorning.Checked)
                            {
                                dt_Row[header_Shift] = "MBO";
                            }
                            else if (cbNight.Checked)
                            {
                                dt_Row[header_Shift] = "NBO";
                            }

                            dt.Rows.Add(dt_Row);
                        }

                        StockInItemCode = cmbParentList.Text;
                        ProductionOrAssembly = text.Assembly;
                        float joinQty = tool.getJoinQty(StockInItemCode, lblPartCode.Text);

                        if (joinQty <= 0)
                        {
                            joinQty = 1;
                        }

                        totalStockIn = totalStockIn / (int)joinQty;
                    }


                    if (totalStockIn > 0)
                    {
                        dt_Row = dt.NewRow();
                        dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
                        dt_Row[header_ItemCode] = StockInItemCode;
                        dt_Row[header_Cat] = text.Cat_Part;

                        dt_Row[header_From] = ProductionOrAssembly;
                        dt_Row[header_To] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();

                        dt_Row[header_Qty] = totalStockIn;
                        dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

                        if (cbMorning.Checked)
                        {
                            dt_Row[header_Shift] = "M";
                        }
                        else if (cbNight.Checked)
                        {
                            dt_Row[header_Shift] = "N";
                        }

                        dt.Rows.Add(dt_Row);
                    }


                    if (directStockIn > 0)
                    {
                        dt_Row = dt.NewRow();
                        dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
                        dt_Row[header_ItemCode] = StockInItemCode;
                        dt_Row[header_Cat] = text.Cat_Part;

                        dt_Row[header_From] = ProductionOrAssembly;
                        dt_Row[header_To] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();

                        dt_Row[header_Qty] = directStockIn;
                        dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

                        if (planStatus == text.planning_status_completed)
                        {
                            if (cbMorning.Checked)
                            {
                                dt_Row[header_Shift] = "MB";
                            }
                            else if (cbNight.Checked)
                            {
                                dt_Row[header_Shift] = "NB";
                            }
                        }
                        else
                        {
                            if (cbMorning.Checked)
                            {
                                dt_Row[header_Shift] = "M";
                            }
                            else if (cbNight.Checked)
                            {
                                dt_Row[header_Shift] = "N";
                            }
                        }


                        dt.Rows.Add(dt_Row);
                    }

                    if (directStockOut > 0)
                    {
                        dt_Row = dt.NewRow();
                        dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
                        dt_Row[header_ItemCode] = StockInItemCode;
                        dt_Row[header_Cat] = text.Cat_Part;

                        dt_Row[header_From] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();
                        dt_Row[header_To] = text.Other;

                        dt_Row[header_Qty] = directStockOut;
                        dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

                        if (cbMorning.Checked)
                        {
                            dt_Row[header_Shift] = "MBO";
                        }
                        else if (cbNight.Checked)
                        {
                            dt_Row[header_Shift] = "NBO";
                        }

                        dt.Rows.Add(dt_Row);
                    }
                }

                #region process to in/out edit page

                frmInOutEdit frm = new frmInOutEdit(dt, true);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.WindowState = FormWindowState.Normal;
                frm.ShowDialog();//Item Edit

                if (!frmInOutEdit.TrfSuccess)
                {
                    MessageBox.Show("Transfer failed or cancelled!");
                }

                ResetInputField();
                #endregion
            }
        }

        private void DirectIn()
        {
            if (Validation())
            {
                if (SaveSuccess())//SaveSuccess()
                {
                    DataTable dt = NewStockInTable();
                    DataRow dt_Row;

                    int directStockIn = int.TryParse(txtIn.Text, out directStockIn) ? directStockIn : 0;

                    int selectedItem = dgvItemList.CurrentCell.RowIndex;

                    string planStatus = dgvItemList.Rows[selectedItem].Cells[header_Status].Value.ToString();
                    int balance = int.TryParse(txtBalanceOfThisShift.Text, out balance) ? balance : 0;

                    if (!string.IsNullOrEmpty(lblPartCode.Text))
                    {
                        //get itemCode
                        //get join qty
                        //get stock in qty
                        string StockInItemCode;

                        if (!string.IsNullOrEmpty(cmbParentList.Text))
                        {
                            StockInItemCode = cmbParentList.Text;

                            float joinQty = tool.getJoinQty(StockInItemCode, lblPartCode.Text);

                            directStockIn = directStockIn / (int)joinQty;
                        }
                        else
                        {
                            StockInItemCode = lblPartCode.Text;
                        }

                        dt_Row = dt.NewRow();
                        dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
                        dt_Row[header_ItemCode] = StockInItemCode;
                        dt_Row[header_Cat] = text.Cat_Part;

                        dt_Row[header_From] = text.Production;
                        dt_Row[header_To] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();

                        dt_Row[header_Qty] = directStockIn;
                        dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

                        if (cbMorning.Checked)
                        {
                            dt_Row[header_Shift] = "MB";
                        }
                        else if (cbNight.Checked)
                        {
                            dt_Row[header_Shift] = "NB";
                        }

                        dt.Rows.Add(dt_Row);
                    }


                    //MatTrfDate = dtpTrfDate.Text;
                    frmInOutEdit frm = new frmInOutEdit(dt, true);
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.WindowState = FormWindowState.Normal;
                    frm.ShowDialog();//Item Edit

                    if (frmInOutEdit.TrfSuccess)
                        ResetInputField();
                }

            }
        }

        private void DirectInWithoutSave()
        {
            if (Validation())
            {
                if (true)//SaveSuccess()
                {
                    DataTable dt = NewStockInTable();
                    DataRow dt_Row;

                    int directStockIn = int.TryParse(txtIn.Text, out directStockIn) ? directStockIn : 0;

                    int selectedItem = dgvItemList.CurrentCell.RowIndex;

                    string planStatus = dgvItemList.Rows[selectedItem].Cells[header_Status].Value.ToString();
                    int balance = int.TryParse(txtBalanceOfThisShift.Text, out balance) ? balance : 0;

                    if (!string.IsNullOrEmpty(lblPartCode.Text))
                    {
                        //get itemCode
                        //get join qty
                        //get stock in qty
                        string StockInItemCode;

                        if (!string.IsNullOrEmpty(cmbParentList.Text))
                        {
                            StockInItemCode = cmbParentList.Text;

                            float joinQty = tool.getJoinQty(StockInItemCode, lblPartCode.Text);

                            directStockIn = directStockIn / (int)joinQty;
                        }
                        else
                        {
                            StockInItemCode = lblPartCode.Text;
                        }

                        dt_Row = dt.NewRow();
                        dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
                        dt_Row[header_ItemCode] = StockInItemCode;
                        dt_Row[header_Cat] = text.Cat_Part;

                        dt_Row[header_From] = text.Production;
                        dt_Row[header_To] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();

                        dt_Row[header_Qty] = directStockIn;
                        dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

                        if (cbMorning.Checked)
                        {
                            dt_Row[header_Shift] = "MB";
                        }
                        else if (cbNight.Checked)
                        {
                            dt_Row[header_Shift] = "NB";
                        }

                        dt.Rows.Add(dt_Row);
                    }


                    //MatTrfDate = dtpTrfDate.Text;
                    frmInOutEdit frm = new frmInOutEdit(dt, true);
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.WindowState = FormWindowState.Normal;
                    frm.ShowDialog();//Item Edit

                    if (frmInOutEdit.TrfSuccess)
                        ResetInputField();
                }

            }
        }

        private void DirectOutWithoutSave()
        {
           
            if (Validation())
            {
                if (true)//SaveSuccess()
                {
                    DataTable dt = NewStockInTable();
                    DataRow dt_Row;

                    int directStockOut = int.TryParse(txtOut.Text, out directStockOut) ? directStockOut : 0;

                    int selectedItem = dgvItemList.CurrentCell.RowIndex;

                    if (!string.IsNullOrEmpty(lblPartCode.Text))
                    {
                        //get itemCode
                        //get join qty
                        //get stock in qty
                        string StockInItemCode;

                        if (!string.IsNullOrEmpty(cmbParentList.Text))
                        {
                            StockInItemCode = cmbParentList.Text;

                            float joinQty = tool.getJoinQty(StockInItemCode, lblPartCode.Text);

                            directStockOut = directStockOut / (int)joinQty;
                        }
                        else
                        {
                            StockInItemCode = lblPartCode.Text;
                        }

                        dt_Row = dt.NewRow();
                        dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
                        dt_Row[header_ItemCode] = StockInItemCode;
                        dt_Row[header_Cat] = text.Cat_Part;

                        dt_Row[header_From] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();
                        dt_Row[header_To] = text.Other;

                        dt_Row[header_Qty] = directStockOut;
                        dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

                        if (cbMorning.Checked)
                        {
                            dt_Row[header_Shift] = "MBO";
                        }
                        else if (cbNight.Checked)
                        {
                            dt_Row[header_Shift] = "NBO";
                        }

                        dt.Rows.Add(dt_Row);
                    }


                    //MatTrfDate = dtpTrfDate.Text;
                    frmInOutEdit frm = new frmInOutEdit(dt, true);
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.WindowState = FormWindowState.Normal;
                    frm.ShowDialog();//Item Edit

                    if (frmInOutEdit.TrfSuccess)
                        ResetInputField();
                }

            }
        }

        private void DirectOut()
        {
            bool outSuccess = false;
            if (Validation())
            {
                if (SaveSuccess())//SaveSuccess()
                {
                    DataTable dt = NewStockInTable();
                    DataRow dt_Row;

                    int directStockOut = int.TryParse(txtOut.Text, out directStockOut) ? directStockOut : 0;

                    int selectedItem = dgvItemList.CurrentCell.RowIndex;

                    if (!string.IsNullOrEmpty(lblPartCode.Text))
                    {
                        //get itemCode
                        //get join qty
                        //get stock in qty
                        string StockInItemCode;

                        if (!string.IsNullOrEmpty(cmbParentList.Text))
                        {
                            StockInItemCode = cmbParentList.Text;

                            float joinQty = tool.getJoinQty(StockInItemCode, lblPartCode.Text);

                            directStockOut = directStockOut / (int)joinQty;
                        }
                        else
                        {
                            StockInItemCode = lblPartCode.Text;
                        }

                        dt_Row = dt.NewRow();
                        dt_Row[header_ProDate] = dtpProDate.Value.ToString("ddMMMMyy");
                        dt_Row[header_ItemCode] = StockInItemCode;
                        dt_Row[header_Cat] = text.Cat_Part;

                        dt_Row[header_From] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_Factory].Value.ToString();
                        dt_Row[header_To] =  text.Other;

                        dt_Row[header_Qty] = directStockOut;
                        dt_Row[header_PlanID] = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PlanID].Value.ToString();

                        if (cbMorning.Checked)
                        {
                            dt_Row[header_Shift] = "MBO";
                        }
                        else if (cbNight.Checked)
                        {
                            dt_Row[header_Shift] = "NBO";
                        }

                        dt.Rows.Add(dt_Row);
                    }


                    //MatTrfDate = dtpTrfDate.Text;
                    frmInOutEdit frm = new frmInOutEdit(dt, true);
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.WindowState = FormWindowState.Normal;
                    frm.ShowDialog();//Item Edit

                    if (frmInOutEdit.TrfSuccess)
                        ResetInputField();
                }

            }
        }

        private void loadPackingData()
        {
            DataTable dt_Packaging = dalItem.CatSearch(text.Cat_Packaging);
            DataTable dt_Packaging_ItemName = dt_Packaging.DefaultView.ToTable(true, "item_name");

            DataTable db_Carton = dalItem.CatSearch(text.Cat_Carton);
            DataTable dt_Carton_ItemName = db_Carton.DefaultView.ToTable(true, "item_name");

            DataTable dt_All = dt_Packaging.Copy();
            dt_All.Merge(db_Carton);

            dt_All.DefaultView.Sort = "item_name ASC";

            cmbPackingName.DataSource = dt_All;
            cmbPackingName.DisplayMember = "item_name";

            cmbPackingName.SelectedIndex = -1;

            cmbPackingCode.DataSource = null;
           
        }

        private void LoadPackingToCMB(string dataToShow)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Data");
            dt.Rows.Add(dataToShow);

            cmbPackingName.DataSource = dt;
            cmbPackingName.DisplayMember = "Data";

            cmbPackingCode.DataSource = dt;
            cmbPackingCode.DisplayMember = "Data";

        }

        private int GetLatestBalanceLeft(DataTable dt, string planID)
        {
            int balance = 0;

            DateTime date = DateTime.MaxValue;

            foreach(DataRow row in dt.Rows)
            {
                bool active = Convert.ToBoolean(row[dalProRecord.Active]);
                if(planID == row[dalProRecord.PlanID].ToString() && active)
                {
                    DateTime proDate = Convert.ToDateTime(row[dalProRecord.ProDate]);
                    int balanceLeft = int.TryParse(row[dalProRecord.CurrentShiftBalance].ToString(), out balanceLeft) ? balanceLeft : 0;

                    if (date == DateTime.MaxValue)
                    {
                        date = proDate;
                        balance = balanceLeft;
                    }
                    else if (proDate > date)
                    {
                        date = proDate;
                        balance = balanceLeft;
                    }
                    else if (proDate == date)
                    {
                        string shift = row[dalProRecord.Shift].ToString();

                        if (shift == text.Shift_Night)
                        {
                            balance = balanceLeft;
                        }
                    }
                }
              
            }

            return balance;
        }

        private void LoadPage()
        {
            //dt_Trf = dalTrf.Select();
            loaded = false;
           // btnShowDailyRecord.Visible = false;
            AddNewSheetUI(false);
            LoadItemListData();
            ShowProductionListUI();
            uProRecord.active = true;
            dgvItemList.ClearSelection();
            dgvRecordHistory.DataSource = null;

            lblCheckBy.Text = "";
            lblCheckDate.Text = "";
            txtTotalProducedRecord.Text = "";
            txtTotalStockInRecord.Text = "";
            cbChecked.Checked = false;
            loaded = true;
        }

        private void frmProductionRecord_Load(object sender, EventArgs e)
        {
            LoadPage();
           
        }

        private void txtMeterStart_TextChanged(object sender, EventArgs e)
        {
            if(sheetLoaded)
            {
                dataSaved = false;
            }
            
            errorProvider5.Clear();
            CalculateHourlyShot();
        }

        private void frmProductionRecord_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainDashboard.NewDailyJobSheetFormOpen = false;
        }

        private void ShowDailyRecordUI()
        {
            LoadPlanInfo();
            tlpList.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 0);
            tlpList.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 100);

            tlpStockInCheck.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100);
            tlpStockInCheck.ColumnStyles[1] = new ColumnStyle(SizeType.Absolute, 100f);
            tlpStockInCheck.ColumnStyles[2] = new ColumnStyle(SizeType.Absolute, 95f);
            tlpStockInCheck.ColumnStyles[3] = new ColumnStyle(SizeType.Absolute, 95f);
            tlpStockInCheck.ColumnStyles[4] = new ColumnStyle(SizeType.Absolute, 40f);

        }

        private void ShowProductionListUI()
        {

            tlpList.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 0);
            tlpList.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100);
        }

        private void txtTotalStockIn_TextChanged(object sender, EventArgs e)
        {
            if (sheetLoaded)
            {
                dataSaved = false;
            }
            errorProvider9.Clear();
            CalculateTotalRejectAndPercentage();
        }

        private void OnlyNumeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void UnableKeyIn_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cmbPackingName_SelectedIndexChanged(object sender, EventArgs e)
        {
            tool.loadItemCodeDataToComboBox(cmbPackingCode,cmbPackingName.Text);

            if(sheetLoaded)
            {
                dataSaved = false;
            }
        }

        private void cbMorning_CheckedChanged(object sender, EventArgs e)
        {
            errorProvider2.Clear();

            if(cbMorning.Checked)
            {
                if (sheetLoaded)
                {
                    dataSaved = false;
                }
                cbNight.Checked = false;
                CreateMeterReadingData();
            }
            else if(!cbNight.Checked)
            {
                dgvMeterReading.DataSource = null;
            }

            if (CheckIfDateOrShiftDuplicate())
            {
                ClearAllError();
                errorProvider1.SetError(lblDate, "Please select a different date");
                errorProvider2.SetError(lblShift, "Please select a different shift");
            }

        }

        private void cbNight_CheckedChanged(object sender, EventArgs e)
        {
            errorProvider2.Clear();

            if (cbNight.Checked)
            {
                if (sheetLoaded)
                {
                    dataSaved = false;
                }
                cbMorning.Checked = false;
                CreateMeterReadingData();
            }
            else if (!cbMorning.Checked)
            {
                dgvMeterReading.DataSource = null;
            }

            if (CheckIfDateOrShiftDuplicate())
            {
                ClearAllError();
                errorProvider1.SetError(lblDate, "Please select a different date");
                errorProvider2.SetError(lblShift, "Please select a different shift");
            }
        }

        private void btnNewSheet_Click(object sender, EventArgs e)
        {
            if (dgvItemList.SelectedRows.Count > 0)
            {
                dt_Mac = dalMac.Select();

                DateTime proDate = DateTime.Today.AddDays(-1);

                dtpProDate.Value = tool.SubtractDayIfSunday(proDate);

                dt_MultiPackaging = null;
                dt_Carton = null;
                sheetLoaded = false;

                addingNewSheet = true;
                AddNewSheetUI(true);
                LoadNewSheetData();

                sheetLoaded = true;
            }
            else
            {
                MessageBox.Show("Please select a item before adding new sheet.");
            }
                
        }

        private void tlpSheet_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            AddNewSheetUI(false);
            dailyRecordLoaded = true;
            dataSaved = true;
        }

        private void btnSaveAndStock_Click(object sender, EventArgs e)
        {
            //SaveAndStock();
            NewSaveAndStock();
        }

        private void ResetInputField()
        {
            if(!dataSaved)
            {
                DialogResult dialogResult = MessageBox.Show(text.Message_DataNotSaved, "Message",
                                                           MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.OK)
                {
                    dt_ProductionRecord = dalProRecord.Select();
                    dataSaved = true;
                    AddNewSheetUI(false);
                    LoadDailyRecord();
                    dgvRecordHistory.ClearSelection();
                    dailyRecordLoaded = true;
                   
                }

            }
            else
            {
                dt_ProductionRecord = dalProRecord.Select();
                AddNewSheetUI(false);
                LoadDailyRecord();
                dgvRecordHistory.ClearSelection();
                dailyRecordLoaded = true;
            }
           
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("lot no: "+ lotNo);
            if(SaveSuccess())
            {
                ResetInputField();
            }
        }

        private int GetINTOnlyFromLotNo(string proLotNo)
        {
            string LotNo_String = Regex.Match(proLotNo, @"\d+").Value;

            int LotNo_INT = int.TryParse(LotNo_String, out LotNo_INT) ? LotNo_INT : -1;

            return LotNo_INT;
        }

        private void ClearData()
        {
            sheetLoaded = false;
            cbMorning.Checked = false;
            cbNight.Checked = false;
            dgvMeterReading.DataSource = null;

            cmbPackingName.SelectedIndex = -1;
            cmbPackingCode.SelectedIndex = -1;
            cmbParentList.SelectedIndex = -1;

            txtProLotNo.Clear();
            txtPackingMaxQty.Clear();
            txtRawMatLotNo.Clear();
            txtColorMatLotNo.Clear();
            txtMeterStart.Clear();
            txtBalanceOfLastShift.Clear();
            txtBalanceOfThisShift.Clear();
            txtIn.Text = "0";
            txtFullBox.Clear();
            txtTotalProduce.Clear();
            txtTotalReject.Clear();
            txtRejectPercentage.Clear();
            txtNote.Clear();
        }

        private void ClearInfo()
        {
            txtPlanID.Clear();
            lblCustomer.Text = "";
            lblPartName.Text = "";
            lblPartCode.Text = "";
            lblPW.Text = "";
            lblRW.Text = "";
            lblRawMat.Text = "";
            lblCavity.Text = "";
            lblColorMat.Text = "";
            lblColorUsage.Text = "";

        }

        private void ProductionListRowSelected()
        {
            if (loaded)
            {
                Cursor = Cursors.WaitCursor;
                if (!dataSaved)
                {
                    DialogResult dialogResult = MessageBox.Show(text.Message_DataNotSaved, "Message",
                                                         MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.OK)
                    {
                        dataSaved = true;
                        errorProvider3.Clear();
                        errorProvider4.Clear();
                        AddNewSheetUI(false);

                        //load record history
                        LoadDailyRecord();
                        ClearInfo();
                        dgvRecordHistory.ClearSelection();


                        dailyRecordLoaded = true;
                    }
                }
                else
                {

                    errorProvider3.Clear();
                    errorProvider4.Clear();
                    AddNewSheetUI(false);

                    //load record history
                    LoadDailyRecord();
                    ClearInfo();
                    dgvRecordHistory.ClearSelection();


                    dailyRecordLoaded = true;
                }
              
              
                Cursor = Cursors.Arrow;
            }
        }
        private void dgvItemList_SelectionChanged(object sender, EventArgs e)
        {
            ProductionListRowSelected();
        }

        private void dgvRecordHistory_SelectionChanged(object sender, EventArgs e)
        {
            if (dailyRecordLoaded)
            {
                errorProvider3.Clear();
                errorProvider4.Clear();

                if(dgvRecordHistory.CurrentCell != null)
                {
                    int rowIndex = dgvRecordHistory.CurrentCell.RowIndex;

                    if (rowIndex >= 0)
                    {
                        if(!dataSaved)
                        {
                            DialogResult dialogResult = MessageBox.Show(text.Message_DataNotSaved, "Message",
                                                          MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (dialogResult == DialogResult.OK)
                            {
                                dataSaved = true;
                                addingNewSheet = false;
                                AddNewSheetUI(true);
                                NewLoadExistingSheetData();
                            }
                        }
                        else
                        {
                            addingNewSheet = false;
                            AddNewSheetUI(true);
                            NewLoadExistingSheetData();
                        }
                    }
                }
            }  
        }

        private void dtpProDate_ValueChanged(object sender, EventArgs e)
        {
            errorProvider1.Clear();
        }

        private void txtBalanceOfLastShift_TextChanged(object sender, EventArgs e)
        {
            errorProvider6.Clear();
            //CalculateTotalRejectAndPercentage();
            CalculateTotalProduce();
        }

        private void txtBalanceOfThisShift_TextChanged(object sender, EventArgs e)
        {
            errorProvider7.Clear();
            CalculateTotalProduce();
            //CalculateTotalRejectAndPercentage();

            if (sheetLoaded)
            {
                balanceInEdited = true;
            }
        }

        private void txtFullBox_TextChanged(object sender, EventArgs e)
        {
            errorProvider8.Clear();

            CalculateTotalProduce();

            if(cmbPackingName.Text != string_MultiPackaging)
            {
                
                SavePackagingToDT();
            }
            
        }

        private void dgvMeterReading_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(ColumnMeter_KeyPress);
            e.Control.KeyPress -= new KeyPressEventHandler(Column2_KeyPress);


            if (dgvMeterReading.CurrentCell.ColumnIndex == 2)//meter
            {
                TextBox tb = e.Control as TextBox;

                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(ColumnMeter_KeyPress);
                }
            }
            else if(dgvMeterReading.CurrentCell.ColumnIndex != 1)
            {
                TextBox tb = e.Control as TextBox;

                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column2_KeyPress);
                }
            }
        }

        private void ColumnOperator_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        private void ColumnMeter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        private void Column2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;

        }

        private void dgvMeterReading_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //calculate hourly
            CalculateHourlyShot();
        }

        private void CalculateHourlyShot()
        {
            if(true)
            {
                int meterStart = 0;

                if (string.IsNullOrEmpty(txtMeterStart.Text))
                {
                    errorProvider5.Clear();
                    errorProvider5.SetError(lblMeterStartAt, "Please input meter start value");
                }
                else
                {
                    meterStart = int.TryParse(txtMeterStart.Text, out meterStart) ? meterStart : 0;
                }

                DataTable dt = (DataTable)dgvMeterReading.DataSource;

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        int meterReading = int.TryParse(row[header_MeterReading].ToString(), out meterReading) ? meterReading : -1;

                        if (meterReading != -1)
                        {
                            row[header_Hourly] = meterReading - meterStart;
                            meterStart = meterReading;
                        }
                        else
                        {
                            row[header_Hourly] = 0;
                        }
                    }
                }


                CalculateTotalShot();
            }
            

        }

        private void CalculateTotalShot()
        {
            if(true)
            {
                DataTable dt = (DataTable)dgvMeterReading.DataSource;

                int totalShot = 0;

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        int hourly = int.TryParse(row[header_Hourly].ToString(), out hourly) ? hourly : 0;

                        totalShot += hourly;
                    }
                }


                txtTotalShot.Text = totalShot.ToString();
            }
        }

        private void CalculateTotalProduce()
        {
            if(true)
            {
                int balanceLeftLastShift = int.TryParse(txtBalanceOfLastShift.Text, out balanceLeftLastShift) ? balanceLeftLastShift : 0;
                int balanceLeftThisShift = int.TryParse(txtBalanceOfThisShift.Text, out balanceLeftThisShift) ? balanceLeftThisShift : 0;

                int directIn = int.TryParse(txtIn.Text, out directIn) ? directIn : 0;
                int directOut = int.TryParse(txtOut.Text, out directOut) ? directOut : 0;

                balanceLeftThisShift += directIn;
                balanceLeftLastShift += directOut;
                if (dt_Carton != null && dt_Carton.Rows.Count >= 2)
                {
                    LoadPackingToCMB(string_MultiPackaging);
                    txtPackingMaxQty.Text = "MULTI";

                    int totalStockIn = 0;
                    int totalFullBox = 0;

                    foreach (DataRow row in dt_Carton.Rows)
                    {
                       
                        string packingCode = row[header_PackagingCode].ToString();

                        if(!string.IsNullOrEmpty(packingCode))
                        {
                            int boxQty = Convert.ToInt32(row[header_PackagingQty].ToString());
                            int maxQty = Convert.ToInt32(row[header_PackagingMax].ToString());

                            string itemType = packingCode.Substring(0, 3);


                            if (itemType == "CTN" || itemType == "CTR")
                            {
                                totalFullBox += boxQty;

                                totalStockIn += boxQty * maxQty;
                            }
                        }
                       

                    }

                    totalStockIn += balanceLeftThisShift - balanceLeftLastShift;
                    txtFullBox.Text = totalFullBox.ToString();
                    txtTotalProduce.Text = totalStockIn.ToString();

                }
                else
                {
                    int packingMaxQty = int.TryParse(txtPackingMaxQty.Text, out packingMaxQty) ? packingMaxQty : 0;

                    int fullBoxQty = int.TryParse(txtFullBox.Text, out fullBoxQty) ? fullBoxQty : 0;

                    txtTotalProduce.Text = (packingMaxQty * fullBoxQty + balanceLeftThisShift - balanceLeftLastShift).ToString();
                }
            }
        }

        private void CalculateTotalRejectAndPercentage()
        {
            if(true)
            {
                int availableQty = int.TryParse(txtAvailableQty.Text, out availableQty) ? availableQty : 0;
                int totalIn = int.TryParse(txtTotalProduce.Text, out totalIn) ? totalIn : 0;

                int totalReject = availableQty - totalIn;
                txtTotalReject.Text = totalReject.ToString();

                string rejectPercentageString = "NULL";

                if (availableQty <= 0)
                {
                    rejectPercentageString = "NULL";
                }
                else
                {
                    decimal rejectPercentage = (decimal)totalReject / availableQty * 100;

                    rejectPercentage = Decimal.Round(rejectPercentage, 2);

                    rejectPercentageString = rejectPercentage.ToString();
                }

                txtRejectPercentage.Text = rejectPercentageString;
            }
        }

        private void dgvMeterReading_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = dgvMeterReading;
            int row = e.RowIndex;
            int col = e.ColumnIndex;

            if(dgv.Columns[col].Name == header_Hourly)
            {
                int hourly = int.TryParse(dgv.Rows[row].Cells[col].Value.ToString(), out hourly) ? hourly : 0;

                if(hourly < 0)
                {
                    dgv.Rows[row].Cells[col].Style.ForeColor = Color.Red;
                }
                else
                {
                    dgv.Rows[row].Cells[col].Style.ForeColor = Color.Black;
                }
            }
        }

        private void txtPackingQty_TextChanged(object sender, EventArgs e)
        {
            errorProvider10.Clear();
            if(sheetLoaded)
            {
                dataSaved = false;
                CalculateTotalProduce();

                if (cmbPackingName.Text != string_MultiPackaging)
                {
                    SavePackagingToDT();
                }
            }
        }

        private void txtTotalShot_TextChanged(object sender, EventArgs e)
        {
            int totalShot = int.TryParse(txtTotalShot.Text, out totalShot) ? totalShot : 0;
            int cavity = int.TryParse(lblCavity.Text, out cavity) ? cavity : 0;

            txtAvailableQty.Text = (totalShot * cavity).ToString();

            
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            //open machine schedule page
            
            frmMachineSchedule frm = new frmMachineSchedule(true)
            {
                StartPosition = FormStartPosition.CenterScreen
            };


            frm.ShowDialog();
            loaded = false;
            LoadItemListData();
            dgvItemList.ClearSelection();
            loaded = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in dgvItemList.SelectedRows)
            {
                string name = row.Cells[header_PartName].Value.ToString();
                MessageBox.Show(name);
            }
        }

        private void RemoveCompleted()
        {
            loaded = false;
            DataGridView dgv = dgvItemList;
            DataTable dt = (DataTable)dgv.DataSource;

            if(dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string planStatus = row[header_Status].ToString();

                    if (planStatus == text.planning_status_completed)
                    {
                        int planID = Convert.ToInt32(row[header_PlanID].ToString());

                        uPlan.plan_id = planID;
                        uPlan.recording = false;

                        if (!dalPlan.RecordingUpdate(uPlan))
                        {

                            MessageBox.Show("Failed to update daily plan!");


                        }
                    }
                }

                LoadItemListData();
                loaded = true;
            }
           
        }

        private void ClearList()
        {
            loaded = false;
            DataGridView dgv = dgvItemList;
            DataTable dt = (DataTable)dgv.DataSource;

            if(dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string planStatus = row[header_Status].ToString();

                    int planID = Convert.ToInt32(row[header_PlanID].ToString());

                    uPlan.plan_id = planID;
                    uPlan.recording = false;

                    if (!dalPlan.RecordingUpdate(uPlan))
                    {

                        MessageBox.Show("Failed to update daily plan!");


                    }
                }

                LoadItemListData();
                loaded = true;
            }
            
        }
        private void dgvItemList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            Cursor = Cursors.WaitCursor; // change cursor to hourglass type
            loaded = false;

            int userID = MainDashboard.USER_ID;
            int userPermission = dalUser.getPermissionLevel(userID);

            //handle the row selection on right click
            
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && (userPermission >= MainDashboard.ACTION_LVL_FIVE || userID == 6))// && userPermission >= MainDashboard.ACTION_LVL_THREE
            {
                ContextMenuStrip my_menu = new ContextMenuStrip();
                dgvItemList.CurrentCell = dgvItemList.Rows[e.RowIndex].Cells[e.ColumnIndex];
                // Can leave these here - doesn't hurt
                dgvItemList.Rows[e.RowIndex].Selected = true;
                dgvItemList.Focus();
                int rowIndex = dgvItemList.CurrentCell.RowIndex;

                try
                {
                    my_menu.Items.Add(text_RemoveRecord).Name = text_RemoveRecord;
                    my_menu.Items.Add(text_ChangePlanID).Name = text_ChangePlanID;

                    my_menu.Show(Cursor.Position.X, Cursor.Position.Y);
                    contextMenuStrip1 = my_menu;
                    my_menu.ItemClicked += new ToolStripItemClickedEventHandler(dgvItemList_ItemClicked);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            loaded = true;
            Cursor = Cursors.Arrow; // change cursor to normal type
        }

        private void dgvItemList_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
            Cursor = Cursors.WaitCursor; // change cursor to hourglass type

            DataGridView dgv = dgvItemList;
            dgv.SuspendLayout();

            string itemClicked = e.ClickedItem.Name.ToString();
            int rowIndex = dgv.CurrentCell.RowIndex;
            int planID = Convert.ToInt32(dgv.Rows[rowIndex].Cells[header_PlanID].Value);
            string itemCode = dgv.Rows[rowIndex].Cells[header_PartCode].Value.ToString();
            string itemName = dgv.Rows[rowIndex].Cells[header_PartName].Value.ToString();
            contextMenuStrip1.Hide();

            if (itemClicked.Equals(text_RemoveRecord))
            {
                if (MessageBox.Show("Are you sure you want to remove (PLAN ID: "+planID+") "+itemName+" from this list?", "Message",
                                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    uPlan.plan_id = planID;
                    uPlan.recording = false;

                    if(dalPlan.RecordingUpdate(uPlan))
                    {
                        loaded = false;
                        MessageBox.Show("Item removed!");

                        DataTable dt = (DataTable)dgv.DataSource;
                       
                        dt.Rows.RemoveAt(rowIndex);
                        dt.AcceptChanges();

                        dgvItemList.ClearSelection();

                        if (dt == null || dt.Rows.Count <= 0)
                        {
                            lblClearList.Hide();
                            lblRemoveCompleted.Hide();
                        }

                        loaded = true;
                        //LoadItemListData();
                    }
                }

            }

            else if(itemClicked.Equals(text_ChangePlanID))
            {
                ChangePlanID(planID, itemCode);
            }

            Cursor = Cursors.Arrow; // change cursor to normal type
            dgv.ResumeLayout();
        }

        private void ChangePlanID(int planID, string itemCode)
        {
            //open machine schedule page

            frmMachineSchedule frm = new frmMachineSchedule(text.DailyAction_ChangePlan, planID, itemCode)
            {
                StartPosition = FormStartPosition.CenterScreen
            };


            frm.ShowDialog();
            loaded = false;
            LoadItemListData();
            dgvItemList.ClearSelection();
            loaded = true;
        }

        private void dgvDailyRecord_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            Cursor = Cursors.WaitCursor; // change cursor to hourglass type

            DataGridView dgv = dgvRecordHistory;
            dgv.SuspendLayout();

            string itemClicked = e.ClickedItem.Name.ToString();
            int rowIndex = dgv.CurrentCell.RowIndex;
            int sheetID = Convert.ToInt32(dgv.Rows[rowIndex].Cells[header_SheetID].Value);
            string itemName = dgvItemList.Rows[dgvItemList.CurrentCell.RowIndex].Cells[header_PartName].Value.ToString();

            contextMenuStrip1.Hide();

            if (itemClicked.Equals("Remove from this list."))
            {
                if (MessageBox.Show("Are you sure you want to remove (SHEET ID: " + sheetID + ") " + itemName + " from this list?", "Message",
                                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int userID = MainDashboard.USER_ID;
                    DateTime updateTime = DateTime.Now;
                    uProRecord.sheet_id = sheetID;
                    uProRecord.active = false;
                    uProRecord.updated_date = updateTime;
                    uProRecord.updated_by = userID;

                    if (dalProRecord.RemoveSheetData(uProRecord))
                    {
                        MessageBox.Show("Item removed!");
                        UndoPreviousRecordIfExist();
                        tool.historyRecord(text.RemoveDailyJobSheet, "Sheet ID: " + sheetID + "(" + lblPartCode.Text + ")", updateTime, userID);
                        dt_ProductionRecord = dalProRecord.Select();
                        LoadDailyRecord();
                        AddNewSheetUI(false);
                    }
                }

                dailyRecordLoaded = true;

            }

            Cursor = Cursors.Arrow; // change cursor to normal type
            dgv.ResumeLayout();
        }

        private void txtPlanID_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void txtSheetID_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cmbPackingCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (sheetLoaded)
            //{
            //    timer1.Stop();
            //    timer1.Start();
                
            //}

            //if (cmbPackingName.Text != string_MultiPackaging)
            //{
            //    //dt_MultiPackaging = null;
            //    SavePackagingToDT();
            //}
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(sheetLoaded)
            {
                timer1.Stop();
                //UpdatePackagingData();
            }
            
        }

        private void dgvRecordHistory_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            //errorProvider3.Clear();
            //errorProvider4.Clear();

            //int rowIndex = dgvRecordHistory.CurrentCell.RowIndex;

            //if (rowIndex > 0)
            //{
            //    addingNewSheet = false;
            //    AddNewSheetUI(true);
            //}
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            cmbPackingName.SelectedIndex = -1;
            cmbPackingCode.SelectedIndex = -1;
        }

        private void dgvRecordHistory_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            Cursor = Cursors.WaitCursor; // change cursor to hourglass type

            DataGridView dgv = dgvRecordHistory;
            int userPermission = dalUser.getPermissionLevel(MainDashboard.USER_ID);
            //handle the row selection on right click

            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && userPermission >= MainDashboard.ACTION_LVL_THREE)
            {
                ContextMenuStrip my_menu = new ContextMenuStrip();
                dgv.CurrentCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                // Can leave these here - doesn't hurt
                dgv.Rows[e.RowIndex].Selected = true;
                dgv.Focus();
                int rowIndex = dgv.CurrentCell.RowIndex;

                try
                {
                    my_menu.Items.Add("Remove from this list.").Name = "Remove from this list.";

                    my_menu.Show(Cursor.Position.X, Cursor.Position.Y);
                    contextMenuStrip1 = my_menu;
                    my_menu.ItemClicked += new ToolStripItemClickedEventHandler(dgvDailyRecord_ItemClicked);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            Cursor = Cursors.Arrow; // change cursor to normal type
        }

        private void AddMorePackaging_Click(object sender, EventArgs e)
        {
            int sheetID = int.TryParse(txtSheetID.Text, out sheetID) ? sheetID : -1;

            frmProPackaging frm;

            if (dt_MultiPackaging != null && dt_MultiPackaging.Rows.Count > 0)
            {
                if (cmbPackingName.Text != string_MultiPackaging)
                {
                    dt_MultiPackaging = null;
                    SavePackagingToDT();
                }
                

                frm = new frmProPackaging(dt_MultiPackaging)
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
            }
            else
            {
                frm = new frmProPackaging(sheetID)
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
            }

            frm.ShowDialog();

            if(frmProPackaging.dataSaved)
            {
                SavePackagingToDT(frmProPackaging.dt_Packaging);
 
            }
           
        }

        private void SavePackagingToDT()//for only one packaging data
        {
            if(sheetLoaded)
            {
                bool DataPassed = cmbPackingName.Text != string_MultiPackaging;

                DataPassed &= cmbPackingCode.Text != string_MultiPackaging;

                DataPassed &= txtPackingMaxQty.Text != string_Multi;

                if (DataPassed && dt_Carton != null && dt_Carton.Rows.Count > 0)
                {
                    int maxQty = int.TryParse(txtPackingMaxQty.Text, out maxQty) ? maxQty : 0;
                    int totalBox = int.TryParse(txtFullBox.Text, out totalBox) ? totalBox : 0;

                    string packagingCode = cmbPackingCode.Text;
                    string packagingName = cmbPackingName.Text;

                    foreach (DataRow row in dt_Carton.Rows)
                    {
                        string itemcode = row[header_PackagingCode].ToString();

                        if(itemcode.Equals(packagingCode))
                        {
                            row[header_PackagingMax] = maxQty;
                            row[header_PackagingQty] = totalBox;
                        }
                    }
                }
            }
           
        }

        private void SavePackagingToDT(DataTable dt)
        {
            dt_MultiPackaging = null;
            dt_MultiPackaging = dt.Copy();

            int rowCount = dt_MultiPackaging.Rows.Count;

            if (rowCount >= 2)
            {
                LoadPackingToCMB(string_MultiPackaging);
                txtPackingMaxQty.Text = string_Multi;

                int totalStockIn = 0;
                int totalFullBox = 0;

                foreach (DataRow row in dt_MultiPackaging.Rows)
                {
                    string packingCode = row[frmProPackaging.header_PackagingCode].ToString();

                    int boxQty = Convert.ToInt32(row[frmProPackaging.header_PackagingQty].ToString());
                    int maxQty = Convert.ToInt32(row[frmProPackaging.header_PackagingMax].ToString());

                    string itemType = packingCode.Substring(0, 3);


                    if (itemType == "CTN" || itemType == "CTR")
                    {
                        totalFullBox += boxQty;

                        totalStockIn += boxQty * maxQty;
                    }

                 
                }


                txtFullBox.Text = totalFullBox.ToString();
                //txtTotalProduce.Text = totalStockIn.ToString();
                //CalculateTotalProduce();
            }
            else if (rowCount == 1)
            {
                loadPackingData();

                string fullboxQty = dt_MultiPackaging.Rows[0][frmProPackaging.header_PackagingQty].ToString();
                string pcsPerBox = dt_MultiPackaging.Rows[0][frmProPackaging.header_PackagingMax].ToString();
                string packingCode = dt_MultiPackaging.Rows[0][frmProPackaging.header_PackagingCode].ToString();

                cmbPackingName.Text = dt_MultiPackaging.Rows[0][frmProPackaging.header_PackagingName].ToString();
                tool.loadItemCodeDataToComboBox(cmbPackingCode, cmbPackingName.Text);
                cmbPackingCode.Text = packingCode;
                txtFullBox.Text = fullboxQty;
                txtPackingMaxQty.Text = pcsPerBox;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtAvailableQty_TextChanged(object sender, EventArgs e)
        {
            CalculateTotalRejectAndPercentage();
        }

        private void txtProLotNo_TextChanged(object sender, EventArgs e)
        {
            if (sheetLoaded)
            {
                dataSaved = false;
            }

            //lotNo = int.TryParse(txtProLotNo.Text, out lotNo) ? lotNo : 0;
        }

        private string SetAlphabetToProLotNo(int macID, int lotNo)
        {
            return tool.GetAlphabet(macID - 1) + " - " + lotNo;
        }

        private void txtProLotNo_Leave(object sender, EventArgs e)
        {

            lotNo = int.TryParse(txtProLotNo.Text, out lotNo) ? lotNo : 0;

            txtProLotNo.Text = SetAlphabetToProLotNo(macID, lotNo);
            //txtProLotNo.Text = tool.GetAlphabet(macID-1) + " - " + lotNo;
        }

        private void txtProLotNo_Enter(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtProLotNo.Text))
            txtProLotNo.Text = lotNo.ToString();
        }

        private void dgvRecordHistory_Click(object sender, EventArgs e)
        {
            if(dgvRecordHistory.CurrentCell != null)
            {
                int row = dgvRecordHistory.CurrentCell.RowIndex;

                if (row >= 0 && dgvRecordHistory.SelectedRows.Count > 0)
                {
                    if (dailyRecordLoaded)
                    {
                        errorProvider3.Clear();
                        errorProvider4.Clear();

                        int rowIndex = dgvRecordHistory.CurrentCell.RowIndex;

                        if (rowIndex >= 0)
                        {
                            addingNewSheet = false;
                            AddNewSheetUI(true);
                            NewLoadExistingSheetData();
                        }
                    }
                }
            }
          
        }

        private void dtpProDate_ValueChanged_1(object sender, EventArgs e)
        {
            if (sheetLoaded)
            {
                dataSaved = false;
            }
            if (CheckIfDateOrShiftDuplicate())
            {
                ClearAllError();
                errorProvider1.SetError(lblDate, "Please select a different date");
                errorProvider2.SetError(lblShift, "Please select a different shift");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        private void txtBalanceOfLastShift_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {
                SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        private void txtBalanceOfThisShift_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {
                SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        private void txtFullBox_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {
                SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        private void txtTotalStockIn_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {
                if (Validation())
                {
                    //SaveAndStock();
                    NewSaveAndStock();

                    AddNewSheetUI(false);
                    LoadDailyRecord();
                    dgvRecordHistory.ClearSelection();
                    dailyRecordLoaded = true;
                }
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {
            
            Cursor = Cursors.WaitCursor;
            RemoveCompleted();
            Cursor = Cursors.Arrow;
        }

        private void lblClearList_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            ClearList();
            Cursor = Cursors.Arrow;
        }

        private void label14_Click_1(object sender, EventArgs e)
        {
            cmbParentList.SelectedIndex = -1;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            LoadPage();
        }

        private void dgvRecordHistory_Sorted(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dgvRecordHistory.DataSource;

            int total = 0;
            //get total
            //foreach(DataRow row in dt.Rows)
            //{
            //    string total_str = row[]
            //}
        }

        private void txtUsed_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtdirectIn_TextChanged(object sender, EventArgs e)
        {
            errorProvider7.Clear();
            CalculateTotalProduce();
        }

        private void txtUsed_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void dgvItemList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = dgvItemList;

            int row = e.RowIndex;
            int col = e.ColumnIndex;

            if (dgv.Columns[col].Name == header_Status)
            {
                string status = dgv.Rows[row].Cells[header_Status].Value.ToString();

                if(status == text.planning_status_completed)
                {
                    dgv.Rows[row].Cells[header_PartName].Style.BackColor = Color.LightBlue;
                    dgv.Rows[row].Cells[header_PartCode].Style.BackColor = Color.LightBlue;
                }
                else
                {
                    dgv.Rows[row].Cells[header_PartName].Style.BackColor = Color.White;
                    dgv.Rows[row].Cells[header_PartCode].Style.BackColor = Color.White;
                }
            }
        }

        private void lblIn_DoubleClick(object sender, EventArgs e)
        {
            //direct save and In
            //DirectIn();
            //AddNewSheetUI(false);
            //LoadDailyRecord();
            //dgvRecordHistory.ClearSelection();
            //dailyRecordLoaded = true;

        }

        private void lblIn_Click(object sender, EventArgs e)
        {
            int balThisShift = int.TryParse(txtBalanceOfThisShift.Text, out balThisShift) ? balThisShift : 0;

            int directIn = int.TryParse(txtIn.Text, out directIn) ? directIn : 0;

            if (directIn <= 0)
            {
                txtBalanceOfThisShift.Text = "0";
                txtIn.Text = (balThisShift + directIn).ToString();
            }

           
        }

        private void lblOut_DoubleClick(object sender, EventArgs e)
        {
            //DirectOut();

            //AddNewSheetUI(false);
            //LoadDailyRecord();
            //dgvRecordHistory.ClearSelection();
            //dailyRecordLoaded = true;
        }

        private void dgvRecordHistory_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvRecordHistory.Columns[header_TotalStockIn].InheritedStyle.Font = new Font("Segoe UI", 6F, FontStyle.Italic);
        }

        private void updateDiff()
        {
            string totalProduced = txtTotalProducedRecord.Text;
            string totalStockIn = txtTotalStockInRecord.Text;

            int producedQty = int.TryParse(totalProduced, out producedQty) ? producedQty : 0;
            int stockInQty = int.TryParse(totalStockIn, out stockInQty) ? stockInQty : 0;
            txtDiff.Text = (producedQty - stockInQty).ToString();

            if (totalStockIn != totalProduced)
            {

                txtDiff.BackColor = Color.Pink;
            }
            else
            {
                if (!string.IsNullOrEmpty(totalProduced) && !string.IsNullOrEmpty(totalStockIn))
                {
                    txtDiff.BackColor = Color.LightGreen;
                }
                else
                {
                    txtDiff.BackColor = Color.White;
                }

            }
        }
        private void txtTotalStockInRecord_TextChanged(object sender, EventArgs e)
        {
            updateDiff();
        }

        private void UpdateCheckedStatus(bool Checked, int checkBy, DateTime date)
        {
            
            DataGridView dgv = dgvItemList;
            int rowIndex = dgv.CurrentCell.RowIndex;
            int planID = Convert.ToInt32(dgv.Rows[rowIndex].Cells[header_PlanID].Value);
            string planStatus = dgv.Rows[rowIndex].Cells[header_Status].Value.ToString() ;
            string itemCode = dgv.Rows[rowIndex].Cells[header_PartCode].Value.ToString();

            uPlan.plan_id = planID;
            uPlan.CheckedDate = date;
            uPlan.CheckedBy = checkBy;
            uPlan.Checked = Checked;

            
            if (dalPlan.CheckedUpdate(uPlan))
            {
                if (planStatus == text.planning_status_completed)
                {
                    uPlan.recording = !Checked;

                    if (!dalPlan.RecordingUpdate(uPlan))
                    {
                        MessageBox.Show("Failed to update recording status!");

                    }

                }
                if (Checked)
                {
                    tool.historyRecord(text.CheckedJobSheet, "Plan ID: " + planID + "(" + itemCode + ")", date, checkBy);

                   
                }
                
                else
                {
                    tool.historyRecord(text.UncheckedJobSheet, "Plan ID: " + planID + "(" + itemCode + ")", date, checkBy);
                }

                dt_ProductionRecord = dalProRecord.Select();
            }

        }
        private void cbChecked_CheckedChanged(object sender, EventArgs e)
        {
            if(!stopCheckedChange && loaded)
            {
                int userID = MainDashboard.USER_ID;
                int userPermission = dalUser.getPermissionLevel(userID);
                string userName = dalUser.getUsername(userID);
                DateTime dateNow = DateTime.Now;
                bool valid = userPermission >= MainDashboard.ACTION_LVL_FIVE || userID == 6;
                bool checkedStatus = cbChecked.Checked;


                if (checkedStatus)
                {
                    if (valid)
                    {
                        lblCheckBy.Text = string_CheckBy + userName;
                        lblCheckDate.Text = string_CheckDate + dateNow;
                        //update
                        UpdateCheckedStatus(checkedStatus, userID, dateNow);

                    }
                    else if (!stopCheckedChange)
                    {
                        stopCheckedChange = true;
                        cbChecked.Checked = false;
                        stopCheckedChange = false;
                    }
                }
                else
                {
                    if (valid)
                    {
                        lblCheckBy.Text = "";
                        lblCheckDate.Text = "";
                        //update
                        UpdateCheckedStatus(checkedStatus, userID, dateNow);
                    }
                    else
                    {
                        stopCheckedChange = true;
                        cbChecked.Checked = true;
                        stopCheckedChange = false;
                    }
                }
            }

            

        }

        private void button4_Click(object sender, EventArgs e)
        {
           
            ShowDailyRecordUI();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!dataSaved)
            {
                DialogResult dialogResult = MessageBox.Show(text.Message_DataNotSaved, "Message",
                                                           MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.OK)
                {
                    dataSaved = true;

                    dataSaved = true;
                    errorProvider3.Clear();
                    errorProvider4.Clear();
                    AddNewSheetUI(false);

                    //load record history
                    LoadDailyRecord();
                    ClearInfo();
                    dgvRecordHistory.ClearSelection();


                    dailyRecordLoaded = true;
                    ShowProductionListUI();
                }
            }
            else
            {
                dataSaved = true;
                errorProvider3.Clear();
                errorProvider4.Clear();
                AddNewSheetUI(false);

                //load record history
                LoadDailyRecord();
                ClearInfo();
                dgvRecordHistory.ClearSelection();


                dailyRecordLoaded = true;

                ShowProductionListUI();
            }
           
        }

        private void lblOut_Click(object sender, EventArgs e)
        {
            int balLastShift = int.TryParse(txtBalanceOfLastShift.Text, out balLastShift) ? balLastShift : 0;

            int directOut = int.TryParse(txtOut.Text, out directOut) ? directOut : 0;

            if(directOut <= 0)
            {
                txtBalanceOfLastShift.Text = "0";
                txtOut.Text = (balLastShift + directOut).ToString();
            }
            
        }

        private void dgvItemList_DoubleClick(object sender, EventArgs e)
        {
            ProductionListRowSelected();
            ShowDailyRecordUI();
        }

        private void txtOut_TextChanged(object sender, EventArgs e)
        {
            errorProvider6.Clear();
            //CalculateTotalRejectAndPercentage();
            CalculateTotalProduce();

            if(sheetLoaded)
            {
                balanceOutEdited = true;
            }
        }

        private void txtRawMatLotNo_TextChanged(object sender, EventArgs e)
        {
            if (sheetLoaded)
            {
                dataSaved = false;
            }
        }

        private void txtColorMatLotNo_TextChanged(object sender, EventArgs e)
        {
            if (sheetLoaded)
            {
                dataSaved = false;
            }
        }

        private void cmbParentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sheetLoaded)
            {
                dataSaved = false;
            }
        }

        private void txtTotalProducedRecord_TextChanged(object sender, EventArgs e)
        {
            updateDiff();
        }

        private void lblAddMoreParent_Click(object sender, EventArgs e)
        {
            int sheetID = int.TryParse(txtSheetID.Text, out sheetID) ? sheetID : -1;

            frmMultiParent frm = new frmMultiParent(lblPartCode.Text);

            frm.ShowDialog();
            //if (dt_MultiPackaging != null && dt_MultiPackaging.Rows.Count > 0)
            //{
            //    frm = new frmProPackaging(dt_MultiPackaging)
            //    {
            //        StartPosition = FormStartPosition.CenterScreen
            //    };
            //}
            //else
            //{
            //    frm = new frmProPackaging(sheetID)
            //    {
            //        StartPosition = FormStartPosition.CenterScreen
            //    };
            //}



            //if (frmProPackaging.dataSaved)
            //{
            //    SavePackagingToDT(frmProPackaging.dt_Packaging);

            //}
        }

        private void tableLayoutPanel9_Paint(object sender, PaintEventArgs e)
        {

        }


        private void button3_Click_1(object sender, EventArgs e)
        {
            frmItemGroupList frm = new frmItemGroupList(lblPartCode.Text, lblPartName.Text)
            {
                StartPosition = FormStartPosition.CenterScreen
            };

            frm.ShowDialog();

            dt_JoinInfo = dalJoin.SelectAll();

            if (dt_Carton == null || dt_Carton.Rows.Count <= 0)
            {
                LoadCartonSetting();

            }


        }

        private void label20_Click(object sender, EventArgs e)
        {
            int sheetID = int.TryParse(txtSheetID.Text, out sheetID) ? sheetID : -1;

            if(dt_Carton == null || dt_Carton.Rows.Count <=0)
            {
                dt_Carton = NewCartonTable();
            }

            frmCartonSetting frm = new frmCartonSetting(dt_Carton)
            {
                StartPosition = FormStartPosition.CenterScreen
            };

            frm.ShowDialog();

            dt_Carton = frmCartonSetting._DT_CARTON;

            string AlertMessage = "";

            if (dt_Carton.Rows.Count > 0)
            {
                int totalStockIn = 0;
                int totalFullBox = 0;

                string name = dt_Carton.Rows[0][header_PackagingName].ToString();
                string code = dt_Carton.Rows[0][header_PartCode].ToString();

                int rowCount = dt_Carton.Rows.Count;

                if (rowCount >= 2)
                {
                    LoadPackingToCMB(string_MultiPackaging);
                    txtPackingMaxQty.Text = "MULTI";

                    txtFullBox.Enabled = false;
                  

                    foreach (DataRow row in dt_Carton.Rows)
                    {
                        int boxQty = int.TryParse(row[header_PackagingQty].ToString(), out boxQty) ? boxQty : 0;
                        int maxQty = int.TryParse(row[header_PackagingMax].ToString(), out maxQty) ? maxQty : 0;

                        totalFullBox += boxQty;

                        totalStockIn += boxQty * maxQty;
                    }


                    txtFullBox.Text = totalFullBox.ToString();

                }
                else if (rowCount == 1)
                {
                    loadPackingData();
                    txtFullBox.Enabled = true;

                    cmbPackingName.Text = name;
                    cmbPackingCode.Text = code;

                    foreach (DataRow row in dt_Carton.Rows)
                    {
                        txtPackingMaxQty.Text = row[header_PackagingMax].ToString();
                        txtFullBox.Text = row[header_PackagingQty].ToString();
                    }


                }
            }
            else
            {
                txtPackingMaxQty.Text = "";
                cmbPackingName.SelectedIndex = -1;
                cmbPackingCode.SelectedIndex = -1;

                AlertMessage = "Carton data not found!\nPlease add a main carton to this item at:\n1. Item Group Edit";
            }

            if(!string.IsNullOrEmpty(AlertMessage))
            MessageBox.Show(AlertMessage, "Carton Alert",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void txtFullBox_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("click test");
        }
    }
}
