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

namespace LayDSPhatLuong
{
    public class LayDSPhatLuong : ICControl
    {
        #region ICControl Members
        private InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        private DataCustomFormControl data;
        private GridControl gcMain;
        Database db = Database.NewDataDatabase();


        public void AddEvent()
        {
            gcMain = data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl;
            gcMain.Enter += new EventHandler(gcMain_Enter);
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

        //chức năng phát lương được xử lý khi grid control main nhận focus
        void gcMain_Enter(object sender, EventArgs e)
        {
            if (data.BsMain.Current == null)
                return;
            DataRowView drv = data.BsMain.Current as DataRowView;
            if (drv["Thang"] == DBNull.Value || drv["NhomLuong"] == DBNull.Value || drv["MaCN"] == DBNull.Value)
            {
                XtraMessageBox.Show("Vui lòng nhập nhóm lương/tháng/chi nhánh để chọn danh sách phát lương!");
                return;
            }
            GridView gvMain = (sender as GridControl).MainView as GridView;
            if (drv.Row.RowState == DataRowState.Added && gvMain.DataRowCount == 0) //chỉ xử lý khi thêm mới và khi grid control chưa có dữ liệu
            {
                string nhom = drv["NhomLuong"].ToString();      //xử lý dựa vào nhóm lương (Giáo viên/Công ty/Nhân viên)
                BindingSource bs = LayDuLieu(nhom, drv["Thang"].ToString(), drv["MaCN"].ToString());    //lấy danh sách tại đây
                FrmDanhSach frm = new FrmDanhSach(nhom, bs);    //màn hình hiển thị danh sách lương để người dùng chọn
                frm.ShowDialog();
                DataTable dt = nhom == "NV" ? bs.DataSource as DataTable : (bs.DataSource as DataSet).Tables[0];
                NapDuLieu(nhom, dt);                            //nạp dữ liệu tại đây
            }
        }

        //hàm đưa danh sách phát lương đã chọn vào grid main
        private void NapDuLieu(string nhom, DataTable dtData)
        {
            DataView dv = new DataView(dtData);
            dv.RowFilter = "Chon = True";
            if (dv.Count > 0 && nhom == "NV" && data.BsMain.Current != null)
            {
                DataRow drMaster = (data.BsMain.Current as DataRowView).Row;
                drMaster["MaCNTT"] = dv[0]["CNTT"].ToString();
            }
            GridView gv = gcMain.MainView as GridView;
            foreach (DataRowView drv in dv)
            {
                gv.AddNewRow();
                gv.SetFocusedRowCellValue(gv.Columns["MaNV"], drv["MaNV"]);
                gv.SetFocusedRowCellValue(gv.Columns["MaCN"], drv["MaCN"]);
                gv.SetFocusedRowCellValue(gv.Columns["TongLuong"], drv["TongLuong"]);
                gv.UpdateCurrentRow();
            }
        } 

        //hàm lấy danh sách phát lương theo nhóm lương
        private BindingSource LayDuLieu(string nhom, string m, string maCN)
        {
            string nam = Config.GetValue("NamLamViec").ToString();
            BindingSource bs = new BindingSource();
            DataColumn dc = new DataColumn("Chon", typeof(Boolean));
            dc.DefaultValue = false;
            switch (nhom)
            {
                case "GV":
                    string sql = "select * from LuongGVCN where Nam = " + nam + " and Thang = " + m + " and MaCN = '" + maCN + "' and MaGV not in " +
                        "(select dt.MaNV from DTPhatLuong dt inner join MTPhatLuong mt on mt.MTPLID = dt.MTPLID " +
                        "where mt.Nam = " + nam + " and mt.Thang = " + m + " and NhomLuong = '" + nhom + "' and mt.MaCN = '" + maCN + "')";
                    DataTable dtGV = db.GetDataTable(sql);
                    sql = "select l.MaGV as MaNV, nv.Hoten, sum(TongLuong) as Luong, 0 as PhuCap, l.MaCN " +
                        "from LuongGVCN l inner join DMNVien nv on l.MaGV = nv.MaNV where Nam = " + nam + " and Thang = " + m + " and l.MaCN = '" + maCN + "' and MaGV not in " +
                        "(select dt.MaNV from DTPhatLuong dt inner join MTPhatLuong mt on mt.MTPLID = dt.MTPLID " +
                        "where mt.Nam = " + nam + " and mt.Thang = " + m + " and NhomLuong = '" + nhom + "' and mt.MaCN = '" + maCN + "') group by l.MaGV,nv.Hoten, l.MaCN ";
                    DataTable mtGV = db.GetDataTable(sql);
                    DataColumn dcTL = new DataColumn("TongLuong", typeof(Decimal));
                    dcTL.Expression = "Luong + PhuCap";
                    mtGV.Columns.Add(dcTL);
                    mtGV.Columns.Add(dc);
                    mtGV.PrimaryKey = new DataColumn[] { mtGV.Columns["MaNV"] };
                    DataSet dsGV = new DataSet();
                    dsGV.Tables.Add(mtGV);
                    dsGV.Tables.Add(dtGV);
                    DataColumn pk = dsGV.Tables[0].Columns["MaNV"];
                    DataColumn fk = dsGV.Tables[1].Columns["MaGV"];
                    DataRelation dr = new DataRelation("Chi tiết lương", pk, fk, true);
                    dsGV.Relations.Add(dr);
                    bs.DataSource = dsGV;
                    bs.DataMember = dsGV.Tables[0].TableName;
                    break;
                case "CT":
                    //Tho bo sung loc nhan vien theo chi nhanh
                    
                    //sql = "select * from LuongGVCT where Nam = " + nam + " and Thang = " + m + " and MaGV not in " +
                    //    "(select dt.MaNV from DTPhatLuong dt inner join MTPhatLuong mt on mt.MTPLID = dt.MTPLID " +
                    //    "where mt.Nam = " + nam + " and mt.Thang = " + m + " and NhomLuong = '" + nhom + "')";
                    sql = "select * from LuongGVCT L inner join DMNVien NV on NV.MaNV=L.MaGV " +
                       "inner join GVCN CN on CN.ID=NV.ID " +
                       "where Nam = " + nam + " and Thang = " + m + " and MaGV not in " +
                       "(select dt.MaNV from DTPhatLuong dt inner join MTPhatLuong mt on mt.MTPLID = dt.MTPLID " +
                       "where mt.Nam = " + nam + " and mt.Thang = " + m + " and NhomLuong = '" + nhom + "' )";
                    // giáo viên không có trong mtCT nhưng có trong dtCT --> lỗi relations
                    // Mạnh bổ sung : không lấy những trường hợp giáo viên không có trong tổng hợp lương  do TongLuong <= 0 trong tháng
                    sql += "AND nv.MaLuong IN ( SELECT Maluong FROM LuongTH WHERE Nam = "+ nam + " and thang = " + m + "  ) ";

                    DataTable dtCT = db.GetDataTable(sql);
                    sql = @"select DISTINCT l.MaCN, l.MaGV as MaNV, nv.Hoten, sum(l.TongLuong) as TongLuong, 
                        PhuCap = case when th.MaLuong not in (select MaLuong from DMNVien where isNV = 1 or isGV = 1) then isnull(th.PhuCap,0) else 0 end, 
                        GiamTru = case when th.MaLuong not in (select MaLuong from DMNVien where isNV = 1 or isGV = 1) then isnull(th.GiamTru,0) else 0 end " +
                        "from LuongGVCT l inner join DMNVien nv on l.MaGV = nv.MaNV " +
                        "INNER JOIN LuongTH AS th ON nv.MaLuong = th.MaLuong " +
                        "where l.Nam = " + nam + " and l.Thang = " + m + " " +
                        "and th.Thang = " + m + " and th.Nam = " + nam + " " +
                        "and MaGV not in (select dt.MaNV from DTPhatLuong dt inner join MTPhatLuong mt on mt.MTPLID = dt.MTPLID " +
                        "where mt.Nam = " + nam + " and mt.Thang = " + m + " and NhomLuong = '" + nhom + "') and l.MaCN is not null " +
                          "group by l.MaCN, l.MaGV,nv.Hoten, th.MaLuong, th.PhuCap, th.GiamTru" +
                          " having sum(l.TongLuong) > 0";

                    DataTable mtCT = db.GetDataTable(sql);
                    mtCT.Columns.Add(dc);
                    //mtCT.PrimaryKey = new DataColumn[] { mtCT.Columns["MaNV"] };
                    DataSet dsCT = new DataSet();
                    dsCT.Tables.Add(mtCT);
                    dsCT.Tables.Add(dtCT);
                    //DataColumn pk1 = dsCT.Tables[0].Columns["MaNV"];
                    //DataColumn fk1 = dsCT.Tables[1].Columns["MaGV"];
                    //DataRelation dr1 = new DataRelation("Chi tiết lương", pk1, fk1, true);
                    //dsCT.Relations.Add(dr1);
                    bs.DataSource = dsCT;
                    bs.DataMember = dsCT.Tables[0].TableName;
                    break;
                case "NV":
                    sql = "select nv.Hoten,l.* from LuongNV l inner join DMNVien nv on l.MaNV = nv.MaNV "+
                        "where Nam = " + nam + " and Thang = " + m + " and l.MaCN = '" + maCN + "' and l.MaNV not in " +
                        "(select dt.MaNV from DTPhatLuong dt inner join MTPhatLuong mt on mt.MTPLID = dt.MTPLID " +
                        "where mt.Nam = " + nam + " and mt.Thang = " + m + " and NhomLuong = '" + nhom + "' and mt.MaCN = '" + maCN + "')";
                    DataTable mtNV = db.GetDataTable(sql);
                    mtNV.Columns.Add(dc);
                    bs.DataSource = mtNV;
                    break;
            }
            return bs;
        }
    }
}
