using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldNoteDisplay : MonoBehaviour
{
    public GameObject skipButton;
    public GameObject[] buttons; 
    public Text promptText;
    public GameObject instructionsText;
    public RawImage image;
    public Texture picturePrompt; 

    public void setQuestion(string text, string[] options, bool skippable)
    {
        image.gameObject.SetActive(false); 
        promptText.text = text;
        instructionsText.SetActive(true); 
        for (int i = 1; i <= options.Length; i++)
        {
            buttons[i-1].SetActive(true);
            buttons[i - 1].GetComponentInChildren<Text>().text = options[i - 1];
            //buttons[i - 1].GetComponentInChildren<SelectableObj>().resetCommand(options[i - 1]); 
        }
        for (int j = options.Length + 1; j <= 6; j++)
        {
            buttons[j - 1].SetActive(false); 
        }
        if (skippable)
        {
            skipButton.SetActive(true); 
        } else
        {
            skipButton.SetActive(false); 
        }
    }

    public void displayFinalQuestion()
    {
        skipButton.SetActive(false);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(false); 
        }
        instructionsText.SetActive(false); 
        promptText.text = "Field note recorded!"; 
    }

    public void displayPicturePrompt()
    {
        image.gameObject.SetActive(true);
        image.texture = picturePrompt; 
        skipButton.SetActive(false);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(false);
        }
        instructionsText.SetActive(false);
        promptText.text = "Take a picture of the sample using \"Adele Capture\"";
    }

    public Texture getPicture()
    {
        return image.texture; 
    }
}
