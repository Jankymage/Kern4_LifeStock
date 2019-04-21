using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skip : MonoBehaviour
{
    public Image checkBox;
    public Sprite checkOff;
    public Sprite checkOn;

    public bool defaultIsSkip;

    // Start is called before the first frame update
    void Start()
    {
        // if the setting doesn't exist
        if (!PlayerPrefs.HasKey("skip"))
        {
            // set it to the default value
            PlayerPrefs.SetInt("skip", defaultIsSkip?1:0);

            // set the checkbox to "off"
            checkBox.sprite = checkOff;
        }
        else   // if the setting DOES exist
        {
            // if not skipping
            if (PlayerPrefs.GetInt("skip") == 0)
                // set checkbox to "off"
                checkBox.sprite = checkOff;
            else   // if ARE skipping
                // set checkbox to "on"
                checkBox.sprite = checkOn;
        }
    }

    public void Switch()
    {
        // if not skipping now
        if (PlayerPrefs.GetInt("skip") == 0)
        {
            // set to skipping
            PlayerPrefs.SetInt("skip", 1);

            // change checkbox accordingly
            checkBox.sprite = checkOn;
        }
        else   // if ARE skipping now
        {
            // set to not skipping
            PlayerPrefs.SetInt("skip", 0);


            // change checkbox accordingly
            checkBox.sprite = checkOff;
        }
    }
}
