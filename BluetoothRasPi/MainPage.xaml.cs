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
        // Costruttore
        public MainPage()
        {
            InitializeComponent();
            //connectionManager = new ConnectionManager();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            AppToDevice();
        }


        private async void AppToDevice()
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
                // Select a paired device. In this example, just pick the first one.
                PeerInformation selectedDevice = pairedDevices[0];
                // Attempt a connection
                StreamSocket socket = new StreamSocket();
                // Make sure ID_CAP_NETWORKING is enabled in your WMAppManifest.xml, or the next 
                // line will throw an Access Denied exception.
                // In this example, the second parameter of the call to ConnectAsync() is the RFCOMM port number, and can range 
                // in value from 1 to 30.
                await socket.ConnectAsync(selectedDevice.HostName, "5");
                dataWriter = new DataWriter(socket.OutputStream);
                ConnectBtn.Content = "Connected";
                ConnectBtn.IsEnabled = false;
            }
        }


        private void SendData_Click(object sender, RoutedEventArgs e)
        {
            string command = "0";
            SendCommand(command);
        }

        public async Task<uint> SendCommand(string command)
        {
            uint sentCommandSize = 0;
            string cmd ;
            if (dataWriter != null)
            {
                uint commandSize = dataWriter.MeasureString(command);
                //dataWriter.WriteByte((byte)commandSize);
                sentCommandSize = dataWriter.WriteString(command);
                await dataWriter.StoreAsync();
            }
            return sentCommandSize;
        }




        private void ThumbStick_OnNewPosition(object sender, string e)
        {
            System.Diagnostics.Debug.WriteLine(e);
            SendCommand(e);
            System.Threading.Thread.Sleep(150);
        }
    }
}