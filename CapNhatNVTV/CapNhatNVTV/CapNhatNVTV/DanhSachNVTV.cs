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

namespace CapNhatNVTV
{
    public partial class DanhSachNVTV : DevExpress.XtraEditors.XtraForm
    {
        public string NhanVien;
	
        public DanhSachNVTV()
        {
            InitializeComponent();
        }

        private void DanhSachNVTV_Load(object sender, EventArgs e)
        {
            Database db = Database.NewDataDatabase();
            string sql = "SELECT * FROM DMNVien WHERE isNghiViec = 0 AND isNV = 1";
            DataTable dt = new DataTable();
            dt = db.GetDataTable(sql);
            gluNV.Properties.DataSource = dt;
            gluNV.Properties.DisplayMember = "Hoten";
            gluNV.Properties.ValueMember = "MaNV";
            gluNV.Properties.PopupFormMinSize = new Size(500,300);
            gluNV.Properties.View.BestFitColumns();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (NhanVien == "" || NhanVien == null)
            {
                XtraMessageBox.Show("Vui lòng chọn nhân viên tư vấn", Config.GetValue("PackageName").ToString());
                return;
            }
            this.DialogResult = DialogResult.OK;
            
        }

        private void gluNV_EditValueChanged(object sender, EventArgs e)
        {
            GridLookUpEdit glu = sender as GridLookUpEdit;
            NhanVien = glu.Properties.View.GetFocusedRowCellValue("MaNV").ToString();
        }
    }
}