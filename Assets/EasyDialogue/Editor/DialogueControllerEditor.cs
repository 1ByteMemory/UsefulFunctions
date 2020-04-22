using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(DialogueController))]
[CanEditMultipleObjects]
public class DialogueControllerEditor : Editor
{
    DialogueController controller;
    
    SerializedObject GetTarget;
    SerializedProperty BlockList;
    SerializedProperty TextSettings;
   
    private void OnEnable()
    {

        controller = (DialogueController)target;
        
        GetTarget = new SerializedObject(controller);

        BlockList = GetTarget.FindProperty("DialogueBlockList");
        TextSettings = GetTarget.FindProperty("GlobalDialogueSettings");
    }

    public override void OnInspectorGUI()
    {
        // Update the block list
        GetTarget.Update();

        SerializedProperty global_TextSettingsRef = TextSettings;
        SerializedProperty global_Sound = global_TextSettingsRef.FindPropertyRelative("TextSound");
        SerializedProperty global_DisplaySpeed = global_TextSettingsRef.FindPropertyRelative("TextDisplaySpeed");
        //SerializedProperty global_BlockDelay = global_TextSettingsRef.FindPropertyRelative("BlockDelay");
        SerializedProperty global_TimeNextThread = global_TextSettingsRef.FindPropertyRelative("TimeToNextThread");
        SerializedProperty global_DecayTime = global_TextSettingsRef.FindPropertyRelative("TextDecayTime");
        SerializedProperty global_TextOutput = global_TextSettingsRef.FindPropertyRelative("GlobalTextOutput");
        

        GUIStyle boldStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold
        };

        GUIStyle wordWarpStyle = new GUIStyle
        {
            wordWrap = true
        };

        GUILayoutOption[] TextLayout = {
            GUILayout.MaxWidth(500),
            GUILayout.MinHeight(45),
            GUILayout.MaxHeight(100)
        };

        GUILayoutOption[] AddRemoveLayout =
        {
            GUILayout.Height(17),
            GUILayout.Width(17)
        };

        GUIStyle AddRemoveStyle = GUI.skin.button;

        AddRemoveStyle.alignment = TextAnchor.MiddleCenter;


        EditorGUILayout.LabelField("Text Display Settings", boldStyle);
        global_Sound.objectReferenceValue = EditorGUILayout.ObjectField
            (
                "    Sound",
                global_Sound.objectReferenceValue,
                typeof(AudioClip),
                true
            );
        global_DisplaySpeed.floatValue = EditorGUILayout.FloatField("    Chararcter Delay Time", global_DisplaySpeed.floatValue);
        //global_BlockDelay.floatValue = EditorGUILayout.FloatField("    Block Delay", global_BlockDelay.floatValue);
        global_TimeNextThread.floatValue = EditorGUILayout.FloatField("    Time To Next Thread", global_TimeNextThread.floatValue);
        global_DecayTime.floatValue = EditorGUILayout.FloatField("    Decay Time", global_DecayTime.floatValue);
        global_TextOutput.objectReferenceValue = EditorGUILayout.ObjectField
            (
                "    Text Output",
                global_TextOutput.objectReferenceValue,
                typeof(TextMeshProUGUI),
                true
            );

        EditorGUILayout.Space();

        // Button to add a new block of dialogue
        if (GUILayout.Button("Add New Block of Dialogue", GUILayout.Height(18)))
        {
            controller.DialogueBlockList.Add(new DialogueController.DialogueBlock());
        }


        if (controller.DialogueBlockList.Count > 1)
        {
            if (GUILayout.Button("Remove a Block of Dialogue", GUILayout.Height(18)))
                controller.DialogueBlockList.RemoveAt(controller.DialogueBlockList.Count - 1);
        }

        // If there is no block, add one (happens when a new container is created)
        if (controller.DialogueBlockList.Count == 0)
        {
            controller.DialogueBlockList.Add(new DialogueController.DialogueBlock());
        }



        EditorGUILayout.Space();

