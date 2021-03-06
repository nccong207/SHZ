using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTDatabase;
using CDTLib;
using Plugins;

namespace NgayHocCuoi
{
    public class NgayHocCuoi : ICData 
    {
        private InfoCustomData info = new InfoCustomData(IDataType.MasterDetailDt);
        private DataCustomData data;
        #region ICData Members

        public DataCustomData Data
        {
            set { data = value; }
        }

        public void ExecuteAfter()
        {
            if (data.CurMasterIndex < 0)
                return;
            Database db = Database.NewDataDatabase();
            if (data.DrTableMaster["TableName"].ToString() == "MT11")
            {
                DataRow drMaster = data.DsData.Tables[0].Rows[data.CurMasterIndex];
                if (drMaster.RowState == DataRowState.Deleted ? drMaster["MaNV",DataRowVersion.Original].ToString() != "HP" : drMaster["MaNV"].ToString() != "HP")
                    return;
                decimal ThucThu = 0, SoBuoiCL = 0, Tien1b = 0, SoBuoiDH = 0;

                string MaHV = drMaster.RowState == DataRowState.Deleted ? drMaster["MaKH", DataRowVersion.Original].ToString() : drMaster["MaKH", DataRowVersion.Current].ToString();
                string MaLop = drMaster.RowState == DataRowState.Deleted ? drMaster["NhomKH", DataRowVersion.Original].ToString() : drMaster["NhomKH", DataRowVersion.Current].ToString();
                string sql1 = @" SELECT  BLTruoc + ISNULL((SELECT SUM(TTien) FROM MT11 WHERE MaNV = 'HP' AND MaKH = '{0}'),0) ThucThu
                                        ,NgayDK,dbo.fc_SoBuoi(NgayDK,MaLop) SoBuoi
                                        , (BLTruoc + ThucThu + ConLai - BLSoTien) /  dbo.fc_SoBuoi(NgayDK,MaLop) Tien 
                                FROM  MTDK WHERE MaHV = '{1}'";
                DataTable dtTien = new DataTable();
                data.DbData.EndMultiTrans();
                dtTien = db.GetDataTable(string.Format(sql1, MaHV, MaHV));
                if (dtTien.Rows.Count > 0)
                {
                    SoBuoiCL = decimal.Parse(dtTien.Rows[0]["SoBuoi"].ToString());
                    ThucThu = decimal.Parse(dtTien.Rows[0]["ThucThu"].ToString());
                    DateTime NgayDK = DateTime.Parse(dtTien.Rows[0]["NgayDK"].ToString());
                    DateTime NgayHoc = DateTime.MinValue;
                    Tien1b = decimal.Parse(dtTien.Rows[0]["Tien"].ToString());
                    if (Tien1b == 0)
                        return;
                    SoBuoiDH = Tien1b==0?0:(Math.Round(ThucThu / Tien1b));
                    if (SoBuoiDH > SoBuoiCL)
                        SoBuoiDH = SoBuoiCL;
                    // Tính ngày học cuối
                    DataTable dtNgayKT = db.GetDataTable(string.Format("exec TinhNgayKT {0},'{1}', '{2}'", SoBuoiDH, NgayDK, MaLop));
                    string MaKH = drMaster.RowState == DataRowState.Deleted ? drMaster["MaKH",DataRowVersion.Original].ToString() : drMaster["MaKH"].ToString();
                    if (dtNgayKT.Rows.Count == 0 || dtNgayKT.Rows[0]["NgayKT"].ToString() == "")
                    {
                        NgayHoc = DateTime.MinValue;
                    }
                    else
                        NgayHoc = DateTime.Parse(dtNgayKT.Rows[0]["NgayKT"].ToString());
                    if (NgayHoc != DateTime.MinValue)
                        db.UpdateByNonQuery(string.Format("UPDATE MTDK SET NgayHocCuoi = '{0}',SoBuoiDH = '{1}' WHERE MaHV = '{2}'", NgayHoc, SoBuoiDH,MaKH));
                    else
                        db.UpdateByNonQuery(string.Format("UPDATE MTDK SET NgayHocCuoi = null,SoBuoiDH = '{0}' WHERE MaHV = '{1}'", SoBuoiDH, MaKH));
                }
            }
        }

