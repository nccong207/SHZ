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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using System.Globalization;
using SHZCommon;
namespace HVBaoLuu
{
    public partial class frmHVBL : DevExpress.XtraEditors.XtraForm
    {
        public frmHVBL()
        {
            InitializeComponent();
        }

        bool flag = false;
        Database db = Database.NewDataDatabase();
        
        private void frmHVBL_Load(object sender, EventArgs e)
        {            
            repositoryItemDateEdit1.EditValueChanged += new EventHandler(repositoryItemDateEdit1_EditValueChanged);            
        }

        
        void repositoryItemDateEdit1_EditValueChanged(object sender, EventArgs e)
        {                        
            //DateEdit dateEdit = sender as DateEdit;
            //if (dateEdit.EditValue == null)
            //    return;
            //SetValue(dateEdit);
        }
                       
        void SetValue(DateEdit dateEdit)
        {
            string isBL = gvHocVien.GetFocusedRowCellValue("IsBL").ToString();
            if (isBL.ToUpper() == "FALSE")
                gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["NgayHH"], null);
            else
            {
                DateTime dtp;
                int songay = 0;
                if (Config.GetValue("HanBaoLuu") != null)
                {
                    songay = int.Parse(Config.GetValue("HanBaoLuu").ToString());
                    dtp = DateTime.Parse(dateEdit.EditValue.ToString());
                    dtp = dtp.AddDays(songay);
                    gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["NgayHH"], dtp);
                }
                //tiền bảo lưu
                if (gvHocVien.GetFocusedRowCellValue("NgayDK").ToString() != "" && gvHocVien.GetFocusedRowCellValue("MaHV").ToString() != "")
                {
                    HocPhi hp = new HocPhi();
                    decimal sotien = hp.TienConLai(DateTime.Parse(dateEdit.EditValue.ToString()), gvHocVien.GetFocusedRowCellValue("MaHV").ToString());
                    gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["BLSoTien"], sotien);
                    //gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["SoBuoiCL"], sobuocl);
                }
            }                 
        }

        void getDSLop()
        {
            string sql = "select MaHV, TenHV, DK.MaLop, LH.TenLop from DMLopHoc LH inner join MTDK DK on LH.MaLop = DK.MaLop " +
                "where isKT = '0' and  MaCN = '" + Config.GetValue("MaCN").ToString() + "'";
            if (txtTenHV.EditValue != null)
                sql += " and DK.TenHV like N'%" + txtTenHV.EditValue.ToString() + "%'";
            sql += " and IsNghiHoc = '0' {0} ";
            sql = String.Format(sql, showAll.Checked ? " " : " and isBL = '0' ");
            sql += " order by DK.tenhv desc";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count == 0)
            {
                XtraMessageBox.Show("Không tìm thấy học viên hoặc lớp của học viên đã kết thúc!", Config.GetValue("PackageName").ToString());
                return;
            }
            lookupLop.Properties.DataSource = dt;
            lookupLop.Properties.DisplayMember = "MaLop";
            lookupLop.Properties.ValueMember = "MaLop";
            lookupLop.EditValue = dt.Rows[0]["MaLop"].ToString();
            
            lookupLop.Properties.BestFit();

            gridLookUpEditHV.Properties.View.Columns.Clear();
            gridLookUpEditHV.Properties.View.OptionsBehavior.AutoPopulateColumns = false;
            gridLookUpEditHV.Properties.View.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
            gridLookUpEditHV.Properties.PopupBorderStyle = DevExpress.XtraEditors.Controls.PopupBorderStyles.NoBorder;

            //gridLookUpEditHV.Properties.View.Columns.Clear();
            gridLookUpEditHV.Properties.DataSource = dt;
            gridLookUpEditHV.Properties.DisplayMember = "TenHV";
            gridLookUpEditHV.Properties.ValueMember = "MaHV";

            GridColumn gcMaHV = new GridColumn();
            gcMaHV.Caption = "Mã học viên";            
            gcMaHV.FieldName = "MaHV";
            
            GridColumn gcTenHV = new GridColumn();
            gcTenHV.Caption = "Tên học viên";
            gcTenHV.FieldName = "TenHV";
            
            GridColumn gcMaLop = new GridColumn();
            gcMaLop.Caption = "Mã lớp";
            gcMaLop.FieldName = "MaLop";
            
            GridColumn gcTenLop = new GridColumn();
            gcTenLop.Caption = "Tên lớp";
            gcTenLop.FieldName = "TenLop";
            

            gridLookUpEditHV.Properties.View.Columns.Add(gcMaHV);
            gridLookUpEditHV.Properties.View.Columns.Add(gcTenHV);
            gridLookUpEditHV.Properties.View.Columns.Add(gcMaLop);
            gridLookUpEditHV.Properties.View.Columns.Add(gcTenLop);

            gridLookUpEditHV.Properties.View.Columns["MaHV"].Caption = "Mã học viên";
            gridLookUpEditHV.Properties.View.Columns["TenHV"].Caption = "Tên học viên";
            gridLookUpEditHV.Properties.View.Columns["MaLop"].Caption = "Mã lớp";
            gridLookUpEditHV.Properties.View.Columns["TenLop"].Caption = "Tên lớp";    

            gridLookUpEditHV.Properties.View.PopulateColumns();
            gridLookUpEditHV.Properties.View.BestFitColumns();
            gridLookUpEditHV.Properties.PopupFormWidth = 500;
        }

        int SoNgayDu(DateTime ngayBD, DateTime ngayKT, DateTime ngayDK, string magio, string ngayBDNghi, string NgayKTNghi)
        {
            int count = 0;
            ngayDK = ngayDK.AddDays(1);
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

        /*decimal TienBL(DateTime ngayDK, DateTime ngayBL, string mahv, string malop)
        {
            string sql = "select NgayBDKhoa, NgayKTKhoa, BDNghi, KTNghi, MaGioHoc from DMLopHoc where MaLop='" + malop + "'";
            DataTable dtLop = db.GetDataTable(sql);                        
            if (dtLop.Rows.Count == 0)
                return 0;
            decimal tienBL = 0;
            if (dtLop.Rows[0]["NgayBDKhoa"].ToString() != "" && dtLop.Rows[0]["NgayKTKhoa"].ToString() != "")
            {
                // % giảm HP + tiền nợ + tiền bảo lưu
                string query = "SELECT ISNULL(Sum(isnull(Ttien,0)),0) TT FROM MT11 WHERE MaNV = 'HP' AND MaKH = '" + mahv + "'";
                DataTable dtTien = new DataTable();
                dtTien = db.GetDataTable(query);
                sql = "select * from mtdk where mahv ='" + mahv + "'";
                DataTable dt = db.GetDataTable(sql);
                decimal giamHP = 0, tienNo = 0, ThucThu = 0, SoBuoiCL = 0, BLTruoc = 0;//, tienConDu = 0;
                if (dt.Rows.Count > 0)
                {
                    giamHP = decimal.Parse(dt.Rows[0]["GiamHP"].ToString());
                    tienNo = decimal.Parse(dt.Rows[0]["ConLai"].ToString());
                    //tienConDu = decimal.Parse(dt.Rows[0]["BLSoTien"].ToString());
                    BLTruoc = decimal.Parse(dt.Rows[0]["BLTruoc"].ToString());
                    SoBuoiCL = decimal.Parse(dt.Rows[0]["SoBuoiCL"].ToString());
                }
                if (dtTien.Rows.Count > 0)
                    ThucThu = (decimal) dtTien.Rows[0]["TT"] + BLTruoc;
                //học phí chuẩn
                sql = " select HPNL.HocPhi, l.sobuoi  " +
                      " from dmlophoc l inner join dmhocphi hp on l.MaNLop=hp.MaNL  " +
                      " inner join HPNL on HPNL.HPID=hp.HPID " +
                      " inner join DMNhomLop NL on NL.MaNLop=hp.MaNL " +
                      " where l.MaLop='" + malop + "' " +
                      " and HPNL.NgayBD <='" + ngayDK.ToString() + "' order by HPNL.NgayBD DESC ";
                dt = db.GetDataTable(sql);
                decimal HPChuan = 0, sobuoiQD = 0; // so buoi quy dinh
                if (dt.Rows.Count > 0)
                {
                    HPChuan = decimal.Parse(dt.Rows[0]["HocPhi"].ToString());
                    if (giamHP != 0)
                        HPChuan = HPChuan - (HPChuan * giamHP) / 100;
                    sobuoiQD = decimal.Parse(dt.Rows[0]["SoBuoi"].ToString());
                    if (ThucThu > HPChuan) ThucThu = HPChuan; //Công thêm 5/6/2019 cho trường hợp thực thu (đã cộng BLTruoc) > học phí phải nộp
                    HPChuan = HPChuan / sobuoiQD;                    
                }
                string magio = dtLop.Rows[0]["MaGioHoc"].ToString();
                if (magio != "" && magio.Length > 1)
                    magio = magio.Substring(0,1);
                  // Số buổi học viên đã học
                    DataTable dt1 = db.GetDataTable(string.Format("exec sp_SobuoiHVdahoc '{0}','{1}' ", mahv, ngayBL));
                    decimal sbdahoc = decimal.Parse(dt1.Rows[0]["sbdahoc"].ToString());
                    // Số buổi được học
                    if (ThucThu == 0)
                    {
                       tienBL = 0;
                    }
                    else
                    {
                        decimal sbduochoc = ThucThu / HPChuan;
                        decimal sobuoiCL = sbduochoc - sbdahoc;//SoNgayDu(DateTime.Parse(dtLop.Rows[0]["NgayBDKhoa"].ToString()), DateTime.Parse(dtLop.Rows[0]["NgayKTKhoa"].ToString()), ngayBL, magio, dtLop.Rows[0]["BDNghi"].ToString(), dtLop.Rows[0]["KTNghi"].ToString());
                        //if (sobuoiCL > sobuoiQD && tienConDu == 0)  // nếu cho bảo lưu trước ngày học thì số buổi còn lại là số buổi quy định -> Công bỏ ngày 5/6/2019 vì đã thêm đk ở trên
                        //    sobuoiCL = sobuoiQD;                    // ngày 21/1/2017: thêm điều kiện tiền còn dư = 0 để bảo đảm công thức tính tiền bảo lưu đã bao gồm luôn tiền còn dư
                        tienBL = sobuoiCL * HPChuan;
                    }
                //Trừ đi số tiền nợ                                
                //tienBL -= tienNo;
                //Cộng tiền bảo lưu
                //tienBL += tienConDu;
            }
            tienBL = RoundNumber(tienBL);
            return tienBL;
        }*/

        decimal RoundNumber(decimal num)
        {
            num = num / 1000;
            num = Math.Round(num, 0, MidpointRounding.AwayFromZero);
            num *= 1000;
            return num;
        }

        private void btnHienThi_Click(object sender, EventArgs e)
        {
            if (flag)
            {
                DialogResult result = XtraMessageBox.Show("Dữ liệu thay đổi chưa được lưu.\n Bạn có muốn lưu không?",Config.GetValue("PackageName").ToString(),MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    btnOK_Click(sender,e);
                }
            }

            if (lookupLop.EditValue == null)
                return;

            if (lookupLop.EditValue.ToString() != "")
            {
                showAll.Enabled = false;

                string sql = "select * from MTDK DK inner join DMHVTV TV on DK.HVTVID=TV.HVTVID "+
                    " where DK.MaLop = '" + lookupLop.EditValue.ToString() + "' and IsNghiHoc = '0' {0}";
                sql = String.Format(sql, showAll.Checked ? " " : " and isBL = '0' ");
                
                DataTable dt = db.GetDataTable(sql);
                gcHocVien.DataSource = dt;
                gvHocVien.BestFitColumns();
                dt.RowChanged += new DataRowChangeEventHandler(dt_RowChanged);                
                flag = false;
            }
        }        

        void dt_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            flag = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DataTable dt = gcHocVien.DataSource as DataTable;
            DataView dv = new DataView(dt);
            dv.RowStateFilter = DataViewRowState.ModifiedCurrent;
            string sql = "";
            
            // Bổ sung trường hợp thêm nhân viên đăng ký bảo lưu
            string username = Config.GetValue("UserName").ToString();

            foreach (DataRowView drv in dv)
            {
             string tmp = Config.GetValue("NgayKhoaSo").ToString();
            DateTime ngayKhoa;
            DateTimeFormatInfo dtInfo = new DateTimeFormatInfo();
            dtInfo.ShortDatePattern = "dd/MM/yyyy"; 
            if (DateTime.TryParse(tmp, dtInfo, DateTimeStyles.None, out ngayKhoa))
            {
                if (drv["NgayBL"] != DBNull.Value)
                {
                    DateTime NgayBL = (DateTime)drv["NgayBL"];
                    if (NgayBL <= ngayKhoa)
                    {
                        string msg = "Kỳ kế toán đã khóa! Không thể chỉnh sửa số liệu!";
                        if (Config.GetValue("Language").ToString() == "1")
                            msg = UIDictionary.Translate(msg);
                        XtraMessageBox.Show(msg);
                        return;
                    }
                }
            }
                
                if (drv["IsBL"].ToString().ToUpper() == "TRUE" && drv["NgayBL"].ToString() != "" && drv["NgayHH"].ToString() != "")
                {
                    username = string.IsNullOrEmpty(drv["MaNVBL"].ToString()) ? username : drv["MaNVBL"].ToString();

                    //Cập nhật sỉ số đăng ký + tiền bảo lưu
                    if (drv.Row["IsBL", DataRowVersion.Original].ToString().ToUpper() != drv.Row["IsBL", DataRowVersion.Current].ToString().ToUpper())
                    {
                        //Trường hợp đăng ký vẫn còn dư tiền, giờ bảo lưu lớp đăng ký thì cộng dồn lại
                        //Tiền nợ thì cho về 0 vì đã trừ vào tiền bảo lưu.
                        sql = "Update MTDK set IsBL = '" + drv["IsBL"].ToString()
                                + "', NgayBL = '" + drv["NgayBL"].ToString()
                                + "', NgayHocCuoi = '" + drv["NgayBL"].ToString() 
                                + "', NgayHH = '" + drv["NgayHH"].ToString() 
                                + "', ConLai = '0', BLSoTien = '" + drv["BLSoTien"].ToString()
                                + "', MaNVBL = '" + username
                                + "', LyDoBL = N'" + drv["LyDoBL"].ToString()
                                + "', BLGhiChu = N'" + drv["BLGhiChu"].ToString()
                                + "' where MaHV = '" + drv["MaHV"].ToString() + "'";
                         db.UpdateByNonQuery(sql);
                        sql = "Update DMLophoc set SiSoHV = case when SiSoHV <= 0 then 0 else (SiSoHV - 1) end where MaLop = '" + drv["MaLop"].ToString() + "'";
                        db.UpdateByNonQuery(sql);
                    }
                    else
                    {   //Trường hợp bảo lưu rồi giờ sửa lại thì gán giá trị mới
                        sql = "Update MTDK set IsBL = '" + drv["IsBL"].ToString()
                            + "', NgayBL = '" + drv["NgayBL"].ToString()
                            + "', NgayHocCuoi = '" + drv["NgayBL"].ToString() 
                            + "', NgayHH = '" + drv["NgayHH"].ToString() 
                            + "', BLSoTien =  '" + drv["BLSoTien"].ToString()
                            + "', MaNVBL = '" + username
                            + "', LyDoBL = N'" + drv["LyDoBL"].ToString()
                            + "', BLGhiChu = N'" + drv["BLGhiChu"].ToString()
                            + "' where MaHV = '" + drv["MaHV"].ToString() + "'";
                         db.UpdateByNonQuery(sql);
                    }
                }
                else if (drv["IsBL"].ToString().ToUpper() == "FALSE" && drv["NgayBL"].ToString() == "" && drv["NgayHH"].ToString() == "")
                {                    
                    if (drv.Row["IsBL", DataRowVersion.Original].ToString().ToUpper() != drv.Row["IsBL", DataRowVersion.Current].ToString().ToUpper())
                    {
                        //xoa bao luu hoc phi
                        sql = string.Format(@"Update MTDK set IsBL = '{0}'
                                               ,ConLai = TienHP - ISNULL(( SELECT Sum(isnull(Ttien,0)) Ttien FROM MT11 WHERE MaNV = 'HP' AND MaKH = '{1}'),0) , NgayBL = null
                                               ,NgayHH = null, BLSoTien = '0', MaNVBL = NULL, LyDoBL = NULL, BLGhiChu = NULL
                                                WHERE MaHV = '{2}'", drv["IsBL"].ToString(), drv["MaHV"].ToString(), drv["MaHV"].ToString());
                        db.UpdateByNonQuery(sql);
                        // Tính lại ngày học cuối
                        DataTable dtHV = db.GetDataTable(string.Format("select MaLop, NgayDK, SoBuoiDH from MTDK where MaHV = '{0}'", drv["MaHV"]));
                        DataTable dtNgayKT = db.GetDataTable(string.Format("exec TinhNgayKT '{0:n0}','{1}', '{2}'", dtHV.Rows[0]["SoBuoiDH"], dtHV.Rows[0]["NgayDK"], dtHV.Rows[0]["MaLop"]));
                        if (dtNgayKT.Rows.Count > 0 && dtNgayKT.Rows[0]["NgayKT"].ToString() != "")
                            db.UpdateByNonQuery(string.Format("update MTDK set NgayHocCuoi = '{0}' where MaHV = '{1}'", dtNgayKT.Rows[0]["NgayKT"], drv["MaHV"]));
                        //Cập nhật sỉ số đăng ký
                        sql = "Update DMLophoc set SiSoHV = (SiSoHV + 1) where MaLop = '" + drv["MaLop"].ToString() + "'";
                        db.UpdateByNonQuery(sql);
                    }
                }
            }
            if (dv.Count > 0)
                XtraMessageBox.Show("Cập nhật thành công!", Config.GetValue("PackageName").ToString());
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void gvHocVien_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {                        
           
        }

        private void lookupLop_EditValueChanged(object sender, EventArgs e)
        {
            showAll.Enabled = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            getDSLop();
        }

        private void gridLookUpEditHV_EditValueChanged(object sender, EventArgs e)
        {                        
            if (gridLookUpEditHV.Properties.View.IsDataRow(gridLookUpEditHV.Properties.View.FocusedRowHandle))
            {                
                string sql = "select * from mtdk where mahv = '" + gridLookUpEditHV.EditValue.ToString() + "'";
                DataTable dt = db.GetDataTable(sql);
                gcHocVien.DataSource = dt;
                gvHocVien.BestFitColumns();                
            }
        }

        private void gvHocVien_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName.ToUpper().Equals("ISBL"))
            {
                if (e.Value.ToString().ToUpper() == "TRUE")
                {
                    gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["NgayBL"], DateTime.Today.ToString());
                    DateTime dtp;
                    int songay = 0;
                    if (Config.GetValue("HanBaoLuu") != null)
                    {
                        songay = int.Parse(Config.GetValue("HanBaoLuu").ToString());
                        dtp = DateTime.Today.AddDays(songay);
                        gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["NgayHH"], dtp);
                    }
                    //tiền bảo lưu
                    if (gvHocVien.GetFocusedRowCellValue("NgayDK").ToString() != "" && gvHocVien.GetFocusedRowCellValue("NgayBL").ToString() != "" && gvHocVien.GetFocusedRowCellValue("MaHV").ToString() != "")
                    {
                        HocPhi hp = new HocPhi();
                        decimal sotien = hp.TienConLai(DateTime.Parse(gvHocVien.GetFocusedRowCellValue("NgayBL").ToString()), gvHocVien.GetFocusedRowCellValue("MaHV").ToString());
                        //gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["SoBuoiCL"], sobuocl);
                        gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["BLSoTien"], sotien);
                    }
                }
                else
                {
                    gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["NgayBL"], null);
                    gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["NgayHH"], null);
                    gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["BLSoTien"], 0);
                    gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["LyDoBL"], string.Empty);
                    gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["BLGhiChu"], string.Empty);
                }
            }
            if (e.Column.FieldName.ToUpper().Equals("NGAYBL"))
            {
                string isBL = gvHocVien.GetFocusedRowCellValue("IsBL").ToString();
                if (isBL.ToUpper() == "FALSE")
                    gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["NgayHH"], null);
                else
                {
                    DateTime dtp;
                    int songay = 0;
                    if (Config.GetValue("HanBaoLuu") != null)
                    {
                        songay = int.Parse(Config.GetValue("HanBaoLuu").ToString());
                        dtp = DateTime.Parse(e.Value.ToString());
                        dtp = dtp.AddDays(songay);
                        gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["NgayHH"], dtp);
                    }
                    //tiền bảo lưu
                    if (gvHocVien.GetFocusedRowCellValue("NgayDK").ToString() != "" && e.Value.ToString() != "" && gvHocVien.GetFocusedRowCellValue("MaHV").ToString() != "")
                    {
                        HocPhi hp = new HocPhi();
                        decimal sotien = hp.TienConLai(DateTime.Parse(e.Value.ToString()), gvHocVien.GetFocusedRowCellValue("MaHV").ToString());
                        gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["BLSoTien"], sotien);
                        //gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["SoBuoiCL"], sobuocl);
                    }
                }
            }
        }                     
    }
}