using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
         
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Server = SERVER\HOATIEU;Database = UCDEMO;User = sa;pwd = ht";   
            string sql = "SELECT * FROM DMLopHoc";
            con.Open();
            SqlDataAdapter ad = new SqlDataAdapter(sql,con);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            gridControl1.DataSource = dt;
            gridView1.BestFitColumns();
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
        }

    }
}