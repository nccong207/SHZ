using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTLib;
using CDTDatabase;

namespace QuanLyThi
{
    public partial class fromShow : DevExpress.XtraEditors.XtraForm
    {
        public fromShow()
        {
            InitializeComponent();
        }
        Database db = Database.NewDataDatabase();
        public DataTable dtHocVien = null;
        public string MaLop="";
        public bool isNghe = false;
        public bool isNoi = false;
        public bool isViet = false;

        private void fromShow_Load(object sender, EventArgs e)
        {
            int thang = 1;
            if (Config.GetValue("KyKeToan") != null)
                thang = int.Parse(Config.GetValue("KyKeToan").ToString());
            else
                thang = DateTime.Today.Month;
            GetDSLop(thang);
        }

        void GetDSLop(int thang)
        {
            string sql = @"select * from dmlophoc where month(NgayKTKhoa) = '" + thang.ToString() + @"' 
                           and year(NgayKTKhoa) = '" + Config.GetValue("NamLamViec").ToString() + @"' 
                           and isKT ='0' and MaCN = '" + Config.GetValue("MaCN").ToString() + "'";
            DataTable dt = db.GetDataTable(sql);
            lookUpLop.Properties.DataSource = dt;
            lookUpLop.Properties.ValueMember = "MaLop";
            lookUpLop.Properties.DisplayMember = "MaLop";
            if (dt.Rows.Count > 0)
                lookUpLop.EditValue = dt.Rows[0]["MaLop"].ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lookUpLop.EditValue == null)
                return;
            if (lookUpLop.EditValue.ToString() == "" || (checkNghe.Checked == false && checkNoi.Checked == false && checkViet.Checked == false))
            {
                XtraMessageBox.Show("Chưa đủ dữ liệu để thực hiện");
                return ;
            }

            GetHocVien();
            MaLop = lookUpLop.EditValue.ToString();

            isNghe = checkNghe.Checked;
            isViet = checkViet.Checked;
            isNoi = checkNoi.Checked;

            this.Close();
        }

        void GetHocVien()
        {
            string sql = "select * from DMKQ where MaLop = '" + lookUpLop.EditValue.ToString() + "'";
            DataTable dtSub = db.GetDataTable(sql);
            if (dtSub.Rows.Count == 0)
            {
                // Add hoc vien
                sql = @"select * from MTDK MT inner join DMHVTV TV on MT.HVTVID=TV.HVTVID 
                        where MT.MaLop = '" + lookUpLop.EditValue.ToString() + "' and MT.isNghiHoc ='0' and MT.isBL = '0' order by mt.mahv asc ";
                dtSub = db.GetDataTable(sql);
                if (dtSub.Rows.Count == 0)
                    XtraMessageBox.Show("Lớp " + lookUpLop.EditValue.ToString() + " không có học viên nào!", Config.GetValue("PackageName").ToString());
                else
                    dtHocVien = dtSub;
            }            
        }
    }
}