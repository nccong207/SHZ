using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using CDTLib;
using CDTDatabase;

namespace TinhLuongGVCT
{
    public partial class frmThang : DevExpress.XtraEditors.XtraForm
    {
        public frmThang(GridView gvDetail)
        {
            InitializeComponent();
            _gvDetail = gvDetail;
        }
        GridView _gvDetail;
        DataTable _dtLuongThangTruoc;
        DataTable _dtDMHVCT;
        Database db = Database.NewDataDatabase();
        public int iThang = 0;
        public int iNam = 0;
        public string iChiNhanh = "";
        private void frmThang_Load(object sender, EventArgs e)
        {
            if (Config.GetValue("KyKeToan") != null)
                spinThang.EditValue = Config.GetValue("KyKeToan");
            else
                spinThang.EditValue = DateTime.Today.Month;

            string sql = @"SELECT * FROM DMBoPhan";
            DataTable dt = db.GetDataTable(sql);

            cbChiNhanh.DataSource = dt;
            cbChiNhanh.DisplayMember = "MaBP";
            cbChiNhanh.ValueMember = "MaBP";

        }

        private void btnTinhLuong_Click(object sender, EventArgs e)
        {
            int nam = DateTime.Today.Year;
            iNam = Int32.Parse(Config.GetValue("NamLamViec").ToString());
            iThang = int.Parse(spinThang.EditValue.ToString());

            //Lấy dữ liệu lương của tháng trước
            int dataNam = iThang == 1 ? iNam - 1 : iNam;
            int dataThang = iThang == 1 ? 12 : iThang - 1;
            string sqlData = string.Format("SELECT * from LuongGVCT WHERE Thang = {0}  and Nam = {1} ", dataThang, dataNam);
            _dtLuongThangTruoc = db.GetDataTable(sqlData);

            //Get data lop hoc
            string lophocsql = string.Format("select * from DMHVCT WHERE MONTH(NgayBD) = {0} and YEAR(NgayBD) = {1}", iThang, iNam);
            _dtDMHVCT = db.GetDataTable(lophocsql);

            iChiNhanh = cbChiNhanh.SelectedValue.ToString();
           
            if (Config.GetValue("NamLamViec") != null)
                nam = Int32.Parse(Config.GetValue("NamLamViec").ToString());
            _gvDetail.ActiveFilterString = "Thang = '" + spinThang.EditValue.ToString() + "' and Nam = '" + nam.ToString() + "' and MaCN = '" + cbChiNhanh.SelectedValue.ToString() + "'";
            if (_gvDetail.DataRowCount > 0)
            {
                this.Close();
                return;
            }
            string sql = @"Select * From DMHVCT HV Where LuongDu > 100 and MaCN = '" + iChiNhanh  + "'";
            DataTable dt = db.GetDataTable(sql);
            this.Close();
            foreach (DataRow row in dt.Rows)
            {
                if (!string.IsNullOrEmpty(row["MaCN"].ToString()))
                {
                    _gvDetail.AddNewRow();
                    _gvDetail.SetFocusedRowCellValue(_gvDetail.Columns["Thang"], spinThang.EditValue);
                    _gvDetail.SetFocusedRowCellValue(_gvDetail.Columns["MaLop"], row["MaLop"]);
                    _gvDetail.SetFocusedRowCellValue(_gvDetail.Columns["MaGV"], row["MaGV"]);
                    _gvDetail.SetFocusedRowCellValue(_gvDetail.Columns["MaCN"], row["MaCN"]);
                    _gvDetail.SetFocusedRowCellValue(_gvDetail.Columns["LuongCLTT"], GetLuongConLaiThangTruoc(spinThang.EditValue.ToString(), row["MaLop"].ToString(), row["MaGV"].ToString(), iNam));
                    _gvDetail.SetFocusedRowCellValue(_gvDetail.Columns["LuongGio"], row["GLuong"].ToString() != "" ? row["GLuong"] : 0);
                    if (row["PCXe"].ToString() != "")
                        _gvDetail.SetFocusedRowCellValue(_gvDetail.Columns["PCXe"], row["PCXe"]);
                    _gvDetail.SetFocusedRowCellValue(_gvDetail.Columns["Nam"], nam);
                }
            }
            _gvDetail.BestFitColumns();
        }

        private string GetLuongConLaiThangTruoc(string curThang, string malop, string maGv, int nam)
        {
            int thang = 0;
            int.TryParse(curThang, out thang);
            if (thang == 0 || thang > 12 || thang < 1) return "0";

            string sql = string.Format("MaLop = '{0}' and MaGV = '{1}'", malop, maGv);
            DataRow[] drs = _dtLuongThangTruoc.Select(sql);
            if (drs.Length > 0)
            {
                return drs[0]["LuongCL"].ToString();
            }
            else
            {
                //Trường hợp lớp học mới vừa được tạo thì lấy lương dư hiện tại.
                string sqlFilter = string.Format("MaLop = '{0}'", malop);
                DataRow[] dtlophoc = _dtDMHVCT.Select(sqlFilter);

                if (dtlophoc.Length > 0)
                {
                    return dtlophoc[0]["LuongDu"].ToString();
                }
            }
            return "0";
        }
    }
}