        public void ExecuteBefore()
        {
          // Tính ngày học cuối và số buổi được học của học viên
            if (data.CurMasterIndex < 0)
                return;
            Database db = Database.NewDataDatabase();
            if (data.DrTableMaster["TableName"].ToString() == "MTDK")
            {
                DataRow drMaster = data.DsData.Tables[0].Rows[data.CurMasterIndex];
                if (drMaster.RowState == DataRowState.Deleted)
                    return;
                decimal ThucThu = 0, SoBuoiCL = 0, Tien1b = 0, SoBuoiDH = 0, TienHP = 0 ;
                string MaLop = drMaster["MaLop"].ToString();
                decimal GiamHP = decimal.Parse( drMaster["GiamHP"].ToString());
                ThucThu =  decimal.Parse(drMaster["BLTruoc"].ToString()) + decimal.Parse(drMaster["ThucThu"].ToString());
                DateTime NgayDK = (DateTime)drMaster["NgayDK"];
                TienHP = decimal.Parse(drMaster["BLTruoc"].ToString()) + decimal.Parse(drMaster["ThucThu"].ToString()) + decimal.Parse(drMaster["ConLai"].ToString()) - decimal.Parse(drMaster["BLSoTien"].ToString());
                if (GiamHP == 100)
                {
                    drMaster["SoBuoiDH"] = drMaster["SoBuoiCL"];
                    DataTable dtNgayKT = db.GetDataTable(string.Format("exec TinhNgayKT {0},'{1}', '{2}'", decimal.Parse(drMaster["SoBuoiCL"].ToString()), NgayDK, MaLop));
                    if (dtNgayKT.Rows.Count == 0 || dtNgayKT.Rows[0]["NgayKT"].ToString() == "")
                    {
                        drMaster["NgayHocCuoi"] = DBNull.Value;
                    }
                    else
                        drMaster["NgayHocCuoi"] = DateTime.Parse(dtNgayKT.Rows[0]["NgayKT"].ToString());
                }
                else
                {
                    string sql = @"SELECT dbo.fc_SoBuoi('{0}','{1}') SoBuoi";
                    DataTable dtHP = new DataTable();
                    dtHP = db.GetDataTable(string.Format(sql, NgayDK, MaLop));
                     
                    if (dtHP.Rows.Count > 0)
                    {
                        SoBuoiCL = decimal.Parse(dtHP.Rows[0]["SoBuoi"].ToString());
                        Tien1b = SoBuoiCL==0?0:(TienHP / SoBuoiCL);
                        SoBuoiDH = Tien1b==0?0:(Math.Round(ThucThu / Tien1b));
                        if (SoBuoiDH > SoBuoiCL)
                        {
                            drMaster["SoBuoiDH"] = SoBuoiCL;
                            SoBuoiDH = SoBuoiCL;
                        }
                        else
                            drMaster["SoBuoiDH"] = SoBuoiDH;
                        // Tính ngày học cuối
                        DataTable dtNgayKT = db.GetDataTable(string.Format("exec TinhNgayKT {0},'{1}', '{2}'", SoBuoiDH, NgayDK, MaLop));
                        if (dtNgayKT.Rows.Count == 0 || dtNgayKT.Rows[0]["NgayKT"].ToString() == "")
                        {
                            drMaster["NgayHocCuoi"] = DBNull.Value;
                        }
                        else
                            drMaster["NgayHocCuoi"] = DateTime.Parse(dtNgayKT.Rows[0]["NgayKT"].ToString());
                    }
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
