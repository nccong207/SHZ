using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using CDTDatabase;
using CDTLib;
using Plugins;
using System.Data;

namespace CapNhatHP
{
    public class CapNhatHP:ICData
    {
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();

        #region ICData Members
 
        public CapNhatHP()
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
        }

        void CapNhatNo()
        {                       
            if (_data.CurMasterIndex < 0)
                return;
            string sql = "";
            string RefValue = "";
            string MaNV = "";//mã nghiệp vụ current
            string MaNVOrg = "";
            string ID = "";

            Database db = Database.NewDataDatabase();            
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
                     
            DataView dvMaster = new DataView(_data.DsData.Tables[0]);
            dvMaster.RowStateFilter = DataViewRowState.Deleted;

            if (dvMaster.Count > 0)
            {                
                MaNV = dvMaster[0]["MaNV"].ToString();
                ID = dvMaster[0]["MT12ID"].ToString();
            }
            else
            {                
                MaNV = drMaster["MaNV"].ToString();
                ID = drMaster["MT12ID"].ToString();
            }
            if (MaNV.ToUpper() != "HOANHP")
                return;

            if (drMaster.RowState == DataRowState.Modified)
                MaNVOrg = drMaster["MaNV", DataRowVersion.Original].ToString();

            DataView dv = new DataView(_data.DsData.Tables[1]);
            dv.RowStateFilter = DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted;
            dv.RowFilter = " MT12ID = '"+ID+"'";
            
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
                    sql = "update MTDK set BLSoTien =  BLSoTien - '" + drv["PS"].ToString().Replace(",", ".") + "' where MaHV = '" + drv["MaKHCt"].ToString() + "'";
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
                        if (MaNVOrg == MaNV)
                        {
                            after = decimal.Parse(drv.Row["Ps", DataRowVersion.Current].ToString());
                            before = decimal.Parse(drv.Row["Ps", DataRowVersion.Original].ToString());
                            sql = "update MTDK set BLSoTien = (BLSoTien + '" + before.ToString().Replace(",", ".") + "') - '" + after.ToString().Replace(",", ".") + "' where MaHV = '" + maHVOrg + "'";
                            db.UpdateByNonQuery(sql);
                        }
                        else
                        {
                            sql = "update MTDK set BLSoTien =  BLSoTien - '" + drv["PS"].ToString().Replace(",", ".") + "' where MaHV = '" + drv["MaKHCt"].ToString() + "'";
                            db.UpdateByNonQuery(sql);
                        }
                    }
                    else
                    {
                        // sửa lại hv khác
                        after = decimal.Parse(drv.Row["Ps", DataRowVersion.Current].ToString());
                        before = decimal.Parse(drv.Row["Ps", DataRowVersion.Original].ToString());
                        if (MaNVOrg == MaNV)
                        {
                            //dang ky - người trước khi sửa
                            sql = "update MTDK set BLSoTien = (BLSoTien + '" + before.ToString().Replace(",", ".") + "') where MaHV = '" + maHVOrg + "'";
                            db.UpdateByNonQuery(sql);

                            //dang ky - người sau khi sửa
                            sql = "update MTDK set BLSoTien = (BLSoTien - '" + after.ToString().Replace(",", ".") + "') where MaHV = '" + maHVCur + "'";
                            db.UpdateByNonQuery(sql);
                        }
                        else
                        {
                            sql = "update MTDK set BLSoTien =  BLSoTien - '" + drv["PS"].ToString().Replace(",", ".") + "' where MaHV = '" + drv["MaKHCt"].ToString() + "'";
                            db.UpdateByNonQuery(sql);
                        }
                    }
                }
                else if (drv.Row.RowState == DataRowState.Deleted)
                {
                    //dang ky
                    sql = "update MTDK set BLSoTien = BLSoTien + '" + drv.Row["Ps", DataRowVersion.Original].ToString() + "' where MaHV = '" + drv.Row["MaKHCt", DataRowVersion.Original].ToString() + "'";
                    db.UpdateByNonQuery(sql);                    
                }
            }           
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
