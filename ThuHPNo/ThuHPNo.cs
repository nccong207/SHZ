using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using CDTDatabase;
using CDTLib;
using Plugins;
using System.Data;

namespace ThuHPNo
{
    public class ThuHPNo:ICData
    {
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();

        #region ICData Members
 
        public ThuHPNo()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
            CapNhatNo();
            CapNhatThucThu();
        }  

        void CapNhatNo()
        {
            //Chú ý phải cập nhật 2 chỗ: bảng đăng ký và khách hàng 
            //Chuyển học phí nợ qua khách hàng để khi chọn đối tượng sẽ thấy số tiền còn nợ

            //Chú ý: vì khi đăng ký thiết lập quy trình mình đã tạo ra 1 phiếu thu
            //nên chỉ xử lý trên các phiếu thu không được tạo ra khi đăng ký (k có trường tham chiếu đến học viên)
            if (_data.CurMasterIndex < 0)
                return;
            string sql = "";
            string RefValue = "";
            string MaNV = "";//mã nghiệp vụ  
            string MaNVOrg = "";
            string ID = "";

            Database db = Database.NewDataDatabase();            
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
                     
            DataView dvMaster = new DataView(_data.DsData.Tables[0]);
            dvMaster.RowStateFilter = DataViewRowState.Deleted;

            if (dvMaster.Count > 0)
            {
                RefValue = dvMaster[0]["HocVien"].ToString();
                MaNV = dvMaster[0]["MaNV"].ToString();
                ID = dvMaster[0]["MT11ID"].ToString();
            }
            else
            {
                RefValue = drMaster["HocVien"].ToString();
                MaNV = drMaster["MaNV"].ToString();
                ID = drMaster["MT11ID"].ToString();
            }
            if (RefValue != "" )
                return;            
            if (drMaster.RowState == DataRowState.Modified)
                MaNVOrg = drMaster["MaNV", DataRowVersion.Original].ToString();
            if (MaNV.ToUpper() == "HP")
            {
                #region MaNV sau là HP
                DataView dv = new DataView(_data.DsData.Tables[1]);
                dv.RowStateFilter = DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted;
                dv.RowFilter = " MT11ID = '" + ID + "'";

                foreach (DataRowView drv in dv)
                {
                    if (drv.Row.RowState == DataRowState.Unchanged)
                        return;
                    if (drv["MaKHct"].ToString().ToUpper().Equals("HVVL") ||
                        drv["MaKHct"].ToString().ToUpper().Equals("HVCT"))
                        return;
                    if (drv.Row.RowState == DataRowState.Added)
                    {
                        //dang ky
                        sql = "update MTDK set Conlai =  Conlai - '" + drv["PS"].ToString().Replace(",", ".") + "' where MaHV = '" + drv["MaKHCt"].ToString() + "'";
                        db.UpdateByNonQuery(sql);
                        //khach hang
                        sql = "update DMKH set HanMucDS =  HanMucDS - '" + drv["PS"].ToString().Replace(",", ".") + "' where MaKH = '" + drv["MaKHCt"].ToString() + "'";
                        db.UpdateByNonQuery(sql);
                    }
                    else if (drv.Row.RowState == DataRowState.Modified)
                    {
                        string maHVOrg = "", maHVCur = "";
                        decimal before = 0, after = 0;
                        maHVOrg = drv.Row["MaKHct", DataRowVersion.Original].ToString();
                        maHVCur = drv.Row["MaKHct", DataRowVersion.Current].ToString();
                        //nếu sửa tiền mà ko sửa mã học viên
                        if (maHVCur == maHVOrg)
                        {
                            //Nghiệp vụ trước vào sau khi sửa đề là HP
                            if (MaNVOrg == MaNV)
                            {
                                after = decimal.Parse(drv.Row["Ps", DataRowVersion.Current].ToString());
                                before = decimal.Parse(drv.Row["Ps", DataRowVersion.Original].ToString());
                                //dang ky
                                sql = "update MTDK set conlai = (conlai + '" + before.ToString().Replace(",", ".") + "') - '" + after.ToString().Replace(",", ".") + "' where MaHV = '" + maHVOrg + "'";
                                db.UpdateByNonQuery(sql);
                                //khach hang
                                sql = "update DMKH set HanMucDS = (HanMucDS + '" + before.ToString().Replace(",", ".") + "') - '" + after.ToString().Replace(",", ".") + "' where MaKH = '" + maHVOrg + "'";
                                db.UpdateByNonQuery(sql);
                            }
                            else
                            {
                                //Nghiệp vụ trước ko là HP, nghiệp vụ sau khi sửa là HP
                                //dang ky
                                sql = "update MTDK set Conlai =  Conlai - '" + drv["PS"].ToString().Replace(",", ".") + "' where MaHV = '" + drv["MaKHCt"].ToString() + "'";
                                db.UpdateByNonQuery(sql);
                                //khach hang
                                sql = "update DMKH set HanMucDS =  HanMucDS - '" + drv["PS"].ToString().Replace(",", ".") + "' where MaKH = '" + drv["MaKHCt"].ToString() + "'";
                                db.UpdateByNonQuery(sql);
                            }
                        }
                        else
                        {
                            // sửa lại hv khác
                            after = decimal.Parse(drv.Row["Ps", DataRowVersion.Current].ToString());
                            before = decimal.Parse(drv.Row["Ps", DataRowVersion.Original].ToString());
                            //Nghiệp vụ trước vào sau khi sửa đề là HP
                            if (MaNVOrg == MaNV)
                            {
                                //dang ky - người trước khi sửa
                                sql = "update MTDK set conlai = (conlai + '" + before.ToString().Replace(",", ".") + "') where MaHV = '" + maHVOrg + "'";
                                db.UpdateByNonQuery(sql);
                                //khach hang - người trước khi sửa
                                sql = "update DMKH set HanMucDS = (HanMucDS + '" + before.ToString().Replace(",", ".") + "') where MaKH = '" + maHVOrg + "'";
                                db.UpdateByNonQuery(sql);

                                //dang ky - người sau khi sửa
                                sql = "update MTDK set conlai = (conlai - '" + after.ToString().Replace(",", ".") + "') where MaHV = '" + maHVCur + "'";
                                db.UpdateByNonQuery(sql);
                                //khach hang - người sau khi sửa
                                sql = "update DMKH set HanMucDS = (HanMucDS - '" + after.ToString().Replace(",", ".") + "') where MaKH = '" + maHVCur + "'";
                                db.UpdateByNonQuery(sql);
                            }
                            else
                            {
                                //dang ky - người sau khi sửa
                                sql = "update MTDK set conlai = (conlai - '" + after.ToString().Replace(",", ".") + "') where MaHV = '" + maHVCur + "'";
                                db.UpdateByNonQuery(sql);
                                //khach hang - người sau khi sửa
                                sql = "update DMKH set HanMucDS = (HanMucDS - '" + after.ToString().Replace(",", ".") + "') where MaKH = '" + maHVCur + "'";
                                db.UpdateByNonQuery(sql);
                            }
                        }
                    }
                    else if (drv.Row.RowState == DataRowState.Deleted)
                    {
                        //dang ky
                        sql = "update MTDK set conlai = conlai + '" + drv.Row["Ps", DataRowVersion.Original].ToString() + "' where MaHV = '" + drv.Row["MaKHCt", DataRowVersion.Original].ToString() + "'";
                        db.UpdateByNonQuery(sql);
                        //khach hang
                        sql = "update DMKH set HanMucDS = HanMucDS + '" + drv.Row["Ps", DataRowVersion.Original].ToString() + "' where MaKH = '" + drv.Row["MaKHCt", DataRowVersion.Original].ToString() + "'";
                        db.UpdateByNonQuery(sql);
                    }
                }
                #endregion
            }
            else if (MaNVOrg.ToUpper() == "HP" && MaNV.ToUpper() != "HP")
            {                
                #region MaNV trước là HP
                DataView dv = new DataView(_data.DsData.Tables[1]);
                dv.RowStateFilter = DataViewRowState.CurrentRows | DataViewRowState.ModifiedCurrent ;
                dv.RowFilter = " MT11ID = '" + ID + "'";

                foreach (DataRowView drv in dv)
                {
                    if (drv.Row.RowState == DataRowState.Deleted)
                        return;                    
                    if (drv["MaKHct"].ToString().ToUpper().Equals("HVVL") ||
                        drv["MaKHct"].ToString().ToUpper().Equals("HVCT"))
                        return;
                    if (drv.Row.RowState == DataRowState.Unchanged)
                    {
                        sql = "update MTDK set Conlai =  Conlai + '" + drv["PS"].ToString().Replace(",", ".") + "' where MaHV = '" + drv["MaKHCt"].ToString() + "'";
                        db.UpdateByNonQuery(sql);
                        //khach hang
                        sql = "update DMKH set HanMucDS =  HanMucDS + '" + drv["PS"].ToString().Replace(",", ".") + "' where MaKH = '" + drv["MaKHCt"].ToString() + "'";
                        db.UpdateByNonQuery(sql);
                    }
                    else if (drv.Row.RowState == DataRowState.Modified)
                    {
                        string maHVOrg = "", maHVCur = "";
                        decimal before = 0, after = 0;
                        maHVOrg = drv.Row["MaKHct", DataRowVersion.Original].ToString();
                        maHVCur = drv.Row["MaKHct", DataRowVersion.Current].ToString();
                        after = decimal.Parse(drv.Row["Ps", DataRowVersion.Current].ToString());
                        before = decimal.Parse(drv.Row["Ps", DataRowVersion.Original].ToString());
                        //nếu sửa tiền mà ko sửa mã học viên
                        if (maHVCur == maHVOrg)
                        {                                                       
                            //dang ky
                            sql = "update MTDK set conlai = conlai + '" + before.ToString().Replace(",", ".") + "' where MaHV = '" + maHVCur + "'";
                            db.UpdateByNonQuery(sql);
                            //khach hang
                            sql = "update DMKH set HanMucDS = HanMucDS + '" + before.ToString().Replace(",", ".") + "' where MaKH = '" + maHVCur + "'";
                            db.UpdateByNonQuery(sql);
                        }
                        else
                        {
                            sql = "update MTDK set Conlai =  Conlai + '" + before.ToString().Replace(",", ".") + "' where MaHV = '" + maHVOrg + "'";
                            db.UpdateByNonQuery(sql);
                            //khach hang
                            sql = "update DMKH set HanMucDS =  HanMucDS + '" + before.ToString().Replace(",", ".") + "' where MaKH = '" + maHVOrg + "'";
                            db.UpdateByNonQuery(sql);
                        }
                    }
                }
                #endregion
            }
        }