        // Display the block  in the inspector
        for (int i = 0; i < BlockList.arraySize; i++)
        {

            SerializedProperty m_BlockRef = BlockList.GetArrayElementAtIndex(i);
            SerializedProperty m_BlockName = m_BlockRef.FindPropertyRelative("BlockName");
            SerializedProperty m_Repeatable = m_BlockRef.FindPropertyRelative("CanBeShownAgain");
            SerializedProperty m_BlockDelay = m_BlockRef.FindPropertyRelative("BlockDelay");
            SerializedProperty m_BlockThread = m_BlockRef.FindPropertyRelative("DialogueThread");
            SerializedProperty m_TriggerQuickAccess = m_BlockRef.FindPropertyRelative("TriggerQuickAccess");
            SerializedProperty m_LocalFontSettings = m_BlockRef.FindPropertyRelative("localDialogueSettings");

            // Add each block to the enum.
        
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("___________________________________________________________________________________________________________________________________________");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Block: " + m_BlockName.stringValue, boldStyle);

            m_BlockName.stringValue = EditorGUILayout.TextField("Block Name", m_BlockName.stringValue);

            if (m_BlockName.stringValue == "")
            {
                m_BlockName.stringValue = i.ToString();
            }

            // If the block doesn't have a thread, add one
            if (m_BlockThread.arraySize == 0)
            {
                m_BlockThread.InsertArrayElementAtIndex(0);
                m_BlockThread.GetArrayElementAtIndex(0).stringValue = "Text...";
            }



            EditorGUILayout.PropertyField(m_Repeatable);
            EditorGUILayout.PropertyField(m_BlockDelay);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            // The bit for adding a new thread
            if (GUILayout.Button("Add New Thread", GUILayout.Width(117), GUILayout.Height(18)))
            {
                m_BlockThread.InsertArrayElementAtIndex(m_BlockThread.arraySize);
                m_BlockThread.GetArrayElementAtIndex(m_BlockThread.arraySize - 1).stringValue = "Text...";
            }
            if (m_BlockThread.arraySize > 1)
            {
                if (GUILayout.Button("Remove a Thread", GUILayout.MaxWidth(130), GUILayout.MaxHeight(20)))
                {
                    m_BlockThread.DeleteArrayElementAtIndex(m_BlockThread.arraySize - 1);
                }
            }
            EditorGUILayout.EndHorizontal();

            for (int n = 0; n < m_BlockThread.arraySize; n++)
            {

                EditorGUILayout.BeginHorizontal();
                // Text box
                m_BlockThread.GetArrayElementAtIndex(n).stringValue = GUILayout.TextArea(m_BlockThread.GetArrayElementAtIndex(n).stringValue, TextLayout);

                EditorGUILayout.BeginVertical();
                // Add Button
                if (GUILayout.Button("+", AddRemoveStyle, AddRemoveLayout))
                {
                    m_BlockThread.InsertArrayElementAtIndex(n);
                    m_BlockThread.GetArrayElementAtIndex(n + 1).stringValue = "Text...";
                }
                // Remove button
                if (m_BlockThread.arraySize > 1)
                {
                    if (GUILayout.Button("-", AddRemoveStyle, AddRemoveLayout))
                    {
                        // Remove TExt box
                        m_BlockThread.DeleteArrayElementAtIndex(n);
                    }
                }

                EditorGUILayout.EndVertical();


                EditorGUILayout.EndHorizontal();
            }



            EditorGUILayout.PropertyField(m_TriggerQuickAccess);

            EditorGUILayout.Space();
            
            

            EditorGUILayout.BeginHorizontal();
            // Add / Remove buttons for the blocks
            if (GUILayout.Button("+", AddRemoveStyle, AddRemoveLayout))
            {
                controller.DialogueBlockList.Insert(i + 1, new DialogueController.DialogueBlock());
            }
            if (controller.DialogueBlockList.Count > 1)
            {

                if (GUILayout.Button("-", AddRemoveStyle, AddRemoveLayout))
                {
                    controller.DialogueBlockList.RemoveAt(i);
                }
            }

            EditorGUILayout.EndHorizontal();
            
        }

        GetTarget.ApplyModifiedProperties();

    }
}
