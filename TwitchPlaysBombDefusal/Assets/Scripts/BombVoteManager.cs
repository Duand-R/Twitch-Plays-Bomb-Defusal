using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BombVoteManager : MonoBehaviour
{
    public BombManager bombManager;

    public float voteTime = 20f;

    public TMP_Text timerText;
    public TMP_Text votesText;

    private Dictionary<string, string> votes =
        new Dictionary<string, string>();

    private float timeLeft;
    private bool voting;

    private void Start()
    {
        RestartBomb();
    }

    private void Update()
    {
        if (!voting) return;

        if (bombManager.gameOver)
        {
            voting = false;
            return;
        }

        timeLeft = Mathf.Max(
            0f, timeLeft - Time.unscaledDeltaTime
        );

        timerText.text = Mathf.CeilToInt(timeLeft).ToString();

        if (timeLeft <= 0f)
            FinishVote();
    }

    public void Vote(string user, string message)
    {
        if (!voting || bombManager.gameOver) return;

        message = message.Trim().ToLowerInvariant();

        string color;

        switch (message)
        {
            case "r":
                color = "red";
                break;

            case "b":
                color = "blue";
                break;

            case "y":
                color = "yellow";
                break;

            default:
                return;
        }

        votes[user.ToLowerInvariant()] = color;
        UpdateVotes();
    }

    private int Count(string color)
    {
        int count = 0;

        foreach (string vote in votes.Values)
        {
            if (vote == color)
                count++;
        }

        return count;
    }

    private void UpdateVotes()
    {
        votesText.text =
            "RED: " + Count("red") +
            "\nBLUE: " + Count("blue") +
            "\nYELLOW: " + Count("yellow");
    }

    private void FinishVote()
    {
        if (votes.Count == 0)
        {
            timeLeft = Mathf.Max(1f, voteTime);
            bombManager.resultText.text =
                "No votes! Vote again: r / b / y";
            return;
        }

        voting = false;

        int highest = Mathf.Max(
            Count("red"),
            Mathf.Max(Count("blue"), Count("yellow"))
        );

        List<string> winners = new List<string>();

        foreach (string color in new[] { "red", "blue", "yellow" })
        {
            if (Count(color) == highest)
                winners.Add(color);
        }

        string selected = winners[Random.Range(0, winners.Count)];

        bombManager.CutWire(selected);
    }

    public void RestartBomb()
    {
        bombManager.ResetBomb();

        votes.Clear();
        timeLeft = Mathf.Max(1f, voteTime);
        voting = true;

        timerText.text = Mathf.CeilToInt(timeLeft).ToString();
        UpdateVotes();
    }
}