using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;
using CDTLib;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace TinhLuongGVCHCN
{
    public class TinhLuongGVCHCN : ICControl
    {
        #region ICControl Members
        private InfoCustomControl info = new InfoCustomControl(IDataType.Single);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();
        string nam, thang;

        public DataCustomFormControl Data
        {
            set { data = value; }
        }

        public InfoCustomControl Info
        {
            get { return info; }
        }

        #endregion

        public void AddEvent()
        {
            nam = Config.GetValue("NamLamViec").ToString();
            thang = Config.GetValue("KyKeToan").ToString();
            data.FrmMain.Shown += new EventHandler(FrmMain_Shown);
        }

        void FrmMain_Shown(object sender, EventArgs e)
        {
            GridControl gcMain = data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl;
            GridView gvMain = gcMain.MainView as GridView;

            if (gvMain.DataRowCount == 0)
            {
                XtraMessageBox.Show("Số liệu chưa có, phần mềm sẽ tự động tổng hợp lương giáo viên chi nhánh tháng " + thang
                    + " để bạn tiến hành cập nhật", "Xác nhận", MessageBoxButtons.OK);
            }
            else
            {
                if (XtraMessageBox.Show("Số liệu đã có, bạn có muốn phần mềm cập nhật lại lương GVCH chi nhánh tháng " + thang + " không?",
                    "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }

            DataTable dtDT = LaySoLieu();
            if (dtDT.Rows.Count == 0)
            {
                XtraMessageBox.Show("Số liệu lương giáo viên chi nhánh tháng " + thang
                    + " chưa có để tổng hợp", "Xác nhận", MessageBoxButtons.OK);
            }
            else
            {
                Cursor.Current = Cursors.WaitCursor;
                CapNhatSoLieu(gvMain, dtDT);
                Cursor.Current = Cursors.Default;
            }
        }

        private void ThemSoLieu(GridView gvMain, DataRow drDT)
        {
            gvMain.AddNewRow();
            gvMain.SetFocusedRowCellValue(gvMain.Columns["MaLuong"], drDT["MaLuong"]);
            gvMain.SetFocusedRowCellValue(gvMain.Columns["ThuNhapCB"], drDT["ThuNhapCB"]);
            gvMain.SetFocusedRowCellValue(gvMain.Columns["ThuongSiSo"], drDT["ThuongSiSo"]);
            gvMain.SetFocusedRowCellValue(gvMain.Columns["ThuongLenLop"], drDT["ThuongLenLop"]);
            gvMain.SetFocusedRowCellValue(gvMain.Columns["ThuongChuyenCan"], drDT["ThuongChuyenCan"]);
            gvMain.SetFocusedRowCellValue(gvMain.Columns["PCLopDB"], drDT["PCLopDB"]);
            gvMain.SetFocusedRowCellValue(gvMain.Columns["PCVuotTiet"], TinhPCVuotTietLK(drDT["MaLuong"].ToString()));
        }

        private void SuaSoLieu(DataRow dr, DataRow drDT)
        {
            dr["ThuNhapCB"] = drDT["ThuNhapCB"];
            dr["ThuongSiSo"] = drDT["ThuongSiSo"];
            dr["ThuongLenLop"] = drDT["ThuongLenLop"];
            dr["ThuongChuyenCan"] = drDT["ThuongChuyenCan"];
            dr["PCLopDB"] = drDT["PCLopDB"];
            dr["PCVuotTiet"] = TinhPCVuotTietLK(drDT["MaLuong"].ToString());
        }

        private void CapNhatSoLieu(GridView gvMain, DataTable dtDT)
        {
            DataTable dtData = data.BsMain.DataSource as DataTable;
            foreach (DataRow drDT in dtDT.Rows)
            {
                string maLuong = drDT["MaLuong"].ToString();
                DataRow[] drs = dtData.Select("MaLuong = '" + maLuong + "'");
                if (drs.Length == 0)
                {
                    ThemSoLieu(gvMain, drDT);
                }
                else
                {
                    DataRow dr = drs[0];
                    SuaSoLieu(dr, drDT);
                }
            }
        }

        private decimal TinhPCVuotTietLK(string maLuong)
        {
            var lstThangTT = new List<string> {"4", "7", "10", "1"} ;
            if (!lstThangTT.Contains(thang)) return 0;

            string sql1 = "SELECT TienVuotThieuLK FROM SoTietGVCH WHERE MaLuong = '{0}' AND Thang = {1} AND Nam = {2}";
            string thangTT, namTT;
            if (thang == "1")
            {
                thangTT = "12";
                namTT = (Convert.ToInt32(nam) - 1).ToString();
            }
            else
            {
                thangTT = (Convert.ToInt32(thang) - 1).ToString();
                namTT = nam;
            }
            var tienVT = db.GetValue(string.Format(sql1, maLuong, thangTT, namTT));

            string sql2 = "SELECT PCVuotTiet FROM LuongGVCHCN WHERE MaLuong = '{0}' AND Thang = {1} AND Nam = {2}";
            var tienDaTT = db.GetValue(string.Format(sql2, maLuong, Convert.ToInt32(thangTT) - 2, namTT));

            decimal dTienVT = (tienVT == null || tienVT.ToString() == "") ? 0 : Convert.ToDecimal(tienVT);
            decimal dTienDaTT = (tienDaTT == null || tienDaTT.ToString() == "") ? 0 : Convert.ToDecimal(tienDaTT);
            return dTienVT > dTienDaTT ? dTienVT - dTienDaTT : 0;
        }

        private DataTable LaySoLieu()
        {
            string sql = @"DECLARE @Nam int = {0}
                        DECLARE @Thang int = {1}
                
                        SELECT	nv.MaLuong, nv.Hoten, ThuNhapCB = isnull(nv.ThuNhapCB, 0), ThuongSiSo = isnull(lcn.ThuongSiSo, 0), ThuongLenLop = isnull(lcn.ThuongLenLop, 0),
		                        ThuongChuyenCan = case when st.TongTietNghi < isnull((select top 1 TietNghiTD from TCThuongCC), 0) and st.TietNghiDoGV = 0 then isnull((select top 1 TienThuongCC from TCThuongCC), 0) else 0 end,
		                        PCLopDB = isnull(ldb.PhuCap, 0) + isnull(lct.PhuCap, 0)
                        FROM		wDMMaLuong nv
                        LEFT JOIN	(SELECT nv.MaLuong, ThuongSiSo = sum(cn.ThuongSS), ThuongLenLop = sum(cn.ThuongLenLop)
			                        FROM LuongGVCN cn INNER JOIN DMNVien nv on cn.MaGV = nv.MaNV
			                        WHERE cn.Nam = @Nam and cn.Thang = @Thang
			                        GROUP BY nv.MaLuong) lcn
	                        ON nv.MaLuong = lcn.MaLuong
                        LEFT JOIN	(SELECT nv.MaLuong, PhuCap = sum(cn.SoTietTT * nl.PhuCapLopDB/24)
			                        FROM LuongGVCN cn INNER JOIN DMNVien nv on cn.MaGV = nv.MaNV
				                        INNER JOIN DMLophoc l ON cn.MaLop = l.MaLop
				                        INNER JOIN DMNhomLop nl ON l.MaNLop = nl.MaNLop and nl.PhuCapLopDB > 0
			                        WHERE cn.Nam = @Nam and cn.Thang = @Thang
			                        GROUP BY nv.MaLuong) ldb
	                        ON nv.MaLuong = ldb.MaLuong
                        LEFT JOIN	(SELECT nv.MaLuong, PhuCap = sum(lct.SoTienTT * lct.PhuCapLopDB/24)
			                        FROM LuongGVCT ct INNER JOIN DMNVien nv on ct.MaGV = nv.MaNV
				                        INNER JOIN DMHVCT lct ON ct.MaLop = lct.MaLop and lct.PhuCapLopDB > 0
			                        WHERE ct.Nam = @Nam and ct.Thang = @Thang
			                        GROUP BY nv.MaLuong) lct
	                        ON nv.MaLuong = lct.MaLuong
                        LEFT JOIN	SoTietGVCH st
	                        ON nv.MaLuong = st.MaLuong and st.Nam = @Nam and st.Thang = @Thang
                        WHERE	(lcn.ThuongSiSo > 0 or lcn.ThuongLenLop > 0 or ldb.PhuCap > 0 or lct.PhuCap > 0)";
            
            return db.GetDataTable(string.Format(sql, nam, thang));
        }
    }
}
