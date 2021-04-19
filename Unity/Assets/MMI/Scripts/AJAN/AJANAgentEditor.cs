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
        agent.Repository = EditorGUILayout.TextField("Repository", agent.Repository);

        var toggleReport = EditorGUILayout.Toggle("Report", agent.Report);
        if (toggleReport)
            agent.Report = true;
        else agent.Report = false;

        if (GUILayout.Button("Get Available Agent Templates"))
        {
            agent.Load();
        }

        GUILayoutOption[] arrayList = new GUILayoutOption[] { };
        agent.index = EditorGUILayout.Popup("AgentTemplate", agent.index, agent.list.ToArray(), arrayList);

        agent.AJANExecute = EditorGUILayout.TextField("Execute", agent.AJANExecute);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Fields to define High-Level Tasklist", EditorStyles.boldLabel);
        var toggleTask = EditorGUILayout.Toggle("TaskList Editor", agent.TaskList);
        if (toggleTask)
            agent.TaskList = true;
        else agent.TaskList = false;
    }
}
