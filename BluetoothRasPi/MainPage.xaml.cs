using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BluetoothRasPi.Resources;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Text;

namespace BluetoothRasPi
{
    public partial class MainPage : PhoneApplicationPage
    {
        //private ConnectionManager connectionManager = null;
        private DataWriter dataWriter;

        StreamSocket _socket;
        private bool _isConnected = false;

        // Costruttore
        public MainPage()
        {
            InitializeComponent();
            //connectionManager = new ConnectionManager();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            TryConnect();
        }


        private void ThumbStick_OnNewPosition(object sender, string e)
        {
            System.Diagnostics.Debug.WriteLine(e);
            if (_isConnected)
            {
                Write(e);
            }
        }


        private async void TryConnect()
        {
            ConnectBtn.Content = "Connecting...";
            // Configure PeerFinder to search for all paired devices.
            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
            var pairedDevices = await PeerFinder.FindAllPeersAsync();

            if (pairedDevices.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No paired devices were found.");
            }
            else
            {
                PeerInformation selectedDevice = pairedDevices[0];
                _socket = new StreamSocket();
                await _socket.ConnectAsync(selectedDevice.HostName, "5");
                _isConnected = true;
                ConnectBtn.Content = "Connected";
                ConnectBtn.IsEnabled = false;
            }
        }

        private async void Write(string str)
        {
            System.Diagnostics.Debug.WriteLine("sto scrivendo " + str);
            var dataBuffer = GetBufferFromByteArray(Encoding.UTF8.GetBytes(str));
            await _socket.OutputStream.WriteAsync(dataBuffer);
        }

        private IBuffer GetBufferFromByteArray(byte[] package)
        {
            using (var dw = new DataWriter())
            {
                dw.WriteBytes(package);
                return dw.DetachBuffer();
            }
        }
    }
}