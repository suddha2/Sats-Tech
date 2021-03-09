using MySql.Data.MySqlClient;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Variedades;

namespace Variedades
{
    class ReminderJob : IJob
    {

        public async Task Execute(IJobExecutionContext context)
        {
            MySqlConnection conn = DbConn.getDBConnection();
            try
            {
                conn.Open();
                String query = " insert into sms_queue (mobile_number,message,status) " +
                    " select mobile,CONCAT('Dear customer,\nYour DTH subscription going to expire.\nPlease pay your bill on or before ', " +
                    " DATE_FORMAT(DATE_ADD(now(), INTERVAL 4 DAY), '%Y/%m/%d'), '.\nHELP LINE:0768866972') as message, 'PENDING' " +
                    " from customer where id in (select customer_id from reload where datediff(expiry_date, now()) = 4 ) ";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Prepare();
                sqlCmd.ExecuteNonQuery();

            }
            catch(Exception err)
            {
                Console.WriteLine(err);
            }
            finally
            {
                conn.Close();
            }

            await Console.Out.WriteLineAsync("Greetings from SMS Job!");
        }
    }
}
