using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;
using CDTLib;
using DevExpress.XtraGrid.Views.Grid;

namespace TongHopLuong
{
    public partial class FrmThang : DevExpress.XtraEditors.XtraForm
    {
        private Database db = Database.NewDataDatabase();
        private GridView _gvCCGV;
        private DataTable _dtHocVien;

        public FrmThang(GridView gvCCGV)
        {
            InitializeComponent();
            _gvCCGV = gvCCGV;
            if (Config.GetValue("KyKeToan") != null && Config.GetValue("KyKeToan").ToString() != null)
                seThang.Text = Config.GetValue("KyKeToan").ToString();
            else
                seThang.Value = DateTime.Today.Month;
        }

        //câu sql nhóm 3 bảng lương giáo viên, công ty và nhân viên lại theo nhóm lương
        private DataTable LayDSLuong()
        {
            string nam = Config.GetValue("NamLamViec").ToString();
            //string sql = "select MaLuong, Hoten, sum(TongLuong) as TongLuong from " +
            //    "(select MaNV, TongLuong from LuongNV where Thang = " + seThang.Text + " and Nam = " + nam +
            //    "union all select MaGV, TongLuong from LuongGVCN where Thang = " + seThang.Text + " and Nam = " + nam +
            //    "union all select MaGV, TongLuong from LuongGVCT where Thang = " + seThang.Text + " and Nam = " + nam +
            //    ")t inner join DMNVien nv on t.MaNV = nv.MaNV group by nv.MaLuong,nv.Hoten ";
            string sql = @"select MaLuong, ( SELECT TOP 1 Hoten FROM DMNVien WHERE MaLuong = nv.MaLuong ) as  HoTen, sum(TongLuong) as TongLuong 
                        from 
                        (select l.MaNV, l.TongLuong
                         from LuongNV l inner join dmnvien nv on l.manv = nv.manv
                         where Thang = " + seThang.Text + @" and Nam = "+ nam + @"
                         union all 
                         select l.MaGV, sum(l.TongLuong) as TongLuong
                         from LuongGVCN l inner join dmnvien nv on l.MaGV = nv.manv
                         where Thang = " + seThang.Text + @" and Nam = "+ nam + @"
                         group by l.MaGV
                         union all 
                         select l.MaGV, sum(l.TongLuong) as TongLuong
                         from LuongGVCT l inner join dmnvien nv on l.MaGV = nv.manv
                         where Thang = " + seThang.Text + @" and Nam = " + nam + @"
                         group by l.MaGV
                        )t inner join DMNVien nv on t.MaNV = nv.MaNV 
                        group by nv.MaLuong 
                        having sum(TongLuong) > 0";
            DataTable dt = db.GetDataTable(sql);
            return dt;
        }

        private void TaoDSLuong(int m)
        {
            string nam = Config.GetValue("NamLamViec").ToString();
            _gvCCGV.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
            _gvCCGV.ActiveFilterString = "Thang = " + m.ToString() + " and Nam = " + nam;
            if (_gvCCGV.DataRowCount > 0)
                return;
            _dtHocVien = LayDSLuong();
            foreach (DataRow drHV in _dtHocVien.Rows)
            {
                _gvCCGV.AddNewRow();
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Thang"], seThang.Text);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Nam"], nam);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaLuong"], drHV["MaLuong"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Hoten"], drHV["Hoten"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["TienLuong"], drHV["TongLuong"]);
                _gvCCGV.UpdateCurrentRow();
            }
            _gvCCGV.BestFitColumns();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            TaoDSLuong(Int32.Parse(seThang.Text));
            this.Close();
        }
    }
}