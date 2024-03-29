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

namespace TinhLuongNV
{
    public partial class FrmThang : DevExpress.XtraEditors.XtraForm
    {
        private Database db = Database.NewDataDatabase();
        private GridView _gvCCGV;
        private DataTable _dtNhanVien;
        public int ThangLuong;

        public FrmThang(GridView gvCCGV)
        {
            InitializeComponent();
            _gvCCGV = gvCCGV;
            //mặc định tháng là kỳ kế toán, nếu chưa có kỳ kế toán, mặc định là tháng hiện tại
            if (Config.GetValue("KyKeToan") != null && Config.GetValue("KyKeToan").ToString() != null)
                seThang.Text = Config.GetValue("KyKeToan").ToString();
            else
                seThang.Value = DateTime.Today.Month;
        }

        //câu sql lấy toàn bộ thông tin cần thiết để lập bảng lương nhân viên
        private DataTable LayDSNhanVien(DateTime dtNgay)
        {
            int nam = int.Parse(Config.GetValue("NamLamViec").ToString());
            int thang = int.Parse(seThang.Text);
            if (thang == 1)
            {
                thang = 12;
                nam -= 1;
            }
            else
                thang -= 1;
            DateTime dtBD = DateTime.Parse(thang.ToString() + "/1/" + nam.ToString());
            DateTime dtKT = dtBD.AddMonths(1).AddDays(-1);
            string sql = @" select sum(isnull(TienThuong,0)) as TongThuong
                            from (
                            SELECT	MaCN, (sum(CASE 
			                            WHEN cast(SSDK*100/SSThuongCS AS decimal(12,2))>=100
			                            THEN ROUND(MucThuongCS*cast(SSDK*100/SSThuongCS AS decimal(12,2))/100,-3)
			                            ELSE 0 END)) AS TienThuong
                            FROM	(
		                            SELECT	l.MaCN, l.MaLop, nl.SSThuongCS, nl.MucThuongCS, sisohv as SSDK				
		                            FROM	DMLopHoc l
				                            INNER JOIN DMNhomLop AS nl ON l.MaNLop = nl.MaNLop
				                            LEFT OUTER JOIN MTDK hv ON hv.MaLop = l.MaLop
		                            WHERE	l.NgayBDKhoa BETWEEN '" + dtBD.ToString() + @"' AND '" + dtKT.ToString() + @"' AND l.IsKT = 0		
		                            GROUP BY l.MaCN, l.MaLop, nl.SSThuongCS, nl.MucThuongCS, sisohv
	                            ) w
                            GROUP BY MaCN )x";
            DataTable dt = db.GetDataTable(sql);
            decimal TongThuong = 0;
            if (dt.Rows.Count > 0 && dt.Rows[0]["TongThuong"].ToString() != "")
                TongThuong = decimal.Parse(dt.Rows[0]["TongThuong"].ToString());

            sql = @"select MaNV, CNTT, LCB, PC1, PC2, PC3, MaCN, ThuongDG, GioTC, TienTC = (LCB/GioCongThang)*GioTC,
                GioVP = case when GioNP > GioCongThang/NgayCongThang then GioNP - GioCongThang/NgayCongThang else 0 end,
                GioTP = case when GioNP > GioCongThang/NgayCongThang then GioCongThang/NgayCongThang else GioNP end, GioPhepNam,
                TruLuong = case when GioNP > GioCongThang/NgayCongThang then round((LCB/GioCongThang)*(GioNP - GioCongThang/NgayCongThang),0) else 0 end,
                GioPNCon = GioPhepNam - (case when GioNP > GioCongThang/NgayCongThang then GioCongThang/NgayCongThang else GioNP end),
                ThuongCS = t.TLThuongCS * cast('"+TongThuong.ToString()+ @"' as decimal(20,6))
                from (select tc.*,dm.MaCN, 
                    ThuongDG = isnull((select ThuongDanhGia from DanhGiaNV dg where tc.MaNV = dg.MaNV and dg.Thang = '" + dtNgay.Month + "' and dg.Nam = '" + dtNgay.Year + @"'),0),
                    GioTC = isnull((select sum(SoGio) from ChamCongNV where PhanLoai = 'TC' and MaNV = tc.MaNV and Month(ngay)= '" + dtNgay.Month + "' and year(ngay)='" + dtNgay.Year + @"'),0),
                    GioNP = isnull((select sum(SoGio) from ChamCongNV where PhanLoai = 'NP' and MaNV = tc.MaNV and Month(ngay)= '" + dtNgay.Month + "' and year(ngay)='" + dtNgay.Year + @"'),0)
                    from DMTCLuongNV tc inner join
                    (select MaNV, max(NgayHieuLuc) as NgayHieuLuc
                    from DMTCLuongNV where NgayHieuLuc <= '" + dtNgay.ToString() + @"' 
                    group by MaNV) t on tc.MaNV = t.MaNV and tc.NgayHieuLuc = t.NgayHieuLuc
                    inner join DMNVien dm on tc.MaNV = dm.MaNV
                    where dm.isNghiViec = 0) t";

            dt = db.GetDataTable(sql);
            return dt;
        }

