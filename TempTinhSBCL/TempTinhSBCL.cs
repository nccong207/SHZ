using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using CDTLib;
using DevExpress;
using DevExpress.XtraEditors;
using Plugins;
namespace TempTinhSBCL
{
    public class TempTinhSBCL:IC
    {
        #region IC Members

        private List<InfoCustom> _lstInfo = new List<InfoCustom>();

        public TempTinhSBCL()
        {
            InfoCustom ic = new InfoCustom(1054, "Tinh so buoi con lai de cap nhat", "Quản lý học viên");
            _lstInfo.Add(ic);
        }

        public void Execute(System.Data.DataRow drMenu)
        {
            int menuID = Int32.Parse(drMenu["MenuPluginID"].ToString());
            if (_lstInfo[0].CType == ICType.Custom && _lstInfo[0].MenuID == menuID)
            {
                frmHVCanSuaL frm = new frmHVCanSuaL();
                frm.Text = "Danh sach record can sua";
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
