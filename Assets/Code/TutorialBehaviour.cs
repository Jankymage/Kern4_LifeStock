using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBehaviour : MonoBehaviour
{
    public GameObject[] popUps;
    //public MultiDimensionalInspectorArray[] cR;
    float[,] cR;
    int cur = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("tutorial") || PlayerPrefs.GetInt("skip") == 0)
        {
            popUps[cur].SetActive(true);

            float[,] initializer =
            {
                // Research
                { .809f, .993f, .013f, .321f },
                { .758f, .887f, .662f, .719f },
                { .749f, .801f, .324f, .363f },
                { .523f, .574f, .546f, .585f },
                // Power
                { .674f, .734f, .013f, .120f },
                {    0 ,    1 ,    0 ,    1  },
                { .287f, .338f, .616f, .652f },
                // Mineral
                { .740f, .800f, .013f, .120f },
                {    0 ,    1 ,    0 ,    1  },
                { .412f, .463f, .582f, .619f },
                // Pods
                { .541f, .670f, .013f, .120f },
                {    0 ,    1 ,    0 ,    1  },
                { .643f, .694f, .594f, .633f }
            };

            cR = initializer;
        }
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // print("X: " + System.Math.Round(Input.mousePosition.x / Screen.width, 3));
        // print("Y: " + System.Math.Round(Input.mousePosition.y / Screen.height, 3));

        if (!PlayerPrefs.HasKey("tutorial") || PlayerPrefs.GetInt("skip") == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                float x = Input.mousePosition.x / Screen.width;
                float y = Input.mousePosition.y / Screen.height;

                //if (x > cR[cur].f[0] && x < cR[cur].f[1] && y > cR[cur].f[2] && y < cR[cur].f[3])
                if (x > cR[cur, 0] && x < cR[cur, 1] && y > cR[cur, 2] && y < cR[cur, 3])
                {
                    popUps[cur].SetActive(false);

                    cur++;

                    if (cur < popUps.Length)
                        popUps[cur].SetActive(true);
                    else
                    {
                        PlayerPrefs.SetInt("tutorial", 1);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    public void ClickedOK(){
        popUps[cur].SetActive(false);

        cur++;

        if (cur < popUps.Length)
            popUps[cur].SetActive(true);
        else
        {
            PlayerPrefs.SetInt("tutorial", 1);
            Destroy(gameObject);
        }
    }
}
