using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using Plugins;
using CDTDatabase;
using CDTLib;
using System.Windows.Forms;
using System.Data;

namespace TaoMaLopCT
{
    public class TaoMaLopCT:ICControl
    {

        private InfoCustomControl info = new InfoCustomControl(IDataType.SingleDt);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();


        #region ICControl Members

        public void AddEvent()
        {
            DataRow drMaster = (data.BsMain.Current as DataRowView).Row;
            if (drMaster.RowState == DataRowState.Deleted)
                return;
            data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(data.BsMain, new EventArgs());
        }

        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataTable dt = data.BsMain.DataSource as DataTable;
            if (dt == null)
                return;
            dt.ColumnChanged += new DataColumnChangeEventHandler(TaoMaLopCT_ColumnChanged);
        }

        void TaoMaLopCT_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            //if (e.Row.RowState == DataRowState.Modified
            //    && e.Row["MaNLop", DataRowVersion.Current] != e.Row["MaNLop", DataRowVersion.Original])
            //{
            //    //msg ...
            //    MessageBox.Show("Phần mềm không cho phép thay đổi mã nhóm lớp khi chỉnh sửa",Config.GetValue("PackageName").ToString());
            //    e.Row.CancelEdit();
            //    return;
            //}

            
            if (e.Column.ColumnName.ToUpper().Equals("MANLOP") && e.Row["MaNLop"].ToString() != "")
            {
                if (e.Row.RowState != DataRowState.Added)
                {

                    MessageBox.Show("Thay đổi nhóm lớp không hợp lệ", Config.GetValue("PackageName").ToString());
                    e.Row.CancelEdit();
                    return;
                }
                string malop = CreateMaLop(e.Row["MaNLop"].ToString());
                if (malop != "")
                {
                    e.Row["MaLop"] = malop;
                    e.Row.EndEdit();
                }
            }
        }

        string CreateMaLop(string MaNhomLop)
        {
            string MaCN = "";
            if (Config.GetValue("MaCN") != null)
                MaCN = Config.GetValue("MaCN").ToString();
            string dk = MaCN + MaNhomLop;
            //string sql = "select MaLop from DMHVCT where MaLop like '" + dk + "%' order by MaLop DESC ";
            string sql = "select MaLop, cast((substring(MaLop,len('" + dk + "')+1, len(MaLop)-len('" + dk + "'))) as int) as STT " +
                         "from DMHVCT where MaLop like '" + dk + "%' order by STT desc";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count == 0)
                dk = dk + "0001";
            else
            {
                string stt = dt.Rows[0]["STT"].ToString();
                //stt = stt.Replace(dk, "");
                if (stt == "")
                {
                    XtraMessageBox.Show("Tạo mã lớp không thành công!", Config.GetValue("PackageName").ToString());
                    return null;
                }
                else
                {
                    int sttLop = int.Parse(stt) + 1;
                    if (sttLop < 10)
                        dk = dk + "000" + sttLop.ToString();
                    else if (sttLop < 100)
                        dk = dk + "00" + sttLop;
                    else if (sttLop < 1000)
                        dk = dk + "0" + sttLop;
                    else                                                
                        dk = dk + sttLop.ToString();
                }
            }           
            return dk;
        }

        public DataCustomFormControl Data
        {
            set { data = value; }
        }

        public InfoCustomControl Info
        {
            get { return info; }
        }

        #endregion
    }
}
