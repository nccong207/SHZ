using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using CDTLib;
using CDTDatabase;
using Plugins;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
using CBSControls;
using System.Data;
using DevExpress.XtraLayout.Utils;
using FormFactory;
using System.Globalization;

namespace HVDK
{ 
    //Tạo mã học viên, tính học phí, nguồn học viên, giáo trình, quà tặng
    public class HVDK:ICControl
    {        
        private InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();
        public DataRow drMaster;              
        GridControl gc;
        GridView gv; 
        LayoutControl lc;
        bool flag = false;// dung để tạo mã học viên
        bool isFilter = true; // dùng để filter

        RadioGroup raGroup;
        RadioGroup raHTMUA;
        decimal tienBLCon = 0; // tiền bảo lưu còn: lưu giữ số tiền dư của hv bảo lưu nếu có sau khi đã trừ học phí cần nộp

        DataView dvLopHoc;
        GridLookUpEdit gluCNHOC;
        GridLookUpEdit gridLKHVDK;
        GridLookUpEdit gridLKHVTV;        
        CalcEdit HPThucNop;
        CalcEdit GiamHP;
        DateEdit NgayDK;
        //DataTable dtCTGioHoc = new DataTable();
        NumberFormatInfo nfi = new NumberFormatInfo();        
        CultureInfo ci = Application.CurrentCulture;
        

        #region ICControl Members

        public void AddEvent()
        {
            nfi.CurrencyDecimalSeparator = ".";
            nfi.CurrencyGroupSeparator = ",";

            string sql = "select MaLop, MaGioHoc from DMLopHoc where isKT = 0";
            DataTable dt = db.GetDataTable(sql);
            dvLopHoc = new DataView(dt);           

            lc = data.FrmMain.Controls.Find("LcMain", true)[0] as LayoutControl;

            gv = (data.FrmMain.Controls.Find("gcMain",true)[0] as GridControl).MainView as GridView;                      
            //Ẩn hiện layout
            raGroup = data.FrmMain.Controls.Find("NguonHV", true)[0] as RadioGroup;
            if (raGroup != null)                
                raGroup.EditValueChanged += new EventHandler(raGroup_EditValueChanged);
             
            raHTMUA = data.FrmMain.Controls.Find("HTMua", true)[0] as RadioGroup;
            if (raHTMUA != null)
                raHTMUA.EditValueChanged += new EventHandler(raHTMUA_EditValueChanged);
            //ActiveFilterString
            gridLKHVDK = data.FrmMain.Controls.Find("MaHVDK", true)[0] as GridLookUpEdit;
            gridLKHVDK.Popup += new EventHandler(gridLKHVDK_Popup);
            gridLKHVDK.Properties.View.ColumnFilterChanged += new EventHandler(View_ColumnFilterChanged);
            gluCNHOC = data.FrmMain.Controls.Find("MaCNHoc",true)[0] as GridLookUpEdit;
            gridLKHVTV = data.FrmMain.Controls.Find("HVTVID", true)[0] as GridLookUpEdit;
            gridLKHVTV.Popup += new EventHandler(gridLKHVTV_Popup);

            CalcEdit calSoBo = data.FrmMain.Controls.Find("SoLuong", true)[0] as CalcEdit;
            calSoBo.EditValueChanged += new EventHandler(calSoBo_EditValueChanged);           

            //////////////////// mới thêm xử lý cho chuyển lớp dùng quy trình (nhập học phí cột bảo lưu ko thay đổi ), xử lý cột bảo lưu
            CalcEdit calThucThu = data.FrmMain.Controls.Find("ThucThu", true)[0] as CalcEdit;
            calThucThu.EditValueChanged += new EventHandler(calThucThu_EditValueChanged);

            //Chỉ hiển thị các lớp để đăng ký là lớp đang học
            GridLookUpEdit gridLKMaLop = data.FrmMain.Controls.Find("MaLop", true)[0] as GridLookUpEdit;
            gridLKMaLop.Popup += new EventHandler(gridLKMaLop_Popup);
            gridLKMaLop.Leave += new EventHandler(gridLKMaLop_Leave);

            // Bổ sung cho trường hợp get focus cho cải tiến 1 và 2
            HPThucNop = data.FrmMain.Controls.Find("ThucThu", true)[0] as CalcEdit;
            GiamHP = data.FrmMain.Controls.Find("GiamHP", true)[0] as CalcEdit;
            GiamHP.Leave += new EventHandler(GiamHP_Leave);
            
            NgayDK = data.FrmMain.Controls.Find("NgayDK", true)[0] as DateEdit;

            data.FrmMain.Shown += new EventHandler(FrmMain_Shown); 

            //tạo mã học viên
            if (data.BsMain.Current == null) // mới thêm để khi chạy thiết lập quy trình từ chuyển lớp k bị lỗi
                return;
            drMaster = (data.BsMain.Current as DataRowView).Row;   

            data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(data.BsMain, new EventArgs());
              
        }

        void View_ColumnFilterChanged(object sender, EventArgs e)
        {
            if (isFilter)
                return;
            isFilter = true;
            GridView gvHVDK = sender as GridView;
            drMaster = (data.BsMain.Current as DataRowView).Row; //nếu ko thêm, khi esc và thêm mới lại báo lỗi
            if (drMaster["NguonHV"].ToString() != "")
            {
                if (drMaster["NguonHV"].ToString() == "1")
                {
                    if (gvHVDK.ActiveFilterString == "")
                        gvHVDK.ActiveFilterString = "IsBL = False and IsNghiHoc = False";
                    else
                        gvHVDK.ActiveFilterString = "(" + gvHVDK.ActiveFilterString + ") and (IsBL = False and IsNghiHoc = False)";
                }
                else if (drMaster["NguonHV"].ToString() == "2")
                {
                    if (gvHVDK.ActiveFilterString == "")
                        gvHVDK.ActiveFilterString = "IsBL = True and isDKL = False and NgayHH >= #" + drMaster["NgayDK"].ToString() + "#";
                    else
                        gvHVDK.ActiveFilterString = "(" + gvHVDK.ActiveFilterString + ") and (IsBL = True and isDKL = False and NgayHH >= #" + drMaster["NgayDK"].ToString() + "#)";
                }
            }
            isFilter = false;
        }

