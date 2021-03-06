using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTLib;

namespace PBDoanhThu
{
    public partial class frmShow : DevExpress.XtraEditors.XtraForm
    {
        public frmShow()
        {
            InitializeComponent();
        }
        public int thang = 0;
        public int loaiPB = 0;
        private void frmShow_Load(object sender, EventArgs e)
        {
            spinThang.EditValue = Config.GetValue("KyKeToan") != null ? Config.GetValue("KyKeToan") : DateTime.Now.Month.ToString();
            radioLoaiPB.EditValue = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            thang = int.Parse(spinThang.EditValue.ToString());
            loaiPB = int.Parse(radioLoaiPB.EditValue.ToString());
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}