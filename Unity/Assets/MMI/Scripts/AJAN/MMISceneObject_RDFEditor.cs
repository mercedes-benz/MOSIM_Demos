/*
 * Created on Tue Aug 10 2021
 *
 * The MIT License (MIT)
 * Copyright (c) 2020 André Antakli (German Research Center for Artificial Intelligence, DFKI).
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Collections;
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
        
        EditorGUILayout.LabelField("Selected Interaction Behavior: ", EditorStyles.boldLabel);
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
        if (msObject.sProperties != null)
        {
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
        }
        
        if (GUILayout.Button("Clear Properties"))
        {
            msObject.ClearProperties();
        }
    }
}
