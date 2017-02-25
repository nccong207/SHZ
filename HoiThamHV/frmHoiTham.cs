using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace HoiThamHV
{
    public partial class frmHoiTham : DevExpress.XtraEditors.XtraForm
    {
        public int Value ;
        public frmHoiTham()
        {
            InitializeComponent();
        }       

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (radioHT.EditValue == null)
                return;
            if(radioHT.EditValue.ToString()=="0")
            {
                Value = 0;
                //frmHVTV frm =new frmHVTV();
                //frm.Text="Danh sách học viên tư vấn";
                //frm.ShowDialog();
            }
            else if (radioHT.EditValue.ToString() == "1")
            {
                Value = 1;
                //frmHVDK frm = new frmHVDK();
                //frm.Text = "Danh sách học viên đăng ký";
                //frm.ShowDialog();
            }
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Value = 2;
            this.Close();
        }        
    }
}