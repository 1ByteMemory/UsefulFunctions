using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class DialoguePlayerTrigger : MonoBehaviour
{
    public DialogueController dialogueController;
    
    AudioSource source;

    [HideInInspector]
    public string BlockToPlay;
    [HideInInspector]
    public int optionsIndex;


    DialogueController.DialogueBlock block;

    [HideInInspector]
    public bool isPlaying, hasAlreadyPlayed, isDecaying, hasStopped;

    bool blockFound;
    bool displayNextThread;

    string TextToDisplay;
    List<string> FracturedText = new List<string>();
    string[] Thread;

    int charStep;
    int threadStep;
    int threadIndex;

    float nextThreadTimer;
    float nextCharTimer;

    float delayEndTime;

    // Text decay variables
    float t;
    float startTime;
    float decayTime;
    Color startColor;
    Color endColor;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isPlaying)
            {
                delayEndTime = Time.time + block.BlockDelay;
                dialogueController.DisplayNewText(gameObject.GetComponent<DialoguePlayerTrigger>());
            }
        }
    }

    public void RequestDialoguePriority()
    {
        if (!isPlaying)
        {
            delayEndTime = Time.time + block.BlockDelay;
            dialogueController.DisplayNewText(gameObject.GetComponent<DialoguePlayerTrigger>());
        }
    }

    public void StartDisplayingText()
    {
        if (!hasAlreadyPlayed && !isPlaying)
        {
            hasAlreadyPlayed = true;
            isPlaying = true;
        }
    }

    public void StopDisplayingText()
    {
        isPlaying = false;
        
        // Reset all of the variables used
        TextToDisplay = "";
        dialogueController.GlobalDialogueSettings.GlobalTextOutput.text = "";

        charStep = 0;
        threadStep = 0;
        threadIndex = 0;

        FracturedText = new List<string>();

        blockFound = false;

        if (block != null)
        {
            if (block.CanBeShownAgain)
                hasAlreadyPlayed = false;
        }

        t = 0;
        startTime = 0;
    }

    public void FindBlock()
    {
        // It only needs to find the block once
        if (!blockFound)
        {
            blockFound = true;

            // Find the block of texts to play
            for (int i = 0; i < dialogueController.DialogueBlockList.Count; i++)
            {
                if (dialogueController.DialogueBlockList[i].BlockName == BlockToPlay)
                {
                    block = dialogueController.DialogueBlockList[i];
                    Thread = block.DialogueThread;
                    
                    // Fracture the text into indiviadual characters
                    for (int t = 0; t < Thread.Length; t++)
                    {
                        string text = Thread[t];
                        for (int c = 0; c < text.Length; c++)
                        {
                            FracturedText.Add(text[c].ToString());
                        }
                    }

                    // Stop looping once the block of dialogue we are looking for has been found
                    break;
                }
            }
        }
    }

    public void SetTextToDisplay()
    {

        // Play the text sound
        if (!source.isPlaying)
        {
            source.Play();
        }

        // Display each character in each thread one by one
        // But only if that char is in the thread
        TextToDisplay += FracturedText[charStep];

        if (charStep + 1 < FracturedText.Count)
        {
            charStep++;
            nextCharTimer = Time.time + dialogueController.GlobalDialogueSettings.TextDisplaySpeed;

            // When the thread has finished, clear move on to the next thread
            if (charStep == Thread[threadIndex].Length + threadStep)
            {
                threadStep += Thread[threadIndex].Length;
                threadIndex++;
                displayNextThread = true;
                source.Stop();

                nextThreadTimer = Time.time + dialogueController.GlobalDialogueSettings.TimeToNextThread;
            }
        }
        else
        {
            // The thread has come to and end.
            isPlaying = false;
            hasStopped = true;
            isDecaying = true;

            source.Stop();
        }

    }

    public void TextDecay()
    {
        dialogueController.GlobalDialogueSettings.GlobalTextOutput.color = TextColor();
    }


    public Color TextColor()
    {
        Color newColor = startColor;
        decayTime = dialogueController.GlobalDialogueSettings.TextDecayTime;
        
        // startTime and startValue are only reset when
        // the camera is centered or the mouse has moved in x axis.
        if (startTime == 0)
        {
            startTime = Time.time;
        }

        // If the player is moveing and the camera is not
        if (isDecaying)
        {
            // Calulate the percentage of where it's at
            t = (Time.time - startTime) / decayTime;

            if (t <= 1)
            {
                // Always return to zero

                newColor = Color.Lerp(startColor, endColor, t);

            }
            else
            {
                StopDisplayingText();
                
            }
        }

        return newColor;
    }

    private void Start()
    {
        source = GetComponent<AudioSource>();
        if (dialogueController.GlobalDialogueSettings.TextSound != null)
            source.clip = dialogueController.GlobalDialogueSettings.TextSound;

        if (dialogueController.GlobalDialogueSettings.GlobalTextOutput != null)
        {
            startColor = dialogueController.GlobalDialogueSettings.GlobalTextOutput.color;
            dialogueController.GlobalDialogueSettings.GlobalTextOutput.text = "";
        }
        endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        FindBlock();
    }

    private void Update()
    {
        FindBlock();

        if (isPlaying && dialogueController.GlobalDialogueSettings.GlobalTextOutput != null)
        {
            dialogueController.GlobalDialogueSettings.GlobalTextOutput.color = startColor;

            // Delay time before showing the text
            if (Time.time >= delayEndTime)
            {

                // Check to display next char
                if (!displayNextThread && Time.time >= nextCharTimer)
                {
                    SetTextToDisplay();
                    dialogueController.GlobalDialogueSettings.GlobalTextOutput.text = TextToDisplay;
                }
                // Check timer to display next thread
                else if (displayNextThread && Time.time >= nextThreadTimer)
                {
                    TextToDisplay = "";
                    dialogueController.GlobalDialogueSettings.GlobalTextOutput.text = TextToDisplay;

                    displayNextThread = false;
                }
            }
        }


        if (hasStopped)
        {
            TextDecay();
        }
    }
}
