using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using FormFactory;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System.Data;
using DevExpress.XtraGrid;
using System.Windows.Forms;
using CDTLib;

namespace LayThuongCS
{
    public class LayThuongCS : ICForm
    {
        private DataCustomFormControl _data;
        private List<InfoCustomForm> _lstInfo = new List<InfoCustomForm>();
        GridView gvDS;
        SpinEdit seThang;
        ReportPreview frmDS;

        public LayThuongCS()
        {
            InfoCustomForm info1 = new InfoCustomForm(IDataType.Single, 0, "Cập nhật thưởng chiêu sinh", "", "LuongNV");
            _lstInfo.Add(info1);
            InfoCustomForm info2 = new InfoCustomForm(IDataType.Single, 1, "Cập nhật thưởng lợi nhuận", "", "LuongNV");
            _lstInfo.Add(info2);
        }
        #region ICForm Members

        public DataCustomFormControl Data
        {
            set { _data = value; }
        }

        public void Execute(int menuID)
        {
            object o = Config.GetValue("ThangLuong");   //tham so nay duoc truyen tu ICC tinh luong qua
            if (o == null || o.ToString() == "")
            {
                XtraMessageBox.Show("Không xác định được tháng tính lương",
                    Config.GetValue("PackageName").ToString());
                return;
            }
            int thang, nam;     //biến lưu tháng, năm tính thưởng CS (do tính lương tháng này thì lấy theo thưởng CS tháng trước)
            nam = Convert.ToInt32(Config.GetValue("NamLamViec"));
            if (Convert.ToInt32(o) == 1)
            {
                thang = 12;
                nam = nam - 1;
            }
            else
                thang = Convert.ToInt32(o) - 1;
            Config.NewKeyValue("@Thang", thang);    //chuyen tham so luong vao tham so bao cao thuong chieu sinh
            Config.NewKeyValue("@Nam", nam);
            string sysReportID = menuID == 0 ? "1663" : "1666";
            frmDS = FormFactory.FormFactory.Create(FormType.Report, sysReportID) as ReportPreview;
            gvDS = (frmDS.Controls.Find("gridControlReport", true)[0] as GridControl).MainView as GridView;
            seThang = frmDS.Controls.Find("Thang", true)[0] as SpinEdit;
            seThang.Properties.ReadOnly = true;
            SpinEdit seNam = frmDS.Controls.Find("Nam", true)[0] as SpinEdit;
            seNam.Properties.ReadOnly = true;

            SimpleButton btnXuLy = (frmDS.Controls.Find("btnXuLy", true)[0] as SimpleButton);
            btnXuLy.Text = "Cập nhật";
            btnXuLy.Click += new EventHandler(btnXuLy_Click);
            frmDS.WindowState = FormWindowState.Maximized;
            frmDS.ShowDialog();
        }

        void btnXuLy_Click(object sender, EventArgs e)
        {
            if (!seThang.Properties.ReadOnly)
                return;
            DataTable dtDS = (gvDS.DataSource as DataView).Table;

            dtDS.AcceptChanges();
            DataRow[] drs = dtDS.Select("Chọn = 1");
            if (drs.Length == 0)
            {
                XtraMessageBox.Show("Bạn chưa chọn nhân viên nào để cập nhật", Config.GetValue("PackageName").ToString());
                return;
            }
            
            DataTable dtLuong = _data.BsMain.DataSource as DataTable;
            string dk = "Nam = " + Config.GetValue("NamLamViec").ToString() + " and Thang = " + Config.GetValue("ThangLuong").ToString();
            drs = dtLuong.Select(dk);
            string colunmName = (frmDS.Data.DrTable["sysReportID"].ToString() == "1663") ? "ThuongCN" : "ThuongLN";
            foreach (DataRow dr in drs)
            {
                object o = dtDS.Compute("sum([Tiền thưởng])", "Chọn = 1 and MaNV = '" + dr["MaNV"].ToString() + "'");
                if (o != null && o.ToString() != "")
                    dr[colunmName] = o;
            }
            frmDS.Close();
        }

        public List<InfoCustomForm> LstInfo
        {
            get { return _lstInfo; }
        }

        #endregion
    }
}
