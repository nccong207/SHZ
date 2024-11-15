using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using CDTDatabase;
using CDTLib;
using Plugins;
using System.Data;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using SHZCommon;

namespace HVChuyenLop
{
    public class HVChuyenLop:ICControl
    {
        #region ICControl Members

        Database db = Database.NewDataDatabase();
        private InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        private DataCustomFormControl data;
        LayoutControl lc;
        bool isHV = false;
        RadioGroup raHinhThuc;
        GridView gv;
        DataRow drMaster;
         
        public void AddEvent()
        {
            drMaster = (data.BsMain.Current as DataRowView).Row;
            if (drMaster.RowState == DataRowState.Deleted)
                return;
            data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(data.BsMain,new EventArgs());
            raHinhThuc = data.FrmMain.Controls.Find("HinhThuc",true)[0] as RadioGroup;
            raHinhThuc.EditValueChanged += new EventHandler(raHinhThuc_EditValueChanged);
            lc = data.FrmMain.Controls.Find("LcMain", true)[0] as LayoutControl;
            data.FrmMain.Shown += new EventHandler(FrmMain_Shown);
            gv = (data.FrmMain.Controls.Find("GcMain", true)[0] as GridControl).MainView as GridView;
        }

        void FrmMain_Shown(object sender, EventArgs e)
        {
            //if (raHinhThuc.EditValue.ToString() == "")            
            //    lc.Items.FindByName("lciSoBo").Visibility = LayoutVisibility.Never;            
        }

        void raHinhThuc_EditValueChanged(object sender, EventArgs e)
        {
            RadioGroup raHinhThuc = sender as RadioGroup;
            if (raHinhThuc == null)
                return;
            if (raHinhThuc.EditValue.ToString() == "")
                return;
            //lc.Items.FindByName("lciSoBo").Visibility = raHinhThuc.EditValue.ToString() == "0" ? LayoutVisibility.Always : LayoutVisibility.Never;

            ///Không hiện các sản phẩm và giáo trình, mà qua màn hình đăng ký sẽ xử lý

            //if (drMaster.RowState == DataRowState.Unchanged) // nếu load xem thì ko chạy
            //    return;

            //drMaster = (data.BsMain.Current as DataRowView).Row;

            //if (drMaster == null)
            //    return;
            ////if (gv.DataRowCount == 0 && drMaster !=null )
            //    BindSanPham(gv, drMaster["MaLopSau"].ToString(), DateTime.Parse(drMaster["NgayDK"].ToString()));                                
        }

        void XoaGridView(GridView gv)
        {
            while (gv.DataRowCount > 0)
                gv.DeleteRow(0);
        }

