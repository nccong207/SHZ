using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTLib;
using Plugins;
using CDTDatabase;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;

namespace TaoPhieuThu
{
    public class TaoPhieuThu : ICReport
    {
        private Database db = Database.NewDataDatabase();
        private DataCustomReport _data;
        private InfoCustomReport _info = new InfoCustomReport(IDataType.Report);
        GridView gvMain;

        #region ICReport Members

        public DataCustomReport Data
        {
            set { _data = value; }
        }

        public void Execute()
        {
            gvMain = (_data.FrmMain.Controls.Find("gridControlReport", true)[0] as GridControl).MainView as GridView;
            SimpleButton btnXL = _data.FrmMain.Controls.Find("btnXuLy", true)[0] as SimpleButton;
            btnXL.Click += new EventHandler(btnXL_Click);
        }

        public InfoCustomReport Info
        {
            get { return _info; }
        }

        #endregion

        void btnXL_Click(object sender, EventArgs e)
        {
            DataView dv = gvMain.DataSource as DataView;
            dv.RowFilter = "Chọn = 1";
            if (dv.Count == 0)
            {
                dv.RowFilter = "";
                XtraMessageBox.Show("Vui lòng chọn học viên", Config.GetValue("PackageName").ToString());
                return;
            }

            string sqlPT = @"  insert into mt11 (mt11id,mact,ngayct,soct,makh,manv,tkno,ttien,tenkh,hocvien,nhomkh,bpthu,nhanvien,diengiai)
                                values(@mt11id,'PT',@ngayct,@soct,@makh,'HP',@tkno,@ttien,@tenkh,@hocvien,@nhomkh,@bpthu,@nhanvien,@diengiai)";
            string[] paraNamePT = new string[]{"@mt11id","@ngayct","@soct","@makh","@tkno","@ttien","@tenkh","@hocvien","@nhomkh","@bpthu","@nhanvien","@diengiai"};

            string sqlCTPT = @"insert into dt11(dt11id,mt11id,makhct,ps,tkco,mabp,tenkhct,diengiaict)
                                values(@dt11id,@mt11id,@makhct,@ps,@tkco,@mabp,@tenkhct,@diengiaict)";
            string[] paraNameCTPT = new string[]{"@dt11id","@mt11id","@makhct","@ps","@tkco","@mabp","@tenkhct","@diengiaict"};

            string sqlMTDK = @"update mtdk set sophieuthu = '{0}' where hvid = '{1}'";

            foreach (DataRowView drv in dv)
            {
                db.EndMultiTrans();
                //Tạo phiếu thu
                string mt11id = Guid.NewGuid().ToString();
                string soct = TaoSoCT();
                string tkno = db.GetValue("select tk1 from dmnv where manv = 'hp'").ToString();
                string tkco = db.GetValue("select tkdu1 from dmnv where manv = 'hp'").ToString();
                string nhanvien = Config.GetValue("UserName").ToString();
                object[] paraValue = new object[] {mt11id,drv.Row["NgayDK"],soct,drv.Row["MaHV"],tkno,drv.Row["ThucThu"]
                         ,drv.Row["TenHV"],drv.Row["MaHV"],drv.Row["MaLop"],drv.Row["MaCNDK"],nhanvien,"Thu Học phí"};
                db.UpdateDatabyPara(sqlPT, paraNamePT, paraValue);
                object[] paraValue1 = new object[] { Guid.NewGuid(), mt11id, drv.Row["MaHV"], drv.Row["ThucThu"],tkco
                         ,drv.Row["MaCNDK"],drv.Row["TenHV"],"Thu Học phí"};
                db.UpdateDatabyPara(sqlCTPT,paraNameCTPT,paraValue1);
                //Cập nhập số ct vào mtdk
                db.UpdateByNonQuery(string.Format(sqlMTDK, soct, drv.Row["HVID"]));
            }
            //Xóa học viên đã copy
            DataTable dtChon = dv.ToTable();
            dv.RowFilter = "";
            dv.Sort = "MaHV";
            foreach (DataRow dr in dtChon.Rows)
            {
                dv.Delete(dv.Find(dr["MaHV"].ToString()));
            }
        }

        private string TaoSoCT()
        { 
            string sql = "", soctNew = "", mact = "", maCN = "", prefix = "";
            mact = "PT";
            if (Config.GetValue("MaCN") != null)
                maCN = Config.GetValue("MaCN").ToString();
            if (maCN != "")
                prefix = mact + maCN +"/";

            if (maCN == "")
                sql = "select Top 1 SoCT  from MT11 order by SoCT DESC";
            else
                sql = "select Top 1 SoCT, cast((substring(SoCT,len('" + prefix + "')+1, len(SoCT)-len('" + prefix + "'))) as int) as STT from MT11 where SoCT like '%" + prefix + "%' order by STT DESC";

            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count > 0)            
                soctNew = GetNewValue(dt.Rows[0]["SoCT"].ToString());
            else
            {
                //if (maCN == "")
                //{
                //    sql = "select EditMask from sysField F inner join sysTable T on F.sysTableID=T.sysTableID" +
                //          " where T.sysTableID = '" + _data.DrTableMaster["sysTableID"].ToString() + "' and F.FieldName = 'SoCT'";
                //    dt = dbCDT.GetDataTable(sql);
                //    if (dt.Rows.Count > 0)
                //        soctNew = GetNewValue(dt.Rows[0]["EditMask"].ToString());
                //}
                //else
                //{
                    soctNew = GetNewValue(prefix+"000");
                //}
            }
            //if (soctNew != "")
            //    drMaster["SoCT"] = soctNew;
            return soctNew;
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
    }
}
