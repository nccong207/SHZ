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
namespace HoiThamHV
{
    public partial class frmHVDK : DevExpress.XtraEditors.XtraForm
    {
        public frmHVDK()
        {
            InitializeComponent();
        }

        Database db = Database.NewDataDatabase();

        private void frmHVDK_Load(object sender, EventArgs e)
        {
            getPhanHoi();
            getDSLop();
        }

        void getDSLop()
        {           
            string sql = "select * from DMLopHoc where SisoHV > 0 ";
            if (Config.GetValue("MaCN") != null)
                sql += " and MaCN = '"+Config.GetValue("MaCN").ToString()+"'";
            sql += " order by NgayBDKhoa desc";
            DataTable dt = db.GetDataTable(sql);
            gridLookUpLop.Properties.DataSource = dt;
            gridLookUpLop.Properties.ValueMember = "MaLop";
            gridLookUpLop.Properties.DisplayMember = "TenLop";
            gridLookUpEdit1View.BestFitColumns();
        }

        void getPhanHoi()
        {
            string sql = "select * from DMPhanhoi";
            DataTable dt = db.GetDataTable(sql);
            //DataRow dr = dt.NewRow();
            //dt.Rows.Add(dr);
            repositoryTTPH.DataSource = dt;
            repositoryTTPH.DisplayMember = "PHoi";
            repositoryTTPH.ValueMember = "PHID";
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            DataTable dt = gcHocVien.DataSource as DataTable;
            if (dt == null)
                return;
            string sql = "";
            DataView dv = new DataView(dt);
            dv.RowStateFilter = DataViewRowState.ModifiedCurrent;
            bool flg = false;
            int i = 0;
            foreach (DataRowView drv in dv)
            {
                i++;
                if (drv["PhanHoiID"] == DBNull.Value || drv["NgayHT2"] == DBNull.Value)
                {
                    XtraMessageBox.Show(string.Format("Dòng {0} chưa nhập thông tin {1}!"
                                , i, drv["PhanHoiID"] == DBNull.Value ? "phản hồi" : "ngày hỏi thăm")
                                , Config.GetValue("PackageName").ToString());
                    return;
                }
                if (drv["PhanHoiID"].ToString() != "")
                {
                    sql += ";update MTDK set PhanhoiID = '" + drv["PhanhoiID"].ToString() + "', NgayHT = '" + drv["NgayHT2"].ToString()
                        + "', GhiChu = N'" + drv["GhiChu"].ToString() + "', KLL = N'" + drv["KLL"].ToString() + "' where MaHV = '" + drv["MaHV"].ToString() + "'";

                    
                    string HVTVID = drv["HVTVID"].ToString();
                    string MaLop = drv["MaLop"].ToString();
                    string PHID = drv["PhanHoiID"].ToString();
                    string KHTT = drv["KHTT"].ToString();
                    string MaNV = Config.GetValue("username").ToString();
                    string GhiChu = drv["GhiChu"].ToString();
                    //DateTime NgayTH = DateTime.Parse(drv["NgayTH"].ToString());
                    DateTime NgayHoiTT = DateTime.Parse(drv["NgayHT2"].ToString());

                    if (drv["NgayTH"].ToString() != "")
                        sql += string.Format(@";Insert DMQTCSHV(HVTVID,Ngay,MaLop,MaNV,PHID,KHTT,NgayTH,GhiChu) Values({0},'{1}','{2}','{3}',{4},N'{5}','{6}',N'{7}')"
                                              , HVTVID, NgayHoiTT, MaLop, MaNV, PHID, KHTT, DateTime.Parse(drv["NgayTH"].ToString()),GhiChu);
                    else
                        sql += string.Format(@";Insert DMQTCSHV(HVTVID,Ngay,MaLop,MaNV,PHID,KHTT,NgayTH,GhiChu) Values({0},'{1}','{2}','{3}',{4},N'{5}',null,N'{6}')"
                                          , HVTVID, NgayHoiTT, MaLop, MaNV, PHID, KHTT,GhiChu);
                }
                else
                    sql = ";update MTDK set PhanhoiID = null , NgayHT = '" + drv["NgayHT2"].ToString() + "', GhiChu = N'"
                        + drv["GhiChu"].ToString() + "', KLL = N'" + drv["KLL"].ToString() + "' where MaHV = '" + drv["MaHV"].ToString() + "'";
                
            }
            db.BeginMultiTrans();
            flg = db.UpdateByNonQuery(sql);
            if (flg)
            {
                db.EndMultiTrans();
                dt.AcceptChanges();
                XtraMessageBox.Show("Cập nhật thành công!", Config.GetValue("PackageName").ToString());
                // this.Close();
            }
            else
            {
                db.RollbackMultiTrans();
                XtraMessageBox.Show("Chưa cập nhật thông tin hỏi thăm cho bất kỳ học viên nào.", Config.GetValue("PackageName").ToString());
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnXemHVDK_Click(object sender, EventArgs e)
        {
            /*select distinct hvtvid
				            ,(select top 1 NgayTH from dmqtcshv where hvtvid = dm.hvtvid order by Ngay desc) [NgayTH]
                            into	#tmpQTCS
                            from	dmqtcshv dm 

                            select	cs.hvtvid,	case when NgayTH is null then 
							    (
							        select top 1 id from dmqtcshv where hvtvid = dm.hvtvid order by Ngay desc
							    ) 
					            else	(
							        select top 1 id from dmqtcshv where hvtvid = dm.hvtvid order by NgayTH desc,Ngay desc
							    ) end [id]
                                into	#tmpQTCS1
                            from	dmhvtv dm inner join #tmpQTCS cs on dm.hvtvid = cs.hvtvid */
            string sql = @" select HVTVID, max(id) as ID into #tmpQTCS from DMQTCSHV group by HVTVID

                            select qt.* INTO #ttam FROM DMQTCSHV qt inner join #tmpQTCS t on qt.id = t.id

                            select MT.MaHV ,tv.TenHV,TV.DienThoai,mt.HVID,tv.hvtvid ,ttam.KHTT, ttam.NgayTH,NgayHoiTT
		                    ,ttam.GhiChu  ,mt.MaLop,GioiTinh ,TV.DiaChi,TV.Email, PhanHoiID = CASE WHEN ttam.PHID is null THEN NULL ELSE ttam.PHID END
                            ,NguonTT,MT.MaCNDK,MT.MaCNHoc,NV1.HoTen [HoTen1],NV.HoTen,mt.NgayDK, mt.KLL
		                    ,TrinhDo ,DanToc ,MucDich, NguonHVM = case when MT.NguonHV ='0' then N'HV mới' when MT.NguonHV ='1' then N'HV cũ' when MT.NguonHV ='2' then N'HV bảo lưu' end ,
                             case when ttam.Ngay is null then null else ttam.Ngay end as NgayHT2
                             from MTDK MT inner join DMHVTV TV on MT.HVTVID=TV.HVTVID 
                             left join MTDK DK on MT.MaHV = DK.MaHVDK
                             left join DMMDHoc MD on MD.MDID=TV.MucDichID
                             left join  DMNguonTT TT on TT.NguonID = TV.NguonID
                             left join  DMDantoc DT on TV.DanTocID = DT.DTocID
                             LEFT JOIN DMNVien NV on MT.NVCS = NV.MaNV
                             left JOIN DMNVien NV1 on tv.MaNVTV = NV1.MaNV
                             LEFT JOIN #ttam ttam ON TV.HVTVID = ttam.HVTVID --AND TV.NgayHoiTT=ttam.Ngay
                             WHERE DK.MaHVDK is null and MT.IsNghiHoc = '0' and MT.isBL = 0";
            if (gridLookUpLop.EditValue != null && gridLookUpLop.EditValue.ToString() != "")
                sql += " AND MT.MaLop ='" + gridLookUpLop.EditValue.ToString() + "'";
//                sqlText = "WHERE  MT.MaLop ='" + gridLookUpLop.EditValue.ToString() + @"' 
//                       and MT.MaHV not in  (select mahvdk from mtdk where mahvdk is not null and mahvdk like '" + gridLookUpLop.EditValue.ToString() + @"%')";
//            else
//                sqlText = "WHERE MT.MaHV not in (select mahvdk from mtdk where mahvdk is not null)";
            else
                sql += " AND MT.MaCNHoc = '" + Config.GetValue("MaCN").ToString() + "'";

            sql += @"  ORDER BY MT.NgayDK desc, MT.MaLop, tv.TenHV 
                        DROP TABLE #ttam 
                        drop table #tmpQTCS";
                        //drop table #tmpQTCS1";
            DataTable dt = db.GetDataTable(sql);
            gcHocVien.DataSource = dt;
            gvHocVien.BestFitColumns();      
        }

        private void gridLookUpLop_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                gridLookUpLop.EditValue = "";
            }
        }

