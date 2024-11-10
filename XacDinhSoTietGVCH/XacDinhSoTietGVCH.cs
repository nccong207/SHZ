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

namespace XacDinhSoTietGVCH
{
    public class XacDinhSoTietGVCH : ICControl
    {
        #region ICControl Members
        private InfoCustomControl info = new InfoCustomControl(IDataType.Single);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();
        string maCN, nam, thang;

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
            maCN = Config.GetValue("MaCN").ToString();
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
                XtraMessageBox.Show("Số liệu chưa có, phần mềm sẽ tự động tổng hợp số tiết GVCH tháng " + thang + " của chi nhánh " + maCN
                    + " để bạn tiến hành cập nhật", "Xác nhận", MessageBoxButtons.OK);
            }
            else
            {
                if (XtraMessageBox.Show("Số liệu đã có, bạn có muốn phần mềm cập nhật lại số tiết GVCH tháng " + thang + " của chi nhánh " + maCN + " không?",
                    "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }

            DataTable dtDT = LaySoLieu();
            if (dtDT.Rows.Count == 0)
            {
                XtraMessageBox.Show("Số liệu số tiết GVCH tháng " + thang + " của chi nhánh " + maCN
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
            gvMain.SetFocusedRowCellValue(gvMain.Columns["TietChuan"], drDT["TietChuan"]);
            gvMain.SetFocusedRowCellValue(gvMain.Columns["TietDayCN"], drDT["TietDayCN"]);
            gvMain.SetFocusedRowCellValue(gvMain.Columns["TietDayCT"], drDT["TietDayCT"]);
            gvMain.SetFocusedRowCellValue(gvMain.Columns["TietDayThay"], drDT["TietDayThay"]);
            gvMain.SetFocusedRowCellValue(gvMain.Columns["TietBuKem"], drDT["TietBuKem"]);
            gvMain.SetFocusedRowCellValue(gvMain.Columns["TongTietNghi"], drDT["TongTietNghi"]);

            var maLuong = drDT["MaLuong"].ToString();
            var tietLK = TinhTietLK(maLuong) + Convert.ToDecimal(gvMain.GetFocusedRowCellValue("TietVuotThieu"));
            gvMain.SetFocusedRowCellValue(gvMain.Columns["TietVuotThieuLK"], tietLK);
            gvMain.SetFocusedRowCellValue(gvMain.Columns["TienVuotThieuLK"],
                TinhTienLK(tietLK, Convert.ToDecimal(drDT["TietChuan"])));
        }

        private void SuaSoLieu(DataRow dr, DataRow drDT)
        {
            dr["TietChuan"] = drDT["TietChuan"];
            dr["TietDayCN"] = drDT["TietDayCN"];
            dr["TietDayCT"] = drDT["TietDayCT"];
            dr["TietDayThay"] = drDT["TietDayThay"];
            dr["TietBuKem"] = drDT["TietBuKem"];
            dr["TongTietNghi"] = drDT["TongTietNghi"];

            var maLuong = drDT["MaLuong"].ToString();
            var tietLK = TinhTietLK(maLuong) + Convert.ToDecimal(dr["TietVuotThieu"]); ;
            dr["TietVuotThieuLK"] = tietLK;
            dr["TienVuotThieuLK"] = TinhTienLK(tietLK, Convert.ToDecimal(drDT["TietChuan"]));
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

        private DataTable LaySoLieu()
        {
            string sql = @"DECLARE @Nam int = {0}
                        DECLARE @Thang int = {1}
                        DECLARE @MaCN varchar(16) = '{2}'

                        SELECT	nv.MaLuong, nv.Hoten, TietChuan = isnull(nv.TietChuan, 0), TietDayCN = isnull(lcn.SoTietTT, 0), TietDayCT = isnull(lct.SoTiet, 0),
		                        TietDayThay = isnull(lcn.SoTietDT, 0), TietBuKem = isnull(db.TietBuKem, 0), TongTietNghi = isnull(lcn.SoTietNghi, 0)
                        FROM		wDMMaLuong nv
                        LEFT JOIN	(SELECT nv.MaLuong, SoTietTT = sum(SoTietTT), SoTietDT = sum(SoTietDT), SoTietNghi = sum(SoTietNghi)
			                        FROM LuongGVCN cn INNER JOIN DMNVien nv on cn.MaGV = nv.MaNV
			                        WHERE cn.Nam = @Nam and cn.Thang = @Thang
			                        GROUP BY nv.MaLuong, Nam, Thang) lcn
	                        ON nv.MaLuong = lcn.MaLuong
                        LEFT JOIN	(SELECT nv.MaLuong, SoTiet = sum(SoTiet)
			                        FROM LuongGVCT ct INNER JOIN DMNVien nv on ct.MaGV = nv.MaNV
			                        WHERE ct.Nam = @Nam and ct.Thang = @Thang
			                        GROUP BY nv.MaLuong, Nam, Thang) lct
	                        ON nv.MaLuong = lct.MaLuong
                        LEFT JOIN	(SELECT MaLuong, Nam, Thang, TietBuKem = sum(SoTiet)
			                        FROM GVDayBu
			                        GROUP BY MaLuong, Nam, Thang) db
	                        ON nv.MaLuong = db.MaLuong and db.Nam = @Nam and db.Thang = @Thang
                        WHERE	nv.MaCN = @MaCN and (lcn.SoTietTT > 0 OR lct.SoTiet > 0)";
            
            return db.GetDataTable(string.Format(sql, nam, thang, maCN));
        }

        private decimal TinhTietLK(string maLuong)
        {
            string sql = "select sum(TietVuotThieu) from SoTietGVCH where MaLuong = '{0}' and ((Nam = {1} and Thang < {2}) or Nam < {1})";
            
            var o = db.GetValue(string.Format(sql, maLuong, nam, thang));
            if (o == null || o.ToString() == string.Empty) return 0;
            return Convert.ToDecimal(o);
        }

        private decimal TinhTienLK(decimal tietLK, decimal tietChuan)
        {
            if (tietLK > 0) return tietLK * 150000;

            return Math.Round(tietLK * (115000 / tietChuan), 0);
        }
    }
}
