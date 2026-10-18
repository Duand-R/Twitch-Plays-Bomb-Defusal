using UnityEngine;

public class BombManager : MonoBehaviour
{
    public static BombManager Instance;

    public string correctWire = "red";

    private bool gameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    public void CutWire(string color)
    {
        if (gameOver) return;

        gameOver = true;

        if (color == correctWire)
        {
            Debug.Log("BOMB DEFUSED!");
        }
        else
        {
            Debug.Log("BOOM! WRONG WIRE!");
        }
    }
}
