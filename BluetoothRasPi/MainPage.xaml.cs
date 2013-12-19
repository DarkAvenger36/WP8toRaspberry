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

namespace BluetoothRasPi
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ConnectionManager connectionManager = null;
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
            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";

            var pairedDevices = await PeerFinder.FindAllPeersAsync();

            if (pairedDevices.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No paired devices found");
            }
            else
            {
                PeerInformation selectedDevice = pairedDevices[0];
                connectionManager.Connect(selectedDevice.HostName);
                ConnectBtn.Content = "Connected";
            }
        }

        private void SendData_Click(object sender, RoutedEventArgs e)
        {
            string command = "Hello world";

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