        void BindSanPham(GridView gv, string malop, DateTime NgayDK)
        {
            if (gv.DataRowCount > 0)
                XoaGridView(gv);
            string sql = "";
            DataTable dt;

            // Giáo trình
            if (malop != "")
            {
                sql = "select vt.mavt,vt.giaban,vt.tkkho, vt.tkgv, vt.tkdt " +
                             "from dmvt vt inner join dmnhomlop nl on vt.manhomlop=nl.MaNLop " +
                             "inner join DMlophoc L on L.MaNLop=nl.MaNLop " +
                             "where L.MaLop='" + malop + "'";
                dt = db.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        gv.AddNewRow();
                        gv.UpdateCurrentRow();
                        gv.SetFocusedRowCellValue(gv.Columns["MaSP"], row["MaVT"].ToString());
                        gv.SetFocusedRowCellValue(gv.Columns["Dongia"], row["giaban"].ToString());
                        if (row["tkdt"].ToString() != "")
                            gv.SetFocusedRowCellValue(gv.Columns["TKDT"], row["tkdt"].ToString());
                        if (row["tkgv"].ToString() != "")
                            gv.SetFocusedRowCellValue(gv.Columns["TKGV"], row["tkgv"].ToString());
                        if (row["tkkho"].ToString() != "")
                            gv.SetFocusedRowCellValue(gv.Columns["TKKho"], row["tkkho"].ToString());
                    }
                }
                if (drMaster == null)
                    return;
                if (drMaster["NguonHV"].ToString() != "1")
                    return;
                //qùa tặng
                //sql = "select G.MaSP, G.soluong, 0 as dongia, vt.tkkho, vt.tkdt, vt.tkgv from  DMQuatang G inner join DMVT VT on VT.MaVT=G.MaSP  and G.NgayHH >= '" + drMaster["NgayDK"].ToString() + "'";
                sql = " select G.MaSP, G.soluong, 0 as dongia, vt.tkkho, vt.tkdt, vt.tkgv " +
                 " from  DMQuatang G inner join DMVT VT on VT.MaVT=G.MaSP " +
                 " inner join DMHocPhi HP on HP.HPID = G.HPID " +
                 " inner join DMNhomLop NL on NL.MaNLop = HP.MaNL " +
                 " inner join DMLopHoc LH on LH.MaNLop = NL.MaNLop " +
                 " where G.NgayHH >= '" + NgayDK.ToString() + "' " +
                 " and LH.MaLop ='" + malop + "'";
                dt = db.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        gv.AddNewRow();
                        gv.UpdateCurrentRow();
                        gv.SetFocusedRowCellValue(gv.Columns["MaSP"], row["MaSP"].ToString());
                        gv.SetFocusedRowCellValue(gv.Columns["Dongia"], row["dongia"].ToString());
                        gv.SetFocusedRowCellValue(gv.Columns["SL"], row["soluong"].ToString());
                        gv.SetFocusedRowCellValue(gv.Columns["isQT"], 1);
                        if (row["tkdt"].ToString() != "")
                            gv.SetFocusedRowCellValue(gv.Columns["TKDT"], row["tkdt"].ToString());
                        if (row["tkgv"].ToString() != "")
                            gv.SetFocusedRowCellValue(gv.Columns["TKGV"], row["tkgv"].ToString());
                        if (row["tkkho"].ToString() != "")
                            gv.SetFocusedRowCellValue(gv.Columns["TKKho"], row["tkkho"].ToString());
                    }
                }
            }
        }

        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataSet dt = data.BsMain.DataSource as DataSet;
            if (dt == null)
                return;
            dt.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(HVChuyenLop_ColumnChanged);
        } 

        void HVChuyenLop_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Row.RowState == DataRowState.Deleted || e.Row.RowState==DataRowState.Detached)
                return;
            //điền tên HV
            if (e.Column.ColumnName.ToUpper().Equals("MAHV") || e.Column.ColumnName.ToUpper().Equals("MALOPHT") || e.Column.ColumnName.ToUpper().Equals("NGAYCL"))
            {
                if (e.Row["MaHV"].ToString() != "")
                {
                    string sql = " select TV.TenHV, DK.NgayDK " +
                                 " from MTDK DK inner join DMHVTV TV on DK.HVTVID=TV.HVTVID " +
                                 " where DK.MaHV = '" + e.Row["MaHV"].ToString() + "'";
                    DataTable dt = db.GetDataTable(sql);
                    //tien con lai   
                    DateTime ngayDK;
                    if (dt.Rows.Count > 0)
                    {
                        if (e.Column.ColumnName.ToUpper().Equals("MAHV") && !isHV)
                        {
                            if (e.Row["TenHV"].ToString() != dt.Rows[0]["TenHV"].ToString())
                            {
                                e.Row["TenHV"] = dt.Rows[0]["TenHV"].ToString();
                                e.Row.EndEdit();
                            }
                            isHV = true;
                        }
                        else
                            isHV = false;

                        ngayDK = DateTime.Parse(dt.Rows[0]["NgayDK"].ToString());
                    }
                    else
                    {
                        XtraMessageBox.Show("Không lấy được ngày đăng ký của học viên này để tính tiền còn lại!", Config.GetValue("PackageName").ToString());
                        return;
                    }
                    if (e.Row["MaHV"].ToString() != "" && e.Row["MaLopHT"].ToString() != "" && e.Row["NgayCL"].ToString() != "")
                    {
                        HocPhi hp = new HocPhi();
                        decimal tienCL = hp.TienConLai(DateTime.Parse(e.Row["NgayCL"].ToString()), e.Row["MaHV"].ToString());
                        e.Row["HPTruoc"] = tienCL;
                        //cap nhat tien phai dong, bao luu
                        if (e.Row["HPSau"].ToString() != "0")
                        {
                            decimal HPSau = decimal.Parse(e.Row["HPSau"].ToString());
                            if (HPSau >= tienCL)
                            {
                                e.Row["TTDong"] = HPSau - tienCL;
                                e.Row["TienBL"] = 0;
                            }
                            else
                            {
                                e.Row["TTDong"] = 0;
                                e.Row["TienBL"] = tienCL - HPSau;
                            }
                        }
                        e.Row.EndEdit();
                    }
                }
            }
            //Tiền HP lop trước còn lại + mã học viên
            if (e.Column.ColumnName.ToUpper().Equals("MALOPSAU") || e.Column.ColumnName.ToUpper().Equals("GIAM") || e.Column.ColumnName.ToUpper().Equals("NGAYHOCLAI"))
            {
                if (e.Column.ColumnName.ToUpper().Equals("MALOPSAU"))
                {
                    string mahv = CreateMaHV(e.Row["MaLopSau"].ToString());
                    e.Row["MaHVSau"] = mahv;
                }

                if (e.Row["MaLopSau"].ToString() != "" && e.Row["NgayHocLai"].ToString() != "")
                {

                    decimal tienCL = decimal.Parse(e.Row["HPTruoc"].ToString());
                    //Tien HP lop sau phai dong                                                                    
                    decimal HPLopSau = TinhHocPhi(DateTime.Parse(e.Row["NgayHocLai"].ToString()), e.Row["MaLopSau"].ToString(), decimal.Parse(e.Row["Giam"].ToString()));
                    e.Row["HPSau"] = HPLopSau;
                    if (HPLopSau > tienCL)
                    {
                        e.Row["TTDong"] = HPLopSau - tienCL;
                        e.Row["TienBL"] = 0;
                    }
                    else
                    {
                        e.Row["TienBL"] = tienCL - HPLopSau;
                        e.Row["TTDong"]=0;
                    }
                    //Cap nhat con tien no de chuyen qua man hinh dang ky
                    if (drMaster != null)
                    {
                        if (drMaster["HPNoTruoc"].ToString() != "")
                        {
                            e.Row["HPNoTruoc"] = drMaster["HPNoTruoc"];
                        }
                    }
                }
                e.Row.EndEdit();
            }
            ////số lượng giáo trình -- còn lỗi 
            //if (e.Column.ColumnName.ToUpper().Equals("SOBO"))
            //{
            //    DataSet ds = data.BsMain.DataSource as DataSet;
            //    if (ds == null)
            //        return;
            //    DataTable dtQT = ds.Tables[1];
            //    DataView dvQT = new DataView(dtQT);
            //    dvQT.RowFilter = "";
            //    foreach (DataRow row in dtQT.Rows)
            //    {
            //        if (row["isQT"].ToString().ToUpper().Equals("FALSE"))
            //            row["SL"] = e.Row["SoBo"].ToString();
            //    }
            //}
        }

        string CreateMaHV(string malop)
        {
            string mahv = malop;
            string sql = "select  MaHV from MTDK where MaHV like '" + malop + "%' order by MaHV DESC";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count == 0)
                mahv += "01";
            else
            {
                string stt = dt.Rows[0]["MaHV"].ToString();
                stt = stt.Replace(malop, "");
                if (stt == "")
                {
                    XtraMessageBox.Show("Tạo mã học sinh không thành công!", Config.GetValue("PackageName").ToString());
                    return null;
                }
                else
                {
                    //string a = GetNewValue(stt); // Thêm mới dang kt
                    int dem = int.Parse(stt) + 1;
                    if (dem < 10)
                        mahv += "0" + dem.ToString();
                    else
                        mahv += dem.ToString();
                }
                if (mahv.Length > 16) // do mã lớp tối đa 14 ký tự, và 2 ký tự cuối là số thứ tự của số lượng học viên trong lớp. 
                {
                    XtraMessageBox.Show("Mã hã học viên tạo ra vượt quá 11 ký tự quy định!", Config.GetValue("PackageName").ToString());
                    return null;
                }
            }
            return mahv;
        }

        private string GetNewValue(string OldValue)
        {
            try
            {
                int i = OldValue.Length - 1;
                for (; i > 0; i--)
                    if (!Char.IsNumber(OldValue, i))
                        break;
                if (i == OldValue.Length - 1)
                {
                    int NewValue = Int32.Parse(OldValue) + 1;
                    return NewValue.ToString();
                }
                string PreValue = OldValue.Substring(0, i + 1);
                string SufValue = OldValue.Substring(i + 1);
                int intNewSuff = Int32.Parse(SufValue) + 1;
                string NewSuff = intNewSuff.ToString().PadLeft(SufValue.Length, '0');
                return (PreValue + NewSuff);
            }
            catch
            {
                return string.Empty;
            }
        }

        int SoNgayDu(DateTime ngayBD, DateTime ngayKT, DateTime ngayDK, string magio, string ngayBDNghi, string NgayKTNghi)
        {
            int count = 0;
            if (ngayBDNghi != "" && NgayKTNghi != "")
            {
                DateTime ngayBDN = DateTime.Parse(ngayBDNghi);
                DateTime ngayKTN = DateTime.Parse(NgayKTNghi);
                for (DateTime dtp = ngayDK; dtp <= ngayKT; dtp = dtp.AddDays(1))
                {
                    if (magio == "L") // 2,4,6
                    {
                        if (dtp < ngayBDN || dtp > ngayKTN) //nếu trong ngày nghỉ thì không tính
                        {
                            if (dtp.DayOfWeek == DayOfWeek.Monday || dtp.DayOfWeek == DayOfWeek.Wednesday || dtp.DayOfWeek == DayOfWeek.Friday)
                                count++;
                        }
                    }
                    else if (magio == "C") //3,5,7
                    {
                        if (dtp < ngayBDN || dtp > ngayKTN)
                        {
                            if (dtp.DayOfWeek == DayOfWeek.Tuesday || dtp.DayOfWeek == DayOfWeek.Thursday || dtp.DayOfWeek == DayOfWeek.Saturday)
                                count++;
                        }
                    }
                    else if (magio == "B")
                    {
                        if (dtp < ngayBDN || dtp > ngayKTN)
                        {
                            if (dtp.DayOfWeek == DayOfWeek.Saturday || dtp.DayOfWeek == DayOfWeek.Sunday)
                                count++;
                        }
                    }
                    else
                    {
                        if (dtp < ngayBDN || dtp > ngayKTN)
                        {
                            if (dtp.DayOfWeek == DayOfWeek.Sunday)
                                count++;
                        }
                    }
                }
            }
            else
            {
                for (DateTime dtp = ngayDK; dtp <= ngayKT; dtp = dtp.AddDays(1))
                {
                    if (magio == "L") // 2,4,6
                    {
                        if (dtp.DayOfWeek == DayOfWeek.Monday || dtp.DayOfWeek == DayOfWeek.Wednesday || dtp.DayOfWeek == DayOfWeek.Friday)
                            count++;
                    }
                    else if (magio == "C") //3,5,7
                    {
                        if (dtp.DayOfWeek == DayOfWeek.Tuesday || dtp.DayOfWeek == DayOfWeek.Thursday || dtp.DayOfWeek == DayOfWeek.Saturday)
                            count++;
                    }
                    else if (magio == "B")
                    {
                        if (dtp.DayOfWeek == DayOfWeek.Saturday || dtp.DayOfWeek == DayOfWeek.Sunday)
                            count++;
                    }
                    else
                    {
                        if (dtp.DayOfWeek == DayOfWeek.Sunday)
                            count++;
                    }
                }
            }
            return count;
        }
        /*
        decimal TienConLai(DateTime ngayDK, DateTime ngayCL, string malop, string mahv)
        {
            if (data.BsMain.Current == null)
                return 0;
            drMaster = (data.BsMain.Current as DataRowView).Row;

            // cập nhật tiền nợ trước để chuyển qua hv đăng ký
            drMaster["HPNoTruoc"] = decimal.Parse(dr["ConLai"].ToString());          
            
            return tienBL;
        }
        */
        decimal RoundNumber(decimal num)
        {                 
            num = num / 1000;
            num = Math.Round(num, 0, MidpointRounding.AwayFromZero);           
            num *= 1000;
            return num;        
        }

        decimal TinhHocPhi(DateTime ngayHL, string malop, decimal giam)
        {
            if (data.BsMain.Current == null)
                return 0;
            drMaster = (data.BsMain.Current as DataRowView).Row;
            string sql = "select NgayBDKhoa, NgayKTKhoa, BDNghi, KTNghi, MaGioHoc from DMLopHoc where MaLop='" + malop + "'";
            DataTable dtLop = db.GetDataTable(sql);
            DataTable dt;
            if (dtLop.Rows.Count == 0)
                return 0;  
            if (dtLop.Rows[0]["NgayBDKhoa"].ToString() != "" && dtLop.Rows[0]["NgayKTKhoa"].ToString() != "")
            {
                sql = " select HPNL.HocPhi, l.sobuoi  " +
                      " from dmlophoc l inner join dmhocphi hp on l.MaNLop=hp.MaNL  " +
                      " inner join HPNL on HPNL.HPID=hp.HPID " +
                      " inner join DMNhomLop NL on NL.MaNLop=hp.MaNL " +
                      " where l.MaLop='" + malop + "' " +
                      " and HPNL.NgayBD <='" + ngayHL.ToString() + "' order by HPNL.NgayBD DESC ";
                dt = db.GetDataTable(sql);

                if (dt.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có học phí nào áp dụng trong khoảng thời gian này!", Config.GetValue("PackageName").ToString());
                    return 0;
                }
                //học phí chuẩn
                decimal hocphi = decimal.Parse(dt.Rows[0]["HocPhi"].ToString());

                //tổng số buổi học
                decimal sobuoi = decimal.Parse(dt.Rows[0]["Sobuoi"].ToString());

                //% khuyến học
                string sqlKH = " select KH.tyle, KH.NgayBD,KH.NgayKT " +
                               " from DMLopHoc L inner join DMHocPhi HP on L.MaNLop = HP.MaNL " +
                               " inner join dmkhuyenhoc KH  on KH.HPID = HP.HPID " +
                               " where L.MaLop = '" + malop + "' " +
                               " and ( '" + ngayHL.ToString() + "' between KH.NgayBD and KH.NgayKT) ";

                DataTable dtKH = db.GetDataTable(sqlKH);

                string magio = dtLop.Rows[0]["MaGioHoc"].ToString();
                if (magio != "" && magio.Length > 1)
                    magio = magio.Substring(0, 1);

                //đăng ký sau ngày khai giảng
                if (ngayHL > DateTime.Parse(dtLop.Rows[0]["NgayBDKhoa"].ToString()) && ngayHL < DateTime.Parse(dtLop.Rows[0]["NgayKTKhoa"].ToString()))
                {
                    //int sobuoitre = SoNgayTre(DateTime.Parse(dtLop.Rows[0]["NgayBDKhoa"].ToString()), DateTime.Parse(dtLop.Rows[0]["NgayKTKhoa"].ToString()), ngayHL, magio, dtLop.Rows[0]["BDNghi"].ToString(), dtLop.Rows[0]["KTNghi"].ToString());
                 // Chỉnh sửa mới SHZ
                    //DataTable dtNgay = new DataTable();
                    //dt = db.GetDataTable(string.Format("SELECT NgayDK FROM MTDK WHERE MaHV = '{0}'",drMaster["MaHV"].ToString()));
                    // Số buổi lớp đã học
                    DataTable dtbh = db.GetDataTable(string.Format("exec sp_Sobuoidahoc '{0}','{1}' ", ngayHL, malop));
                    decimal sbdahoc = decimal.Parse(dtbh.Rows[0]["sobuoidh"].ToString());
                    decimal conlai = sobuoi - sbdahoc;
                    hocphi = (hocphi / sobuoi) * conlai;
                    //Thêm mới để chuyển số buổi còn lại qua đăng ký, làm cơ sở tính doanh thu
                    if (drMaster != null
                     && (drMaster["SBCL"].ToString() == "" || decimal.Parse(drMaster["SBCL"].ToString()) != conlai))
                        drMaster["SBCL"] = (int)conlai;
                }
                else
                {
                    if (ngayHL >= DateTime.Parse(dtLop.Rows[0]["NgayKTKhoa"].ToString()))
                    {
                        XtraMessageBox.Show("Ngày học lại lớn hơn ngày kết thúc của lớp đăng ký.", Config.GetValue("PackageName").ToString());
                        drMaster["SBCL"] = 0;
                        return 0;
                    }
                    if (drMaster != null
                     && (drMaster["SBCL"].ToString() == "" || decimal.Parse(drMaster["SBCL"].ToString()) != sobuoi))
                        drMaster["SBCL"] = (int)sobuoi;
                }
                //lấy % khuyến học và tính học phí còn lại (chỉ tính khuyến học cho đăng ký đúng ngày)
                if (dtKH.Rows.Count > 0 && ngayHL <= DateTime.Parse(dtLop.Rows[0]["NgayBDKhoa"].ToString()))
                {
                    decimal kh = decimal.Parse(dtKH.Rows[0]["tyle"].ToString());
                    hocphi = hocphi - (hocphi * kh) / 100;
                }
                //lấy mức giảm học phí và tính học phí cần nộp
                if (giam != 0)
                {
                    hocphi = hocphi - (hocphi * giam) / 100;
                }
                hocphi = RoundNumber(hocphi);               
                return hocphi;
            }
            else
                return 0;
        }

        int SoNgayTre(DateTime ngayBD, DateTime ngayKT, DateTime ngayDK, string magio, string ngayBDNghi, string NgayKTNghi)
        {
            int count = 0;
            if (ngayBDNghi != "" && NgayKTNghi != "")
            {
                DateTime ngayBDN = DateTime.Parse(ngayBDNghi);
                DateTime ngayKTN = DateTime.Parse(NgayKTNghi);
                for (DateTime dtp = ngayBD; dtp < ngayDK; dtp = dtp.AddDays(1))
                {
                    if (magio == "L") // 2,4,6
                    {
                        if (dtp < ngayBDN || dtp > ngayKTN) //nếu trong ngày nghỉ thì không tính
                        {
                            if (dtp.DayOfWeek == DayOfWeek.Monday || dtp.DayOfWeek == DayOfWeek.Wednesday || dtp.DayOfWeek == DayOfWeek.Friday)
                                count++;
                        }
                    }
                    else if (magio == "C") //3,5,7
                    {
                        if (dtp < ngayBDN || dtp > ngayKTN)
                        {
                            if (dtp.DayOfWeek == DayOfWeek.Tuesday || dtp.DayOfWeek == DayOfWeek.Thursday || dtp.DayOfWeek == DayOfWeek.Saturday)
                                count++;
                        }
                    }
                    else if (magio == "B")
                    {
                        if (dtp < ngayBDN || dtp > ngayKTN)
                        {
                            if (dtp.DayOfWeek == DayOfWeek.Saturday || dtp.DayOfWeek == DayOfWeek.Sunday)
                                count++;
                        }
                    }
                    else
                    {
                        if (dtp < ngayBDN || dtp > ngayKTN)
                        {
                            if (dtp.DayOfWeek == DayOfWeek.Sunday)
                                count++;
                        }
                    }
                }
            }
            else
            {
                for (DateTime dtp = ngayBD; dtp < ngayDK; dtp = dtp.AddDays(1))
                {
                    if (magio == "L") // 2,4,6
                    {
                        if (dtp.DayOfWeek == DayOfWeek.Monday || dtp.DayOfWeek == DayOfWeek.Wednesday || dtp.DayOfWeek == DayOfWeek.Friday)
                            count++;
                    }
                    else if (magio == "C") //3,5,7
                    {
                        if (dtp.DayOfWeek == DayOfWeek.Tuesday || dtp.DayOfWeek == DayOfWeek.Thursday || dtp.DayOfWeek == DayOfWeek.Saturday)
                            count++;
                    }
                    else if (magio == "B")
                    {
                        if (dtp.DayOfWeek == DayOfWeek.Saturday || dtp.DayOfWeek == DayOfWeek.Sunday)
                            count++;
                    }
                    else
                    {
                        if (dtp.DayOfWeek == DayOfWeek.Sunday)
                            count++;
                    }
                }
            }
            return count;
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
