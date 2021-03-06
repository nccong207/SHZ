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
using DevExpress.XtraLayout;
using DevExpress.XtraGrid.Views.Grid;

namespace TaoMaLop
{
    public class TaoMaLop : ICControl
    {
        #region ICControl Members

        private InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();

        LayoutControl lcMain;
        GridView gvLichThi;
        DataRow drMaster;
        TextEdit TenLop;
        DataTable dtNgayThi = new DataTable();
        DataTable dtMonthi = new DataTable();
        DataTable dtKyThi = new DataTable();
        public void AddEvent()
        {
            lcMain = data.FrmMain.Controls.Find("lcMain",true) [0] as LayoutControl;
            SimpleButton btn = new SimpleButton();
            btn.Name = "CreatLichThi";
            btn.Text = "Tạo lịch thi";
            LayoutControlItem lci = lcMain.AddItem("", btn);
            lci.Name = "cusLichThi";
            btn.Click += new EventHandler(btn_Click);
            gvLichThi = (data.FrmMain.Controls.Find("TLLichThi", true)[0] as GridControl).MainView as GridView;

            TenLop = data.FrmMain.Controls.Find("TenLop",true) [0] as TextEdit;

            if (data.BsMain == null)
                return;
             drMaster = (data.BsMain.Current as DataRowView).Row;

            if (drMaster == null)
                return;
            if (drMaster.RowState == DataRowState.Deleted)
                return;
            data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(data.BsMain, new EventArgs());
        }
        // Tạo lịch thi 
        void btn_Click(object sender, EventArgs e)
        {
            if (TenLop.Properties.ReadOnly)
            {
                XtraMessageBox.Show("Vui lòng chuyển qua chế độ chỉnh sửa", Config.GetValue("PackageName").ToString());
                return;
            }
            DataTable dt = new DataTable();
            dt = db.GetDataTable("SELECT * FROM TempLichHoc WHERE MaLop = '" + drMaster["MaLop"].ToString() + "'");
            if (dt.Rows.Count == 0)
            {
                XtraMessageBox.Show("Vui lòng thực hiện chức năng này sau khi lưu trữ dữ liệu", Config.GetValue("PackageName").ToString());
                return;
            }
            if (gvLichThi.DataRowCount != 0)
            {
                DialogResult result = XtraMessageBox.Show("Lịch thi đã có ! bạn có muốn tạo lại",Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                    return;
                else
                {
                    for (int i = gvLichThi.DataRowCount - 1; i >= 0; i--)
                    {
                        gvLichThi.DeleteRow(i);

                    }
                    CreateLichThi();
                }
            }
            else
                CreateLichThi();
            
        }
        void CreateLichThi()
        {
            dtMonthi = db.GetDataTable("SELECT * FROM DMMonThi WHERE NgungSD = 0 ORDER BY ThuTu");
         
            dtNgayThi = db.GetDataTable(string.Format(@" SELECT MAX(Ngay) [Max],MIN(Ngay) [Min] FROM
                                                          (
                                                          SELECT TOP 2 Ngay FROM TempLichHoc 
                                                          WHERE MaLop = '{0}'
                                                          ORDER BY Ngay DESC )w",drMaster["MaLop"].ToString()));
            dtKyThi = db.GetDataTable("SELECT * FROM DMKyThi WHERE isFinal = 1");
            if (dtMonthi.Rows.Count > 0)
            {
                foreach (DataRow  row in dtMonthi.Rows)
                {
                    gvLichThi.AddNewRow();
                    gvLichThi.SetFocusedRowCellValue(gvLichThi.Columns["MonThi"],row["MTID"]);
                    gvLichThi.SetFocusedRowCellValue(gvLichThi.Columns["KyThi"],(int)dtKyThi.Rows[0]["KTID"]);
                    if(row["ThuTu"].ToString() == "1" && dtNgayThi.Rows.Count > 0 )
                        gvLichThi.SetFocusedRowCellValue(gvLichThi.Columns["NgayThi"], (DateTime)dtNgayThi.Rows[0]["Min"]);
                    else if (row["ThuTu"].ToString() != "1" && dtNgayThi.Rows.Count > 0)
                        gvLichThi.SetFocusedRowCellValue(gvLichThi.Columns["NgayThi"], (DateTime)dtNgayThi.Rows[0]["Max"]);
                    gvLichThi.UpdateCurrentRow();
                }
            }
        }
        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataSet ds = data.BsMain.DataSource as DataSet;
            if (ds == null)
                return;
            ds.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(TaoMaLop_ColumnChanged);
        }

        void TaoMaLop_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Row.RowState == DataRowState.Deleted)
                return;
            if (e.Column.ColumnName.ToUpper().Equals("MACN") || e.Column.ColumnName.ToUpper().Equals("MANLOP"))
            {
                if (e.Row["MaCN"].ToString() != "" && e.Row["MaNLop"].ToString() != "")
                {
                    string malop = CreateMaLop(e.Row["MaCN"].ToString(), e.Row["MaNLop"].ToString());
                    if (malop != "")
                        e.Row["MaLop"] = malop;
                    e.Row["TenLop"] = CreateTenLop(e.Row["MaNLop"].ToString(), e.Row["MaCN"].ToString());
                    e.Row.EndEdit();
                }
            }
        }

