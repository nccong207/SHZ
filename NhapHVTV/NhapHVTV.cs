using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using CDTLib;
using CDTDatabase;
using Plugins;
using System.Globalization;
using DevExpress.XtraEditors.Repository;
using System.Data;
using System.Threading;
using DevExpress.XtraLayout;

namespace NhapHVTV
{

    public class NhapHVTV:ICControl
    {
        private InfoCustomControl info = new InfoCustomControl(IDataType.SingleDt);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();
        LayoutControl lcMain;
        //GridLookUpEdit glNVCS;
        //GridLookUpEdit glTenNVCS;
        GridLookUpEdit glNVTV;
        TextEdit TenHV;
        DataRow drMaster;
        //DataRow drCur;
        #region ICControl Members

        public void AddEvent()
        {
            //data.FrmMain.Shown += new EventHandler(FrmMain_Shown);

            //Viết hoa chữ cái đầu tiên của họ tên học viên
            DataRow drMaster = (data.BsMain.Current as DataRowView).Row;
            TenHV = data.FrmMain.Controls.Find("TenHV", true)[0] as TextEdit;
            //glNVCS = data.FrmMain.Controls.Find("NVCS", true)[0] as GridLookUpEdit;
            //glTenNVCS = data.FrmMain.Controls.Find("NVCS_HoTen", true)[0] as GridLookUpEdit;
            glNVTV = data.FrmMain.Controls.Find("MaNVTV", true)[0] as GridLookUpEdit;
            data.FrmMain.Shown += new EventHandler(FrmMain_Shown);

            if (drMaster.Table.Columns.Contains("TenHV"))
            {
                TenHV.LostFocus += new EventHandler(TenHV_LostFocus);
            }
            lcMain = data.FrmMain.Controls.Find("lcMain", true)[0] as LayoutControl;
            data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(data.BsMain, new EventArgs());
        }

        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataTable ds = data.BsMain.DataSource as DataTable;
            if (data.BsMain.Current != null)
                drMaster = (data.BsMain.Current as DataRowView).Row;
            ds.ColumnChanged += new DataColumnChangeEventHandler(NhapHVTV_ColumnChanged);
        }

        void NhapHVTV_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            object obj = DBNull.Value;
            if (glNVTV.EditValue != DBNull.Value)
            {
                obj = db.GetValue("select chonnvcs from dmnvien where manv='" + glNVTV.EditValue.ToString() +"'");
            }
            if (e.Column.ColumnName.ToUpper().Equals("MANVTV"))
            {
                if(obj != null)
                    e.Row["NVCS"] = obj.ToString();
            }
            if (e.Column.ColumnName.ToUpper().Equals("NVCS"))
            {
                if (obj != null)
                    if (e.Row["NVCS"].ToString() != obj.ToString())
                    {
                        if(!bool.Parse(Config.GetValue("admin").ToString()))
                        {
                                e.Row["NVCS"] = obj.ToString();
                        }
                    }
            }
            e.Row.EndEdit();
        }

        void FrmMain_Shown(object sender, EventArgs e)
        {

            DataRow drMaster = (data.BsMain.Current as DataRowView).Row;
            if (drMaster.RowState == DataRowState.Added)
            {
                if (Config.GetValue("username") != null)
                {
                    drMaster["MaNVTV"] = Config.GetValue("username").ToString();
                }
                if (Config.GetValue("MaCN") != null)
                {
                    drMaster["MaCN"] = Config.GetValue("MaCN").ToString();
                }
            }

        }

        void TenHV_LostFocus(object sender, EventArgs e)
        {
            TextEdit txtTenHV = sender as TextEdit;
            if (txtTenHV.Properties.ReadOnly)
                return;
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo txtInfo = cultureInfo.TextInfo;            
            txtTenHV.Text = txtInfo.ToTitleCase(txtTenHV.Text.ToLower());
        }

        //void FrmMain_Shown(object sender, EventArgs e)
        //{
        //    DataRow drMaster = (data.BsMain.Current as DataRowView).Row;
        //    if (drMaster.RowState == DataRowState.Added)
        //    {
        //        if (Config.GetValue("username") != null)
        //        {
        //            drMaster["MaNVTV"] = Config.GetValue("username").ToString();
        //        }
        //        if (Config.GetValue("MaCN") != null)
        //        {
        //            drMaster["MaCN"] = Config.GetValue("MaCN").ToString();
        //        }
        //    }
        //}

        public DataCustomFormControl Data
        {
            set { data=value; }
        }

        public InfoCustomControl Info
        {
            get { return info; }
        }

        #endregion
    }
}
