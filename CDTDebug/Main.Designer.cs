using DevExpress.XtraBars.ViewInfo;
using DevExpress.Utils.Drawing;
using System.Drawing;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
namespace CDT
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.barManagerMain = new DevExpress.XtraBars.BarManager(this.components);
            this.barMainMenu = new DevExpress.XtraBars.Bar();
            this.barSubItemSystem = new DevExpress.XtraBars.BarSubItem();
            this.iChangePassword = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iUserConfig = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iTransferData = new DevExpress.XtraBars.BarButtonItem();
            this.iBackup = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iRestore = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iExportData = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iSyncData = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iCollectData = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iViewHistory = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iCheckData = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iNewDb = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iExit = new DevExpress.XtraBars.BarButtonItem();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bsiCurrentUser = new DevExpress.XtraBars.BarStaticItem();
            this.bsiLoginTime = new DevExpress.XtraBars.BarStaticItem();
            this.bsiCurrentPath = new DevExpress.XtraBars.BarStaticItem();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dpRefDoc = new DevExpress.XtraBars.Docking.DockPanel();
            this.dpHelp = new DevExpress.XtraBars.Docking.DockPanel();
            this.dpReminder = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.button1 = new System.Windows.Forms.Button();
            this.bsiUserName = new DevExpress.XtraBars.BarStaticItem();
            this.iHelpOnline = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iRemote = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iHelp = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iTLKT = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iAbout = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iRegister = new DevExpress.XtraBars.BarLargeButtonItem();
            this.iUserTrace = new DevExpress.XtraBars.BarLargeButtonItem();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.bbiDashboard = new DevExpress.XtraBars.BarLargeButtonItem();
            this.bbiReminder = new DevExpress.XtraBars.BarLargeButtonItem();
            this.beiDb = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemComboBox2 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.beiKyKeToan = new DevExpress.XtraBars.BarEditItem();
            this.beiNamLV = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemComboBox3 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repositoryItemComboBox5 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.beiStatus = new DevExpress.XtraBars.BarEditItem();
            this.riStatus = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.bciFloat = new DevExpress.XtraBars.BarCheckItem();
            this.lstImages = new DevExpress.Utils.ImageCollection(this.components);
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.repositoryItemTextEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.repositoryItemTextEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.riLueDb = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.riKyKeToan = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.riNamLV = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repositoryItemComboBox4 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.repositoryItemTextEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.defaultToolTipController1 = new DevExpress.Utils.DefaultToolTipController(this.components);
            this.barSubItemHelp = new DevExpress.XtraBars.BarSubItem();
            this.mdiTabMain = new DevExpress.XtraTabbedMdi.XtraTabbedMdiManager(this.components);
            this.barMdi = new DevExpress.XtraBars.BarMdiChildrenListItem();
            this.bsiModule = new DevExpress.XtraBars.BarSubItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.rcModule = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.rpModule = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.rpgModule = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.pmSubModule = new DevExpress.XtraBars.PopupMenu(this.components);
            this.lstImages2 = new DevExpress.Utils.ImageCollection(this.components);
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dpRefDoc.SuspendLayout();
            this.dpHelp.SuspendLayout();
            this.dpReminder.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lstImages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riLueDb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riKyKeToan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riNamLV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mdiTabMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pmSubModule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lstImages2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // barManagerMain
            // 
            this.barManagerMain.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.barMainMenu,
            this.bar1});
            this.barManagerMain.Controller = this.barAndDockingController1;
            this.barManagerMain.DockControls.Add(this.barDockControlTop);
            this.barManagerMain.DockControls.Add(this.barDockControlBottom);
            this.barManagerMain.DockControls.Add(this.barDockControlLeft);
            this.barManagerMain.DockControls.Add(this.barDockControlRight);
            this.barManagerMain.DockManager = this.dockManager1;
            this.barManagerMain.Form = this;
            this.barManagerMain.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barSubItemSystem,
            this.bsiUserName,
            this.iChangePassword,
            this.iCheckData,
            this.iNewDb,
            this.iHelpOnline,
            this.iRemote,
            this.iHelp,
            this.iTLKT,
            this.iAbout,
            this.iRegister,
            this.iBackup,
            this.iRestore,
            this.iUserConfig,
            this.iViewHistory,
            this.iUserTrace,
            this.iCollectData,
            this.iSyncData,
            this.iExportData,
            this.iExit,
            this.barStaticItem1,
            this.bbiDashboard,
            this.bbiReminder,
            this.beiDb,
            this.beiKyKeToan,
            this.beiNamLV,
            this.beiStatus,
            this.bciFloat,
            this.iTransferData,
            this.bsiCurrentUser,
            this.bsiLoginTime,
            this.bsiCurrentPath});
            this.barManagerMain.LargeImages = this.lstImages;
            this.barManagerMain.MainMenu = this.barMainMenu;
            this.barManagerMain.MaxItemId = 73;
            this.barManagerMain.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTextEdit1,
            this.repositoryItemTextEdit2,
            this.repositoryItemTextEdit3,
            this.riLueDb,
            this.riKyKeToan,
            this.riNamLV,
            this.repositoryItemComboBox2,
            this.repositoryItemComboBox3,
            this.repositoryItemComboBox4,
            this.repositoryItemTextEdit4,
            this.repositoryItemComboBox1,
            this.riStatus});
            this.barManagerMain.StatusBar = this.bar1;
            this.barManagerMain.ToolTipController = this.defaultToolTipController1.DefaultController;
            // 
            // barMainMenu
            // 
            this.barMainMenu.BarName = "Main menu";
            this.barMainMenu.DockCol = 0;
            this.barMainMenu.DockRow = 0;
            this.barMainMenu.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barMainMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemSystem)});
            this.barMainMenu.OptionsBar.DrawDragBorder = false;
            this.barMainMenu.OptionsBar.MultiLine = true;
            this.barMainMenu.OptionsBar.UseWholeRow = true;
            this.barMainMenu.Text = "Main menu";
            // 
            // barSubItemSystem
            // 
            this.barSubItemSystem.Caption = "Hệ thống";
            this.barSubItemSystem.Id = 0;
            this.barSubItemSystem.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.iChangePassword),
            new DevExpress.XtraBars.LinkPersistInfo(this.iUserConfig),
            new DevExpress.XtraBars.LinkPersistInfo(this.iTransferData),
            new DevExpress.XtraBars.LinkPersistInfo(this.iBackup),
            new DevExpress.XtraBars.LinkPersistInfo(this.iRestore),
            new DevExpress.XtraBars.LinkPersistInfo(this.iExportData),
            new DevExpress.XtraBars.LinkPersistInfo(this.iSyncData),
            new DevExpress.XtraBars.LinkPersistInfo(this.iCollectData),
            new DevExpress.XtraBars.LinkPersistInfo(this.iCheckData),
            new DevExpress.XtraBars.LinkPersistInfo(this.iNewDb),
            new DevExpress.XtraBars.LinkPersistInfo(this.iExit)});
            this.barSubItemSystem.Name = "barSubItemSystem";
            // 
            // iChangePassword
            // 
            this.iChangePassword.Caption = "Đổi mật khẩu";
            this.iChangePassword.Id = 13;
            this.iChangePassword.Name = "iChangePassword";
            // 
            // iUserConfig
            // 
            this.iUserConfig.Caption = "Tham số người dùng";
            this.iUserConfig.Id = 24;
            this.iUserConfig.Name = "iUserConfig";
            // 
            // iTransferData
            // 
            this.iTransferData.Caption = "Chuyển số liệu";
            this.iTransferData.Id = 69;
            this.iTransferData.Name = "iTransferData";
            // 
            // iBackup
            // 
            this.iBackup.Caption = "Sao lưu số liệu";
            this.iBackup.Id = 22;
            this.iBackup.Name = "iBackup";
            // 
            // iRestore
            // 
            this.iRestore.Caption = "Phục hồi số liệu";
            this.iRestore.Id = 23;
            this.iRestore.Name = "iRestore";
            // 
            // iExportData
            // 
            this.iExportData.Caption = "Kết xuất số liệu";
            this.iExportData.Id = 30;
            this.iExportData.Name = "iExportData";
            // 
            // iSyncData
            // 
            this.iSyncData.Caption = "Đồng bộ số liệu";
            this.iSyncData.Id = 29;
            this.iSyncData.Name = "iSyncData";
            // 
            // iCollectData
            // 
            this.iCollectData.Caption = "Tổng hợp số liệu";
            this.iCollectData.Id = 28;
            this.iCollectData.Name = "iCollectData";
            // 
            // iViewHistory
            // 
            this.iViewHistory.Caption = "Xem nhật ký cập nhật";
            this.iViewHistory.Id = 25;
            this.iViewHistory.Name = "iViewHistory";
            // 
            // iCheckData
            // 
            this.iCheckData.Caption = "Kiểm tra số liệu thô";
            this.iCheckData.Id = 14;
            this.iCheckData.Name = "iCheckData";
            // 
            // iNewDb
            // 
            this.iNewDb.Caption = "Tạo số liệu mới";
            this.iNewDb.Id = 31;
            this.iNewDb.Name = "iNewDb";
            // 
            // iExit
            // 
            this.iExit.Caption = "Kết thúc";
            this.iExit.Id = 36;
            this.iExit.Name = "iExit";
            // 
            // bar1
            // 
            this.bar1.BarName = "Custom 3";
            this.bar1.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bsiCurrentUser),
            new DevExpress.XtraBars.LinkPersistInfo(this.bsiLoginTime),
            new DevExpress.XtraBars.LinkPersistInfo(this.bsiCurrentPath)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DisableCustomization = true;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Custom 3";
            // 
            // bsiCurrentUser
            // 
            this.bsiCurrentUser.Caption = "Người dùng hiện tại: ";
            this.bsiCurrentUser.Id = 70;
            this.bsiCurrentUser.Name = "bsiCurrentUser";
            this.bsiCurrentUser.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // bsiLoginTime
            // 
            this.bsiLoginTime.Caption = "Thời gian đăng nhập: ";
            this.bsiLoginTime.Id = 71;
            this.bsiLoginTime.Name = "bsiLoginTime";
            this.bsiLoginTime.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // bsiCurrentPath
            // 
            this.bsiCurrentPath.Caption = "Thư mục làm việc: ";
            this.bsiCurrentPath.Id = 72;
            this.bsiCurrentPath.Name = "bsiCurrentPath";
            this.bsiCurrentPath.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // barAndDockingController1
            // 
            this.barAndDockingController1.AppearancesRibbon.Item.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.barAndDockingController1.AppearancesRibbon.Item.ForeColor = System.Drawing.Color.DarkRed;
            this.barAndDockingController1.AppearancesRibbon.Item.Options.UseFont = true;
            this.barAndDockingController1.AppearancesRibbon.Item.Options.UseForeColor = true;
            this.barAndDockingController1.LookAndFeel.SkinName = "Money Twins";
            this.barAndDockingController1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
            // 
            // dockManager1
            // 
            this.dockManager1.Controller = this.barAndDockingController1;
            this.dockManager1.DockModeVS2005FadeSpeed = 150;
            this.dockManager1.Form = this;
            this.dockManager1.HiddenPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dpRefDoc,
            this.dpHelp,
            this.dpReminder});
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"});
            this.dockManager1.ClosingPanel += new DevExpress.XtraBars.Docking.DockPanelCancelEventHandler(this.dockManager1_ClosingPanel);
            // 
            // dpRefDoc
            // 
            this.dpRefDoc.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.dpRefDoc.ID = new System.Guid("6a8edd78-e740-4459-8885-5619d0fa9c33");
            this.dpRefDoc.Location = new System.Drawing.Point(509, 75);
            this.dpRefDoc.Name = "dpRefDoc";
            this.dpRefDoc.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.dpRefDoc.SavedIndex = 0;
            this.dpRefDoc.Size = new System.Drawing.Size(210, 379);
            this.dpRefDoc.Text = "Tài liệu kế toán";
            this.dpRefDoc.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            // 
            // dpHelp
            // 
            this.dpHelp.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.dpHelp.ID = new System.Guid("f2cc83c3-5d3d-4bda-bbee-af143b74046f");
            this.dpHelp.Location = new System.Drawing.Point(464, 75);
            this.dpHelp.Name = "dpHelp";
            this.dpHelp.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.dpHelp.SavedIndex = 0;
            this.dpHelp.Size = new System.Drawing.Size(255, 379);
            this.dpHelp.Text = "Hướng dẫn thực hành";
            this.dpHelp.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            // 
            // dpReminder
            // 
            this.dpReminder.Controls.Add(this.dockPanel1_Container);
            this.dpReminder.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.dpReminder.ID = new System.Guid("2546313d-2e89-44b9-b522-252555bbef4f");
            this.dpReminder.Location = new System.Drawing.Point(604, 119);
            this.dpReminder.Name = "dpReminder";
            this.dpReminder.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.dpReminder.SavedIndex = 0;
            this.dpReminder.Size = new System.Drawing.Size(275, 326);
            this.dpReminder.Text = "Hệ thống nhắc nhở";
            this.dpReminder.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.button1);
            this.dockPanel1_Container.Location = new System.Drawing.Point(2, 24);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(271, 300);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Image = global::CDT.Properties.Resources.icon_refresh;
            this.button1.Location = new System.Drawing.Point(247, 1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 23);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bsiUserName
            // 
            this.bsiUserName.Caption = "Người dùng";
            this.bsiUserName.Id = 7;
            this.bsiUserName.Name = "bsiUserName";
            this.bsiUserName.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // iHelpOnline
            // 
            this.iHelpOnline.Caption = "Hỗ trợ trực tuyến";
            this.iHelpOnline.Id = 61;
            this.iHelpOnline.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F10);
            this.iHelpOnline.Name = "iHelpOnline";
            // 
            // iRemote
            // 
            this.iRemote.Caption = "Kết nối từ xa";
            this.iRemote.Id = 62;
            this.iRemote.Name = "iRemote";
            // 
            // iHelp
            // 
            this.iHelp.Caption = "Hướng dẫn thực hành";
            this.iHelp.Id = 15;
            this.iHelp.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F1);
            this.iHelp.Name = "iHelp";
            // 
            // iTLKT
            // 
            this.iTLKT.Caption = "Tài liệu kế toán";
            this.iTLKT.Id = 64;
            this.iTLKT.Name = "iTLKT";
            // 
            // iAbout
            // 
            this.iAbout.Caption = "Thông tin phần mềm";
            this.iAbout.Id = 65;
            this.iAbout.Name = "iAbout";
            // 
            // iRegister
            // 
            this.iRegister.Caption = "Đăng ký sử dụng";
            this.iRegister.Id = 55;
            this.iRegister.Name = "iRegister";
            // 
            // iUserTrace
            // 
            this.iUserTrace.Caption = "Xem lưu vết người dùng";
            this.iUserTrace.Id = 26;
            this.iUserTrace.Name = "iUserTrace";
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Caption = "Thông tin hệ thống:";
            this.barStaticItem1.Id = 40;
            this.barStaticItem1.Name = "barStaticItem1";
            this.barStaticItem1.OwnFont = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.barStaticItem1.TextAlignment = System.Drawing.StringAlignment.Near;
            this.barStaticItem1.UseOwnFont = true;
            // 
            // bbiDashboard
            // 
            this.bbiDashboard.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.bbiDashboard.Caption = "Đồ thị phân tích";
            this.bbiDashboard.Id = 54;
            this.bbiDashboard.Name = "bbiDashboard";
            // 
            // bbiReminder
            // 
            this.bbiReminder.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.bbiReminder.Caption = "Hệ thống nhắc nhở";
            this.bbiReminder.Id = 60;
            this.bbiReminder.Name = "bbiReminder";
            // 
            // beiDb
            // 
            this.beiDb.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.beiDb.Caption = "Đơn vị";
            this.beiDb.Edit = this.repositoryItemComboBox2;
            this.beiDb.Id = 58;
            this.beiDb.Name = "beiDb";
            // 
            // repositoryItemComboBox2
            // 
            this.repositoryItemComboBox2.AutoHeight = false;
            this.repositoryItemComboBox2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox2.Name = "repositoryItemComboBox2";
            // 
            // beiNamLamViec
            // 
            this.beiNamLV.Caption = "Năm tài chính";
            this.beiNamLV.Edit = this.repositoryItemComboBox5;
            this.beiNamLV.Id = 60;
            this.beiNamLV.Name = "beiNamLV";
            // 
            // beiKyKeToan
            // 
            this.beiKyKeToan.Caption = "Kỳ kế toán";
            this.beiKyKeToan.Edit = this.repositoryItemComboBox3;
            this.beiKyKeToan.Id = 59;
            this.beiKyKeToan.Name = "beiKyKeToan";
            // 
            // repositoryItemComboBox3
            // 
            this.repositoryItemComboBox3.AutoHeight = false;
            this.repositoryItemComboBox3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox3.Name = "repositoryItemComboBox3";
            // 
            // repositoryItemComboBox5
            // 
            this.repositoryItemComboBox5.AutoHeight = false;
            this.repositoryItemComboBox5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox5.Name = "repositoryItemComboBox5";
            // 
            // beiStatus
            // 
            this.beiStatus.Caption = "Status";
            this.beiStatus.Edit = this.riStatus;
            this.beiStatus.Id = 66;
            this.beiStatus.Name = "beiStatus";
            // 
            // riStatus
            // 
            this.riStatus.AutoHeight = false;
            this.riStatus.Name = "riStatus";
            // 
            // bciFloat
            // 
            this.bciFloat.Caption = "On top";
            this.bciFloat.Id = 1;
            this.bciFloat.Name = "bciFloat";
            this.bciFloat.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.bciFloat_CheckedChanged);
            // 
            // lstImages
            // 
            this.lstImages.ImageSize = new System.Drawing.Size(32, 32);
            this.lstImages.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("lstImages.ImageStream")));
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // repositoryItemTextEdit2
            // 
            this.repositoryItemTextEdit2.AutoHeight = false;
            this.repositoryItemTextEdit2.Name = "repositoryItemTextEdit2";
            // 
            // repositoryItemTextEdit3
            // 
            this.repositoryItemTextEdit3.AutoHeight = false;
            this.repositoryItemTextEdit3.Name = "repositoryItemTextEdit3";
            // 
            // riLueDb
            // 
            this.riLueDb.AutoHeight = false;
            this.riLueDb.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.riLueDb.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DbName", "Name7", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CompanyName", "Name3", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None)});
            this.riLueDb.Name = "riLueDb";
            this.riLueDb.NullText = "";
            this.riLueDb.DropDownRows = 10;
            this.riLueDb.ShowHeader = false;
            // 
            // riKyKeToan
            // 
            this.riKyKeToan.AutoHeight = false;
            this.riKyKeToan.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.riKyKeToan.DropDownRows = 12;
            this.riKyKeToan.Name = "riKyKeToan";
            this.riKyKeToan.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // riNamLV
            // 
            this.riNamLV.AutoHeight = false;
            this.riNamLV.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.riNamLV.DropDownRows = 20;
            this.riNamLV.Name = "riNamLV";
            this.riNamLV.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            // 
            // repositoryItemComboBox4
            // 
            this.repositoryItemComboBox4.AutoHeight = false;
            this.repositoryItemComboBox4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox4.Name = "repositoryItemComboBox4";
            // 
            // repositoryItemTextEdit4
            // 
            this.repositoryItemTextEdit4.AutoHeight = false;
            this.repositoryItemTextEdit4.Name = "repositoryItemTextEdit4";
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            // 
            // defaultToolTipController1
            // 
            // 
            // 
            // 
            this.defaultToolTipController1.DefaultController.InitialDelay = 1;
            this.defaultToolTipController1.DefaultController.ReshowDelay = 1;
            // 
            // barSubItemHelp
            // 
            this.barSubItemHelp.Caption = "Trợ giúp";
            this.barSubItemHelp.Id = 27;
            this.barSubItemHelp.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.iRegister),
            new DevExpress.XtraBars.LinkPersistInfo(this.iHelp),
            new DevExpress.XtraBars.LinkPersistInfo(this.iHelpOnline),
            new DevExpress.XtraBars.LinkPersistInfo(this.iRemote),
            new DevExpress.XtraBars.LinkPersistInfo(this.iTLKT),
            new DevExpress.XtraBars.LinkPersistInfo(this.iAbout)});
            this.barSubItemHelp.Name = "barSubItemHelp";
            // 
            // mdiTabMain
            // 
            this.mdiTabMain.Controller = this.barAndDockingController1;
            this.mdiTabMain.HeaderButtonsShowMode = DevExpress.XtraTab.TabButtonShowMode.WhenNeeded;
            this.mdiTabMain.MdiParent = this;
            this.mdiTabMain.SelectedPageChanged += new System.EventHandler(this.mdiTabMain_SelectedPageChanged);
            this.mdiTabMain.PageRemoved += new DevExpress.XtraTabbedMdi.MdiTabPageEventHandler(this.mdiTabMain_PageRemoved);
            // 
            // barMdi
            // 
            this.barMdi.Caption = "Chuyển cửa &sổ";
            this.barMdi.Id = 32;
            this.barMdi.Name = "barMdi";
            // 
            // bsiModule
            // 
            this.bsiModule.Caption = "&Phần hành nghiệp vụ";
            this.bsiModule.Id = 34;
            this.bsiModule.Name = "bsiModule";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // rcModule
            // 
            this.rcModule.ApplicationButtonKeyTip = "";
            this.rcModule.ApplicationIcon = null;
            this.rcModule.Controller = this.barAndDockingController1;
            this.rcModule.Font = new System.Drawing.Font("Arial", 9.75F);
            this.rcModule.Location = new System.Drawing.Point(0, 25);
            this.rcModule.Name = "rcModule";
            this.rcModule.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.rpModule});
            this.rcModule.SelectedPage = this.rpModule;
            this.rcModule.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            this.rcModule.ShowToolbarCustomizeItem = false;
            this.rcModule.Size = new System.Drawing.Size(879, 94);
            this.rcModule.Toolbar.ShowCustomizeItem = false;
            this.rcModule.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            this.rcModule.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.rcModule_ItemClick);
            // 
            // rpModule
            // 
            this.rpModule.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.rpgModule});
            this.rpModule.KeyTip = "";
            this.rpModule.Name = "rpModule";
            this.rpModule.Text = "Chọn phân hệ";
            // 
            // rpgModule
            // 
            this.rpgModule.KeyTip = "";
            this.rpgModule.Name = "rpgModule";
            this.rpgModule.ShowCaptionButton = false;
            // 
            // pmSubModule
            // 
            this.pmSubModule.Name = "pmSubModule";
            this.pmSubModule.Ribbon = this.rcModule;
            // 
            // lstImages2
            // 
            this.lstImages2.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("lstImages2.ImageStream")));
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureEdit1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureEdit1.EditValue = global::CDT.Properties.Resources.NewLogo_small2;
            this.pictureEdit1.Location = new System.Drawing.Point(764, 27);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.AllowFocused = false;
            this.pictureEdit1.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(221)))), ((int)(((byte)(245)))));
            this.pictureEdit1.Properties.Appearance.Options.UseBackColor = true;
            this.pictureEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pictureEdit1.Properties.PictureAlignment = System.Drawing.ContentAlignment.BottomRight;
            this.pictureEdit1.Properties.ReadOnly = true;
            this.pictureEdit1.Properties.ShowMenu = false;
            this.pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.pictureEdit1.Size = new System.Drawing.Size(112, 37);
            this.pictureEdit1.TabIndex = 7;
            this.pictureEdit1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureEdit1_MouseClick);
            this.pictureEdit1.MouseLeave += new System.EventHandler(this.pictureEdit1_MouseLeave);
            this.pictureEdit1.MouseHover += new System.EventHandler(this.pictureEdit1_MouseHover);
            // 
            // Main
            // 
            this.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.Appearance.Options.UseBackColor = true;
            this.ClientSize = new System.Drawing.Size(879, 445);
            this.Controls.Add(this.pictureEdit1);
            this.Controls.Add(this.rcModule);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.LookAndFeel.SkinName = "Blue";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "Main";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Main_Load);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Main_KeyUp);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dpRefDoc.ResumeLayout(false);
            this.dpHelp.ResumeLayout(false);
            this.dpReminder.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lstImages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riLueDb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riKyKeToan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riNamLV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mdiTabMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pmSubModule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lstImages2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManagerMain;
        private DevExpress.XtraBars.Bar barMainMenu;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarSubItem barSubItemSystem;
        private DevExpress.XtraBars.BarStaticItem bsiUserName;
        private DevExpress.XtraBars.BarLargeButtonItem iChangePassword;
        private DevExpress.XtraBars.BarLargeButtonItem iCheckData;
        private DevExpress.XtraBars.BarLargeButtonItem iNewDb;
        private DevExpress.XtraBars.BarLargeButtonItem iHelp;
        private DevExpress.XtraBars.BarLargeButtonItem iHelpOnline;
        private DevExpress.XtraBars.BarLargeButtonItem iRemote;
        private DevExpress.XtraBars.BarLargeButtonItem iTLKT;
        private DevExpress.XtraBars.BarLargeButtonItem iRegister;
        private DevExpress.XtraBars.BarLargeButtonItem iAbout;
        private DevExpress.XtraBars.BarLargeButtonItem iBackup;
        private DevExpress.XtraBars.BarLargeButtonItem iRestore;
        private DevExpress.XtraBars.BarLargeButtonItem iUserConfig;
        private DevExpress.XtraTabbedMdi.XtraTabbedMdiManager mdiTabMain;
        private DevExpress.XtraBars.BarLargeButtonItem iViewHistory;
        private DevExpress.XtraBars.BarLargeButtonItem iUserTrace;
        private DevExpress.XtraBars.BarSubItem barSubItemHelp;
        private DevExpress.XtraBars.BarLargeButtonItem iCollectData;
        private DevExpress.XtraBars.BarLargeButtonItem iSyncData;
        private DevExpress.XtraBars.BarLargeButtonItem iExportData;
        private DevExpress.XtraBars.BarMdiChildrenListItem barMdi;
        private DevExpress.XtraBars.BarSubItem bsiModule;
        private DevExpress.XtraBars.BarButtonItem iExit;
        private DevExpress.XtraBars.BarStaticItem barStaticItem1;
        private DevExpress.Utils.ImageCollection lstImages;
        private DevExpress.Utils.DefaultToolTipController defaultToolTipController1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dpRefDoc;
        private DevExpress.XtraBars.Docking.DockPanel dpHelp;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit2;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit3;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit riLueDb;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox riKyKeToan;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox riNamLV;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraBars.BarLargeButtonItem bbiDashboard;
        private DevExpress.XtraBars.BarLargeButtonItem bbiReminder;
        private DevExpress.XtraBars.BarEditItem beiDb;
        private DevExpress.XtraBars.BarEditItem beiKyKeToan;
        private DevExpress.XtraBars.BarEditItem beiNamLV;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox5;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox4;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox2;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox3;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit4;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private DevExpress.XtraBars.Docking.DockPanel dpReminder;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraBars.BarEditItem beiStatus;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit riStatus;
        private DevExpress.XtraBars.BarCheckItem bciFloat;
        private DevExpress.XtraBars.Ribbon.RibbonControl rcModule;
        private DevExpress.XtraBars.Ribbon.RibbonPage rpModule;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgModule;
        private DevExpress.XtraBars.PopupMenu pmSubModule;
        private DevExpress.XtraBars.BarButtonItem iTransferData;
        private DevExpress.Utils.ImageCollection lstImages2;
        private BarAndDockingController barAndDockingController1;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private System.Windows.Forms.Button button1;
        private Bar bar1;
        private BarStaticItem bsiCurrentUser;
        private BarStaticItem bsiLoginTime;
        private BarStaticItem bsiCurrentPath;
    }
}

