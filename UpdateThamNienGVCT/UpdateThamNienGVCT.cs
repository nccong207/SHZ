using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using Plugins;
using CDTLib;

namespace UpdateThamNienGVCT
{
    public class UpdateThamNienGVCT :ICData
    {
        private InfoCustomData info = new InfoCustomData(IDataType.Single);
        private DataCustomData data;
        Database db = Database.NewDataDatabase();

        #region ICData Members

        public DataCustomData Data
        {
            set { data = value ; }
        }

        public void ExecuteAfter()
        {
            CapNhatThamNien();
        }

        public void ExecuteBefore()
        {
           
        }

        private void CapNhatThamNien()
        {
            data.DbData.EndMultiTrans();
            //string maCN = Config.GetValue("MaCN").ToString();
            //string sql = "update DMNVien set ThamNienMoi = ThamNien + isnull((select count(*) from (select Thang,GVID from ChamCongGV where Thang <= " + seThang.Text + " group by Thang,GVID) t where t.GVID = ID),0)";
            //db.UpdateByNonQuery(sql);
            string nam = Config.GetValue("NamLamViec").ToString();
            string sql = "update DMNVien set ThamNienMoi = ThamNien + isnull((select count(*) from (select Thang,Nam,MaGV from LuongGVCT where (Nam < '" + nam + "' or (Nam = '" + nam + "' and Thang <= (SELECT  Max(Thang) FROM LuongGVCT WHERE Nam ='"+ nam +"'  )" + ")) group by Thang,Nam,MaGV) t where t.MaGV = DMNVien.MaNV),0) WHERE isCT = 1";
            db.UpdateByNonQuery(sql);
        }


        public InfoCustomData Info
        {
            get { return info; }
        }

        #endregion
    }
}
