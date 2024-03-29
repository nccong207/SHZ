using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Configuration;
using DevExpress.XtraEditors;
using DevExpress.XtraNavBar;
using DevExpress.XtraTreeList.Nodes;
using FormFactory;
using CDTControl;
using CDTLib;
using DataFactory;
using CDTSystem;
using Plugins;
using HtmlHelp;
using HtmlHelp.ChmDecoding;
using HtmlHelp.UIComponents;
using System.IO;
using DevExpress.XtraEditors.Repository;
using System.Globalization;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;

namespace CDT
{
    public partial class Main : DevExpress.XtraEditors.XtraForm
    {
        private Command _cmd;
        string sysPackageID;
        DataTable dtMenu;
        SysMenu _sysMenu = new SysMenu();
        PluginManager pm = new PluginManager();
        private List<string> attachFiles = new List<string>();
        private DataTable dtVideo = new DataTable();
        private int _delayPopup = 0;
        private bool _isRefresh = false;
        private bool _vi = Config.GetValue("Language").ToString() == "0";
        FrmVisualUI currentVs;
        private string _oldPeriod = "";
        private string _oldYear = "";
        private int barId1 = 0, barId2 = 0;
        #region htmlHelp

        string _prefDumpOutput = "";
        string _prefURLPrefix = "mk:@MSITStore:";
        bool _prefUseHH2TreePics = false;
        DumpingInfo _dmpInfo;
        #endregion

        public Main(DataRow drUser)
        {
            InitializeComponent();
            this.barMainMenu.Appearance.Font = SystemFonts.MenuFont;
            this.barMainMenu.Appearance.Options.UseFont = true;
            sysPackageID = drUser["sysPackageID"].ToString();
            _cmd = new Command(pm, this, _sysMenu);
            UpdateDatabases();
            //_sysMenu.SynchronizeWithPlugins(pm);
            InitializeMenu();
            InitializeForm(drUser);
            GetDatabases();
            GetStatusBarInfo();
            //InitializeHelp(tocTree2, Application.StartupPath + @"\TLKT.chm");
            backgroundWorker1.RunWorkerAsync(drUser);
        }

        //private void CustomerLogo()
        //{
        //    object o = Config.GetValue("Logo");
        //    if (o == null || o.ToString() == "")
        //        cLogo.Visible = false;
        //    else
        //    {
        //        byte[] b = (byte[])o;
        //        Image i = GetImage(b);
        //        cLogo.Image = i;
        //    }
        //}

        private void Main_Load(object sender, EventArgs e)
        {
            if (Config.GetValue("Language").ToString() == "1")
            {
                Translate();
            }
            //CustomerLogo();
        }

        private void GetStatusBarInfo()
        {
            object o = Config.GetValue("FullName");
            if (o == null || o.ToString() == "")
                bsiCurrentUser.Caption += Config.GetValue("UserName").ToString();
            else
                bsiCurrentUser.Caption += o.ToString();
            bsiLoginTime.Caption += DateTime.Now.ToString("hh:mm dd/MM/yyyy");
            bsiCurrentPath.Caption += Application.StartupPath;
            bsiCurrentPath.Alignment = BarItemLinkAlignment.Right;
        }

        private void UpdateDatabases()
        {
            if (File.Exists("UpdateDb.sql"))
            {
                string sql = File.ReadAllText("UpdateDb.sql");
                SysPackage sp = new SysPackage();
                if (sp.UpdateDb(sql))
                    File.Delete("UpdateDb.sql");
            }
        }

        private void GetDatabases()
        {
            string sysUserID = Config.GetValue("sysUserID").ToString();
            SysPackage sp = new SysPackage();
            DataTable dtDb = sp.GetDatabases2(sysUserID);
            if (dtDb.Rows.Count <= 1)
            {
                beiDb.Visibility = BarItemVisibility.Never;
                return;
            }
            riLueDb.DataSource = dtDb;
            riLueDb.ValueMember = "DbName";
            riLueDb.DisplayMember = "DbName";
            riLueDb.BestFit();
            riLueDb.EditValueChanged += new EventHandler(riLueDb_EditValueChanged);
            if (Config.GetValue("MaCN") != null)
                beiDb.EditValue = Config.GetValue("MaCN");
            else
                beiDb.EditValue = Config.GetValue("DbName");
        }

        void riLueDb_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit lue = sender as LookUpEdit;
            string oldvalue = Config.GetValue("MaCN") != null ? Config.GetValue("MaCN").ToString() : Config.GetValue("DbName").ToString();
            DataRowView drOldDb = riLueDb.GetDataSourceRowByKeyValue(oldvalue) as DataRowView;
            DataRowView drDb = riLueDb.GetDataSourceRowByKeyValue(lue.EditValue) as DataRowView;
            if (drDb == null)
                return;
            if (drDb.Row.Table.Columns.Contains("IsAdmin"))     //kiem tra phien ban CDT co phan quyen theo DbName khong?
                if ((!Boolean.Parse(drDb["IsAdmin"].ToString()) || !Boolean.Parse(drOldDb["IsAdmin"].ToString()))
                    && !SysUser.ComparePermission(drOldDb["sysUserSiteID"], drDb["sysUserSiteID"]))
                {
                    XtraMessageBox.Show("Bộ số liệu mới chọn không cùng quyền sử dụng với bộ số liệu trước!\n" +
                        "Hệ thống cần khởi động lại, vui lòng chọn bộ số liệu tại màn hình đăng nhập!", Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK);
                    Config.NewKeyValue("NoBackup", 1);
                    Application.Restart();
                }
            string structCnn = Config.GetValue("StructConnection").ToString();
            string structDb = Config.GetValue("StructDb").ToString();
            string CompanyName = drDb["CompanyName"].ToString();
            string MST = drDb["CPUID"].ToString();
            string dbName;
            if (drDb.DataView.Table.Columns.Contains("DbName2") && drDb["DbName2"] != DBNull.Value)
            {
                dbName = drDb["DbName2"].ToString();    //lay dbname theo dbname2
                Config.NewKeyValue("MaCN", drDb["DbName"].ToString());
            }
            else
                dbName = drDb["DbName"].ToString();    //lay dbname theo dbname
            Config.NewKeyValue("DbName", dbName);
            string DataConnection;    //dbname nay dung trong chuoi ket noi
            if (drDb["DbRemote"] == DBNull.Value)
                DataConnection = structCnn.Replace(structDb, dbName);
            else
                DataConnection = drDb["DbRemote"].ToString() + ";Database=" + dbName;
            string dbNameDK = drDb["DbName"].ToString();            //dbname nay dung de kiem tra license
            if (Config.GetValue("SiteCode").ToString() == "CDT")
                DataConnection = structCnn;
            Config.NewKeyValue("DataConnection", DataConnection);
            Config.NewKeyValue("MaSoThue", MST);
            Config.NewKeyValue("TenCongTy", CompanyName);
            if (drDb.DataView.Table.Columns.Contains("Logo"))
                Config.NewKeyValue("Logo", drDb["Logo"]);
            SysConfig sc = new SysConfig();
            sc.InitSysvar();
            //kiem tra license
            string key = drDb["RegisterNumber"].ToString();
            string siteName = this.Text.Substring(0, this.Text.IndexOf(" -"));
            string subTitle = "";
            if (key != "")
            {
                CDTLicense Cpu = new CDTLicense(CompanyName + dbNameDK, MST);
                if (key == Cpu.KeyString)
                {
                    Config.NewKeyValue("Licensed", 1);
                    subTitle = CompanyName;
                }
                else
                {
                    Config.NewKeyValue("Licensed", 0);
                    subTitle = dbName.Contains("DEMO") ? "Số liệu tham khảo" : "Phiên bản chưa đăng ký";
                    if (!_vi)
                        subTitle = UIDictionary.Translate(subTitle);
                }
            }
            else
            {
                Config.NewKeyValue("Licensed", 0);
                subTitle = dbName.Contains("DEMO") ? "Số liệu tham khảo" : "Phiên bản chưa đăng ký";
                if (!_vi)
                    subTitle = UIDictionary.Translate(subTitle);
            }
            this.Text = siteName + " - " + subTitle;
            iRegister.Enabled = (!Config.GetValue("DbName").ToString().Contains("DEMO") && Config.GetValue("Licensed").ToString() == "0");
            AddYearForAccounting();
        }

        private void Translate()
        {
            for (int i = 0; i < barManagerMain.Items.Count; i++)
                barManagerMain.Items[i].Caption = UIDictionary.Translate(barManagerMain.Items[i].Caption.Replace("&",""));
            barMdi.Caption = UIDictionary.Translate(barMdi.Caption.Replace("&",""));
            for (int i = 0; i < dockManager1.Panels.Count; i++)
            {
                //dockManager1.Panels[i].Text = UIDictionary.Translate(dockManager1.Panels[i].Text);
                DevLocalizer.Translate(dockManager1.Panels[i]);
            }
        }

