﻿using System;
using FactoryManagementSoftware.DAL;
using FactoryManagementSoftware.BLL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryManagementSoftware.Module
{
    class Text
    {
        Tool tool = new Tool();
        itemDAL dalItem = new itemDAL();
        planningActionBLL uPlanningAction = new planningActionBLL();


        #region Action String

        public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public string System { get; } = "System";

        //password
        public string PW_UnlockPmmaStock { get; } = "038989!";
        public string PW_UnlockSBBCustomerDiscount { get; } = "038989";
        public string PW_UnlockSBBSalesReport { get; } = "Safplas038989!";



        #region Admin
        public string Terminated { get; } = "Terminated";
        public string Activate { get; } = "Activate";

        public string TransferHistory { get; } = "Transfer History";


        public string LogIn { get; } = "LogIn";
        public string LogOut { get; } = "LogOut";

        public string AddItem { get; } = "AddItem";
        public string ItemEdit { get; } = "ItemEdit";
        public string ItemDelete { get; } = "ItemDelete";

        public string CheckedJobSheet { get; } = "CheckedJobSheet";
        public string UncheckedJobSheet { get; } = "UncheckedJobSheet";
        public string AddDailyJobSheet { get; } = "AddDailyJobSheet";
        public string EditDailyJobSheet { get; } = "EditDailyJobSheet";
        public string RemoveDailyJobSheet { get; } = "RemoveDailyJobSheet";

        public string AddItemJoin { get; } = "AddItemJoin";
        public string ItemJoinEdit { get; } = "ItemJoinEdit";
        public string ItemJoinDelete { get; } = "ItemJoinDelete";

        public string AddFactory { get; } = "AddFactory";
        public string FactoryEdit { get; } = "FactoryEdit";
        public string FactoryDelete { get; } = "FactoryDelete";

        public string AddCustomer { get; } = "AddCustomer";
        public string CustomerEdit { get; } = "CustomerEdit";
        public string CustomerDelete { get; } = "CustomerDelete";

        public string AddUser { get; } = "AddUser";
        public string UserEdit { get; } = "UserEdit";
        public string UserDelete { get; } = "UserDelete";
        #endregion

        #region Company Info

        public string Company_Name_CN { get; } = "安全塑膠有限公司";
        public string Company_Name_EN { get; } = "SAFETY PLASTICS SDN. BHD.";
        public string Company_RegistrationNo { get; } = "No. Syarikat: 198901011676 (188981-U)";

        public string Company_Head_AddressAndContact { get; } =
 @"No. 2, Jalan 10/152, Taman Perindustrian O.U.G, Batu 6,Jalan Puchong, 58200 Kuala Lumpur.
(Tel) 03 - 7785 5278, 03 - 7782 0399 (Fax) 03 - 7782 0399 (Email) safety_plastic@yahoo.com";

        public string Company_Semenyih_AddressAndContact { get; } =
 @"No.17, PT 2507, Jalan Hi-Tech 2, Kawasan Perindustrian Hi-Tech, Jalan Sungai Lalang, 43500 Semenyih, Selangor.
(Tel) 016 - 282 8195 (Email) safetyplastics.my@gmail.com";

        #endregion

        #region Font Type

        public string Font_Type_KaiTi { get; } = "KaiTi";
        public string Font_Type_TimesNewRoman { get; } = "Times New Roman";

        #endregion

        #region History Action
        public string Transfer { get; } = "Transfer";
        public string TransferUndo { get; } = "TransferUndo";
        public string TransferUndoBySystem { get; } = "TransferUndoBySystem";
        public string TransferRedo { get; } = "TransferRedo";
        public string Undo { get; } = "Undo";
        public string Passed { get; } = "Passed";

        public string AddOrder { get; } = "AddOrder";
        public string OrderRequest { get; } = "OrderRequest";
        public string OrderApprove { get; } = "OrderApprove";
        public string OrderCancel { get; } = "OrderCancel";
        public string OrderReceive { get; } = "OrderReceive";
        public string OrderFollowUp { get; } = "OrderFllowUp";
        public string OrderClose { get; } = "OrderClose";
        public string OrderActionEdit { get; } = "OrderActionEdit";
        public string OrderActionUndo { get; } = "OrderActionUndo";

        public string ForecastEdit { get; } = "ForecastEdit";
        public string ForecastInsert { get; } = "ForecastInsert";
        public string ForecastReport { get; } = "ForecastReport";
        public string StockReport { get; } = "StockReport";
        public string TransferReport { get; } = "TransferReport";
        public string OrderReport { get; } = "OrderReport";
        public string MaterialUsedReport { get; } = "MaterialUsedReport";

        public string Excel { get; } = "Excel";
        public string PMMAEdit { get; } = "PMMA Edit";

        public string DO_Added { get; } = "D/O ADDED";
        public string DO_Edited { get; } = "D/O EDITED";
        public string DO_Removed { get; } = "D/O REMOVED";
        public string DO_Delivered { get; } = "D/O DELIVERED";
        public string DO_Exported { get; } = "D/O EXPORTED";
        public string DO_Incomplete { get; } = "D/O INCOMPLETE";
        public string DO_UndoRemove { get; } = "D/O UNDO REMOVE";
        public string DO_ChangeDONumber { get; } = "D/O NUMBER CHANGE";
        public string DO_AddedFromPlanner { get; } = "D/O ADDED FROM PLANNER";

        public string PO_Added { get; } = "P/O ADDED";
        public string PO_Edited { get; } = "P/O EDITED";
        public string PO_Removed { get; } = "P/O REMOVED";

       // public string PO_Delivered { get; } = "D/O DELIVERED";
        //public string PO_Exported { get; } = "D/O EXPORTED";

        public string DataType_ToDelivery { get; } = "To Delivery";

        #endregion

        #region SBB ITEM TYPE

        public string SBB_TYPE_EQUAL { get; } = "EQUAL";
        public string SBB_TYPE_MTA { get; } = "MTA";
        public string SBB_TYPE_FTA { get; } = "FTA";
        public string SBB_TYPE_ENDCAP { get; } = "END CAP";
        public string SBB_TYPE_POLYORING { get; } = "POLY O RING";
        public string SBB_TYPE_REDUCING { get; } = "REDUCING";
        public string SBB_TYPE_MALE { get; } = "MALE";
        public string SBB_TYPE_FEMALE { get; } = "FEMALE";

        #endregion

        #region Report Type

        public string Report_Type_Production { get; } = "ProductionReport";

        #endregion

        #region Planning 
        public string planning_status_idle { get; } = "IDLE";//blue
        public string planning_status_pending { get; } = "PENDING";//blue
        public string planning_status_cancelled { get; } = "CANCELLED";//white
        public string planning_status_warning { get; } = "WARNING";//red
        public string planning_status_running { get; } = "RUNNING";//green
        public string planning_status_delayed { get; } = "DELAYED";//yellow
        public string planning_status_completed { get; } = "COMPLETED";//white
        public string planning_Family_mould_Remark { get; } = "[FAMILY MOULD]";

        //PLANNING ACTION HISTORY////////////////////////////////////////////////////////////////////////////
        public string plan_Added { get; } = "PLAN ADD";
        public string plan_Target_Qty_Change { get; } = "PLAN TARGET QTY CHANGE";

        public string plan_schedule_change { get; } = "PLAN SCHEDULE CHANGE";
        public string plan_status_change { get; } = "PLAN STATUS CHANGE";
        public string plan_family_with_change { get; } = "PLAN FAMILY CHANGE";
        public string plan_proday_change { get; } = "PLAN PRODUCTION DAY CHANGE";
        public string plan_prohour_change { get; } = "PLAN PRODUCTION HOUR CHANGE";
        public string plan_prohourperday_change { get; } = "PLAN PRODUCTION HOUR PER DAY CHANGE";
        #endregion

        #region Habit
        public string habit_insert { get; } = "HABIT INSERT";
        public string habit_belongTo_PlanningPage { get; } = "PLANNING PAGE";
        public string habit_planning_HourPerDay { get; } = "PLANNING: HOUR PER DAY";
        public string habit_planning_Wastage { get; } = "PLANNING: MATERIAL WASTAGE %";
        #endregion

        #region Location

        public string Factory { get; } = "Factory";

        public string Factory_2 { get; } = "No.2";
        public string Factory_9 { get; } = "No.9";
        public string Factory_11 { get; } = "No.11";
        public string Factory_30 { get; } = "No.30";
        public string Factory_40 { get; } = "No.40";
        
        public string Factory_Store { get; } = "STORE";
        public string Factory_Semenyih { get; } = "Semenyih";
        public string Factory_Bina { get; } = "BINA";


        public string Other { get; } = "Other";
        public string Customer { get; } = "Customer";
        public string Production { get; } = "Production";
        public string Assembly { get; } = "Assembly";
        public string Inspection { get; } = "Inspection";
        public string Inspection_Pass { get; } = "(OK";

        #endregion

        #region Database String
         //<add name = "connstrng" connectionString="SERVER=DESKTOP-MFUKGH2;DATABASE=Factory;USER ID=stock;PASSWORD=stock"/>

        public string DB_Semenyih { get; } = "SERVER=DESKTOP-MFUKGH2;DATABASE=Factory;USER ID=stock;PASSWORD=stock";
        public string DB_OUG { get; } = "SERVER=192.168.0.149;DATABASE=Factory;USER ID=stock;PASSWORD=stock";
        public string DB_JunPC { get; } = @"Data Source=.\SQLEXPRESS01;Initial Catalog=Factory;Integrated Security=True";



        #endregion

        #region Shift
        public string Shift_Morning { get; } = "MORNING";
        public string Shift_Night { get; } = "NIGHT";
        #endregion

        #region Unit
        public string Unit_KG { get; } = "kg";
        public string Unit_g { get; } = "g";
        public string Unit_Set { get; } = "set";
        public string Unit_Piece { get; } = "piece";
        //public string Unit_PCS { get; } = "pcs";
        public string Unit_Meter { get; } = "meter";
        public string Unit_Bag{ get; } = "bag";
        public string Unit_Millimetre { get; } = "mm";
        public string Unit_Inch { get; } = "in";
        public string Unit_Roll { get; } = "roll";
        #endregion

        #region Item Category

        public string Cat_RawMat { get; } = "RAW Material";
        public string Cat_MB { get; } = "Master Batch";
        public string Cat_Pigment { get; } = "Pigment";
        public string Cat_Part { get; } = "Part";
        public string Cat_Carton { get; } = "Carton";
        public string Cat_PolyBag { get; } = "Poly Bag";
        public string Cat_SubMat { get; } = "Sub Material";
        public string Cat_Mould { get; } = "Mould";
        public string Cat_Packaging { get; } = "Packaging";

        public string Cat_Terminated { get; } = "(TERMINATED)";


        #endregion

        #region  DGV TABLE HEADER NAME
        public string Header_Index { get; } = "#";
        public string Header_PartCodeWithParent { get; } = "PART CODE_(PARENT)";
        public string Header_PartCode { get; } = "PART CODE";
        public string Header_JoinQty { get; } = "JOIN QTY";
        public string Header_JoinMax { get; } = "JOIN MAX";
        public string Header_JoinMin { get; } = "JOIN MIN";
        public string Header_PartNameWithParent { get; } = "PART NAME_(PARENT)";
        public string Header_PartName { get; } = "PART NAME";
        public string Header_Parent { get; } = "PARENT";
        public string Header_OpeningStock { get; } = "OPENING STOCK";
        public string Header_In_KG_Piece { get; } = "IN(KG/PIECE)";
        public string Header_Used_KG_Piece { get; } = "USED(KG/PIECE)";
        public string Header_DirectOut_KG_Piece { get; } = "DIRECT OUT(KG/PIECE)";
        public string Header_BalStock { get; } = "BAL. STOCK";
        public string Header_Percentage { get; } = "%";
        public string Header_Wastage { get; } = "WASTAGE";
        public string Header_Adjust { get; } = "ADJUST";
        public string Header_Remark { get; } = "REMARK";
        public string Header_Note { get; } = "NOTE";
        public string Header_Type { get; } = "TYPE";
        public string Header_MatType { get; } = "MAT. TYPE";
        public string Header_Mat { get; } = "MATERIAL";
        public string Header_MatCode { get; } = "MAT. CODE";
        public string Header_MatName { get; } = "MAT. NAME";
        public string Header_Color { get; } = "COLOR";
        public string Header_ItemWeight_KG { get; } = "ITEM WEIGHT(KG)";
        public string Header_ItemWeight_G { get; } = "ITEM WEIGHT(g)";
        public string Header_ParentStillNeed { get; } = "PARENT STILL NEED";
        public string Header_StillNeed { get; } = "STILL NEED";
        public string Header_Delivered { get; } = "DELIVERED";
        public string Header_MaterialUsed_G { get; } = "MAT. USED(g)";
        public string Header_MaterialUsed_KG_Piece { get; } = "MAT. USED(KG/PIECE)";
        public string Header_MaterialUsed_KG { get; } = "MAT. USED(KG)";
        public string Header_MaterialUsedWithWastage { get; } = "MAT. USED AFTER WASTAGE";
        public string Header_TotalMaterialUsed_KG_Piece { get; } = "TOTAL MAT. USED(KG/PIECE)";
        public string Header_TotalMaterialUsed_KG { get; } = "TOTAL MAT. USED(KG)";
        #endregion

        #region SPP/SBB Info; Item Type

        public string SPP_BrandName { get; } = "SPP";
        public string SBB_BrandName { get; } = "SBB";

        public string EqualElbow_Short { get; } = "EE";
        public string EqualSocket_Short { get; } = "ES";
        public string EqualTee_Short { get; } = "ET";

        public string ReducingSocket_Short { get; } = "RS";
        public string ReducingElbow_Short { get; } = "RE";
        public string ReducingTee_Short { get; } = "RT";

        public string MTA_Short { get; } = "MTA";
        public string FTA_Short { get; } = "FTA";

        public string MaleElbow_Short { get; } = "ME";
        public string MaleTee_Short { get; } = "MT";

        public string FemaleElbow_Short { get; } = "FE";
        public string FemaleTee_Short { get; } = "FT";

        public string EndCap_Short { get; } = "ENDC";

        public string PolyORing_Short { get; } = "OR";

        #endregion

        #region SPP/SBB CATEGORY
        public string Cat_CommonPart { get; } = "COMMON PART";
        public string Cat_Body { get; } = "BODY";
        public string Cat_Assembled { get; } = "ASSEMBLED";
        public string Cat_ReadyGoods { get; } = " READY GOODS";
        #endregion

        #region SPP/SBB TYPE
        public string Type_EqualSocket { get; } = "EQUAL SOCKET";
        public string Type_EqualElbow { get; } = "EQUAL ELBOW";
        public string Type_EqualTee { get; } = "EQUAL TEE";
        public string Type_Bush { get; } = "BUSH";
        public string Type_Grip { get; } = "GRIP";
        public string Type_Oring { get; } = "O RING";
        public string Type_Cap { get; } = "CAP";

        public string Type_ReducingSocket { get; } = "REDUCING SOCKET";
        public string Type_ReducingElbow { get; } = "REDUCING ELBOW";
        public string Type_ReducingTee { get; } = "REDUCING TEE";

        public string Type_MTA { get; } = "MTA";
        public string Type_FTA { get; } = "FTA";

        public string Type_MaleElbow { get; } = "MALE ELBOW";
        public string Type_MaleTee { get; } = "MALE TEE";

        public string Type_FemaleElbow { get; } = "FEMALE ELBOW";
        public string Type_FemaleTee { get; } = "FEMALE TEE";

        public string Type_EndCap { get; } = "END CAP";

        public string Type_PolyNipple { get; } = "POLY NIPPLE";

        public string Type_PolyReducingBush { get; } = "POLY R.Bush";

        public string Type_PolyORing { get; } = "POLY O RING";



        #endregion

        #region Message
        public string Message_DataNotSaved { get; } = "You changes have not been saved.\nDiscard changes?";
        public string Note_OldBalanceStockOut { get; } = "Old Balance Stock Out";
        public string Note_BalanceStockIn { get; } = "Balance Stock In";
        #endregion

        #region SPP/SBB STOCK LEVEL
        public int StockLevel_20 { get; } = 50;
        public int StockLevel_25 { get; } = 60;
        public int StockLevel_32 { get; } = 40;
        public int StockLevel_50 { get; } = 30;
        public int StockLevel_63 { get; } = 0;
        #endregion

        #region Delivery Status

        public string Delivery_Processing { get; } = "PROCESSING";
        public string Delivery_DOPending { get; } = "D/O PENDING";
        public string Delivery_DOOpened { get; } = "D/O OPENED";
        public string Delivery_ToDeliver { get; } = "TO DELIVER";
        public string Delivery_Delayed { get; } = "DELAYED";
        public string Delivery_Cancelled { get; } = "CANCELLED";
        public string Delivery_Delivered { get; } = "DELIVERED";
        public string Delivery_Removed { get; } = "REMOVED";

        #endregion

        public string DO_CustOwnDO { get; } = "*USE CUSTOMER OWN D/O";


        public string DailyAction_ChangePlan { get; } = "CHANGE PLAN TO";

        #endregion

        #region CSV File Name

        //string path = @"G:\Other computers\Semenyih666\Server\(SOFTWARE SYSTEM)\(CSV)\";

        public string CSV_SemenyihServer_Path { get; } = @"\\DESKTOP-MFUKGH2\Server\(SOFTWARE SYSTEM)\(CSV)\";

        public string CSV_Jun_GoogleDrive_SemenyihServer_Path { get; } = @"G:\Other computers\Semenyih666\Server\(SOFTWARE SYSTEM)\(CSV)\";

        public string CSV_dalItemCust_SPPCustSearchWithTypeAndSize { get; } = "dalItemCust_SPPCustSearchWithTypeAndSize.db";
        public string CSV_dalItem_Select { get; } = "dalItem_Select.db";
        public string CSV_dalSBB_DOWithTrfInfoSelect_StartEnd { get; } = "dalSBB_DOWithTrfInfoSelect_StartEnd.db";
        //public string CSV_dalSBB_DOWithTrfInfoSelect_StartEnd { get; } = "dalSBB_DOWithTrfInfoSelect_StartEnd.csv";



        #endregion


        #region History Detail String

        public string Success { get; } = "Success";
        public string Failed { get; } = "Access Denied";

        public string GetPONumberAndCustomer(string PONo, string customer)
        {
            return "PO NO. :" + PONo + "( " + customer + ")";
        }

        public string GetDONumberAndCustomer(int DONo, string customer)
        {
            return "DO NO. :"+DONo.ToString("D6") + "( "+customer+")";
        }

        public string ChangeDONumber(int DONo, string customer, string oldData, string newData)
        {
            return "DO NO. :" + DONo.ToString("D6") + "( " + customer + ") " + " change D/O number from " + oldData + " to " + newData;
        }

        public string GetDOEditDetail(int DONo, string customer, string size, string type, string dataType, string oldData, string newData)
        {
            return "DO NO. :" + DONo.ToString("D6") + "( " + customer + ") " + size+" "+type +" "+dataType+" amend from "+oldData+" to "+newData;
        }

        public string GetDOExportDetail(bool openFile, bool printFile, bool printPreview)
        {

            string detail = "";

            if(openFile)
            {
                detail = "Open File; ";
            }

            if (printFile)
            {
                if(printPreview)
                {
                    detail += "Print File(with print preview);";
                }
                else
                {
                    detail += "Print File;";
                }
                
            }

            return detail;
        }


        public string getTransferDetailString(int ID, float qty, string unit, string itemCode, string from, string to)
        {
            string detail = "(ID:"+ID.ToString()+") "+qty.ToString()+" "+unit+" ("+itemCode+") "+tool.getItemName(itemCode)+" from "+from+" to "+to;

            return detail;
        }

        public string getNewOrderString(int orderID, float qty, string unit, string itemCode, string date)
        {
            string detail = "(ID:" + orderID.ToString() + ") " + qty.ToString() + " " + unit + " (" + itemCode + ") " + tool.getItemName(itemCode) + " at " + date;

            return detail;
        }

        public string getNewPlanningDetail(PlanningBLL u)
        {
            string detail = u.part_name +"( "+u.part_code+") :"+u.production_target_qty+" Target QTY run at machine "+u.machine_id;

            return detail;
        }

        public string getPlanningTargetQtyChangeDetail(PlanningBLL u)
        {
            //string detail = u.part_name + "( " + u.part_code + ") :" + u.production_Old_target_qty + " Target QTY run at machine " + u.machine_id;

            string detail = "[Plan's Target Qty Changed : "+ u.plan_id + "]" + u.production_Old_target_qty + " -> " + u.production_target_qty;

            return detail;
        }

        //public string getPlanningScheduleChangeDetail(PlanningBLL u)
        //{
        //    string detail = u.part_name + "( " + u.part_code + ") :" + u.production_target_qty + " Target QTY run at machine " + u.machine_id;

        //    return detail;
        //}

        public string getOrderString(int ID, float qty, string unit, string itemCode, string date)
        {
            string detail = "(ID:" + ID.ToString() + ") " + qty.ToString() + " " + unit + " (" + itemCode + ") " + tool.getItemName(itemCode) + " estimate received date at " + date;

            return detail;
        }

        public string getOrderStatusChangeString(int orderID)
        {
            string detail = "ID:" + orderID.ToString();

            return detail;
        }

        public string getForecastMonthEditString(string outMonth, string newMonth)
        {
            string detail = "Forecast 1 Month change from "+outMonth+" to "+newMonth;

            return detail;
        }

        public string getForecastEditString(string customer, string forecastNum, string itemCode, string from, string to)
        {
            if(int.TryParse(customer, out int i))
            {
                customer = tool.getCustName(Convert.ToInt32(customer));
            }

            string detail = "[" + customer + "_" + forecastNum + "] " + tool.getItemName(itemCode) + " (" + itemCode + "):" + from + " -> " + to;

            return detail;
        }

        public string getForecastInsertString(itemForecastBLL u)
        {
            string customer = "null";
            if (int.TryParse(u.cust_id.ToString(), out int i))
            {
                customer = tool.getCustName(Convert.ToInt32(u.cust_id));
            }

            string detail = "[" + customer + "_" + u.forecast_month + u.forecast_year + "] " + tool.getItemName(u.item_code) + " (" + u.item_code + "):" + u.forecast_qty;

            return detail;

        }
        public string getForecastEditString(itemForecastBLL u)
        {
            string customer = "null";
            if (int.TryParse(u.cust_id.ToString(), out int i))
            {
                customer = tool.getCustName(Convert.ToInt32(u.cust_id));
            }

            string detail = "[" + customer + "_" + u.forecast_month +u.forecast_year+ "] " + tool.getItemName(u.item_code) + " (" + u.item_code + "):" + u.forecast_old_qty + " -> " + u.forecast_qty;

            return detail;
        }

        public string getExcelString(string fileName)
        {
            string detail = fileName;

            return detail;
        }

        public string getPMMAEditString(string itemCode, string date, string subject, string oldValue, string newValue)
        {
            string detail = "("+itemCode+"_"+date+") Change "+subject+" from "+oldValue+" to "+newValue;

            return detail;
        }

        #endregion
    }
}
