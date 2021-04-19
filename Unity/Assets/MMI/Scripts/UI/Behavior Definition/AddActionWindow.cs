#if UNITY_EDITOR

using MMICoSimulation;
using MMIStandard;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Window for adding actions to the MInstruction
/// </summary>
public class AddActionWindow : EditorWindow
{
    #region private fields

    private MInstruction instruction;
    private List<MInstruction> otherInstructions;
    private string[] references;
    private int referenceIndex;
    private int selectedTypeIndex;


    private string[] availableTypes = new string[] { CoSimTopic.OnStart, CoSimTopic.OnEnd };
    private string[] availableActions = new string[] { CoSimAction.StartInstruction, CoSimAction.EndInstruction};
    private int selectedActionIndex;

    #endregion

    /// <summary>
    /// Basic constructor accepting the MInstruction (as reference) and all other available instructions
    /// </summary>
    /// <param name="instruction"></param>
    /// <param name="instructions"></param>
    public AddActionWindow(ref MInstruction instruction, ref List<MInstruction> instructions)
    {
        this.instruction = instruction;
        this.otherInstructions = instructions;
        this.references = instructions.Select(s => s.Name).ToArray();
    }


    private void OnGUI()
    {
        EditorGUILayout.LabelField("Add Action:");

        EditorGUILayout.LabelField("Type:");

        this.selectedTypeIndex = EditorGUILayout.Popup(this.selectedTypeIndex, this.availableTypes);

        EditorGUILayout.LabelField("Reference:");
        this.referenceIndex = EditorGUILayout.Popup(this.referenceIndex, this.references);


        EditorGUILayout.LabelField("Action:");
        this.selectedActionIndex = EditorGUILayout.Popup(this.selectedActionIndex, this.availableActions);


        if (GUILayout.Button("Ok"))
        {
            this.instruction.Properties.Add(this.availableTypes[this.selectedTypeIndex], this.references[this.referenceIndex] + ":" + this.availableActions[this.selectedActionIndex]);
            Close();
        }


        if (GUILayout.Button("Abort"))
        {
            Close();
        }

    }
}
#endif