        private void InitializeForm(DataRow drUser)
        {
            //if (!Boolean.Parse(drUser["CoreAdmin"].ToString()))
                iCheckData.Visibility = BarItemVisibility.Never;
            if (!Boolean.Parse(Config.GetValue("Admin").ToString()))
            {
                iNewDb.Visibility = BarItemVisibility.Never;
                iUserTrace.Visibility = BarItemVisibility.Never;
                iCollectData.Visibility = BarItemVisibility.Never;
                iUserConfig.Visibility = BarItemVisibility.Never;
            }
            this.Text = drUser["SiteName"].ToString();
            string subTitle;
            if (Config.GetValue("Licensed").ToString() == "1")
                subTitle = Config.GetValue("TenCongTy").ToString();
            else
            {
                subTitle = Config.GetValue("DbName").ToString().Contains("DEMO") ? "Số liệu tham khảo" : "Phiên bản chưa đăng ký";
                if (!_vi)
                    subTitle = UIDictionary.Translate(subTitle);
            }
            if (Config.GetValue("SiteCode").ToString().ToUpper() == "QLCV")
            {
                iBackup.Visibility = BarItemVisibility.Never;
                iRestore.Visibility = BarItemVisibility.Never;
                iCollectData.Visibility = BarItemVisibility.Never;
                iTransferData.Visibility = BarItemVisibility.Never;
                iSyncData.Visibility = BarItemVisibility.Never;
                iExportData.Visibility = BarItemVisibility.Never;
                Config.NewKeyValue("NoBackup", 1);
            }
            iNewDb.Visibility = Config.GetValue("SiteCode").ToString() == "HTA" ? BarItemVisibility.Always : BarItemVisibility.Never;
            this.Text += " - " + subTitle;
        }

        private void AddPeriodForAccounting()
        {
            object o = Config.GetValue("ThangLV");
            if (o != null && o.ToString() == "1")  //lay theo 12 thang, khong lay tu BLTK
            {
                if (riKyKeToan.Items.Count > 0)
                {
                    RefreshDataForPeriod();
                    return;
                }
                beiKyKeToan.Caption = "Tháng";
                for (int i = 1; i < 13; i++)
                    riKyKeToan.Items.Add(i);
            }
            else
            {
                SysPackage sp = new SysPackage();
                DataTable dtData = sp.GetPeriodForAccounting();
                if (dtData == null) //neu khong phai la PMKT thi khong chay tiep
                {
                    beiKyKeToan.Visibility = BarItemVisibility.Never;
                    beiNamLV.Visibility = BarItemVisibility.Never;
                    bbiDashboard.Visibility = BarItemVisibility.Never;
                    return;
                }
                _oldPeriod = "";
                riKyKeToan.Items.Clear();
                foreach (DataRow dr in dtData.Rows)
                {
                    riKyKeToan.Items.Add(dr[0]);
                }
            }

            riKyKeToan.EditValueChanged += new EventHandler(riKyKeToan_EditValueChanged);
            if (riKyKeToan.Items.Count == 0)
            {
                beiKyKeToan.EditValue = null;
                Config.NewKeyValue("KyKeToan", null);
            }
            else
            {
                if (o != null && o.ToString() == "1")           //lay mac dinh ban dau theo thang hien tai
                    beiKyKeToan.EditValue = DateTime.Today.Month;
                else
                    beiKyKeToan.EditValue = riKyKeToan.Items[riKyKeToan.Items.Count - 1];   //thang cuoi trong BLTK
                SavePeriodConfig(beiKyKeToan.EditValue);
            }
            RefreshDataForPeriod();
        }

        private void AddYearForAccounting()
        {
            SysPackage sp = new SysPackage();
            DataTable dtData = sp.GetYearForAccounting();
            if (dtData == null)
            {
                beiKyKeToan.Visibility = BarItemVisibility.Never;
                beiNamLV.Visibility = BarItemVisibility.Never;
                bbiDashboard.Visibility = BarItemVisibility.Never;
                return;
            }
            riNamLV.Items.Clear();
            foreach (DataRow dr in dtData.Rows)
            {
                riNamLV.Items.Add(dr[0]);
            }
            riNamLV.EditValueChanged += new EventHandler(riNamLV_EditValueChanged);
            object o = riNamLV.Items[riNamLV.Items.Count - 1]; ;
            beiNamLV.EditValue = o;
            Config.NewKeyValue("NamLamViec", o);
            AddPeriodForAccounting();
        }

        void riNamLV_EditValueChanged(object sender, EventArgs e)
        {
            object o = (sender as ComboBoxEdit).EditValue;
            if (o != null && o.ToString() == _oldYear)
                return;
            _oldYear = o.ToString();
            Config.NewKeyValue("NamLamViec", o);
            AddPeriodForAccounting();
        }

        private void SavePeriodConfig(object period)
        {
            Config.NewKeyValue("KyKeToan", period);
            Config.NewKeyValue("@Thang", period);
            Config.NewKeyValue("@Thang1", period);
            Config.NewKeyValue("@Thang2", period);
            Config.NewKeyValue("@TuThang", period);
            Config.NewKeyValue("@DenThang", period);
            DateTime d1 = DateTime.Parse(period.ToString() + "/01/" + Config.GetValue("NamLamViec").ToString());
            DateTime d2 = d1.AddMonths(1).AddDays(-1);
            Config.NewKeyValue("@NgayCT1", d1);
            Config.NewKeyValue("@NgayCT2", d2);
            Config.NewKeyValue("@NgayCT", d2);
        }

        void riKyKeToan_EditValueChanged(object sender, EventArgs e)
        {
            object o = (sender as ComboBoxEdit).EditValue;
            if (o != null && o.ToString() == _oldPeriod)
                return;
            _oldPeriod = o.ToString();
            SavePeriodConfig(o);
            RefreshDataForPeriod();
        }

        private void ViewUpdateHistory()
        {
            if (this.ActiveMdiChild == null)
                return;
            CDTForm frm = (this.ActiveMdiChild as CDTForm);
            string msg = "Cần chọn số liệu trước để xem nhật ký cập nhật số liệu đó!";
            msg = _vi ? msg : UIDictionary.Translate(msg);
            if (frm == null)
            {
                XtraMessageBox.Show(msg);
                return;
            }
            string pkValue = frm.GetPkValue();
            string sysTableID = frm.GetTableID();
            if (pkValue == "" || sysTableID == "")
            {
                XtraMessageBox.Show(msg);
                return;
            }
            DataReport _dataRpt = new DataReport("85", true);
            Config.NewKeyValue("PkValue", pkValue);
            Config.NewKeyValue("sysTableID", sysTableID);
            ReportPreview rp = new ReportPreview(_dataRpt);
            rp.WindowState = FormWindowState.Maximized;
            rp.ShowDialog();
        }

