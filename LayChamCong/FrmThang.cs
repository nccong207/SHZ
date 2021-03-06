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

namespace LayChamCong
{
    public partial class FrmThang : DevExpress.XtraEditors.XtraForm
    {
        private Database db = Database.NewDataDatabase();
        private GridView _gvCCGV;

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

        //lấy danh sách tất cả các lớp chưa kết thúc
        private DataTable LayDSLop()
        {
            string sql = @"select gv.*, lh.MaGioHoc, NgayKTKhoa = isnull(gv.NgayKT,lh.NgayKTKhoa), lh.BDNghi, lh.KTNghi, nv.ID as GVID from 
                          GVPhuTrach gv inner join DMLopHoc lh on gv.MaLop = lh.MaLop inner join DMNVien nv on gv.MaGV = nv.MaNV 
                          where lh.isKT = 0 and lh.MaCN ='" + Config.GetValue("MaCN").ToString() + "'";            
            DataTable dt = db.GetDataTable(sql);
            return dt;
        }

        private DataTable LayLop(string malop, DateTime ngayBD)
        {
            string sql = @"select gv.*, lh.MaGioHoc, NgayKTKhoa = isnull(gv.NgayKT,lh.NgayKTKhoa), lh.BDNghi, lh.KTNghi, nv.ID as GVID from 
                          GVPhuTrach gv inner join DMLopHoc lh on gv.MaLop = lh.MaLop inner join DMNVien nv on gv.MaGV = nv.MaNV 
                          where lh.isKT = 0 and lh.MaCN ='" + Config.GetValue("MaCN").ToString() + @"' and lh.MaLop = '" + malop + @"'
                          and (NgayKT is null or (NgayKT is not null and NgayKT > '" + ngayBD.ToString() + @"'))
                          order by gv.NgayBD asc ";
            DataTable dt = db.GetDataTable(sql);
            return dt;
        }
        // Lấy thứ theo chỉnh sửa mới SHZ
        private DataTable GetValue(string magio)
        {
            string sql = string.Format("SELECT Value FROM CTGioHoc WHERE MaGioHoc = '{0}'", magio);
            DataTable dt = db.GetDataTable(sql);
            return dt;

        }

        #region Không dùng

        //hàm tạo lịch trong tháng m để chấm công giáo viên
        private void TaoLich(int m)
        {
            string namlv = Config.GetValue("NamLamViec").ToString();
            _gvCCGV.OptionsView.NewItemRowPosition = NewItemRowPosition.None;   //ẩn dòng thêm mới
            _gvCCGV.ActiveFilterString = "Thang = " + m.ToString() + " and Nam = " + namlv;     //lọc xem đã có sẵn số liệu chưa
            if (_gvCCGV.DataRowCount > 0)   //nếu có rồi sẽ không tạo lịch nữa, chỉ cho xem và điều chỉnh
            {
                _gvCCGV.CollapseAllGroups();
                return;
            }
            DataTable dtLop = LayDSLop();
            DateTime dtBD = DateTime.Parse(m.ToString() + "/1/" + namlv);
            DateTime dtKT = dtBD.AddMonths(1).AddDays(-1);
            foreach (DataRow drLop in dtLop.Rows)   //duyệt qua từng lớp trong danh sách để tạo lịch dạy cho từng lớp
            {
                DateTime dtBDDay = DateTime.Parse(drLop["NgayBD"].ToString());
                DateTime dtKTLop = DateTime.Parse(drLop["NgayKTKhoa"].ToString());
                if (dtKT < dtBDDay || dtKTLop < dtBD)
                    continue;               
                DateTime dtBDTinh = dtBDDay > dtBD ? dtBDDay : dtBD;    //kiểm tra giới hạn thời gian để tạo lịch
                DateTime dtKTTinh = dtKTLop < dtKT ? dtKTLop : dtKT;
                List<DateTime> lich = LayLich(dtBDTinh, dtKTTinh, drLop["MaGioHoc"].ToString().Substring(0, 1),
                    drLop["Tyle"].ToString(), drLop["BDNghi"].ToString(), drLop["KTNghi"].ToString());
                foreach (DateTime dtNgay in lich)   //mỗi buổi học trong lịch sẽ tạo thành một dòng chấm công
                {
                    _gvCCGV.AddNewRow();
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["GVID"], drLop["GVID"]);
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaLop"], drLop["MaLop"]);
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaGio"], drLop["MaGioHoc"]);
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Ngay"], dtNgay);
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Thang"], m);
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Nam"], namlv);
                    _gvCCGV.UpdateCurrentRow();
                }
            }
            _gvCCGV.BestFitColumns();
            _gvCCGV.CollapseAllGroups();            
        }
        private List<DateTime> LayLich(DateTime ngayBD, DateTime ngayKT, string magio, string tyle, string ngayBDNghi, string NgayKTNghi)
        {
            List<DateTime> lich = new List<DateTime>();
            List<DayOfWeek> thu = LayThu(tyle, magio, ngayBD);
            Boolean Chan = false;
            Boolean Le = false;
            DataTable dtVa = GetValue(magio);
            // Chỉnh sửa mới SHZ
            foreach (DataRow row in dtVa.Rows)
            {
                if (row["Value"].ToString() == "2" || row["Value"].ToString() == "4" || row["Value"].ToString() == "6")
                    Chan = true;
                else if (row["Value"].ToString() == "3" || row["Value"].ToString() == "5" || row["Value"].ToString() == "7")
                    Le = true;
            }
            if (ngayBDNghi != "" && NgayKTNghi != "")
            {
                DateTime ngayBDN = DateTime.Parse(ngayBDNghi);
                DateTime ngayKTN = DateTime.Parse(NgayKTNghi);
                for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                {
                    if (Chan == true) // 2,4,6
                    {
                        if (dtp < ngayBDN || dtp > ngayKTN) //nếu trong ngày nghỉ thì không tính
                        {
                            if (thu.Contains(dtp.DayOfWeek))
                                lich.Add(dtp);
                        }
                    }
                    else if (Le == true) //3,5,7
                    {
                        if (dtp < ngayBDN || dtp > ngayKTN)
                        {
                            if (thu.Contains(dtp.DayOfWeek))
                                lich.Add(dtp);
                        }
                    }
                    else
                    {
                        if (dtp < ngayBDN || dtp > ngayKTN)
                        {
                            if (thu.Contains(dtp.DayOfWeek))
                                lich.Add(dtp);
                        }
                    }
                }
            }
            else
            {
                for (DateTime dtp = ngayBD; dtp < ngayKT; dtp = dtp.AddDays(1))
                {
                    if (Chan == true) // 2,4,6
                    {
                        if (thu.Contains(dtp.DayOfWeek))
                            lich.Add(dtp);
                    }
                    else if (Le == true) //3,5,7
                    {
                        if (thu.Contains(dtp.DayOfWeek))
                            lich.Add(dtp);
                    }
                    else
                    {
                        if (thu.Contains(dtp.DayOfWeek))
                            lich.Add(dtp);
                    }
                }
            }
            return lich;
        }

