using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    // buttons from the skill tree
    public Button[] skillButtons;

    private SkillClass[] skills;   // the SkillClass component (.cs script) on each button
    // serialized so it can be set from the editor
    [SerializeField] private int researching;   // Skill-ID of the skill being researched

    private ResourceManager gameManager;   // to get the research-amounts from

    // amount of researched skills counting towards the progress
    private float winProgress = 0;
    // the amount of aforementioned skills needed to win
    public float winPoint = 6;
    // the scene name that tells the player they won
    public string winScene = "SceneWon";

    public BarBehaviour winProgressBar;
    public Text winProgressText;
    public BarBehaviour researchBar;
    public NotificationBehaviour notification;

    private MainSoundManagement musicPlayer;

    // Start is called before the first frame update
    void Start()
    {
        // we'll have as many skills as there are buttons for it (hopefully)
        skills = new SkillClass[skillButtons.Length];

        // now actually copy those classes for easy access
        for (int s = 0; s < skillButtons.Length; s++)
        {
            skills[s] = skillButtons[s].GetComponent<SkillClass>();

            // if the skill has not yet been unlocked
            if (skills[s].state == 0)
                // make the button in question un-clickable
                skillButtons[s].interactable = false;
        }

        // find the fist instatiated object's ResourceManager component (.cs script)
        gameManager = FindObjectOfType<ResourceManager>();

        // find the fist instatiated object's ResourceManager component (.cs script)
        musicPlayer = FindObjectOfType<MainSoundManagement>();
    }

    // Update is called once per frame
    void Update()
    {
        // if researching AT ALL (-1 = not researching)
        if (researching >= 0)
        {
            // (int) tries to shove the float in as if it's an int... 'cos I'm stupid.
            int i = (int)skills[researching].requirement[0];

            // put the proper amount of research into the skill, multiplied and all
            // "/60f" because it's on minute-basis
            float researchProgress = skills[researching].Research(gameManager.researches[i] * gameManager.researchMod[i] / 60f * Time.deltaTime);

            // scale the research bar to the % research done
            researchBar.Scale(researchProgress);

            // if 100% progressed (or more 'cos floats)
            if (researchProgress >= 1)
            {   // you only get here if the skill's done!

                // show mid-screen notification about it
                notification.Notify("Tech \"" + skills[researching].GetComponentInChildren<Text>().text + "\" is fully researched!", false);

                // Make the music one step techier
                musicPlayer.ChangeMusicTech(-(100f / (skills.Length - 1)));

                // play out skill effects
                if (skills[researching].Finish())
                    // if it actually happened; progress 1 towards winning
                    StartCoroutine(WinProgress(1));

                // make the button of the just-researched skill un-clickable again...
                skillButtons[researching].interactable = false;

                // foreach locked skill the researched one should unlock
                foreach (int u in skills[researching].unlocks)
                {
                    // tell their classes they've been unlocked
                    skills[u].Unlock();

                    // make its button clickable again!
                    skillButtons[u].interactable = true;
                }

                // stop researching this thing
                researching = -1;
            }
        }
    }

    public void ResearchSkill(int i)
    {
        // set the other on hold
        if (researching >= 0 && researching <= skills.Length)
            skills[researching].Hold();

        // change the skill being research from now
        researching = i;
    }

    IEnumerator WinProgress(int add)
    {
        winProgress += add;

        winProgressBar.Scale(winProgress / winPoint);
        winProgressText.text = "Win Progress: " + winProgress + "/" + winPoint;

        if (winProgress >= winPoint)
        {
            // stop music from playing; if someone replays the music would be played twice
            musicPlayer.StopMusic();

            yield return new WaitForSeconds(2.6f);
            SceneManager.LoadScene(winScene);
        }
    }
}