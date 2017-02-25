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
    public partial class frmBCDiemThi : DevExpress.XtraEditors.XtraForm
    {
        public frmBCDiemThi()
        {
            InitializeComponent();
        }
        public string HVID = "";
        Database db = Database.NewDataDatabase();
        private void frmBCDiemThi_Load(object sender, EventArgs e)
        {
            string sql = string.Format(@"   select top 4 MaLop,Viet,Nghe,Noi,DiemTB 
                                            from dmkq where hvid = '{0}' 
                                            order by kqid desc"
                                            , HVID);
            using(DataTable dt = db.GetDataTable(sql))
            {
                if (dt.Rows.Count > 0)
                {
                    gcListDiemThi.DataSource = dt;
                    gvListDiemThi.BestFitColumns();                    
                }
            }
        }
    }
}