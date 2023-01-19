using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FileSendClient
{
    public class FileSendClient
    {
        public event SendFileEventHandler sendFileEventHandler = null;

        public string ipString { get; private set; }
        public int port { get; private set; }
        public string fileName { get; private set; }

        const int PACKET_SIZE = 1024;
        public FileSendClient(string ipstring, int _port)
        {
            ipString = ipstring;
            port = _port;
        }

        delegate void sendAsyncDelegate(string filename);
        public void sendAsync(string filename)
        {
            sendAsyncDelegate dele = send;
            dele.BeginInvoke(filename, null, null);
        }

        public void send(string filename)
        {

            if (!File.Exists(filename))
            {
                return;
            }

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse(ipString), port);
            socket.Connect(iPEnd);

            byte[] packet = new byte[PACKET_SIZE];
            MemoryStream ms = new MemoryStream(packet);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(filename);

            bw.Close();
            ms.Close();

            socket.Send(packet);

            FileStream fs = File.OpenRead(filename);
            ms = new MemoryStream(packet);
            bw = new BinaryWriter(ms);
            bw.Write(fs.Length);

            bw.Close();
            ms.Close();

            socket.Send(packet, 0, 8, SocketFlags.None);

            long remain = fs.Length;
            int sendedLenth;

            while (remain >= PACKET_SIZE)
            {
                fs.Read(packet, 0, PACKET_SIZE);
                sendedLenth = socket.Send(packet);

                while (sendedLenth < PACKET_SIZE)
                {
                    sendedLenth += socket.Send(packet, sendedLenth, PACKET_SIZE - sendedLenth, SocketFlags.None);
                }

                // remain event handler ?
                sendFileEventHandler(this, new SendFileEventArgs(filename, remain));
                remain -= PACKET_SIZE;
            }
            fs.Read(packet, 0, (int)remain);
            sendedLenth = socket.Send(packet);

            while (sendedLenth < remain)
            {
                sendedLenth += socket.Send(packet, sendedLenth, (int)remain - sendedLenth, SocketFlags.None);
            }
            // remain event handler ?
            sendFileEventHandler(this, new SendFileEventArgs(filename, remain));
            fs.Close();
            socket.Close();
        }
    }
}