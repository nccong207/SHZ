using System;
using System.Collections.Generic;
using System.Data;
using CDTDatabase;
using CDTLib;
using DevExpress.XtraEditors;
using Plugins;

namespace TaoPhieuChi
{
    public class TaoPhieuChi: ICData
    {
        private InfoCustomData info = new InfoCustomData(IDataType.MasterDetailDt);
        private DataCustomData data;
        Database db = Database.NewDataDatabase();

        bool blFlag = true;
        private string sopc = "";
        public void ExecuteBefore()
        {
            DataRow drMaster = data.DsData.Tables[0].Rows[data.CurMasterIndex];

            if ((drMaster.RowState == DataRowState.Added || drMaster.RowState == DataRowState.Modified)
                && drMaster["SoPC"].ToString() == "")
            {
                TaoPhieuChiPL(drMaster);
                drMaster["SoPC"] = blFlag ? sopc : string.Empty;
            }
        }

        public void ExecuteAfter()
        {
        }

        private void TaoPhieuChiPL(DataRow drMaster)
        {
            string maCN = drMaster["MaCN"].ToString();
            string mtplid = drMaster["MTPLID"].ToString();
            string soct = LaySoCT(maCN, "PC");
            sopc = soct;
            DateTime ngayct = DateTime.Parse(drMaster["NgayCT"].ToString());
            string macntt = drMaster["MaCNTT"].ToString();
            string mt12id = Guid.NewGuid().ToString();

            DataRow detailBP = GetDMBP(maCN);
            string diachi = detailBP["DiaChi"].ToString();
            string nhomluong = drMaster["NhomLuong"].ToString().Equals("CT") ? "CT" :drMaster["NhomLuong"].ToString().Equals("GV") ? "LUONGGV" : "LUONGNV";
            string tkno = GetTKNo(nhomluong);
            string diengiai = "Lương T." + drMaster["Thang"].ToString();
            double tongtien = Double.Parse(drMaster["TongTien"].ToString());
            string tenBp = detailBP["TenKH"].ToString();

            string sqlM12 = string.Format(@"INSERT INTO MT12(MT12ID, NgayCt, MaCT, SoCt, MaKH, DiaChi, OngBa, MaNV, DienGiai, MaNT, TyGia, TKCo, TtienNt, Ttien, TkThue, TTThue, TTienCT, TenKH, RefValue, NguoiLap, BPChi)
                                            VALUES('{0}', '{1}', 'PC', '{2}','{3}',N'{4}', NULL, '{5}',N'{6}', 'VND', 1.0, 1111, 0.0, {7}, 1331, 0.0, {8}, N'{9}', NULL, '{10}','{11}');"
                                            , mt12id, ngayct, soct, macntt, diachi, nhomluong, diengiai, tongtien, tongtien, tenBp, macntt, maCN);

            DataRow[] dv = data.DsData.Tables[1].Select("MTPLID = '" + mtplid + "'");

            List<PhieuChi> PhieuChiLst = new List<PhieuChi>();
            foreach (DataRow rowView in dv)
            {
                string maCN2 = rowView["MaCN"].ToString();
                var item = PhieuChiLst.Find(s => s.MaCN.Equals(maCN2));
                if (item != null)
                {
                    item.Money += Convert.ToDouble(rowView["TongLuong"].ToString());
                }
                else
                {
                    PhieuChi newPC = new PhieuChi();
                    newPC.MaCN = maCN2;
                    newPC.Money = Convert.ToDouble(rowView["TongLuong"].ToString());
                    PhieuChiLst.Add(newPC);
                }
            }

            if (PhieuChiLst.Count > 0 )
            {
                foreach (PhieuChi phieuChi in PhieuChiLst)
                {
                    string dt12id = Guid.NewGuid().ToString();

                    string tenBpPC = null;
                    if (!string.IsNullOrEmpty(phieuChi.MaCN))
                    {
                        DataRow detailBPPC = GetDMBP(phieuChi.MaCN);
                        tenBpPC = detailBPPC["TenKH"].ToString();
                    }
                   

                    sqlM12 += string.Format(@"INSERT INTO DT12(DT12ID,MT12ID,DienGiaiCt,MaKHCt,PsNT,Ps,TkNo,MaPhi,MaVV,MaBP,MaSP,MaCongTrinh,TenKHCt)
                                             VALUES ('{0}','{1}',N'{2}','{3}',0.0,'{4}','{5}', NULL,'{6}','{7}', NULL, NULL,N'{8}');"
                                            , dt12id, mt12id, "Chi tiền lương", phieuChi.MaCN, phieuChi.Money, tkno, "LUONGGV", phieuChi.MaCN, tenBpPC);
                }
            }

            //Insert BLTK
            sqlM12 += string.Format(@"INSERT INTO BLTK (MaSP, MaCongTrinh, MaCT, MTID, SoCT, NgayCT, DienGiai, MaKH, TenKH, TK, TKDu, PsNo, PsCo, NhomDK, MTIDDT, MaBP, MaVV, MaPhi, PsNoNT, PsCoNT, MaNT, OngBa, TyGia)
                                        SELECT  d.MaSP, d.MaCongTrinh, m.MaCT, m.MT12ID, m.SoCT, m.NgayCT, d.DienGiaiCt, d.MaKHCt, d.TenKHCt, d.TkNo, m.TkCo, d.Ps, 0, 'PC', d.DT12ID, d.MaBP, d.MaVV, d.MaPhi, d.PsNT, 0.0, m.MaNT, m.OngBa, m.TyGia
                                        FROM    DT12 AS d INNER JOIN MT12 AS m ON d.MT12ID = m.MT12ID
                                        WHERE	m.MT12ID = '{0}';

                                    INSERT INTO BLTK(MaSP, MaCongTrinh, MaCT, MTID, SoCT, NgayCT, DienGiai, MaKH, TenKH, TK, TKDu, PsNo, PsCo, NhomDK, MTIDDT, MaBP, MaVV, MaPhi, PsNoNT, PsCoNT, MaNT, OngBa, TyGia)
                                        SELECT  d.MaSP,  d.MaCongTrinh, m.MaCT, m.MT12ID, m.SoCT, m.NgayCT, d.DienGiaiCt, m.MaKH, m.TenKH, m.TKCo, d.TkNo, 0.0, d.Ps, 'PC', d.DT12ID, d.MaBP, d.MaVV, d.MaPhi, 0.0, d.PsNT, m.MaNT, m.OngBa, m.TyGia
                                        FROM    DT12 AS d INNER JOIN MT12 AS m ON d.MT12ID = m.MT12ID
                                        WHERE	m.MT12ID = '{0}';", mt12id);
            db.BeginMultiTrans();
            blFlag = db.UpdateByNonQuery(sqlM12);
            if (blFlag)
                db.EndMultiTrans();
            else
            {
                info.Result = false;
                XtraMessageBox.Show("Lỗi tạo phiếu chi", Config.GetValue("PackageName").ToString());
            }
        }

        private string LaySoCT(string MaCN, string MaCT)
        {
            string prefix = MaCT + MaCN + "/";
            string sql = "select Top 1 SoCT, cast((substring(SoCT,len('" + prefix + "')+1, len(SoCT)-len('" + prefix + "'))) as int) as STT from MT12 where SoCt like '%" + prefix + "%' order by STT DESC";

            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count > 0)
                return GetNewValue(dt.Rows[0]["SoCT"].ToString());
            else
            {
                return GetNewValue(prefix + "000");
            }
        }

