using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SyncAJANEditorCall))]
public class SyncAJANEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SyncAJANEditorCall call = (SyncAJANEditorCall)target;
        if(GUILayout.Button("Synchronize AJAN Editor Repository")) {
            call.CallAJAN();
        }
    }
}
