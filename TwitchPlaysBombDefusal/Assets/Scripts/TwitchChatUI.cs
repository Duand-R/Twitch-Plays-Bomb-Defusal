using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TwitchChatUI : MonoBehaviour
{
    public TMP_Text chatText;
    public int maxMessages = 15;

    private Queue<string> messages = new Queue<string>();

    private void Start()
    {
        chatText.text = "";
    }

    public void AddMessage(string username, string message)
    {
        messages.Enqueue(username + ": " + message);

        while (messages.Count > Mathf.Max(1, maxMessages))
        {
            messages.Dequeue();
        }

        chatText.text = string.Join("\n", messages);
    }
}