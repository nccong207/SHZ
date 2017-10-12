using CDTDatabase;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SaoChepGroup
{
    public partial class Main : XtraForm
    {
        Database db = Database.NewStructDatabase();
        public Main()
        {
            InitializeComponent();
        }

        private void loadDataForDrop()
        {
            DataTable data = db.GetDataTable("SELECT DbName, CompanyName FROM sysDatabase WHERE sysSiteID = 18 ORDER BY DbName");
            gridLookUpEdit1.Properties.DataSource = data;
            gridLookUpEdit1.Properties.ValueMember = "DbName";
            //gridLookUpEdit1.Properties.DisplayMember = "CompanyName";
            gridLookUpEdit1View.Columns["DbName"].Width = 100;
            gridLookUpEdit1View.Columns["CompanyName"].Width = 300;

            gridLookUpEdit1.Properties.PopupFormMinSize = new Size(400, 300);

            gridLookUpEdit2.Properties.DataSource = data;
            gridLookUpEdit2.Properties.ValueMember = "DbName";
            //gridLookUpEdit2.Properties.DisplayMember = "CompanyName";
            gridLookUpEdit2View.Columns["DbName"].Width = 100;
            gridLookUpEdit2View.Columns["CompanyName"].Width = 300;
            gridLookUpEdit2.Properties.PopupFormMinSize = new Size(400, 300);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            loadDataForDrop();
            gridLookUpEdit1.EditValue = "";
            gridLookUpEdit2.EditValue = "";
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (gridLookUpEdit1.EditValue == null || string.IsNullOrEmpty(gridLookUpEdit1.EditValue.ToString()))
            {
                XtraMessageBox.Show("Phải chọn chi nhánh nguồn", "Lỗi nhập liệu");
                return;
            }

            if (gridLookUpEdit2.EditValue == null || string.IsNullOrEmpty(gridLookUpEdit2.EditValue.ToString()))
            {
                XtraMessageBox.Show("Phải chọn chi nhánh đích", "Lỗi nhập liệu");
                return;
            }

            string manguon = gridLookUpEdit1.EditValue.ToString();
            string madich = gridLookUpEdit2.EditValue.ToString();
            
            if (manguon.Equals(madich))
            {
                XtraMessageBox.Show("Phải chọn chi nhánh đích khác với chi nhánh nguồn", "Lỗi nhập liệu");
                return;
            }

            if (checkCopiedGroup(madich))
            {
                saochepUserSite(manguon, madich);
                // lọc isgroup = 1
                DataTable siteIdNguon = db.GetDataTable(string.Format(@"SELECT sysUserSiteID FROM sysUserSite a join sysUser b on a.sysUserID = b.sysUserID WHERE b.IsGroup = 1 and DbName = '{0}'", manguon));
                DataTable siteIdDich = db.GetDataTable(string.Format("SELECT sysUserSiteID FROM sysUserSite WHERE DbName = '{0}'", madich));
                saochepUserMenu(siteIdNguon, siteIdDich);
                saochepUserTable(siteIdNguon, siteIdDich);
                saochepUserField(siteIdNguon, siteIdDich);
                XtraMessageBox.Show(string.Format("Sao chép dữ liệu từ {0} sang {1} thành công.", manguon, madich), "Hoa Tieu");
            }
        }

        private void saochepUserSite (string manguon, string madich )
        {
            string updateQuery = @"INSERT INTO sysUserSite (sysUserID, sysSiteID, IsAdmin, DbName)
                                   SELECT a.sysUserID, a.sysSiteID, a.IsAdmin, '{0}'
                                   FROM sysUserSite a join sysUser b on a.sysUserID = b.sysUserID
                                   WHERE b.IsGroup = 1 and  DbName = '{1}'";

            db.UpdateByNonQuery(string.Format(updateQuery, madich, manguon));
        }

        private void saochepUserMenu(DataTable siteIdNguon, DataTable siteIdDich)
        {
            for (int i = 0; i < siteIdNguon.Rows.Count; i++)
            {
                var oldRow = siteIdNguon.Rows[i]["sysUserSiteID"];
                var newRow = siteIdDich.Rows[i]["sysUserSiteID"];

                string update = @"INSERT INTO sysUserMenu (sysMenuID, Executable, sysUserSiteID, sysMenuParentID)
                                  SELECT sysMenuID, Executable, {0}, sysMenuParentID
                                  FROM sysUserMenu WHERE sysUserSiteID = {1}";
                db.UpdateByNonQuery(string.Format(update, newRow, oldRow));
            }
        }
        private void saochepUserTable(DataTable siteIdNguon, DataTable siteIdDich)
        {
            for (int i = 0; i < siteIdNguon.Rows.Count; i++)
            {
                var oldRow = siteIdNguon.Rows[i]["sysUserSiteID"];
                var newRow = siteIdDich.Rows[i]["sysUserSiteID"];

                string update = @"INSERT INTO sysUserTable (sysTableID, sSelect, sInsert, sUpdate, sDelete, sysUserSiteID, sysMenuID)
                                SELECT sysTableID, sSelect, sInsert, sUpdate, sDelete, {0}, sysMenuID
                                FROM sysUserTable WHERE sysUserSiteID = {1}";
                db.UpdateByNonQuery(string.Format(update, newRow, oldRow));
            }
        }
        private void saochepUserField(DataTable siteIdNguon, DataTable siteIdDich)
        {
            for (int i = 0; i < siteIdNguon.Rows.Count; i++)
            {
                var oldRow = siteIdNguon.Rows[i]["sysUserSiteID"];
                var newRow = siteIdDich.Rows[i]["sysUserSiteID"];

                string update = @"INSERT INTO sysUserField (sysFieldID, Viewable, Editable, sysUserSiteID, sysTableID)
                                  SELECT sysFieldID, Viewable, Editable, {0}, sysTableID
                                  FROM sysUserField WHERE sysUserSiteID = {1}";
                db.UpdateByNonQuery(string.Format(update, newRow, oldRow));
            }
        }
        private bool checkCopiedGroup(string madich)
        {
            string sql = @"SELECT Count(DbName) 
                            FROM sysUserSite a join sysUser b on a.sysUserID = b.sysUserID
                            WHERE b.IsGroup = 1 and DbName = '{0}'";

            object value = db.GetValue(string.Format(sql, madich));
            if (value != null && Convert.ToInt32(value) > 0)
            {
                XtraMessageBox.Show("Chi nhánh đích đã được tạo group", "Lỗi sao chép");
                return false;
            }
            return true;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
