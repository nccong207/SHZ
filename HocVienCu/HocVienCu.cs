using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using CDTLib;
using CDTDatabase;
using Plugins;
using DevExpress.XtraEditors.Repository;
using System.Data;
namespace HocVienCu
{
    public class HocVienCu:ICData
    {
        //Sau khi học viên đăng ký thì ko còn là học viên tư vấn
        //dùng cho điều kiện lọc hiển thị học viên tư vấn khi đăng ký
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();

        #region ICData Members

        public HocVienCu()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }

        public DataCustomData Data
        {
            set { _data = value; }
        } 

        public void ExecuteAfter()
        {
            
        }

        private bool CheckNguonCu(DataRow dr)
        {
            if (dr.RowState == DataRowState.Deleted)
                return true;
            bool rs = true;
            if (dr["NguonHV"].ToString() == "1" || dr["NguonHV"].ToString() == "2")
                rs = dr["MaHVDK"].ToString() != "";
            return rs;
        }
        
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
                    DateTime ngayDK = Convert.ToDateTime(drMainMaster["NgayDK"].ToString());
                    if (ngayDK.Date != DateTime.Today.Date)
                    {
                        check = false;
                    }
                }

                if (!check)
                {
                    XtraMessageBox.Show("Chỉ được sửa thông tin học viên đăng ký trong ngày hôm nay.",
                            Config.GetValue("PackageName").ToString());
                    _info.Result = false;
                    return;
                }
            }

            if (_data.CurMasterIndex < 0)
                return;
            DataTable dt = _data.DsData.Tables[0];
            using (DataView dv = new DataView(dt))
            {
                DataRow drMaster = dt.Rows[_data.CurMasterIndex];
                if (!CheckNguonCu(drMaster))
                {
                    XtraMessageBox.Show("Cần chọn học viên cũ đối với nguồn học viên cũ/bảo lưu",
                        Config.GetValue("PackageName").ToString());
                    _info.Result = false;
                    return;
                }
                else
                    _info.Result = true;
                dv.RowStateFilter = DataViewRowState.Deleted;
                string id = "";
                string sql = "";
                if (dv.Count > 0)
                {
                    //xoa
                    id = dv[0]["HVTVID"].ToString();
                    sql = "select * from MTDK where HVTVID = '" + id + "'";
                    DataTable dtt = db.GetDataTable(sql);
                    if (dtt.Rows.Count == 1)
                    {
                        sql = "update dmhvtv set isMoi = '1' where HVTVID = '" + id + "'";
                        _data.DbData.UpdateByNonQuery(sql);
                    }
                    sql = "update mtdk set BLSoTien = " + dv[0]["BLTruoc"].ToString() + " where MaHV = '" + dv[0]["MaHVDK"].ToString() + "'";
                    _data.DbData.UpdateByNonQuery(sql);
                }
                else
                {
                    id = drMaster["HVTVID"].ToString();
                    sql = "update dmhvtv set isMoi = '0' where HVTVID = '" + id + "'";
                    _data.DbData.UpdateByNonQuery(sql);
                }
            }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
