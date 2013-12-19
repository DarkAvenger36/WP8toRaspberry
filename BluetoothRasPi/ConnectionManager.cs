using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace BluetoothRasPi
{
    class ConnectionManager
    {
        //Socket per la connessione
        private StreamSocket socket;
        //DataWriter per scrivere dati
        private DataWriter dataWriter;

        public void Initialize()
        {
            socket = new StreamSocket();
        }


        public async void Connect(HostName hostName)
        {
            await socket.ConnectAsync(hostName, "5");
            dataWriter = new DataWriter(socket.OutputStream);
        }

        public async Task<uint> SendCommand(string command)
        {
            uint sentCommandSize = 0;
            if (dataWriter != null)
            {
                uint commandSize = dataWriter.MeasureString(command);
                dataWriter.WriteByte((Byte)commandSize);
                sentCommandSize = dataWriter.WriteString(command);
                await dataWriter.StoreAsync();
            }
            return sentCommandSize;
        }

    }
}
