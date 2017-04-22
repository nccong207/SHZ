using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CDTLib;
using DevExpress.XtraEditors;
using Plugins;

namespace KiemTraQuyenUser
{
    public class KiemTraQuyenUser: ICData
    {
        private InfoCustomData _info;
        private DataCustomData _data;
        public void ExecuteBefore()
        {
            string isAdmin = Config.GetValue("Admin").ToString();

            DataRow drMainMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMainMaster.RowState == DataRowState.Modified)
            {
                bool check = true;
                //Kiem tra quyen admin
                if (!Convert.ToBoolean(isAdmin))
                {
                    //Quyen dc update khi khong phai la Admin
                    var isUpdate = _data.DrTable["sUpdate"].ToString();
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

                if (!check)
                {
                    XtraMessageBox.Show("Chỉ được sửa thông tin phiếu bán hàng trong ngày hôm nay.",
                            Config.GetValue("PackageName").ToString());
                    _info.Result = false;
                    return;
                }
            }
        }

        public void ExecuteAfter()
        {
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        public DataCustomData Data
        {
            set { _data = value; }
        }
    }
}
