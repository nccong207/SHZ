using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CDTDatabase;
using CDTLib;
using DevExpress.XtraEditors;
using Plugins;

namespace KiemTraNhapExcelHVTV
{
    public class KiemTraNhapExcelHVTV: ICControl
    {
        private InfoCustomControl _info = new InfoCustomControl(IDataType.Single);
        private DataCustomFormControl _data;
        Database db = Database.NewDataDatabase();
        public void AddEvent()
        {
            _data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(_data.BsMain, new EventArgs());
        }

        private void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataTable dsData = _data.BsMain.DataSource as DataTable;
            dsData.RowChanged += DsDataOnRowChanged;
        }

        private void DsDataOnRowChanged(object sender, DataRowChangeEventArgs e)
        {
            //check the row is new row:
            if (e.Action != DataRowAction.Add)
                return;

            DataRow addedRow = e.Row;

            string newName = addedRow["TenHV"] != null ? addedRow["TenHV"].ToString().Trim() : "";
            string newPhone = addedRow["DienThoai"] != null ? addedRow["DienThoai"].ToString() : "";
            if (!string.IsNullOrEmpty(newName))
            {
                DataRow[] dataRows = (_data.BsMain.DataSource as DataTable).Select("TenHV = '" + newName + "' and DienThoai = '" + newPhone + "'", null, DataViewRowState.Unchanged);
                if (dataRows.Length > 0)
                {

                    string msg = string.Format("Học viên {0} với SĐT {1} đã tồn tại trong hệ thống, bạn có muốn tiếp tục nhập không ?", newName, newPhone);

                    if (XtraMessageBox.Show(msg, Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo)
                        == DialogResult.No)
                        //huy bo them hoc vien..
                        e.Row.RejectChanges();
                }

            }
        }

        public InfoCustomControl Info
        {
            get { return _info; }
        }

        public DataCustomFormControl Data
        {
             set { _data = value; }
        }
    }
}
