﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MMISceneObject_RDF))]
public class MMISceneObject_RDFEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MMISceneObject_RDF msObject = (MMISceneObject_RDF)target;
        
        EditorGUILayout.LabelField("Selected Behavior: ", EditorStyles.boldLabel);
        if (GUILayout.Button("Select a Breakdown"))
        {
            msObject.path = EditorUtility.OpenFilePanel("Select Breakdown", "", "ttl");
        }
        msObject.path = EditorGUILayout.TextField("Breakdown Path", msObject.path);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Add RDF Property: ", EditorStyles.boldLabel);

        msObject.property = EditorGUILayout.TextField("Property", msObject.property);
        msObject.value = EditorGUILayout.TextField("Value", msObject.value);
        if (GUILayout.Button("Add RDF Property"))
        {
            msObject.AddProperty();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("RDF Properties: ", EditorStyles.boldLabel);
        StringReader strReader = new StringReader(msObject.sProperties);
        while (true)
        {
            string aLine = strReader.ReadLine();
            if (aLine != null)
            {
                EditorGUILayout.LabelField(aLine);
            }
            else
            {
                break;
            }
        }
        /*foreach (KeyValuePair<string, string> entry in msObject.properties)
        {
            EditorGUILayout.LabelField("mosim:This " + entry.Key + " '" + entry.Value + "' .");
        }
        msObject.sProperties = EditorGUILayout.TextField("RDF", msObject.sProperties);*/
        if (GUILayout.Button("Clear Properties"))
        {
            msObject.ClearProperties();
        }
    }
}
