#if UNITY_EDITOR

using MMIStandard;
using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class AddParameterWindow : EditorWindow
{
    #region private variables


    private MInstruction instruction;
    private MMUDescription description;
    private string parameterName;
    private string value;
    private string[] values;
    private string[] sceneObjectNames;
    private string[] sceneObjectIDs;

    private int selectedSceneObjectIndex;
    private int selectedValueIndex;


    private string[] availableParameters = new string[0];
    private int selectedParameterNameIndex = 0;
    private string customParameterName;

    private bool boolValue = false;

    #endregion

    private string SelectedParameterName
    {
        get
        {
            if (this.selectedParameterNameIndex >= this.availableParameters.Length)
                return null;

            return this.availableParameters[this.selectedParameterNameIndex];
        }
    }


    private MParameter SelectedParameter
    {
        get
        {
            //Return null if out of range
            if (this.selectedParameterNameIndex >= this.availableParameters.Length)
                return null;

            //Return custom parameter
            if(this.availableParameters[this.selectedParameterNameIndex] == "Custom")
            {
                return new MParameter()
                {
                    Description = "A custom parameter",
                    Type = "String"
                };
            }

            //Return the parameter of the description
            else
                return this.description.Parameters.Find(s => s.Name == SelectedParameterName);
        }
    }

    private enum ValueType
    {
        String,
        ID,
        Bool,
        Set
    }

    private bool isID = false;

    private ValueType selectedValueType = ValueType.String;


    /// <summary>
    /// Basic constructor instantiating a new AddParameterWindow
    /// </summary>
    /// <param name="instruction"></param>
    /// <param name="description"></param>
    public AddParameterWindow(ref MInstruction instruction, MMUDescription description)
    {
        //Assign the variables
        this.description = description;
        this.instruction = instruction;
            
        //Get all available parameters
        List<string> parameterList = description.Parameters.Select(s => s.Name).ToList();

        //Add custom -> for undefined parameter
        parameterList.Add("Custom");

        //Set the parameter names for the popup list
        this.availableParameters = parameterList.ToArray();
    }

    private void OnGUI()
    {
        this.UpdateAvailableParameters();

        EditorGUILayout.LabelField("Parameter Name");
        selectedParameterNameIndex = EditorGUILayout.Popup(selectedParameterNameIndex, this.availableParameters);

        MParameter selected = SelectedParameter;

        //Update the available parameters

        if (selected != null)
        {
            this.customParameterName = selected.Name;

            //Custom parameter
            if (selected.Type == "Custom")
            {
                parameterName = EditorGUILayout.TextField("Name", parameterName);
                value = EditorGUILayout.TextField("Value", value);

                this.selectedValueType = ValueType.String;
            }

            //Parameter as specified by the description
            else
            {
                //Visualize the parameter type
                EditorGUILayout.LabelField("Parameter Type: " + selected.Type);
                EditorGUILayout.LabelField("Required: " + selected.Type);
                EditorGUILayout.LabelField("Description: " + selected.Description);





                //Check the type and suggest an input field according to the type
                if (selected.Type.Contains("{") && selected.Type.Contains("}"))
                {
                    this.values = selected.Type.Replace("{", "").Replace("}", "").Split(',');
                    this.selectedValueIndex = EditorGUILayout.Popup(selectedValueIndex, this.values);

                    this.selectedValueType = ValueType.Set;
                }

                //Provide the possibility to select a scene object
                else if (selected.Type == "ID")
                {
                    //Allow to select id of scene object
                    if (this.sceneObjectNames == null)
                    {
                        this.sceneObjectNames = UnitySceneAccess.Instance.GetSceneObjects().Select(s => s.Name).ToArray();
                        this.sceneObjectIDs = UnitySceneAccess.Instance.GetSceneObjects().Select(s => s.ID).ToArray();
                    }

                    this.selectedSceneObjectIndex = EditorGUILayout.Popup(this.selectedSceneObjectIndex, this.sceneObjectNames);

                    this.selectedValueType = ValueType.ID;
                }


                //Handle boolean parameters
                else if(selected.Type =="Bool" || selected.Type == "Boolean" || selected.Type == "bool")
                {
                    this.boolValue= EditorGUILayout.Toggle("Value: ", this.boolValue);

                    this.selectedValueType = ValueType.Bool;

                }

                //Default free text field
                else
                {
                    this.isID = EditorGUILayout.Toggle("Treat as ID: ", this.isID);

                    if (isID)
                    {
                        //Allow to select id of scene object
                        if (this.sceneObjectNames == null)
                        {
                            this.sceneObjectNames = UnitySceneAccess.Instance.GetSceneObjects().Select(s => s.Name).ToArray();
                            this.sceneObjectIDs = UnitySceneAccess.Instance.GetSceneObjects().Select(s => s.ID).ToArray();
                        }

                        this.selectedSceneObjectIndex = EditorGUILayout.Popup(this.selectedSceneObjectIndex, this.sceneObjectNames);

                        this.selectedValueType = ValueType.ID;
                    }
                    else
                    {
                        this.customParameterName = EditorGUILayout.TextField("Parameter Name", customParameterName);

                        this.value = EditorGUILayout.TextField("Value", value);
                        this.selectedValueType = ValueType.String;
                    }
                }
            }


        }

        if (GUILayout.Button("Ok"))
        {
            switch (this.selectedValueType)
            {
                case ValueType.ID:
                    this.AddParameter(this.availableParameters[this.selectedParameterNameIndex], this.sceneObjectIDs[this.selectedSceneObjectIndex]);
                    break;

                case ValueType.Set:
                    this.AddParameter(this.availableParameters[this.selectedParameterNameIndex], this.values[this.selectedValueIndex]);

                    break;

                case ValueType.String:
                    this.AddParameter(this.parameterName, this.value);

                    break;

                case ValueType.Bool:
                    this.AddParameter(this.availableParameters[this.selectedParameterNameIndex], this.boolValue.ToString());

                    break;

            }
            Close();
        }


        if (GUILayout.Button("Abort"))
            Close();

    }

    private void AddParameter(string name, string value)
    {

        if (instruction.Properties == null)
            instruction.Properties = new Dictionary<string, string>();

        //Create the new paramter and add
        instruction.Properties.Add(name, value);
    }

    private void UpdateAvailableParameters()
    {
        List<string> result = new List<string>();
        for(int i=0; i< this.availableParameters.Length; i++)
        {
            //Check if already set
            if (!this.instruction.Properties.ContainsKey(this.availableParameters[i]))
            {
                result.Add(this.availableParameters[i]);
            }
        }
        this.availableParameters = result.ToArray();
    }

}

#endif