using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Variedades
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            //_ = RunProgramRunExample();

        }

        //Hide and show sidebar menu
        private void ToggleMenu(object sender, RoutedEventArgs e)
        {
            if (Sidebar.Width == new GridLength(1, GridUnitType.Star))
            {
                Duration duration = new Duration(TimeSpan.FromMilliseconds(500));

                var animation = new GridLengthAnimation
                {
                    Duration = duration,
                    From = new GridLength(1, GridUnitType.Star),
                    To = new GridLength(0, GridUnitType.Star)
                };

                Sidebar.BeginAnimation(ColumnDefinition.WidthProperty, animation);
            }
            else
            {
                Duration duration = new Duration(TimeSpan.FromMilliseconds(500));

                var animation = new GridLengthAnimation
                {
                    Duration = duration,
                    From = new GridLength(0, GridUnitType.Star),
                    To = new GridLength(1, GridUnitType.Star)
                };

                Sidebar.BeginAnimation(ColumnDefinition.WidthProperty, animation);
            }
        }

        //Change content usercontrol from sidebar menu
        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc = null;
            ContentMain.Children.Clear();
            
            //Change usercontrol 
            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "ItemClients":
                    usc = new UserControlClient();
                    ContentMain.Children.Add(usc);
                    break;
                case "ItemReload":
                    usc = new RechargeClient();
                    ContentMain.Children.Add(usc);
                    break;
                case "ItemSMS":
                    usc = new SmsControll();
                    ContentMain.Children.Add(usc);
                    break;
                default:
                    break;
            }
        }
        private static async Task RunProgramRunExample()
        {
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                // and start it off
                await scheduler.Start();

                // define the job and tie it to our HelloJob class
                IJobDetail smsjob = JobBuilder.Create<SmsJob>()
                    .WithIdentity("SMSJOB", "SatsTech")
                    .Build();

                IJobDetail reminderJob = JobBuilder.Create<ReminderJob>()
                    .WithIdentity("ReminderJob", "SatsTech")
                    .Build();

                // Trigger the job to run now, and then repeat every 10 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("SMSTig", "SatsTech")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever())
                    .Build();


                ITrigger runOnceTrigger = TriggerBuilder.Create().Build();

                // Tell quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(smsjob, trigger);
                await scheduler.ScheduleJob(reminderJob, runOnceTrigger);
                // some sleep to show what's happening
                await Task.Delay(TimeSpan.FromSeconds(60));

                // and last shut down the scheduler when you are ready to close your program
                await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }
        private class ConsoleLogProvider : ILogProvider
        {
            public Logger GetLogger(string name)
            {
                return (level, func, exception, parameters) =>
                {
                    if (level >= LogLevel.Info && func != null)
                    {
                        Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                    }
                    return true;
                };
            }

            public IDisposable OpenNestedContext(string message)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenMappedContext(string key, string value)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
            {
                throw new NotImplementedException();
            }
        }
    }
}