        void gridLKMaLop_Leave(object sender, EventArgs e)
        {
            GiamHP.Focus();
        }       

        void GiamHP_Leave(object sender, EventArgs e)
        {
            HPThucNop.Focus();
        }                       
        
        void calThucThu_EditValueChanged(object sender, EventArgs e)
        {
            Application.CurrentCulture.NumberFormat = nfi;

            CalcEdit calThucThu = sender as CalcEdit;
            if (calThucThu == null)
                return;
            if (calThucThu.Properties.ReadOnly == true)
                return;
            if (NgayDK.Properties.ReadOnly == false)
                return;
            DataSet ds = data.BsMain.DataSource as DataSet;
            if (ds == null)
                return;
            //if (drMaster != null)
            //    return;
            DataRow dr;
            if (data.BsMain.Current != null)
                dr = (data.BsMain.Current as DataRowView).Row;
            else
            {
                DataTable dt0 = ds.Tables[0];
                DataView dv0 = new DataView(dt0);
                dv0.RowStateFilter = DataViewRowState.Added | DataViewRowState.ModifiedCurrent;
                if (dv0.Count == 0)
                    return;
                dr = dv0[0].Row;
            }
            if (dr == null)
                return;
            dr["ThucThu"] = calThucThu.EditValue.ToString();
            decimal tienHP = decimal.Parse(dr["TienHP"].ToString(),nfi);
            decimal tienTN = decimal.Parse(calThucThu.EditValue.ToString(),nfi);
            decimal tienCL = tienHP - tienTN;
            decimal tienBLDu = decimal.Parse(dr["BLSoTien"].ToString(),nfi); // trường hợp tiền còn lại của lớp trước > hp lớp đăng ký
            if (tienCL < 0)
            {
                dr["ConLai"] = 0;
                dr["BLSoTien"] = RoundNumber(Math.Abs(tienCL)+ tienBLDu);
            }
            else
            {
                dr["ConLai"] = RoundNumber(tienCL);
                dr["BLSoTien"] = tienBLDu;
            }
            dr.EndEdit();
            Application.CurrentCulture = ci;
        }        

        void gridLKMaLop_Popup(object sender, EventArgs e)
        {
            if (drMaster == null || drMaster.RowState == DataRowState.Deleted)
                return;
            GridLookUpEdit gridLKMaLop = sender as GridLookUpEdit;
            GridView gvLKMaLop = gridLKMaLop.Properties.View as GridView;
            gvLKMaLop.ClearColumnsFilter();
            gvLKMaLop.ActiveFilterString = "MaCN = '"+drMaster["MaCNHoc"].ToString()+"' and isKT = 0";
        }

        void calSoBo_EditValueChanged(object sender, EventArgs e)
        {
            CalcEdit cal = sender as CalcEdit;
            if (cal.Properties.ReadOnly || cal.EditValue == null || cal.EditValue == DBNull.Value)
                return;
            DataSet ds = data.BsMain.DataSource as DataSet;
            if (ds == null)
                return;
            DataRow drCurrent;
            if (data.BsMain.Current == null)
            {
                DataTable dt0 = ds.Tables[0];
                DataView dv0 = new DataView(dt0);
                dv0.RowStateFilter = DataViewRowState.Added | DataViewRowState.ModifiedCurrent;
                if (dv0.Count == 0)
                    return;
                drCurrent = dv0[0].Row;
            }
            else            
                drCurrent = (data.BsMain.Current as DataRowView).Row;
            
            drCurrent["SoLuong"] = cal.EditValue;
            DataTable dtQT = ds.Tables[1];
            DataView dvQT = new DataView(dtQT);
            dvQT.RowFilter = "HVID = '" + drCurrent["HVID"].ToString() + "'";
            foreach (DataRowView drv in dvQT)
            {
                if (drv["isQT"].ToString().ToUpper().Equals("FALSE"))
                    drv["SL"] = cal.EditValue;
            }
        }

        void gridLKHVTV_Popup(object sender, EventArgs e)
        {
            isFilter = true;
            GridLookUpEdit gridLKHVTV = sender as GridLookUpEdit;
            GridView gvHVTV = gridLKHVTV.Properties.View as GridView;
            gvHVTV.ClearColumnsFilter();
                       
            GridView gvHVDK = gridLKHVDK.Properties.View as GridView;
            gvHVDK.ClearColumnsFilter();
            drMaster = (data.BsMain.Current as DataRowView).Row; //nếu ko thêm, khi esc và thêm mới lại báo lỗi
            if (drMaster["NguonHV"].ToString() != "")
            {
                if (drMaster["NguonHV"].ToString() == "0")
                {                    
                    //gvHVTV.ActiveFilterString = " isMoi = 1";                    
                } 
                else if (drMaster["NguonHV"].ToString() == "1")
                {
                    gvHVDK.ActiveFilterString = " IsBL = False and IsNghiHoc = False";
                }
                else if (drMaster["NguonHV"].ToString() == "2")
                {
                    gvHVDK.ActiveFilterString = "IsBL = True and isDKL = False and NgayHH >= #" + drMaster["NgayDK"].ToString() + "#";
                }
            }
            isFilter = false;
        }

