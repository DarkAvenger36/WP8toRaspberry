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

namespace BluetoothRasPi
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ConnectionManager connectionManager = null;
        private DataWriter dataWriter;
        // Costruttore
        public MainPage()
        {
            InitializeComponent();
            connectionManager = new ConnectionManager();
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
                //DoSomethingUseful(socket);
            }
        }
        private async void AppToDevice2()
        {
            ConnectBtn.Content = "Connecting...";
            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
            var pairedDevices = await PeerFinder.FindAllPeersAsync();

            if (pairedDevices.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No paired devices were found.");
            }
            else
            {
                foreach (var pairedDevice in pairedDevices)
                {
                    if (pairedDevice.DisplayName == "raspberrypi-0")
                    {
                        connectionManager.Connect(pairedDevice.HostName);
                        ConnectBtn.Content = "Connected";
                        //DeviceName.IsReadOnly = true;
                        ConnectBtn.IsEnabled = false;
                        continue;
                    }
                }
            }
        }

        private void SendData_Click(object sender, RoutedEventArgs e)
        {
            string command = "Hello world";
            SendCommand(command);
        }

        public async Task<uint> SendCommand(string command)
        {
            uint sentCommandSize = 0;
            if (dataWriter != null)
            {
                uint commandSize = dataWriter.MeasureString(command);
                dataWriter.WriteByte((byte)commandSize);
                sentCommandSize = dataWriter.WriteString(command);
                await dataWriter.StoreAsync();
            }
            return sentCommandSize;
        }

        // Codice di esempio per la realizzazione di una ApplicationBar localizzata
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Imposta la barra delle applicazioni della pagina su una nuova istanza di ApplicationBar
        //    ApplicationBar = nuova ApplicationBar();

        //    // Crea un nuovo pulsante e imposta il valore del testo sulla stringa localizzata da AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Crea una nuova voce di menu con la stringa localizzata da AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}