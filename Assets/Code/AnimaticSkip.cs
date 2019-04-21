using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class AnimaticSkip : MonoBehaviour
{
    //lengte van animatie clips
    public float clipTime = 5;
    //het compoment met de playable director
    private PlayableDirector playableDirector;

    void Start()
    {
        playableDirector = FindObjectOfType<PlayableDirector>();
    }
    
    void Update()
    {
        //als spatie ingdrukt wordt, of de met de muis wordt geklikt/scherm getikt, skipt de player naar volgende clip in de animatic
        if(Input.GetButtonDown("Skip")){
            Debug.Log(playableDirector.time);
            playableDirector.time += clipTime - (playableDirector.time % clipTime);
        }

        // wanneer hij stilstaat en de tijd 0 is (gereset is, dus), lijkt hij klaar met spelen.
        if (playableDirector.state == 0 && playableDirector.time == 0)
            // Door naar de volgende scene, dus!
            SceneManager.LoadScene("MainScene");
    }
}