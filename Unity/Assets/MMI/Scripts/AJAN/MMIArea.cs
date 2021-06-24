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