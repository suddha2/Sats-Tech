using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Variedades
{
    /// <summary>
    /// Interaction logic for UserEdit.xaml
    /// </summary>
    public partial class UserEdit : Window
    {
        public Customer editCustomer = null;
        public UserEdit()
        {
            InitializeComponent();
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            sid.Text = r;
            sid.IsEnabled = false;
        }

        public UserEdit(Customer cus)
        {
            InitializeComponent();

            name.Text = cus?.Name;
            mobile.Text = cus?.Mobile;
            vc.Text = cus?.VC;
            sid.Text = cus?.SID;
            sid.IsEnabled = false;
        }

        private void bt_save_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = new Customer();
            customer.Name = name.Text;
            customer.Mobile = mobile.Text;
            customer.VC = vc.Text;
            customer.SID = sid.Text;
            MySqlConnection conn = DbConn.getDBConnection();
            try { 
                conn.Open();
                String query = "insert into customer (name, mobile, vc_number, sid) values(@name,@mobile,@vc,@sid) ON DUPLICATE KEY UPDATE name=@name,vc_number=@vc, mobile=@mobile ";
                MySqlCommand sqlCmd = new MySqlCommand(query, conn);
                
                sqlCmd.Parameters.AddWithValue("@name", name.Text);
                sqlCmd.Parameters.AddWithValue("@mobile", mobile.Text);
                sqlCmd.Parameters.AddWithValue("@vc", vc.Text);
                sqlCmd.Parameters.AddWithValue("@sid", sid.Text);
                sqlCmd.Prepare();
                sqlCmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show(" Saved Successfully ! ");
                this.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            finally
            {
                conn.Close();
            }

        }

        private void bt_reset_Click(object sender, RoutedEventArgs e)
        {
            name.Text = editCustomer?.Name;
            mobile.Text = editCustomer?.Mobile;
            vc.Text = editCustomer?.VC;
            sid.Text = editCustomer?.SID;
        }
    }
}
