using CDTLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace SaoChepGroup
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //tuy theo moi soft co productName khac nhau
            string siteCode = "HTA"; //giá trị mặc định
            if (args.Length > 0)
                siteCode = args[0];
            Config.NewKeyValue("SiteCode", siteCode);

            InitApp();
            SetEnvironment(siteCode);

            var form = new Main();
            form.StartPosition = FormStartPosition.CenterScreen;
            Application.Run(form);
        }

        private static void InitApp()
        {
            //lay style mac dinh cho form
            DevExpress.UserSkins.BonusSkins.Register();
            DevExpress.UserSkins.OfficeSkins.Register();
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeelMain = new DevExpress.LookAndFeel.DefaultLookAndFeel();
            defaultLookAndFeelMain.LookAndFeel.SetSkinStyle("Money Twins");
        }

        private static void SetEnvironment(string siteCode)
        {
            System.Globalization.CultureInfo CultureInfo = System.Windows.Forms.Application.CurrentCulture.Clone() as System.Globalization.CultureInfo;
            CultureInfo = new CultureInfo("en-US");
            DateTimeFormatInfo dtInfo = new DateTimeFormatInfo();
            dtInfo.LongDatePattern = "MM/dd/yyyy h:mm:ss tt";
            dtInfo.ShortDatePattern = "MM/dd/yyyy";
            CultureInfo.DateTimeFormat = dtInfo;
            System.Windows.Forms.Application.CurrentCulture = CultureInfo;

            //lay chuoi ket noi
            AppCon ac = new AppCon();
            string StructConnection = ac.GetValue("StructDb");
            if (StructConnection == "" && File.Exists("InstallerMng.exe"))
            {
                ProcessStartInfo psi = new ProcessStartInfo("InstallerMng.exe", siteCode);
                Process.Start(psi);
                Environment.Exit(0);
            }
            StructConnection = Security.DeCode(StructConnection);
            string structDb = "CDT" + ac.GetValue("ShortName");
            Config.NewKeyValue("StructDb", structDb);
            Config.NewKeyValue("StructConnection", StructConnection);
            Config.NewKeyValue("DataConnection", "STDSHZ");
        }
    }
}
