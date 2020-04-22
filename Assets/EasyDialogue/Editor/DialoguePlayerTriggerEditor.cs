using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialoguePlayerTrigger))]
[CanEditMultipleObjects]
public class DialoguePlayerTriggerEditor : Editor
{
    DialoguePlayerTrigger trigger;
    DialogueController controller;
    
    SerializedObject GetTarget;

    SerializedProperty blockName;
    SerializedProperty optionsIndex;

    //int index;
    int prevIndex;
    string[] options;


    private void OnEnable()
    {

        trigger = (DialoguePlayerTrigger)target;

        GetTarget = new SerializedObject(trigger);
        blockName = GetTarget.FindProperty("BlockToPlay");
        optionsIndex = GetTarget.FindProperty("optionsIndex");
    }
    
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GetTarget.Update();

        controller = trigger.dialogueController;

        if (controller != null)
        {
            options = new string[controller.DialogueBlockList.Count];

            for (int i = 0; i < controller.DialogueBlockList.Count; i++)
            {
                options[i] = controller.DialogueBlockList[i].BlockName;
            }

        }
        else
        {
            options = new string[0];
        }
        

        optionsIndex.intValue = EditorGUILayout.Popup("Block to Play: ", optionsIndex.intValue, options, EditorStyles.popup);
        
        if (controller != null)
            blockName.stringValue = options[optionsIndex.intValue].ToString();

        GetTarget.ApplyModifiedProperties();
    }
}