        string CreateMaLop(string MaCN, string MaNhomLop)
        {            
            string dk = MaCN + MaNhomLop;
            //string sql = "select MaLop, subtring(MaLop,len('"+dk+"'),len(MaLop)-len('"+dk+"')) as STT from DMLophoc where MaLop like '"+dk+"%' order by STT DESC ";
            string sql = "select MaLop, cast((substring(MaLop,len('" + dk + "')+1, len(MaLop)-len('" + dk + "'))) as int) as STT " +
                         "from DMLophoc where MaLop like '" + dk + "%' and isnumeric(substring(MaLop,len('" + dk + "')+1, len(MaLop)-len('" + dk + "')))=1 "+
                         "order by STT desc";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count == 0)
                dk = dk + "001";
            else
            {
                string stt = dt.Rows[0]["STT"].ToString();
                //stt = stt.Replace(dk, "");
                if (stt == "")
                {
                    XtraMessageBox.Show("Tạo mã lớp không thành công!", Config.GetValue("PackageName").ToString());
                    return null;
                }
                else
                {
                    int sttLop = int.Parse(stt) + 1;
                    if (sttLop < 10)
                        dk = dk + "00" + sttLop.ToString();
                    else if (sttLop < 100)
                        dk = dk + "0" + sttLop.ToString();
                    else
                        dk = dk + sttLop.ToString();

                }
            }
            if (dk.Length > 14)
            {
                XtraMessageBox.Show("Mã lớp được tạo có hơn 14 ký tự quy định!", Config.GetValue("PackageName").ToString());
                return null;
            }
            else
                return dk;
        }

        int GetSTT(string MaCN, string MaNhomLop)
        {
            string dk = MaCN + MaNhomLop;
            string sql = "select MaLop, cast((substring(MaLop,len('" + dk + "')+1, len(MaLop)-len('" + dk + "'))) as int) as STT " +
                         "from DMLophoc where MaLop like '" + dk + "%' and isnumeric(substring(MaLop,len('" + dk + "')+1, len(MaLop)-len('" + dk + "')))=1 " +
                         "order by STT desc";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count == 0)
                return 1;
            else
            {
                string stt = dt.Rows[0]["STT"].ToString();
                if (stt == "")
                {
                    XtraMessageBox.Show("Lấy số thứ tự lớp không thành công!", Config.GetValue("PackageName").ToString());
                    return 0;
                }
                else
                {
                    int sttLop = int.Parse(stt) + 1;
                    return sttLop;
                }
            }
        }

        string CreateTenLop(string MaNhomLop, string MaCN)
        {
            string TenNLop = "";
            if (MaNhomLop == "" || MaCN == "")
                return TenNLop;
            string sql = "select TenNLop from DMNhomLop where MaNLop = '" + MaNhomLop + "'";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count == 0)
                return TenNLop;
            TenNLop = dt.Rows[0]["TenNLop"].ToString();
            int i = GetSTT(MaCN, MaNhomLop);
            TenNLop += " - " + i.ToString();
            return TenNLop;
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
