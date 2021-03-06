using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using CDTDatabase;
using Plugins;
using CDTLib;

namespace HVNghiHoc
{
    public class HVNghiHoc:IC
    {
        #region IC Members
        private List<InfoCustom> _lstInfo = new List<InfoCustom>();

        public HVNghiHoc()
        {
            InfoCustom ic = new InfoCustom(1103, "Cho học viên nợ học phí nghỉ học ", "Quản lý học viên");
            _lstInfo.Add(ic);
        }

        public void Execute(System.Data.DataRow drMenu)
        {
            int menuID = Int32.Parse(drMenu["MenuPluginID"].ToString());
            if (_lstInfo[0].CType == ICType.Custom && _lstInfo[0].MenuID == menuID)
            {
                frmHocVien frm = new frmHocVien();
                frm.Text = "Danh sách học viên nợ học phí";
                frm.ShowDialog();
            }            
        }

        public List<InfoCustom> LstInfo
        {
            get { return _lstInfo; }
        }

        #endregion
    }
}
