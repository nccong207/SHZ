using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using CDTDatabase;
using Formula;

namespace Inventory
{
    class UpdateFormula
    {
        private string _condition = "";
        public string mt;
        public string dt;
        private string mtName;
        private string dtName;
        private string giaField;
        string ngayctField;
        private DateTime Tungay;
        private DateTime Denngay;
        DataTable Struct;
        List<string> CalculatedField=new List<string>();
        List<string> lstSubFormuar=new List<string>();
        List<string> lstham = new List<string>();
        private Database _dbData  =Database.NewDataDatabase();
        private Database _dbStruct = Database.NewStructDatabase();
        private const int _MROUND = 6;

        public UpdateFormula(Database dbData, string MT, string DT,string GiaField,DateTime tungay,DateTime denngay,string ngayCtField, string condition)
        {
            _condition = condition;
            _dbData = dbData;
            mt = MT;
            dt = DT;
            Tungay=tungay;
            Denngay = denngay;
            mtName = _dbStruct.GetValue("select tableName from systable where systableid=" + mt).ToString();
            dtName = _dbStruct.GetValue("select tableName from systable where systableid=" + dt).ToString();
            ngayctField = ngayCtField;
            giaField = GiaField;
            Struct = GetStruct();
        }
        private DataTable GetStruct()
        {
            string sql = "select systableID,fieldName, Formula from sysfield where systableID=" + mt + " or systableID= " + dt;
            return _dbStruct.GetDataTable(sql);

        }

        public void Update()
        {
            updateMTDTCongthuc(giaField.ToUpper());
        }

        private void updateMTDTCongthuc(string ChangedField)
        {
            DataRow[] lstField=Struct.Select("Formula like '*" + ChangedField + "*'");
            List<string> lstBien;            
            string varField;
            string sql;
            string formularSql;
            foreach (DataRow drv in lstField)//duyệt qua các trường có biến là trường đang thay đổi
            {
                string[] strFormula = drv["Formula"].ToString().Split(',');
                formularSql = strFormula[0].ToUpper();
                int round = strFormula.Length == 1 ? _MROUND : Int32.Parse(strFormula[1]);
                lstBien = getLstBien(drv["Formula"].ToString());
                if ((lstBien.Contains(ChangedField.ToUpper())))
                {
                    foreach (string fVar in lstBien)
                    {
                        varField = Struct.Select("fieldName='" + fVar + "'")[0]["systableID"].ToString();
                        if (varField == mt)
                        {
                            varField = "b." + fVar.Trim();//Nếu biến là trường thuộc bảng mt thì b, mt alias b                        
                        }
                        else
                        {
                            varField = "a." + fVar.Trim();//Nếu biến là trường thuộc bảng dt thì b, dt alias b
                        }
                        formularSql = formularSql.Replace("@" + fVar.Trim(), varField);
                    }

                    varField = Struct.Select("fieldName='" + drv["fieldName"].ToString() + "'")[0]["systableID"].ToString();

                    //Tạo câu sql update
                    string whereSql;

                    if (varField == dt)//Nếu trường cần tính là trường trong bảng dt
                    {
                        whereSql = " from " + dtName + " a," + mtName + " b  where a." + mtName.Trim() + "id = b." + mtName.Trim() + "id and ";
                        whereSql += "b." + ngayctField + " between cast('" + Tungay.ToString() + "' as datetime) and cast('" + Denngay.ToString() + "' as datetime)";
                        sql = "update " + dtName + " set ";
                        sql += drv["fieldName"].ToString() + " = round(" + formularSql + "," + round.ToString() + ") ";
                        sql += whereSql;
                        if (_condition != "")
                            sql += " and (" + _condition + ")";
                    }
                    else
                    {
                        formularSql = "round(" + formularSql + "," + round.ToString() + ")";
                        formularSql = formatFormular(formularSql);
                        sql = "update " + mtName + " set ";
                        sql += drv["fieldName"].ToString() + " = " + formularSql;
                        sql += " ( " + ngayctField + " between cast('" + Tungay.ToString();
                        sql += "' as datetime) and cast('" + Denngay.ToString() + "' as datetime))";
                        
                    }
                    //if (lstBien.Contains("TYGIA"))  //xu ly rieng truong hop tien VND thi khong tinh cac cong thuc lien quan den tien ngoai te
                    //    sql += " and b.MaNT <> 'VND'";
                    if (mtName == "MT33" || mtName == "MT42")
                    {
                        sql += " and b.NhapTb=1";
                    }
                    _dbData.UpdateByNonQuery(sql);                    
                }
            }
            foreach (DataRow drv in lstField)//duyệt qua các trường có biến là trường đang thay đổi
            {
                formularSql = drv["Formula"].ToString().ToUpper();
                lstBien = getLstBien(drv["Formula"].ToString());
                if ((lstBien.Contains(ChangedField.ToUpper())))
                { 
                    updateMTDTCongthuc(drv["fieldName"].ToString().ToUpper());
                }
            }
        }
        
        private string formatFormular(string FormularSql)
        {
            lstSubFormuar.Clear();
            lstham.Clear();            
            getSubFormular(FormularSql);
            if (lstSubFormuar.Count==0)
            {
                return FormularSql + " from " + mtName + " b where ";
            }
            for (int i=0; i < lstSubFormuar.Count; i++)
            {
                FormularSql = FormularSql.Replace(lstham[i] + "(" + lstSubFormuar[i] + ")", "x" + i.ToString() + ".x");
            }
            FormularSql += " from " + mtName + " b ";
            for (int i = 0; i < lstSubFormuar.Count; i++)
            {
                FormularSql += ",( select " + lstham[i] + "(" + lstSubFormuar[i] + ") as x," + mtName.Trim() + "id";
                FormularSql += " from " + dtName + " a group by " + mtName.Trim() + "id) x" + i.ToString();
            }
            FormularSql += " where ";
            for (int i = 0; i < lstSubFormuar.Count; i++)
            {
                FormularSql += " b." + mtName.Trim() + "id = x" + i.ToString() + "." + mtName.Trim() + "id and ";
            }
            return FormularSql;
        }
        
        private void getSubFormular(string FormularSql)
        {
            int i;
            int j;
            int daumo = 0;
            string ham;
            string subFormular;
            i = FormularSql.IndexOf("SUM(");
            ham = "SUM";
            if (i < 0)
            {
                i = FormularSql.IndexOf("ABS(");
                ham = "ABS";
            }
            if (i >= 0)
            {
                for (j = i + 1; j < FormularSql.Length; j++)
                {
                    if (FormularSql.Substring(j, 1) == "(")
                    {
                        daumo = daumo + 1;
                    }
                    else if (FormularSql.Substring(j, 1) == ")")
                    {
                        daumo = daumo - 1;
                        if (daumo == 0)
                        {
                            subFormular = FormularSql.Substring(i + 4, j - i-4);
                            FormularSql=FormularSql.Replace(ham + "(" + subFormular + ")", "");
                            lstSubFormuar.Add(subFormular);
                            lstham.Add(ham);
                            getSubFormular(FormularSql);
                            break;
                        }
                    }
                }
            }
        }
        private List<string> getLstBien(string Formular)
        {
            BieuThuc a = new Formula.BieuThuc(Formular);
            return a.variables;
        }
    }
}
