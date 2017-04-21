using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using CDTLib;
using Plugins;
using System.Data;
using DevExpress.XtraEditors;


namespace CapNhatThanhToan
{
    public class CapNhatThanhToan :ICData 
    {
        private InfoCustomData info = new InfoCustomData(IDataType.MasterDetailDt);
        private DataCustomData data;
        Database db = Database.NewDataDatabase();
        private bool isUpdateOfUser = true;
        #region ICData Members

        public DataCustomData Data
        {
            set { data = value ; }
        }

        public void ExecuteAfter()
        {
            if (!isUpdateOfUser)
            {
                info.Result = false;
                return;
            }

            data.DbData.EndMultiTrans();           
            string MaDT = "";
            if(data.CurMasterIndex < 0)
                return;
            DataRow drMaster = data.DsData.Tables[0].Rows[data.CurMasterIndex];
            MaDT = drMaster.RowState == DataRowState.Deleted ? drMaster["MaKH", DataRowVersion.Original].ToString() : drMaster["MaKH",DataRowVersion.Current].ToString();
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
            dt = db.GetDataTable(string.Format(sql,MaDT));
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
                if (query != "")
                    db.UpdateByNonQuery(query);
            }
        }

        public void ExecuteBefore()
        {
            //check quyen cua user
            string isAdmin = Config.GetValue("Admin").ToString();

            DataRow drMainMaster = data.DsData.Tables[0].Rows[data.CurMasterIndex];
            if (drMainMaster.RowState == DataRowState.Modified)
            {
                bool check = true;
                //Kiem tra quyen admin
                if (!Convert.ToBoolean(isAdmin))
                {
                    //Quyen dc update khi khong phai la Admin
                    var isUpdate = data.DrTable["sUpdate"].ToString();
                    
                    if (String.IsNullOrEmpty(isUpdate) || !Convert.ToBoolean(isUpdate))
                    {
                        check = false;
                    }

                    //Kiem tra ngay dang ky voi ngay hien tai
                    DateTime ngayDK = Convert.ToDateTime(drMainMaster["NgayCT"].ToString());
                    if (ngayDK.Date != DateTime.Today.Date)
                    {
                        check = false;
                    }
                }

                isUpdateOfUser = check;
                if (!check)
                {
                    XtraMessageBox.Show("Chỉ được sửa thông tin phiếu thu trong ngày hôm nay.",
                            Config.GetValue("PackageName").ToString());
                    info.Result = false;
                }
            }
            
        }

        public InfoCustomData Info
        {
            get { return info; }
        }

        #endregion
    }
}
