
#if UNITY_EDITOR

using MMIStandard;
using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InstructionDefinitionWindow : EditorWindow
{


    private List<MMUDescription> mmuDescriptions = null;
    private MInstruction instruction;

    public string[] eventOptions = new string[] { mmiConstants.MSimulationEvent_End, mmiConstants.MSimulationEvent_Start };
    public string[] motionTypeOptions = new string[0];
    public int motionTypeIndex = 0;

    private MMIAvatar avatar;
    private List<MInstruction> instructionList;

    public InstructionDefinitionWindow(MMIAvatar avatar, ref MInstruction rootInstruction, MInstruction availableInstruction = null)
    {
        this.avatar = avatar;
        this.mmuDescriptions = this.avatar.MMUAccess.GetLoadableMMUs();


        this.motionTypeOptions = mmuDescriptions.Select(s => s.MotionType).ToArray();
        this.instructionList = rootInstruction.Instructions;

        if (availableInstruction == null)
        {
            this.instruction = new MInstruction();
            this.instruction.ID = MInstructionFactory.GenerateID();
            this.instruction.Properties = new Dictionary<string, string>();
        }
        else
        {
            this.instruction = availableInstruction;

            //Preselect the motion type
            if (this.motionTypeOptions.Contains(this.instruction.MotionType))
            {
                this.motionTypeIndex = this.motionTypeOptions.ToList().IndexOf(this.instruction.MotionType);
            }

        }


    }

    
    void OnGUI()
    {
        EditorGUILayout.LabelField("Add Instruction:");

        instruction.Name = EditorGUILayout.TextField("Name", instruction.Name);
        instruction.ID = EditorGUILayout.TextField("ID", instruction.ID);
        EditorGUILayout.LabelField("Motion Type:");
        motionTypeIndex = EditorGUILayout.Popup(motionTypeIndex, motionTypeOptions);

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Properties:");
        foreach (var entry in instruction.Properties)
        {
            EditorGUILayout.LabelField(entry.Key + " : " + entry.Value);
        }

        if (GUILayout.Button("Add Parameter"))
        {
            //Get selected MMUDescription
            MMUDescription description = this.mmuDescriptions[this.motionTypeIndex];

            AddParameterWindow window = new AddParameterWindow(ref instruction, description);
            window.Show();
        }

        EditorGUILayout.LabelField("Start Condition:");


        if(this.instruction.StartCondition != null)
        {
            EditorGUILayout.LabelField(this.instruction.StartCondition);
        }

        EditorGUILayout.LabelField("End Condition:");

        if (this.instruction.EndCondition != null)
        {
            EditorGUILayout.LabelField(this.instruction.EndCondition);
        }


        if (GUILayout.Button("Add Condition"))
        {
            AddConditionWindow window = new AddConditionWindow(ref this.instruction, ref this.instructionList);
            window.Show();
        }

        if (GUILayout.Button("Add Action"))
        {
            //Get selected MMUDescription
            MMUDescription description = this.mmuDescriptions[this.motionTypeIndex];

            AddActionWindow window = new AddActionWindow(ref instruction, ref this.instructionList);
            window.Show();
        }


        if (GUILayout.Button("Ok"))
        {
            //Finally assign the motion type
            this.instruction.MotionType = this.motionTypeOptions[this.motionTypeIndex];

            //Check if all required parameters are set 
            if (this.CheckParameters())
            {
                this.instructionList.Add(this.instruction);
                Close();
            }
        }


        //if (GUILayout.Button("Save"))
        //{
        //    //Finally assign the motion type
        //    this.instruction.MotionType = this.motionTypeOptions[this.motionTypeIndex];

        //    string outputPath = EditorUtility.SaveFilePanel("Select the output file", "Instructions/", name, "json");

        //    System.IO.File.WriteAllText(outputPath, MMICSharp.Common.Communication.Serialization.ToJsonString(this.instruction));

        //    EditorUtility.DisplayDialog("Instruction list successfully saved.", "The instructions have been successfully exported to your desired output directory.", "Continue");
        //}


        if (GUILayout.Button("Abort"))
            Close();
    }


    private bool CheckParameters()
    {
        if(this.instruction.Name == null || this.instruction.Name.Length < 3)
        {
            EditorUtility.DisplayDialog("Required parameter name not set.", "Each instruction must contain a name with at least 3 chars.", "Continue");


            return false;
        }

        if (this.mmuDescriptions[this.motionTypeIndex].Parameters != null)
        {
            //Check if all required parameters are set 
            List<MParameter> unspecifiedParameters = this.mmuDescriptions[this.motionTypeIndex].Parameters.Where(s => s.Required && !instruction.Properties.ContainsKey(s.Name)).ToList();

            if (unspecifiedParameters.Count > 0)
            {
                string missingParameters = "";

                foreach (MParameter parameter in unspecifiedParameters)
                    missingParameters += parameter.Name + ", ";

                EditorUtility.DisplayDialog("Required parameters not set.", "The following required parameters are not specified:" + missingParameters, "Continue");

                return false;
            }
        }

        return true;
    }
}

#endif