        private void SystemMenuClick(object sender, ItemClickEventArgs e)
        {
            switch (e.Item.Name)
            {
                case "iNewDb":
                    FrmNewDb frmNewDb = new FrmNewDb();
                    frmNewDb.ShowDialog();
                    break;
                case "iRestart":
                    Config.NewKeyValue("NoBackup", 1);
                    Application.Restart();
                    break;
                case "iExit":
                    this.Close();
                    break;
                case "iCheckData":
                    CheckData frmCheckData = new CheckData();
                    frmCheckData.ShowDialog();
                    break;
                case "iViewHistory":
                    ViewUpdateHistory();
                    break;
                case "iChangePassword":
                    ChangePassword frmChangePwd = new ChangePassword();
                    frmChangePwd.ShowDialog();
                    break;
                case "iRegister":
                    GetLicense rf = new GetLicense();
                    if (rf.ShowDialog() == DialogResult.OK)
                    {
                        Config.NewKeyValue("NoBackup", 1);
                        Application.Restart();
                    }
                    break;
                case "iAbout":
                    About frmAbout = new About();
                    frmAbout.ShowDialog();
                    break;
                case "iRemote":
                    if (File.Exists("TeamViewer.exe"))
                        System.Diagnostics.Process.Start("TeamViewer.exe");
                    else
                        XtraMessageBox.Show("Không tìm thấy phần mềm kết nối từ xa!");
                    break;
                case "iHelp":
                    if (File.Exists("[HTA]Huong_dan_thuc_hanh.doc"))
                        Process.Start("[HTA]Huong_dan_thuc_hanh.doc");
                    break;
                case "iTLKT":
                    dpHelp.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                    dpRefDoc.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                    dpReminder.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                    break;
                case "iTransferData":
                    FrmDataTransfer frmtf = new FrmDataTransfer();
                    frmtf.ShowDialog();
                    break;
                case "iBackup":
                    FrmBackUpRestore frmBackup = new FrmBackUpRestore(true);
                    frmBackup.ShowDialog();
                    break;
                case "iRestore":
                    FrmBackUpRestore frmRestore = new FrmBackUpRestore(false);
                    frmRestore.ShowDialog();
                    break;
                case "iUserConfig":
                    UserConfig frmUC = new UserConfig();
                    frmUC.ShowDialog();
                    UpdateStartConfig();
                    break;
                case "iExportData":
                    FrmDataExport frmDe = new FrmDataExport();
                    frmDe.ShowDialog();
                    break;
                case "iCollectData":
                    FrmDataCollection frmDc = new FrmDataCollection();
                    frmDc.ShowDialog();
                    break;
                case "iSyncData":
                    FrmDataSync frmDs = new FrmDataSync();
                    frmDs.ShowDialog();
                    break;
                case "barMdi":
                    //he thong xu ly
                    break;
                case "bbiDashboard":
                    foreach (Form frm in this.MdiChildren)
                        if (frm.GetType() == typeof(FrmDashboard) && frm.Text.Contains("Đồ thị phân tích"))
                        {
                            frm.Activate();
                            return;
                        }
                    //if (Config.GetValue("KyKeToan") == null)
                    //{
                        //string msg = "Chưa có số liệu để phân tích!";
                        //msg = _vi ? msg : UIDictionary.Translate(msg);
                        //XtraMessageBox.Show(msg);
                        //return;
                    //}
                    FrmDashboard frmDb = new FrmDashboard(_sysMenu, false);
                    frmDb.MdiParent = this;
                    frmDb.Show();
                    break;
                case "bbiReminder":
                    if (dpReminder.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible)
                        return;
                    foreach (Form frm in this.MdiChildren)
                        if (frm.GetType() == typeof(FrmDashboard) && (frm.Text.Contains("Hệ thống nhắc nhở") || frm.Text.Contains("Notification")))
                        {
                            frm.Activate();
                            return;
                        }
                    //if (Config.GetValue("Package").ToString().Contains("HTA") && Config.GetValue("KyKeToan") == null)
                    //{
                    //    string msg = "Chưa có số liệu!";
                    //    msg = _vi ? msg : UIDictionary.Translate(msg);
                    //    XtraMessageBox.Show(msg);
                    //    return;
                    //}
                    FrmDashboard frmRm = new FrmDashboard(_sysMenu, true);
                    if (frmRm.SoBaoCao > 3)
                    {
                        frmRm.MdiParent = this;
                        frmRm.Show();
                    }
                    else
                    {
                        dpReminder.Tag = frmRm;
                        dpReminder.Controls[0].Controls.RemoveByKey("layoutControl1");
                        foreach (Control c in frmRm.Controls)
                            dpReminder.Controls.Add(c);
                        dpReminder.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                        dpHelp.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                        dpRefDoc.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                    }
                    break;
            }
        }

