using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using CDTDatabase;
using CDTLib;
using Plugins;
using System.Data;
using DevExpress.XtraEditors;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;

namespace KiemTraTL
{
    public class KiemTraTL:ICData
    {
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();
        DateTimeFormatInfo dfi = new DateTimeFormatInfo();

        #region ICData Members
 
        public KiemTraTL() 
        {
            dfi.LongDatePattern = "MM/dd/yyyy hh:mm:ss";
            dfi.ShortDatePattern = "MM/dd/yyyy";
        }

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
            if (_data.CurMasterIndex < 0)
                return;
            _data.DbData.EndMultiTrans();
                DataTable dt = new DataTable();
                dt = db.GetDataTable("SELECT * FROM DMLopHoc");
                foreach (DataRow drLop in dt.Rows)
                {
                    _data.DbData.UpdateByNonQuery("delete from TempLichHoc where  MaLop = '" + drLop["MaLop"] + "'");
                    // Note : phải lấy Value để so sánh không phải Thu
                    //lay them thong tin ngay gio hoc
                    string s = @"select gh.MaGioHoc, gh.MaCa, ct.Value, ct.TGBD, ct.TGKT from 
                    DMNgayGioHoc gh inner join CTGioHoc ct on gh.MaGioHoc = ct.MaGioHoc
                    where gh.MaGioHoc = '" + drLop["MaGioHoc"].ToString() + "'";
                    DataTable dtLH = _data.DbData.GetDataTable(s);

                    DateTime dtBD = DateTime.Parse(drLop["NgayBDKhoa"].ToString());
                    DateTime dtKT = DateTime.Parse(drLop["NgayKTKhoa"].ToString());
                    string sql = @"insert into templichhoc(MaLop, Ngay, MaGio, MaCa, TGBD, TGKT)
                        values(@MaLop, @Ngay, @MaGio, @MaCa, @TGBD, @TGKT)";
                    string[] paras = new string[] { "MaLop", "Ngay", "MaGio", "MaCa", "TGBD", "TGKT" };

                    foreach (DataRow dr in dtLH.Rows)
                    {
                        DataTable dtNgayDay = LayNgay(dtBD, dtKT, drLop, dr["Value"].ToString());
                        foreach (DataRow drvNgay in dtNgayDay.Rows)
                        {
                            object[] values = new object[] { drLop["MaLop"], drvNgay["NgayDay"], dr["MaGioHoc"], dr["MaCa"], dr["TGBD"], dr["TGKT"] };
                            _data.DbData.UpdateDatabyPara(sql, paras, values);
                        }
                    }
                }
            }
       // }
        #region Tạo lịch tạm của lớp

        private bool DoiLich(DataRow drLop)
        {
            string oMaLop = drLop["MaLop", DataRowVersion.Original].ToString();
            string oMaGioHoc = drLop["MaGioHoc", DataRowVersion.Original].ToString();
            string oNgayBDKhoa = drLop["NgayBDKhoa", DataRowVersion.Original].ToString();
            string oNgayKTKhoa = drLop["NgayKTKhoa", DataRowVersion.Original].ToString();

            string MaLop = drLop["MaLop", DataRowVersion.Current].ToString();
            string MaGioHoc = drLop["MaGioHoc", DataRowVersion.Current].ToString();
            string NgayBDKhoa = drLop["NgayBDKhoa", DataRowVersion.Current].ToString();
            string NgayKTKhoa = drLop["NgayKTKhoa", DataRowVersion.Current].ToString();

            if (oMaLop == MaLop && oMaGioHoc == MaGioHoc && oNgayBDKhoa == NgayBDKhoa && oNgayKTKhoa == NgayKTKhoa)
                return false;
            return true;
        }

        private bool TrungLichNghi(DateTime ngay, DataView dvLN)
        {
            foreach (DataRowView drv in dvLN)
                if (ngay >= DateTime.Parse(drv["NgayNghi"].ToString(), dfi)
                    && ngay <= DateTime.Parse(drv["DenNgay"].ToString(), dfi))
                    return true;
            return false;
        }

        private DataTable LayNgay(DateTime ngayBD, DateTime ngayKT, DataRow drLop, string value)
        {
            DataTable dtLich = new DataTable(); // Danh sach cac ngay day cua lop 
            DataColumn colNgay = new DataColumn("NgayDay", typeof(DateTime));
            dtLich.Columns.Add(colNgay);
            DayOfWeek dow;
            switch (value)
            {
                case "2":
                    dow = DayOfWeek.Monday;
                    break;
                case "3":
                    dow = DayOfWeek.Tuesday;
                    break;
                case "4":
                    dow = DayOfWeek.Wednesday;
                    break;
                case "5":
                    dow = DayOfWeek.Thursday;
                    break;
                case "6":
                    dow = DayOfWeek.Friday;
                    break;
                case "7":
                    dow = DayOfWeek.Saturday;
                    break;
                default:
                    dow = DayOfWeek.Sunday;
                    break;
            }
            //duyệt qua lịch học, so sánh với lịch nghỉ và lịch dạy để lấy ngày
            string ml = drLop["MaLop"].ToString();
            DataView dvLN = new DataView(_data.DsData.Tables[2]);
            dvLN.RowFilter = "MaLop = '" + ml + "'";
            for (DateTime dtp = ngayBD; dtp <= ngayKT; dtp = dtp.AddDays(1))
            {
                if (TrungLichNghi(dtp, dvLN))
                    continue;
                if (dtp.DayOfWeek == dow)
                {
                    DataRow dr = dtLich.NewRow();
                    dr["NgayDay"] = dtp;
                    dtLich.Rows.Add(dr);
                }
            }
            return dtLich;
        }
        #endregion


// Code cũ SHZ
        void KiemTra()
        {
            if (_data.CurMasterIndex < 0)
                return;
            Database db = Database.NewDataDatabase();
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster.RowState == DataRowState.Deleted)
                return;
            DataView dv = new DataView(_data.DsData.Tables[1]);
            dv.RowFilter = " MaLop = '" + drMaster["MaLop"].ToString() + "'";
            //nếu 1 người phụ trách là rỗng, 2 người là 1/3 và 2/3 hoặc 1/2, 3 người là 1/3 
            if (dv.Count == 0)
                return;

