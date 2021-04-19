
#if UNITY_EDITOR

using MMIStandard;
using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


/// <summary>
/// An editor window which provides the capability to define/load and edit motion descriptions for manual behavior control and definition.
/// </summary>
public class BehaviorDefinitionWindow : EditorWindow
{



    [MenuItem("MMI/Define Behavior")]
    public static void CreateDescription()
    {
        if (EditorApplication.isPlaying)
        {
            BehaviorDefinitionWindow window = new BehaviorDefinitionWindow(GameObject.FindObjectOfType<MMIAvatar>());
            window.Show();
        }
        else
        {
            EditorUtility.DisplayDialog("Cannot open window", "Please enter play mode to open the window. The available instructions can be only displayed if a connection to the MMI framework is established.", "OK");
        }
    }

    #region private variables


    /// <summary>
    /// The created root instruction which contains all sub instructions
    /// </summary>
    private MInstruction rootInstruction;

    /// <summary>
    /// The corresponding avatar
    /// </summary>
    private MMIAvatar avatar;

    /// <summary>
    /// Array representing the available instructions
    /// </summary>
    private string[] instructionsArray = new string[0];

    /// <summary>
    /// The index of the currently selected instruction
    /// </summary>
    private int selectedInstructionIndex = -1;

    #endregion

    /// <summary>
    /// Basic constructor
    /// </summary>
    /// <param name="avatar"></param>
    public BehaviorDefinitionWindow(MMIAvatar avatar)
    {
        this.avatar = avatar;
        this.rootInstruction = new MInstruction()
        {
            Instructions = new List<MInstruction>(),
            ID = MInstructionFactory.GenerateID(),
            MotionType = "Composite" 
        };

        this.instructionsArray = this.rootInstruction.Instructions.Select(s => s.Name + " , " + s.MotionType + " , " + s.ID).ToArray();
    }


    private void OnGUI()
    {
        EditorGUILayout.LabelField("Behavior:");
        this.rootInstruction.Name = EditorGUILayout.TextField("Name", this.rootInstruction.Name);
        this.rootInstruction.MotionType = EditorGUILayout.TextField("MotionType", this.rootInstruction.MotionType);

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Available Instructions:");
        this.instructionsArray = this.rootInstruction.Instructions.Select(s => s.Name + " , " + s.MotionType + " , " + s.ID).ToArray();
        this.selectedInstructionIndex = EditorGUILayout.Popup(this.selectedInstructionIndex, this.instructionsArray);

        EditorGUILayout.Separator();


        //Button to add a new instruction to the available root instruction
        if (GUILayout.Button("Add Instruction"))
        {
            InstructionDefinitionWindow window = new InstructionDefinitionWindow(this.avatar, ref this.rootInstruction);
            window.Show();
        }

        //Only propose the entries/Buttons if an instruction is selected
        if (this.selectedInstructionIndex >= 0 && this.instructionsArray.Length >0)
        {
            if (GUILayout.Button("Edit Instruction"))
            {
                InstructionDefinitionWindow window = new InstructionDefinitionWindow(this.avatar, ref this.rootInstruction, this.rootInstruction.Instructions[this.selectedInstructionIndex]);
                window.Show();
            }

            if (GUILayout.Button("Remove Instruction"))
            {
                //Remove the instruction
                this.rootInstruction.Instructions.RemoveAt(this.selectedInstructionIndex);
            }
        }


        //Applies the specified instructions at the co-simulation
        if (GUILayout.Button("Apply"))
        {
            this.avatar.CoSimulator.AssignInstruction(this.rootInstruction, new MSimulationState()
            {
                Initial = this.avatar.GetPosture(),
                Current = this.avatar.GetPosture()
            });
        }


        //Saves the specified instruction to a user specified location on the file system
        if (GUILayout.Button("Save"))
        {
            string outputPath = EditorUtility.SaveFilePanel("Select the output file", "Instructions/", name, "json");

            System.IO.File.WriteAllText(outputPath, MMICSharp.Common.Communication.Serialization.ToJsonString(this.rootInstruction));

            EditorUtility.DisplayDialog("Instruction list successfully saved.", "The instructions have been successfully exported to your desired output directory.", "Continue");

        }

        //Loads a set of specified instructions from the file system
        if (GUILayout.Button("Load"))
        {
            string loadingPath = EditorUtility.OpenFilePanel("Select the file to load", "Instructions/", "json");
            this.rootInstruction = MMICSharp.Common.Communication.Serialization.FromJsonString<MInstruction>(System.IO.File.ReadAllText(loadingPath));
        }

        //Aborts the current tasks of the co-simulation
        if (GUILayout.Button("Abort Tasks"))
        {
            this.avatar.CoSimulator.Abort();
        }
    }

}
#endif