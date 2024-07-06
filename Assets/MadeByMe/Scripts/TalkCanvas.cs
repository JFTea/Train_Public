using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

public class TalkCanvas : MonoBehaviour
{
    /// <summary>
    /// The sprite of the NPC the player is talking to
    /// </summary>
    [SerializeField]
    private Image npcSprite;

    /// <summary>
    /// What the NPC will say when asking for a ticket to the 2nd class carriage
    /// </summary>
    private string fancyText = "Please can I have a ticket for the 2nd class carriage.";

    /// <summary>
    /// What the NPC will say when asking for a ticket to the 3rd class carriage
    /// </summary>
    private string nonFancyText = "Please can I have a ticket for the 3rd class carriage.";

    /// <summary>
    /// What the player character will say to the NPC
    /// </summary>
    private string replyText = "Here you go.";

    /// <summary>
    /// The full list of possible lines an NPC could say
    /// </summary>
    private List<string> otherText = new List<string>();

    /// <summary>
    /// A list of lines an NPC getting on the 2nd class carriage could say
    /// </summary>
    private List<string> otherFancySpeech = new List<string>();

    /// <summary>
    /// A list of lines an NPC getting on the 3rd class carriage could say
    /// </summary>
    private List<string> otherNotFancySpeech = new List<string>();

    /// <summary>
    /// The speed multiplier for the text animation
    /// </summary>
    public float speedMultiplier = 1;

    /// <summary>
    /// The text UI element
    /// </summary>
    [SerializeField]
    private TMP_Text talkText;

    /// <summary>
    /// Checks if the talk canvas is currently in use
    /// </summary>
    private bool isTalking = false;

    /// <summary>
    /// Stores the current counter of the character in the current string
    /// </summary>
    private int charCounter = 0;

    /// <summary>
    /// The event that is triggered when the next is finished
    /// </summary>
    public UnityEvent finished = new UnityEvent();

    /// <summary>
    /// The submit button for a reply from the player
    /// </summary>
    [SerializeField]
    private Button submit;

    private void Start()
    {
        //Adds all text to the main list
        otherText.Add("I can't wait to try the new train route from Wadebridge to Bodmin that opened today!");
        otherText.Add("The new train is so loud, sometimes I cant sleep at night.");
        otherText.Add("I finally know the way to the train station!");

        otherText.Add("I need to pack my umbrella just in case it rains. The third class carrage does not have a roof!");
        otherText.Add("I heard that the thrid class carrage used to carry sand.");

        //Adds 2nd class carriage text to the 2nd class list
        otherFancySpeech.Add("I can't wait to try the new train route from Wadebridge to Bodmin that opened today!");
        otherFancySpeech.Add("The new train is so loud, sometimes I cant sleep at night.");
        otherFancySpeech.Add("I finally know the way to the train station!");

        //Adds 3rd class carriage text to the 3rd class list
        otherNotFancySpeech.Add("I need to pack my umbrella just in case it rains. The third class carrage does not have a roof!");
        otherNotFancySpeech.Add("I heard that the thrid class carrage used to carry sand.");
    }

    /// <summary>
    /// Sets the speed of the text animation
    /// </summary>
    public void SetTextSpeed()
    {
        if(speedMultiplier == 1)
        {
            speedMultiplier = 0.5f;
        }
        else
        {
            speedMultiplier = 1;
        }
    }

    /// <summary>
    /// Starts text talking
    /// </summary>
    /// <param name="npc">The NPC the player is talking to</param>
    public void TStart(GameObject npc)
    {
        List<string> list = new List<string>();
        npcSprite.sprite = npc.GetComponent<SpriteRenderer>().sprite;
        switch (npc.GetComponent<NPC>().targetCarrage)
        {
            case "Fancy":
                //Creates the list of dialogue options for this NPC, including asking for a ticket
                list.Add(otherFancySpeech[Random.Range(0, otherFancySpeech.Count)]);
                list.Add(fancyText);
                SetupText(list);
                break;
            case "NotFancy":
                //Creates the list of dialogue options for this NPC, including asking for a ticket
                list.Add(otherNotFancySpeech[Random.Range(0, otherNotFancySpeech.Count)]);
                list.Add(nonFancyText);
                SetupText(list);
                break;
            case "N/A":
                //Creates the list of dialogue options for this NPC, without asking for a ticket
                list.Add(otherText[Random.Range(0, otherText.Count)]);
                SetupText(list);
                break;
        }
    }

    /// <summary>
    /// Sets up the canvas for the conversation
    /// </summary>
    /// <param name="text"></param>
    private void SetupText(List<string> text)
    {
        GetComponent<Canvas>().enabled = true;
        isTalking = true;
        StartCoroutine(DisplayText(text));
    }

    /// <summary>
    /// Sets the players response to the NPC
    /// </summary>
    public void OnReply()
    {
        //Creates the list of possible player responses
        List<string> list = new List<string>();
        list.Add(replyText);

        talkText.text = "";
        submit.interactable = false;
        talkText.alignment = TextAlignmentOptions.TopRight;
        SetupText(list);
        StartCoroutine(finishedReply());
    }

    /// <summary>
    /// Resets the canvas once the player has replied
    /// </summary>
    /// <returns></returns>
    private IEnumerator finishedReply()
    {
        //Waits until the reply has stopped
        yield return new WaitUntil(() => !isTalking);
        talkText.alignment = TextAlignmentOptions.TopLeft;
        finished.Invoke();
        GetComponent<Canvas>().enabled = false;
    }

    //Cited from https://docs.unity3d.com/ScriptReference/WaitForSeconds.html
    //Author Unity Technologies
    //Date 28/03/2023
    IEnumerator DisplayText(List<string> text)
    {
        string currentText = "";
        //Gets each line then prints it out character by character after a small delay based on the speed multiplier
        foreach(string sentence in text)
        {
            talkText.text = "";
            charCounter = 0;
            while (charCounter < sentence.Length)
            {
                talkText.text += sentence[charCounter];
                yield return new WaitForSeconds(0.1f * speedMultiplier);
                charCounter++;
            }

            yield return new WaitForSeconds(0.2f * speedMultiplier);
            currentText = sentence;
        }

        //If the NPC is not asking for a ticket it resets the talking canvas without a player reply
        if (text.Count == 1)
        {
            submit.enabled = false;
            submit.GetComponent<Image>().enabled = false;
            submit.interactable = false;
            submit.GetComponentInChildren<TMP_Text>().enabled = false;
            isTalking = false;
            charCounter = 0;
            talkText.alignment = TextAlignmentOptions.TopLeft;
            yield return new WaitForSeconds(0.2f);
            GetComponent<Canvas>().enabled = false;
            finished.Invoke();
        }

        //If a player reply is needed the button is set up, otherwise the conversation ends
        if (charCounter > currentText.Length - 1 && text.Count > 1)
        {
            isTalking = false;
            charCounter = 0;
            //Checks to see if the player can reply
            if (!submit.enabled)
            {
                submit.enabled = true;
                submit.GetComponent<Image>().enabled = true;
                submit.interactable = true;
                submit.GetComponentInChildren<TMP_Text>().enabled = true;
            }
            else
            {
                submit.enabled = false;
                submit.GetComponent<Image>().enabled = false;
                submit.interactable = false;
                submit.GetComponentInChildren<TMP_Text>().enabled = false;
                talkText.alignment = TextAlignmentOptions.TopLeft;
            }
        }
    }
}
