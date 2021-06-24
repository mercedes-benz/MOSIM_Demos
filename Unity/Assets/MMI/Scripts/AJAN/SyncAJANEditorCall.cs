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
using MMIUnity.TargetEngine;
using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SyncAJANEditorCall : MonoBehaviour
{
    public MMISettings mmiSettings;
    public string Repository = "http://localhost:8090/rdf4j/repositories/test_knowledge";
    public void CallAJAN()
    {
        string mObjects = GetAllMSceneObjects();
        Debug.Log(mObjects);
        UpdateRepo(mObjects);
    }

    private string GetAllMSceneObjects()
    {
        StringBuilder RDFObjects = new StringBuilder();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int i = 0;
        foreach(GameObject obj in allObjects) {
            MMISceneObject CC = obj.GetComponent<MMISceneObject>();
            if (CC != null)
            {
                RDFObjects.Append(GetMSceneObjectRDF(obj.name, i++));
            }
        }
        return RDFObjects.ToString();
    }

    private void UpdateRepo(string mObjects)
    {
        StartCoroutine(Delete(mObjects));
    }

    private string GetMSceneObjectRDF(string name, int i)
    {
        StringBuilder graph = new StringBuilder();
        string sceneObject = "<tcp://" + mmiSettings.RemoteSceneAccessAddress + ":" + mmiSettings.RemoteSceneAccessPort + "/" + i + ">";
        graph.Append(sceneObject + " " + "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" + " " + "<http://www.dfki.de/mosim-ns#MSceneObject> .");
        graph.Append(sceneObject + " " + "<http://www.w3.org/2000/01/rdf-schema#label>" + " " + "'" + name + "'" + ".");
        return graph.ToString();
    }

    IEnumerator Delete(string mObjects)
    {
        WWWForm form = new WWWForm();
        form.AddField("update", "DELETE {?s ?p ?o} WHERE {?s ?p ?o}");
        using (UnityWebRequest www = UnityWebRequest.Post(Repository + "/statements", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                StartCoroutine(Upload(mObjects));
            }
        }
    }

    IEnumerator Upload(string mObjects)
    {
        WWWForm form = new WWWForm();
        form.AddField("update", "INSERT DATA {" + mObjects + "} ");
        using (UnityWebRequest www = UnityWebRequest.Post(Repository + "/statements", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
}
