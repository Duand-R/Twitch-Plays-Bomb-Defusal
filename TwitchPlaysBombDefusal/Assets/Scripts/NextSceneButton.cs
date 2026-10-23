using UnityEngine;

public class NextSceneButton : MonoBehaviour
{
    public string nextSceneName;

    public void LoadNextScene()
    {
        SceneTransition.Load(nextSceneName);
    }
}