        void gridLKHVDK_Popup(object sender, EventArgs e)
        {
            isFilter = true;
            GridLookUpEdit gridLKHVDK = sender as GridLookUpEdit;
            GridView gvHVDK = gridLKHVDK.Properties.View as GridView;
            gvHVDK.ClearColumnsFilter();

            GridView gvHVTV = gridLKHVTV.Properties.View as GridView;
            gvHVTV.ClearColumnsFilter();
           drMaster = (data.BsMain.Current as DataRowView).Row; //nếu ko thêm, khi esc và thêm mới lại báo lỗi
            if (drMaster["NguonHV"].ToString() != "")
            {
                if (drMaster["NguonHV"].ToString() == "0")
                {
                    //gvHVTV.ActiveFilterString = "isMoi = True";
                }
                else if (drMaster["NguonHV"].ToString() == "1")
                {
                    gvHVDK.ActiveFilterString = " IsBL = False and IsNghiHoc = False";
                }
                else if (drMaster["NguonHV"].ToString() == "2")
                {
                    gvHVDK.ActiveFilterString = "IsBL = True and isDKL = False and NgayHH >= #" + drMaster["NgayDK"].ToString() + "#";
                }
            }
            isFilter = false;
        }
        void FrmMain_Shown(object sender, EventArgs e)
        {
            if (raHTMUA.EditValue == null || raHTMUA.EditValue.ToString() == "")
            {
                lc.Items.FindByName("lciSoLuong").Visibility = LayoutVisibility.Never;
            }
            if (raGroup.EditValue == null || raGroup.EditValue.ToString() == "")
            {
                lc.Items.FindByName("lciHVTVID").Visibility = LayoutVisibility.Never;
                lc.Items.FindByName("lciMaHVDK").Visibility = LayoutVisibility.Never; 
            }

            //bổ sung cho cải tiến 1 và 2
            if (NgayDK.Properties.ReadOnly == true && HPThucNop.Properties.ReadOnly == false)
                HPThucNop.Focus();

            drMaster = (data.BsMain.Current as DataRowView).Row;
            if (drMaster == null) // them de khi chuyển lớp ko bị lỗi
                return;

            if (drMaster.RowState == DataRowState.Unchanged)
            {
                if (drMaster["NguonHV"].ToString() == "0")
                {
                    lc.Items.FindByName("lciHVTVID").Visibility = LayoutVisibility.Always;
                    lc.Items.FindByName("lciMaHVDK").Visibility = LayoutVisibility.Never;
                }
                else if (drMaster["NguonHV"].ToString() == "1" || drMaster["NguonHV"].ToString() == "2")
                {
                    lc.Items.FindByName("lciHVTVID").Visibility = LayoutVisibility.Always;
                    lc.Items.FindByName("lciMaHVDK").Visibility = LayoutVisibility.Always;
                }
            }     
     
            //Bổ sung chọn nhân viên chăm sóc theo chi nhánh học

            if (drMaster.RowState == DataRowState.Added && gluCNHOC.Properties.ReadOnly == false)
            {
                if (Config.GetValue("MaCN") != null)
                    drMaster["MaCNHoc"] = Config.GetValue("MaCN").ToString();
            }
        }

        void raHTMUA_EditValueChanged(object sender, EventArgs e)
        {
            RadioGroup raHTMUA = sender as RadioGroup;

            if (data.BsMain == null || data.BsMain.Current == null || raHTMUA.Properties.ReadOnly || raHTMUA.EditValue.ToString() == "")
                return;
            drMaster = (data.BsMain.Current as DataRowView).Row;
            if (drMaster == null) // thêm điều kiện drMaster != null để không bị lỗi khi chuyển lớp
                return;
            lc.Items.FindByName("lciSoLuong").Visibility = raHTMUA.EditValue.ToString() == "0" ? LayoutVisibility.Always : LayoutVisibility.Never;
            //Thêm vào cho trường hợp nếu chỉ xem thì ko chạy
            //if (drMaster.RowState == DataRowState.Unchanged)
            //    return;
            drMaster["HTMua"] = raHTMUA.EditValue.ToString();
            
            BindSanPham(gv, drMaster["MaLop"].ToString(), DateTime.Parse(drMaster["NgayDK"].ToString()));
            //nếu chọn mua trọn bộ rồi, mà chọn lại mua lẻ thì set lại số lượng
            if (raHTMUA.EditValue.ToString() == "1")
                drMaster["SoLuong"] = 0;
        }

        void XoaGridView(GridView gv)
        {
            while (gv.DataRowCount > 0)
                gv.DeleteRow(0);
        }

