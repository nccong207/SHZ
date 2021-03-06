using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTLib;
using Plugins;
using CDTDatabase;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;

namespace CopyHocVien
{
    public class CopyHocVien : ICReport
    {
        private Database db = Database.NewDataDatabase();
        private DataCustomReport _data;
        private InfoCustomReport _info = new InfoCustomReport(IDataType.Report);
        GridView gvMain;

        #region ICReport Members

        public DataCustomReport Data
        {
            set { _data = value; }
        }

        public void Execute()
        {
            gvMain = (_data.FrmMain.Controls.Find("gridControlReport", true)[0] as GridControl).MainView as GridView;
            SimpleButton btnXL = _data.FrmMain.Controls.Find("btnXuLy", true)[0] as SimpleButton;
            btnXL.Click += new EventHandler(btnXL_Click);
        }

        public InfoCustomReport Info
        {
            get { return _info; }
        }

        #endregion

        void btnXL_Click(object sender, EventArgs e)
        {
            DataView dv = gvMain.DataSource as DataView;
            dv.RowFilter = "Chọn = 1";
            if (dv.Count == 0)
            {
                dv.RowFilter = "";
                XtraMessageBox.Show("Vui lòng chọn học viên", Config.GetValue("PackageName").ToString());
                return;
            }
            string sql = @" INSERT INTO mtdk(TenHV,MaHV,NgayDK,MaLop,ConLai,MaCNDK,MaCNHoc,BLSoTien,NgayBL,NgayHH,NguonHV,MaHVDK
                            ,HTMua,TienHP,ThucThu,GiamHP,SoBuoiCL,IsBL,IsNghiHoc,isChuyenLop,NgayNghi,SoLuong,TTienGT,MaNhomLop,NgayHT
                            ,PhanHoiID,HVCL,LyDoBL,BLTruoc,HPNoTruoc,isCL,HVTVID,isDKL,PhongHoc,GhiChu,MaCT,MaNV,MaNVBL,SoBuoiDH
                            ,NgayHocCuoi,HVID,SoHD) 
                            Values(@TenHV,@MaHV,@NgayDK,@MaLop,@ConLai,@MaCNDK,@MaCNHoc,@BLSoTien,@NgayBL,@NgayHH,@NguonHV,@MaHVDK
                            ,@HTMua,@TienHP,@ThucThu,@GiamHP,@SoBuoiCL,@IsBL,@IsNghiHoc,@isChuyenLop,@NgayNghi,@SoLuong,@TTienGT,@MaNhomLop,@NgayHT
                            ,@PhanHoiID,@HVCL,@LyDoBL,@BLTruoc,@HPNoTruoc,@isCL,@HVTVID,@isDKL,@PhongHoc,@GhiChu,@MaCT,@MaNV,@MaNVBL,@SoBuoiDH
                            ,@NgayHocCuoi,@HVID,@SoHD) ;
                          ";
             string[] paraName = new string[] { "@TenHV","@MaHV","@NgayDK","@MaLop","@ConLai","@MaCNDK","@MaCNHoc","@BLSoTien","@NgayBL","@NgayHH","@NguonHV","@MaHVDK"
                            ,"@HTMua","@TienHP","@ThucThu","@GiamHP","@SoBuoiCL","@IsBL","@IsNghiHoc","@isChuyenLop","@NgayNghi","@SoLuong","@TTienGT","@MaNhomLop","@NgayHT"
                            ,"@PhanHoiID","@HVCL","@LyDoBL","@BLTruoc","@HPNoTruoc","@isCL","@HVTVID","@isDKL","@PhongHoc","@GhiChu","@MaCT","@MaNV","@MaNVBL","@SoBuoiDH"
                            ,"@NgayHocCuoi","@HVID","@SoHD"};

             string sqlLopHoc = @"INSERT INTO [DMLophoc]([MaCN],[MaLop],[Siso],[NgayBDKhoa],[NgayKTKhoa],[MaGioHoc]
                                    ,[BDNghi],[KTNghi],[TenLop],[MaNLop],[isKT],[SoBuoi],[TTPhuongAn],[TTTGGQ],[TTYKBGH]
                                    ,[TTKQ],[TTGhiChu],[SiSoHV],[PhongHoc],[Ngay10],[Ngay25],[GhiChu])
                                SELECT N'{0}',N'{1}',[SiSo],[NgayBDKhoa],[NgayKTKhoa],[MaGioHoc],[BDNghi],[KTNghi]
                                      ,[TenLop],[MaNLop],[isKT],[SoBuoi],[TTPhuongAn],[TTTGGQ],[TTYKBGH],[TTKQ]
                                      ,[TTGhiChu],[SiSoHV],[PhongHoc],[Ngay10],[Ngay25],[GhiChu]
                                FROM [DMLophoc] where MaLop = '{2}'";

