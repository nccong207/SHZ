using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using CDTLib;
using DevExpress;
using DevExpress.XtraEditors;
using Plugins;
using System.Windows.Forms;
namespace HoiThamHV
{
    public class  HoiThamHV: IC
    {
        #region IC Members

        private List<InfoCustom> _lstInfo = new List<InfoCustom>();

        public HoiThamHV()
        {
            InfoCustom ic = new InfoCustom(1173, "Chăm sóc học viên", "Quản lý học viên");
            _lstInfo.Add(ic);
        }

        public void Execute(System.Data.DataRow drMenu)
        {
            int menuID = Int32.Parse(drMenu["MenuPluginID"].ToString());
            if (_lstInfo[0].CType == ICType.Custom && _lstInfo[0].MenuID == menuID)
            {
                frmHoiTham frm = new frmHoiTham();
                frm.Text = "Chăm sóc học viên";
                frm.ShowDialog();
                Form main = null;
                foreach (Form fr in Application.OpenForms)
                    if (fr.IsMdiContainer)
                        main = fr;

                if (frm.Value == 0)
                {
                    foreach (Form fr in Application.OpenForms)
                    {
                        if (fr.Text == "Chăm sóc học viên tư vấn")
                        {
                            fr.Activate();
                            return;
                        }
                    }
                    frmHVTV fr1 = new frmHVTV();//drMenu["ExtraSql"].ToString()
                    fr1.Text = "Chăm sóc học viên tư vấn";
                    if (main == null)
                    {
                        fr1.WindowState = System.Windows.Forms.FormWindowState.Normal;
                        fr1.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                        fr1.ShowDialog();
                    }
                    else
                    {
                        fr1.MdiParent = main;
                        fr1.Show();
                    }
                }
                else if (frm.Value == 1)
                {
                    foreach (Form fr in Application.OpenForms)
                    {
                        if (fr.Text == "Chăm sóc học viên đăng ký")
                        {
                            fr.Activate();
                            return;
                        }
                    }
                    frmHVDK fr1 = new frmHVDK();
                    fr1.Text = "Chăm sóc học viên đăng ký";
                    if (main == null)
                    {
                        fr1.WindowState = System.Windows.Forms.FormWindowState.Normal;
                        fr1.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                        fr1.ShowDialog();
                    }
                    else
                    {
                        fr1.MdiParent = main;
                        fr1.Show();
                    }
                }
                
            }
        }

        public List<InfoCustom> LstInfo
        {
            get { return _lstInfo; }
        }

        #endregion
    }
}
