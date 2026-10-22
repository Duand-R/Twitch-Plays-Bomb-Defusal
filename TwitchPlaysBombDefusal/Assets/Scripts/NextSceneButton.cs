using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneButton : MonoBehaviour
{
    public string nextSceneName;

    public void LoadNextScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }
}