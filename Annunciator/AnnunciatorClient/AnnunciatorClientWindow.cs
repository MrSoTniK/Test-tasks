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
using AnnunciatorClient.AnnunciatorServiceHost;

namespace AnnunciatorClient
{
    public partial class MainWindow : Window, IAnnunciatorServiceCallback
    {
        private bool _isConnected = false;
        private AnnunciatorServiceClient _client;
        private int _id;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void CallBackMessage(string message)
        {
                MessageListBox.Items.Add(message);                        
        }

        private void ConnectUser() 
        {
            if (!_isConnected) 
            {
                _client = new AnnunciatorServiceClient(new System.ServiceModel.InstanceContext(this));
                _id = _client.Connect();
                ConDisconButton.Content = "Отключиться";
                _isConnected = true;
            }
        }

        private void DisconnectUser()
        {
            if (_isConnected) 
            {
                _client.Disconnect(_id);
                _client = null;
                ConDisconButton.Content = "Подключиться";
                _isConnected = false;
            }
        }      

        private void ConDisconButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isConnected) 
            {
                DisconnectUser();
            }
            else 
            {
                ConnectUser();
            }
        }

        private void ClientWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }     
    }
}