        //lấy giờ phép năm đã sử dụng
        private DataView LayGNP(string thang, string nam)
        {
            string sql = "select MaNV, sum(GioTP) as TongGio from LuongNV where (Nam = '" + nam + "'and Thang < '" + thang + "') group by MaNV";
            DataTable dt = db.GetDataTable(sql);
            DataView dv = new DataView(dt);
            return dv;
        }

        //danh sách thưởng chiêu sinh cá nhân
        private DataView DSThuongCSCN(int m, int y)
        {
            string sql = @"select x.MaNVTV, nv.Hoten, tc.SLMM, sum(x.SLMoi) + sum(x.SLCu) + sum(x.SLBL) as ThucHien,
                        case when tc.SLMM <> 0 then round(cast((sum(x.SLMoi) + sum(x.SLCu) + sum(x.SLBL))/tc.SLMM*100 as decimal(20,6)),0)
                        else 0 end as ketqua
                        from ( select TV.MaNVTV , LH.MaLop,
                         (select count(*) from mtdk mt1 inner join dmhvtv tv1 on mt1.hvtvid = tv1.hvtvid
                          where mt1.MaLop = LH.MaLop and tv1.MaNVTV = TV.MaNVTV
                         and mt1.nguonhv = 0 and isbl = 0 and (isnghihoc = 0 or (isnghihoc =1 and ischuyenlop = 1))
                         ) as SLMoi,
                         (select count(*) from mtdk mt1 inner join dmtcluongnv tc1 on mt1.macnhoc = tc1.macntinh
                         where mt1.MaLop = LH.MaLop and mt1.nguonhv = 1 and isbl = 0 and tc1.isTinh = 1 and isCL = 0 
                         and (isnghihoc = 0  OR (isnghihoc = 1 and isChuyenLop = 1))
                         and tc1.manv = tv.manvtv  
                         )  as SLCu,
                         (select count(*) from mtdk mt1 inner join dmhvtv tv1 on mt1.hvtvid = tv1.hvtvid 
                          where mt1.MaLop = LH.MaLop and tv1.MaNVTV = TV.MaNVTV
                          and mt1.nguonhv = 2 and isbl = 0 and isnghihoc = 0 
                         ) as SLBL
                        from dmlophoc LH inner join MTDK MT on MT.MaLop = LH.MaLop
                        inner join dmhvtv TV on MT.HVTVID = TV.HVTVID
                        inner join dmnvien NV on NV.MaNV = TV.MaNVTV
                        where month(ngaybdkhoa) = '" + m.ToString() + @"' and year(ngaybdkhoa)= '" + y.ToString() + @"'                        
                        group by TV.MaNVTV, LH.MaLop
                        ) x inner join dmnvien NV on NV.MaNV = x.MaNVTV
                        inner join MucTieuTV tc on tc.ID = nv.ID
                        where tc.Thang ='" + m.ToString() + @"' and tc.Nam ='" + y.ToString() + @"'
                        group by x.MaNVTV, NV.HoTen, tc.SLMM";
            DataTable dt = db.GetDataTable(sql);
            DataView dv = new DataView(dt);
            return dv;
        }

