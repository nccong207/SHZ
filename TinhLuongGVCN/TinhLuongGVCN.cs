using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using CDTLib;
using System.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace TinhLuongGVCN
{
    public class TinhLuongGVCN :ICControl
    {
        #region ICControl Members
        private InfoCustomControl info = new InfoCustomControl(IDataType.Single);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();


        public void AddEvent()
        {
            data.FrmMain.Shown += new EventHandler(FrmMain_Shown);
            data.FrmMain.KeyDown += new System.Windows.Forms.KeyEventHandler(FrmMain_KeyDown);
        }

        void FrmMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.F9)
            {
                GridControl gcMain = data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl;
                FrmThang frm = new FrmThang(gcMain);
                frm.ShowDialog();
            }
        }

        void FrmMain_Shown(object sender, EventArgs e)
        {
            //GridView gvMain = (data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            GridControl gcMain = data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl;
            FrmThang frm = new FrmThang(gcMain);
            frm.ShowDialog();
        }

        public DataCustomFormControl Data
        {
            set { data = value; }
        }

        public InfoCustomControl Info
        {
            get { return info; }
        }

        #endregion
    }
}
