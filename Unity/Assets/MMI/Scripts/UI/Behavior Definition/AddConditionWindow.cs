#if UNITY_EDITOR

using MMIStandard;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AddConditionWindow : EditorWindow
{
    private MInstruction instruction;
    private List<MInstruction> otherInstructions;
    private string[] references;
    private int referenceIndex;
    private int eventIndex;
    private int selectedTypeIndex;

    private string[] availableEvents = new string[] { mmiConstants.MSimulationEvent_Start, mmiConstants.MSimulationEvent_End, mmiConstants.MSimulationEvent_Abort };
    private string[] availableTypes = new string[] { "StartCondition", "EndCondition" };

    public AddConditionWindow(ref MInstruction instruction, ref List<MInstruction> instructions)
    {
        this.instruction = instruction;
        this.otherInstructions = instructions;
        this.references = instructions.Select(s => s.Name).ToArray();
    }


    private void OnGUI()
    {
        EditorGUILayout.LabelField("Add Condition:");

        EditorGUILayout.LabelField("Type:");

        this.selectedTypeIndex = EditorGUILayout.Popup(this.selectedTypeIndex, this.availableTypes);

        EditorGUILayout.LabelField("Reference:");
        this.referenceIndex = EditorGUILayout.Popup(this.referenceIndex, this.references);


        EditorGUILayout.LabelField("Event:");
        this.eventIndex = EditorGUILayout.Popup(this.eventIndex, this.availableEvents);


        if (GUILayout.Button("Ok"))
        {
            switch (this.availableTypes[this.selectedTypeIndex])
            {
                case "StartCondition":
                    this.instruction.StartCondition = this.otherInstructions[this.referenceIndex].ID + ":" + this.availableEvents[this.eventIndex];
                    break;

                case "EndCondition":
                    this.instruction.EndCondition = this.otherInstructions[this.referenceIndex].ID + ":" + this.availableEvents[this.eventIndex];
                    break;
            }

            Close();
        }


        if (GUILayout.Button("Abort"))
        {
            Close();
        }

    }
}

#endif