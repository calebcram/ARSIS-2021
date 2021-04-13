//using Org.BouncyCastle.Asn1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages field notes data.  
/// </summary>
public class FieldNotesManager : MonoBehaviour
{
    public Question firstQuestion;
    public Question currentQuestion; 
    public static FieldNotesManager s;

    private string selectedAnswer = ""; 

    int currentSelectionIndex = -1;

    public bool inProgress = false;

    public ResponseRepository savedResponses;

    private bool waitingForPicture = false;
    private bool waitingForRecording = false; 

    //public RawImage test;

    void Start()
    {
        s = this;
        waitingForPicture = false;

        /* Question first = new Question("What is the type of the sample?", new string[] { "rock", "regolith" });

         Question genSize = new Question("General size", new string[] { "small", "medium", "large" }); 
         Question genColor = new Question("Color/tone", new string[] { "gray", "red", "black", "light-toned" });
         Question otherColor = new Question("Other color descriptor", new string[] { "banded", "streaked", "flat", "shiny" });
         Question texture = new Question("Texture", new string[] { "fine", "medium", "coarse" });
         Question otherTexture = new Question("Other texture descriptor", new string[] { "vesicular", "glassy", "metallic" });
         Question durability = new Question("Durability", new string[] { "hard to break", "crumbles" });

         genSize.setNextQuestion(genColor);
         genColor.setNextQuestion(otherColor);
         otherColor.setNextQuestion(texture);
         otherTexture.setNextQuestion(durability); 

         Question genColorReg = new Question("Color/tone", new string[] { "gray", "red", "black", "light-toned" });
         Question otherColorReg = new Question("Other color descriptor", new string[] { "banded", "streaked", "flat", "shiny" });
         Question grainSize = new Question("Grain size", new string[] { "fine", "medium", "coarse" });
         Question grainType = new Question("Grain type", new string[] { "clay", "silt", "sand", "gravel" });

         genColorReg.setNextQuestion(otherColorReg);
         otherColorReg.setNextQuestion(grainSize); 

         first.setNextQuestions(new Question[] { genColor, genColorReg }); */

        Question first = new Question("Color/tone", new string[] { "gray", "red", "black", "light-toned" });
        Question otherColor = new Question("Other color descriptor", new string[] { "banded", "streaked", "flat", "shiny" });
        Question grainSize = new Question("Grain size", new string[] { "fine", "medium", "coarse" });
        Question otherTexture = new Question("Other texture descriptor", new string[] { "vesicular", "glassy", "metallic" });
        Question durability = new Question("Durability", new string[] { "hard to break", "crumbles" });

        first.setNextQuestion(otherColor);
        otherColor.setNextQuestion(grainSize);
        grainSize.setNextQuestion(otherTexture);
        otherTexture.setNextQuestion(durability); 

        firstQuestion = first;
        currentQuestion = first;

        savedResponses = new ResponseRepository();

        LoadFile();

        VoiceManager.S.captureEvent += confirmationMessage;  
    }

    public void startFieldNote()
    {
        inProgress = true;
        waitingForPicture = false;
        waitingForRecording = false; 

        //clear list and get response repo from file - to clear any incomplete entries 
        LoadFile();

        savedResponses.addResponse();

        //showAudioPrompt(); 
        showFirstQuestion();
    }

    public void showFirstQuestion()
    {
        currentSelectionIndex = -1;
        currentQuestion = firstQuestion;
        MenuController.s.m_newFieldNote.GetComponent<FieldNoteDisplay>().setQuestion(currentQuestion.prompt, currentQuestion.options, currentQuestion.variableNextQuestion());
    }

    public void nextQuestion()
    {
        waitingForPicture = false;
        if (selectedAnswer == "") return;
        savedResponses.responses[savedResponses.responses.Count-1].addEntry(currentQuestion.prompt, selectedAnswer);
        if (!currentQuestion.variableNextQuestion())
        {
            if (currentSelectionIndex > currentQuestion.nextQuestions.Length)
            {
                showAudioPrompt(); 
                return; 
            } else
            {
                currentQuestion = currentQuestion.nextQuestions[currentSelectionIndex];
            }
            
        } else
        {
            if (currentQuestion.nextQuestion == null)
            {
                showAudioPrompt(); 
                return; 
            } else
            {
                currentQuestion = currentQuestion.nextQuestion;
            }
            
        }
        selectedAnswer = "";
        currentSelectionIndex = -1; 
        MenuController.s.m_newFieldNote.GetComponent<FieldNoteDisplay>().setQuestion(currentQuestion.prompt, currentQuestion.options, currentQuestion.variableNextQuestion());
    }

    public void finalQuestion()
    {
        MenuController.s.m_newFieldNote.GetComponent<FieldNoteDisplay>().displayPicturePrompt();

        waitingForPicture = true; 

        // Note: at this point we wait for the capture delegate in VoiceManager to go to the next step, which is confirmationMessage() 
    }

