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
using System.Globalization;

namespace HVNghiHoc
{
    public partial class frmHocVien : DevExpress.XtraEditors.XtraForm
    {        
        public frmHocVien()
        {
            InitializeComponent();     
        }
        Database db = Database.NewDataDatabase();        
        private void frmHocVien_Load(object sender, EventArgs e)
        {
            dateEdit.EditValue = System.DateTime.Now;
            getDSLop();
        }
        bool hasChanged = false;

        //lấy lớp đang học
        void getDSLop()
        {            
            //string sql = "select * from dmLophoc where (month(NgayBDKhoa) >= '" + Thang.ToString() + "' and month(NgayKTKhoa) >= '" + Thang.ToString() + "'" + "and year(NgayBDKhoa) = '" + nam + "')" +
            //            " or (month(NgayBDKhoa) >= '" + Thang.ToString() + "' and month(NgayKTKhoa) <= '" + Thang.ToString() + "'" + "and year(NgayBDKhoa) >= '" + nam + "')";
            //string sql = "select * from DMLopHoc LH where ('"+ dateEdit.EditValue.ToString() +"' between NgayBDKhoa and NgayKTKhoa) and MaCN = '"+Config.GetValue("MaCN").ToString()+"'";            
            string sql = "select * from DMLopHoc where isKT = '0' and  MaCN = '"+Config.GetValue("MaCN").ToString()+"'";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count == 0)
            {
                XtraMessageBox.Show("Không có lớp nào đang học trong khoảng thời gian này!",Config.GetValue("PackageName").ToString());
                return;
            }
            gluMaLop.Properties.DataSource = dt;
            gluMaLop.Properties.DisplayMember = "MaLop";
            gluMaLop.Properties.ValueMember = "MaLop";
            gluMaLop.Properties.View.BestFitColumns();
        }        

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            DataTable dt = gcHocVien.DataSource as DataTable;
            if (dt == null)
                return;            
            foreach (DataRow row in dt.Rows)
            {
                row["IsNghiHoc"] = chkAll.Checked;
                if (chkAll.Checked)
                    row["NgayNghi"] = DateTime.Today;
                else
                    row["NgayNghi"] = DBNull.Value;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string tmp = Config.GetValue("NgayKhoaSo").ToString();
            DateTime ngayKhoa;
            DateTimeFormatInfo dtInfo = new DateTimeFormatInfo();
            dtInfo.ShortDatePattern = "dd/MM/yyyy";
            DataTable dt = gcHocVien.DataSource as DataTable;
            //db.UpdateDataTable(sqlUpdate,dt);
            DataView dv = new DataView(dt);
            dv.RowStateFilter = DataViewRowState.ModifiedCurrent;
            foreach (DataRowView dr in dv)
            {
                if (dr["NgayNghi"].ToString() != "")
                {
                    DateTime NgayNghi = (DateTime)dr["NgayNghi"];
                    if (DateTime.TryParse(tmp, dtInfo, DateTimeStyles.None, out ngayKhoa))
                    {
                        if (NgayNghi <= ngayKhoa)
                        {
                            string msg = "Kỳ kế toán đã khóa! Không thể chỉnh sửa số liệu!";
                            if (Config.GetValue("Language").ToString() == "1")
                                msg = UIDictionary.Translate(msg);
                            XtraMessageBox.Show(msg);
                            return;
                        }
                    }
                }
                else
                {
                    DataTable dt1 = db.GetDataTable(string.Format("SELECT * FROM MTDK WHERE MaHV = '{0}' AND isNghiHoc = 1",dr["MaHV"].ToString()));
                    if (dt1.Rows.Count > 0)
                    {
                        if (DateTime.TryParse(tmp, dtInfo, DateTimeStyles.None, out ngayKhoa))
                        {
                            DateTime NgayNghi = (DateTime)dt1.Rows[0]["NgayNghi"];
                            if (NgayNghi <= ngayKhoa)
                            {
                                string msg = "Kỳ kế toán đã khóa! Không thể chỉnh sửa số liệu!";
                                if (Config.GetValue("Language").ToString() == "1")
                                    msg = UIDictionary.Translate(msg);
                                XtraMessageBox.Show(msg);
                                return;
                            }
                        }
                    }

                 }
            }
            string sql = "";
            foreach (DataRowView drv in dv)
            {                
                if (drv["IsNghiHoc"].ToString().ToUpper() == "TRUE" && drv["NgayNghi"].ToString() != "")
                {
                    //Cập nhật sỉ số
                    sql = "select * from mtdk where mahv = '" + drv["MaHV"].ToString() + "' and (isBL = '1' or isNghiHoc = '1')";
                    dt = db.GetDataTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        sql = "Update DMLophoc set SiSoHV = case when SiSoHV <= 0 then 0 else (SiSoHV - 1) end where MaLop = '" + drv["MaLop"].ToString() + "'";
                        db.UpdateByNonQuery(sql);
                    }                      
                    sql = "Update MTDK set IsNghiHoc = '" + drv["IsNghiHoc"].ToString() + "', NgayNghi = '" + drv["NgayNghi"].ToString() + "' where MaHV = '" + drv["MaHV"].ToString() + "'";
                    db.UpdateByNonQuery(sql);
                    
                }
                else if (drv["IsNghiHoc"].ToString().ToUpper() == "FALSE" )
                {
                    //Cập nhật sỉ số
                    sql = "select * from mtdk where mahv = '" + drv["MaHV"].ToString() + "' and (isBL = '1' or isNghiHoc = '1')";
                    dt = db.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        sql = "Update DMLophoc set SiSoHV = (SiSoHV + 1) where MaLop = '" + drv["MaLop"].ToString() + "'";
                        db.UpdateByNonQuery(sql);
                    }                                        
                    sql = "Update MTDK set IsNghiHoc = '" + drv["IsNghiHoc"].ToString() + "', NgayNghi = null where MaHV = '" + drv["MaHV"].ToString() + "'";
                    db.UpdateByNonQuery(sql);
                }
            }
            XtraMessageBox.Show("Cập nhật thành công!",Config.GetValue("PackageName").ToString());
            hasChanged = false;
        }

        private void btnHienthi_Click(object sender, EventArgs e)
        {
            if (hasChanged)
            {
                DialogResult result = XtraMessageBox.Show("Dữ liệu của lớp trước chưa được lưu!\n Bạn có muốn lưu hay không?",
                    Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    btnOK_Click(sender, e);
                }
            }
            
            //if (gluMaLop.EditValue != null)
                GetHocVien();
            hasChanged = false;
        } 

        void GetHocVien()
        {
            //ghi chú: đã nộp = Nộp khi đăng ký + nộp = phiếu thu
            string sql = " select MT.MaLop, MT.MaHV, MT.TenHV as HoTen, MT.TienHP + MT.BLTruoc as PhaiNop, (MT.TienHP + MT.BLTruoc - MT.ConLai) as DaNop, MT.ConLai as HPConLai, " +
                         " 0.0 as DuocHoc, 0.0 as DaHoc, 0.0 as ConLai, SoBuoiCL, LH.NgayBDKhoa, LH.NgayKTKhoa, "+
                         " LH.BDNghi, LH.KTNghi, LH.MaGioHoc, MT.IsNghiHoc, MT.NgayNghi "+
                         //" from MTDK MT inner join DMHVTV TV ON MT.HVTVID=TV.HVTVID " +
                         " from MTDK MT inner join DMLopHoc LH on LH.MaLop=MT.MaLop " +
                         " inner join DMNhomLop NL on NL.MaNLop = LH.MaNLop  " +
                         " where isBL = 0 and isChuyenLop = 0  and MT.MaCNHoc = '"+Config.GetValue("MaCN").ToString()+"' ";
            string sqlText = "";
            if (gluMaLop.EditValue != null && gluMaLop.EditValue.ToString() != "")
                sqlText = " and MT.MaLop = '" + gluMaLop.EditValue.ToString() + "' ";
            if (!showAll.Checked)
                sqlText += " and MT.isNghiHoc = '0' ";
            if (sqlText != "")
                sql += sqlText;
            DataTable dt = db.GetDataTable(sql);
            decimal HPbuoi = 0, duochoc = 0, dahoc = 0, conlai = 0, danop = 0, sobuoi = 0;            
            foreach (DataRow row in dt.Rows)
            {
                //tinh toan
                sobuoi = decimal.Parse(row["SoBuoiCL"].ToString());
                if (sobuoi == 0)
                    continue;
                HPbuoi = decimal.Parse(row["PhaiNop"].ToString()) / sobuoi ;
                danop = decimal.Parse(row["DaNop"].ToString());
                if ((danop == 0 && HPbuoi == 0) || HPbuoi == 0)
                    continue;
                //duochoc = danop / HPbuoi;
                string magio = row["MaGioHoc"].ToString();
                if (magio != "")
                    magio = magio.Substring(0,1);
                // Số buổi học viên đã học
                DataTable dt1 = db.GetDataTable(string.Format("exec sp_SobuoiHVdahoc '{0}','{1}' ", row["MaHV"].ToString(), DateTime.Parse(dateEdit.EditValue.ToString())));
                dahoc = decimal.Parse(dt1.Rows[0]["sbdahoc"].ToString());
                // Số buổi được học
                duochoc= danop / HPbuoi;
                //dahoc = //SoNgay(DateTime.Parse(row["NgayBDKhoa"].ToString()), DateTime.Parse(row["NgayKTKhoa"].ToString()),DateTime.Parse(dateEdit.EditValue.ToString()), magio, row["BDNghi"].ToString(), row["KTNghi"].ToString());
                duochoc = Math.Round(duochoc, 1);
                dahoc = Math.Round(dahoc, 1);                
                conlai = duochoc - dahoc;
                conlai = Math.Round(conlai,1);
                                
                //cap nhat
                row["DuocHoc"] = duochoc;
                row["DaHoc"] = dahoc;
                row["ConLai"] = conlai;
            }                                                         
          
            gcHocVien.DataSource = dt;
            gvHocVien.BestFitColumns();

            dt.RowChanged += new DataRowChangeEventHandler(dt_RowChanged);            
            
        }

        void dt_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            hasChanged = true;
        }

        int SoNgay(DateTime ngayBD, DateTime ngayKT, DateTime ngayDK, string magio, string ngayBDNghi, string NgayKTNghi)
        {
            int count = 0;
            if (ngayBDNghi != "" && NgayKTNghi != "")
            {
                DateTime ngayBDN = DateTime.Parse(ngayBDNghi);
                DateTime ngayKTN = DateTime.Parse(NgayKTNghi);
                for (DateTime dtp = ngayBD; dtp <= ngayDK; dtp = dtp.AddDays(1))
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
                    else if (magio == "B") //7, CN
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
                for (DateTime dtp = ngayBD; dtp <= ngayDK; dtp = dtp.AddDays(1))
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
        
        private void gvHocVien_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName.ToUpper().Equals("ISNGHIHOC"))
            {
                if (e.Value.ToString().ToUpper() == "TRUE")
                    gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["NgayNghi"], dateEdit.EditValue.ToString());
                else
                    gvHocVien.SetFocusedRowCellValue(gvHocVien.Columns["NgayNghi"], null);
            }
        }

        private void gluMaLop_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                gluMaLop.EditValue = null;
        }
    }
}