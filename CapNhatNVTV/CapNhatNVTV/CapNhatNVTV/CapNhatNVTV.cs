using System;
using System.Collections.Generic;
using System.Text;
using System.Data;  
using CDTDatabase;
using CDTLib;
using Plugins;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;

namespace CapNhatNVTV
{
    public class CapNhatNVTV : ICReport      
    {
        private Database db = Database.NewDataDatabase();
        private DataCustomReport data;
        private InfoCustomReport info = new InfoCustomReport(IDataType.Report);
        GridView gvMain;
        #region ICReport Members

        public DataCustomReport Data
        {
            set { data = value; }
        }

        public void Execute()
        {
            gvMain = (data.FrmMain.Controls.Find("gridControlReport", true)[0] as GridControl).MainView as GridView;
            SimpleButton btnXL = data.FrmMain.Controls.Find("btnXuLy", true)[0] as SimpleButton;
            btnXL.Click += new EventHandler(btnXL_Click);
            //UpdateNVTV();
        }

        private void UpdateNVTV()
        {
            
            //DataView dv = new DataView(data.DtSource);
            DataView dv = gvMain.DataSource as DataView;
            dv.RowFilter = "[Chọn] = 1";
            if (dv.Count == 0)
            {
                XtraMessageBox.Show("Vui lòng đánh dấu chọn vào học viên cần xử lý", Config.GetValue("PackageName").ToString());
                return;
            }
            string sql = "  UPDATE DMHVTV SET MaNVTV = '{0}' WHERE  HVTVID = {1};  ";
            string query = "";
            DanhSachNVTV frm = new DanhSachNVTV();
            frm.ShowDialog();

            if (frm.DialogResult == DialogResult.OK)
            {
                if(frm.NhanVien.ToString() != "" )
                    foreach (DataRowView drv in dv )
	                {
                        query += string.Format(sql,frm.NhanVien,(int)drv.Row["HVTVID"]);
	                }
                if (db.UpdateByNonQuery(query))
                    XtraMessageBox.Show("Cập nhật thành công", Config.GetValue("PackageName").ToString());

            }
        }

        void btnXL_Click(object sender, EventArgs e)
        {
            UpdateNVTV();
        }

        public InfoCustomReport Info
        {
            get { return info; }
        }

        #endregion
    }
}
