using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BoycoT_Virtual_ZPL_Printer
{
    public class ZplTcpServer
    {
        private readonly int _port;
        private TcpListener? _listener;

        public ZplTcpServer(int port = 9100)
        {
            _port = port;
        }

        public void Start(Action<string> onZplReceived)
        {
            _listener = new TcpListener(IPAddress.Loopback, _port);
            _listener.Start();

            System.Diagnostics.Debug.WriteLine($"ZPL TCP server listening on port {_port}...");

            Task.Run(async () =>
            {
                while (true)
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client, onZplReceived);
                }
            });
        }

        private async Task HandleClientAsync(TcpClient client, Action<string> onZplReceived)
        {
            using var stream = client.GetStream();

            var buffer = new byte[4096];
            var sb = new StringBuilder();

            try
            {
                // Buffer all incoming bytes until the connection closes or we see ^XZ
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                    // Stop reading once we have at least one complete label block
                    if (sb.ToString().Contains("^XZ", StringComparison.OrdinalIgnoreCase))
                        break;
                }

                string payload = sb.ToString().Trim();

                // Extract every complete ^XA...^XZ block and fire once per block.
                // Any garbage before/between/after blocks is silently ignored —
                // no more WrapTextAsZpl producing phantom tabs.
                var matches = System.Text.RegularExpressions.Regex.Matches(
                    payload,
                    @"\^XA.*?\^XZ",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase |
                    System.Text.RegularExpressions.RegexOptions.Singleline);

                if (matches.Count > 0)
                {
                    // Re-assemble all labels in one shot so multi-label streams
                    // (e.g. a ~DG preamble + two ^XA...^XZ blocks) stay together
                    var combined = new StringBuilder();

                    // Preserve any ~DG / ~DY commands that appear before the first ^XA
                    int firstXa = payload.IndexOf("^XA", StringComparison.OrdinalIgnoreCase);
                    if (firstXa > 0)
                        combined.Append(payload[..firstXa]);

                    foreach (System.Text.RegularExpressions.Match m in matches)
                        combined.Append(m.Value);

                    onZplReceived?.Invoke(combined.ToString());
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No complete ^XA...^XZ block found — ignoring payload.");
                }
            }
            catch (IOException)
            {
                // Normal — spooler closed the connection
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