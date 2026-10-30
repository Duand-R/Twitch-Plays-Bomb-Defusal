using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class TwitchConnect : MonoBehaviour
{
    public string username;   
    public string oauth;   
    public string channel;
    public BombVoteManager bomb;
    public TwitchChatUI chatUI;
    private TcpClient client;
    private StreamReader reader;
    private StreamWriter writer;
    private Task<string> nextMessage;
    private bool connected;
    private bool closed;

    private async void Start()
    {
        try
        {
            client = new TcpClient();
            await client.ConnectAsync("irc.chat.twitch.tv", 6697);

            if (closed) return;

            SslStream stream = new SslStream(client.GetStream());
            await stream.AuthenticateAsClientAsync("irc.chat.twitch.tv");

            if (closed) return;

            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            writer.NewLine = "\r\n";
            writer.AutoFlush = true;

            await writer.WriteLineAsync("PASS " + oauth.Trim());
            await writer.WriteLineAsync("NICK " + username.Trim().ToLower());
            await writer.WriteLineAsync(
                "JOIN #" + channel.Trim().TrimStart('#').ToLower()
            );

            if (closed) return;

            nextMessage = reader.ReadLineAsync();
            connected = true;
        }
        catch (System.Exception)
        {
            if (!closed)

            Disconnect();
        }
    }

    private void Update()
    {
        if (!connected || !nextMessage.IsCompleted)
            return;

        try
        {
            string line = nextMessage.GetAwaiter().GetResult();

            if (line == null)
            {
                
                Disconnect();
                return;
            }

            if (line.StartsWith("PING"))
            {
                writer.WriteLine("PONG" + line.Substring(4));
            }
            else if (line.Contains(" PRIVMSG "))
            {
                int nameEnd = line.IndexOf('!');
                int messageStart = line.IndexOf(" :");

                if (nameEnd > 1 && messageStart >= 0)
                {
                    string user = line.Substring(1, nameEnd - 1);
                    string message = line.Substring(messageStart + 2);

                    if (chatUI != null)
                        chatUI.AddMessage(user, message);

                    if (bomb != null)
                        bomb.Vote(user, message);
                }
            }
            else if (line.Contains("Login authentication failed"))
            {
                Disconnect();
                return;
            }

            nextMessage = reader.ReadLineAsync();
        }
        catch (System.Exception)
        {
            Disconnect();
        }
    }

    private void Disconnect()
    {
        connected = false;
        client?.Close();
    }

    private void OnDestroy()
    {
        closed = true;
        Disconnect();
    }
}