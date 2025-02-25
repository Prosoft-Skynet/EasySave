using System.Net.WebSockets;
using System.Text;

class ConsoleDéportée
{
    private static async Task ConnectAsync(string serverUrl)
    {
        using (var client = new ClientWebSocket())
        {
            await client.ConnectAsync(new Uri(serverUrl), CancellationToken.None);
            Console.WriteLine($"Connected to WebSocket {serverUrl}");

            var buffer = new byte[1024];

            while (client.State == WebSocketState.Open)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"[LOG] {message}");
            }
        }
    }

    static async Task Main()
    {
        string serverUrl = "ws://localhost:5000/";
        await ConnectAsync(serverUrl);
    }
}
