using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;
using DevExpress.XtraGrid.Views.Grid;
using CDTLib;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid;

namespace LayDSPhatLuong
{
    public partial class FrmDanhSach : DevExpress.XtraEditors.XtraForm
    {
        private Database db = Database.NewStructDatabase();
        private string _nhom;
        private bool _first = true;
        private DataTable dtStruct = null;

        public FrmDanhSach(string nhom, BindingSource bs)
        {
            InitializeComponent();
            gcDS.DataSource = bs;
            _nhom = nhom;
            if (nhom == "NV")
                this.WindowState = FormWindowState.Maximized;
            FormatGrid();   //tự động format grid theo CDT
            gvDS.BestFitColumns();
            gvDS.Columns[0].Width = 80;
        }

        //sự kiện dùng để format grid detail
        void gcDS_ViewRegistered(object sender, ViewOperationEventArgs e)
        {
            FormatDetail(e.View as GridView); 
            if (!_first)
                return;
            _first = false;
            this.WindowState = FormWindowState.Maximized;
        }

        private void FormatDetail(GridView gvDetail)
        {
            string tbName = "";
            switch (_nhom)
            {
                case "GV":
                    tbName = "LuongGVCN";
                    break;
                case "CT":
                    tbName = "LuongGVCT";
                    break;
                case "NV":
                    tbName = "LuongNV";
                    break;
            }
            if (dtStruct == null)
                dtStruct = db.GetDataTable("select f.* from sysField f inner join sysTable t on f.sysTableID = t.sysTableID " +
                    "where t.TableName = '" + tbName + "' and t.sysPackageID = " + Config.GetValue("sysPackageID").ToString() + " order by TabIndex");
            
            for (int i = 0; i < dtStruct.Rows.Count; i++)
            {
                DataRow dr1 = dtStruct.Rows[i];
                string f = dr1["FieldName"].ToString();
                GridColumn gcl = gvDetail.Columns.ColumnByFieldName(f);
                if (gcl == null)
                    continue;
                int t = Int32.Parse(dr1["Type"].ToString());
                int n = Int32.Parse(dr1["TabIndex"].ToString());
                gcl.Caption = dr1["LabelName"].ToString();
                gcl.VisibleIndex = n;
                if ((t == 5 || t == 8) && dr1["EditMask"].ToString() != string.Empty && !dr1["EditMask"].ToString().StartsWith("@"))
                {
                    gcl.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    gcl.DisplayFormat.FormatString = dr1["EditMask"].ToString();
                }
                if (t == 9)
                {
                    gcl.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    gcl.DisplayFormat.FormatString = "dd/MM/yyyy";
                }
                if (t == 3 || t == 6)
                    gcl.Visible = false;
                if (Boolean.Parse(dr1["IsFixCol"].ToString()))
                    gcl.Fixed = FixedStyle.Left;
            }
            gvDetail.BestFitColumns();
        }

        private void FormatGrid()
        {
            gvDS.Columns["MaNV"].Caption = "Mã";
            gvDS.Columns["Hoten"].Caption = "Họ tên"; 
            gvDS.Columns["MaNV"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            gvDS.Columns["Hoten"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            gvDS.Columns["TongLuong"].Caption = "Tổng lương";
            gvDS.Columns["TongLuong"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gvDS.Columns["TongLuong"].DisplayFormat.FormatString = "### ### ###";
            gvDS.Columns["Chon"].OptionsColumn.AllowEdit = true;
            gvDS.Columns["Chon"].Caption = "Chọn";
            gvDS.Columns["Chon"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            switch (_nhom)
            {
                case "GV":
                    gvDS.Columns["Luong"].Caption = "Tiền lương";
                    gvDS.Columns["Luong"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    gvDS.Columns["Luong"].DisplayFormat.FormatString = "### ### ###";
                    gvDS.Columns["PhuCap"].Caption = "Chi nhánh phụ cấp";
                    gvDS.Columns["PhuCap"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    gvDS.Columns["PhuCap"].DisplayFormat.FormatString = "### ### ###";
                    gcDS.ViewRegistered += new ViewOperationEventHandler(gcDS_ViewRegistered);
                    break;
                case "CT":
                    gcDS.ViewRegistered += new ViewOperationEventHandler(gcDS_ViewRegistered);
                    break;
                case "NV":
                    FormatDetail(gvDS);
                    break;
            }
            for (int i = 0; i < gvDS.Columns.Count; i++)
                if (gvDS.Columns[i].FieldName != "Chon")
                    gvDS.Columns[i].OptionsColumn.AllowEdit = false;
        }

        //kiểm tra đã chọn chưa trước khi đóng form
        private void btnLayDS_Click(object sender, EventArgs e)
        {
            BindingSource bs = gcDS.DataSource as BindingSource;
            DataTable dt = _nhom == "NV" ? bs.DataSource as DataTable : (bs.DataSource as DataSet).Tables[0];
            DataView dv = new DataView(dt);
            dv.RowFilter = "Chon = True";
            if (dv.Count == 0)
            {
                XtraMessageBox.Show("Bạn chưa chọn nhân viên/giáo viên để phát lương!");
                return;
            }
            this.Close();
        }

        //tự động chọn tất cả và đóng form
        private void btnLayTatCa_Click(object sender, EventArgs e)
        {
            BindingSource bs = gcDS.DataSource as BindingSource;
            DataTable dt = _nhom == "NV" ? bs.DataSource as DataTable : (bs.DataSource as DataSet).Tables[0];
            foreach (DataRow dr in dt.Rows)
                dr["Chon"] = true;
            this.Close();
        }

        //kiểm tra danh sách chọn phải thuộc cùng một chi nhánh thanh toán
        private void FrmDanhSach_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_nhom != "NV")
                return;
            BindingSource bs = gcDS.DataSource as BindingSource;
            DataTable dt = bs.DataSource as DataTable;
            DataView dv = new DataView(dt);
            dv.RowFilter = "Chon = True";
            if (dv.Count == 0)
                return;
            string cntt = dv[0]["CNTT"].ToString();
            foreach (DataRowView drv in dv)
                if (drv["CNTT"].ToString() != cntt)
                {
                    XtraMessageBox.Show("Vui lòng chọn các nhân viên cùng chi nhánh thanh toán cho một lần phát lương!",
                        Config.GetValue("PackageName").ToString());
                    e.Cancel = true;
                    return;
                }
        }
    }
}