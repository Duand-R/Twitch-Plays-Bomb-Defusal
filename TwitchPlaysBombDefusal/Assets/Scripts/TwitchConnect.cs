using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class TwitchConnect : MonoBehaviour
{
    public string username;   // Token 所属账号，填小写
    public string oauth;      // 新 Token，带 oauth: 前缀
    public string channel;    // 主播账号名，不带 #

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

            Debug.Log("Twitch 连接已建立，正在加入频道。");
        }
        catch (System.Exception)
        {
            if (!closed)
                Debug.LogError("Twitch 连接失败，请检查网络和登录信息。");

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
                Debug.LogWarning("Twitch 连接已断开。");
                Disconnect();
                return;
            }

            // 回复服务器心跳。
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

                    // 显示所有聊天内容。
                    if (chatUI != null)
                        chatUI.AddMessage(user, message);

                    // 投票脚本会自行判断是不是 r、b、y。
                    if (bomb != null)
                        bomb.Vote(user, message);
                }
            }
            else if (line.Contains("Login authentication failed"))
            {
                Debug.LogError("Twitch 登录失败，请检查账号和新 Token。");
                Disconnect();
                return;
            }

            nextMessage = reader.ReadLineAsync();
        }
        catch (System.Exception)
        {
            Debug.LogWarning("Twitch 读取失败，连接已关闭。");
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