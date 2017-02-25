using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTDatabase;

namespace Inventory
{
    public class PostBl
    {
        private string _timeCon;
        private string _condition = "";
        private Database _dataDb;
        private Database strucDb = Database.NewStructDatabase();
        private string _mtTableID;
        public PostBl(Database dbData, string systableid, string timeCon, string condition)
        {
            _timeCon = timeCon;
            _condition = condition;
            _dataDb = dbData;
            _mtTableID = systableid;
        }
        public void Post()
        {
            DataTable tbNhomdk = GetDtConfig();
            DataTable dtNhomdk;
            string sqlUpdate;
            foreach (DataRow drNhomdk in tbNhomdk.Rows)
            {
                dtNhomdk = GetDtConfigDetail(drNhomdk["blConfigID"].ToString());

                {
                    sqlUpdate = GenUpdateString(dtNhomdk, drNhomdk);
                    _dataDb.UpdateByNonQuery(sqlUpdate);
                }


            }
        }

        private string GenUpdateString(DataTable dt, DataRow drNhomdk)
        {
            //mtdt: 0: update bang mt, 1: update bang dt, 2: update bang cong thuc(mt, dt)
            string s1 = drNhomdk["mtTableName"].ToString().Trim();
            string s2 = drNhomdk["dtTableName"].ToString().Trim();
            string bl = drNhomdk["blTableName"].ToString().Trim();
            string sql = " update " + bl + " set ";
            string giatri = "";

            foreach (DataRow dr in dt.Rows)
            {
                giatri = "";
                if (!(dr["mtFieldID"] is DBNull))
                {
                    giatri = s1 + "." + dr["mtFieldName"].ToString();
                }
                else if (!(dr["dtFieldID"] is DBNull))
                {
                    giatri = s2 + "." + dr["dtFieldname"].ToString();
                }
                else if (!(dr["Formula"] is DBNull) && dr["Formula"].ToString() != "")
                {
                    giatri = dr["Formula"].ToString();
                }
                if (giatri != "")
                    sql += dr["blFieldName"].ToString() + " = " + giatri + " ,";
            }
            sql = sql.Substring(0, sql.Length - 1);

            if (s2 == "")
            {
                sql += " from " + bl + "," + s1 + " where ";
                sql += bl + "." + drNhomdk["RootIDName"].ToString().Trim() + " = ";
                sql += s1 + "." + s1 + "ID ";
            }
            else
            {
                sql += " from " + bl + "," + s1 + "," + s2 + " where ";
                sql += bl + "." + drNhomdk["RootIDName"].ToString().Trim() + " = ";
                sql += s1 + "." + s1 + "ID and ";
                sql += bl + "." + drNhomdk["DTID"].ToString().Trim() + " = ";
                sql += s2 + "." + s2 + "ID ";
                if (_condition != "")
                    sql += " and " + _condition.Replace("MaVT in", s2 + ".MaVT in");
            }
            sql += " and " + _timeCon;
            if (!(drNhomdk["nhomdk"] is DBNull))
            {
                sql += " and " + bl + ".nhomdk" + " = '" + drNhomdk["Nhomdk"].ToString() + "'";
            }
            if (!(drNhomdk["Condition"] is DBNull))
            {
                sql += " and " + drNhomdk["Condition"].ToString();
            }

            return sql;

        }
        private DataTable GetDtConfig()
        {
            string s = "select bl.*, tb1.TableName as blTableName, tb2.TableName as mtTableName, tb3.TableName as dtTableName " +
                " from sysDataConfig bl, sysTable tb1, sysTable tb2, sysTable tb3 " +
                " where bl.mtTableID = " + _mtTableID + " and bl.sysTableID *= tb1.sysTableID and bl.mtTableID *= tb2.sysTableID and bl.dtTableID *= tb3.sysTableID";
            Database db = Database.NewStructDatabase();
            return (db.GetDataTable(s));
        }
        private DataTable GetDtConfigDetail(string blConfigID)
        {
            string s = "select bld.*, sf1.FieldName as blFieldName, sf2.FieldName as mtFieldName, sf3.FieldName as dtFieldName " +
                " from sysDataConfigDt bld, sysField sf1, sysField sf2, sysField sf3 " +
                " where bld.blConfigID = " + blConfigID + " and bld.blFieldID *= sf1.sysFieldID and bld.mtFieldID *= sf2.sysFieldID and bld.dtFieldID *= sf3.sysFieldID";
            Database db = Database.NewStructDatabase();
            return (db.GetDataTable(s));
        }
    }
}
