using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BoycoT_Virtual_ZPL_Printer
{
    public class ZplTcpServer
    {
        private readonly int _port;
        private TcpListener _listener;

        public ZplTcpServer(int port = 9100) // Port 9100 is common for Zebra printers
        {
            _port = port;
        }

        public void Start(Action<string> onZplReceived)
        {
            _listener = new TcpListener(IPAddress.Loopback, _port); // use IPAddress.Any to allow remote clients
            _listener.Start();

            Task.Run(async () =>
            {
                Console.WriteLine($"ZPL TCP server listening on port {_port}...");

                while (true)
                {
                    try
                    {
                        var client = await _listener.AcceptTcpClientAsync();
                        _ = HandleClientAsync(client, onZplReceived);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("TCP accept error: " + ex.Message);
                    }
                }
            });
        }

        private async Task HandleClientAsync(TcpClient client, Action<string> onZplReceived)
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            try
            {
                string zpl = await reader.ReadToEndAsync();
                onZplReceived?.Invoke(zpl);
            }
            catch (IOException ex)
            {
                Console.WriteLine("TCP read error: " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        public void Stop()
        {
            _listener?.Stop();
        }
    }
}
