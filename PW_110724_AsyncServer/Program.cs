using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PW_110724_AsyncServer
{
    class AsyncServer
    {
        private IPEndPoint endP;
        private Socket socket;

        public AsyncServer(string strAddr, int port)
        {
            endP = new IPEndPoint(IPAddress.Parse(strAddr), port);
        }

        private void MyAcceptCallbackFunction(IAsyncResult ia)
        {
            Socket serverSocket = (Socket)ia.AsyncState;
            Socket clientSocket = serverSocket.EndAccept(ia);
            Console.WriteLine(clientSocket.RemoteEndPoint.ToString());

            byte[] buffer = new byte[1024];
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(MyReceiveCallbackFunction), new object[] { clientSocket, buffer });

            serverSocket.BeginAccept(new AsyncCallback(MyAcceptCallbackFunction), serverSocket);
        }

        private void MyReceiveCallbackFunction(IAsyncResult ia)
        {
            object[] state = (object[])ia.AsyncState;
            Socket clientSocket = (Socket)state[0];
            byte[] buffer = (byte[])state[1];

            int bytesRead = clientSocket.EndReceive(ia);
            if (bytesRead > 0)
            {
                string clientMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"{DateTime.Now.ToLongTimeString()} from {clientSocket.RemoteEndPoint} а string is received: {clientMessage}");

                byte[] sendBuffer = Encoding.ASCII.GetBytes($"{DateTime.Now.ToLongTimeString()} Hello, client!");
                clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, new AsyncCallback(MySendCallbackFunction), clientSocket);
            }
        }
        private void MySendCallbackFunction(IAsyncResult ia)
        {
            Socket clientSocket = (Socket)ia.AsyncState;
            int bytesSent = clientSocket.EndSend(ia);
            clientSocket.Shutdown(SocketShutdown.Send);
            clientSocket.Close();
        }
        public void StartServer()
        {
            if (socket != null)
                return;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endP);
            socket.Listen(10);
            // Начинаем асинхронный Accept, при подключении клиента вызовется обработчик MyAcceptCallbackFunction
            socket.BeginAccept(new AsyncCallback(MyAcceptCallbackFunction), socket);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            AsyncServer server = new AsyncServer("127.0.0.1", 11000);
            server.StartServer();
            Console.ReadLine();
        }
    }
}
