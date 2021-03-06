using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTDatabase;
using CDTLib;
using Plugins;
using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using System.Windows.Forms;

namespace PBDoanhThu
{
    public class PBDoanhThu : ICControl
    {
        /// <summary>
        /// Hỗ trợ phân bổ doanh thu trong MT51
        /// </summary>

        private DataCustomFormControl data;
        private InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        Database db = Database.NewDataDatabase();

        #region ICControl Members

        public void AddEvent()
        {
            data.FrmMain.Shown += new EventHandler(FrmMain_Shown);
            TextEdit txtMaNV = data.FrmMain.Controls.Find("MaNV", true)[0] as TextEdit;
        }

        void FrmMain_Shown(object sender, EventArgs e)
        {
            DataRowView drv = data.BsMain.Current as DataRowView;
            if (drv.Row.RowState == DataRowState.Added)
            {
                int thang = 0;
                int loaiPB = 0;
                frmShow frm = new frmShow();
                frm.Text = "Phân bổ doanh thu";
                frm.ShowDialog();
                thang = frm.thang;
                loaiPB = frm.loaiPB;
                string nam = Config.GetValue("NamLamViec") != null ? Config.GetValue("NamLamViec").ToString() : DateTime.Now.Year.ToString();
                if (thang != 0 )
                {
                    string sql = "select * from MT51 where RefValue ='" + thang.ToString() + "/" + nam + "/" + Config.GetValue("MaCN").ToString() + "'";
                    if (db.GetDataTable(sql).Rows.Count == 0)
                    {
                        GridView gvChiTiet = (data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
                        if (loaiPB == 0)
                        {                            
                            db.UpdateByNonQuery("delete from TempDTLuongGV");

                            var lastday = DateTime.DaysInMonth(Convert.ToInt32(nam), thang);
                            var ngaykt = new DateTime(Convert.ToInt32(nam), thang, lastday);
                            var ngaybd = new DateTime(Convert.ToInt32(nam), thang, 1);

                            db.UpdateDatabyStore("sp_Month_DTVaLuongGV",
                                new string[] { "NgayBD", "NgayKT", "MaCN" }, new object[] { ngaybd, ngaykt, Config.GetValue("MaCN") });
                            DataTable dt = db.GetDataTable("select * from TempDTLuongGV");
                            if (dt.Rows.Count > 0 && ThemMaPhiMoi(dt, false))
                            {
                                Cursor.Current = Cursors.WaitCursor;
                                drv["RefValue"] = thang.ToString() + "/" + nam + "/" + Config.GetValue("MaCN").ToString();
                                drv["NgayCT"] = DateTime.Parse(thang.ToString() + "/01/" + nam).AddMonths(1);
                                drv["DienGiai"] = "Phân bổ doanh thu tháng " + thang.ToString();
                                addRows(dt, gvChiTiet, "3387", "5111", loaiPB);
                                setViews(gvChiTiet);
                                Cursor.Current = Cursors.Default;
                            }
                        }
                        else //if (loaiPB == 1)
                        {
                            sql = string.Format("execute sp_Month_PBDTLopCT {0},{1},{2}", nam, thang, Config.GetValue("MaCN"));
                            DataTable dt = db.GetDataTable(sql);
                            if (dt.Rows.Count > 0 && ThemMaPhiMoi(dt, true))
                            {
                                Cursor.Current = Cursors.WaitCursor;
                                drv["RefValue"] = thang.ToString() + "/" + nam + "/" + Config.GetValue("MaCN").ToString();
                                drv["NgayCT"] = DateTime.Parse(thang.ToString() + "/01/" + nam).AddMonths(1);
                                drv["DienGiai"] = "Phân bổ doanh thu tháng " + thang.ToString();
                                addRows(dt, gvChiTiet, "3381", "5114", loaiPB);
                                setViews(gvChiTiet);
                                Cursor.Current = Cursors.Default;
                            }                       
                        }
                    }
                    else
                        XtraMessageBox.Show("Đã phân bổ doanh thu tháng  " + thang.ToString() + "/" + nam, Config.GetValue("PackageName").ToString());
                }
            }
        }

        void addRows(DataTable dt, GridView gvChiTiet, string tkno, string tkco, int loaiPB)
        {
            foreach (DataRow row in dt.Rows)
            {                
                gvChiTiet.AddNewRow();
                gvChiTiet.UpdateCurrentRow();
                gvChiTiet.SetFocusedRowCellValue(gvChiTiet.Columns["DienGiaiCT"], "Phân bổ doanh thu lớp " + row["MaLop"].ToString());
                gvChiTiet.SetFocusedRowCellValue(gvChiTiet.Columns["TkNo"], tkno);
                gvChiTiet.SetFocusedRowCellValue(gvChiTiet.Columns["TkCo"], tkco);
                gvChiTiet.SetFocusedRowCellValue(gvChiTiet.Columns["Ps"], row["DoanhThu"]);
                if (loaiPB == 0)
                    gvChiTiet.SetFocusedRowCellValue(gvChiTiet.Columns["MaBP"], Config.GetValue("MaCN"));
                else
                    gvChiTiet.SetFocusedRowCellValue(gvChiTiet.Columns["MaBP"], row["MaCN"]);
                gvChiTiet.SetFocusedRowCellValue(gvChiTiet.Columns["MaPhi"], row["MaLop"].ToString());
            }
        }

        void setViews(GridView gvChiTiet)
        {
            gvChiTiet.Columns["DienGiaiCT"].OptionsColumn.AllowEdit = false;
            gvChiTiet.Columns["TkNo"].OptionsColumn.AllowEdit = false;
            gvChiTiet.Columns["TkCo"].OptionsColumn.AllowEdit = false;
            gvChiTiet.Columns["Ps"].OptionsColumn.AllowEdit = false;
            gvChiTiet.Columns["MaBP"].OptionsColumn.AllowEdit = false;
            gvChiTiet.Columns["MaPhi"].OptionsColumn.AllowEdit = false;
        }

        private bool ThemMaPhiMoi(DataTable dt, bool lopct)
        {   //luu y la ma nhom lop can phai ton tai san trong danh muc phi (co the dung cau hinh dong du lieu 2 bang DMPhi va DMNhomLop)
            string phiMoi = "";
            foreach (DataRow dr in dt.Rows)
            {
                object o = db.GetValue("select MaPhi from DMPhi where MaPhi = '" + dr["MaLop"].ToString() + "'");
                if (o == null || o.ToString() == "")
                    phiMoi += "'" + dr["MaLop"].ToString() + "',";
            }
            if (phiMoi == "")
                return true; 
            phiMoi = phiMoi.Remove(phiMoi.Length - 1);
            string sql = "insert into DMPhi(MaPhi, MaPhiMe, TenPhi) " +
                "select MaLop, MaNLop, TenLop " +
                "from " + (lopct ? "DMHVCT" : "DMLophoc") + " where MaLop in (" + phiMoi + ")";
            return (db.UpdateByNonQuery(sql));
        }

        public DataCustomFormControl Data
        {
            set { data = value; }
        }

        public InfoCustomControl Info
        {
            get { return info; }
        }

        #endregion
    }
}