             string sqlGV = @"  INSERT INTO [GVPhuTrach]([MaLop],[MaGV],[Tyle],[NgayBD],[NgayKT])
                                SELECT '{0}',[MaGV],[Tyle],[NgayBD],[NgayKT]
                                FROM [GVPhuTrach] where MaLop = '{1}'";

             string sqlKH = @"  insert into DMKH(MaKH,TenKH,DiaChi,Mobile,IsKH,Nhom1,InActive,HanMucDS,MTID) 
                                select N'{0}',tenkh,diachi,mobile,iskh,nhom1,inactive,hanmucds,'{1}' from dmkh
                                where makh = '{2}'";

             string sqlNhomKH = @"insert into DMNhomKH(MaNhomKH,TenNhom)
                                  select N'{0}',TenNhom from DMNhomKH where MaNhomKH = '{1}'";

            foreach (DataRowView drv in dv)
            {
                db.EndMultiTrans();
                string maHocVien="";
                string maLopMoi = drv.Row["MaLop"].ToString().Replace(drv.Row["MaCNHoc"].ToString(),Config.GetValue("MaCN").ToString());
                //KIỂM TRA LỚP CÓ CHƯA, CHƯA CÓ TẠO LỚP MỚI
                object obj = db.GetValue("select count(*) from dmlophoc where malop = '"+ maLopMoi +"'");
                if (Convert.ToInt32(obj) == 0)
                {
                    //copy lớp học
                    db.UpdateByNonQuery(string.Format(sqlLopHoc, Config.GetValue("MaCN").ToString(), maLopMoi, drv.Row["MaLop"]));
                    //Copy giáo viên phụ trách
                    db.UpdateByNonQuery(string.Format(sqlGV, maLopMoi, drv.Row["MaLop"]));
                    //Tạo mã học viên
                    maHocVien = maLopMoi + "01";
                }
                else
                {
                    //db.EndMultiTrans();
                    //Tạo mã học viên
                    string sql1 = "select Top 1 Replace(MaHV,MaLop,'') from mtdk where MaLop = '" + maLopMoi + "' order by MaHV Desc";
                    int siso = Convert.ToInt32(db.GetValue(sql1)) + 1;
                    maHocVien = maLopMoi + siso.ToString("00");
                }
                //Copy học viên
                string hvid = Guid.NewGuid().ToString();
                object[] paraValue = new object[] {drv.Row["TenHV"],maHocVien,drv.Row["NgayDK"],maLopMoi,drv.Row["ConLai"],Config.GetValue("MaCN").ToString(),Config.GetValue("MaCN").ToString()
                            ,drv.Row["BLSoTien"],drv.Row["NgayBL"],drv.Row["NgayHH"],drv.Row["NguonHV"],drv.Row["MaHVDK"],drv.Row["HTMua"],drv.Row["TienHP"],drv.Row["ThucThu"]
                            ,drv.Row["GiamHP"],drv.Row["SoBuoiCL"],drv.Row["IsBL"],drv.Row["IsNghiHoc"],drv.Row["isChuyenLop"],drv.Row["NgayNghi"],drv.Row["SoLuong"],drv.Row["TTienGT"]
                            ,drv.Row["MaNhomLop"],drv.Row["NgayHT"],drv.Row["PhanHoiID"],drv.Row["HVCL"],drv.Row["LyDoBL"],drv.Row["BLTruoc"],drv.Row["HPNoTruoc"],drv.Row["isCL"]
                            ,drv.Row["HVTVID"],drv.Row["isDKL"],drv.Row["PhongHoc"],drv.Row["GhiChu"],drv.Row["MaCT"],drv.Row["MaNV"],drv.Row["MaNVBL"],drv.Row["SoBuoiDH"]
                            ,drv.Row["NgayHocCuoi"],hvid,drv.Row["SoHD"]};
                db.UpdateDatabyPara(sql, paraName, paraValue);
                //Copy danh mục khách hàng
                db.UpdateByNonQuery(string.Format(sqlKH, maHocVien, hvid, drv.Row["MaHV"]));
                //Copy danh mục nhóm khách hàng
                if (Convert.ToInt32(db.GetValue("select count(*) from DMNhomKH where manhomkh = '" + maLopMoi + "'")) == 0)
                {
                    db.UpdateByNonQuery(string.Format(sqlNhomKH, maLopMoi, drv.Row["MaLop"]));
                }   
            }
            //Xóa học viên đã copy
            DataTable dtChon = dv.ToTable();
            dv.RowFilter = "";
            dv.Sort = "MaHV";
            foreach (DataRow dr in dtChon.Rows)
            {
                dv.Delete(dv.Find(dr["MaHV"].ToString()));
            }
        }
    }
}
