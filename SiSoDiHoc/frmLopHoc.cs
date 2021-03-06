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

namespace SiSoDiHoc
{
    public partial class frmLopHoc : DevExpress.XtraEditors.XtraForm
    {
        DataRow drMenuTT;
        public frmLopHoc(DataRow drMenu)
        {
            InitializeComponent();
            drMenuTT = drMenu;
        }
        Database db = Database.NewDataDatabase();
        public DataTable dtHienthi;        
        private void frmLopHoc_Load(object sender, EventArgs e)
        {
            dtHienthi = BindData();
            if (checkAll.Checked)
                dtHienthi.DefaultView.RowFilter = "";
            else
                dtHienthi.DefaultView.RowFilter = " isKT = 0";

            if (drMenuTT.Table.Columns.Contains("ExtraSql"))
            {
                //sỉ số
                if (drMenuTT["ExtraSql"].ToString().ToUpper().Equals("1=1"))
                {
                    GV1.Visible = true;
                    GV2.Visible = true;
                    GV3.Visible = true;
                    DK.Visible = true;
                    DH.Visible = true;
                    col10.Visible = true;
                    col25.Visible = true;
                    colghichu.Visible = true;
                    ChoNghi.Visible = false;
                    NGHI.Visible = true;
                }//cho nghỉ
                else if (drMenuTT["ExtraSql"].ToString().ToUpper().Equals("2=2"))
                {
                    GV1.Visible = false;
                    GV2.Visible = false;
                    GV3.Visible = false;
                    DK.Visible = false;
                    DH.Visible = false;
                    col10.Visible = false;
                    col25.Visible = false;
                    colghichu.Visible = false;
                    NGHI.Visible = false;
                    ChoNghi.Visible = true;
                }                               
            }
            gcLop.DataSource = dtHienthi;
            gvLop.BestFitColumns();
        }      

        DataTable BindData()
        {
            //string sql = "select * "+
            //        "from dmlophoc L where MaCN = '"+Config.GetValue("MaCN").ToString()+"'";
            string sql = @"select MaLop, TenLop, NgayBDKhoa, NgayKTKhoa, Siso, isKT ,Ngay10,Ngay25,GhiChu, 
                        (select  count(MT.MaLop) 
                        from MTDK MT inner join DMLophoc LH on MT.MaLop=LH.MaLop
                        where MT.NgayDK <= '"+DateTime.Today.ToString()+@"' and MT.MaLop = L.MaLop and
                        ((isNghiHoc = '0' and NgayNghi is null) 
                        or (isNghiHoc='1' and NgayNghi > '" + DateTime.Today.ToString() + @"'))
                        and ((isBL='0' and NgayBL is null) 
                        or ( isBL = '1' and NgayBL > '" + DateTime.Today.ToString() + @"')) ) as  SiSoHV
                        from dmlophoc L where MaCN = '" +Config.GetValue("MaCN").ToString()+"'";
            DataTable dt = db.GetDataTable(sql);
            DataColumn col1 = new DataColumn("GV",typeof(string));
            DataColumn col2 = new DataColumn("GV2", typeof(string));
            DataColumn col3 = new DataColumn("GV3", typeof(string));
            dt.Columns.Add(col1);
            dt.Columns.Add(col2);
            dt.Columns.Add(col3);
            foreach (DataRow row in dt.Rows)
            {
                sql = "select NV.* from dmlophoc L inner join GVPhuTrach GV on L.MaLop=GV.MaLop " +
                          "inner join DMNVien NV on NV.MaNV = GV. MaGV " +
                          "where L.MaLop ='" + row["MaLop"].ToString() + "'";
                DataTable dtSub = db.GetDataTable(sql);
                int count = 1 ;
                foreach (DataRow dr in dtSub.Rows)
                {
                    if (count == 1)
                        row["GV"] = dr["HoTen"];
                    else if (count == 2)
                        row["GV2"] = dr["HoTen"];
                    else if (count == 3)
                        row["GV3"] = dr["HoTen"];
                    count++;                        
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtHienthi);
            dv.RowStateFilter = DataViewRowState.ModifiedCurrent;
            string sql = "";
            if (dv.Count > 0)
            {
                if (drMenuTT["ExtraSql"].ToString().ToUpper().Equals("1=1"))
                {
                    //si so
                    foreach (DataRowView drv in dv)
                    {
                        sql = string.Format(@"update DMLopHoc set SiSo = {0},Ngay10 = {1},Ngay25 = {2},GhiChu = '{3}' 
                                               where MaLop = '{4}'",int.Parse(drv["SiSo"].ToString())
                                             , int.Parse(drv["Ngay10"].ToString()), int.Parse(drv["Ngay25"].ToString())
                                             , drv["GhiChu"].ToString(), drv["MaLop"].ToString());
                       
                        db.UpdateByNonQuery(sql);
                    }
                }
                else if (drMenuTT["ExtraSql"].ToString().ToUpper().Equals("2=2"))
                {
                    //cho nghi
                    foreach (DataRowView drv in dv)
                    {
                        sql = "update DMLopHoc set isKT = '" + drv["isKT"].ToString() + "' where MaLop = '" + drv["MaLop"].ToString() + "'";
                        db.UpdateByNonQuery(sql);
                    }
                }
                XtraMessageBox.Show("Cập nhật thành công!",Config.GetValue("PackageName").ToString());
                this.Close();
            }
        }

        private void checkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkAll.Checked)
                dtHienthi.DefaultView.RowFilter = "";
            else
                dtHienthi.DefaultView.RowFilter = " isKT = 0";
        }
    }
}