    public void showAudioPrompt()
    {
        MenuController.s.m_newFieldNote.GetComponent<FieldNoteDisplay>().displayRecordButton(); 
    }

    public void toggleRecording()
    {
        if (!waitingForRecording)
        {
            waitingForRecording = true; 
            MenuController.s.m_newFieldNote.GetComponent<FieldNoteDisplay>().setRecordButtonText("End"); 
            MicrophoneRecord.S.startRecording();
        }
        else
        {
            AudioClip clip = MicrophoneRecord.S.stopRecording();
            savedResponses.responses[savedResponses.responses.Count - 1].audio = clip;
            waitingForRecording = false;
            finalQuestion();  
        }

    }

    public void confirmationMessage()
    {
        if (!waitingForPicture) return;

        Debug.Log("We got to the confirmation message");

        Texture picture = MenuController.s.m_newFieldNote.GetComponent<FieldNoteDisplay>().getPicture();
        //test.texture = picture;
        savedResponses.responses[savedResponses.responses.Count - 1].picture = picture;

        SaveFile();

        MenuController.s.m_newFieldNote.GetComponent<FieldNoteDisplay>().displayFinalQuestion();
        inProgress = false;
        waitingForPicture = false; 
    }

    public void selectFirstAnswer()
    {
        selectedAnswer = currentQuestion.options[0];
        currentSelectionIndex = 0; 
    }

    public void selectSecondAnswer()
    {
        selectedAnswer = currentQuestion.options[1];
        currentSelectionIndex = 1; 
    }

    public void selectThirdAnswer()
    {
        selectedAnswer = currentQuestion.options[2];
        currentSelectionIndex = 2; 
    }

    public void selectFourthAnswer()
    {
        selectedAnswer = currentQuestion.options[3];
        currentSelectionIndex = 3; 
    }

    public void selectFifthAnswer()
    {
        selectedAnswer = currentQuestion.options[4];
        currentSelectionIndex = 4; 
    }

    public void selectSixthAnswer()
    {
        selectedAnswer = currentQuestion.options[5];
        currentSelectionIndex = 5; 
    }

    public void setSkipped()
    {
        selectedAnswer = "skip"; 
    }

    public void showAllFieldNotes()
    {
        MenuController.s.m_fieldNotes.GetComponent<FieldNotesListDisplay>().showFieldNotes(savedResponses);
    }

    public void SaveFile()
    {
       /* string destination = Application.persistentDataPath + "/responserepo.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);        

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, savedResponses);
        file.Close();*/
    }

    public void LoadFile()
    {
        /*string destination = Application.persistentDataPath + "/responserepo.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            savedResponses = new ResponseRepository(); 
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        ResponseRepository data = (ResponseRepository)bf.Deserialize(file);
        file.Close();

        savedResponses = data; */
    }

    public void ClearAllSavedResponses()
    {
        string destination = Application.persistentDataPath + "/responserepo.dat";
        if (File.Exists(destination)) File.Delete(destination);
        LoadFile();
        savedResponses = new ResponseRepository();  
        MenuController.s.m_fieldNotes.GetComponent<FieldNotesListDisplay>().showFieldNotes(savedResponses);
    }
}

public class Question 
{
    [Header("Option")]
    public string prompt;
    public string[] options;
    public Question[] nextQuestions; // used for branching options - note that in this case the question will be non-skippable
    public Question nextQuestion; // used if next question is always the same 

    public Question(string text, string[] options)
    {
        this.prompt = text;
        this.options = options;
        nextQuestions = null;
        nextQuestion = null; 
    }

    public string getPrompt()
    {
        return prompt; 
    }

    public string getOption(int index)
    {
        if (options.Length > index)
        {
            return null; 
        }
        else
        {
            return options[index]; 
        }
    }

    public bool variableNextQuestion()
    {
        return nextQuestions == null; 
    }

    public void setNextQuestions(Question[] q)
    {
        nextQuestion = null; // only one should be set at a time 
        nextQuestions = q;
    }

    public void setNextQuestion(Question q)
    {
        nextQuestions = null; // only one should be set at a time 
        nextQuestion = q; 
    }
}

// Object structure for storage of field notes 
[System.Serializable] 
public class ResponseRepository
{
    public List<Response> responses;
    
    public ResponseRepository()
    {
        responses = new List<Response>(); 
    }

    public void addResponse()
    {
        responses.Add(new Response()); 
    }
}

[System.Serializable]
public class Response
{
    public DateTime date; 
    public List<Entry> entries;
    public Texture picture;
    public AudioClip audio; 

    public Response()
    {
        date = DateTime.Now;
        entries = new List<Entry>(); 
    }

    public void addEntry(string prompt, string response)
    {
        entries.Add(new Entry(prompt, response)); 
    }
}

[System.Serializable]
public class Entry
{
    public string prompt;
    public string response; 

    public Entry(string prompt, string response)
    {
        this.prompt = prompt;
        this.response = response; 
    }
}