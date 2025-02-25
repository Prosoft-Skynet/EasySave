using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace EasySaveLogger
{
    public class WebSocketServer
    {
        private readonly HttpListener _httpListener;
        private readonly List<WebSocket> _clients = new List<WebSocket>();
        private readonly string _logFilePath;

        public WebSocketServer(string url, string logFilePath)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(url);
            _logFilePath = logFilePath;
        }

        public async Task StartAsync()
        {
            _httpListener.Start();

            while (true)
            {
                var context = await _httpListener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    var wsContext = await context.AcceptWebSocketAsync(null);
                    WebSocket webSocket = wsContext.WebSocket;
                    _clients.Add(webSocket);

                    await SendLogsHistoryAsync(webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async Task SendLogsHistoryAsync(WebSocket client)
        {
            try
            {
                if (File.Exists(_logFilePath))
                {
                    string[] logs = File.ReadAllLines(_logFilePath);
                    foreach (var log in logs)
                    {
                        if (client.State == WebSocketState.Open)
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes(log);
                            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public async Task BroadcastLogAsync(string logMessage)
        {
            var buffer = Encoding.UTF8.GetBytes(logMessage);
            var tasks = new List<Task>();

            foreach (var client in _clients)
            {
                if (client.State == WebSocketState.Open)
                {
                    tasks.Add(client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None));
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}
