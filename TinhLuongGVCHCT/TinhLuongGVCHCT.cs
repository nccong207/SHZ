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

namespace TinhLuongGVCHCT
{
    public class TinhLuongGVCHCT : ICControl
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
                XtraMessageBox.Show("Số liệu chưa có, phần mềm sẽ tự động tổng hợp lương giáo viên công ty tháng " + thang
                    + " để bạn tiến hành cập nhật", "Xác nhận", MessageBoxButtons.OK);
            }
            else
            {
                if (XtraMessageBox.Show("Số liệu đã có, bạn có muốn phần mềm cập nhật lại lương GVCH công ty tháng " + thang + " không?",
                    "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }

            DataTable dtDT = LaySoLieu();
            if (dtDT.Rows.Count == 0)
            {
                XtraMessageBox.Show("Số liệu lương giáo viên công ty tháng " + thang
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
            gvMain.SetFocusedRowCellValue(gvMain.Columns["LuongGVCT"], drDT["LuongGVCT"]);
        }

        private void SuaSoLieu(DataRow dr, DataRow drDT)
        {
            dr["ThuNhapCB"] = drDT["ThuNhapCB"];
            dr["LuongGVCT"] = drDT["LuongGVCT"];
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

                        SELECT	nv.MaLuong, nv.Hoten, ThuNhapCB = isnull(nv.ThuNhapCB, 0), LuongGVCT = lct.TongLuong
                        FROM		wDMMaLuong nv
                        INNER JOIN	(SELECT nv.MaLuong, TongLuong = sum(ct.TongLuong)
			                        FROM LuongGVCT ct INNER JOIN DMNVien nv on ct.MaGV = nv.MaNV
			                        WHERE ct.Nam = @Nam and ct.Thang = @Thang
			                        GROUP BY nv.MaLuong, Nam, Thang) lct
	                        ON nv.MaLuong = lct.MaLuong
                        WHERE	lct.TongLuong > 0";
            
            return db.GetDataTable(string.Format(sql, nam, thang));
        }
    }
}