        private decimal TCThuongCSCN(decimal pc)
        {
            string sql = "select * from TCThuongCSCN where Muc <= '" + pc.ToString() + "' order by Muc DESC";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count > 0)
                return decimal.Parse(dt.Rows[0]["SoTien"].ToString());
            else
                return 0;
        }

        // Tổng hợp số lượng chiêu sinh chi nhánh - Bổ sung mới 
        private DataView TongHop ( int m, int y )
        {
            string sql = @"
                            SELECT MaCN [CN],SUM(SLMM) [MucTieu],SUM(ThucHien) [ThucHien],ROUND( SUM(ThucHien)/SUM(SLMM)*100,0) [Datduoc]
                            FROM
                            (
                            SELECT x.MaNV MaNV, nv.Hoten , tc.SLMM, sum(x.SLMoi) + sum(x.SLCu) + sum(x.SLBL) AS [ThucHien],
                            CASE WHEN tc.SLMM <> 0 THEN round((sum(x.SLMoi) + sum(x.SLCu) + sum(x.SLBL))/tc.SLMM*100,0)
                            ELSE 0 END AS [datduoc]
                            FROM (
		                            SELECT  NV.MaNV , LH.MaLop,
				                            (select count(*) from mtdk mt1 inner join dmhvtv tv1 on mt1.hvtvid = tv1.hvtvid
				                             where mt1.MaLop = LH.MaLop and tv1.MaNVTV = NV.MaNV
				                             and mt1.nguonhv = 0 and isbl = 0 and (isnghihoc = 0 or (isnghihoc =1 and ischuyenlop = 1))) 
				                            as [SLMoi],
				                            (select count(*) from mtdk mt1 inner join dmtcluongnv tc1 on mt1.macnhoc = tc1.macntinh
				                             where mt1.MaLop = LH.MaLop and mt1.nguonhv = 1 and isbl = 0 and tc1.isTinh = 1 and isCL = 0 
				                            and (isnghihoc = 0  OR (isnghihoc = 1 and isChuyenLop = 1))
				                             and tc1.manv = NV.MaNV  ) 
				                            as [SLCu],
				                            (select count(*) from mtdk mt1 inner join dmhvtv tv1 on mt1.hvtvid = tv1.hvtvid
				                             where mt1.MaLop = LH.MaLop and tv1.MaNVTV = NV.MaNV
				                             and mt1.nguonhv = 2 and isbl = 0 and isnghihoc = 0 ) 
				                            as [SLBL], null as [Ghi chú]
		                            from dmlophoc LH inner join MTDK MT on MT.MaLop = LH.MaLop, dmnvien NV 
		                            inner join MucTieuTV tc on tc.ID = nv.ID
		                            where tc.Thang = {0} and tc.Nam = {1}
		                            and month(ngaybdkhoa) = {0} and year(ngaybdkhoa)= {1}
		                            group by NV.MaNV, NV.HoTen, LH.MaLop
	                            )x inner join dmnvien NV ON NV.MaNV = x.MaNV
		                            inner join MucTieuTV tc ON tc.ID = nv.ID
		                            WHERE tc.Thang = {0} and tc.Nam = {1}
		                            GROUP BY x.MaNV, NV.HoTen, tc.SLMM
                            )W INNER JOIN DMNVien n ON W.MaNV = n.MaNV
                            GROUP BY MaCN";
            DataTable dt = db.GetDataTable(string.Format(sql,m,y));
            DataView dv = new DataView(dt);
            return dv;
        }

        // Thưởng chi nhánh

        private decimal TCTChiNhanh(decimal kq,string cn)
        {
            string sql = "select * from TCThuongCN where   Muc <= {0} AND ChiNhanh = '{1}' order by Muc DESC";
            DataTable dt = db.GetDataTable(string.Format(sql,kq,cn));
            if (dt.Rows.Count > 0)
                return (decimal)dt.Rows[0]["SoTien"] *(decimal) dt.Rows[0]["MucThuong"]/100;
            else
                return 0;
        }
        private void CapNhatThamNien()
        {
            string nam = Config.GetValue("NamLamViec").ToString();
            string sql = "update DMNVien set ThamNienMoi = ThamNien + 1 + isnull((select count(*) from (select Thang,Nam,MaNV from LuongNV where (Nam < '" + nam + "' or (Nam = '" + nam + "' and Thang <= " + seThang.Text + ")) group by Thang,Nam,MaNV) t where t.MaNV = DMNVien.MaNV),0) WHERE isNV = 1";
            db.UpdateByNonQuery(sql);
        }

