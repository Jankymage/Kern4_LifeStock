using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public string intro = "IntroAnimation";
    public string main = "MainScene";

    public void StartIt()
    {
        if (PlayerPrefs.GetInt("skip") == 0)
            GetComponent<SceneLoader>().Load(intro);
        else
            GetComponent<SceneLoader>().Load(main);
    }
}