        //hàm lấy những thứ trong tuần giáo viên sẽ dạy dựa vào tỷ lệ dạy của giáo viên
        private List<DayOfWeek> LayThu(string tyle, string magio, DateTime ngayBD)
        {
            List<DayOfWeek> tmp = new List<DayOfWeek>();
            string s = tyle == "" ? "" : tyle.Substring(0, 1);
            Boolean Chan = false;
            Boolean Le = false;
            Boolean CN = false;
            DataTable dtVa = GetValue(magio);
            // Chỉnh sửa mới SHZ
            foreach (DataRow row in dtVa.Rows)
            {
                if (row["Value"].ToString() == "2" || row["Value"].ToString() == "4" || row["Value"].ToString() == "6")
                    Chan = true;
                else if (row["Value"].ToString() == "3" || row["Value"].ToString() == "5" || row["Value"].ToString() == "7")
                    Le = true;
                else if (row["Value"].ToString() == "1" || row["Value"].ToString() == "7")
                    CN = true;
            }
            switch (s)
            {
                case "":
                    tmp.Add(ngayBD.DayOfWeek);
                    if (Chan == true || Le  == true)
                    {
                        tmp.Add(ngayBD.AddDays(2).DayOfWeek);
                        tmp.Add(ngayBD.AddDays(4).DayOfWeek);
                    }
                    else
                        tmp.Add(ngayBD.AddDays(1).DayOfWeek);
                    break;
                case "1":
                    tmp.Add(ngayBD.DayOfWeek);
                    break;
                case "2":
                    tmp.Add(ngayBD.DayOfWeek);
                    tmp.Add(ngayBD.AddDays(2).DayOfWeek);
                    break;
            }
            return tmp;
        }

       //private List<DayOfWeek> LayThu(string tyle, string magio, DateTime ngayBD)
        //{
        //    List<DayOfWeek> tmp = new List<DayOfWeek>();
        //    string s = tyle == "" ? "" : tyle.Substring(0, 1);
        //    switch (s)
        //    {
        //        case "":
        //            #region tỷ lể rỗng
        //            for (DateTime dtp = ngayBD; dtp < ngayBD.AddDays(7); dtp = dtp.AddDays(1))
        //            {
        //                if (magio == "L") //2,4,6
        //                {
        //                    if (dtp.DayOfWeek.ToString() == DayOfWeek.Monday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(2).DayOfWeek);
        //                        tmp.Add(dtp.AddDays(4).DayOfWeek);
        //                        break;
        //                    }
        //                    else if (dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(-2).DayOfWeek);
        //                        tmp.Add(dtp.AddDays(2).DayOfWeek);
        //                        break;
        //                    }
        //                    else if (dtp.DayOfWeek.ToString() == DayOfWeek.Friday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(-2).DayOfWeek);
        //                        tmp.Add(dtp.AddDays(-4).DayOfWeek);
        //                        break;
        //                    }                            
        //                }
        //                else if (magio == "C") //3,5,7
        //                {
        //                    if (dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(2).DayOfWeek);
        //                        tmp.Add(dtp.AddDays(4).DayOfWeek);
        //                        break;
        //                    }
        //                    else if (dtp.DayOfWeek.ToString() == DayOfWeek.Thursday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(-2).DayOfWeek);
        //                        tmp.Add(dtp.AddDays(2).DayOfWeek);
        //                        break;
        //                    }
        //                    else if (dtp.DayOfWeek.ToString() == DayOfWeek.Saturday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(-2).DayOfWeek);
        //                        tmp.Add(dtp.AddDays(-4).DayOfWeek);
        //                        break;
        //                    }
        //                    break;
        //                }
        //                else //7, cn
        //                {
        //                    if (dtp.DayOfWeek.ToString() == DayOfWeek.Saturday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(1).DayOfWeek);
        //                        break;
        //                    }
        //                    else if (dtp.DayOfWeek.ToString() == DayOfWeek.Sunday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(-1).DayOfWeek);
        //                        break;
        //                    }                            
        //                }
        //            }
        //            #endregion
        //            break;
        //        case "1":
        //            for (DateTime dtp = ngayBD; dtp < ngayBD.AddDays(7); dtp = dtp.AddDays(1))
        //            {
        //                if (magio == "L") //2,4,6
        //                {
        //                    if (dtp.DayOfWeek.ToString() == DayOfWeek.Monday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(2).DayOfWeek);
        //                        tmp.Add(dtp.AddDays(4).DayOfWeek);
        //                        break;
        //                    }
        //                    else if (dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(-2).DayOfWeek);
        //                        tmp.Add(dtp.AddDays(2).DayOfWeek);
        //                        break;
        //                    }
        //                    else if (dtp.DayOfWeek.ToString() == DayOfWeek.Friday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(-2).DayOfWeek);
        //                        tmp.Add(dtp.AddDays(-4).DayOfWeek);
        //                        break;
        //                    }
        //                }
        //                else if (magio == "C") //3,5,7
        //                {
        //                    if (dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(2).DayOfWeek);
        //                        tmp.Add(dtp.AddDays(4).DayOfWeek);
        //                        break;
        //                    }
        //                    else if (dtp.DayOfWeek.ToString() == DayOfWeek.Thursday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(-2).DayOfWeek);
        //                        tmp.Add(dtp.AddDays(2).DayOfWeek);
        //                        break;
        //                    }
        //                    else if (dtp.DayOfWeek.ToString() == DayOfWeek.Saturday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(-2).DayOfWeek);
        //                        tmp.Add(dtp.AddDays(-4).DayOfWeek);
        //                        break;
        //                    }
        //                    break;
        //                }
        //                else //7, cn
        //                {
        //                    if (dtp.DayOfWeek.ToString() == DayOfWeek.Saturday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(1).DayOfWeek);
        //                        break;
        //                    }
        //                    else if (dtp.DayOfWeek.ToString() == DayOfWeek.Sunday)
        //                    {
        //                        tmp.Add(dtp.DayOfWeek.ToString());
        //                        tmp.Add(dtp.AddDays(-1).DayOfWeek);
        //                        break;
        //                    }
        //                }
        //            }
        //            break;
        //        case "2":
        //            tmp.Add(ngayBD.DayOfWeek);
        //            tmp.Add(ngayBD.AddDays(2).DayOfWeek);
        //            break;
        //    }
        //    return tmp;
     //}

        //lập lịch dạy cho một lớp

        #endregion 

