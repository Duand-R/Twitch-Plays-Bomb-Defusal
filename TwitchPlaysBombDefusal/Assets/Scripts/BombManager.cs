using TMPro;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    public static BombManager Instance;

    public string correctWire = "red";

    public TMP_Text resultText;
    public GameObject redWire;
    public GameObject blueWire;
    public GameObject yellowWire;

    public bool gameOver { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void CutWire(string color)
    {
        if (gameOver) return;

        color = color.Trim().ToLowerInvariant();

        if (color != "red" && color != "blue" && color != "yellow")
            return;

        gameOver = true;

        if (color == "red") redWire.SetActive(false);
        if (color == "blue") blueWire.SetActive(false);
        if (color == "yellow") yellowWire.SetActive(false);

        bool success =
            color == correctWire.Trim().ToLowerInvariant();

        resultText.text = success
            ? "BOMB DEFUSED!"
            : "BOOM! WRONG WIRE!";

        Debug.Log(resultText.text);
    }

    public void ResetBomb()
    {
        gameOver = false;

        redWire.SetActive(true);
        blueWire.SetActive(true);
        yellowWire.SetActive(true);

        resultText.text = "Vote: r / b / y";
    }
}