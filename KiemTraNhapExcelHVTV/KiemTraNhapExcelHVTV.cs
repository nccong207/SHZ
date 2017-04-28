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
            //the datasource in this case is DataTable, not dataset, you knew already, why you write that:
            //DataSet dsData = _data.BsMain.DataSource as DataSet;
            

            _data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(_data.BsMain, new EventArgs());

            //you still need to repeat the definition of RowChanged event here again to run it in the first time the form load
            //if (dsData == null)
            //    return;
            //dsData.Tables[0].TableNewRow += new DataTableNewRowEventHandler(Check_DuplicateHVTV);
        }

        private void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataTable dsData = _data.BsMain.DataSource as DataTable;
            //use this event instead of TableNewRow
            dsData.RowChanged += DsDataOnRowChanged;
            //dsData.TableNewRow += new DataTableNewRowEventHandler(Check_DuplicateHVTV);
        }

        private void DsDataOnRowChanged(object sender, DataRowChangeEventArgs e)
        {
            //check the row is new row:
            //e.Action == DataRowAction.Add

            //cancel adding row: (please notice you need to cancel the adding row, not the existed row is duplicated like your below code
            //e.Row.RejectChanges();
        }

        private void Check_DuplicateHVTV(object sender, DataTableNewRowEventArgs e)
        {
            if (_data.BsMain.Current == null)
                return;

            DataRowView seletedRow = _data.BsMain.Current as DataRowView;

            DataRow test = e.Row;
            test.RejectChanges();
            if (seletedRow == null)
            {
                return;                
            }

            if (!string.IsNullOrEmpty(seletedRow["HVTVID"].ToString()))
            {
                return;                
            }

            //DataTable allRows = (_data.BsMain.DataSource as DataSet);

            string newName = seletedRow["TenHV"] != null ? seletedRow["TenHV"].ToString().Trim(): "";
            string newPhone = seletedRow["DienThoai"] != null ? seletedRow["DienThoai"].ToString(): "";
            if (!string.IsNullOrEmpty(newName))
            {
                DataRow[] dataRows = (_data.BsMain.DataSource as DataTable).Select("TenHV = '" + newName + "' and DienThoai = '" + newPhone + "'");
                if (dataRows.Length > 0)
                {

                    string msg = string.Format("Học viên {0} với SĐT {1} đã tồn tại trong hệ thống, bạn có muốn tiếp tục nhập không ?", newName, newPhone);

                        if (XtraMessageBox.Show(msg, Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo)
                            == DialogResult.No)
                            //xoa học viên => cực kỳ tầm bậy vì yêu cầu là kiểm tra trùng thì không add, chứ ko phải trùng thì xóa cái row có sẵn trong database mà trùng với row đang add :((
                           dataRows[0].Delete();
                        else
                        {
                            return;
                        }
                    }
                
            }

            

            //foreach (DataRow dr in allRows.Rows)
            //{
            //    string newName = seletedRow["TenHV"] != null ? seletedRow["TenHV"].ToString().Trim(): "";
            //    string newPhone = seletedRow["DienThoai"] != null ? seletedRow["DienThoai"].ToString(): "";

            //    string curName = dr["TenHV"] != null ? dr["TenHV"].ToString().Trim() : "";
            //    string curPhone = dr["DienThoai"] != null ? dr["DienThoai"].ToString().Trim() : "";

            //    if (newName.Equals(curName) && newPhone.Equals(curPhone))
            //    {
            //        string msg = "Học viên {} đã tồn tại trong hệ thống, bạn có muốn tiếp tục nhập không ?";

            //        if (XtraMessageBox.Show(msg, Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo)
            //            == DialogResult.No)
            //            break;
            //        else
            //        {
                        
            //        }
            //    }

              
            //}


          
            //string msg = "Bạn muốn chương trình tự động lấy tất cả số liệu đang mở theo kỳ này không?";
           
            //if ( XtraMessageBox.Show(msg, Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo)
            //    == DialogResult.No)
            //    return;


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
