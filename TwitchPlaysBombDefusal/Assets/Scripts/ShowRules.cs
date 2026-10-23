using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowRules : MonoBehaviour
{
    public GameObject[] ruleImages;
    public string nextSceneName;

    public float delay = 3f;

    private IEnumerator Start()
    {
        foreach (GameObject image in ruleImages)
        {
            image.SetActive(false);
        }

        for (int i = 0; i < ruleImages.Length; i++)
        {
            ruleImages[i].SetActive(true);

            if (i < ruleImages.Length - 1)
            {
                yield return new WaitForSecondsRealtime(delay);
            }
        }

        yield return new WaitForSecondsRealtime(5f);

        SceneTransition.Load(nextSceneName);
    }
}