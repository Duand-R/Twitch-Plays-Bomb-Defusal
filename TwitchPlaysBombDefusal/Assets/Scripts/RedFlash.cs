using UnityEngine;
using UnityEngine.UI;

public class RedFlash : MonoBehaviour
{
    public float flashSpeed = 5f;

    [Range(0f, 1f)]
    public float maxAlpha = 0.3f;

    private Image image;
    private bool flashing;
    private float flashTime;

    private void Awake()
    {
        image = GetComponent<Image>();
        SetAlpha(0f);
    }

    private void Update()
    {
        if (!flashing) return;

        flashTime += Time.unscaledDeltaTime;

        float alpha =
            (1f - Mathf.Cos(flashTime * flashSpeed)) * 0.5f;

        SetAlpha(alpha * maxAlpha);
    }

    public void SetFlashing(bool active)
    {
        if (flashing == active) return;

        flashing = active;
        flashTime = 0f;
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}