        void barManagerMain_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.Item.GetType() != typeof(BarSubItem))
            {
                DataRow dr = e.Item.Tag as DataRow;
                if (dr == null)
                    SystemMenuClick(sender, e);
                else
                {
                    FrmVisualUI currentVs;
                    if (this.MdiChildren.Length > 0 && this.MdiChildren[0].GetType() == typeof(FrmVisualUI))
                    {
                        currentVs = (this.MdiChildren[0] as FrmVisualUI);
                    }
                    else
                        currentVs = new FrmVisualUI(_sysMenu, pm);
                    if (currentVs.Cmd == null)
                        currentVs.Cmd = _cmd;
                    Rectangle rct = e.Item.Links[0].ScreenBounds;
                    Point p = rct.Location;
                    p.X += rct.Width / 2;
                    p.Y += rct.Height / 2;
                    currentVs.DrCurrent = dr;
                    currentVs.ButtonAction(p, true, null);
                }
            }
        }

        private void InitializeMenu()
        {
            barId1 = barManagerMain.GetNewItemId();
            barId2 = barId1 + 100;
            dtMenu = _sysMenu.GetMenu();
            if (dtMenu == null)
                return;
            DataTable dtMenuForSharing = _sysMenu.GetMenuForSharing();

            if (!Boolean.Parse(Config.GetValue("Admin").ToString()))
                dtMenuForSharing.DefaultView.RowFilter = "Executable = 1";
            foreach (DataRowView dr in dtMenuForSharing.DefaultView)
            {
                string sysMenuParent = dr["sysMenuParent"].ToString();
                if (sysMenuParent == string.Empty)  //menu cha
                {
                    string menuName = _vi ? dr["MenuName"].ToString() : dr["MenuName2"].ToString();
                    BarSubItem bsi = new BarSubItem(barManagerMain, menuName);
                    BarManagerCategory bmc = barManagerMain.Categories.Add(menuName);
                    bsi.Id = barId2 + Int32.Parse(dr["sysMenuID"].ToString()) + 100;
                    barMainMenu.LinksPersistInfo.Add(new LinkPersistInfo(bsi));
                    LoopMenu(dtMenuForSharing, dr.Row, bsi, bmc);
                }

            }

            AddMenu();
            AddYearForAccounting();
            AddModule();

            barManagerMain.Items.Add(barMdi);
            barMainMenu.LinksPersistInfo.Add(new LinkPersistInfo(barMdi));
            barManagerMain.Items.Add(barSubItemHelp);
            barMainMenu.LinksPersistInfo.Add(new LinkPersistInfo(barSubItemHelp));
            iRegister.Enabled = (!Config.GetValue("DbName").ToString().Contains("DEMO") && Config.GetValue("Licensed").ToString() == "0");
            if (Config.GetValue("Package").ToString() != "HTA")
            {
                iHelp.Visibility = BarItemVisibility.Never;
                iHelpOnline.Visibility = BarItemVisibility.Never;
                iTLKT.Visibility = BarItemVisibility.Never;
            }
            AddStartConfig();
            barManagerMain.ItemClick += new ItemClickEventHandler(barManagerMain_ItemClick);
        }

        private void AddMenu()
        {
            DataTable dtModule = _sysMenu.GetModule();
            if (!Boolean.Parse(Config.GetValue("Admin").ToString()))
                dtModule.DefaultView.RowFilter = "Executable = 1";

            BarSubItem bsiDM = new BarSubItem(barManagerMain, _vi ? "Danh mục" : "Lists of data");
            bsiDM.Id = barId1++;
            BarSubItem bsiNV = new BarSubItem(barManagerMain, _vi ? "Nghiệp vụ" : "Business Process");
            bsiNV.Id = barId1++;
            BarSubItem bsiBC = new BarSubItem(barManagerMain, _vi ? "Báo cáo" : "Reports");
            bsiBC.Id = barId1++;

            foreach (DataRowView dr in dtModule.DefaultView)
            {
                string s = _vi ? dr["MenuName"].ToString() : dr["MenuName2"].ToString();
                BarManagerCategory bmc = barManagerMain.Categories.Add(s);
                BarSubItem bsiDMsub = new BarSubItem(barManagerMain, s);
                bsiDMsub.Name = "DM" + dr["sysMenuID"].ToString();
                bsiDMsub.Id = barId1++;
                BarSubItem bsiNVsub = new BarSubItem(barManagerMain, s);
                bsiNVsub.Name = "NV" + dr["sysMenuID"].ToString();
                bsiNVsub.Id = barId1++;
                BarSubItem bsiBCsub = new BarSubItem(barManagerMain, s);
                bsiBCsub.Name = "BC" + dr["sysMenuID"].ToString();
                bsiBCsub.Id = barId1++;
                string m = dr["sysMenuID"].ToString();
                DataTable dtSubMenu = _sysMenu.GetMenuForModule(Int32.Parse(m), true);
                string cond = "";
                if (!Boolean.Parse(Config.GetValue("Admin").ToString()))
                    cond = "Executable = 1 and ";
                for (int i = 1; i <= 5; i++)
                {
                    DataView dv = new DataView(dtSubMenu);
                    dv.RowFilter = cond + "UIType = " + i.ToString() +
                        " and (sysTableID is not null or sysReportID is not null or MenuPluginID is not null)";
                    foreach (DataRowView drv in dv)
                    {
                        switch (i)
                        {
                            case 1:
                                s = _vi ? drv["MenuName"].ToString() : drv["MenuName2"].ToString();
                                BarLargeButtonItem biDM = new BarLargeButtonItem(barManagerMain, s);
                                biDM.Name = drv["sysMenuID"].ToString();
                                biDM.Id = barId2 + Int32.Parse(drv["sysMenuID"].ToString());
                                biDM.Tag = drv.Row;
                                biDM.Category = bmc;
                                bsiDMsub.LinksPersistInfo.Add(new LinkPersistInfo(biDM));
                                break;
                            case 2:
                            case 3:
                            case 4:
                                s = _vi ? drv["MenuName"].ToString() : drv["MenuName2"].ToString();
                                BarLargeButtonItem biNV = new BarLargeButtonItem(barManagerMain, s);
                                biNV.Name = drv["sysMenuID"].ToString();
                                biNV.Id = barId2 + Int32.Parse(drv["sysMenuID"].ToString());
                                biNV.Tag = drv.Row;
                                biNV.Category = bmc;
                                bsiNVsub.LinksPersistInfo.Add(new LinkPersistInfo(biNV));
                                break;
                            case 5:
                                s = _vi ? drv["MenuName"].ToString() : drv["MenuName2"].ToString();
                                BarLargeButtonItem biBC = new BarLargeButtonItem(barManagerMain, s);
                                biBC.Name = drv["sysMenuID"].ToString();
                                biBC.Id = barId2 + Int32.Parse(drv["sysMenuID"].ToString());
                                biBC.Tag = drv.Row;
                                biBC.Category = bmc;
                                bsiBCsub.LinksPersistInfo.Add(new LinkPersistInfo(biBC));
                                break;
                        }
                    }
                }
                if (bsiDMsub.LinksPersistInfo.Count > 0)
                    bsiDM.LinksPersistInfo.Add(new LinkPersistInfo(bsiDMsub));
                if (bsiNVsub.LinksPersistInfo.Count > 0)
                    bsiNV.LinksPersistInfo.Add(new LinkPersistInfo(bsiNVsub));
                if (bsiBCsub.LinksPersistInfo.Count > 0)
                    bsiBC.LinksPersistInfo.Add(new LinkPersistInfo(bsiBCsub));
            }
            if (bsiDM.LinksPersistInfo.Count > 0)
                barMainMenu.LinksPersistInfo.Add(new LinkPersistInfo(bsiDM));
            if (bsiNV.LinksPersistInfo.Count > 0)
                barMainMenu.LinksPersistInfo.Add(new LinkPersistInfo(bsiNV));
            if (bsiBC.LinksPersistInfo.Count > 0)
                barMainMenu.LinksPersistInfo.Add(new LinkPersistInfo(bsiBC));
        }

        //private void AddToolbar()
        //{
            //iHelp.BorderStyle = BarItemBorderStyle.Single;
            //iHelp.LargeImageIndex = 1;
            //tbSystem.LinksPersistInfo.Add(new LinkPersistInfo(iHelp));
            //iHelpOnline.BorderStyle = BarItemBorderStyle.Single;
            //iHelpOnline.LargeImageIndex = 2;
            //tbSystem.LinksPersistInfo.Add(new LinkPersistInfo(iHelpOnline));
            //iRemote.BorderStyle = BarItemBorderStyle.Single;
            //iRemote.LargeImageIndex = 3;
            //tbSystem.LinksPersistInfo.Add(new LinkPersistInfo(iRemote));
            //iTLKT.BorderStyle = BarItemBorderStyle.Single;
            //iTLKT.LargeImageIndex = 4;
            //tbSystem.LinksPersistInfo.Add(new LinkPersistInfo(iTLKT));
            //iAbout.BorderStyle = BarItemBorderStyle.Single;
            //iAbout.LargeImageIndex = 5;
            //tbSystem.LinksPersistInfo.Add(new LinkPersistInfo(iAbout));
            //bbiDashboard.BorderStyle = BarItemBorderStyle.Single;
            //bbiDashboard.LargeImageIndex = 6;
            //tbSystem.LinksPersistInfo.Add(new LinkPersistInfo(bbiDashboard));
        //}

        private void AddStartConfig()
        {
            string code = Config.GetValue("SiteCode").ToString().ToUpper();
            if (code == "QLCV")
            {
                beiStatus.Alignment = BarItemLinkAlignment.Right;
                beiStatus.Edit = riStatus;
                riStatus.Leave += new EventHandler(riStatus_Leave);
                LinkPersistInfo lpiStatus = new LinkPersistInfo(BarLinkUserDefines.PaintStyle | BarLinkUserDefines.Width, beiStatus);
                barMainMenu.LinksPersistInfo.Add(lpiStatus);
                beiStatus.EditValue = SysUser.GetStatus();
            }

            if (code == "QLCV" || code == "HTM")
            {
                LinkPersistInfo lpiFloat = new LinkPersistInfo(BarLinkUserDefines.PaintStyle | BarLinkUserDefines.Width, bciFloat);
                barMainMenu.LinksPersistInfo.Add(lpiFloat);
            }
            beiDb.Alignment = BarItemLinkAlignment.Right;
            beiDb.Edit = riLueDb;
            LinkPersistInfo lpiDb = new LinkPersistInfo(BarLinkUserDefines.PaintStyle | BarLinkUserDefines.Width, beiDb);
            lpiDb.UserWidth = 80;
            lpiDb.UserPaintStyle = BarItemPaintStyle.Caption;
            barMainMenu.LinksPersistInfo.Add(lpiDb);

            beiKyKeToan.Alignment = BarItemLinkAlignment.Right;
            beiKyKeToan.Edit = riKyKeToan;
            LinkPersistInfo lpiKyKeToan = new LinkPersistInfo(BarLinkUserDefines.PaintStyle | BarLinkUserDefines.Width, beiKyKeToan);
            lpiKyKeToan.UserWidth = 40;
            lpiKyKeToan.UserPaintStyle = BarItemPaintStyle.Caption;
            barMainMenu.LinksPersistInfo.Add(lpiKyKeToan);

            beiNamLV.Alignment = BarItemLinkAlignment.Right;
            beiNamLV.Edit = riNamLV;
            LinkPersistInfo lpiNamLV = new LinkPersistInfo(BarLinkUserDefines.PaintStyle | BarLinkUserDefines.Width, beiNamLV);
            lpiNamLV.UserWidth = 60;
            lpiNamLV.UserPaintStyle = BarItemPaintStyle.Caption;
            barMainMenu.LinksPersistInfo.Add(lpiNamLV);
            
            SysConfig sc = new SysConfig();
            sc.GetStartConfig();
            for (int i = 0; i < sc.DsStartConfig.Tables[0].Rows.Count; i++)
            {
                DataRow dr = sc.DsStartConfig.Tables[0].Rows[i];
                string caption = _vi ? dr["DienGiai"].ToString() : dr["DienGiai2"].ToString();
                BarButtonItem bbi = new BarButtonItem(barManagerMain, caption + ": " + dr["_Value"].ToString());
                bbi.Id = barId1++;
                bbi.Name = "iUserConfig";
                bbi.Tag = i;
                bbi.Alignment = BarItemLinkAlignment.Right;
                barMainMenu.LinksPersistInfo.Add(new LinkPersistInfo(bbi));
            }
        }

        void riStatus_Leave(object sender, EventArgs e)
        {
            string s = (sender as TextEdit).Text;
            SysUser.ChangeStatus(s);
        }

        private void UpdateStartConfig()
        {
            SysConfig sc = new SysConfig();
            sc.GetStartConfig();
            DataTable dt = sc.DsStartConfig.Tables[0];
            foreach (LinkPersistInfo lpi in barMainMenu.LinksPersistInfo)
            {
                if (lpi.Item.Name == "iUserConfig")
                {
                    lpi.Item.Caption = dt.Rows[Int32.Parse(lpi.Item.Tag.ToString())]["DienGiai"].ToString() + ": " + dt.Rows[Int32.Parse(lpi.Item.Tag.ToString())]["_Value"].ToString();
                }
            }
            AddPeriodForAccounting();
        }

        private Image GetImage(byte[] b)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(b);
            if (ms == null)
                return null;
            Image im = Image.FromStream(ms);
            return (im);
        }

        private void CreateReportMenu(PopupMenu nbcSub, DataTable dtMenu, DataRow drModule)
        {
            string module = drModule["sysMenuID"].ToString();
            DataView dv = new DataView(dtMenu);
            dv.RowFilter = "UIType = 5 and sysMenuParent = " + module;  //Bao cao
            if (dv.Count == 0)
                return;
            dv.RowFilter = "UIType = 5 and (sysReportID is not null or sysTableID is not null or MenuPluginID is not null) and sysMenuParent = " + module;
            for (int i = 0; i < dv.Count; i++)
            {
                string exe = Boolean.Parse(Config.GetValue("Admin").ToString()) ? "" : dv[i]["Executable"].ToString();
                if (exe != "" && !Boolean.Parse(exe))
                    continue;
                BarButtonItem nbi = new BarButtonItem(barManagerMain, _vi ? dv[i]["MenuName"].ToString() : dv[i]["MenuName2"].ToString());
                nbi.ImageIndex = 2;
                nbi.Tag = dv[i].Row;
                if (i == 0)
                    nbcSub.AddItem(nbi).BeginGroup = true;
                else
                    nbcSub.AddItem(nbi);
            }
            dv.RowFilter = "UIType = 5 and sysReportID is null and sysTableID is null and MenuPluginID is null and sysMenuParent = " + module;
            if (dv.Count > 0)       //Bao cao da phan nhom
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    string s = _vi ? dv[i]["MenuName"].ToString() : dv[i]["MenuName2"].ToString();
                    BarSubItem nbg = new BarSubItem(barManagerMain, s);
                    if (i == 0)
                        nbcSub.AddItem(nbg).BeginGroup = true;
                    else
                        nbcSub.AddItem(nbg);
                    DataView dvNew = new DataView(dtMenu);
                    dvNew.RowFilter = "UIType = 5 and (sysReportID is not null or sysTableID is not null or MenuPluginID is not null) and sysMenuParent = " + dv[i]["sysMenuID"].ToString();
                    for (int j = 0; j < dvNew.Count; j++)
                    {
                        string exe = Boolean.Parse(Config.GetValue("Admin").ToString()) ? "" : dvNew[j]["Executable"].ToString();
                        if (exe != "" && !Boolean.Parse(exe))
                            continue;
                        BarButtonItem nbi = new BarButtonItem(barManagerMain, _vi ? dvNew[j].Row["MenuName"].ToString() : dvNew[j].Row["MenuName2"].ToString());
                        nbi.ImageIndex = 2;
                        nbi.Tag = dvNew[j].Row;
                        nbg.AddItem(nbi);
                    }
                    if (nbg.ItemLinks.Count == 0)
                        nbg.Visibility = BarItemVisibility.Never;
                }
            }
        }

        private PopupMenu GetModuleMenu(DataRow drModule)
        {
            int sysMenuParent = Convert.ToInt32(drModule["sysMenuID"]);
            PopupMenu pm = new PopupMenu(barManagerMain);
            pm.Ribbon = rcModule;
            DataTable dtModule = _sysMenu.GetMenuForModule(sysMenuParent, true);
            DataView dv = new DataView(dtModule);
            dv.RowFilter = "UIType <> 0 and UIType <> 6 and UIType <> 7";
            if (!Boolean.Parse(Config.GetValue("Admin").ToString()))
                dv.RowFilter += " and Executable = 1";
            if (dv.Count == 0)
                return pm;
            dv.RowFilter += " and UIType <> 5";
            if (dv.Count > 0)
            {
                dv.Sort = "UIType";
                int oldt = Int32.Parse(dv[0]["UIType"].ToString());
                for (int i = 0; i < dv.Count; i++)
                {
                    string s = _vi ? dv[i]["MenuName"].ToString() : dv[i]["MenuName2"].ToString();
                    BarButtonItem bbi = new BarButtonItem(barManagerMain, s);
                    int im = 1;
                    int type = Int32.Parse(dv[i]["UIType"].ToString());
                    if (type == 1)
                        im = 0;
                    if (type == 5)
                        im = 2;
                    bbi.ImageIndex = im;
                    bbi.Tag = dv[i].Row;
                    if (oldt == type)
                        pm.AddItem(bbi);
                    else
                        pm.AddItem(bbi).BeginGroup = true;
                    oldt = type;
                }
            }
            CreateReportMenu(pm, dtMenu, drModule);
            return pm;
        }

        //private PopupMenu GetRecentMenu(int sysMenuParent)
        //{
        //    PopupMenu pm = new PopupMenu(barManagerMain);
        //    pm.ShowCaption = true;
        //    pm.MenuCaption = _vi ? "Chức năng thường dùng" : "Most used fuctions";
        //    pm.Ribbon = rcModule;
        //    DataTable dt = _sysMenu.GetMenuForModule(sysMenuParent, false);
        //    DataView dv = new DataView(dt);
        //    dv.RowFilter = "UIType <> 6 and UIType <> 7";
        //    if (!Boolean.Parse(Config.GetValue("Admin").ToString()))
        //        dv.RowFilter += " and Executable = 1";
        //    dv.Sort = "Used desc";
        //    int n = Math.Min(dv.Count, 10);
        //    DataTable dt1 = dt.Clone();
        //    for (int i = 0; i < n; i++)
        //        dt1.ImportRow(dv[i].Row);
        //    dt1.DefaultView.Sort = "UIType";
        //    int oldt = Int32.Parse(dt1.DefaultView[0]["UIType"].ToString());
        //    for (int i = 0; i < n; i++)
        //    {
        //        string s = _vi ? dt1.DefaultView[i]["MenuName"].ToString() : dt1.DefaultView[i]["MenuName2"].ToString();
        //        BarButtonItem bbi = new BarButtonItem(barManagerMain, s);
        //        int im = 1;
        //        int type = Int32.Parse(dt1.DefaultView[i]["UIType"].ToString());
        //        if (type == 1)
        //            im = 0;
        //        if (type == 5)
        //            im = 2;
        //        bbi.ImageIndex = im;
        //        bbi.Tag = dt1.DefaultView[i].Row;
        //        if (oldt == type)
        //            pm.AddItem(bbi);
        //        else
        //            pm.AddItem(bbi).BeginGroup = true;
        //        oldt = type;
        //    }

        //    return pm;
        //}

        private void AddModule()
        {
            DataTable dtModule = _sysMenu.GetModule();
            if (!Boolean.Parse(Config.GetValue("Admin").ToString()))
                dtModule.DefaultView.RowFilter = "Executable = 1";
            if (dtModule.DefaultView.Count == 0)
                return;
            rcModule.LargeImages = lstImages;
            barManagerMain.Images = lstImages2;
            int n = 3;
            int w = Screen.PrimaryScreen.WorkingArea.Width <= 1024 ? 70 : 80;
            for (int i = 0; i < dtModule.DefaultView.Count; i++)
            {
                DataRowView dr = dtModule.DefaultView[i];
                string s = _vi ? dr["MenuName"].ToString() : dr["MenuName2"].ToString();
                BarButtonItem bsi = new BarButtonItem();
                bsi.Caption = s;
                bsi.ButtonStyle = BarButtonStyle.DropDown;
                bsi.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
                bsi.LargeWidth = w;
                bsi.GroupIndex = 1;
                if (dtModule.Columns.Contains("Icon") && dr["Icon"] != DBNull.Value)
                {
                    lstImages.AddImage(GetImage(dr["Icon"] as byte[]));
                    bsi.LargeImageIndex = n;
                    n++;
                }
                bsi.Tag = dr.Row;
                rcModule.Items.Add(bsi);
                rpgModule.ItemLinks.Add(bsi);
                //bsi.DropDownControl = GetRecentMenu(Int32.Parse(dr["sysMenuID"].ToString()));
                PopupMenu pm = GetModuleMenu(dr.Row);
                if (pm.ItemLinks.Count == 0)
                    bsi.Visibility = BarItemVisibility.Never;
                else
                    bsi.DropDownControl = pm;
            }
            if (Config.GetValue("NhacNho") != null && Config.GetValue("NhacNho").ToString() != "0")
            {
                dpReminder.Text = _vi ? "Hệ thống nhắc nhở" : "Notification";
                bbiReminder.Caption = _vi ? "Hệ thống nhắc nhở" : "Notification";
                bbiReminder.LargeWidth = w;
                bbiReminder.LargeImageIndex = 1;
                rcModule.Items.Add(bbiReminder);
                rpgModule.ItemLinks.Add(bbiReminder);
                barSubItemSystem.ItemLinks.Add(bbiReminder);
            }
            if (Config.GetValue("Package").ToString() == "HTA")
            {
                //neu la PMKT ma ko co menu nao trong dashboard thi ko hien thi bbiDashboard
                DataRow[] drs = _sysMenu.DtMenu.Select("UIType = 6");
                if (drs.Length > 0)
                {
                    bbiDashboard.LargeWidth = w;
                    bbiDashboard.LargeImageIndex = 0;
                    rcModule.Items.Add(bbiDashboard);
                    rpgModule.ItemLinks.Add(bbiDashboard);
                    bbiDashboard.Caption = _vi ? "Đồ thị phân tích" : "Dashboard";
                }
                iHelp.LargeWidth = w;
                iHelp.LargeImageIndex = 2;
                iHelp.Caption = _vi ? "Hướng dẫn thực hành" : "Case study";
                rcModule.Items.Add(iHelp);
                rpgModule.ItemLinks.Add(iHelp);
            }
            //mac dinh la chay phan he dau tien khi moi mo phan mem len
            rcModule_ItemClick(rcModule, new ItemClickEventArgs(rcModule.Items[0], rpgModule.ItemLinks[0]));
            //(rcModule.Items[0] as BarButtonItem).Down = true;
            if (dtModule.DefaultView.Count == 1)
            {
                rcModule.Visible = false;
                pictureEdit1.Visible = false;
            }
            dtModule.DefaultView.RowFilter = "";
            if (Config.GetValue("DbName").ToString().Contains("DEMO") && Config.GetValue("KyKeToan") != null)  //mac dinh hien thi dashboard tren ban demo
            {
                FrmDashboard frmDb = new FrmDashboard(_sysMenu, false);
                if (frmDb.IsEmpty)
                    frmDb.Dispose();
                else
                {
                    frmDb.MdiParent = this;
                    frmDb.Show();
                }
            }
        }

        private Shortcut GetShortcut(string strShortcut)
        {
            Array arrShortcut = Enum.GetValues(typeof(Shortcut));
            foreach (Shortcut sctmp in arrShortcut)
                if (sctmp.ToString() == strShortcut)
                    return sctmp;
            return Shortcut.None;
        }

        private void LoopMenu(DataTable dtMenu, DataRow dr, BarSubItem bsi, BarManagerCategory bmc)
        {
            foreach (DataRowView drChild in dtMenu.DefaultView)
            {
                if (drChild["sysMenuParent"].ToString() == dr["sysMenuID"].ToString())
                {
                    string menuName = _vi ? drChild["MenuName"].ToString() : drChild["MenuName2"].ToString();
                    if (_sysMenu.HasChild(drChild["sysMenuID"].ToString()))  //vua cha vua con
                    {
                        BarSubItem bsiChild = new BarSubItem(barManagerMain, menuName);
                        bsiChild.Id = barId2 + Int32.Parse(drChild["sysMenuID"].ToString());
                        bsi.LinksPersistInfo.Add(new LinkPersistInfo(bsiChild));
                        LoopMenu(dtMenu, drChild.Row, bsiChild, bmc);
                    }
                    else
                    {   //menu con
                        BarLargeButtonItem bbi = new BarLargeButtonItem(barManagerMain, menuName);
                        bbi.Name = drChild["sysMenuID"].ToString();
                        bbi.Id = barId2 + Int32.Parse(drChild["sysMenuID"].ToString());
                        bbi.Hint = menuName;
                        bbi.Tag = drChild.Row;
                        bbi.Category = bmc;
                        bbi.CaptionAlignment = BarItemCaptionAlignment.Bottom;
                        bsi.LinksPersistInfo.Add(new LinkPersistInfo(bbi));
                    }
                }
            }
        }

        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin))
                if (this.MdiChildren.Length > 0)
                    this.MdiChildren[0].Activate();
            if (e.Modifiers == Keys.Control)
                switch (e.KeyCode)
                {
                    case Keys.D1:
                        if (this.MdiChildren.Length > 1)
                            this.MdiChildren[1].Activate();
                        break;
                    case Keys.D2:
                        if (this.MdiChildren.Length > 2)
                            this.MdiChildren[2].Activate();
                        break;
                    case Keys.D3:
                        if (this.MdiChildren.Length > 3)
                            this.MdiChildren[3].Activate();
                        break;
                    case Keys.D4:
                        if (this.MdiChildren.Length > 4)
                            this.MdiChildren[4].Activate();
                        break;
                    case Keys.D5:
                        if (this.MdiChildren.Length > 5)
                            this.MdiChildren[5].Activate();
                        break;
                    case Keys.D6:
                        if (this.MdiChildren.Length > 6)
                            this.MdiChildren[6].Activate();
                        break;
                    case Keys.D7:
                        if (this.MdiChildren.Length > 7)
                            this.MdiChildren[7].Activate();
                        break;
                    case Keys.D8:
                        if (this.MdiChildren.Length > 8)
                            this.MdiChildren[8].Activate();
                        break;
                    case Keys.D9:
                        if (this.MdiChildren.Length > 9)
                            this.MdiChildren[9].Activate();
                        break;
                    case Keys.R:
                        Config.NewKeyValue("NoBackup", 1);
                        Application.Restart();
                        break;
                }
        }

        private void mdiTabMain_SelectedPageChanged(object sender, EventArgs e)
        {
            if (mdiTabMain.SelectedPage != null && mdiTabMain.SelectedPage.MdiChild.GetType() == typeof(FrmVisualUI))
                mdiTabMain.HeaderButtonsShowMode = DevExpress.XtraTab.TabButtonShowMode.Never;
            else
            {
                mdiTabMain.HeaderButtonsShowMode = DevExpress.XtraTab.TabButtonShowMode.WhenNeeded;
                if ((mdiTabMain.SelectedPage.MdiChild as CDTForm) != null)
                {
                    CDTData data = (mdiTabMain.SelectedPage.MdiChild as CDTForm).Data;
                    if (data != null)
                        data.RefreshData(null);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _delayPopup += 1;
            if (_delayPopup == 5)
            {
                if ((timer1.Tag as FrmVisualUI) != null)
                {
                    (timer1.Tag as FrmVisualUI).ButtonAction(Control.MousePosition, false, null);
                }
                _delayPopup = 0;
                timer1.Enabled = false;
            }
        }

        private void RefreshDataForPeriod()
        {
            //kiem tra man hinh dashboard dang mo truoc
            if (this.ActiveMdiChild != null && this.ActiveMdiChild.GetType() == typeof(FrmDashboard))
            {
                bool isReminder = (this.ActiveMdiChild as FrmDashboard).IsReminder;
                this.ActiveMdiChild.Close();
                FrmDashboard frmDb = new FrmDashboard(_sysMenu, isReminder);
                frmDb.MdiParent = this;
                frmDb.Show();
            }
            //lay man hinh dang su dung
            DataRow drCur = null;
            if (this.ActiveMdiChild != null && (this.ActiveMdiChild as CDTForm) != null)// &&
                //((this.ActiveMdiChild as CDTForm).FrmType == FormType.MasterDetail
                //|| (this.ActiveMdiChild as CDTForm).FrmType == FormType.MultiDetail
                //|| (this.ActiveMdiChild as CDTForm).FrmType == FormType.Report))
                drCur = (this.ActiveMdiChild as CDTForm).Data.DrTable;
            //kiem tra co mo man hinh chung tu nao khong
            bool flag = false;
            foreach (Form frm in this.MdiChildren)
                if ((frm as CDTForm) != null)// &&
                    //((frm as CDTForm).FrmType == FormType.MasterDetail ||
                    //(frm as CDTForm).FrmType == FormType.MultiDetail ||
                    //(frm as CDTForm).FrmType == FormType.Report))
                {
                    flag = true;
                    break;
                }
            string msg = "Bạn muốn chương trình tự động lấy tất cả số liệu đang mở theo kỳ này không?";
            msg = _vi ? msg : UIDictionary.Translate(msg);
            if (!flag || XtraMessageBox.Show(msg, Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo)
                == DialogResult.No)
                return;
            _isRefresh = true;
            //bat dau nap lai so lieu cac man hinh
            Cursor.Current = Cursors.WaitCursor;
            List<DataRow> lstData = new List<DataRow>();
            for (int i = this.MdiChildren.Length - 1; i >= 0; i--)
            {
                Form frm = this.MdiChildren[i];
                if ((frm as CDTForm) != null)// &&
                    //((frm as CDTForm).FrmType == FormType.MasterDetail
                    //|| (frm as CDTForm).FrmType == FormType.MultiDetail
                    //|| (frm as CDTForm).FrmType == FormType.Report))
                {
                    lstData.Add((frm as CDTForm).Data.DrTable);
                    frm.Close();
                }
            }
            Form activeForm = null;
            for (int i = lstData.Count - 1; i >= 0; i--)
            {
                if (lstData[i]["sysReportID"].ToString() == "")
                    _cmd.ShowTable(lstData[i], FormAction.All);
                else
                    _cmd.ShowReport(lstData[i]);
                if (drCur == lstData[i])
                    activeForm = this.ActiveMdiChild;
            }
            if (activeForm != null)
                this.ActivateMdiChild(activeForm);
            Cursor.Current = Cursors.Default;
            _isRefresh = false;
        }

        private void mdiTabMain_PageRemoved(object sender, DevExpress.XtraTabbedMdi.MdiTabPageEventArgs e)
        {
            object t = Config.GetValue("ThangLV");  //lay theo 12 thang, khong lay tu BLTK
            if (t != null && t.ToString() == "1")
                return;
            if (_isRefresh)
                return;
            Form frm = e.Page.MdiChild;
            if ((frm as CDTForm) != null &&
                ((frm as CDTForm).FrmType == FormType.MasterDetail
                || (frm as CDTForm).FrmType == FormType.MultiDetail))
            {
                SysPackage sp = new SysPackage();
                DataTable dtData = sp.GetPeriodForAccounting();
                if (dtData == null) //khong phai la ban phan mem ke toan
                    return;
                dtData.PrimaryKey = new DataColumn[] { dtData.Columns[0] };
                bool hasChanged = false;
                foreach (DataRow dr in dtData.Rows)
                {
                    if (!riKyKeToan.Items.Contains(dr[0]))
                    {
                        riKyKeToan.Items.Add(dr[0]);
                        hasChanged = true;
                    }
                }
                for (int i = 0; i < riKyKeToan.Items.Count; i++)
                {
                    object o = riKyKeToan.Items[i];
                    if (!dtData.Rows.Contains(o))
                    {
                        riKyKeToan.Items.Remove(o);
                        hasChanged = true;
                    }
                }

                if (hasChanged && riKyKeToan.Items.Count > 0)
                {
                    beiKyKeToan.EditValue = riKyKeToan.Items[riKyKeToan.Items.Count - 1];
                    Config.NewKeyValue("KyKeToan", beiKyKeToan.EditValue);
                }
                if (riKyKeToan.Items.Count == 0)
                {
                    beiKyKeToan.EditValue = null;
                    Config.NewKeyValue("KyKeToan", null);
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            DataRow drUser = e.Argument as DataRow;
            string dbName = drUser["DbName"].ToString();
            //kiem tra license
            if (Config.GetValue("Licensed").ToString() == "1" || dbName.Contains("DEMO"))
            {
                double d = Security.GetExpired();
                if (d <= 3)
                {
                    string msg;
                    if (d > 0)
                        msg = dbName.Contains("DEMO") ? "Phần mềm sắp hết hạn dùng thử!\r\nVui lòng liên hệ Phòng Kinh doanh công ty Hoa Tiêu!" :
                        "Phần mềm sắp tạm ngưng sử dụng!\r\nVui lòng liên hệ Phòng Tư vấn Triển khai công ty Hoa Tiêu!";
                    else
                        msg = dbName.Contains("DEMO") ? "Phần mềm đã hết hạn dùng thử!\r\nVui lòng liên hệ Phòng Kinh doanh công ty Hoa Tiêu!" :
                        "Phần mềm đang tạm ngưng sử dụng!\r\nVui lòng liên hệ Phòng Tư vấn Triển khai công ty Hoa Tiêu!";
                    msg = _vi ? msg : UIDictionary.Translate(msg);
                    ExpireMsg frm = new ExpireMsg(msg, d);
                    frm.ShowDialog();
                    if (d <= 0)
                    {
                        AppCon ac = new AppCon();
                        ac.SetValue("Expire", Security.EnCode(DateTime.MinValue.ToString("MM/dd/yyyy")));
                        Environment.Exit(0);
                    }
                }
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((Config.GetValue("NoBackup") == null || Config.GetValue("NoBackup").ToString() == "0") &&
                !Config.GetValue("DbName").ToString().Contains("DEMO") && !Config.GetValue("Package").ToString().Contains("CDT"))
            {
                string msg = "Có sao lưu số liệu trước khi thoát khỏi phần mềm không?";
                msg = _vi ? msg : UIDictionary.Translate(msg);
                DialogResult dr = XtraMessageBox.Show(msg, Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    AutoBackup ab = new AutoBackup();
                    ab.Backup();
                }
                else
                    if (dr == DialogResult.Cancel)
                        e.Cancel = true;
            }
            if (!e.Cancel)
            {
                SysHistory sh = new SysHistory();
                sh.InsertHistory("Logout");
                //AppCon ac = new AppCon();
                //bool visible = dpMenu.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible;
                //ac.SetValue("PanelVisible", visible.ToString());
            }
        }
        #region NavbarControl
        //private void CreateReportMenu(NavBarControl nbcSub, DataTable dtMenu, DataRow drModule)
        //{
        //    string module = drModule["sysMenuID"].ToString();
        //    DataView dv = new DataView(dtMenu);
        //    dv.RowFilter = "UIType = 5 and sysMenuParent = " + module;  //Bao cao
        //    if (dv.Count == 0)
        //        return;
        //    dv.RowFilter = "UIType = 5 and (sysReportID is not null or sysTableID is not null or MenuPluginID is not null) and sysMenuParent = " + module;
        //    if (dv.Count > 0)       //Bao cao chua phan nhom
        //    {
        //        string s = _vi ? "Báo cáo chung" : "Common reports";
        //        NavBarGroup nbg = new NavBarGroup(s);
        //        nbg.GroupStyle = NavBarGroupStyle.SmallIconsList;
        //        nbg.SmallImage = lstImages2.Images[2];
        //        nbcSub.Groups.Add(nbg);
        //        for (int i = 0; i < dv.Count; i++)
        //            AddNavBarItem(dv[i].Row, nbcSub, nbg);
        //    }
        //    dv.RowFilter = "UIType = 5 and sysReportID is null and sysTableID is null and MenuPluginID is null and sysMenuParent = " + module;
        //    if (dv.Count > 0)       //Bao cao da phan nhom
        //    {
        //        for (int i = 0; i < dv.Count; i++)
        //        {
        //            string s = _vi ? dv[i]["MenuName"].ToString() : dv[i]["MenuName2"].ToString();
        //            NavBarGroup nbg = new NavBarGroup(s);
        //            nbg.GroupStyle = NavBarGroupStyle.SmallIconsList;
        //            nbg.SmallImage = lstImages2.Images[2];
        //            nbcSub.Groups.Add(nbg);
        //            DataView dvNew = new DataView(dtMenu);
        //            dvNew.RowFilter = "UIType = 5 and (sysReportID is not null or sysTableID is not null or MenuPluginID is not null) and sysMenuParent = " + dv[i]["sysMenuID"].ToString();
        //            for (int j = 0; j < dvNew.Count; j++)
        //                AddNavBarItem(dvNew[j].Row, nbcSub, nbg);
        //        }
        //    }
        //}

        //private void CreateGroupContent(DataTable dtMenu, DataRow drModule)
        //{
        //    nbcSub.Groups.Clear();
        //    nbcSub.Items.Clear();
        //    DataView dv = new DataView(dtMenu);
        //    for (int t = 0; t <= 4; t++)
        //    {
        //        dv.RowFilter = "UIType = " + t.ToString();
        //        dv.Sort = "MenuOrder";
        //        if (dv.Count > 0)
        //        {
        //            string s = "";
        //            int im = 1;
        //            switch (t)
        //            {
        //                case 1:
        //                    im = 0;
        //                    s = _vi ? "Danh mục" : "List of data";
        //                    break;
        //                case 2:
        //                case 4:
        //                    im = 1;
        //                    s = _vi ? "Tiện ích Quản trị" : "Utilities";
        //                    break;
        //                case 3:
        //                    im = 1;
        //                    s = _vi ? "Nghiệp vụ" : "Business process";
        //                    break;
        //            }
        //            NavBarGroup nbg = new NavBarGroup(s);
        //            nbg.GroupStyle = NavBarGroupStyle.SmallIconsText;
        //            nbg.SmallImage = lstImages2.Images[im];
        //            nbcSub.Groups.Add(nbg);
        //            for (int i = 0; i < dv.Count; i++)
        //                AddNavBarItem(dv[i].Row, nbcSub, nbg);
        //            if (t == 3)
        //                nbg.Expanded = true;
        //        }
        //    }
        //    CreateReportMenu(nbcSub, dtMenu, drModule);
        //}

        //private void AddNavBarItem(DataRow dr, NavBarControl nbcSub, NavBarGroup nbg)
        //{
        //    string exe = Boolean.Parse(Config.GetValue("Admin").ToString()) ? "" : dr["Executable"].ToString();
        //    if (exe != "" && !Boolean.Parse(exe))
        //        return;
        //    NavBarItem nbi = new NavBarItem(_vi ? dr["MenuName"].ToString() : dr["MenuName2"].ToString());
        //    nbi.Tag = dr;
        //    nbcSub.Items.Add(nbi);
        //    nbg.ItemLinks.Add(nbi);
        //    if (nbi.Caption.Length > 30)
        //        nbi.Hint = nbi.Caption;
        //}

        //void nbcSub_LinkClicked(object sender, NavBarLinkEventArgs e)
        //{
        //    DataRow dr = e.Link.Item.Tag as DataRow;
        //    if (dr == null)
        //        return;
        //    currentVs.DrCurrent = dr;
        //    currentVs.ButtonAction(Control.MousePosition, true, null);
        //}

        //void nbcSub_MouseUp(object sender, MouseEventArgs e)
        //{
        //    NavBarHitInfo hInfo = ((NavBarControl)sender).CalcHitInfo(e.Location);
        //    if (hInfo.InGroupCaption && !hInfo.InGroupButton)
        //        hInfo.Group.Expanded = !hInfo.Group.Expanded;
        //}
        #endregion
        private void rcModule_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow drModule = e.Item.Tag as DataRow;
            if (drModule != null)
                ShowModule(drModule);
            else
                SystemMenuClick(sender, new ItemClickEventArgs(e.Item, e.Link));
        }

        private void ShowModule(DataRow drModule)
        {
            DataTable dtModule = _sysMenu.GetMenuForModule(Int32.Parse(drModule["sysMenuID"].ToString()), true);
            //DataView dv = new DataView(dtModule);
            //dv.RowFilter = "UIType = 0";    //kiểm tra có phân hệ con không?
            //if (dv.Count > 0)
            //{
            //    pmSubModule.ItemLinks.Clear();
            //    for (int i = 0; i < dv.Count; i++)
            //    {
            //        string s = _vi ? dv[i]["MenuName"].ToString() : dv[i]["MenuName2"].ToString();
            //        BarButtonItem bbi = new BarButtonItem();
            //        bbi.Caption = s;
            //        bbi.Tag = dv[i].Row;
            //        pmSubModule.AddItem(bbi);
            //    }
            //    pmSubModule.ShowPopup(Control.MousePosition);
            //}
            //else
            //{
                if (this.MdiChildren.Length > 0 && this.MdiChildren[0].GetType() == typeof(FrmVisualUI))
                {
                    currentVs = (this.MdiChildren[0] as FrmVisualUI);
                }
                else
                {
                    currentVs = new FrmVisualUI(_sysMenu, pm);
                    currentVs.MdiParent = this;
                }
                _cmd.VisualUI = currentVs;
                if (currentVs.Cmd == null)
                    currentVs.Cmd = _cmd;
                currentVs.DrCurrent = drModule;
                currentVs.ButtonAction(Control.MousePosition, true, dtModule);
                //dpMenu.Text = _vi ? drModule["MenuName"].ToString() : drModule["MenuName2"].ToString();
                //tao subGroup
                //CreateGroupContent(currentVs.DtMenu, drModule);
            //}
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            if (Config.GetValue("SiteCode").ToString().ToUpper() == "QLCV")
            {
                ChangeStatus frmcs = new ChangeStatus();
                frmcs.ShowDialog();
            }
            //AppCon ac = new AppCon();
            //if (ac.GetValue("PanelVisible") != "")
            //{
            //    bool visible = Boolean.Parse(ac.GetValue("PanelVisible"));
            //    if (visible)
            //        dpMenu.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            //    else
            //        dpMenu.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
            //}
            if (_sysMenu.GetMenuForDashboard("7").Rows.Count == 0)
            {   //khong co bao cao nhac nho
                bbiReminder.Visibility = BarItemVisibility.Never;
                return;
            }
            if (Config.GetValue("NhacNho") == null || Config.GetValue("NhacNho").ToString() != "1")
                return;
            FrmDashboard frm = new FrmDashboard(_sysMenu, true);
            if (frm.IsEmpty)
            {
                frm.Dispose();
                return;
            }
            if (frm.SoBaoCao > 3)
            {
                frm.MdiParent = this;
                frm.Show();
            }
            else
            {
                dpReminder.Tag = frm;
                foreach (Control c in frm.Controls)
                    dpReminder.Controls.Add(c);
                dpReminder.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                //dpMenu.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
            }
        }

        private void dockManager1_ClosingPanel(object sender, DevExpress.XtraBars.Docking.DockPanelCancelEventArgs e)
        {
            if (e.Panel.Tag != null && e.Panel.Tag.GetType() == typeof(FrmDashboard))
                (e.Panel.Tag as FrmDashboard).Close();
        }
        
        //customize them cho cac item tren toolbar duoc user dua them vao
        //private void barManagerMain_EndCustomization(object sender, EventArgs e)
        //{
        //    int imgId = 8;
        //    for (int i = 0; i < tbSystem.ItemLinks.Count; i++)
        //    {
        //        if (tbSystem.ItemLinks[i].Item.GetType() != typeof(BarLargeButtonItem))
        //            continue;
        //        BarLargeButtonItem bi = tbSystem.ItemLinks[i].Item as BarLargeButtonItem;
        //        if (!bi.IsLargeImageExist || bi.LargeImageIndex >= 8)
        //        {
        //            bi.BorderStyle = BarItemBorderStyle.Single;
        //            bi.LargeImageIndex = imgId++;
        //            int stt = imgId - 8;
        //            bi.ItemShortcut = new BarShortcut(GetShortcut("Alt" + stt.ToString()));
        //        }
        //    }
        //}

        private void bciFloat_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            if (bciFloat.Checked)
            {
                int w = 280;
                int h = SystemInformation.PrimaryMonitorSize.Height;
                this.WindowState = FormWindowState.Normal;
                this.Width = w;
                this.Height = h;
                this.Left = SystemInformation.PrimaryMonitorSize.Width - w;
                this.Top = SystemInformation.PrimaryMonitorSize.Height - h;
                this.TopMost = true;
                dpReminder.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                this.TopMost = false;
                dpReminder.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            }
        }

        //private void nbcSub_GroupExpanded(object sender, NavBarGroupEventArgs e)
        //{
        //    if (isSearching)
        //        return;
        //    for (int i = 0; i < nbcSub.Groups.Count; i++)
        //        if (nbcSub.Groups[i].Expanded && nbcSub.Groups[i] != e.Group)
        //            nbcSub.Groups[i].Expanded = false;
        //}

        //private void VisibleNavBarItems(string s)
        //{
        //    isSearching = s != "";
        //    for (int i = 0; i < nbcSub.Items.Count; i++)
        //        nbcSub.Items[i].Visible = s == "" || nbcSub.Items[i].Caption.ToUpper().Contains(s.ToUpper());
        //    if (s == "")
        //    {
        //        if (nbcSub.Groups.Count > 1)
        //        {
        //            nbcSub.Groups[1].Expanded = true;
        //            nbcSub_GroupExpanded(nbcSub, new NavBarGroupEventArgs(nbcSub.Groups[1]));
        //        }
        //        else
        //            if (nbcSub.Groups.Count > 0)
        //            {
        //                nbcSub.Groups[0].Expanded = true;
        //                nbcSub_GroupExpanded(nbcSub, new NavBarGroupEventArgs(nbcSub.Groups[0]));
        //            }
        //    }
        //    else
        //        for (int i = 0; i < nbcSub.Groups.Count; i++)
        //            nbcSub.Groups[i].Expanded = true;
        //}

        //private void teQuickSearch_Enter(object sender, EventArgs e)
        //{
        //    if (beSearch.Text == "Tìm nhanh chức năng...")
        //        beSearch.Text = "";
        //}

        //private void teQuickSearch_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter && beSearch.Text.Trim() != "")
        //    {
        //        VisibleNavBarItems(beSearch.Text.Trim());
        //        beSearch.Properties.Buttons[0].Visible = true;
        //    }
        //}

        //private void teQuickSearch_Leave(object sender, EventArgs e)
        //{
        //    if (!beSearch.Properties.Buttons[0].Visible)
        //        beSearch.Text = "Tìm nhanh chức năng...";
        //}

        //private void beSearch_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    VisibleNavBarItems("");
        //    beSearch.Properties.Buttons[0].Visible = false;
        //    beSearch.Text = "Tìm nhanh chức năng...";
        //    nbcSub.Focus();
        //}

        private void pictureEdit1_MouseClick(object sender, MouseEventArgs e)
        {
            Process.Start("http://www.hoatieuvietnam.com");
        }

        private void pictureEdit1_MouseHover(object sender, EventArgs e)
        {
            pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
        }

        private void pictureEdit1_MouseLeave(object sender, EventArgs e)
        {
            pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmDashboard frmRm = new FrmDashboard(_sysMenu, true);
            dpReminder.Tag = frmRm;
            dpReminder.Controls[0].Controls.RemoveByKey("layoutControl1");
            foreach (Control c in frmRm.Controls)
                dpReminder.Controls.Add(c);
        }
    }
}