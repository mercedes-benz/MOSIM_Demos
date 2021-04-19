using MMIStandard;
using MMIUnity.TargetEngine.Scene;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MMIArea : MonoBehaviour
{
    public Uri Uri;
    private MSceneObject sceneObject;
    private List<string> contains = new List<string>();

    // Use this for initialization
    void Start()
    {
        sceneObject = gameObject.GetComponent<MMISceneObject>().MSceneObject;
        if(sceneObject.Properties.Count == 0)
        {
            sceneObject.Properties = new Dictionary<string, string>();
            sceneObject.Properties.Add("type", "Area");
            sceneObject.Properties.Add("contains", "{}");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerStay(Collider other)
    {
        MMISceneObject obj = other.GetComponent<MMISceneObject>();
        if(obj != null) {
            if (!contains.Contains(obj.MSceneObject.ID))
            {
                Debug.Log("Contains: " + obj.name);
                contains.Add(obj.MSceneObject.ID);
                sceneObject.Properties.Remove("contains");
                sceneObject.Properties.Add("contains", GetListAsString(contains));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MMISceneObject obj = other.GetComponent<MMISceneObject>();
        if (obj != null)
        {
            if(contains.Contains(obj.MSceneObject.ID))
            {
                contains.Remove(obj.MSceneObject.ID);
                sceneObject.Properties.Remove("contains");
                sceneObject.Properties.Add("contains", GetListAsString(contains));
            } 
        }
    }

    private string GetListAsString(List<string> list)
    {
        string stringList = "{";
        foreach(string item in list)
        {
            stringList += item;
            if (list.IndexOf(item) < list.Count-1)
            {
                stringList += ",";
            }
        }
        return stringList + "}";
    }
}