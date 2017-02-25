using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;

namespace HoiThamHV
{
    public partial class frmBCQuaTrinhDK : DevExpress.XtraEditors.XtraForm
    {
        public frmBCQuaTrinhDK()
        {
            InitializeComponent();
        }

        public string HVTVID = "";
        Database db = Database.NewDataDatabase();
        private void frmBCQuaTrinhDK_Load(object sender, EventArgs e)
        {
            string sql = string.Format(@"   select	NgayDK,m.MaLop,l.NgayBDKhoa,l.NgayKTKhoa,m.ConLai,m.MaCNDK,m.MaCNHoc,nv.HoTen
                                            from	mtdk m 
		                                            inner join dmlophoc l on m.malop = l.malop
		                                            left join dmnvien nv on m.manv = nv.manv
                                            where hvtvid = '{0}' order by ngaydk", HVTVID);
            using (DataTable dt = db.GetDataTable(sql))
            {
                if (dt.Rows.Count > 0)
                {
                    gcListQTDK.DataSource = dt;
                    gvQTDK.BestFitColumns();
                }
            }
        }
    }
}