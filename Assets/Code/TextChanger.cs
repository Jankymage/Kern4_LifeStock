using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChanger : MonoBehaviour
{
    private Text[] GetText;
     public Font myFont;
 
     // Use this for initialization
     void Start () {
         GetText = Resources.FindObjectsOfTypeAll<Text> ();
 
         foreach (Text go in GetText)
             go.font = myFont;
     }
}
