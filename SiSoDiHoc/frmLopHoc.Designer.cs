namespace SiSoDiHoc
{
    partial class frmLopHoc
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.checkAll = new DevExpress.XtraEditors.CheckEdit();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.gcLop = new DevExpress.XtraGrid.GridControl();
            this.gvLop = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GV1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GV2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GV3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.DK = new DevExpress.XtraGrid.Columns.GridColumn();
            this.DH = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
            this.ChoNghi = new DevExpress.XtraGrid.Columns.GridColumn();
            this.NGHI = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.col25 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.col10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colghichu = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkAll.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcLop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvLop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCalcEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.checkAll);
            this.layoutControl1.Controls.Add(this.btnCancel);
            this.layoutControl1.Controls.Add(this.btnOK);
            this.layoutControl1.Controls.Add(this.gcLop);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(788, 362);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // checkAll
            // 
            this.checkAll.Location = new System.Drawing.Point(7, 7);
            this.checkAll.Name = "checkAll";
            this.checkAll.Properties.Caption = "Hiện lớp nghỉ";
            this.checkAll.Size = new System.Drawing.Size(775, 19);
            this.checkAll.StyleController = this.layoutControl1;
            this.checkAll.TabIndex = 8;
            this.checkAll.CheckedChanged += new System.EventHandler(this.checkAll_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(396, 334);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 22);
            this.btnCancel.StyleController = this.layoutControl1;
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(308, 334);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(77, 22);
            this.btnOK.StyleController = this.layoutControl1;
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "Đồng ý";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // gcLop
            // 
            this.gcLop.EmbeddedNavigator.Name = "";
            this.gcLop.Location = new System.Drawing.Point(7, 37);
            this.gcLop.MainView = this.gvLop;
            this.gcLop.Name = "gcLop";
            this.gcLop.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCalcEdit1});
            this.gcLop.Size = new System.Drawing.Size(775, 286);
            this.gcLop.TabIndex = 4;
            this.gcLop.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvLop});
            // 
            // gvLop
            // 
            this.gvLop.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.GV1,
            this.GV2,
            this.GV3,
            this.DK,
            this.DH,
            this.col10,
            this.col25,
            this.colghichu,
            this.ChoNghi,
            this.NGHI});
            this.gvLop.GridControl = this.gcLop;
            this.gvLop.Name = "gvLop";
            this.gvLop.OptionsView.ColumnAutoWidth = false;
            this.gvLop.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Mã lớp";
            this.gridColumn1.FieldName = "MaLop";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Tên lớp";
            this.gridColumn2.FieldName = "TenLop";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Khai giảng";
            this.gridColumn3.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.gridColumn3.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn3.FieldName = "NgayBDKhoa";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Kết thúc";
            this.gridColumn4.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.gridColumn4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn4.FieldName = "NgayKTKhoa";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // GV1
            // 
            this.GV1.Caption = "Giáo viên 1";
            this.GV1.FieldName = "GV";
            this.GV1.Name = "GV1";
            this.GV1.OptionsColumn.AllowEdit = false;
            this.GV1.Visible = true;
            this.GV1.VisibleIndex = 4;
            // 
            // GV2
            // 
            this.GV2.Caption = "Giáo viên 2";
            this.GV2.FieldName = "GV2";
            this.GV2.Name = "GV2";
            this.GV2.OptionsColumn.AllowEdit = false;
            this.GV2.Visible = true;
            this.GV2.VisibleIndex = 5;
            // 
            // GV3
            // 
            this.GV3.Caption = "Giáo viên 3";
            this.GV3.FieldName = "GV3";
            this.GV3.Name = "GV3";
            this.GV3.OptionsColumn.AllowEdit = false;
            this.GV3.Visible = true;
            this.GV3.VisibleIndex = 6;
            // 
            // DK
            // 
            this.DK.Caption = "Sỉ số đăng ký";
            this.DK.DisplayFormat.FormatString = "###";
            this.DK.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.DK.FieldName = "SiSoHV";
            this.DK.Name = "DK";
            this.DK.OptionsColumn.AllowEdit = false;
            this.DK.Visible = true;
            this.DK.VisibleIndex = 7;
            // 
            // DH
            // 
            this.DH.Caption = "Sỉ số đi học";
            this.DH.ColumnEdit = this.repositoryItemCalcEdit1;
            this.DH.DisplayFormat.FormatString = "###";
            this.DH.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.DH.FieldName = "Siso";
            this.DH.Name = "DH";
            this.DH.Visible = true;
            this.DH.VisibleIndex = 8;
            // 
            // repositoryItemCalcEdit1
            // 
            this.repositoryItemCalcEdit1.AutoHeight = false;
            this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
            // 
            // ChoNghi
            // 
            this.ChoNghi.Caption = "Cho nghỉ";
            this.ChoNghi.FieldName = "isKT";
            this.ChoNghi.Name = "ChoNghi";
            this.ChoNghi.Visible = true;
            this.ChoNghi.VisibleIndex = 12;
            // 
            // NGHI
            // 
            this.NGHI.Caption = "Nghỉ";
            this.NGHI.FieldName = "isKT";
            this.NGHI.Name = "NGHI";
            this.NGHI.OptionsColumn.AllowEdit = false;
            this.NGHI.Visible = true;
            this.NGHI.VisibleIndex = 13;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem1,
            this.emptySpaceItem2,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(788, 362);
            this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcLop;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 30);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem1.Size = new System.Drawing.Size(786, 297);
            this.layoutControlItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnOK;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.layoutControlItem2.Location = new System.Drawing.Point(301, 327);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem2.Size = new System.Drawing.Size(88, 33);
            this.layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem2.Text = "layoutControlItem2";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnCancel;
            this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            this.layoutControlItem3.Location = new System.Drawing.Point(389, 327);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem3.Size = new System.Drawing.Size(87, 33);
            this.layoutControlItem3.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem3.Text = "layoutControlItem3";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextToControlDistance = 0;
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 327);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.emptySpaceItem1.Size = new System.Drawing.Size(301, 33);
            this.emptySpaceItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.emptySpaceItem1.Text = "emptySpaceItem1";
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
            this.emptySpaceItem2.Location = new System.Drawing.Point(476, 327);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.emptySpaceItem2.Size = new System.Drawing.Size(310, 33);
            this.emptySpaceItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.emptySpaceItem2.Text = "emptySpaceItem2";
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.checkAll;
            this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlItem5.Size = new System.Drawing.Size(786, 30);
            this.layoutControlItem5.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem5.Text = "layoutControlItem5";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextToControlDistance = 0;
            this.layoutControlItem5.TextVisible = false;
            // 
            // col25
            // 
            this.col25.Caption = "25";
            this.col25.ColumnEdit = this.repositoryItemCalcEdit1;
            this.col25.DisplayFormat.FormatString = "###";
            this.col25.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.col25.FieldName = "Ngay25";
            this.col25.Name = "col25";
            this.col25.Visible = true;
            this.col25.VisibleIndex = 10;
            // 
            // col10
            // 
            this.col10.Caption = "10";
            this.col10.ColumnEdit = this.repositoryItemCalcEdit1;
            this.col10.DisplayFormat.FormatString = "###";
            this.col10.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.col10.FieldName = "Ngay10";
            this.col10.Name = "col10";
            this.col10.Visible = true;
            this.col10.VisibleIndex = 9;
            // 
            // colghichu
            // 
            this.colghichu.Caption = "Ghi chú";
            this.colghichu.FieldName = "GhiChu";
            this.colghichu.Name = "colghichu";
            this.colghichu.Visible = true;
            this.colghichu.VisibleIndex = 11;
            // 
            // frmLopHoc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 362);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmLopHoc";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmLopHoc";
            this.Load += new System.EventHandler(this.frmLopHoc_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkAll.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcLop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvLop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCalcEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn GV1;
        private DevExpress.XtraGrid.Columns.GridColumn GV2;
        private DevExpress.XtraGrid.Columns.GridColumn GV3;
        private DevExpress.XtraGrid.Columns.GridColumn DK;
        private DevExpress.XtraGrid.Columns.GridColumn DH;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraGrid.Columns.GridColumn ChoNghi;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraGrid.GridControl gcLop;
        private DevExpress.XtraGrid.Views.Grid.GridView gvLop;
        private DevExpress.XtraEditors.CheckEdit checkAll;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit repositoryItemCalcEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn NGHI;
        private DevExpress.XtraGrid.Columns.GridColumn col10;
        private DevExpress.XtraGrid.Columns.GridColumn col25;
        private DevExpress.XtraGrid.Columns.GridColumn colghichu;
    }
}