using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using CDTLib;
using Plugins;
using CDTDatabase;
using System.Data;

namespace QuanLyThi
{
    public class QuanLyThi:ICControl
    {
        private InfoCustomControl info = new InfoCustomControl(IDataType.Single);
        private DataCustomFormControl data;
        Database db = Database.NewDataDatabase();
        GridView gv;
        int count = 0;
        bool ThiNghe = false;
        bool ThiNoi = false;
        bool ThiViet = false;

        #region ICControl Members

        public void AddEvent()
        {
            data.FrmMain.Shown += new EventHandler(FrmMain_Shown);
            gv = (data.FrmMain.Controls.Find("gcMain",true)[0] as GridControl).MainView as GridView;
        }        

        void FrmMain_Shown(object sender, EventArgs e)
        {
            //Ghi chú: K: không thi, V: vắng
            fromShow frm = new fromShow();
            frm.Text = "Chọn lớp để quản lý";
            frm.ShowDialog();

            gv.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
            gv.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(gv_CellValueChanged);

            if (frm.MaLop == "")
                return;
            //so môn thi
            if (frm.isNghe)
            {
                ThiNghe = frm.isNghe;
                count++;
            }
            if (frm.isNoi)
            {
                ThiNoi = frm.isNoi; 
                count++;
            }
            if (frm.isViet)
            {
                ThiViet = frm.isViet;
                count++;
            }            
            gv.ActiveFilterString = " MaLop = '" + frm.MaLop + "'";
            if (gv.DataRowCount == 0 )
            {
                if (frm.dtHocVien == null)
                    return;
                foreach (DataRow row in frm.dtHocVien.Rows)
                {
                    gv.AddNewRow();                    
                    gv.SetFocusedRowCellValue(gv.Columns["MaLop"],frm.MaLop);
                    gv.SetFocusedRowCellValue(gv.Columns["HVID"], row["HVID"].ToString());
                    gv.SetFocusedRowCellValue(gv.Columns["TenHV"], row["TenHV"].ToString());
                    gv.UpdateCurrentRow();
                    gv.BestFitColumns();                                      
                } 
            }
            if (frm.isNghe == false)            
                gv.Columns["Nghe"].Visible = false;                            
            if (frm.isNoi == false)            
                gv.Columns["Noi"].Visible = false;             
            if (frm.isViet == false)            
                gv.Columns["Viet"].Visible = false;             
            gv.BestFitColumns();
        }

