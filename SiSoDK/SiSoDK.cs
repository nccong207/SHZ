using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using CDTDatabase;
using CDTLib;
using Plugins;
using System.Data;
using DevExpress.XtraEditors;

namespace SiSoDK
{
    public class SiSoDK:ICData
    {
        //Dùng cho các việc sau
        // + Cập nhật sỉ số hiện tại cho lớp trong danh mục lớp học
        // + Cập nhật cột isDKL cho học viên bảo lưu nếu đăng lại
        // + Cập nhật số tiền bảo lưu, còn lại (nợ) của học viên cũ 
        // (học viên có mã A đăng ký Lớp mới sẽ có mã B : cập nhật tiền lại cho mã A )

        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();

        #region ICData Members
 
        public SiSoDK()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
            if (_data.CurMasterIndex < 0)
                return;
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster == null)
                return;
            _data.DbData.EndMultiTrans();   
            string tableName = _data.DrTableMaster["TableName"].ToString();
            tableName = tableName.Trim().ToUpper();
            //chức năng cập nhật sỉ số đã chuyển 1 phần qua plugins CapNhatCL
            if (tableName.Equals("MTDK"))
            {
                CapNhat(drMaster, tableName);            
                //HV bảo lưu mà đăng ký lại thì cập nhật cột isDKL để không hiện lên khi chọn hv            
                CapNhatDKL(drMaster, tableName);
            }
        }

        void CapNhat(DataRow drMaster, String tableName)
        {                                 
            DataTable dt = new DataTable();
            string malop = "", sql = "";            
            if (drMaster.RowState == DataRowState.Added)
            {
                malop = tableName == "MTDK" ? drMaster["MaLop", DataRowVersion.Current].ToString() : drMaster["MaLopHT", DataRowVersion.Current].ToString();
                //Nếu là chuyển lớp thì giảm sỉ số
                //if (tableName.Equals("MTCHUYENLOP"))
                //    sql = "Update DMLophoc set SiSoHV = case when SiSoHV <= 0 then 0 else (SiSoHV - 1) end where MaLop = '" + malop + "'";
                //else if (tableName.Equals("MTDK"))
                //    sql = "Update DMLophoc set SiSoHV = (SiSoHV + 1) where MaLop = '" + malop + "'";

                sql = @"Update DMLophoc set SiSoHV = (select count(*)
                        from mtdk where malop ='" + malop + @"' and isbl = 0 and isnghihoc = 0) 
                        where MaLop = '" + malop + "'";
                db.UpdateByNonQuery(sql);
            }
            else if (drMaster.RowState == DataRowState.Modified)
            {
                if (tableName == "MTDK")
                {
                    string maLopOrg = drMaster["MaLop", DataRowVersion.Original].ToString();
                    malop = drMaster["MaLop", DataRowVersion.Current].ToString();
                    string mahv = drMaster["MaHV", DataRowVersion.Original].ToString();
                    if (malop != maLopOrg)
                    {
                        ////Nếu học viên đang học gì giảm
                        //sql = "select * from mtdk where mahv = '" + mahv + "' and (isBL = '1' or isNghiHoc = '1')";
                        //dt = db.GetDataTable(sql);
                        //if (dt.Rows.Count == 0)
                        //{
                        //    sql = "Update DMLophoc set SiSoHV = case when SiSoHV <= 0 then 0 else (SiSoHV - 1) end where MaLop = '" + maLopOrg + "'";
                        //    db.UpdateByNonQuery(sql);
                        //}
                        ////Tăng sỉ số cho hv mới tạo
                        //sql = "Update DMLophoc set SiSoHV = (SiSoHV + 1) where MaLop = '" + malop + "'";
                        //db.UpdateByNonQuery(sql);

                        sql = @"Update DMLophoc set SiSoHV = (select count(*)
                        from mtdk where malop ='" + malop + @"' and isbl = 0 and isnghihoc = 0) 
                        where MaLop = '" + malop + "'";
                        db.UpdateByNonQuery(sql);

                        sql = @"Update DMLophoc set SiSoHV = (select count(*)
                        from mtdk where malop ='" + maLopOrg + @"' and isbl = 0 and isnghihoc = 0) 
                        where MaLop = '" + maLopOrg + "'";
                        db.UpdateByNonQuery(sql);
                    }
                }
                else if (tableName == "MTCHUYENLOP")
                {
                    //string mahvOrg = drMaster["MaHV", DataRowVersion.Original].ToString();
                    //string mahvCur = drMaster["MaHV", DataRowVersion.Current].ToString();
                    //string maLopOrg = drMaster["MaLopHT", DataRowVersion.Original].ToString();
                    //string maLopCur = drMaster["MaLopHT", DataRowVersion.Current].ToString();
                    //if (maLopOrg != maLopCur)
                    //{
                    //    sql = "select * from mtdk where mahv = '" + mahvOrg + "' and (isBL = '1' or isNghiHoc = '1')";
                    //    dt = db.GetDataTable(sql);
                    //    if (dt.Rows.Count == 0)
                    //    {
                    //        // Tăng lại sỉ số cho lớp trước đó.                        
                    //        sql = "Update DMLophoc set SiSoHV = (SiSoHV + 1) where MaLop = '" + maLopOrg + "'";
                    //        db.UpdateByNonQuery(sql);
                    //    }
                    //    //Giảm sỉ số cho lớp mới sửa lại                  
                    //    sql = "Update DMLophoc set SiSoHV = case when SiSoHV <= 0 then 0 else (SiSoHV - 1) end where MaLop = '" + maLopCur + "'";
                    //    db.UpdateByNonQuery(sql);
                    //}
                }
            }
            else if (drMaster.RowState == DataRowState.Deleted)
            {
                string mahv = drMaster["MaHV", DataRowVersion.Original].ToString();
                malop = tableName == "MTDK" ? drMaster["MaLop", DataRowVersion.Original].ToString() : drMaster["MaLopHT", DataRowVersion.Original].ToString();
                //Nếu là chuyển lớp sẽ tăng lên, còn đăng ký sẽ giảm đi
                //if (tableName == "MTDK")
                //{
                //    //Tình huống nếu đăng ký - và đã cho nghỉ học, bảo lưu thì sỉ số đã giảm rồi.
                //    //kiểm tra nếu chưa giảm mới xóa đi.
                //    sql = "select * from mtdk where mahv = '" + mahv + "' and (isBL = '1' or isNghiHoc = '1')";
                //    dt = db.GetDataTable(sql);
                //    if (dt.Rows.Count == 0)
                //        sql = "Update DMLophoc set SiSoHV = case when SiSoHV <= 0 then 0 else (SiSoHV - 1) end where MaLop = '" + malop + "'";
                //}
                //else
                //    sql = "Update DMLophoc set SiSoHV = (SiSoHV + 1) where MaLop = '" + malop + "'";

                sql = @"Update DMLophoc set SiSoHV = (select count(*)
                        from mtdk where malop ='" + malop + @"' and isbl = 0 and isnghihoc = 0) 
                        where MaLop = '" + malop + "'";
                db.UpdateByNonQuery(sql);                
            }
            //HV bảo lưu mà đăng ký lại thì cập nhật cột isDKL để không hiện lên khi chọn hv
            //CapNhatDKL(drMaster, tableName);
        }

        void CapNhatDKL(DataRow drMaster, string tableName)
        {
            if (tableName == "MTCHUYENLOP")
                return;
            string nguonHV = "";
            if (drMaster.RowState == DataRowState.Deleted)
                nguonHV = drMaster["NguonHV", DataRowVersion.Original].ToString();
            else
                nguonHV = drMaster["NguonHV"].ToString();
            if (nguonHV != "2")
                return;                        
            string sql = "";                  
            
            if (drMaster.RowState == DataRowState.Added)
            {
                sql = "update mtdk set isDKL = '1' where mahv = '" + drMaster["MaHVDK"].ToString() + "'";
                db.UpdateByNonQuery(sql);
            }
            else if (drMaster.RowState == DataRowState.Modified)
            {
                string mahvOrg = drMaster["MaHVDK", DataRowVersion.Original].ToString();
                string mahvCur = drMaster["MaHVDK", DataRowVersion.Current].ToString();
                if (mahvOrg != mahvCur)
                {
                    sql = "update mtdk set isDKL = '1' where mahv = '" + mahvCur + "'";
                    db.UpdateByNonQuery(sql);
                    sql = "update mtdk set isDKL = '0' where mahv = '" + mahvOrg + "'";
                    db.UpdateByNonQuery(sql);
                }
            }
            else if (drMaster.RowState == DataRowState.Deleted)
            {
                sql = "update mtdk set isDKL = '0' where mahv = '" + drMaster["MaHVDK", DataRowVersion.Original].ToString() + "'";
                db.UpdateByNonQuery(sql);
            }
        }

        void CapNhatTien(String tableName)
        {
            if (_data.CurMasterIndex < 0)
                return;            
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster.RowState == DataRowState.Deleted)
                return;
            if (drMaster["NguonHV"].ToString() == "0")
                return;
            string mahv = "", sql = "";
            mahv = drMaster["MaHVDK", DataRowVersion.Current].ToString();
            if (mahv == "")
                return;
            if (drMaster.RowState == DataRowState.Added)
            {
                sql = "Update MTDK set ConLai = 0, BLSoTien = 0 where MaHV = '" + mahv + "'";
                db.UpdateByNonQuery(sql);
            }
            else if (drMaster.RowState == DataRowState.Modified)
            {
                string mahvOrg = drMaster["MaHVDK", DataRowVersion.Original].ToString();
                if (mahv != mahvOrg)
                {
                    sql = "Update MTDK set ConLai = 0, BLSoTien = 0 where MaHV = '" + mahv + "'";
                    db.UpdateByNonQuery(sql);
                }
            }
        }

        public void ExecuteBefore()
        {
            string tableName = _data.DrTableMaster["TableName"].ToString();
            tableName = tableName.Trim().ToUpper();
            if (tableName.Equals("MTDK"))
                CapNhatTien(tableName);
        }  

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
} 
