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
using DevExpress.XtraGrid;

namespace TinhLuongGVCN
{
    public partial class FrmThang : DevExpress.XtraEditors.XtraForm
    {
        private Database db = Database.NewDataDatabase();
        private GridView _gvCCGV;
        private GridControl _gcCCGV;
        private DataTable _dtGiaoVien;
        private DataTable _dtTCThuongLL;             

        public FrmThang(GridControl gCCCGV)
        {
            InitializeComponent();
            _gcCCGV = gCCCGV;
            _gvCCGV = gCCCGV.MainView as GridView;
            //mặc định tháng là kỳ kế toán, nếu chưa có kỳ kế toán, mặc định là tháng hiện tại
            if (Config.GetValue("KyKeToan") != null && Config.GetValue("KyKeToan").ToString() != null)
                seThang.Text = Config.GetValue("KyKeToan").ToString();
            else
                seThang.Value = DateTime.Today.Month;
        }

        //lấy danh sách giáo viên đang dạy của chi nhánh hiện tại, tính toán trước một số thông tin (lương cơ bản, tiết thực tế, tiết nghỉ)
        private DataTable LayDSGiaoVien()
        {
            string maCN = Config.GetValue("MaCN").ToString();
            string nam = Config.GetValue("NamLamViec").ToString();
            DateTime deCT = new DateTime(Int32.Parse(nam), Convert.ToInt32(seThang.Value), 1);
            deCT = deCT.AddMonths(1).AddDays(-1);
            string sql = "select lh.*,gv.MaGV,gv.NgayBD,nlu.MaNL,nlu.SoTietChuan,nv.PhuCapGV as PCCN, nv.CNPC ,nv.ThamNienMoi,LCB = nlu.LCBTiet*nlu.SoTietChuan," +
                "SoTietTT = isnull((select count(*) from ChamCongGV cc where cc.MaLop = lh.MaLop and cc.GVID = nv.ID and cc.Thang = " + seThang.Text + " and cc.Nam = " + nam + "),0) * gh.SoTiet," +
                "SoTietNghi = isnull((select count(*) from ChamCongGV cc where cc.MaLop = lh.MaLop and cc.GVID = nv.ID and month(cc.Ngay) = " + seThang.Text + " and cc.Nam = " + nam + " and cc.TinhTrang = 0),0) * gh.SoTiet, " +
                "SoTietNghi2 = isnull((select count(*) from ChamCongGV cc where cc.MaLop = lh.MaLop and cc.GVID = nv.ID and month(cc.Ngay) = " + seThang.Text + " and cc.Nam = " + nam + " and cc.GVDayThay is not null),0) * gh.SoTiet, " +
                "0.0 as SLCK, 0.0 as SSLL, nl.SSToiThieu " + // Thêm cột sỉ số cuối khóa và cột sỉ số lên lớp
                "from DMLopHoc lh inner join GVPhuTrach gv on lh.MaLop = gv.MaLop inner join DMNVien nv on gv.MaGV = nv.MaNV " +
                "inner join DMNhomLop nl on lh.MaNLop = nl.MaNLop inner join DMNhomLuong nlu on nl.NhomLuong = nlu.MaNL inner join DMBoPhan cn on lh.MaCN = cn.MaBP inner join DMNgaygiohoc gh on lh.MaGioHoc = gh.MaGioHoc " +
                "where ((gv.NgayBD <= '{0}') or (gv.NgayBD >= '{0}')) " +
                "and ((lh.NgayKTKhoa >= '{0}') or (lh.NgayKTKhoa <= '{0}')) and lh.MaCN = '{1}'";

            DataTable dt = db.GetDataTable(string.Format(sql, deCT, maCN));
            return dt;
        }

        //luôn cập nhật thâm niêm mới trước khi tính lương
        private void CapNhatThamNien()
        {
            string sql = string.Format("exec sp_TinhThamNienMoi");
            db.UpdateByNonQuery(sql);
        }

        //hàm tính lương thâm niên theo thâm niên
        private decimal LuongThamNien(decimal ThamNien, DataTable dtTCThamNien)
        {
            foreach (DataRowView dr in dtTCThamNien.DefaultView)
                if (dr["SoThang"].ToString() != "" && ThamNien >= Decimal.Parse(dr["SoThang"].ToString()))
                    return Decimal.Parse(dr["TienThuong"].ToString());
            return 0;
        }