        void BindSanPham(GridView gv, string malop, DateTime NgayDK)
        {                        
            if (gv.DataRowCount > 0)
            {
                XoaGridView(gv);
            }
            string sql = "";
            DataTable dt; 
           
            // Giáo trình
            if (malop != "")
            {                
                sql = " select vn.MaVT,vn.MaNLop, vt.giaban "+
                      " from vtnl vn inner join dmnhomlop nl on nl.MaNLop = vn.MaNLop  "+
                      " inner join DMlophoc L on L.MaNLop=nl.MaNLop  " +
                      " inner join dmvt vt on vt.mavt=vn.mavt "+
                      " where  L.malop ='"+malop+"'";
                dt = db.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        gv.AddNewRow();
                        gv.UpdateCurrentRow();
                        gv.SetFocusedRowCellValue(gv.Columns["MaSP"], row["MaVT"].ToString());
                        if(row["giaban"].ToString() != "")
                            gv.SetFocusedRowCellValue(gv.Columns["Dongia"], row["giaban"].ToString());                        

                        //if (row["tkdt"].ToString()!="")
                        //    gv.SetFocusedRowCellValue(gv.Columns["TKDT"], row["tkdt"].ToString());
                        //if (row["tkgv"].ToString() != "")
                        //    gv.SetFocusedRowCellValue(gv.Columns["TKGV"], row["tkgv"].ToString());
                        //if (row["tkkho"].ToString() != "")
                        //    gv.SetFocusedRowCellValue(gv.Columns["TKKho"], row["tkkho"].ToString());
                    }
                }
            }
            if (drMaster == null)
                return;
            //qùa tặng: nếu là học viên cũ hoặc mới thì được tặng quà và phải đóng đủ tiền 
            if (drMaster["NguonHV"].ToString() == "2")
                return;
            if (decimal.Parse(drMaster["Conlai"].ToString()) > 0) // nộp hết tiền mới có quà tặng
                return;
            if (drMaster["isCL"].ToString() == "1")
                return;
            sql = " select G.MaSP, G.soluong, G.MaCN, 0 as dongia, vt.tkkho, vt.tkdt, vt.tkgv, LH.NgayBDKhoa, HP.Thang "+
                  " from  DMQuatang G inner join DMVT VT on VT.MaVT=G.MaSP "+
                  " inner join DMHocPhi HP on HP.HPID = G.HPID "+
                  " inner join DMNhomLop NL on NL.MaNLop = HP.MaNL "+
                  " inner join DMLopHoc LH on LH.MaNLop = NL.MaNLop "+
                  " where G.NgayHH >= '"+NgayDK.ToString()+"' " +
                  " and LH.MaLop ='"+malop+"'";
            dt = db.GetDataTable(sql);
            if (dt.Rows.Count == 0)
                return;
            //Nếu các lớp khai giảng trong tháng khai báo thì mới được tính (tháng có kiểu datetime)
            if (dt.Rows[0]["Thang"].ToString().Trim() != "")
            {
                DateTime ngayKG = DateTime.Parse(dt.Rows[0]["NgayBDKhoa"].ToString());
                DateTime Thang = DateTime.Parse(dt.Rows[0]["Thang"].ToString());
                if (ngayKG.Month != Thang.Month || ngayKG.Year != Thang.Year)
                    return;
            }
            DataView dvQuaTang = new DataView(dt);
            string macn = "";
            if (malop.Length > 2)
                macn = malop.Substring(0,2);
            if (macn != "")
                dvQuaTang.RowFilter = "MaCN = '" + macn + "' OR MaCN is null";
            else
                dvQuaTang.RowFilter = "MaCN is null";
            //if (dt.Rows.Count > 0)
            //{
            //    foreach (DataRow row in dt.Rows)
            //    {
            //        gv.AddNewRow();
            //        gv.UpdateCurrentRow();
            //        gv.SetFocusedRowCellValue(gv.Columns["MaSP"], row["MaSP"].ToString());
            //        gv.SetFocusedRowCellValue(gv.Columns["Dongia"], row["dongia"].ToString());
            //        gv.SetFocusedRowCellValue(gv.Columns["SL"], row["soluong"].ToString());
            //        gv.SetFocusedRowCellValue(gv.Columns["isQT"], 1);
            //        if (row["tkdt"].ToString() != "")
            //            gv.SetFocusedRowCellValue(gv.Columns["TKDT"], row["tkdt"].ToString());
            //        if (row["tkgv"].ToString() != "")
            //            gv.SetFocusedRowCellValue(gv.Columns["TKGV"], row["tkgv"].ToString());
            //        if (row["tkkho"].ToString() != "")
            //            gv.SetFocusedRowCellValue(gv.Columns["TKKho"], row["tkkho"].ToString());
            //    }
            //}

