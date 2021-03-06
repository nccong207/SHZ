using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using CDTDatabase;
using CDTLib;
using Plugins;
using System.Data;

namespace SuaTenHV
{
    public class SuaTenHV:ICData
    {
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();        
        public SuaTenHV()
        {
            _info = new InfoCustomData(IDataType.Single);
        }

       
        #region ICData Members

        public DataCustomData Data
        {
            set { _data = value; }
        }
         
        public void ExecuteAfter()
        {
            if (_data.CurMasterIndex < 0)
                return;
            if (_data.DsData == null)
                return;
            DataRow drMaster = _data.DsDataCopy.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster == null)
                return;
            if (drMaster.RowState == DataRowState.Added || drMaster.RowState == DataRowState.Modified)
            {
                InsertCSHV(_data.DsData.Tables[0].Rows[_data.CurMasterIndex]);
            }
        }

        public void ExecuteBefore()
        {
            update();
        }

        private void update()
        {
            if (_data.CurMasterIndex < 0)
                return;
            if (_data.DsData == null)
                return;
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster == null)
                return;

            if (drMaster.RowState == DataRowState.Modified)
            {
                CheckChangeCSHV(drMaster);

                if (_info.Result)
                    ChangeName(drMaster);
            }
        }

        private void InsertCSHV(DataRow drMaster)
        {
            object phID = drMaster["PhanHoiID"];
            object KHTT = drMaster["KHTT"];
            object ngayTH = drMaster["NgayTHien"];
            object ghiChu = drMaster["GhiChu"];
            if (String.IsNullOrEmpty(phID.ToString())
                && String.IsNullOrEmpty(KHTT.ToString())
                && String.IsNullOrEmpty(ngayTH.ToString()))
                return;
            //chi thuc hien cho lan cap nhat du lieu CSHV dau tien o DMHVTV
            if (drMaster.RowState == DataRowState.Modified
                && !(String.IsNullOrEmpty(drMaster["PhanHoiID", DataRowVersion.Original].ToString())
                     && String.IsNullOrEmpty(drMaster["KHTT", DataRowVersion.Original].ToString())
                     && String.IsNullOrEmpty(drMaster["NgayTHien", DataRowVersion.Original].ToString())))
                return;

            string sql = "insert into DMQTCSHV(HVTVID, Ngay, PHID, MaNV, KHTT, NgayTH, GhiChu)" +
                         " values(@HVTVID, @Ngay, @PHID, @MaNV, @KHTT, @NgayTH, @GhiChu)";

            _data.DbData.EndMultiTrans();
            if (!db.UpdateDatabyPara(sql, new[] {"HVTVID", "Ngay", "PHID", "MaNV", "KHTT", "NgayTH", "GhiChu"},
                new[] {drMaster["HVTVID"], drMaster["NgayTV"], phID, drMaster["MaNVTV"], KHTT, ngayTH, ghiChu}))
            {
                XtraMessageBox.Show("Thông tin nhân viên đã được cập nhật thành công\n" +
                                    "Tuy nhiên có lỗi phát sinh khi cập nhật quá trình CSHV",
                    Config.GetValue("PackageName").ToString());
                return;
            }
        }

        private void CheckChangeCSHV(DataRow drMaster)
        {
            object phID = drMaster["PhanHoiID", DataRowVersion.Original];
            object KHTT = drMaster["KHTT", DataRowVersion.Original];
            object ngayTH = drMaster["NgayTHien", DataRowVersion.Original];
            //chi thuc hien cho lan cap nhat du lieu CSHV dau tien o DMHVTV
            bool isChanged = !(string.IsNullOrEmpty(phID.ToString()) && string.IsNullOrEmpty(KHTT.ToString()) &&
                                string.IsNullOrEmpty(ngayTH.ToString()))
                            && !(phID.Equals(drMaster["PhanHoiID"])
                                && KHTT.Equals(drMaster["KHTT"])
                                && ngayTH.Equals(drMaster["NgayTHien"]));
            if (isChanged)
            {
                XtraMessageBox.Show("Thông tin chăm sóc học viên chỉ được nhập lần đầu tại đây\n" +
                                    "Vui lòng tiếp tục cập nhật ở nghiệp vụ CSHV", Config.GetValue("PackageName").ToString());
                _info.Result = false;
                return;
            }
            _info.Result = true;
        }

        private void ChangeName(DataRow drMaster)
        {
            if (drMaster["TenHV", DataRowVersion.Original].ToString() == drMaster["TenHV", DataRowVersion.Current].ToString())
                return;
            string code = drMaster[_data.DrTable["pk"].ToString()].ToString();
            string newName = drMaster["TenHV"].ToString();
            // HV tư vấn, hv đăng ký, dm khách hàng, hv chuyển lớp, phiếu thu, phiếu chi, blvt, bltk.
            //MTDK
            string MaHV = "", sql = "";
            sql = "Select * from MTDK where HVTVID = '" + code + "'";
            DataTable dt = db.GetDataTable(sql);            
            foreach (DataRow row in dt.Rows)
            {
                MaHV = row["MaHV"].ToString();
                sql = "Update MTDK set TenHV = N'" + newName + "' where HVTVID = '" + code + "' and MaHV = '" + MaHV + "'";
                db.UpdateByNonQuery(sql);
                sql = "Update DMKH set TenKH = N'" + newName + "' where MaKH = '" + MaHV + "'";
                db.UpdateByNonQuery(sql);
                sql = "Update MTChuyenLop set TenHV = N'" + newName + "' where MaHV = '" + MaHV + "'";
                db.UpdateByNonQuery(sql);
                sql = "Update MT11 set TenKH = N'" + newName + "' where MaKH = '" + MaHV + "'";
                db.UpdateByNonQuery(sql);
                sql = "Update DT11 set TenKHCt = N'" + newName + "' where MaKhCt = '" + MaHV + "'";
                db.UpdateByNonQuery(sql);
                sql = "Update MT12 set TenKH = N'" + newName + "' where MaKH = '" + MaHV + "'";
                db.UpdateByNonQuery(sql);
                sql = "Update DT12 set TenKHCt = N'" + newName + "' where MaKhCt = '" + MaHV + "'";
                db.UpdateByNonQuery(sql);
                sql = "Update BLVT set TenKH = N'" + newName + "' where MaKH = '" + MaHV + "'";
                db.UpdateByNonQuery(sql);
                sql = "Update BLTK set TenKH = N'" + newName + "' where MaKH = '" + MaHV + "'";
                db.UpdateByNonQuery(sql);
                sql = "Update DMKQ set TenHV = N'" + newName + "' where HVID = '" + row["HVID"].ToString() + "'";
                db.UpdateByNonQuery(sql);
                sql = "Update MT32 set TenKH = N'" + newName + "' where MaKH = '" + MaHV + "'";
                db.UpdateByNonQuery(sql);
            }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