        //hàm tính thưởng lên lớp theo tỷ lệ cung cấp
        private decimal ThuongLL(decimal tyle)
        {
            foreach (DataRowView dr in _dtTCThuongLL.DefaultView)
                if (dr["TyLe"].ToString() != "" && tyle >= Decimal.Parse(dr["TyLe"].ToString()))
                    return Decimal.Parse(dr["TienThuong"].ToString());
            return 0;
        }

        //hàm tính thưởng sỉ số theo sỉ số và nhóm lớp cung cấp
        private decimal ThuongSS(int ss, string maNLop)
        {
            DataTable dtTCThuongSS = db.GetDataTable("select * from DMTCThuongSS where MaNLop = '" + maNLop + "'");
            dtTCThuongSS.DefaultView.Sort = "SiSo desc";
            foreach (DataRowView dr in dtTCThuongSS.DefaultView)
                if (dr["SiSo"].ToString() != "" && ss >= Decimal.Parse(dr["SiSo"].ToString()))
                    return Decimal.Parse(dr["TienThuong"].ToString());
            return 0;
            
        }

        //hàm tính tỷ lệ lên lớp theo mã lớp cũ
        private decimal TyLeLenLop(string maLop)
        {
            //string sql = "select count(*) from MTDK dk1 inner join MTDK dk2 on dk1.HVTVID = dk2.HVTVID " +
            //    "where dk1.MaLop = '" + maLop + "' and dk1.isBL = 0 and dk1.isNghiHoc = 0 " +
            //    "and dk2.MaLop <> '" + maLop + "' and dk2.NgayDK > dk1.NgayDK and dk1.mahv = dk2.mahvdk";

            string sql = @"select count(*) from MTDK dk1 inner join MTDK dk2 on dk1.MaHV = dk2.MaHVDK 
                           where dk1.Malop ='" + maLop + @"' and dk1.isBL = 0 and dk1.isNghiHoc = 0 ";
            object o = db.GetValue(sql);
            decimal ssll = o == null || o.ToString() == "" ? 0 : Decimal.Parse(o.ToString());
            if (ssll == 0)
                return 0;
            sql = "select count(*) from MTDK where MaLop = '" + maLop + "' and isBL = 0 and isNghiHoc = 0";
            o = db.GetValue(sql);
            decimal ssck = o == null || o.ToString() == "" ? 0 : Decimal.Parse(o.ToString());
            if (ssck == 0)
                return 0;    
                   
            return Math.Round(ssll / ssck,2);
        }

        //hàm tính sỉ số lên lớp - Bổ sung mới
        private decimal SiSoLL(string maLop)
        {
            //string sql = "select count(*) from MTDK dk1 inner join MTDK dk2 on dk1.HVTVID = dk2.HVTVID " +
            //   "where dk1.MaLop = '" + maLop + "' and dk1.isBL = 0 and dk1.isNghiHoc = 0 " +
            //   "and dk2.MaLop <> '" + maLop + "' and dk2.NgayDK > dk1.NgayDK and dk1.mahv = dk2.mahvdk";
            string sql = @"select count(*) from MTDK dk1 inner join MTDK dk2 on dk1.MaHV = dk2.MaHVDK 
                           where dk1.Malop ='" + maLop + @"' and dk1.isBL = 0 and dk1.isNghiHoc = 0 ";
            object o = db.GetValue(sql);
            decimal ssll = o == null || o.ToString() == "" ? 0 : Decimal.Parse(o.ToString());
            if (ssll == 0)
                return 0;
            else
                return ssll;
        }

        //hàm tính sỉ số cuối khóa - Bổ sung mới
        private decimal SiSoCK(string maLop)
        {
            string sql = "select count(*) from MTDK where MaLop = '" + maLop + "' and isBL = 0 and isNghiHoc = 0";
            object o = db.GetValue(sql);
            decimal ssck = o == null || o.ToString() == "" ? 0 : Decimal.Parse(o.ToString());
            if (ssck == 0)
                return 0;
            else
                return ssck;
        }

