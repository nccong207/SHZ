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
            string sql = string.Format("exec sp_TinhThamNienMoi");
            db.UpdateByNonQuery(sql);
        }


        public InfoCustomData Info
        {
            get { return info; }
        }

        #endregion
    }
}