        public class PhieuChi
        {
            public string MaCN { get; set; }
            public double Money { get; set; }
        }
        private DataRow GetDMBP(string maCN)
        {
            string sqlDMBP = string.Format("SELECT *  FROM DMKH WHERE MaKH = '{0}'", maCN);
            DataTable dt = db.GetDataTable(sqlDMBP);
            if (dt.Rows.Count > 0)
                return dt.Rows[0];
            else
            {
                return null;
            }
        }

        private string GetTKNo(string maMV)
        {
            string sqlDMNV = string.Format("SELECT *  FROM DMNV WHERE MaNV = '{0}'", maMV);
            DataTable dt = db.GetDataTable(sqlDMNV);
            if (dt.Rows.Count > 0)
                return dt.Rows[0]["TK1"].ToString();
            else
            {
                return null;
            }
        }

        private string GetNewValue(string OldValue)
        {
            try
            {
                int i = OldValue.Length - 1;
                for (; i > 0; i--)
                    if (!Char.IsNumber(OldValue, i))
                        break;
                if (i == OldValue.Length - 1)
                {
                    int NewValue = Int32.Parse(OldValue) + 1;
                    return NewValue.ToString();
                }
                string PreValue = OldValue.Substring(0, i + 1);
                string SufValue = OldValue.Substring(i + 1);
                int intNewSuff = Int32.Parse(SufValue) + 1;
                string NewSuff = intNewSuff.ToString().PadLeft(SufValue.Length, '0');
                return (PreValue + NewSuff);
            }
            catch
            {
                return string.Empty;
            }
        }
        public InfoCustomData Info
        {
            get { return info; }
        }

        public DataCustomData Data
        {
            set { data = value; }
        }
    }
}
