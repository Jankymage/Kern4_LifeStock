using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    // UI.Texts to display resources in
    public Text powerText;
    public Text mineralText;
    public Text[] researchTexts;
    // needed for the tech "Carbonate Power Plants"
    public float powerMod = 1f;

    // OverInfo components (.cs scripts) to show a verbose info overlay on mouse hover
    public OverlayInfo powerInfo;
    public OverlayInfo mineralInfo;
    public OverlayInfo[] researchInfos;
    // gameobject to show things like "Not enough Power!" around the mouse when that's the case
    public GameObject errorMessage;
    // prefab to spawn to indicate pods died
    public GameObject deadText;
    // gameobject to display # of deaths on, wether the player is looking or not
    public NotificationBehaviour notification;

    // resources
    private int currentPower, powerTreshold, currentMineral, mineralTreshold;
    // how much of each can be researched per minute
    public int[] researches;
    // multiplier of how much each type researches
    public float[] researchMod;
    // multiplier for how long miners stay
    public float mineTimeMod;

    // Start is called before the first frame update
    void Start()
    {
        // we're going to have a Modifier for each research type
        researchMod = new float[researches.Length];
        // and they're going to be ×1 by default
        for (int m = 0; m < researches.Length; m++)
            researchMod[m] = 1;
    }

    // public function that only ASKS wether there's enough resources
    // (this is because of the pay-but-don't-place bug from earlier)
    public bool CanPay(int ptAmount, int mtAmount)
    {
        // if the current power is lower than the treshold+power pay amount and not clicking the UI
        if (currentPower < powerTreshold + ptAmount && !EventSystem.current.IsPointerOverGameObject())
        {
            // error overlay close to mouse displays text
            errorMessage.GetComponentInChildren<Text>().text = "Need more Power!";
            // and does its job
            errorMessage.GetComponent<FadeOut>().FadeNow();

            // play the notification-sound to notify the player of the error
            FMODUnity.RuntimeManager.PlayOneShot("event:/Interaction/Notification");

            // tell whoever called the function that no, it can't be paid...
            return false;
        }
        else if (currentMineral < mineralTreshold + mtAmount && !EventSystem.current.IsPointerOverGameObject())   // same but for mineral
        {
            // error overlay close to mouse displays text
            errorMessage.GetComponentInChildren<Text>().text = "Need more Mineral!";
            // and does its job
            errorMessage.GetComponent<FadeOut>().FadeNow();

            // play the notification-sound to notify the player of the error
            FMODUnity.RuntimeManager.PlayOneShot("event:/Interaction/Notification");

            // tell whoever called the function that no, it can't be paid...
            return false;
        }
        else   // not short on power nor mineral?
        {
            // hide error message (no longer relevant)
            errorMessage.GetComponent<FadeOut>().StopFading();

            // tell whoever called the function that yes, it can be paid!
            return true;
        }
    }
    

    // =============== CHANGE PARAMETERS =================== \\
    public bool AdjustCurrentPower(int add)
    {
        // if the amount is a positive value
        if (add > 0)
            // multiply it my the powerModifier
            add = Mathf.RoundToInt(add * powerMod);

        // in case power goes down; it shouldn't "cause a blackout"
        if (currentPower + add > powerTreshold)
        {
            // adjust power
            currentPower += add;
            // overlay info's first argument now displays the correct amount of power
            powerInfo.args[0] = currentPower;

            // UI.Text now displays the correct amount of 'power left'
            UpdatePowerText();

            // tell whoever called the function that yes, the power has been updated!
            return true;
        }
        else
            // tell whoever called the function that no, the power couldn't go down that much...
            return false;
    }
    public bool AdjustPowerTreshold(int add)
    {
        // in case the treshold goes up; it shouldn't "cause a blackout"
        if (powerTreshold + add <= currentPower)
        {
            // adjust the treshold
            powerTreshold += add;
            // overlay info's second argument now displays the correct treshold
            powerInfo.args[1] = powerTreshold;

            // UI.Text now displays the correct amount of 'power left'
            UpdatePowerText();

            // tell whoever called the function that yes, the treshold has been updated!
            return true;
        }
        else
            // tell whoever called the function that no, the treshold couldn't go up that much...
            return false;
    }
    private void UpdatePowerText()
    {
        // UI.Text.text = 'how much power left'
        powerText.text = (currentPower - powerTreshold).ToString();
    }
    public bool AdjustPowerMod(float change)
    {
        // adjust all power so far
        if (AdjustCurrentPower(Mathf.RoundToInt(currentPower * change)))
        {
            // if that succeeds, actually change the Modifier for further reference
            powerMod += change;
            // and tell whoever called this function that yes, the change has been made!
            return true;
        }
        else   // if that couldn't be possible (would be lower than the treshold)
            // tell whoever called this function that no, the change hasn't been made!
            return false;
    }

    public bool AdjustCurrentMineral(int add)
    {
        // to return
        bool nobodyDied = true;

        // adjust mineral
        currentMineral += add;
        // overlay info's first argument now displays the correct amount
        mineralInfo.args[0] = currentMineral;

        // PodHeads could've died if mineral went down
        if (add < 0)
        {
            // in case some are left without mineral now...
            if (currentMineral < mineralTreshold)
                nobodyDied = false;   // some are going to die, let that be known

            int killCount = 0;

            while (currentMineral < mineralTreshold)
            {
                // find the oldest
                GameObject eldest = GameObject.FindGameObjectsWithTag("Mortal")[killCount];

                // remove their research contribution
                ChangeResearch(eldest.GetComponent<GenerateResource>().resourceType - 3, -eldest.GetComponent<GenerateResource>().generatesAmount);

                // remove their need for power
                AdjustMineralTreshold(-1);

                // spawn a floating text at the victim's position telling they died
                deadText.GetComponent<FloatUpward>().SetText("Died", new Color(.7843137f, .7960784f, .9137255f));
                Instantiate(deadText, eldest.transform.position, new Quaternion());

                // actually remove the victim
                Destroy(eldest.gameObject);
                killCount++;
            }   // reverb, resound, and repeat

            // now show this in the notification
            if (!nobodyDied)
                notification.Notify(killCount + " PodHeads have been unplugged and killed due to a mineral shortage.\nPlease keep your miners in check!", true);
        }

        // UI.Text now displays the correct amount of 'mineral left'
        UpdateMineralText();

        // tell whoever called the function wether PodHeads were harmed in the process
        return nobodyDied;
    }
    public bool AdjustMineralTreshold(int add)
    {
        // in case the treshold goes up; no Pod should spawn without minerals
        if (add < 0 || mineralTreshold + add <= currentMineral)
        {
            // adjust the treshold
            mineralTreshold += add;
            // overlay info's second argument now displays the correct treshold
            mineralInfo.args[1] = mineralTreshold;

            // UI.Text now displays the correct amount of 'mineral left'
            UpdateMineralText();

            // tell whoever called the function that yes, the treshold has been updated!
            return true;
        }
        else
            // tell whoever called the function that no, the treshold couldn't go down that much...
            return false;
    }
    private void UpdateMineralText()
    {
        // UI.Text.text = 'how much power left'
        mineralText.text = (currentMineral - mineralTreshold).ToString();
    }

    public bool ChangeResearch(int type, int add)
    {
        // never go full retard
        if (researches[type] + add >= 0)
        {
            // adjust research per minute
            researches[type] += add;

            // show the updated info to the player
            UpdateResearchTexts();

            // tell whoever called the function that yes, the research per minute has been updated!
            return true;
        }
        else
            // tell whoever called the function that no, the research per minute couldn't go down that much...
            return false;
    }
    public void ChangeMod(int id, float add)
    {
        researchMod[id] += add;
        UpdateResearchTexts();
    }
    public void ChangeAllMods(float add)
    {
        for (int m = 0; m < researchMod.Length; m++)
        {
            researchMod[m] += add;
        }

        UpdateResearchTexts();
    }
    private void UpdateResearchTexts()
    {
        for (int r = 0; r < researches.Length; r++)
        {
            // change verbose info to show correct amount of research gained from both sources
            researchInfos[r].args[0] = researches[r];
            researchInfos[r].args[1] = researches[r] * (researchMod[r] - 1);

            // update text to show total research
            researchTexts[r].text = (researches[r] * researchMod[r]).ToString();
        }
    }
}
