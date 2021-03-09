using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Variedades
{
    /// <summary>
    /// Interaction logic for SmsControll.xaml
    /// </summary>
    public partial class SmsControll : UserControl
    {
        List<String> selectedProviders = new List<string>();
        public SmsControll()
        {
            InitializeComponent();
        }

        public void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            string content = (sender as CheckBox).Content.ToString();
            selectedProviders.Add(content);
        }
        public void checkBox_Unchecked (object sender, RoutedEventArgs e) 
        {
            string content = (sender as CheckBox).Content.ToString();
            selectedProviders.Remove(content);
        }
        public void bt_save_Click(object sender, RoutedEventArgs e)
        {
            selectedProviders.ForEach(Console.WriteLine);
            if (selectedProviders.Count<1)
            {
                MessageBox.Show("Please select Provider !");
                return;
            }
            TextRange txt = new TextRange(smsMsg.Document.ContentStart, smsMsg.Document.ContentEnd);

            if (txt.Text==String.Empty)
            {
                MessageBox.Show("Please add message !");
                return;
            }
            SaveData(txt.Text, string.Join(",", selectedProviders));
        }
        public void bt_reset_Click(object sender, RoutedEventArgs e)
        {
            VideoCon.IsChecked = false;
            TataSky.IsChecked = false;
            Airtel.IsChecked = false;
            D2H.IsChecked = false;
            DishTv.IsChecked = false;
            TvLanka.IsChecked = false;
            SunDirect.IsChecked = false;
            smsMsg.Document.Blocks.Clear();
        }

        private void SaveData(string msg, string providers)
        {
            MySqlConnection conn = DbConn.getDBConnection();
            try
            {
                conn.Open();
                String query = " insert into sms_queue (mobile_number,message,status) select mobile,@msg as message, 'PENDING' " +
"from customer where id in(select customer_id from reload where  FIND_IN_SET(provider, @providers))  ";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                sqlCmd.Prepare();
                sqlCmd.Parameters.AddWithValue("@msg", msg);
                sqlCmd.Parameters.AddWithValue("@providers", providers);
                sqlCmd.ExecuteNonQuery();
                MessageBox.Show("SMS sent !");
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            finally
            {
                conn.Close();
            }
        }

    }
}
