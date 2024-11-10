using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Security.Authentication;
using CDTDatabase;
using Newtonsoft.Json.Linq;

namespace PostToCRM
{
    public class PostToCRM : ICData
    {
        public const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
        public const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;

        private InfoCustomData _info;
        private DataCustomData _data;
        public DataCustomData Data
        {
            set { _data = value; }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        public PostToCRM()
        {
            ServicePointManager.SecurityProtocol = Tls12;
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }

        public void ExecuteAfter()
        {
            if (_data.CurMasterIndex < 0)
                return;
            if (_data.DsData == null)
                return;
            DataRow drMaster = _data.DsDataCopy.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster == null)
                return;
            _data.DbData.EndMultiTrans();
            if (drMaster.RowState == DataRowState.Added || drMaster.RowState == DataRowState.Modified)
            {
                var data = ProductData(drMaster);

                var productId = GetProductId(drMaster["MaLop"].ToString());
                var response = Post(data, productId);
                
                UpdateProductId(response);
            }
        }

        public void ExecuteBefore()
        {
            if (_data.CurMasterIndex < 0)
                return;
            if (_data.DsData == null)
                return;
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster.RowState == DataRowState.Deleted)
            {
                var productId = GetProductId(drMaster["MaLop", DataRowVersion.Original].ToString());
                if (productId != null && productId != DBNull.Value)
                    Delete(productId);
            }
        }

        private object GetProductId(string maLop)
        {
            string sql = string.Format("select ProductId from DMLopHoc where MaLop = '{0}'", maLop);
            return _data.DbData.GetValue(sql);
        }

        private void UpdateProductId(JObject response)
        {
            string sql = string.Format("update DMLopHoc set ProductId = '{0}' where MaLop = '{1}'", response["id"], response["short_name"]);
            _data.DbData.UpdateByNonQuery(sql);
        }

        private object GetPrice(object maLop)
        {
            string sql = @"select top 1 HPNL.HocPhi
                          from dmlophoc l 
                          inner join dmhocphi hp on l.MaNLop = hp.MaNL
                          inner join HPNL on HPNL.HPID = hp.HPID
                          where l.MaLop='{0}' and HPNL.NgayBD <= l.NgayBDKhoa order by HPNL.NgayBD DESC";

            return _data.DbData.GetValue(string.Format(sql, maLop));
        }

        private string ProductData(DataRow drData)
        {
            var data = new
            {
                short_name = drData["MaLop"],
                name = string.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy} ({2})", drData["NgayBDKhoa"], drData["NgayKTKhoa"], drData["TenLop"]),
                price = GetPrice(drData["MaLop"]),
                custom_attributes = new
                {
                    ma_gio_hoc = drData["MaGioHoc"],
                    ngay_bat_dau = drData["NgayBDKhoa"],
                    ngay_ket_thuc = drData["NgayKTKhoa"],
                    so_buoi_hoc = drData["SoBuoi"],
                    branch = drData["MaCN"],
                    class_group = drData["MaNLop"]
                }
            };
            return JsonConvert.SerializeObject(data);
        }

        private JObject Post(string jsonData, object productId)
        {
            string endPoint = "https://app.hoatieucrm.vn/api/v1/accounts/5/products";
            var isUpdate = (productId != null && productId != DBNull.Value);
            if (isUpdate)   endPoint += "/" + productId.ToString();

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(endPoint);
            request.Method = isUpdate ? "PATCH" : "POST";
            request.ContentType = "application/json";
            request.Headers.Add("api_access_token", "hw1GJ89C8hZP1M1zJ26qjWAn");

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(jsonData);
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseBody = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<JObject>(responseBody);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void Delete(object productId)
        {
            string endPoint = "https://app.hoatieucrm.vn/api/v1/accounts/5/products/" + productId.ToString();
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(endPoint);
            request.Method = "DELETE";
            request.Headers.Add("api_access_token", "hw1GJ89C8hZP1M1zJ26qjWAn");

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
            }
        }
    }
}
