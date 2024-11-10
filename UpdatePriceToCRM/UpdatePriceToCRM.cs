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

namespace UpdatePriceToCRM
{
    public class UpdatePriceToCRM : ICData
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

        public UpdatePriceToCRM()
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
                var contactId = GetContactId(drMaster["HVTVID"].ToString());
                if (contactId == null || contactId.ToString() == string.Empty) return;

                var response = Post(contactId, drMaster["TienHP"]);
            }
        }

        public void ExecuteBefore()
        {
        }

        private object GetContactId(string hvtvID)
        {
            string sql = string.Format("select ContactId from DMHVTV where HVTVID = {0}", hvtvID);
            return _data.DbData.GetValue(sql);
        }

        private JObject Post(object contactId, object price)
        {
            string endPoint = "https://app.hoatieucrm.vn/api/v1/accounts/5/contacts/" + contactId.ToString();

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(endPoint);
            request.Method = "PATCH";
            request.ContentType = "application/json";
            request.Headers.Add("api_access_token", "hw1GJ89C8hZP1M1zJ26qjWAn");

            var data = new
            {
                id = contactId,
                po_value = price
            };
            var jsonData = JsonConvert.SerializeObject(data);

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
    }
}
