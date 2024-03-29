using System;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTLib;
using CDTDatabase;

namespace HoiThamHV
{
    public partial class frmHVTV : DevExpress.XtraEditors.XtraForm
    {
        public frmHVTV()
        {
            InitializeComponent();
        }
        Database db = Database.NewDataDatabase();        

        private void frmHVTV_Load(object sender, EventArgs e)
        {
            getPhanHoi();
            getNhomLop();
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

        void getNhomLop()
        {
            string sql = @"select NL.MaNLop, NL.TenNLop from ( 
                         select MaNLop as MaNLop from dmhvtv 
                         where isMoi='1' and MaNLop is not null and MaCN = '" + Config.GetValue("MaCN").ToString() + @"' 
                         union all 
                         select MaNhomLop2 as MaNLop from dmhvtv 
                         where isMoi='1' and MaNhomLop2 is not null and MaCN = '" + Config.GetValue("MaCN").ToString() + @"' 
                         ) x inner join DMNhomLop NL on x.MaNLop = NL.MaNLop group by NL.MaNLop, NL.TenNLop order by NL.MaNLop ASC";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count == 0)
            {
                XtraMessageBox.Show("Không tìm thấy nhóm lớp nào!", Config.GetValue("PackageName").ToString());
                return;
            }
            DataRow row = dt.NewRow();
            row["TenNLop"] = "Rỗng";
            dt.Rows.InsertAt(row,0);
            lookUpNhomLop.Properties.DataSource = dt;
            lookUpNhomLop.Properties.DisplayMember = "MaNLop";
            lookUpNhomLop.Properties.ValueMember = "MaNLop";
            lookUpNhomLop.EditValue = dt.Rows[0]["MaNLop"].ToString();                   
        }

        private void lookUpNhomLop_EditValueChanged(object sender, EventArgs e)
        {
            string sql = @" SELECT	cs.HVTVID, MAX(cs.ID) ID
                            INTO    #tmpQTCS
                            FROM	DMQTCSHV cs INNER JOIN DMHVTV dm on cs.HVTVID = dm.HVTVID
                            GROUP BY cs.HVTVID

                            select qt.* INTO #ttam FROM DMQTCSHV qt inner join #tmpQTCS t on qt.id = t.id

                            SELECT	tv.HVTVID, NV.HoTen,NV1.HoTen [HoTen1], tv.MaLop, tgh.DienGiai [TGMongMuon], ttam.*, tv.*, tt.*, kll.MucDich
                            FROM    DMHVTV TV 
                                    LEFT JOIN MTDK DK on TV.HVTVID = DK.HVTVID
                                    LEFT JOIN DMNguonTT TT on TV.NguonID=TT.NguonID 
                                    LEFT JOIN DMNVien NV on TV.MaNVTV = NV.MaNV
                                    LEFT JOIN DMNVien NV1 on tv.NVCS = NV1.MaNV
		                            LEFT JOIN #ttam ttam ON TV.HVTVID = ttam.HVTVID --AND TV.NgayHoiTT=ttam.Ngay
                                    LEFT JOIN DMMDHoc kll on TV.KenhLLID = kll.MDID
                                    LEFT JOIN DMTGMuonHoc tgh on TV.TGMuonHoc = tgh.ID
                            WHERE    isMoi = '1' AND DK.HVTVID IS NULL AND year(NgayTV) >= 2016

                            ";
            string sqlText = "";
            if (lookUpNhomLop.EditValue.ToString() != "")
                sqlText = "and ( MaNLop = '" + lookUpNhomLop.EditValue.ToString() + "' or MaNhomLop2 = '" + lookUpNhomLop.EditValue.ToString() + "') ";
            if (Config.GetValue("MaCN") != null)
                sql += " and TV.MaCN = '" + Config.GetValue("MaCN").ToString() + "'";
            if (sqlText != "")
                sql += sqlText;
            sql += @"   DROP TABLE #ttam 
                        drop table #tmpQTCS";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count == 0)
            {
                XtraMessageBox.Show("Không có học viên tư vấn nào trong nhóm lớp này!",Config.GetValue("PackageName").ToString());
                return;
            }
            gcHocvien.DataSource = dt;
            gvHocVien.BestFitColumns();                            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DataTable dt = gcHocvien.DataSource as DataTable;            
            if (dt == null)
                return;
            string sql = "";
            DataView dv = new DataView(dt);
            dv.RowStateFilter = DataViewRowState.ModifiedCurrent;
            bool flg = false;
            int i = 0;
            foreach (DataRowView drv in dv)
            {
                // Cập nhật ngày chỉnh sửa mới nhất: y/cầu 6/6/2013
                i++;
                if (drv["PHID"] == DBNull.Value || drv["Ngay"] == DBNull.Value)
                {
                    XtraMessageBox.Show(string.Format("Dòng {0} chưa nhập thông tin {1}!"
                                , i, drv["PHID"] == DBNull.Value ? "phản hồi" : "ngày hỏi thăm")
                                , Config.GetValue("PackageName").ToString());
                    return;
                }
                    string HVTVID = drv["HVTVID"].ToString();
                    string MaLop = drv["MaLop"].ToString();
                    string PHID = drv["PHID"].ToString();
                    string KHTT = drv["KHTT"].ToString();
                    string MaNV = Config.GetValue("username").ToString();
                    string GhiChu = drv["GhiChu"].ToString();
                    DateTime NgayHoiTham = DateTime.Parse(drv["Ngay"].ToString());

                    if (drv["NgayTH"].ToString() != "")
                        sql += string.Format(@";Insert DMQTCSHV(HVTVID,Ngay,MaLop,MaNV,PHID,KHTT,NgayTH,GhiChu) Values({0},'{1}','{7}','{2}',{3},N'{4}','{5}',N'{6}')", HVTVID, NgayHoiTham, MaNV, PHID, KHTT, DateTime.Parse(drv["NgayTH"].ToString()),GhiChu,MaLop);
                    else
                        sql += string.Format(@";Insert DMQTCSHV(HVTVID,Ngay,MaLop,MaNV,PHID,KHTT,NgayTH,GhiChu) Values({0},'{1}','{6}','{2}',{3},N'{4}',null,N'{5}')", HVTVID, NgayHoiTham, MaNV, PHID, KHTT,GhiChu,MaLop);
            }
            db.BeginMultiTrans();
            flg = db.UpdateByNonQuery(sql);

            if (flg)
            {
                db.EndMultiTrans();
                dt.AcceptChanges();
                XtraMessageBox.Show("Cập nhật thành công!", Config.GetValue("PackageName").ToString());
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

        private void btnCSHV_Click(object sender, EventArgs e)
        {
            int[] indexs = gvHocVien.GetSelectedRows();
            if (indexs.Length == 0)
            {
                XtraMessageBox.Show("Vui lòng chọn học viên để xem " + btnCSHV.Text, Config.GetValue("PackageName").ToString());
                return;
            }
            string HVTVID = gvHocVien.GetRowCellValue(indexs[0], "HVTVID").ToString();

            frmBCChamSocHV frm = new frmBCChamSocHV();
            frm.Text = "Tình hình chăm sóc học viên";
            frm.HVTVID = HVTVID;
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }

        private void gcHocvien_Click(object sender, EventArgs e)
        {

        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            gcHocvien.ShowPrintPreview();
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