            #region old code chưa xử lý tình huống nhập thêm gv dạy thay
            //if (dv.Count == 1 && dv[0]["TyLe"].ToString() != "")
            //{
            //    XtraMessageBox.Show("Tỷ lệ dạy bị sai, vui lòng kiểm tra lại!",Config.GetValue("PackageName").ToString());
            //    _info.Result = false;
            //    return;
            //}
            //else if (dv.Count == 2)
            //{
            //    if ((dv[0]["TyLe"].ToString() != "1/2" || dv[1]["TyLe"].ToString() != "1/2") &&
            //        (dv[0]["TyLe"].ToString() != "1/3" || dv[1]["TyLe"].ToString() != "2/3") &&
            //        (dv[0]["TyLe"].ToString() != "2/3" || dv[1]["TyLe"].ToString() != "1/3") )
            //    {
            //        XtraMessageBox.Show("Tỷ lệ dạy giữa các giáo viên bị sai, vui lòng kiểm tra lại!", Config.GetValue("PackageName").ToString());
            //        _info.Result = false;   
            //    }
            //}
            //else if (dv.Count == 3)
            //{
            //    if (dv[0]["TyLe"].ToString() != "1/3" || dv[1]["TyLe"].ToString() != "1/3" || dv[2]["TyLe"].ToString() != "1/3" )
            //    {                
            //        XtraMessageBox.Show("Tỷ lệ dạy giữa các giáo viên bị sai, vui lòng kiểm tra lại!", Config.GetValue("PackageName").ToString());
            //        _info.Result = false;
            //    }
            //}
            //else if (dv.Count > 4)
            //{
            //    XtraMessageBox.Show("Một lớp học không được vượt quá 3 giáo viên phụ trách!", Config.GetValue("PackageName").ToString());
            //    _info.Result = false;
            //}
            #endregion

            dv.RowFilter = " MaLop = '" + drMaster["MaLop"].ToString() + "' and NgayKT is null";
            if (dv.Count == 1 && dv[0]["TyLe"].ToString() != "")
            {
                XtraMessageBox.Show("Tỷ lệ dạy bị sai, vui lòng kiểm tra lại!", Config.GetValue("PackageName").ToString());
                _info.Result = false;
                return;
            }
            else if (dv.Count == 2)
            {
                if ((dv[0]["TyLe"].ToString() != "1/2" || dv[1]["TyLe"].ToString() != "1/2") &&
                    (dv[0]["TyLe"].ToString() != "1/3" || dv[1]["TyLe"].ToString() != "2/3") &&
                    (dv[0]["TyLe"].ToString() != "2/3" || dv[1]["TyLe"].ToString() != "1/3"))
                {
                    XtraMessageBox.Show("Tỷ lệ dạy giữa các giáo viên bị sai, vui lòng kiểm tra lại!", Config.GetValue("PackageName").ToString());
                    _info.Result = false;
                }
            }
            else if (dv.Count == 3)
            {
                if (dv[0]["TyLe"].ToString() != "1/3" || dv[1]["TyLe"].ToString() != "1/3" || dv[2]["TyLe"].ToString() != "1/3")
                {
                    XtraMessageBox.Show("Tỷ lệ dạy giữa các giáo viên bị sai, vui lòng kiểm tra lại!", Config.GetValue("PackageName").ToString());
                    _info.Result = false;
                }
            }
            else if (dv.Count > 4)
            {
                XtraMessageBox.Show("Một lớp học không được vượt quá 3 giáo viên phụ trách!", Config.GetValue("PackageName").ToString());
                _info.Result = false;
            }
        }

        private void KiemTraDoiLich()
        {
            if (_data.CurMasterIndex < 0)
                return;
            Database db = Database.NewDataDatabase();
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster.RowState == DataRowState.Deleted)
                return;
            string sql = "select MaLop from MTDK where MaLop = '" + drMaster["MaLop"].ToString() + "'";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count > 0 && DoiLich(drMaster))
            {
                if (XtraMessageBox.Show("Bạn đang thay đổi lịch học của lớp đã có học viên đăng ký học!" +
                    "\nNếu đồng ý thay đổi lịch, bạn nên kiểm tra lại dữ liệu đăng ký học của lớp này",
                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo) == DialogResult.Yes)
                    _info.Result = true;
                else
                    _info.Result = false;
            }
            else
                _info.Result = true;
        }

        public void ExecuteBefore()
        {
            //KiemTra();
            //KiemTraDoiLich();
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
} 
