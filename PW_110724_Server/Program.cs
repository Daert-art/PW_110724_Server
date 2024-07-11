using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PW_110724_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint ep = new IPEndPoint(ip, 11000);
            s.Bind(ep);
            s.Listen(10);

            try
            {
                while (true)
                {
                    Socket ns = s.Accept();
                    IPEndPoint remoteIpEndPoint = ns.RemoteEndPoint as IPEndPoint;
                    string clientIp = remoteIpEndPoint.Address.ToString();

                    byte[] buffer = new byte[1024];
                    int receivedBytes = ns.Receive(buffer);
                    string clientMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);

                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} from {clientIp} а string is received: {clientMessage}");

                    string serverMessage = $"{DateTime.Now.ToLongTimeString()} Hello, client!";
                    ns.Send(Encoding.ASCII.GetBytes(serverMessage));

                    ns.Shutdown(SocketShutdown.Both);
                    ns.Close();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