        void gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Value == null)
                return;
            if (e.Column.FieldName.ToUpper().Equals("NGHE") && e.Value != null)
            {
                string ketqua = DiemTrungBinh(e.Value.ToString(), gv.GetFocusedRowCellValue("Noi").ToString(), gv.GetFocusedRowCellValue("Viet").ToString());
                if (ketqua != "")
                {
                    string[] arr = ketqua.Split(new char[] { '_' });
                    if (arr.Length == 1)
                        return;
                    gv.SetFocusedRowCellValue(gv.Columns["DiemTB"], decimal.Parse(arr[0]));
                    gv.SetFocusedRowCellValue(gv.Columns["XL"], arr[1]);
                    gv.SetFocusedRowCellValue(gv.Columns["GhiChu"], "");
                }
                else
                {
                    gv.SetFocusedRowCellValue(gv.Columns["DiemTB"], null);
                    gv.SetFocusedRowCellValue(gv.Columns["XL"], null);
                    gv.SetFocusedRowCellValue(gv.Columns["GhiChu"], "Thi bổ sung");
                }
            }
            else if (e.Column.FieldName.ToUpper().Equals("NOI") && e.Value != null)
            {
                string ketqua = DiemTrungBinh(gv.GetFocusedRowCellValue("Nghe").ToString(), e.Value.ToString(), gv.GetFocusedRowCellValue("Viet").ToString());
                if (ketqua != "")
                {
                    string[] arr = ketqua.Split(new char[] { '_' });
                    if (arr.Length == 1)
                        return;
                    gv.SetFocusedRowCellValue(gv.Columns["DiemTB"], decimal.Parse(arr[0]));
                    gv.SetFocusedRowCellValue(gv.Columns["XL"], arr[1]);
                    gv.SetFocusedRowCellValue(gv.Columns["GhiChu"], "");
                }
                else
                {
                    gv.SetFocusedRowCellValue(gv.Columns["DiemTB"], null);
                    gv.SetFocusedRowCellValue(gv.Columns["XL"], null);
                    gv.SetFocusedRowCellValue(gv.Columns["GhiChu"], "Thi bổ sung");
                }
            }
            else if (e.Column.FieldName.ToUpper().Equals("VIET") && e.Value != null)
            {
                string ketqua = DiemTrungBinh(gv.GetFocusedRowCellValue("Nghe").ToString(), gv.GetFocusedRowCellValue("Noi").ToString(), e.Value.ToString());
                if (ketqua != "")
                {
                    string[] arr = ketqua.Split(new char[] { '_' });
                    if (arr.Length == 1)
                        return;
                    gv.SetFocusedRowCellValue(gv.Columns["DiemTB"], decimal.Parse(arr[0]));
                    gv.SetFocusedRowCellValue(gv.Columns["XL"], arr[1]);
                    gv.SetFocusedRowCellValue(gv.Columns["GhiChu"], "");
                }
                else
                {
                    gv.SetFocusedRowCellValue(gv.Columns["DiemTB"], null);
                    gv.SetFocusedRowCellValue(gv.Columns["XL"], null);
                    gv.SetFocusedRowCellValue(gv.Columns["GhiChu"], "Thi bổ sung");
                }
            }
            gv.BestFitColumns();
        }

        string DiemTrungBinh(string nghe, string noi, string viet)
        {
            decimal dnghe = 0, dnoi = 0, dviet = 0;
            if (count == 2)
            {
                #region Thi nghe va noi
                if (!ThiViet)
                {
                    if (nghe == "" || noi == "")
                        return "";

                    //if (isNumber(nghe) && isNumber(noi))
                    //{
                    string ketqua = "";
                    dnghe = decimal.Parse(nghe); dnoi = decimal.Parse(noi);
                    decimal dtb = (dnghe + dnoi) / 2;
                    dtb = Math.Round(dtb, 1);
                    ketqua = dtb.ToString().Length == 1 ? dtb.ToString() + ".0" : dtb.ToString();
                    if (dnghe < 4 || dnoi < 4)
                        return "";
                    else if (dtb < 5)
                        return ketqua + "_Thi lại";
                    else if (dtb >= 5 && dtb < 7)
                    {
                        if (dnghe >= 4 && dnoi >= 4)
                            return ketqua + "_TB";
                        else
                            return ketqua + "_Thi lại";
                    }
                    else if (dtb >= 7 && dtb < 9)
                    {
                        if (dnghe >= 6 && dnoi >= 6)
                            return ketqua + "_Khá";
                        else if (dnghe >= 4 && dnoi >= 4)
                            return ketqua + "_TB";
                        else
                            return ketqua + "_Thi lại";
                    }
                    else if (dtb >= 9)
                    {
                        if (dnghe >= 8 && dnoi >= 8)
                            return ketqua + "_Giỏi";
                        else if (dnghe >= 6 && dnoi >= 6)
                            return ketqua + "_Khá";
                        else if (dnghe >= 4 && dnoi >= 4)
                            return ketqua + "_TB";
                        else
                            return ketqua + "_Thi lại";
                    }
                    else
                        return "";
                    //}
                    //else
                    //    return "";
                }
                #endregion ///

                #region Thi nói và viết
                else if (!ThiNghe)
                {
                    if (noi == "" || viet == "")
                        return "";
                    //if (isNumber(viet) && isNumber(noi))
                    //{
                    dviet = decimal.Parse(viet); dnoi = decimal.Parse(noi);
                    decimal dtb = (dviet + dnoi) / 2;
                    dtb = Math.Round(dtb, 1);
                    string ketqua = "";
                    ketqua = dtb.ToString().Length == 1 ? dtb.ToString() + ".0" : dtb.ToString();
                    if (dviet < 4 || dnoi < 4)
                        return "";
                    else if (dtb < 5)
                        return ketqua + "_Thi lại";
                    else if (dtb >= 5 && dtb < 7)
                    {
                        if (dviet >= 4 && dnoi >= 4)
                            return ketqua + "_TB";
                        else
                            return ketqua + "_Thi lại";
                    }
                    else if (dtb >= 7 && dtb < 9)
                    {
                        if (dviet >= 6 && dnoi >= 6)
                            return ketqua + "_Khá";
                        else if (dviet >= 4 && dnoi >= 4)
                            return ketqua + "_TB";
                        else
                            return ketqua + "_Thi lại";
                    }
                    else if (dtb >= 9)
                    {
                        if (dviet >= 8 && dnoi >= 8)
                            return ketqua + "_Giỏi";
                        else if (dviet >= 6 && dnoi >= 6)
                            return ketqua + "_Khá";
                        else if (dviet >= 4 && dnoi >= 4)
                            return ketqua + "_TB";
                        else
                            return ketqua + "_Thi lại";
                    }
                    else
                        return "";
                    //}
                    //else
                    //    return "";
                }
                #endregion ///

                #region Thi nghe và viết
                else if (!ThiNoi)
                {
                    if (nghe == "" || viet == "")
                        return "";
                    //if (isNumber(viet) && isNumber(nghe))
                    //{
                    dviet = decimal.Parse(viet); dnghe = decimal.Parse(nghe);
                    decimal dtb = (dviet + dnghe) / 2;
                    dtb = Math.Round(dtb, 1);
                    string ketqua = "";
                    ketqua = dtb.ToString().Length == 1 ? dtb.ToString() + ".0" : dtb.ToString();
                    if (dviet < 4 || dnghe < 4)
                        return "";
                    else if (dtb < 5)
                        return ketqua + "_Thi lại";
                    else if (dtb >= 5 && dtb < 7)
                    {
                        if (dviet >= 4 && dnghe >= 4)
                            return ketqua + "_TB";
                        else
                            return ketqua + "_Thi lại";
                    }
                    else if (dtb >= 7 && dtb < 9)
                    {
                        if (dviet >= 6 && dnghe >= 6)
                            return ketqua + "_Khá";
                        else if (dviet >= 4 && dnghe >= 4)
                            return ketqua + "_TB";
                        else
                            return ketqua + "_Thi lại";
                    }
                    else if (dtb >= 9)
                    {
                        if (dviet >= 8 && dnghe >= 8)
                            return ketqua + "_Giỏi";
                        else if (dviet >= 6 && dnghe >= 6)
                            return ketqua + "_Khá";
                        else if (dviet >= 4 && dnghe >= 4)
                            return ketqua + "_TB";
                        else
                            return ketqua + "_Thi lại";
                    }
                    else
                        return "";
                    //}
                    //else
                    //    return "";
                }
                else
                    return "";
                #endregion //
            }
            else
            {
                if (nghe == "" || noi == "" || viet == "")
                    return "";
                //if (isNumber(nghe) && isNumber(noi) && isNumber(viet))
                //{
                dnghe = decimal.Parse(nghe); dnoi = decimal.Parse(noi); dviet = decimal.Parse(viet);
                decimal dtb = (dnghe + dnoi + dviet) / 3;
                dtb = Math.Round(dtb, 1);
                string ketqua = "";
                ketqua = dtb.ToString().Length == 1 ? dtb.ToString() + ".0" : dtb.ToString();
                if (dnghe < 4 || dnoi < 4 || dviet < 4)
                    return "";
                else if (dtb < 5)
                    return ketqua + "_Thi lại";
                else if (dtb >= 5 && dtb < 7)
                {
                    if (dnghe >= 4 && dnoi >= 4 && dviet >= 4)
                        return ketqua + "_TB";
                    else
                        return ketqua + "_Thi lại";
                }
                else if (dtb >= 7 && dtb < 9)
                {
                    if (dnghe >= 6 && dnoi >= 6 && dviet >= 6)
                        return ketqua + "_Khá";
                    else if (dnghe >= 4 && dnoi >= 4 && dviet >= 4)
                        return ketqua + "_TB";
                    else
                        return ketqua + "_Thi lại";
                }
                else if (dtb >= 9)
                {
                    if (dnghe >= 8 && dnoi >= 8 && dviet >= 8)
                        return ketqua + "_Giỏi";
                    else if (dnghe >= 6 && dnoi >= 6 && dviet >= 6)
                        return ketqua + "_Khá";
                    else if (dnghe >= 4 && dnoi >= 4 && dviet >= 4)
                        return ketqua + "_TB";
                    else
                        return ketqua + "_Thi lại";
                }
                else
                    return "";
                //}
                //else
                //    return "";
            }
        }


        bool isNumber(string c)
        {
            for (int i = 0; i < c.Length;i++ )
                if (!char.IsDigit(c[i]) && c[i]!=',' && c[i]!='.' )                                   
                    return false;
            return true;
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