        private void TaoBangLuong(int m)
        {
            string nam = Config.GetValue("NamLamViec").ToString();
            _gvCCGV.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
            _gvCCGV.ActiveFilterString = "Thang = '" + m.ToString() + "' and Nam = '" + nam + "'";
            if (_gvCCGV.DataRowCount > 0)
                return;
            CapNhatThamNien();
            int thang = m;
            int namtc = int.Parse(nam);
            if (thang == 1)
            {
                thang = 12;
                namtc--;
            }
            else
                thang--;
            DataView dvNVHuong = DSThuongCSCN(thang, namtc);
            DataView dvCNThuong = TongHop(thang,namtc); 
            DateTime dtBD = DateTime.Parse(m.ToString() + "/1/" + nam);
            DateTime dtKT = dtBD.AddMonths(1).AddDays(-1);
            _dtNhanVien = LayDSNhanVien(dtKT);
            DataView dvGioPhep = LayGNP(m.ToString(), nam);
            decimal GP = 0;
            foreach (DataRow drNV in _dtNhanVien.Rows)
            {
                _gvCCGV.AddNewRow();
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Nam"], nam);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Thang"], seThang.Text);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["LCB"], drNV["LCB"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaCN"], drNV["MaCN"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["CNTT"], drNV["CNTT"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaNV"], drNV["MaNV"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["PC1"], drNV["PC1"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["PC2"], drNV["PC2"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["PC3"], drNV["PC3"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["ThuongDG"], drNV["ThuongDG"]);                
                // Thưởng chiêu sinh cá nhân
                dvNVHuong.RowFilter = "MaNVTV = '" + drNV["MaNV"].ToString() + "'";
                dvCNThuong.RowFilter = "CN ='" + drNV["CNTT"].ToString() + "'";
                if (dvNVHuong.Count > 0)
                {
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["ThuongCN"], TCThuongCSCN(decimal.Parse(dvNVHuong[0]["ketqua"].ToString())));
                  // Thưởng theo chi nhánh
                    if(dvCNThuong.Count > 0)
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["TChiNhanh"], TCTChiNhanh((decimal)dvCNThuong[0]["Datduoc"], dvCNThuong[0]["CN"].ToString()));
                }
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["ThuongCS"], drNV["ThuongCS"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["GioTC"], drNV["GioTC"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["TienTC"], drNV["TienTC"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["GioVP"], drNV["GioVP"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["TruLuong"], drNV["TruLuong"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["GioTP"], drNV["GioTP"]);
                //Giờ phép năm còn = giờ phép năm - tổng giờ đã nghỉ(trước tháng hiện tại) - giờ nghỉ tháng hiện tại
                dvGioPhep.RowFilter = "MaNV = '" + drNV["MaNV"].ToString() + "'";
                if (decimal.Parse(drNV["GioPhepNam"].ToString()) == 0)
                    GP = 0;
                else if (dvGioPhep.Count > 0)
                    GP = decimal.Parse(drNV["GioPhepNam"].ToString()) - (decimal.Parse(dvGioPhep[0]["TongGio"].ToString()) + decimal.Parse(drNV["GioTP"].ToString()));
                else
                    GP = decimal.Parse(drNV["GioPhepNam"].ToString()) - decimal.Parse(drNV["GioTP"].ToString());
                
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["GioPNCon"], GP);
                _gvCCGV.UpdateCurrentRow();
            }
            _gvCCGV.RefreshData();
            _gvCCGV.BestFitColumns();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ThangLuong = Int32.Parse(seThang.Text);
            TaoBangLuong(ThangLuong);
            this.DialogResult = DialogResult.OK;
        }
    }
}