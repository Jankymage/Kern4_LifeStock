using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToSender : MonoBehaviour
{
    private Transform sendersGaze;

    // Start is called before the first frame update
    void Start()
    {
        // get the transform of the camera to look at
        sendersGaze = GameObject.FindGameObjectWithTag("MainCamera").transform;

        // "LookAt" makes us 'look at the thing' with our back..? So mirror yourself to show ourselves nicely!
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        // do the turning thing :D
        transform.LookAt(sendersGaze.position);
    }
}
