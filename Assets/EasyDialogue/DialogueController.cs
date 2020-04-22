using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DialogueController : MonoBehaviour
{
    [Serializable]
    public class DialogueSettings
    {
        //[Header("Default Global Values")]
        public AudioClip TextSound;
        public float TextDisplaySpeed = 0.1f;
        // public float TextToNextBlock = 25;
        public float TimeToNextThread = 5;
        public float TextDecayTime = 20;

        public TextMeshProUGUI GlobalTextOutput;

    }

    [Serializable]
    public class DialogueBlock
    {
        public string BlockName;

        public bool CanBeShownAgain;
        public float BlockDelay = 0;

        public string[] DialogueThread = new string[0];

        public GameObject TriggerQuickAccess;
        
    }
    

    public DialogueSettings GlobalDialogueSettings;

    public List<DialogueBlock> DialogueBlockList = new List<DialogueBlock>(1);

    public bool isDisplayingText;
    public GameObject TextTriggerOrigin;


    public void DisplayNewText(DialoguePlayerTrigger trigger)
    {
        // Find all triggers
        DialoguePlayerTrigger[] objects = FindObjectsOfType<DialoguePlayerTrigger>();

        // Force all of them to stop playing
        foreach (var item in objects)
        {
            item.StopDisplayingText();
        }

        // Have the trigger that called this function to display text
        trigger.StartDisplayingText();
    }

}