        void CapNhatThucThu()
        {
            Database db = Database.NewDataDatabase();
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            _data.DbData.EndMultiTrans();
            
            string MaHV = drMaster.RowState == DataRowState.Deleted ? drMaster["MaKH", DataRowVersion.Original].ToString() : drMaster["MaKH", DataRowVersion.Current].ToString();
            //string sql = @" UPDATE MTDK SET ConLai = TienHP - ISNULL(( SELECT Sum(isnull(Ttien,0)) Ttien FROM MT11 WHERE MaNV = 'HP' AND MaKH = '{0}' ),0) 
            //                                , ThucThu = ISNULL(( SELECT Sum(isnull(Ttien,0)) Ttien FROM MT11 WHERE MaNV = 'HP' AND MaKH = '{0}' ),0) WHERE MaHV = '{1}'";
            string sql = @" UPDATE MTDK SET ThucThu = ISNULL(( SELECT Sum(isnull(Ttien,0)) FROM MT11 WHERE MaNV = 'HP' AND MaKH = '{0}' ),0) WHERE MaHV = '{0}'";
            db.UpdateByNonQuery(string.Format(sql, MaHV));
            //ngày 2/8/2015: chỉ cập nhật còn lại và bảo lưu với điều kiện thực thu > 0 [start]
            sql = @"UPDATE MTDK SET ConLai = case when TienHP > ThucThu then TienHP - ThucThu else 0 end, BLSoTien = case when TienHP < ThucThu then ThucThu - TienHP else 0 end WHERE MaHV = '{0}' and ThucThu > 0";
            db.UpdateByNonQuery(string.Format(sql, MaHV));
            //ngày 2/8/2015: chỉ cập nhật còn lại và bảo lưu với điều kiện thực thu > 0 [start]
            string query = @" UPDATE DMKH set HanMucDS = ISNULL(( SELECT TienHP - ThucThu  FROM MTDK WHERE  MaHV = '{0}'),0) WHERE MaKH = '{0}'";
            db.UpdateByNonQuery(string.Format(query,MaHV));
        }

        public void ExecuteBefore()
        {
            
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
