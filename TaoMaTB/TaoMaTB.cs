using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevExpress;
using DevExpress.XtraEditors;
using Plugins;
using CDTLib;
using CDTDatabase;
using System.Data;

namespace TaoMaTB
{
    public class TaoMaTB:ICControl
    {
        #region ICControl Members

        private InfoCustomControl info = new InfoCustomControl(IDataType.SingleDt);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();

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
            dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);
        }

        void dt_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Row.RowState == DataRowState.Deleted)
                return;

            if (e.Column.ColumnName.ToUpper().Equals("LOAITB"))
            {
                if (Config.GetValue("MaCN") == null)
                {
                    XtraMessageBox.Show("Không nhận diện được chi nhánh đăng nhập.");
                    return;
                }
                if (e.Row.RowState == DataRowState.Added)                                    
                    e.Row["MaTB"] = CreateMaTB(Config.GetValue("MaCN").ToString(), e.Row["LoaiTB"].ToString());
                else // xóa ko chạy vào
                {
                    bool bl = CheckUse(e.Row["MaTB"].ToString());
                    if (bl)                        
                        e.Row["MaTB"] = CreateMaTB(Config.GetValue("MaCN").ToString(), e.Row["LoaiTB"].ToString());                    
                }
                e.Row.EndEdit();
            } 
        }

        bool CheckUse(string matb)
        {
            string sql = "select matb from suachuatb where matb ='" + matb + "'" +
                        " union all " +
                        " select matb from thanhlytb where matb = '"+matb+"'";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                XtraMessageBox.Show("Thiết bị này đã được sử dụng cho các chứng từ khác.\nVui lòng xóa các dữ liệu liên quan nếu muốn sửa.");
                return false;
            }
            else
                return true;
        }

        string CreateMaTB(string MaCN, string LoaiTB)
        {
            string dk = MaCN + LoaiTB;
            string sql = @"select MaTB from NhapTB 
                           where MaTB like '" + dk + @"%'
                           and PATINDEX('[a-z]%',substring(MaTB,len('" + dk + @"')+1,len(MaTB)-1)) = 0
                           order by MaTB DESC ";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count == 0)
                dk = dk + "001";
            else
            {
                string stt = dt.Rows[0]["MaTB"].ToString();
                stt = stt.Replace(dk, "");
                if (stt == "")
                {
                    XtraMessageBox.Show("Tạo mã lớp không thành công!", Config.GetValue("PackageName").ToString());
                    return null;
                }
                else
                {
                    int sttMa = int.Parse(stt) + 1;
                    if (sttMa < 10)
                        dk = dk + "00" + sttMa.ToString();
                    else
                        dk = dk + "0" + sttMa.ToString();
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
