using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void Load(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}