        private void gcHocVien_Click(object sender, EventArgs e)
        {

        }

        private void btnDiemThi_Click(object sender, EventArgs e)
        {
            int[] indexs = gvHocVien.GetSelectedRows();
            if (indexs.Length == 0)
            {
                XtraMessageBox.Show("Vui lòng chọn học viên để xem " + btnDiemThi.Text, Config.GetValue("PackageName").ToString());
                return;
            }
            string HVID = gvHocVien.GetRowCellValue(gvHocVien.GetSelectedRows()[0], "HVID").ToString();
            
            frmBCDiemThi frm = new frmBCDiemThi();
            frm.Text = "Danh sách điểm thi";
            frm.HVID = HVID;
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }

        private void btnCSHV_Click(object sender, EventArgs e)
        {
            int[] indexs = gvHocVien.GetSelectedRows();
            if (indexs.Length == 0)
            {
                XtraMessageBox.Show("Vui lòng chọn học viên để xem " + btnCSHV.Text, Config.GetValue("PackageName").ToString());
                return;
            }
            string HVTVID = gvHocVien.GetRowCellValue(indexs[0], "hvtvid").ToString();

            frmBCChamSocHV frm = new frmBCChamSocHV();
            frm.Text = "Tình hình chăm sóc học viên";
            frm.HVTVID = HVTVID;
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }

        private void btnQTDK_Click(object sender, EventArgs e)
        {
            int[] indexs = gvHocVien.GetSelectedRows();
            if (indexs.Length == 0)
            {
                XtraMessageBox.Show("Vui lòng chọn học viên để xem " + btnQTDK.Text, Config.GetValue("PackageName").ToString());
                return;
            }
            string HVTVID = gvHocVien.GetRowCellValue(indexs[0], "hvtvid").ToString();

            frmBCQuaTrinhDK frm = new frmBCQuaTrinhDK();
            frm.Text = "Quá trình đăng ký";
            frm.HVTVID = HVTVID;
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            gcHocVien.ShowPrintPreview();
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            txtSearch.Properties.Buttons[0].Visible = txtSearch.Text.Trim() != "";
        }

        private void txtSearch_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            txtSearch.Properties.Buttons[0].Visible = false;
            txtSearch.Text = "Tìm nhanh...";
            gvHocVien.ActiveFilterString = string.Empty;
        }

        private void txtSearch_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            string s = e.NewValue.ToString().Trim();
            if (s == "Tìm nhanh..." || s.Length < 3)
            {
                gvHocVien.ActiveFilterString = string.Empty;
            }
            else
            {
                gvHocVien.ActiveFilterString = "TenHV Like '%" + s + "%'";
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Tìm nhanh...")
                txtSearch.Text = "";
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (txtSearch.Text == "")
                txtSearch.Text = "Tìm nhanh...";
            else
                if (txtSearch.Text.Length < 3)
                    XtraMessageBox.Show("Vui lòng nhập tối thiểu 3 ký tự để tìm kiếm!");
        } 
    }
}