        //hàm tính sỉ số hiện tại của lớp
        private int SSHienTai(string maLop, DateTime NgayKT)
        {
//            string sql = @"select count(*) 
//                        from MTDK where MaLop = '" + maLop + @"' 
//                        and (isBL = 0 or (isBL = 1 and month(NgayBL) < " + seThang.Text + @")) 
//                        and (isNghiHoc = 0 or (isNghiHoc = 1 and month(NgayNghi) < " + seThang.Text + "))";

            string sql = @" select count(MT.MaLop) 
                        from MTDK MT inner join DMLophoc L on MT.MaLop=L.MaLop                        
                        where MT.NgayDK <= '" + NgayKT.ToString() + @"' and 
                        ((isNghiHoc = '0' and NgayNghi is null) or (isNghiHoc='1' and NgayNghi > '" + NgayKT.ToString() + @"'))
                         and ((isBL='0' and NgayBL is null) or ( isBL = '1' and NgayBL > '" + NgayKT.ToString() + @"')) 
                        and MT.MaLop = '" + maLop + "'";                     
            object o = db.GetValue(sql);
            int ssht = o == null || o.ToString() == "" ? 0 : int.Parse(o.ToString());
            return ssht;
        }

        //hàm lấy danh sách giáo viên dạy thay của lớp
        private DataTable LayGVDayThay(string maGV, string maLop)
        {
            string sql = "select gvdt.MaNV as MaGV,gvdt.PhuCapGV as PCCN, gvdt.CNPC, cc.MaLop, count(*)*gh.SoTiet as SoTietDT  "+
                "from DMNVien nv inner join ChamCongGV cc on nv.ID = cc.GVID " +
                "inner join DMNVien gvdt on cc.GVDayThay = gvdt.ID inner join DMLopHoc lh on cc.MaLop = lh.MaLop inner join DMNgayGioHoc gh on lh.MaGioHoc = gh.MaGioHoc " +
                "where nv.MaNV = '" + maGV + "' and cc.MaLop = '" + maLop + "' and cc.GVDayThay is not null and month(cc.Ngay) = " + seThang.Text + //and cc.TinhTrang = 0 "  // chỉnh sửa cho việc gv đó nghỉ mà ko trừ lương - vẫn check ở cột tình trạng
                "group by gvdt.MaNV, cc.MaLop, gh.SoTiet, gvdt.PhuCapGV, gvdt.CNPC";
            return db.GetDataTable(sql);
        }

        // hàm lấy mã lớp cũ  - Bổ sung mới 
        private object MaLopCu(string malop)
        {
            string sql = string.Format(@"SELECT DISTINCT dk1.MaLop 
                                        FROM MTDK dk1 inner join MTDK dk2 on dk1.MaHV = dk2.MaHVDK 
                                        WHERE dk2.MaLop = '{0}'", malop);
            object o = db.GetValue(sql);

            return o;

        }

