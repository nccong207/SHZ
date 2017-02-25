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

namespace LichChieuSinh
{
    public partial class frmShow : DevExpress.XtraEditors.XtraForm
    {
        Database db = Database.NewDataDatabase();
        public DataTable dtLopHoc;
        public DataTable dtGVPT;
        public int iNam = 0;
        public int iThang =0;
        public string MaCN = "";

        public frmShow()
        {
            InitializeComponent();
        }

        private void frmShow_Load(object sender, EventArgs e)
        {
            spNam.Value = Convert.ToDecimal(Config.GetValue("NamLamViec"));
            spThang.Value = DateTime.Today.Month;
            iNam = Convert.ToInt32(spNam.Value);
            iThang = Convert.ToInt32(spThang.Value);
            loadMaCN();            
        }

        private void loadMaCN()
        {
            DataTable dtCN;
            string sql = @" SELECT	MaBP, TenBP, DiaChi, DienThoai
                            FROM	DMBoPhan
                            WHERE	MaBP NOT IN ('KT','QL')";
            dtCN = db.GetDataTable(sql);
            lookMaCN.Properties.DataSource = dtCN;
            lookMaCN.Properties.ValueMember = "MaBP";
            lookMaCN.Properties.DisplayMember = "MaBP";
            
            if (dtCN.Rows.Count > 0)
                lookMaCN.EditValue = dtCN.Rows[0]["MaBP"];
            lookMaCN.Properties.BestFit();
        }

        private DataTable getGVPhuTrach(string _MaCN)
        {
            string sql = string.Format("EXEC sp_GiaoVienPT {0}", _MaCN);
            return db.GetDataTable(sql);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lookMaCN.EditValue == null || string.IsNullOrEmpty(lookMaCN.EditValue.ToString()))
                return;

            MaCN = lookMaCN.EditValue.ToString();
            iNam = Convert.ToInt32(spNam.Value);
            iThang = Convert.ToInt32(spThang.Value);
            dtGVPT = getGVPhuTrach(MaCN);
            string sql = string.Format(@"EXEC sp_LichChieuSinh {0},{1},{2}", iThang, iNam, MaCN);
            dtLopHoc = db.GetDataTable(sql);

            this.Close();
        }
    }
}