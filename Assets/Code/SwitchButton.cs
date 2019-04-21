using UnityEngine;

public class SwitchButton : MonoBehaviour
{
    public GameObject thingToSwitch;
    public string optionalHotKey = "Open Tech Tree";

    void Update()
    {
        if (Input.GetButtonDown(optionalHotKey))
            SwitchTheThing();
    }

    public void SwitchTheThing()
    {
        // if thing is not active
        if (!thingToSwitch.activeInHierarchy)
            // make thing active
            thingToSwitch.SetActive(true);
        else   // if thing is active
            // make thing not active
            thingToSwitch.SetActive(false);
    }
}