        //hàm tạo bảng lương theo tháng m
        private void TaoBangLuong(int m)
        {
            string maCN = Config.GetValue("MaCN").ToString();
            string nam = Config.GetValue("NamLamViec").ToString();
            _dtTCThuongLL = db.GetDataTable("select * from DMTCThuongLL");  //tạo sẵn bảng thưởng lên lớp để xử lý khi người dùng nhập thông tin vào bảng lương
            _dtTCThuongLL.DefaultView.Sort = "TyLe desc";
            //khai báo sự kiện tính thưởng lên lớp
            _gvCCGV.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(_gvCCGV_CellValueChanged);
            _gvCCGV.OptionsView.NewItemRowPosition = NewItemRowPosition.None;       //ẩn dòng thêm mới
            _gvCCGV.ActiveFilterString = "Thang = " + m.ToString() + " and Nam = " + Config.GetValue("NamLamViec").ToString() + " and MaCN = '" + maCN + "'";  //kiểm tra đã tính lương chưa
            if (_gvCCGV.DataRowCount > 0)
                return;
            CapNhatThamNien();  //cập nhật lại thâm niên trước khi tính lương
            DateTime dtBD = DateTime.Parse(m.ToString() + "/1/" + Config.GetValue("NamLamViec").ToString());
            DateTime dtKT = dtBD.AddMonths(1).AddDays(-1);
            _dtGiaoVien = LayDSGiaoVien();      //lấy danh sách các giáo viên và các lớp dạy trong tháng của chi nhánh hiện tại
            DataTable dtTCTN = db.GetDataTable("select * from DMTCThamNien");   //tạo bảng tiêu chí thâm niên để tính thưởng thâm niên
            dtTCTN.DefaultView.Sort = "SoThang desc";
            foreach (DataRow drGV in _dtGiaoVien.Rows)  //mỗi giáo viên và một lớp dạy tạo thành một dòng trong bảng lương
            {
                if (drGV["SoTietTT"] == DBNull.Value || Decimal.Parse(drGV["SoTietTT"].ToString()) == 0)
                    continue;
                _gvCCGV.AddNewRow();
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Nam"], nam);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Thang"], seThang.Text);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaLop"], drGV["MaLop"]);
                // tự nhảy mã lớp cũ _ bổ sung mới
                //_gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaLopCu"], MaLopCu(drGV["MaLop"].ToString()));
                //_gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["LopGQ"], false);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaCN"], drGV["MaCN"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaGV"], drGV["MaGV"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaNhomLuong"], drGV["MaNL"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SoTietChuan"], drGV["SoTietChuan"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SoTietTT"], drGV["SoTietTT"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["LCB"], drGV["LCB"]);
                if (drGV["CNPC"].ToString() != "" && drGV["MaLop"].ToString() != "" && drGV["MaLop"].ToString().Contains(drGV["CNPC"].ToString()))
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["PCCN"], drGV["PCCN"]);
                else
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["PCCN"], 0);
                decimal ltn = LuongThamNien(Decimal.Parse(drGV["ThamNienMoi"].ToString()), dtTCTN);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["LuongTN"], ltn);
                int ss = SSHienTai(drGV["MaLop"].ToString(), dtKT);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SSLop"], ss);
                bool isCheck = false;
                if (ss < (int)decimal.Parse(drGV["SSToiThieu"].ToString()))
                    isCheck = true;
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["LopGQ"], isCheck);             
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["ThuongSS"], ThuongSS(ss, drGV["MaNLop"].ToString()));
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SoTietNghi"], drGV["SoTietNghi"]);
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["GVChinh"], drGV["MaGV"]);
                _gvCCGV.UpdateCurrentRow();
                if (drGV["SoTietNghi"].ToString() != "" && Int32.Parse(drGV["SoTietNghi"].ToString()) > 0)  //nếu có tiết nghỉ, sẽ cập nhật luôn thông tin dạy thay
                {
                    DataTable dtDT = LayGVDayThay(drGV["MaGV"].ToString(), drGV["MaLop"].ToString());       //lấy danh sách các giáo viên dạy thay cho lớp
                    foreach (DataRow drGVDT in dtDT.Rows)
                    {
                        _gvCCGV.AddNewRow();
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Nam"], nam);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Thang"], seThang.Text);             //được hưởng theo toàn bộ chế độ lương của giáo viên chính
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaLop"], drGV["MaLop"]);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["LopGQ"], isCheck);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaCN"], drGV["MaCN"]);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaGV"], drGVDT["MaGV"]);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaNhomLuong"], drGV["MaNL"]);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SoTietChuan"], drGV["SoTietChuan"]);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SoTietTT"], 0);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["LCB"], drGV["LCB"]);
                        if (drGV["CNPC"].ToString() != "" && drGV["MaLop"].ToString() != "" && drGV["MaLop"].ToString().Contains(drGV["CNPC"].ToString()))
                            _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["PCCN"], drGV["PCCN"]);
                        else
                            _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["PCCN"], 0);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["LuongTN"], ltn);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SSLop"], ss);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["ThuongSS"], ThuongSS(ss, drGV["MaNLop"].ToString()));
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SoTietDT"], drGVDT["SoTietDT"]);    //chỉ có số tiết dạy thay là lấy từ bảng dạy thay                        
                        //Thêm cột mới để biết ai là gv dạy chính, dạy thay 
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["isDayThay"], 1);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["GVChinh"], drGV["MaGV"]);
                    }
                }
                else if (drGV["SoTietNghi2"].ToString() != "" && Int32.Parse(drGV["SoTietNghi2"].ToString()) > 0)
                {
                    DataTable dtDT = LayGVDayThay(drGV["MaGV"].ToString(), drGV["MaLop"].ToString());       //lấy danh sách các giáo viên dạy thay cho lớp
                    foreach (DataRow drGVDT in dtDT.Rows)
                    {
                        _gvCCGV.AddNewRow();
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Nam"], nam);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Thang"], seThang.Text);             //được hưởng theo toàn bộ chế độ lương của giáo viên chính
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaLop"], drGV["MaLop"]);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["LopGQ"], isCheck);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaCN"], drGV["MaCN"]);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaGV"], drGVDT["MaGV"]);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaNhomLuong"], drGV["MaNL"]);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SoTietChuan"], drGV["SoTietChuan"]);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SoTietTT"], 0);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["LCB"], drGV["LCB"]);
                        if (drGV["CNPC"].ToString() != "" && drGV["MaLop"].ToString() != "" && drGV["MaLop"].ToString().Contains(drGV["CNPC"].ToString()))
                            _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["PCCN"], drGV["PCCN"]);
                        else
                            _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["PCCN"], 0);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["LuongTN"], ltn);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SSLop"], ss);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["ThuongSS"], ThuongSS(ss, drGV["MaNLop"].ToString()));
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SoTietDT"], drGVDT["SoTietDT"]);    //chỉ có số tiết dạy thay là lấy từ bảng dạy thay                        
                        //Thêm cột mới để biết ai là gv dạy chính, dạy thay 
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["isDayThay"], 1);
                        _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["GVChinh"], drGV["MaGV"]);
                    }
                }
            }
            _gvCCGV.BestFitColumns();
        }


  

        //sự kiện dùng để xử lý thưởng lên lớp (dựa vào mã lớp cũ, lớp giải quyết và tỷ lệ lên lớp)
        void _gvCCGV_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "MaLopCu")
            {
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["TLLenLop"], TyLeLenLop(e.Value.ToString()));
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SLCuoiKhoa"], SiSoCK(e.Value.ToString()));
                _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["SLLenLop"], SiSoLL(e.Value.ToString()));
            }
            if (e.Column.FieldName == "LopGQ")
            {
                if (!Boolean.Parse(e.Value.ToString()) && _gvCCGV.GetFocusedRowCellValue("TLLenLop") != DBNull.Value)
                {
                    decimal tlll = Decimal.Parse(_gvCCGV.GetFocusedRowCellValue("TLLenLop").ToString());
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["ThuongLenLop"], ThuongLL(tlll));                   
                }
                else
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["ThuongLenLop"], 0);
                DataRow row = _gvCCGV.GetDataRow(_gvCCGV.FocusedRowHandle);
                SetData(row);
            }
            if (e.Column.FieldName == "TLLenLop" && e.Value != DBNull.Value)
            {
                if (!Boolean.Parse(_gvCCGV.GetFocusedRowCellValue("LopGQ").ToString()))
                {
                    decimal tlll = Decimal.Parse(e.Value.ToString());
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["ThuongLenLop"], ThuongLL(tlll));                    
                }
                else
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["ThuongLenLop"], 0);
                DataRow row = _gvCCGV.GetDataRow(_gvCCGV.FocusedRowHandle);
                SetData(row);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            TaoBangLuong(Int32.Parse(seThang.Text));
            this.Close();
        }

        private void SetData( DataRow row)
        {            
            for (int i = 0; i < _gvCCGV.DataRowCount; i++)
            {
                DataRow dr = _gvCCGV.GetDataRow(i);
                if (dr["MaLop"].ToString() != row["MaLop"].ToString())
                    continue;
                if (decimal.Parse(dr["ThuongLenLop"].ToString()) != decimal.Parse(row["ThuongLenLop"].ToString())
                        && Boolean.Parse(dr["isDayThay"].ToString()))
                {
                    if (dr["GVChinh"].ToString() == row["MaGV"].ToString())
                        dr["ThuongLenLop"] = row["ThuongLenLop"];
                }
            }
        }
    }
}