            if (dvQuaTang.Count>0)
            {
                foreach (DataRowView drv in dvQuaTang)
                {
                    gv.AddNewRow();
                    gv.UpdateCurrentRow();
                    gv.SetFocusedRowCellValue(gv.Columns["MaSP"], drv["MaSP"].ToString());
                    gv.SetFocusedRowCellValue(gv.Columns["Dongia"], drv["dongia"].ToString());
                    gv.SetFocusedRowCellValue(gv.Columns["SL"], drv["soluong"].ToString());
                    gv.SetFocusedRowCellValue(gv.Columns["isQT"], 1);
                    if (drv["tkdt"].ToString() != "")
                        gv.SetFocusedRowCellValue(gv.Columns["TKDT"], drv["tkdt"].ToString());
                    if (drv["tkgv"].ToString() != "")
                        gv.SetFocusedRowCellValue(gv.Columns["TKGV"], drv["tkgv"].ToString());
                    if (drv["tkkho"].ToString() != "")
                        gv.SetFocusedRowCellValue(gv.Columns["TKKho"], drv["tkkho"].ToString());
                }
            }
        }

        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataSet ds = data.BsMain.DataSource as DataSet;
            if (ds == null)
                return;
            if (data.BsMain.Current != null)
                drMaster = (data.BsMain.Current as DataRowView).Row;
            ds.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(HVDK_ColumnChanged);            
        }

        private void TinhNgayHoc(DataRow drData)
        {
            if (drData["NgayDKMoi"].ToString() == "" || drData["MaLop"].ToString() == "") return;
            var ngayDK = Convert.ToDateTime(drData["NgayDKMoi"]);
            var maLop = drData["MaLop"].ToString();

            var sql = string.Format("select top 1 Ngay from TempLichHoc where MaLop = '{0}' and Ngay >= '{1}' order by Ngay", maLop, ngayDK);
            var ngay = db.GetValue(sql);

            if (ngay == DBNull.Value)
                drData["NgayDK"] = DBNull.Value;
            else
                drData["NgayDK"] = Convert.ToDateTime(ngay);
        }

        void HVDK_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                if (e.Row.RowState == DataRowState.Deleted)
                    return;
                drMaster = e.Row;
                Application.CurrentCulture.NumberFormat = nfi;
                if ((e.Row.RowState == DataRowState.Detached || e.Row.RowState == DataRowState.Added)
                    && e.Row["NguonHV"] == DBNull.Value)   //mac dinh cho nguonhv
                {
                    e.Row["NguonHV"] = 0;
                    //e.Row.EndEdit();
                }
                if (e.Column.ColumnName.ToUpper().Equals("MALOP") && e.Row["MaLop"].ToString() != "" && !flag)
                {
                    //tao ma hoc vien
                    string mahv = CreateMaHV(e.Row["MaLop"].ToString());
                    if (mahv != "")
                    {
                        flag = true;
                        e.Row["MaHV"] = mahv;
                        //e.Row.EndEdit();                    
                        //set focus
                        //GiamHP.Focus();
                    }
                }
                else
                    flag = false;

                if (e.Column.ColumnName.ToUpper() == "NGAYDKMOI" || e.Column.ColumnName.ToUpper() == "MALOP") TinhNgayHoc(e.Row);

                //tinh học phí
                if (e.Column.ColumnName.ToUpper().Equals("NGAYDKMOI") || e.Column.ColumnName.ToUpper().Equals("NGAYDK") 
                    || e.Column.ColumnName.ToUpper().Equals("MALOP") || e.Column.ColumnName.ToUpper().Equals("GIAMHP")
                    || e.Column.ColumnName.ToUpper().Equals("NGUONHV") || e.Column.ColumnName.ToUpper().Equals("MAHVDK"))
                {
                    if (e.Row["NgayDK"].ToString() != "" && e.Row["MaLop"].ToString() != "" && e.Row["GiamHP"].ToString() != "" && e.Row["NguonHV"].ToString() != "")
                    {
                        decimal giam = e.Row["GiamHP"].ToString() != "" ? decimal.Parse(e.Row["GiamHP"].ToString(), nfi) : 0;
                        bool flg = e.Row["NguonHV"].ToString() == "2" ? true : false;
                        //tiền bảo lưu + học phí
                        decimal tienBL = 0, hocphi = 0;
                        //ma gio hoc
                        dvLopHoc.RowFilter = " MaLop = '" + e.Row["MaLop"].ToString() + "'";
                        if (dvLopHoc.Count == 0)
                            return;
                        string magio = dvLopHoc[0]["MaGioHoc"].ToString();
                        if (magio != "" && magio.Length > 1)
                            magio = magio.Substring(0, 1);
                        if (flg)// nếu là học viên bảo lưu
                        {
                            if (e.Row["MaHVDK"].ToString() != "") // sau khi chọn mã học sinh mới tính 
                            {
                                string sql = "select BLSoTien from MTDK where mahv ='" + e.Row["MaHVDK"].ToString() + "' and NgayHH >= '" + e.Row["NgayDK"].ToString() + "'";
                                DataTable dt = db.GetDataTable(sql);
                                if (dt.Rows.Count > 0)
                                {
                                    tienBL = decimal.Parse(dt.Rows[0]["BLSoTien"].ToString(), nfi);
                                }
                                hocphi = TinhHocPhi(DateTime.Parse(e.Row["NgayDKMoi"].ToString()), DateTime.Parse(e.Row["NgayDK"].ToString()), e.Row["MaLop"].ToString(), giam, flg, tienBL, magio);
                                e.Row["TienHP"] = hocphi;
                                //Bo sung yêu cầu hiện số tiền bảo lưu do học viên bảo lưu
                                e.Row["BLTruoc"] = tienBL;
                            }
                        }
                        else
                        {
                            hocphi = TinhHocPhi(DateTime.Parse(e.Row["NgayDKMoi"].ToString()), DateTime.Parse(e.Row["NgayDK"].ToString()), e.Row["MaLop"].ToString(), giam, flg, tienBL, magio);
                            e.Row["TienHP"] = hocphi;
                            //Bo sung yêu cầu hiện số tiền bảo lưu do đăng ký học còn dư
                            e.Row["BLTruoc"] = drMaster["BLTruoc"];
                        }
                        if (drMaster != null)
                            if (drMaster["SoBuoiCL"].ToString() != "")
                                e.Row["SoBuoiCL"] = drMaster["SoBuoiCL"].ToString();
                        // Bổ sung ngày 2012-07-02 cho lỗi thay đổi ngày, số tiền thực nộp sẽ cập nhật lại tiền bl
                        decimal TienTN = decimal.Parse(e.Row["ThucThu"].ToString(), nfi);
                        //e.Row.EndEdit();
                    }

                }
                //số lượng giáo trình - Ẩn đi thay thế bằng việc find controls và xử lý nó để không ảnh 

                #region hidden code
                //hưởng đến việc chuyển lớp từ thiết lập quy trình

                //if (e.Column.ColumnName.ToUpper().Equals("SOLUONG"))
                //{
                //    DataSet ds = data.BsMain.DataSource as DataSet;
                //    if (ds == null)
                //        return;
                //    DataTable dtQT = ds.Tables[1];
                //    DataView dvQT = new DataView(dtQT);
                //    dvQT.RowFilter = "HVID = '"+e.Row["HVID"].ToString()+"'";
                //    foreach (DataRowView drv in dvQT)
                //    {
                //        if (drv["isQT"].ToString().ToUpper().Equals("FALSE"))
                //            drv["SL"] = e.Row["SoLuong"].ToString();
                //    }
                //}
                #endregion

                //Cập nhật số tiền bảo lưu nếu Tiền BL > học phí phải nộp
                if (e.Column.ColumnName.ToUpper().Equals("TIENHP") || e.Column.ColumnName.ToUpper().Equals("THUCTHU"))
                {
                    if (e.Row["TienHP"].ToString() == "0" && tienBLCon != 0 && e.Row["NguonHV"].ToString() == "2")
                        e.Row["BLSotien"] = RoundNumber(tienBLCon);
                    //Thêm mới: Bỏ công thức tính của cột số tiền còn nợ: Nợ = HP phải đóng - Học phí thực nộp
                    decimal TienHP = decimal.Parse(e.Row["TienHP"].ToString(), nfi);
                    decimal TienTN = 0;
                    if (e.Row["ThucThu"].ToString() != "")
                        TienTN = decimal.Parse(e.Row["ThucThu"].ToString(), nfi);
                    if (TienTN >= TienHP)
                    {
                        e.Row["ConLai"] = 0;
                        e.Row["BLSoTien"] = RoundNumber(TienTN - TienHP + tienBLCon);
                    }
                    else
                    {
                        e.Row["ConLai"] = RoundNumber(TienHP - TienTN + tienBLCon);
                        e.Row["BLSoTien"] = 0;
                    }
                    //e.Row.EndEdit();
                }
                //Bổ sung tự nhảy nvcs theo nvtv
                if (e.Column.ColumnName.ToUpper().Equals("MACNHOC") || e.Column.ColumnName.ToUpper().Equals("NVCS"))
                {
                    object obj = null;
                    if (e.Row["MaCNHoc"] != DBNull.Value)
                    {
                        obj = db.GetValue("select nvcs from dmbophan where mabp='" + e.Row["MaCNHoc"].ToString() + "'");
                    }
                    if (e.Column.ColumnName.ToUpper().Equals("MACNHOC"))
                    {
                        if (obj != null)
                        {
                            e.Row["NVCS"] = obj.ToString();
                        }
                    }
                    if (e.Column.ColumnName.ToUpper().Equals("NVCS"))
                    {
                        if (obj != null)
                            if (e.Row["NVCS"].ToString() != obj.ToString())
                            {
                                if (!bool.Parse(Config.GetValue("admin").ToString()))
                                {
                                    e.Row["NVCS"] = obj.ToString();
                                    //e.Row.EndEdit();
                                }
                            }
                    }
                }
                e.Row.EndEdit();
                Application.CurrentCulture = ci;
            }
            catch(Exception){}
        }

        string CreateMaHV(string malop)
        {
            string mahv = malop;
            string sql = "select  MaHV from MTDK where MaHV like '"+malop+"%' order by MaHV DESC";
            DataTable dt = db.GetDataTable(sql);
            if (dt.Rows.Count == 0)
                mahv += "01";
            else
            {
                string stt = dt.Rows[0]["MaHV"].ToString();
                stt = stt.Replace(malop, "");
                if (stt == "")
                {
                    XtraMessageBox.Show("Tạo mã học sinh không thành công!", Config.GetValue("PackageName").ToString());
                    return null;
                }
                else
                {
                    int dem = int.Parse(stt)+1;
                    if (dem < 10)
                        mahv += "0" + dem.ToString();
                    else
                        mahv += dem.ToString();
                }
                if (mahv.Length > 16) // do mã lớp tối đa 14 ký tự, và 2 ký tự cuối là số thứ tự của số lượng học viên trong lớp. 
                {
                    XtraMessageBox.Show("Mã hã học viên tạo ra vượt quá 16 ký tự quy định!",Config.GetValue("PackageName").ToString());
                    return null;
                }                
            }
            return mahv;
        }

        decimal TinhHocPhi(DateTime ngayDK, DateTime ngayHoc, string malop, decimal giam, bool isBL,decimal tienBL, string magio)
            {
            string sql = "select NgayBDKhoa, NgayKTKhoa, BDNghi, KTNghi, MaGioHoc from DMLopHoc where MaLop='" + malop + "'";
            DataTable dtLop = db.GetDataTable(sql);
            DataTable dt;
            if (dtLop.Rows.Count == 0 || data.BsMain.Current == null)
                return 0;
            drMaster = (data.BsMain.Current as DataRowView).Row;
            // Thêm mới SHZ : tính theo Atlanta
            DataTable dtbh = db.GetDataTable(string.Format("exec sp_Sobuoidahoc '{0}','{1}' ", ngayHoc, malop));
            decimal sbdahoc = decimal.Parse(dtbh.Rows[0]["sobuoidh"].ToString());
            if (dtLop.Rows[0]["NgayBDKhoa"].ToString() != "" && dtLop.Rows[0]["NgayKTKhoa"].ToString() != "")
            {
                if (ngayDK >= DateTime.Parse(dtLop.Rows[0]["NgayKTKhoa"].ToString()))
                {
                    XtraMessageBox.Show("Lớp này đã kết thúc trước ngày đăng ký học, sẽ không tính học phí cho trường hợp này!",
                        Config.GetValue("PackageName").ToString());
                    return 0;
                }
                sql = " select HPNL.HocPhi, l.sobuoi  " +
                      " from dmlophoc l inner join dmhocphi hp on l.MaNLop=hp.MaNL  " +
                      " inner join HPNL on HPNL.HPID=hp.HPID " +
                      " inner join DMNhomLop NL on NL.MaNLop=hp.MaNL " +
                      " where l.MaLop='" + malop + "' " +
                      " and HPNL.NgayBD <='" + ngayDK.ToString() + "' order by HPNL.NgayBD DESC ";
                dt = db.GetDataTable(sql);
                 
                if (dt.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có học phí nào áp dụng trong khoảng thời gian này!", Config.GetValue("PackageName").ToString());
                    return 0;
                }
                //học phí chuẩn
                decimal hocphi = decimal.Parse(dt.Rows[0]["HocPhi"].ToString(),nfi);

                //tổng số buổi học
                decimal sobuoi = decimal.Parse(dt.Rows[0]["Sobuoi"].ToString(),nfi);

                //% khuyến học
                string sqlKH = " select KH.tyle, KH.NgayBD,KH.NgayKT " +
                               " from DMLopHoc L inner join DMHocPhi HP on L.MaNLop = HP.MaNL " +
                               " inner join dmkhuyenhoc KH  on KH.HPID = HP.HPID " +
                               " where L.MaLop = '" + malop + "' " +
                               " and ( '" + ngayDK.ToString() + "' between KH.NgayBD and KH.NgayKT) ";

                DataTable dtKH = db.GetDataTable(sqlKH);

                //học sau ngày khai giảng
                if (ngayHoc > DateTime.Parse(dtLop.Rows[0]["NgayBDKhoa"].ToString()) && ngayHoc < DateTime.Parse(dtLop.Rows[0]["NgayKTKhoa"].ToString()))
                {
                    //int sobuoitre = SoNgayTre(DateTime.Parse(dtLop.Rows[0]["NgayBDKhoa"].ToString()), DateTime.Parse(dtLop.Rows[0]["NgayKTKhoa"].ToString()), ngayDK, magio, dtLop.Rows[0]["BDNghi"].ToString(), dtLop.Rows[0]["KTNghi"].ToString());
                   // Thay đổi theo bổ sung SHZ
                    /*dtCTGioHoc = db.GetDataTable(string.Format(@"SELECT l.MaGioHoc, Thu, [Value] 
                                            FROM CTGioHoc h INNER JOIN DMNgayGioHoc n ON h.MaGioHoc = n.MaGioHoc
                                                            INNER JOIN DMLopHoc l ON n.MaGioHoc = l.MaGioHoc 
                                            WHERE MaLop = '{0}'", drMaster["MaLop"]));*/
                    //int sobuoitre = SoNgayTre(DateTime.Parse(dtLop.Rows[0]["NgayBDKhoa"].ToString()), DateTime.Parse(dtLop.Rows[0]["NgayKTKhoa"].ToString()), ngayDK, dtCTGioHoc, dtLop.Rows[0]["BDNghi"].ToString(), dtLop.Rows[0]["KTNghi"].ToString());
                    decimal conlai = sobuoi - sbdahoc;
                    hocphi = (hocphi / sobuoi) * conlai;
                    if (drMaster != null)
                        drMaster["SoBuoiCL"] = conlai;
                }
                else
                {
                    if (drMaster != null)
                        drMaster["SoBuoiCL"] = sobuoi;
                }

                //lấy % khuyến học và tính học phí còn lại (chỉ tính khuyến học cho đăng ký đúng ngày)
                if (dtKH.Rows.Count > 0 && ngayHoc <= DateTime.Parse(dtLop.Rows[0]["NgayBDKhoa"].ToString()))
                {
                    decimal kh = decimal.Parse(dtKH.Rows[0]["tyle"].ToString(),nfi);
                    hocphi = hocphi - (hocphi * kh) / 100;
                }
                //lấy mức giảm học phí và tính học phí cần nộp
                if (giam != 0)
                {
                    hocphi = hocphi - (hocphi * giam) / 100;
                }
                //Nếu là học viên bảo lưu
                if (isBL)
                {
                    hocphi = RoundNumber(hocphi);
                    if (tienBL > hocphi)
                    {
                        tienBLCon = tienBL - hocphi;
                        hocphi = 0;
                    }
                    else
                    {
                        hocphi = hocphi - tienBL;
                        tienBLCon = tienBL = 0;                        
                    }

                }
                //Mới thêm cho trường hợp nếu chuyển lớp, hoặc bảo lưu và đăng ký mới mà vẫn dư tiền thì khi đăng ký mới trừ đi tiền bảo lưu
                if (drMaster != null)
                {
                    if (drMaster["MaHVDK"].ToString() != "" && drMaster["NguonHV"].ToString() != "" && drMaster["NguonHV"].ToString() == "1")
                    {
                        sql = "select * from MTDK where MaHV = '" + drMaster["MaHVDK"].ToString() + "'";
                        dt = db.GetDataTable(sql);
                        if (dt.Rows.Count > 0)
                        {
                            // Cộng dồn tiền nợ nếu có
                            decimal tienConNo = decimal.Parse(dt.Rows[0]["ConLai"].ToString(),nfi);
                            hocphi += tienConNo;
                            drMaster["HPNoTruoc"] = tienConNo;
                            //Tiền bảo lưu
                            decimal tienBLCL = decimal.Parse(dt.Rows[0]["BLSoTien"].ToString(),nfi);
                            drMaster["BLTruoc"] = tienBLCL;
                            if (tienBLCL < hocphi)                            
                                hocphi -= tienBLCL;                                                            
                            else // bổ sung mới 2012-03-24. nếu tiền còn lại của chuyển lớp > hp thì bảo lưu số tiền dư. nếu học xong và đăng ký lớp khác nữa mà tiền BL > HP thì tiếp tục bảo lưu tiền còn dư
                            {
                                tienBLCon = tienBLCL - RoundNumber(hocphi);
                                hocphi = 0;
                            }                            
                        }
                    } 
                }
                //Làm tròn đến hàng ngàn.
                hocphi = RoundNumber(hocphi);
                return hocphi;
            }
            else
            {
                XtraMessageBox.Show("Không tìm thấy ngày bắt đầu và kết thúc khóa học!",Config.GetValue("PackageName").ToString());
                return 0;
            }                
        }

        decimal RoundNumber(decimal num)
        {            
            num = num / 1000;
            num = Math.Round(num, 0, MidpointRounding.AwayFromZero);           
            num *= 1000;
            return num;
        }
