using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Quartz;
using Variedades;

namespace Variedades
{
    public class SmsJob : Quartz.IJob
    {

        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("Greetings from SMS Job!");
            List<SMS> sms = new List<SMS>();
            MySqlConnection conn = DbConn.getDBConnection();
            try
            {
                conn.Open();
                String query = " select id, mobile_number, message,status from sms_queue where status='PENDING' limit 10";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Prepare();
                MySqlDataReader rdr = sqlCmd.ExecuteReader();
                
                while (rdr.Read())
                {
                    SMS newSms = new SMS();
                    newSms.id = rdr.GetInt32(0);
                    newSms.mobile = rdr.GetString(1);
                    newSms.message = rdr.GetString(2);
                    newSms.status = rdr.GetString(3);

                    sms.Add(newSms);
                }
                
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            finally
            {
                conn.Close();
            }
            SendSMS(sms);
        }
        private void SendSMS(List<SMS> sms)
        {
           
            
            Console.WriteLine("Start SMS Method call ");
            string URL = "https://app.notify.lk/api/v1/send";
            //string urlParameters = "?user_id=13005&api_key=erBfSX6dNq4cdkR2Oj5h&sender_id=NotifyDEMO&to=94" + mobile.TrimStart(new Char[] { '0' });
            
            foreach (var smsItem in sms)
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(URL);
                var urlParameters = (FormattableString)$"?user_id=13005&api_key=erBfSX6dNq4cdkR2Oj5h&sender_id=NotifyDEMO&to=94{smsItem.mobile.TrimStart(new Char[] { '0' })}&message={smsItem.message}";
                HttpResponseMessage response = client.GetAsync(urlParameters.ToString()).Result;
                Console.WriteLine(response.RequestMessage.RequestUri);
                Console.WriteLine(smsItem.mobile + " sms status" + response.StatusCode);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UpdateSMSStatus(smsItem.id, "SENT");
                }
                else
                {
                    UpdateSMSStatus(smsItem.id, "FAILED");
                }
            }
            Console.WriteLine("Done SMS Method call ");
        }

        private void UpdateSMSStatus(int id, string status)
        {
            Console.WriteLine("Updating SMS status ");
            MySqlConnection conn = DbConn.getDBConnection();
            try
            {
                conn.Open();
                String query = " update sms_queue set status=@status where id=@id";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Parameters.AddWithValue("@id", id);
                sqlCmd.Parameters.AddWithValue("@status", status);
                sqlCmd.Prepare();
                sqlCmd.ExecuteNonQuery();

            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            finally
            {
                conn.Close();
            }
            Console.WriteLine("Done Updating SMS status ");
        }
        public class SMS
        {
            public int id { set; get; }
            public string mobile { set; get; }
            public string message { set;get; }
            public string status { set; get; }
        }
    }
}
