using System;
using System.Collections.Generic;
using System.Text;
using CDTLib;
using Plugins;
using System.Data;
using CDTDatabase;

namespace CapNhatThanhToan2
{
    public class CapNhatThanhToan2 :ICData 
    {
        private InfoCustomData info = new InfoCustomData(IDataType.MasterDetailDt);
        private DataCustomData data;
        Database db = Database.NewDataDatabase();

        #region ICData Members

        public DataCustomData Data
        {
            set { data = value ; }
        }

        public void ExecuteAfter()
        {
            data.DbData.EndMultiTrans();
            string MaDT = "";
            if (data.CurMasterIndex < 0)
                return;
            DataRow drMaster = data.DsData.Tables[0].Rows[data.CurMasterIndex];
            MaDT = drMaster.RowState == DataRowState.Deleted ? drMaster["MaKH", DataRowVersion.Original].ToString() : drMaster["MaKH", DataRowVersion.Current].ToString();
            string sql = @"SELECT ISNULL(SUM(PS),0) TongPS,ISNULL(Max(NgayCT),NULL) NgayCT
                            FROM
                            (
                            SELECT NgayCT,PS FROM MT11 m INNER JOIN DT11 d ON m.MT11ID = d.MT11ID
                            WHERE TKCo = '131' AND MaKHCt = '{0}'
                            UNION all
                            SELECT NgayCT,PS FROM MT51 m INNER JOIN DT51 d ON m.MT51ID = d.MT51ID
                            WHERE TKCo = '131' AND MaKHCt = '{0}'
                            )W";
            DataTable dt = new DataTable();
            dt = db.GetDataTable(string.Format(sql, MaDT));
            string query = "";
            if (dt.Rows.Count > 0)
            {
                decimal TTien = (decimal)dt.Rows[0]["TongPS"];
                if (TTien == 0 && dt.Rows[0]["NgayCT"].ToString() == "")
                {
                    query = string.Format("UPDATE DMHVCT SET NgayDK = NULL ,SoTienTT = 0 WHERE MaLop = '{0}'", MaDT);
                }
                else
                {
                    DateTime NgayCT = (DateTime)dt.Rows[0]["NgayCT"];
                    query = string.Format("UPDATE DMHVCT SET NgayDK = '{0}',SoTienTT = {1} WHERE MaLop = '{2}'", NgayCT, TTien, MaDT);
                }
                if(query != "")
                    db.UpdateByNonQuery(query);
            }
        }

        public void ExecuteBefore()
        {
            
        }

        public InfoCustomData Info
        {
            get { return info; }
        }

        #endregion
    }
}
