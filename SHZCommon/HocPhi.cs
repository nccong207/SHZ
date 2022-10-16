using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTDatabase;
using DevExpress.XtraEditors;

namespace SHZCommon
{
    public class HocPhi
    {
        Database db = Database.NewDataDatabase();
        public decimal TienConLai(DateTime ngayTinh, string mahv)
        {
            //- Công thức:
            //Học phí còn lại = học phí trên buổi * số buổi còn lại + HP dư - HP hoàn

            //- Trong đó:
            //Học phí trên buổi = HP khai báo (trong DM học phí) * (100% - khuyến học) / số buổi khai báo (trong DM lớp học)
            //Số buổi còn lại = Số buổi được học - Số buổi đã học
            //HP dư = Số tiền bảo lưu (trong học viên đăng ký)
            //HP hoàn = Số tiền (trong phiếu chi theo mã nghiệp vụ hoàn học phí)

            //  - Trong đó:
            //Số buổi được học = HP đóng / Học phí trên buổi
            //HP đóng = Số tiền bảo lưu lớp trước + Số tiền thực nộp - Số tiền nợ lớp trước (tất cả trong học viên đăng ký)

            string sql = string.Format(@"SELECT	TOP 1 hv.TenHV, lh.SoBuoi, 
		                            HocPhi = hp.HocPhi * (100 - hv.GiamHP) / 100,
		                            HPDong = hv.BLTruoc - hv.HPNoTruoc + hv.ThucThu,
		                            HPDu = hv.BLSoTien,
                                    hv.ConLai
                            FROM	MTDK hv
		                            INNER JOIN DMLophoc lh ON hv.MaLop = lh.MaLop
		                            LEFT JOIN DMHocPhi hpnl ON hpnl.MaNL = lh.MaNLop
		                            LEFT JOIN HPNL hp ON hp.HPID = hpnl.HPID AND (hp.NgayBD IS NULL OR hp.NgayBD <= hv.NgayDKMoi)
                            WHERE	hv.MaHV ='{0}'
                            ORDER BY hp.NgayBD DESC", mahv);
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count == 0 || dt.Rows[0]["HocPhi"].ToString() == "")
            {
                XtraMessageBox.Show("Không tìm thấy đủ thông tin để tính học phí còn lại");
                return 0;
            }
            DataRow dr = dt.Rows[0];

            //lấy số buổi khai báo
            decimal soBuoiCuaLop = decimal.Parse(dr["SoBuoi"].ToString());

            //lấy học phí khai báo
            decimal hocPhi = decimal.Parse(dr["HocPhi"].ToString());
            //lấy học phí một buổi
            decimal hp1Buoi = hocPhi / soBuoiCuaLop;

            //lấy số buổi được học
            decimal hpDong = decimal.Parse(dr["HPDong"].ToString());
            decimal soBuoiDuocHoc = hpDong > hocPhi ? soBuoiCuaLop : hpDong / hp1Buoi; //cần tính lại chứ ko lấy trong MTDK vì có thể lẻ

            //tính số buổi còn lại
            decimal soBuoiDaHoc = decimal.Parse(db.GetDataTable(string.Format("exec sp_SobuoiHVdahoc '{0}','{1}' ", mahv, ngayTinh)).Rows[0]["sbdahoc"].ToString());
            decimal soBuoiCL = soBuoiDuocHoc - soBuoiDaHoc;

            //tính hp dư - hp hoàn
            decimal hpDu = decimal.Parse(dr["HPDu"].ToString());

            decimal tienBL = hp1Buoi * soBuoiCL + hpDu;
            return RoundNumber(tienBL);
        }

        decimal RoundNumber(decimal num)
        {
            num = num / 1000;
            num = Math.Round(num, 0);
            num *= 1000;
            return num;
        }
    }
}
