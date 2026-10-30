using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    private static SceneTransition instance;
    private Image blackImage;
    private float fadeTime = 1.5f;

    public static void Load(string sceneName)
    {
        if (instance != null) return;

        GameObject obj = new GameObject("SceneTransition");
        instance = obj.AddComponent<SceneTransition>();

        DontDestroyOnLoad(obj);

        instance.StartCoroutine(instance.Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        GameObject canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.SetParent(transform, false);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;

        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("BlackImage");
        imageObj.transform.SetParent(canvasObj.transform, false);

        blackImage = imageObj.AddComponent<Image>();
        blackImage.color = new Color(0f, 0f, 0f, 0f);

        RectTransform rect = blackImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        yield return Fade(0f, 1f);
        Time.timeScale = 1f;
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return Fade(1f, 0f);

        Destroy(gameObject);
    }

    private IEnumerator Fade(float from, float to)
    {
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;

            float alpha = Mathf.Lerp(from, to, timer / fadeTime);
            blackImage.color = new Color(0f, 0f, 0f, alpha);

            yield return null;
        }

        blackImage.color = new Color(0f, 0f, 0f, to);
    }
}