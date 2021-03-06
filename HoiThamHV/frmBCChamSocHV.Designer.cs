namespace HoiThamHV
{
    partial class frmBCChamSocHV
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
            this.gcListCSHV = new DevExpress.XtraGrid.GridControl();
            this.gvListCSHV = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GhiChu = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gcListCSHV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvListCSHV)).BeginInit();
            this.SuspendLayout();
            // 
            // gcListCSHV
            // 
            this.gcListCSHV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcListCSHV.EmbeddedNavigator.Name = "";
            this.gcListCSHV.Location = new System.Drawing.Point(0, 0);
            this.gcListCSHV.MainView = this.gvListCSHV;
            this.gcListCSHV.Name = "gcListCSHV";
            this.gcListCSHV.Size = new System.Drawing.Size(706, 350);
            this.gcListCSHV.TabIndex = 0;
            this.gcListCSHV.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvListCSHV});
            // 
            // gvListCSHV
            // 
            this.gvListCSHV.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.GhiChu});
            this.gvListCSHV.GridControl = this.gcListCSHV;
            this.gvListCSHV.Name = "gvListCSHV";
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Ngày hỏi thăm";
            this.gridColumn1.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.gridColumn1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn1.FieldName = "Ngay";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Người hỏi thăm";
            this.gridColumn2.FieldName = "MaNV";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Phản hồi HV";
            this.gridColumn3.FieldName = "PHoi";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Kế hoạch tiếp theo";
            this.gridColumn4.FieldName = "KHTT";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Ngày thực hiện";
            this.gridColumn5.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.gridColumn5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn5.FieldName = "NgayTH";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            // 
            // GhiChu
            // 
            this.GhiChu.Caption = "Ghi chú";
            this.GhiChu.FieldName = "GhiChu";
            this.GhiChu.Name = "GhiChu";
            this.GhiChu.Visible = true;
            this.GhiChu.VisibleIndex = 5;
            // 
            // frmBCChamSocHV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 350);
            this.Controls.Add(this.gcListCSHV);
            this.Name = "frmBCChamSocHV";
            this.Text = "frmBCChamSocHV";
            this.Load += new System.EventHandler(this.frmBCChamSocHV_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gcListCSHV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvListCSHV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gcListCSHV;
        private DevExpress.XtraGrid.Views.Grid.GridView gvListCSHV;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn GhiChu;
    }
}