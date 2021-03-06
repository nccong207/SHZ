using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;
using CDTLib;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Data.Filtering;

namespace LayChamCongNV
{
    public partial class FrmThang : DevExpress.XtraEditors.XtraForm
    {
        private Database db = Database.NewDataDatabase();
        private GridView _gvCCGV;
        string nam = Config.GetValue("NamLamViec").ToString();

        public FrmThang(GridView gvCCGV)
        {
            InitializeComponent();
            _gvCCGV = gvCCGV;            
            if (Config.GetValue("KyKeToan") != null && Config.GetValue("KyKeToan").ToString() != null)
                seThang.Text = Config.GetValue("KyKeToan").ToString();
            else
                seThang.Value = DateTime.Today.Month;
        }
   
        private void btnOk_Click(object sender, EventArgs e)
       {
           _gvCCGV.ActiveFilterString = "Thang = '" + seThang.EditValue.ToString() + "' and Nam = '" + nam + "'";
            if (_gvCCGV.DataRowCount > 0)
                _gvCCGV.CollapseAllGroups();
            _gvCCGV.InitNewRow += new InitNewRowEventHandler(_gvCCGV_InitNewRow);               
            this.Close();
        }

        void _gvCCGV_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            _gvCCGV.SetRowCellValue(e.RowHandle, "Thang", seThang.EditValue);
            _gvCCGV.SetRowCellValue(e.RowHandle, "Nam", nam);
        }
        
    }
}