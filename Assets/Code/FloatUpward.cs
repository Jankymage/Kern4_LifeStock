using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatUpward : MonoBehaviour
{
    public float targetHeight;   // where to stop
    public float seconds;   // when to stop
    private float floatSpeed;   // for our own readability

    // Start is called before the first frame update
    void Start()
    {
        // user sets seconds in the inspector, but here speed is easier to use
        floatSpeed = targetHeight / seconds;

        // make sure the target height is relative to where we are now
        targetHeight += transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // go upward
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        // if at target height or past it
        if (transform.position.y >= targetHeight)
            // remove yourself
            Destroy(gameObject);
    }

    public void SetText(string t, Color c)
    {
        GetComponentInChildren<TextMeshPro>().text = t;
        GetComponentInChildren<TextMeshPro>().color = c;
    }
}
