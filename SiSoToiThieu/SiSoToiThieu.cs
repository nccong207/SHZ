using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using Plugins;
using CDTDatabase;
using CDTLib;
using System.Windows.Forms;
using System.Data;

namespace SiSoToiThieu
{
    public class SiSoToiThieu:IC
    {       

        #region IC Members

        //private List<InfoCustom> _lstInfo = new List<InfoCustom>();

        //public SiSoToiThieu()
        //{
        //    InfoCustom ic = new InfoCustom(1093, "Theo dõi sỉ số tối thiểu", "Quản lý học viên");
        //    _lstInfo.Add(ic);
        //}

        //public void Execute(DataRow drMenu)
        //{
        //    int menuID = Int32.Parse(drMenu["MenuPluginID"].ToString());
        //    if (_lstInfo[0].CType == ICType.Custom && _lstInfo[0].MenuID == menuID)
        //    {
        //        frmDSLop frm = new frmDSLop();
        //        frm.Text = "Danh sách lớp";
        //        frm.ShowDialog();
        //    }
        //}
                    
        //public List<InfoCustom> LstInfo
        //{
        //    get { throw new Exception("The method or operation is not implemented."); }
        //}

         private List<InfoCustom> _lstInfo = new List<InfoCustom>();

        public SiSoToiThieu()
        {
            InfoCustom ic = new InfoCustom(1007, "Theo dõi sỉ số tối thiểu", "Quản lý học viên");
            _lstInfo.Add(ic);
        }

        public void Execute(System.Data.DataRow drMenu)
        {
            int menuID = Int32.Parse(drMenu["MenuPluginID"].ToString());
            if (_lstInfo[0].CType == ICType.Custom && _lstInfo[0].MenuID == menuID)
            {
                frmDSLop frm = new frmDSLop();
                frm.Text = "Danh sách lớp";
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
