using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using CDTLib;
using System.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace TinhLuongGVCT
{
    public class TinhLuongGVCT :ICControl
    {
        #region ICControl Members
        private InfoCustomControl info = new InfoCustomControl(IDataType.Single);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();
        GridView gvMain;        
        frmThang frm;

        public void AddEvent()
        {
            data.FrmMain.Shown += new EventHandler(FrmMain_Shown);
            //gvMain = (data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
 
        }

        void FrmMain_Shown(object sender, EventArgs e)
        {
            gvMain = (data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            gvMain.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(gvMain_CellValueChanged);
            gvMain.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(gvMain_CellValueChanging);
            //gvMain.KeyDown += new KeyEventHandler(gvMain_KeyDown);
            (data.BsMain.DataSource as DataTable).RowDeleting += new DataRowChangeEventHandler(TinhLuongGVCT_RowDeleting);
            frm = new frmThang(gvMain);
            frm.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            frm.Text = "Chọn tháng tính lương";
            frm.ShowDialog();
        }

        void TinhLuongGVCT_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            KiemTraThangLuong(e.Row["MaLop"].ToString(), int.Parse(e.Row["Thang"].ToString()));
        }

        void gvMain_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Name.ToUpper().Equals("CLLUONGDAY") || e.Column.Name.ToUpper().Equals("CLMALOP") || e.Column.Name.ToUpper().Equals("CLMALOPEX"))
            {
                int ThangCurr = frm.iThang;
                string MaLop="";

                if (gvMain.GetFocusedRowCellValue("MaLop") != null)
                {
                    if (gvMain.GetFocusedRowCellValue("Thang") != null && gvMain.GetFocusedRowCellValue("Thang").ToString() != "")
                        ThangCurr = Int32.Parse(gvMain.GetFocusedRowCellValue("Thang").ToString());
                    MaLop = gvMain.GetFocusedRowCellValue("MaLop").ToString();
                    //int MaxThang = CalcMaxThang(MaLop);
                    KiemTraThangLuong(MaLop, ThangCurr);
                }
            }
        }

        //void gvMain_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Delete)
        //    {
        //        int[] a = gvMain.;
        //        for (int i = 0; i < a.Length; i++)
        //        {
        //            string MaLop = gvMain.GetFocusedRowCellValue("MaLop").ToString();
                    
                    
        //        }
        //    }
        //}
        void gvMain_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Name.ToLower().Equals("clmalop") || e.Column.Name.ToLower().Equals("clmalopex"))
            {
                int ThangCurr = frm.iThang;
                string MaLop = "";

                if (gvMain.GetFocusedRowCellValue("MaLop") != null)
                {
                    if (gvMain.GetFocusedRowCellValue("Thang") != null && gvMain.GetFocusedRowCellValue("Thang").ToString() != "")
                        ThangCurr = Int32.Parse(gvMain.GetFocusedRowCellValue("Thang").ToString());
                    MaLop = e.Value.ToString();
                    //int MaxThang = CalcMaxThang(MaLop);
                    KiemTraThangLuong(MaLop, ThangCurr);
                }
            }
        }
        void KiemTraThangLuong(string Malop,int ThangCurr)
        {
            //string sql = string.Format("select isnull(max(thang),0) from luonggvct where malop = '{0}' and nam = {1}", Malop, Config.GetValue("NamLamViec").ToString());
            DataTable dt = data.BsMain.DataSource as DataTable;
            DataRow[] rows = dt.Select("MaLop = '" + Malop + "' and Nam = " + Config.GetValue("NamLamViec").ToString(), " Thang Desc");
            
            int MaxThang = 0;
            if (rows.Length > 0)
                MaxThang = int.Parse(rows[0]["Thang"].ToString());
            
            if (ThangCurr < MaxThang)
            {
                XtraMessageBox.Show("Bạn đang thay đổi dữ liệu bảng lương tháng " + ThangCurr + " không phải là bảng lương mới nhất (tháng " + MaxThang + " ).\nNếu tiếp tục có thể gây lỗi dữ liệu khi đối chiếu lương giáo viên công ty !", Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
