/*
 * Created on Tue Nov 10 2020
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

using MMIUnity.TargetEngine;
using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AJANAgent))]
public class AJANAgentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AJANAgent agent = (AJANAgent)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Fields to establish a connection with AJAN", EditorStyles.boldLabel);
        agent.mmiSettings = (MMISettings)EditorGUILayout.ObjectField("MMISettings", agent.mmiSettings, typeof(MMISettings));
        agent.AJANServer = EditorGUILayout.TextField("AJANServer", agent.AJANServer);
        agent.AJANPort = EditorGUILayout.IntField("AJANPort", agent.AJANPort);
        agent.AgentCLPort = EditorGUILayout.IntField("AgentCLPort", agent.AgentCLPort);
        agent.Repository = EditorGUILayout.TextField("Repository", agent.Repository);

        var toggleReport = EditorGUILayout.Toggle("Report", agent.Report);
        if (toggleReport)
            agent.Report = true;
        else agent.Report = false;

        var toggleDocker = EditorGUILayout.Toggle("Docker", agent.Docker);
        if (toggleDocker)
            agent.Docker = true;
        else agent.Docker = false;

        if (GUILayout.Button("Get Available Agent Templates"))
        {
            agent.Load();
        }

        GUILayoutOption[] arrayList = new GUILayoutOption[] { };
        agent.index = EditorGUILayout.Popup("AgentTemplate", agent.index, agent.list.ToArray(), arrayList);

        agent.AJANExecute = EditorGUILayout.TextField("Execute", agent.AJANExecute);
        // TODO: Commented to fix error
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Fields to define High-Level Tasklist", EditorStyles.boldLabel);
        var toggleTask = EditorGUILayout.Toggle("TaskList Editor", agent.TaskList);
        if (toggleTask)
            agent.TaskList = true;
        else agent.TaskList = false;
    }
}
