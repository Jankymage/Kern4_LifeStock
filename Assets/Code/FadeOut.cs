using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public float inSeconds;

    private bool fadeNow = false;
    private float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().color = new Color(0, 0, 0, 0);
        GetComponentInChildren<Text>().color = new Color(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeNow)
        {
            if (currentTime > 0)
            {
                transform.position = Input.mousePosition;

                currentTime -= Time.deltaTime;
                if (currentTime < 0)
                    currentTime = 0;

                float cur = currentTime / inSeconds;

                GetComponent<Image>().color = new Color(0, 0, 0, cur/2);
                GetComponentInChildren<Text>().color = new Color(1, 0, 0, cur);

                if (currentTime == 0)
                    fadeNow = false;
            }
        }
    }

    public void FadeNow()
    {
        fadeNow = true;
        currentTime = Time.deltaTime + inSeconds;

        GetComponent<Image>().color = new Color(0, 0, 0, .5f);
        GetComponentInChildren<Text>().color = new Color(1, 0, 0, 1);
    }

    public void StopFading()
    {
        currentTime = 0;

        GetComponent<Image>().color = new Color(0, 0, 0, 0);
        GetComponentInChildren<Text>().color = new Color(0, 0, 0, 0);

        fadeNow = false;
    }

    public bool IsFading()
    {
        return fadeNow;
    }
}
