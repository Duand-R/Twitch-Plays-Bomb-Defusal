using UnityEngine;

public class Wire : MonoBehaviour
{
    public string wireColor;

    private void OnMouseDown()
    {
        BombManager.Instance.CutWire(wireColor);
    }
}
