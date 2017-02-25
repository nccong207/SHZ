using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DevExpress.XtraGrid.Views.Grid;
using Plugins;
using CDTDatabase;
using CDTLib;
using DevExpress.XtraGrid;

namespace LichChieuSinh
{
    // Lấy tất cả lớp học theo chi nhánh và có Ngày bắt đầu học thuộc tháng truyền vào
    // Số HV ĐK cũ	    Lấy sỉ số hiện tại hoặc sỉ số tháng cuối cùng
    // Số HV đi học	    Lấy theo Sỉ số đi học trong danh mục lớp
    // Số HV(lớp mới)   Lấy theo sỉ số hiện tại
    public class LichChieuSinh:ICControl
    {
        private InfoCustomControl info = new InfoCustomControl(IDataType.Single);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();

        GridView gv;
        DataTable dtGiaoVien;

        public DataCustomFormControl Data
        {
            set { data = value; }
        }

        public InfoCustomControl Info
        {
            get { return info; }
        }

        public void AddEvent()
        {
            data.FrmMain.Shown += new EventHandler(FrmMain_Shown);
            gv = (data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
        }
        
        void FrmMain_Shown(object sender, EventArgs e)
        {
            string MaCN = "";
            int iMonth = DateTime.Today.Month;
            int iYear = DateTime.Today.Year;

            frmShow frm = new frmShow();
            frm.Text = "Lịch chiêu sinh tháng";
            frm.ShowDialog();

            gv.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
            gv.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(gv_CellValueChanged);

            if (frm.MaCN == "")
                return;

            MaCN = frm.MaCN;
            iMonth = frm.iThang;
            iYear = frm.iNam;
            dtGiaoVien = frm.dtGVPT;

            gv.ActiveFilterString = string.Format(" MaCN = '{0}' AND Thang = {1} AND Nam = {2} ", MaCN, iMonth, iYear);

            bool isUpdate = (gv.DataRowCount > 0);
            if (frm.dtLopHoc == null)
                return;
            foreach (DataRow row in frm.dtLopHoc.Rows)
            {
                if (isUpdate)
                {
                    if ((data.BsMain.DataSource as DataTable).Select(
                        string.Format(" MaCN = '{0}' AND Thang = {1} AND Nam = {2} and MaLop = '{3}' ", MaCN, iMonth, iYear, row["MaLop"])).Length > 0)
                        continue;
                }
                gv.AddNewRow();
                gv.SetFocusedRowCellValue(gv.Columns["MaCN"], MaCN);
                gv.SetFocusedRowCellValue(gv.Columns["Thang"], iMonth);
                gv.SetFocusedRowCellValue(gv.Columns["Nam"], iYear);
                gv.SetFocusedRowCellValue(gv.Columns["MaLop"], row["MaLop"]);
                gv.SetFocusedRowCellValue(gv.Columns["PhongHoc"], row["PhongHoc"]);
                gv.SetFocusedRowCellValue(gv.Columns["NgayKG"], row["NgayKG"]);
                gv.SetFocusedRowCellValue(gv.Columns["NgayKT"], row["NgayKT"]);
                gv.SetFocusedRowCellValue(gv.Columns["SoHV"], row["SoHV"]);
                gv.SetFocusedRowCellValue(gv.Columns["MaGioHoc"], row["MaGioHoc"]);
                gv.SetFocusedRowCellValue(gv.Columns["GVLopMoi"], row["GVLopMoi"]);
                gv.UpdateCurrentRow();
            }

            gv.BestFitColumns();
        }

        void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Value == null)
                return;

            if (e.Column.ColumnEditName.ToUpper() == "MALOPCU")
            {             
                DataView dv = new DataView(dtGiaoVien);
                dv.RowFilter = string.Format(" MaLop='{0}' ", e.Value.ToString());
                if (dv.Count > 0)
                {
                    gv.SetFocusedRowCellValue(gv.Columns["GVLopCu1"], dv[0].Row["GV1"]);
                    gv.SetFocusedRowCellValue(gv.Columns["GVLopCu2"], dv[0].Row["GV2"]);
                    gv.SetFocusedRowCellValue(gv.Columns["GVLopCu3"], dv[0].Row["GV3"]);
                    gv.SetFocusedRowCellValue(gv.Columns["SoHVDH"], dv[0].Row["SoHVDH"]);
                    gv.SetFocusedRowCellValue(gv.Columns["SoHVCu"], dv[0].Row["SoHVDK"]);
                }
                else
                {
                    //xóa thông tin liên quan của lớp cũ khi delete cell
                    gv.SetFocusedRowCellValue(gv.Columns["GVLopCu1"], DBNull.Value);
                    gv.SetFocusedRowCellValue(gv.Columns["GVLopCu2"], DBNull.Value);
                    gv.SetFocusedRowCellValue(gv.Columns["GVLopCu3"], DBNull.Value);
                    gv.SetFocusedRowCellValue(gv.Columns["SoHVDH"], 0);
                    gv.SetFocusedRowCellValue(gv.Columns["SoHVCu"], 0);
                }
            }
        }
    }
}
