using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OverlayInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // the mouse overlay object
    public GameObject overlay;

    // text to display bold at the top of the overlay window
    public string titleText;
    // min 0 max 4 icons and short texts for parameters, costs and such
    public Sprite[] icons;
    public string[] values;
    // non-bold text to display under the icons and their texts with <br> for breaks and {n} (replace n with an integer) for arguments
    public string bodyText;
    // replaces the nth {n} in bodyText
    public float[] args;

    // actual text to display (with working linebreaks and such)
    private string parsedText;
    // wether to update the hover text or not
    private bool hover = false;
    // for own reference; how many icons and/or respective texts there are to scale bodyText with
    private int valueCount;
    
    void Start()
    {
        // parse <br> into actual working linebreaks (no, typing "\n" in the editor does not add a working linebreak)
        parsedText = bodyText.Replace("<br>", "\n");

        // for further reference; to scale bodyText with
        valueCount = Mathf.Max(icons.Length, values.Length);
    }

    void Update()
    {
        if (hover)
        {
            string displayText = parsedText;

            // parse all {n} arguments to realtime updated values
            for (int r = 0; r < args.Length; r++)
                displayText = displayText.Replace("{" + r + "}", args[r].ToString());

            // set the bodyText with its realtime arguments
            overlay.GetComponentsInChildren<Text>()[1].text = displayText;
        }
    }

    virtual public void OnPointerEnter(PointerEventData pointerEventData)
    {
        // turn the overlay on
        overlay.SetActive(true);
        // avoid button-in-the-way bug
        overlay.GetComponent<MouseOverlay>().ResetPosition();

        // tell yourself the hover text needs to be updated!
        hover = true;
        
        Image[] images = overlay.GetComponentsInChildren<Image>(true);
        Text[] texts = overlay.GetComponentsInChildren<Text>(true);

        // set the title
        texts[0].text = titleText;

        for (int i = 1; i < 5; i++)
            images[i].gameObject.SetActive(false);
        for (int t = 2; t < 6; t++)
            texts[t].gameObject.SetActive(false);

        int id = 1;
        foreach (Sprite i in icons)
        {
            images[id].gameObject.SetActive(true);
            images[id].sprite = i;

            id++;
        }

        id = 2;
        foreach (string v in values)
        {
            texts[id].gameObject.SetActive(true);
            texts[id].text = v;

            id++;
        }

        // scale body text to available area
        texts[1].rectTransform.sizeDelta = new Vector2(texts[1].rectTransform.sizeDelta.x, 20 + 40 * (4 - valueCount));
    }

    // when not hovering this thing
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        // stop changing the text, you're no longer relevant
        hover = false;

        // hide overlay
        overlay.SetActive(false);
    }
}