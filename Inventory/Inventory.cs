using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;

namespace Inventory
{
    class Inventory : IC
    { 
        #region IC Members

        public void Execute(DataRow drMenu)
        {
            int menuID = Int32.Parse(drMenu["MenuPluginID"].ToString());
            if (_lstInfo[0].CType == ICType.Custom && _lstInfo[0].MenuID == menuID)
            {
                FormHTK frm = new FormHTK();
                frm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                frm.Text = _lstInfo[0].MenuName;
                frm.ShowDialog();
            }
        }

        public List<InfoCustom> LstInfo
        {
            get { return _lstInfo; }
        }

        #endregion

        private List<InfoCustom> _lstInfo = new List<InfoCustom>();
        public Inventory()
        {
            InfoCustom ic = new InfoCustom(1003, "Tính giá tồn kho", "Quản lý kho");
            _lstInfo.Add(ic);
        }

    }
}
