using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;

namespace TinhLuongCL
{
    //dùng để tính lương còn lại của lớp công ty
    public class TinhLuongCL : ICData
    {
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();

        public TinhLuongCL()
        {
            _info = new InfoCustomData(IDataType.Single);
        }

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {

        }

        // Plugin sai ở TH: LuongDu(Lương còn lại) trong DMHVCT không khớp với [Tổng lương HD] - LuongDay
        // Xử lý trước khi lưu bảng lương tháng của giáo viên công ty
        public void ExecuteBefore()
        {
            decimal conlai = 0;
            string sql = "update DMHVCT set LuongDu = {1} where MaLop = '{0}'";
            DataRow dr = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            
            DataRowVersion drv = dr.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Default;
            string maLop = dr["MaLop", drv].ToString();
            string sqlText = "Select * From DMHVCT Where Malop = '" + maLop + "'";
            DataTable dt = db.GetDataTable(sqlText);
            
            if (dt.Rows.Count > 0)
                conlai = decimal.Parse(dt.Rows[0]["LuongDu"].ToString());
            // TH Thêm
            if (dr.RowState == DataRowState.Added)
                conlai -= decimal.Parse(dr["TongLuong"].ToString());
            
            // TH Sửa
            if (dr.RowState == DataRowState.Modified)
            {
                conlai += decimal.Parse(dr["TongLuong", DataRowVersion.Original].ToString());
                conlai -= decimal.Parse(dr["TongLuong", DataRowVersion.Current].ToString());
            }
            // TH xóa
            if (dr.RowState == DataRowState.Deleted)
                conlai += decimal.Parse(dr["TongLuong",DataRowVersion.Original].ToString());

            // Cập nhật cột LuongDu(lương còn lại) trong DMHVTV
            string s = String.Format(sql, maLop, conlai.ToString().Replace(',', '.'));
            _info.Result = db.UpdateByNonQuery(s);

            //cập nhật luôn trong từng dòng của bảng lương tháng trước khi lưu
            if (dr.RowState != DataRowState.Deleted)
                dr["LuongCL"] = conlai;
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }
    }
}