// SHZ Cũ 
     //   int SoNgayTre(DateTime ngayBD, DateTime ngayKT, DateTime ngayDK, string magio, string ngayBDNghi, string NgayKTNghi)
        //{
        //    int count = 0;
        //    if (ngayBDNghi != "" && NgayKTNghi != "")
        //    {
        //        DateTime ngayBDN = DateTime.Parse(ngayBDNghi);
        //        DateTime ngayKTN = DateTime.Parse(NgayKTNghi);
        //        for (DateTime dtp = ngayBD; dtp < ngayDK; dtp = dtp.AddDays(1))
        //        {
        //            if (magio == "L") // 2,4,6
        //            {
        //                if (dtp < ngayBDN || dtp > ngayKTN) //nếu trong ngày nghỉ thì không tính
        //                {
        //                    if (dtp.DayOfWeek == DayOfWeek.Monday || dtp.DayOfWeek == DayOfWeek.Wednesday || dtp.DayOfWeek == DayOfWeek.Friday)
        //                        count++;
        //                }
        //            }
        //            else if (magio == "C") //3,5,7
        //            {
        //                if (dtp < ngayBDN || dtp > ngayKTN)
        //                {
        //                    if (dtp.DayOfWeek == DayOfWeek.Tuesday || dtp.DayOfWeek == DayOfWeek.Thursday || dtp.DayOfWeek == DayOfWeek.Saturday)
        //                        count++;
        //                }
        //            }
        //            else if (magio == "B")
        //            {
        //                if (dtp < ngayBDN || dtp > ngayKTN)
        //                {
        //                    if (dtp.DayOfWeek == DayOfWeek.Saturday || dtp.DayOfWeek == DayOfWeek.Sunday)
        //                        count++;
        //                }
        //            }
        //            else
        //            {
        //                if (dtp < ngayBDN || dtp > ngayKTN)
        //                {
        //                    if (dtp.DayOfWeek == DayOfWeek.Sunday)
        //                        count++;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (DateTime dtp = ngayBD; dtp < ngayDK; dtp = dtp.AddDays(1))
        //        {
        //            if (magio == "L") // 2,4,6
        //            {
        //                if (dtp.DayOfWeek == DayOfWeek.Monday || dtp.DayOfWeek == DayOfWeek.Wednesday || dtp.DayOfWeek == DayOfWeek.Friday)
        //                    count++;
        //            }
        //            else if (magio == "C") //3,5,7
        //            {
        //                if (dtp.DayOfWeek == DayOfWeek.Tuesday || dtp.DayOfWeek == DayOfWeek.Thursday || dtp.DayOfWeek == DayOfWeek.Saturday)
        //                    count++;
        //            }
        //            else if (magio == "B")
        //            {
        //                if (dtp.DayOfWeek == DayOfWeek.Saturday || dtp.DayOfWeek == DayOfWeek.Sunday)
        //                    count++;
        //            }
        //            else
        //            {
        //                if (dtp.DayOfWeek == DayOfWeek.Sunday)
        //                    count++;
        //            }
        //        }
        //    }
        //    return count;
        //}
        int SoNgayTre(DateTime ngayBD, DateTime ngayKT, DateTime ngayDK, DataTable DTNgayHoc, string ngayBDNghi, string NgayKTNghi)
        {
            int count = 0;
            if (ngayBDNghi != "" && NgayKTNghi != "")
            {
                DateTime ngayBDN = DateTime.Parse(ngayBDNghi);
                DateTime ngayKTN = DateTime.Parse(NgayKTNghi);
                for (DateTime dtp = ngayBD; dtp < ngayDK; dtp = dtp.AddDays(1))
                {
                    foreach (DataRow row in DTNgayHoc.Rows)
                    {
                        if ((dtp < ngayBDN || dtp > ngayKTN) && row["Value"].ToString() != "") //nếu trong ngày nghỉ thì không tính
                        {
                            if (dtp.DayOfWeek == GetThu(int.Parse(row["Value"].ToString())))
                                count++;
                        }
                    }
                }
            }
            else
            {
                for (DateTime dtp = ngayBD; dtp < ngayDK; dtp = dtp.AddDays(1))
                {
                    foreach (DataRow row in DTNgayHoc.Rows)
                    {
                        if (row["Value"].ToString() != "")
                        {
                            if (dtp.DayOfWeek == GetThu(int.Parse(row["Value"].ToString())))
                                count++;
                        }
                    }
                }
            }
            return count;
        }

        DayOfWeek GetThu(int i)
        {
            switch (i)
            {
                case 1:
                    return DayOfWeek.Sunday;
                case 2:
                    return DayOfWeek.Monday;
                case 3:
                    return DayOfWeek.Tuesday;
                case 4:
                    return DayOfWeek.Wednesday;
                case 5:
                    return DayOfWeek.Thursday;
                case 6:
                    return DayOfWeek.Friday;
                default:
                    return DayOfWeek.Saturday;
            }
        }


        void raGroup_EditValueChanged(object sender, EventArgs e)
        {
            RadioGroup raGroup = sender as RadioGroup;
            if (raGroup.EditValue.ToString() == "")
                return;
            if (raGroup.EditValue.ToString() == "0")
            {                
                lc.Items.FindByName("lciHVTVID").Visibility = LayoutVisibility.Always;
                lc.Items.FindByName("lciMaHVDK").Visibility = LayoutVisibility.Never;
                //GridView gv = gLKHVTV.Properties.View as GridView;
                //gv.ActiveFilterString = "";
            }
            else if (raGroup.EditValue.ToString() == "1")
            {
                lc.Items.FindByName("lciHVTVID").Visibility = LayoutVisibility.Always;
                lc.Items.FindByName("lciMaHVDK").Visibility = LayoutVisibility.Always;
                //so buoi con lai                
                //GridView gv = gLKHVC.Properties.View as GridView;
                //gv.ActiveFilterString = "BLSoTien <> 0";
            }
            else if (raGroup.EditValue.ToString() == "2")
            {

                lc.Items.FindByName("lciHVTVID").Visibility = LayoutVisibility.Always;
                lc.Items.FindByName("lciMaHVDK").Visibility = LayoutVisibility.Always;

                //so buoi con lai                
                //GridView gv = gLKHVC.Properties.View as GridView;
                //gv.ActiveFilterString = "BLSoTien <> 0";
            }

            //Thêm mới để debug 
            //GridView gvHVTV = gridLKHVTV.Properties.View as GridView;
            //gvHVTV.ActiveFilterString = "isMoi = 1 and MaCN = '" + Config.GetValue("MaCN").ToString() + "'";

        }

        public DataCustomFormControl Data
        {
            set { data = value; }
        }

        public InfoCustomControl Info
        {
            get { return info; }
        }

        #endregion
    }
}
