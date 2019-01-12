using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class scr_TextManager : MonoBehaviour
{

    public TextAsset script;
    public Text textArea;
    public GameObject button;
    public GameObject portrait;
    public Sprite[] portraitArray;

    public string[] tempArray;
    public string[,] scriptArray = new string[,] { };
    public string currentSentence;
    string sentenceHolder;
    string portraitIndex;

    int j = 0;
    int l = 0;
    int indexA = 0;
    int indexB = 0;


    //keeps track of when textbox is typing to see if player can skip through the text or not
    bool isTyping;

    public float delay;


    // Use this for initialization
    void Start()
    {
        button.SetActive(false);
        portrait.SetActive(false);
        tempArray = script.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);

        /* I don't know how to make a 2D array of unknown length so I just set the max index to 
         * the number of lines in the script
        */
        scriptArray = new string[tempArray.Length, tempArray.Length];

        /* cycles through the array generated from text and splits up each line into dialogue sequences
         * each dialogue sequence ends with the line "BREAK" and a new one starts
         * j is the index of the dialogue sequence, l is the index of the sentences within the dialogue sequence
         */
        for (int i = 0; i < tempArray.Length; i++)
        {
            if (tempArray[i] == "BREAK")
            {
                j++;
                l = 0;
            }
            else
            {
                //Debug.Log(j + ", " + l);
                scriptArray[j, l] = tempArray[i];
                //Debug.Log(scriptArray[j, l]);
                l++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //test function to call and test textbox, delete later
        if (Input.GetKey("z") && button.activeInHierarchy == false)
        {
            button.SetActive(true);
            portrait.SetActive(true);
            ShowTextbox(0, 0, delay);
        }
    }

    //starts text sequence
    //called when text should be shown on screen
    //takes location/index of script, assigns it to textArea, opens textbox so player can advance script
    public void ShowTextbox(int indexC, int indexD, float delayInput)
    {
        indexA = indexC;
        indexB = indexD;
        delay = delayInput;

        SetSentence();
        StartCoroutine(StartTyping(delay));
    }

    //advances to next line in script
    //called whenever textbox is clicked
    public void NextLine()
    {
        //check if the coroutine is still running if the textbox is clicked, if so, "skip" the typing animation
        if (isTyping)
        {
            isTyping = false;
            textArea.text = currentSentence;
        }
        else
        {
            //check if the last line in script is being read, if so, close textbox
            /*
             * this is done by checking if the next line is null because idk how to intialize a 2D array
             * with a variable length so I just made the row and col have the same # of elements as the
             * initial string because it'll never result in an out of bounds exception.
             * Make this cleaner later.
            */
            if (scriptArray[indexA, indexB + 1] == null)
            {
                //remove the textbox, the current dialogue is finished
                button.SetActive(false);
                portrait.SetActive(false);
            }
            else
            {
                //go to next line in script
                indexB++;

                //set currentSentence to next line in script
                SetSentence();
                StartCoroutine(StartTyping(delay));
            }
        }


    }

    /* function to check if portrait index is given within sentence
     * if there is a portrait index, truncate it from the rest of the text 
     * after, set the truncated text to be the current sentence
     */
    public void SetSentence()
    {
        currentSentence = scriptArray[indexA, indexB];
        //check if portrait index is given in script
        if (currentSentence.Contains("["))
        {
            //if so, extract the portrait index from the script
            portraitIndex = currentSentence.Substring(1, currentSentence.IndexOf("]") - 1);
            //truncate currentSentence so that it begins after the portrait index
            currentSentence = currentSentence.Substring(currentSentence.IndexOf("]") + 1);

            //switch case for portrait index
            switch (System.Int32.Parse(portraitIndex))
            {
                //to do: find out better way to assign sprites, getting the image component every time is very messy
                //to do: find way to assign sprites to array automatically, allow for more than 5 portraits
                case 0:
                    portrait.GetComponent<Image>().sprite = portraitArray[0];
                    break;
                case 1:
                    portrait.GetComponent<Image>().sprite = portraitArray[1];
                    break;
                case 2:
                    portrait.GetComponent<Image>().sprite = portraitArray[2];
                    break;
                case 3:
                    portrait.GetComponent<Image>().sprite = portraitArray[3];
                    break;
                case 4:
                    portrait.GetComponent<Image>().sprite = portraitArray[4];
                    break;
                default:
                    break;
            }
        }
    }

    /*makes each character in currentSentence visible
     * call whenever you assign a value to the textbox because they all start off as invisible
     */
    IEnumerator StartTyping(float delay)
    {
        isTyping = true;

        for (int i = 0; i <= currentSentence.Length; i++)
        {
            /* changes each character in the current sentence to be visible one at a time, while keeping the rest 
             * of the sentence invisible. This makes it so the entire sentence is typed out beforehand, preventing
             * line skipping while typing.
             */
            if (isTyping)
            {
                sentenceHolder = "<color=#000000>" + currentSentence.Substring(0, i) + "</color><color=#FFFFFF00>" + currentSentence.Substring(i) + "</color>";
                textArea.text = sentenceHolder;

                yield return new WaitForSeconds(delay);
            }

        }

        isTyping = false;
    }


}

