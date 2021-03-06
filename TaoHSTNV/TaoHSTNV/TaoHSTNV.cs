using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using System.Data;

namespace TaoHSTNV
{
    public class TaoHSTNV : ICControl
    {
        private DataCustomFormControl _data;
        private InfoCustomControl _info = new InfoCustomControl(IDataType.Single);

        #region ICControl Members

        public void AddEvent()
        {
            _data.FrmMain.Shown += new EventHandler(FrmMain_Shown); //them su kien nay de add duoc cho dong Nhan vien dau tien
        }

        void FrmMain_Shown(object sender, EventArgs e)
        {
            Database db = Database.NewDataDatabase();
            DataTable dtDMHST = db.GetDataTable(@"select nv.MaNV, hs.ID 
                                    from DMHST hs, DMNVien nv 
                                    where hs.InActive = 0 and nv.IsNV = 1 and nv.MaNV in
	                                    (select MaNV from DMTCLuongNV where CNT is not null or TOT is not null)");
            AddHST(dtDMHST);
        }

        private void AddHST(DataTable dtDMHST)
        {
            GridView gvMain = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            gvMain.Columns["HSTID"].SortIndex = 0;
            gvMain.OptionsView.NewItemRowPosition = NewItemRowPosition.None;

            string s = "MaNV = '{0}' and HSTID = {1}";
            DataTable dtHST = _data.BsMain.DataSource as DataTable;

            foreach (DataRow dr in dtDMHST.Rows)
            {
                DataRow[] drs = dtHST.Select(string.Format(s, dr["MaNV"], dr["ID"]));
                if (drs.Length > 0)
                    continue;
                gvMain.AddNewRow();
                gvMain.UpdateCurrentRow();
                gvMain.SetFocusedRowCellValue(gvMain.Columns["MaNV"], dr["MaNV"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["HSTID"], dr["ID"]);
            }
            gvMain.RefreshData();
        }

        public DataCustomFormControl Data
        {
            set { _data = value; }
        }

        public InfoCustomControl Info
        {
            get { return _info; }
        }

        #endregion
    }
}
