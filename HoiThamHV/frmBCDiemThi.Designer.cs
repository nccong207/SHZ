namespace HoiThamHV
{
    partial class frmBCDiemThi
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
            this.gvListDiemThi = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcListDiemThi = new DevExpress.XtraGrid.GridControl();
            ((System.ComponentModel.ISupportInitialize)(this.gvListDiemThi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcListDiemThi)).BeginInit();
            this.SuspendLayout();
            // 
            // gvListDiemThi
            // 
            this.gvListDiemThi.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5});
            this.gvListDiemThi.GridControl = this.gcListDiemThi;
            this.gvListDiemThi.Name = "gvListDiemThi";
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
            this.gridColumn2.Caption = "Điểm viết";
            this.gridColumn2.DisplayFormat.FormatString = "##.#0";
            this.gridColumn2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn2.FieldName = "Viet";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Điểm nghe";
            this.gridColumn3.DisplayFormat.FormatString = "##.#0";
            this.gridColumn3.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn3.FieldName = "Nghe";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Điểm nói";
            this.gridColumn4.DisplayFormat.FormatString = "##.#0";
            this.gridColumn4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn4.FieldName = "Noi";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Điểm trung bình";
            this.gridColumn5.DisplayFormat.FormatString = "##.#0";
            this.gridColumn5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn5.FieldName = "DiemTB";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            // 
            // gcListDiemThi
            // 
            this.gcListDiemThi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcListDiemThi.EmbeddedNavigator.Name = "";
            this.gcListDiemThi.Location = new System.Drawing.Point(0, 0);
            this.gcListDiemThi.MainView = this.gvListDiemThi;
            this.gcListDiemThi.Name = "gcListDiemThi";
            this.gcListDiemThi.Size = new System.Drawing.Size(639, 409);
            this.gcListDiemThi.TabIndex = 0;
            this.gcListDiemThi.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvListDiemThi});
            // 
            // frmBCDiemThi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 409);
            this.Controls.Add(this.gcListDiemThi);
            this.Name = "frmBCDiemThi";
            this.Text = "frmBCDiemThi";
            this.Load += new System.EventHandler(this.frmBCDiemThi_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvListDiemThi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcListDiemThi)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.Views.Grid.GridView gvListDiemThi;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.GridControl gcListDiemThi;



    }
}