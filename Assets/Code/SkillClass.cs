using UnityEngine;
using UnityEngine.UI;

public class SkillClass : MonoBehaviour
{
    public int skillID = -1;   // which skill it is, for resolving later on
    public float[] requirement = { 0, 2000, 2000 };   // [0]=type of research, [1]=amount of research, [2]=max amount
    public int state = 1;   // 0=locked / cannot be done, 1=unlocked / can be done, 2=researched / done
    public int[] unlocks;   // which techs it unlocks in the tech tree

    public Image stateIcon;
    public Sprite[] stateSources;

    public bool Unlock()
    {
        // show icon we're unlocked
        stateIcon.sprite = stateSources[0];
        stateIcon.color = Color.yellow;

        // if not unlocked yet...
        if (state < 1)
        {
            // unlock self
            state = 1;

            // and tell whoever called this function that yes, this skill has now been unlocked!
            return true;
        }
        else   // so it's already unlocked...
            // tell tell whoever called this function that no changes were made, the skill has already been unlocked...
            return false;
    }
    public bool Finish()
    {
        // show icon we're finished
        stateIcon.sprite = stateSources[3];
        stateIcon.color = Color.green;

        // if not finished yet...
        if (state < 2)
        {
            // play out effects
            switch (skillID)
            {
                case 0:   // Un-Human intervetion
                    FindObjectOfType<ResourceManager>().ChangeAllMods(.25f);
                    break;

                case 1:   // What The People Want
                    FindObjectOfType<ResourceManager>().ChangeAllMods(.1f);
                    break;

                case 2:   // Some Are More Equal Than Others
                    FindObjectOfType<ResourceManager>().ChangeMod(1, .5f);
                    break;

                case 3:   // Twenty-Four Karats
                    FindObjectOfType<ResourceManager>().mineTimeMod += .5f;
                    break;

                case 4:   // Toxium Carbonate Plants
                    FindObjectOfType<ResourceManager>().AdjustPowerMod(.15f);
                    break;

                default:
                    Debug.LogError("skillID not set!");
                    break;
            }

            // finish self
            state = 2;

            // play the sound
            FMODUnity.RuntimeManager.PlayOneShot("event:/Interaction/Progression");

            // and tell whoever called this function that yes, this skill has now been finished!
            return true;
        }
        else   // so it's already finished...
            // tell tell whoever called this function that no changes were made, the skill has already been finished...
            return false;
    }

    public void Hold()
    {
        if (state == 1)   // if unlocked (but not finished)
        {
            stateIcon.sprite = stateSources[2];
            stateIcon.color = Color.blue;
        }
    }

    public float Research(float amount)
    {
        // if still not done with it...
        if (requirement[1] > amount)
        {
            // substract that bit from it
            requirement[1] -= amount;

            // make sure icon stays "on research"
            if (stateIcon.sprite != stateSources[1] || stateIcon.color != Color.cyan)
            {
                stateIcon.sprite = stateSources[1];
                stateIcon.color = Color.cyan;
            }

            // tell whoever called this function how much % has been researched...
            return (requirement[2] - requirement[1]) / requirement[2];
        }
        else   // so it's done with it...
            // tell whoever called this function that 100% has been researched!
            return 1;
    }
}