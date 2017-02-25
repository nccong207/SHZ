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
    public partial class frmBCChamSocHV : DevExpress.XtraEditors.XtraForm
    {
        public frmBCChamSocHV()
        {
            InitializeComponent();
        }
        public string HVTVID = "";
        Database db = Database.NewDataDatabase();
        private void frmBCChamSocHV_Load(object sender, EventArgs e)
        {
            string sql = string.Format(@"Select top 6 dm.*,ph.PHoi from DMQTCSHV dm inner join dmphanhoi ph on dm.phid = ph.phid  where HVTVID = '{0}' order by ID desc", HVTVID);
            using (DataTable dt = db.GetDataTable(sql))
            {
                if (dt.Rows.Count > 0)
                {
                    gcListCSHV.DataSource = dt;
                    gvListCSHV.BestFitColumns();
                }
            }
        }
    }
}