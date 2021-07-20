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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using Annunciator;
using AnnunciatorHost.AnnunciatorServiceHost;

namespace AnnunciatorHost
{  
    public partial class MainWindow : Window, IAnnunciatorServiceCallback
    {
        private AnnunciatorServiceClient _annunciator;

        public MainWindow()
        {
            var host = new ServiceHost(typeof(AnnunciatorService));            
            host.Open();

            _annunciator = new AnnunciatorServiceClient(new InstanceContext(this)); 
            InitializeComponent();
        }     

        private void SendingButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageTextBox.Text != null) 
            {
                _annunciator.SendMessage(MessageTextBox.Text);
                ListBoxLog.Items.Add(DateTime.Now.ToString() + ":" + MessageTextBox.Text);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        public void CallBackMessage(string message) 
        { } 
    }
}
