

using DevExpress.XtraEditors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PMSReport.Class
{
    class PoliceReportAPIInterface
    {
        
        private static string APIKeyDesc = "XApiKey";
        private static string APIKeyValue = "d628cf55-4377-4248-CNET-423b1ba1fb5e";
        private static string APICheckURL = "http://192.168.1.183:8988/api/check";
        private static string SendOneReportURL = "http://192.168.1.183:8988/api/PoliceReport";
        private static string SendMultipleReportURL = "http://192.168.1.183:8988/api/PoliceReport/dump";



        public static bool APICheck()
        {
            bool connected = false;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, APICheckURL))
                    {
                        try
                        {
                            HttpResponseMessage response = client.SendAsync(request).Result;
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                connected = true;
                            }
                        }
                        catch
                        {
                            XtraMessageBox.Show("Could not connect with API.", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            connected = false;
                        }
                    }
                }
            }
            catch
            {
                XtraMessageBox.Show("Could not connect with API.", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                connected = false;
            }
            return connected;
        }

        public async static void APIPostRequest(PoliceReportGovernment PoliceReport)
        {
            

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                client.DefaultRequestHeaders.Add(APIKeyDesc, APIKeyValue);

                var newPostJason = JsonConvert.SerializeObject(PoliceReport);
                var payload = new StringContent(newPostJason, Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = await client.PostAsync(SendOneReportURL, payload))
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        XtraMessageBox.Show("Fail to send Police Report to server.", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        public async static void APIPostRequest(List<PoliceReportGovernment> PoliceReportlist)
        {
            
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
                client.DefaultRequestHeaders.Add(APIKeyDesc, APIKeyValue);

                var newPostJason = JsonConvert.SerializeObject(PoliceReportlist);
                var payload = new StringContent(newPostJason, Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = await client.PostAsync(SendMultipleReportURL, payload))
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        XtraMessageBox.Show("Fail to send Police Report to server.", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}