        private void TaoLichMoi(int m)
        {
            string namlv = Config.GetValue("NamLamViec").ToString();
            _gvCCGV.OptionsView.NewItemRowPosition = NewItemRowPosition.None;   //ẩn dòng thêm mới
            _gvCCGV.ActiveFilterString = "Thang = " + m.ToString() + " and Nam = " + namlv +" and MaLop like '"+Config.GetValue("MaCN").ToString()+"%'";     //lọc xem đã có sẵn số liệu chưa
            if (_gvCCGV.DataRowCount > 0)   //nếu có rồi sẽ không tạo lịch nữa, chỉ cho xem và điều chỉnh
            {
                _gvCCGV.CollapseAllGroups();
                return;
            }
            DataTable dtLop = LayDSLop();
            DataView dvLop = new DataView(dtLop);
            DateTime dtBD = DateTime.Parse(m.ToString() + "/1/" + namlv);
            DateTime dtKT = dtBD.AddMonths(1).AddDays(-1);
            foreach (DataRow drLop in dtLop.Rows)   //duyệt qua từng lớp trong danh sách để tạo lịch dạy cho từng lớp
            {                
                DateTime dtBDDay = DateTime.Parse(drLop["NgayBD"].ToString());
                DateTime dtKTLop = DateTime.Parse(drLop["NgayKTKhoa"].ToString());
                if (dtKT < dtBDDay || dtKTLop < dtBD)
                    continue;
                DateTime dtBDTinh = dtBDDay;  //kiểm tra giới hạn thời gian để tạo lịch
                DateTime dtKTTinh = dtKTLop < dtKT ? dtKTLop : dtKT;               
                string tyle = drLop["TyLe"].ToString().Trim();
                string snd = tyle == "" ? "1" : tyle.Substring(tyle.Length - 1, 1);
                DataTable dtNgayDay = LayNgay(dtBDTinh, dtKTTinh, drLop["MaGioHoc"].ToString(), tyle,
                    snd, drLop["BDNghi"].ToString(), drLop["KTNghi"].ToString(), dvLop, drLop["MaLop"].ToString());
                DataView dvNgayDay = new DataView(dtNgayDay);
                dvNgayDay.RowFilter = "NgayDay >= '" + dtBD.ToString() + "'";
                foreach (DataRowView drvNgay in dvNgayDay)   //mỗi buổi học trong lịch sẽ tạo thành một dòng chấm công
                {
                    _gvCCGV.AddNewRow();
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["GVID"], drLop["GVID"]);
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaLop"], drLop["MaLop"]);
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["MaGio"], drLop["MaGioHoc"]);
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Ngay"], drvNgay["NgayDay"]);
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Thang"], m);
                    _gvCCGV.SetFocusedRowCellValue(_gvCCGV.Columns["Nam"], namlv);
                    _gvCCGV.UpdateCurrentRow();
                }                 
            }
            _gvCCGV.BestFitColumns();
            _gvCCGV.CollapseAllGroups();
            _gvCCGV.RefreshData();
        }

        private DataTable LayNgay(DateTime ngayBD, DateTime ngayKT, string magio, string tyle, string snd, string ngayBDNghi, string NgayKTNghi, DataView dvLop, string malop)
        {
            DataTable dtLich = new DataTable(); // Danh sach cac ngay day cua lop 
            DataColumn colNgay = new DataColumn("NgayDay", typeof(DateTime));
            dtLich.Columns.Add(colNgay);
            // Lấy thứ
            DataTable dtThu = new DataTable(); // Danh sach cac ngay day cua lop 
            DataColumn colThu = new DataColumn("DayOfWeek", typeof(string));
            dtThu.Columns.Add(colThu);

            Boolean Chan = false;
            Boolean Le = false;    
            Boolean CN = false;    
            DataTable dtVa = GetValue(magio);
            // Chỉnh sửa mới SHZ
            foreach (DataRow row in dtVa.Rows)
            {
                if (row["Value"].ToString() == "2")
                {
                    DataRow dr = dtThu.NewRow();
                    dr["DayOfWeek"] = "Monday";
                    dtThu.Rows.Add(dr);
                }
                if (row["Value"].ToString() == "3")
                {
                    DataRow dr = dtThu.NewRow();
                    dr["DayOfWeek"] = "Tuesday";
                    dtThu.Rows.Add(dr);
                }
                if (row["Value"].ToString() == "4")
                {
                    DataRow dr = dtThu.NewRow();
                    dr["DayOfWeek"] = "Wednesday";
                    dtThu.Rows.Add(dr);
                }
                if (row["Value"].ToString() == "5")
                {
                    DataRow dr = dtThu.NewRow();
                    dr["DayOfWeek"] = "Thursday";
                    dtThu.Rows.Add(dr);
                }
                if (row["Value"].ToString() == "6")
                {
                    DataRow dr = dtThu.NewRow();
                    dr["DayOfWeek"] = "Friday";
                    dtThu.Rows.Add(dr);
                }
                if (row["Value"].ToString() == "7")
                {
                    DataRow dr = dtThu.NewRow();
                    dr["DayOfWeek"] = "Saturday";
                    dtThu.Rows.Add(dr);
                }
                if (row["Value"].ToString() == "1")
                {
                    DataRow dr = dtThu.NewRow();
                    dr["DayOfWeek"] = "Sunday";
                    dtThu.Rows.Add(dr);
                }
                //if (row["Value"].ToString() == "2" || row["Value"].ToString() == "4" || row["Value"].ToString() == "6")
                //    Chan = true;
                //else if (row["Value"].ToString() == "3" || row["Value"].ToString() == "5" || row["Value"].ToString() == "7")
                //    Le = true;
                //else if (row["Value"].ToString() == "1" || row["Value"].ToString() == "7")
                //    CN = true;
            }
            if (ngayBDNghi != "" && NgayKTNghi != "")
            {
                DateTime ngayBDN = DateTime.Parse(ngayBDNghi);
                DateTime ngayKTN = DateTime.Parse(NgayKTNghi);
                if (snd == "1")
                {
                    #region 1 người dạy và học 1 hoặc 2 hoặc 3 buổi/tuần
                    for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                    {
                        if (dtp < ngayBDN || dtp > ngayKTN)
                        {
                            foreach (DataRow row in dtThu.Rows)
                            {
                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString())
                                {
                                    DataRow dr = dtLich.NewRow();
                                    dr["NgayDay"] = dtp;
                                    dtLich.Rows.Add(dr);
                                }
                            }
                        }
                        #region Code Old
                        //if (Chan == true) // 2,4,6
                        //{
                        //    if (dtp < ngayBDN || dtp > ngayKTN) //nếu trong ngày nghỉ thì không tính
                        //    {
                        //        if (dtp.DayOfWeek.ToString() == DayOfWeek.Monday || dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday || dtp.DayOfWeek.ToString() == DayOfWeek.Friday)
                        //        {
                        //            DataRow dr = dtLich.NewRow();
                        //            dr["NgayDay"] = dtp;
                        //            dtLich.Rows.Add(dr);
                        //        }
                        //    }
                        //}
                        //else if (Le == true) //3,5,7
                        //{
                        //    if (dtp < ngayBDN || dtp > ngayKTN)
                        //    {
                        //        if (dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday || dtp.DayOfWeek.ToString() == DayOfWeek.Thursday || dtp.DayOfWeek.ToString() == DayOfWeek.Saturday)
                        //        {
                        //            DataRow dr = dtLich.NewRow();
                        //            dr["NgayDay"] = dtp;
                        //            dtLich.Rows.Add(dr);
                        //        }
                        //    }
                        //}
                        //else if (CN == true)//7,CN
                        //{
                        //    if (dtp < ngayBDN || dtp > ngayKTN)
                        //    {
                        //        if (dtp.DayOfWeek.ToString() == DayOfWeek.Saturday || dtp.DayOfWeek.ToString() == DayOfWeek.Sunday)
                        //        {
                        //            DataRow dr = dtLich.NewRow();
                        //            dr["NgayDay"] = dtp;
                        //            dtLich.Rows.Add(dr);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    if (dtp < ngayBDN || dtp > ngayKTN)
                        //    {
                        //        if (dtp.DayOfWeek.ToString() == DayOfWeek.Sunday)
                        //        {
                        //            DataRow dr = dtLich.NewRow();
                        //            dr["NgayDay"] = dtp;
                        //            dtLich.Rows.Add(dr);
                        //        }
                        //    }
                        //}
                        #endregion 
                    }
                    #endregion
                }
                else if (snd == "2") // t7, cn
                {
                    #region 2 người dạy và học 2 buổi/tuần
                    if (CN == true)
                    {
                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(7))
                        {
                            if (dtp < ngayBDN || dtp > ngayKTN)
                            {
                                foreach (DataRow row in dtThu.Rows)
                                {
                                    if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString())
                                    {
                                        DataRow dr = dtLich.NewRow();
                                        dr["NgayDay"] = dtp;
                                        dtLich.Rows.Add(dr);
                                    }
                                }
                            }
                            //if (dtp < ngayBDN || dtp > ngayKTN)
                            //{
                            //    if (dtp.DayOfWeek.ToString() == DayOfWeek.Saturday || dtp.DayOfWeek.ToString() == DayOfWeek.Sunday)
                            //    {
                            //        DataRow dr = dtLich.NewRow();
                            //        dr["NgayDay"] = dtp;
                            //        dtLich.Rows.Add(dr);
                            //    }
                            //}
                        }
                    }
                    #endregion
                }
                else if (snd == "3")
                {
                    //kiem tra xem lop do hoc may buoi?
                    dvLop.RowFilter = "MaLop = '" + malop + "'";
                    bool flag = false;//false la 3 buoi, true la 2 buoi
                    foreach (DataRowView drv in dvLop)
                    {
                        if (drv["TyLe"].ToString().Equals("2/3"))
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (!flag)
                    {
                        #region 3 người dạy và học 3 buổi/tuần ==> tỷ lệ đều là 1/3
                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(7))
                        {
                            if (dtp < ngayBDN || dtp > ngayKTN) //nếu trong ngày nghỉ thì không tính
                            {
                                foreach (DataRow row in dtThu.Rows)
                                {
                                    if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString())
                                    {
                                        DataRow dr = dtLich.NewRow();
                                        dr["NgayDay"] = dtp;
                                        dtLich.Rows.Add(dr);
                                    }
                                }
                            }
                            //if (Chan == true) // 2,4,6
                            //{
                            //    if (dtp < ngayBDN || dtp > ngayKTN) //nếu trong ngày nghỉ thì không tính
                            //    {
                            //        if (dtp.DayOfWeek.ToString() == DayOfWeek.Monday || dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday || dtp.DayOfWeek.ToString() == DayOfWeek.Friday)
                            //        {
                            //            DataRow dr = dtLich.NewRow();
                            //            dr["NgayDay"] = dtp;
                            //            dtLich.Rows.Add(dr);
                            //        }
                            //    }
                            //}
                            //else if (Le == true) //3,5,7
                            //{
                            //    if (dtp < ngayBDN || dtp > ngayKTN)
                            //    {
                            //        if (dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday || dtp.DayOfWeek.ToString() == DayOfWeek.Thursday || dtp.DayOfWeek.ToString() == DayOfWeek.Saturday)
                            //        {
                            //            DataRow dr = dtLich.NewRow();
                            //            dr["NgayDay"] = dtp;
                            //            dtLich.Rows.Add(dr);
                            //        }
                            //    }
                            //}
                        }
                        #endregion
                    }
                    else
                    {
                        #region 2 người dạy và học 3 buổi/tuần ==> tỷ lệ 1/3 và 2/3
                        if (tyle == "1/3")
                        {
                            for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(7))
                            {    
                                    foreach (DataRow row in dtThu.Rows)
                                    {
                                        if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString())
                                        {
                                            if (dtp < ngayBDN || dtp > ngayKTN)
                                            {
                                                DataRow dr = dtLich.NewRow();
                                                dr["NgayDay"] = dtp;
                                                dtLich.Rows.Add(dr);
                                            }
                                        }
                                   }
                                //if (dtp < ngayBDN || dtp > ngayKTN)
                                //{
                                //    DataRow dr = dtLich.NewRow();
                                //    dr["NgayDay"] = dtp;
                                //    dtLich.Rows.Add(dr);
                                //}
                            }
                        }
                        else if (tyle == "2/3")
                        {
                            #region oldcode
                            //if (magio == "L")
                            //{
                            //    #region
                            //    if (ngayBD.DayOfWeek == DayOfWeek.Monday)
                            //    {
                            //        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                            //        {
                            //            if (dtp < ngayBDN || dtp > ngayKTN)
                            //            {
                            //                if (dtp.DayOfWeek.ToString() == DayOfWeek.Monday || dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday)
                            //                {
                            //                    DataRow dr = dtLich.NewRow();
                            //                    dr["NgayDay"] = dtp;
                            //                    dtLich.Rows.Add(dr);
                            //                }
                            //            }
                            //        }
                            //    }
                            //    else if (ngayBD.DayOfWeek == DayOfWeek.Wednesday)
                            //    {
                            //        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                            //        {
                            //            if (dtp < ngayBDN || dtp > ngayKTN)
                            //            {
                            //                if (dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday || dtp.DayOfWeek.ToString() == DayOfWeek.Friday)
                            //                {
                            //                    DataRow dr = dtLich.NewRow();
                            //                    dr["NgayDay"] = dtp;
                            //                    dtLich.Rows.Add(dr);
                            //                }
                            //            }
                            //        }
                            //    }
                            //    else if (ngayBD.DayOfWeek == DayOfWeek.Friday)
                            //    {
                            //        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                            //        {
                            //            if (dtp < ngayBDN || dtp > ngayKTN)
                            //            {
                            //                if (dtp.DayOfWeek.ToString() == DayOfWeek.Friday || dtp.DayOfWeek.ToString() == DayOfWeek.Monday)
                            //                {
                            //                    DataRow dr = dtLich.NewRow();
                            //                    dr["NgayDay"] = dtp;
                            //                    dtLich.Rows.Add(dr);
                            //                }
                            //            }
                            //        }
                            //    }
                            //    #endregion
                            //}
                            //else if (magio == "C")
                            //{
                            //    #region
                            //    if (ngayBD.DayOfWeek == DayOfWeek.Tuesday)
                            //    {
                            //        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                            //        {
                            //            if (dtp < ngayBDN || dtp > ngayKTN)
                            //            {
                            //                if (dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday || dtp.DayOfWeek.ToString() == DayOfWeek.Thursday)
                            //                {
                            //                    DataRow dr = dtLich.NewRow();
                            //                    dr["NgayDay"] = dtp;
                            //                    dtLich.Rows.Add(dr);
                            //                }
                            //            }
                            //        }
                            //    }
                            //    else if (ngayBD.DayOfWeek == DayOfWeek.Thursday)
                            //    {
                            //        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                            //        {
                            //            if (dtp < ngayBDN || dtp > ngayKTN)
                            //            {
                            //                if (dtp.DayOfWeek.ToString() == DayOfWeek.Thursday || dtp.DayOfWeek.ToString() == DayOfWeek.Saturday)
                            //                {
                            //                    DataRow dr = dtLich.NewRow();
                            //                    dr["NgayDay"] = dtp;
                            //                    dtLich.Rows.Add(dr);
                            //                }
                            //            }
                            //        }
                            //    }
                            //    else if (ngayBD.DayOfWeek == DayOfWeek.Saturday)
                            //    {
                            //        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                            //        {
                            //            if (dtp < ngayBDN || dtp > ngayKTN)
                            //            {
                            //                if (dtp.DayOfWeek.ToString() == DayOfWeek.Saturday || dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday)
                            //                {
                            //                    DataRow dr = dtLich.NewRow();
                            //                    dr["NgayDay"] = dtp;
                            //                    dtLich.Rows.Add(dr);
                            //                }
                            //            }
                            //        }
                            //    }
                            //    #endregion
                            //}

                            #endregion

                            // Added 2012-07-16 cho trường hợp ngày bd khóa học đặc biệt
                            bool flg = GetGV(malop, ngayBD, magio);
                            if (!flg) // Trường hợp ngày bắt đầu khóa bình thường
                            {
                                if (Chan == true)
                                {
                                    #region
                                    if (ngayBD.DayOfWeek == DayOfWeek.Monday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Monday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Wednesday")
                                                {
                                                    if (dtp < ngayBDN || dtp > ngayKTN)
                                                    {
                                                        DataRow dr = dtLich.NewRow();
                                                        dr["NgayDay"] = dtp;
                                                        dtLich.Rows.Add(dr);
                                                    }
                                                }
                                            }

                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Monday || dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday)
                                            //{
                                            //    if (dtp < ngayBDN || dtp > ngayKTN)
                                            //    {
                                            //        DataRow dr = dtLich.NewRow();
                                            //        dr["NgayDay"] = dtp;
                                            //        dtLich.Rows.Add(dr);
                                            //    }
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Wednesday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Friday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Wednesday")
                                                {
                                                    if (dtp < ngayBDN || dtp > ngayKTN)
                                                    {
                                                        DataRow dr = dtLich.NewRow();
                                                        dr["NgayDay"] = dtp;
                                                        dtLich.Rows.Add(dr);
                                                    }
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday || dtp.DayOfWeek.ToString() == DayOfWeek.Friday)
                                            //{
                                            //    if (dtp < ngayBDN || dtp > ngayKTN)
                                            //    {
                                            //        DataRow dr = dtLich.NewRow();
                                            //        dr["NgayDay"] = dtp;
                                            //        dtLich.Rows.Add(dr);
                                            //    }
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Friday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Friday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Monday")
                                                {
                                                    if (dtp < ngayBDN || dtp > ngayKTN)
                                                    {
                                                        DataRow dr = dtLich.NewRow();
                                                        dr["NgayDay"] = dtp;
                                                        dtLich.Rows.Add(dr);
                                                    }
                                                }
                                            }

                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Friday || dtp.DayOfWeek.ToString() == DayOfWeek.Monday)
                                            //{
                                            //    if (dtp < ngayBDN || dtp > ngayKTN)
                                            //    {
                                            //        DataRow dr = dtLich.NewRow();
                                            //        dr["NgayDay"] = dtp;
                                            //        dtLich.Rows.Add(dr);
                                            //    }
                                            //}
                                        }
                                    }
                                    #endregion
                                }
                                else if (Le == true)
                                {
                                    #region
                                    if (ngayBD.DayOfWeek == DayOfWeek.Tuesday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Tuesday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Thursday")
                                                {
                                                    if (dtp < ngayBDN || dtp > ngayKTN)
                                                    {
                                                        DataRow dr = dtLich.NewRow();
                                                        dr["NgayDay"] = dtp;
                                                        dtLich.Rows.Add(dr);
                                                    }
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday || dtp.DayOfWeek.ToString() == DayOfWeek.Thursday)
                                            //{
                                            //    if (dtp < ngayBDN || dtp > ngayKTN)
                                            //    {
                                            //        DataRow dr = dtLich.NewRow();
                                            //        dr["NgayDay"] = dtp;
                                            //        dtLich.Rows.Add(dr);
                                            //    }
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Thursday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Thursday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Saturday")
                                                {
                                                    if (dtp < ngayBDN || dtp > ngayKTN)
                                                    {
                                                        DataRow dr = dtLich.NewRow();
                                                        dr["NgayDay"] = dtp;
                                                        dtLich.Rows.Add(dr);
                                                    }
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Thursday || dtp.DayOfWeek.ToString() == DayOfWeek.Saturday)
                                            //{
                                            //    if (dtp < ngayBDN || dtp > ngayKTN)
                                            //    {
                                            //        DataRow dr = dtLich.NewRow();
                                            //        dr["NgayDay"] = dtp;
                                            //        dtLich.Rows.Add(dr);
                                            //    }
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Saturday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {

                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Tuesday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Saturday")
                                                {
                                                    if (dtp < ngayBDN || dtp > ngayKTN)
                                                    {
                                                        DataRow dr = dtLich.NewRow();
                                                        dr["NgayDay"] = dtp;
                                                        dtLich.Rows.Add(dr);
                                                    }
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Saturday || dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday)
                                            //{
                                            //    if (dtp < ngayBDN || dtp > ngayKTN)
                                            //    {
                                            //        DataRow dr = dtLich.NewRow();
                                            //        dr["NgayDay"] = dtp;
                                            //        dtLich.Rows.Add(dr);
                                            //    }
                                            //}
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else // trường hợp ngày bắt đầu khóa rơi vào trường hợp đặc biệt.
                            {
                                if (Chan == true)
                                {
                                    #region
                                    if (ngayBD.DayOfWeek == DayOfWeek.Monday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Monday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Friday")
                                                {
                                                    if (dtp < ngayBDN || dtp > ngayKTN)
                                                    {
                                                        DataRow dr = dtLich.NewRow();
                                                        dr["NgayDay"] = dtp;
                                                        dtLich.Rows.Add(dr);
                                                    }
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Monday || dtp.DayOfWeek.ToString() == DayOfWeek.Friday)
                                            //{
                                            //    if (dtp < ngayBDN || dtp > ngayKTN)
                                            //    {
                                            //        DataRow dr = dtLich.NewRow();
                                            //        dr["NgayDay"] = dtp;
                                            //        dtLich.Rows.Add(dr);
                                            //    }
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Wednesday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Wednesday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Monday")
                                                {
                                                    if (dtp < ngayBDN || dtp > ngayKTN)
                                                    {
                                                        DataRow dr = dtLich.NewRow();
                                                        dr["NgayDay"] = dtp;
                                                        dtLich.Rows.Add(dr);
                                                    }
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday || dtp.DayOfWeek.ToString() == DayOfWeek.Monday)
                                            //{
                                            //    if (dtp < ngayBDN || dtp > ngayKTN)
                                            //    {
                                            //        DataRow dr = dtLich.NewRow();
                                            //        dr["NgayDay"] = dtp;
                                            //        dtLich.Rows.Add(dr);
                                            //    }
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Friday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Wednesday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Friday")
                                                {
                                                    if (dtp < ngayBDN || dtp > ngayKTN)
                                                    {
                                                        DataRow dr = dtLich.NewRow();
                                                        dr["NgayDay"] = dtp;
                                                        dtLich.Rows.Add(dr);
                                                    }
                                                }
                                            }

                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Friday || dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday)
                                            //{
                                            //    if (dtp < ngayBDN || dtp > ngayKTN)
                                            //    {
                                            //        DataRow dr = dtLich.NewRow();
                                            //        dr["NgayDay"] = dtp;
                                            //        dtLich.Rows.Add(dr);
                                            //    }
                                            //}
                                        }
                                    }
                                    #endregion
                                }
                                else if (Le == true)
                                {
                                    #region
                                    if (ngayBD.DayOfWeek == DayOfWeek.Tuesday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Tuesday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Saturday")
                                                {
                                                    if (dtp < ngayBDN || dtp > ngayKTN)
                                                    {
                                                        DataRow dr = dtLich.NewRow();
                                                        dr["NgayDay"] = dtp;
                                                        dtLich.Rows.Add(dr);
                                                    }
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday || dtp.DayOfWeek.ToString() == DayOfWeek.Saturday)
                                            //{
                                            //    if (dtp < ngayBDN || dtp > ngayKTN)
                                            //    {
                                            //        DataRow dr = dtLich.NewRow();
                                            //        dr["NgayDay"] = dtp;
                                            //        dtLich.Rows.Add(dr);
                                            //    }
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Thursday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Tuesday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Thursday")
                                                {
                                                    if (dtp < ngayBDN || dtp > ngayKTN)
                                                    {
                                                        DataRow dr = dtLich.NewRow();
                                                        dr["NgayDay"] = dtp;
                                                        dtLich.Rows.Add(dr);
                                                    }
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Thursday || dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday)
                                            //{
                                            //    if (dtp < ngayBDN || dtp > ngayKTN)
                                            //    {
                                            //        DataRow dr = dtLich.NewRow();
                                            //        dr["NgayDay"] = dtp;
                                            //        dtLich.Rows.Add(dr);
                                            //    }
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Saturday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Saturday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Thursday")
                                                {
                                                    if (dtp < ngayBDN || dtp > ngayKTN)
                                                    {
                                                        DataRow dr = dtLich.NewRow();
                                                        dr["NgayDay"] = dtp;
                                                        dtLich.Rows.Add(dr);
                                                    }
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Saturday || dtp.DayOfWeek.ToString() == DayOfWeek.Thursday)
                                            //{
                                            //    if (dtp < ngayBDN || dtp > ngayKTN)
                                            //    {
                                            //        DataRow dr = dtLich.NewRow();
                                            //        dr["NgayDay"] = dtp;
                                            //        dtLich.Rows.Add(dr);
                                            //    }
                                            //}
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            else
            {
                if (snd == "1")
                {
                    #region 1 người dạy và học 1 hoặc 2 hoặc 3 buổi/tuần
                    for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                    {
                        foreach (DataRow row in dtThu.Rows)
                        {
                            if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString())
                            {                            
                                    DataRow dr = dtLich.NewRow();
                                    dr["NgayDay"] = dtp;
                                    dtLich.Rows.Add(dr);
                            }
                        }
                        //if (Chan == true) // 2,4,6
                        //{
                        //    if (dtp.DayOfWeek.ToString() == DayOfWeek.Monday || dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday || dtp.DayOfWeek.ToString() == DayOfWeek.Friday)
                        //    {
                        //        DataRow dr = dtLich.NewRow();
                        //        dr["NgayDay"] = dtp;
                        //        dtLich.Rows.Add(dr);
                        //    }
                        //}
                        //else if (Le == true) //3,5,7
                        //{
                        //    if (dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday || dtp.DayOfWeek.ToString() == DayOfWeek.Thursday || dtp.DayOfWeek.ToString() == DayOfWeek.Saturday)
                        //    {
                        //        DataRow dr = dtLich.NewRow();
                        //        dr["NgayDay"] = dtp;
                        //        dtLich.Rows.Add(dr);
                        //    }
                        //}
                        //else if (CN == true)
                        //{
                        //    if (dtp.DayOfWeek.ToString() == DayOfWeek.Saturday || dtp.DayOfWeek.ToString() == DayOfWeek.Sunday)
                        //    {
                        //        DataRow dr = dtLich.NewRow();
                        //        dr["NgayDay"] = dtp;
                        //        dtLich.Rows.Add(dr);
                        //    }
                        //}
                        //else
                        //{
                        //    if (dtp.DayOfWeek.ToString() == DayOfWeek.Sunday)
                        //    {
                        //        DataRow dr = dtLich.NewRow();
                        //        dr["NgayDay"] = dtp;
                        //        dtLich.Rows.Add(dr);
                        //    }
                        //}
                    }
                    #endregion
                }
                else if (snd == "2") // t7, cn
                {
                    #region 2 người dạy và học 2 buổi/tuần
                    if (CN == true)
                    {
                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(7))
                        {
                            foreach (DataRow row in dtThu.Rows)
                            {
                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString())
                                {
                                    DataRow dr = dtLich.NewRow();
                                    dr["NgayDay"] = dtp;
                                    dtLich.Rows.Add(dr);
                                }
                            }
                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Saturday || dtp.DayOfWeek.ToString() == DayOfWeek.Sunday)
                            //{
                            //    DataRow dr = dtLich.NewRow();
                            //    dr["NgayDay"] = dtp;
                            //    dtLich.Rows.Add(dr);
                            //}
                        }
                    }
                    #endregion
                }
                else if (snd == "3")
                {
                    //Kiểm tra xem lớp này có mấy người dạy
                    dvLop.RowFilter = "MaLop = '" + malop + "' and NgayKT is null";                    
                    bool flag = false;//false la 3 gv dạy, true la 2 gv dạy
                    foreach (DataRowView drv in dvLop)
                    {
                        if (drv["TyLe"].ToString().Equals("2/3"))
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (!flag)
                    {
                        #region 3 người dạy và học 3 buổi/tuần ==> tỷ lệ đều là 1/3
                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(7))
                        {
                            foreach (DataRow row in dtThu.Rows)
                            {
                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString())
                                {
                                    DataRow dr = dtLich.NewRow();
                                    dr["NgayDay"] = dtp;
                                    dtLich.Rows.Add(dr);
                                }
                            }
                            //if (Chan == true) // 2,4,6
                            //{
                            //    if (dtp.DayOfWeek.ToString() == DayOfWeek.Monday || dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday || dtp.DayOfWeek.ToString() == DayOfWeek.Friday)
                            //    {
                            //        DataRow dr = dtLich.NewRow();
                            //        dr["NgayDay"] = dtp;
                            //        dtLich.Rows.Add(dr);
                            //    }
                            //}
                            //else if (Le == true) //3,5,7
                            //{
                            //    if (dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday || dtp.DayOfWeek.ToString() == DayOfWeek.Thursday || dtp.DayOfWeek.ToString() == DayOfWeek.Saturday)
                            //    {
                            //        DataRow dr = dtLich.NewRow();
                            //        dr["NgayDay"] = dtp;
                            //        dtLich.Rows.Add(dr);
                            //    }
                            //}
                        }
                        #endregion
                    }
                    else
                    {
                        #region 2 người dạy và học 3 buổi/tuần ==> tỷ lệ 1/3 và 2/3
                        if (tyle == "1/3")
                        {
                            for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(7))
                            {
                                foreach (DataRow row in dtThu.Rows)
                                {
                                    if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString())
                                    {
                                        DataRow dr = dtLich.NewRow();
                                        dr["NgayDay"] = dtp;
                                        dtLich.Rows.Add(dr);
                                    }
                                }
                                //DataRow dr = dtLich.NewRow();
                                //dr["NgayDay"] = dtp;
                                //dtLich.Rows.Add(dr);
                            }
                        }
                        else if (tyle == "2/3")
                        {
                            // Added 2012-07-16 cho trường hợp ngày bd khóa học đặc biệt
                            bool flg = GetGV(malop, ngayBD, magio);
                            if (!flg) // Trường hợp ngày bắt đầu khóa bình thường
                            {
                                if (Chan == true)
                                {
                                    #region
                                    if (ngayBD.DayOfWeek == DayOfWeek.Monday)
                                    {
                                        
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Monday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Wednesday")
                                                {
                                                    DataRow dr = dtLich.NewRow();
                                                    dr["NgayDay"] = dtp;
                                                    dtLich.Rows.Add(dr);
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Monday || dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday)
                                            //{
                                            //    DataRow dr = dtLich.NewRow();
                                            //    dr["NgayDay"] = dtp;
                                            //    dtLich.Rows.Add(dr);
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Wednesday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Friday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Wednesday")
                                                {
                                                    DataRow dr = dtLich.NewRow();
                                                    dr["NgayDay"] = dtp;
                                                    dtLich.Rows.Add(dr);
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday || dtp.DayOfWeek.ToString() == DayOfWeek.Friday)
                                            //{
                                            //    DataRow dr = dtLich.NewRow();
                                            //    dr["NgayDay"] = dtp;
                                            //    dtLich.Rows.Add(dr);
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Friday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Friday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Monday")
                                                {
                                                    DataRow dr = dtLich.NewRow();
                                                    dr["NgayDay"] = dtp;
                                                    dtLich.Rows.Add(dr);
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Friday || dtp.DayOfWeek.ToString() == DayOfWeek.Monday)
                                            //{
                                            //    DataRow dr = dtLich.NewRow();
                                            //    dr["NgayDay"] = dtp;
                                            //    dtLich.Rows.Add(dr);
                                            //}
                                        }
                                    }
                                    #endregion
                                }
                                else if (Le ==true)
                                {
                                    #region
                                    if (ngayBD.DayOfWeek == DayOfWeek.Tuesday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Tuesday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Thursday")
                                                {
                                                    DataRow dr = dtLich.NewRow();
                                                    dr["NgayDay"] = dtp;
                                                    dtLich.Rows.Add(dr);
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday || dtp.DayOfWeek.ToString() == DayOfWeek.Thursday)
                                            //{
                                            //    DataRow dr = dtLich.NewRow();
                                            //    dr["NgayDay"] = dtp;
                                            //    dtLich.Rows.Add(dr);
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Thursday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Saturday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Thursday")
                                                {
                                                    DataRow dr = dtLich.NewRow();
                                                    dr["NgayDay"] = dtp;
                                                    dtLich.Rows.Add(dr);
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Thursday || dtp.DayOfWeek.ToString() == DayOfWeek.Saturday)
                                            //{
                                            //    DataRow dr = dtLich.NewRow();
                                            //    dr["NgayDay"] = dtp;
                                            //    dtLich.Rows.Add(dr);
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Saturday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Saturday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Tuesday")
                                                {
                                                    DataRow dr = dtLich.NewRow();
                                                    dr["NgayDay"] = dtp;
                                                    dtLich.Rows.Add(dr);
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Saturday || dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday)
                                            //{
                                            //    DataRow dr = dtLich.NewRow();
                                            //    dr["NgayDay"] = dtp;
                                            //    dtLich.Rows.Add(dr);
                                            //}
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else // trường hợp ngày bắt đầu khóa rơi vào trường hợp đặc biệt.
                            {
                                if (Chan == true)
                                {
                                    #region
                                    if (ngayBD.DayOfWeek == DayOfWeek.Monday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Friday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Monday")
                                                {
                                                    DataRow dr = dtLich.NewRow();
                                                    dr["NgayDay"] = dtp;
                                                    dtLich.Rows.Add(dr);
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Monday || dtp.DayOfWeek.ToString() == DayOfWeek.Friday)
                                            //{
                                            //    DataRow dr = dtLich.NewRow();
                                            //    dr["NgayDay"] = dtp;
                                            //    dtLich.Rows.Add(dr);
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Wednesday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Wednesday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Monday")
                                                {
                                                    DataRow dr = dtLich.NewRow();
                                                    dr["NgayDay"] = dtp;
                                                    dtLich.Rows.Add(dr);
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday || dtp.DayOfWeek.ToString() == DayOfWeek.Monday)
                                            //{
                                            //    DataRow dr = dtLich.NewRow();
                                            //    dr["NgayDay"] = dtp;
                                            //    dtLich.Rows.Add(dr);
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Friday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Wednesday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Friday")
                                                {
                                                    DataRow dr = dtLich.NewRow();
                                                    dr["NgayDay"] = dtp;
                                                    dtLich.Rows.Add(dr);
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Friday || dtp.DayOfWeek.ToString() == DayOfWeek.Wednesday)
                                            //{
                                            //    DataRow dr = dtLich.NewRow();
                                            //    dr["NgayDay"] = dtp;
                                            //    dtLich.Rows.Add(dr);
                                            //}
                                        }
                                    }
                                    #endregion
                                }
                                else if (Le == true)
                                {
                                    #region
                                    if (ngayBD.DayOfWeek == DayOfWeek.Tuesday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Tuesday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Saturday")
                                                {
                                                    DataRow dr = dtLich.NewRow();
                                                    dr["NgayDay"] = dtp;
                                                    dtLich.Rows.Add(dr);
                                                }
                                            }
                                        //    if (dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday || dtp.DayOfWeek.ToString() == DayOfWeek.Saturday)
                                        //    {
                                        //        DataRow dr = dtLich.NewRow();
                                        //        dr["NgayDay"] = dtp;
                                        //        dtLich.Rows.Add(dr);
                                        //    }
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Thursday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Tuesday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Thursday")
                                                {
                                                    DataRow dr = dtLich.NewRow();
                                                    dr["NgayDay"] = dtp;
                                                    dtLich.Rows.Add(dr);
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Thursday || dtp.DayOfWeek.ToString() == DayOfWeek.Tuesday)
                                            //{
                                            //    DataRow dr = dtLich.NewRow();
                                            //    dr["NgayDay"] = dtp;
                                            //    dtLich.Rows.Add(dr);
                                            //}
                                        }
                                    }
                                    else if (ngayBD.DayOfWeek == DayOfWeek.Saturday)
                                    {
                                        for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
                                        {
                                            foreach (DataRow row in dtThu.Rows)
                                            {
                                                if (dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Saturday" || dtp.DayOfWeek.ToString() == row["DayOfWeek"].ToString() && row["DayOfWeek"].ToString() == "Thursday")
                                                {
                                                    DataRow dr = dtLich.NewRow();
                                                    dr["NgayDay"] = dtp;
                                                    dtLich.Rows.Add(dr);
                                                }
                                            }
                                            //if (dtp.DayOfWeek.ToString() == DayOfWeek.Saturday || dtp.DayOfWeek.ToString() == DayOfWeek.Thursday)
                                            //{
                                            //    DataRow dr = dtLich.NewRow();
                                            //    dr["NgayDay"] = dtp;
                                            //    dtLich.Rows.Add(dr);
                                            //}
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            return dtLich;
        }

    
        private void btnOk_Click(object sender, EventArgs e)
        {
            //TaoLich(Int32.Parse(seThang.Text));
            TaoLichMoi(Int32.Parse(seThang.Text));
            this.Close();
        }

        private bool GetGV(string malop, DateTime ngayBD, string magio)
        {
            // Hàm dùng để kiểm tra ngày bắt đầu dạy có khách thường hay không
            // vd lớp A học vào 2,4,6 mà GV1 có tỷ lệ 2/3 (dạy vào thứ 2,4), gv2 tỷ lệ 1/3 (dạy vào thứ 6)
            //nhưng ngày bắt đầu khóa rơi vào thứ 4, thì ko thể thiết lập ngày bắt đầu dạy của gv như bình thường.
            // vì 2/3 sẽ lấy các cặp ngày là 2-4, 4-6, 6-2, mà ngày bd là thứ 4 nếu lấy 4-6 sẽ sai vì gv1 dạy vào 2-4
            DataTable dt = LayLop(malop, ngayBD);                       
            if (dt.Rows.Count > 1)
            {
                DateTime ngayBD1, ngayBD2;
                string tyle1 = "";
                ngayBD1 = DateTime.Parse(dt.Rows[0]["NgayBD"].ToString());
                ngayBD2 = DateTime.Parse(dt.Rows[1]["NgayBD"].ToString());
                tyle1 = dt.Rows[0]["TyLe"].ToString();
                // Chỉnh sửa mới SHZ theo mã giờ học 
                using (DataTable dtValue = GetValue(magio))
                {
                    if (dtValue.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtValue.Rows)
                        {
                            if (row["Value"].ToString() == "2" || row["Value"].ToString() == "4" || row["Value"].ToString() == "6")
                            {
                                if (ngayBD1.DayOfWeek == DayOfWeek.Monday && ngayBD2.DayOfWeek == DayOfWeek.Wednesday && tyle1 == "2/3")
                                    return true;
                                else if (ngayBD1.DayOfWeek == DayOfWeek.Wednesday && ngayBD2.DayOfWeek == DayOfWeek.Friday && tyle1 == "2/3")
                                    return true;
                                else if (ngayBD1.DayOfWeek == DayOfWeek.Friday && ngayBD2.DayOfWeek == DayOfWeek.Monday && tyle1 == "2/3")
                                    return true;
                            }
                            else if (row["Value"].ToString() == "3" || row["Value"].ToString() == "5" || row["Value"].ToString() == "7")
                            {
                             if (ngayBD1.DayOfWeek == DayOfWeek.Tuesday && ngayBD2.DayOfWeek == DayOfWeek.Thursday && tyle1 == "2/3")
                                 return true;
                             else if (ngayBD1.DayOfWeek == DayOfWeek.Thursday && ngayBD2.DayOfWeek == DayOfWeek.Saturday && tyle1 == "2/3")
                                 return true;
                             else if (ngayBD1.DayOfWeek == DayOfWeek.Saturday && ngayBD2.DayOfWeek == DayOfWeek.Tuesday && tyle1 == "2/3")
                                 return true;
                            }
                        }
                    }

                }
                #region SHZ Old
                //if (magio == "L")
                //{
                //    if (ngayBD1.DayOfWeek == DayOfWeek.Monday && ngayBD2.DayOfWeek == DayOfWeek.Wednesday && tyle1=="2/3")
                //        return true;
                //    else if (ngayBD1.DayOfWeek == DayOfWeek.Wednesday && ngayBD2.DayOfWeek == DayOfWeek.Friday && tyle1 == "2/3")
                //        return true;
                //    else if (ngayBD1.DayOfWeek == DayOfWeek.Friday && ngayBD2.DayOfWeek == DayOfWeek.Monday && tyle1 == "2/3")
                //        return true;
                //}
                //else if (magio == "C")
                //{
                //    if (ngayBD1.DayOfWeek == DayOfWeek.Tuesday && ngayBD2.DayOfWeek == DayOfWeek.Thursday && tyle1 == "2/3")
                //        return true;
                //    else if (ngayBD1.DayOfWeek == DayOfWeek.Thursday && ngayBD2.DayOfWeek == DayOfWeek.Saturday && tyle1 == "2/3")
                //        return true;
                //    else if (ngayBD1.DayOfWeek == DayOfWeek.Saturday && ngayBD2.DayOfWeek == DayOfWeek.Tuesday && tyle1 == "2/3")
                //        return true;
                //}
                #endregion 
            }
            return false;
        }
    }
}