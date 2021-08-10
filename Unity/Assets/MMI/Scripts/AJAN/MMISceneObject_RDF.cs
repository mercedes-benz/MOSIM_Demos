using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

[RequireComponent(typeof(MMISceneObject))]
public class MMISceneObject_RDF : MonoBehaviour
{

    private MMISceneObject msObject;

    public string path;
    public string breakdown;

    public string property;
    public string value;
    public string sProperties;
    public Dictionary<string, string> properties = new Dictionary<string, string>();

    void OnEnable()
    {
        msObject = GetComponent<MMISceneObject>(); 
    }

    void Start()
    {
        Debug.Log(path);
        Debug.Log(msObject.MSceneObject.Properties.Count);
        if (!msObject.MSceneObject.Properties.ContainsKey("RDF"))
        {
            string RDF = GetRDF();
            Debug.Log(RDF);
            if (RDF.Length > 0)
            {
                Debug.Log("Load RDF stuff");
                msObject.MSceneObject.Properties.Add("RDF", RDF);
            }
        }
        Debug.Log(msObject.MSceneObject.Properties.Count);
    }

    private string GetRDF()
    {
        StringBuilder RDF = new StringBuilder();
        if (path.Length > 0)
        {
            RDF.Append("@prefix mosim: <http://www.dfki.de/mosim-ns#>.");
            RDF.Append("@prefix bt: <http://www.ajan.de/behavior/bt-ns#>.");
            StreamReader reader = new StreamReader(path);
            breakdown = reader.ReadToEnd();
            reader.Close();
            string root = GetRoot(breakdown);
            Debug.Log(root);
            if (root != null)
            {
                RDF.Append("mosim:This bt:behavior <" + GetRoot(breakdown) + "> .");
                RDF.Append(breakdown);
            }
        }
        RDF.Append(sProperties);
        return RDF.ToString();
    }

    public void AddProperty()
    {
        properties.Add("mosim:" + this.property, this.value);
        this.property = null;
        this.value = null;
        sProperties = ReadProperties(properties);
    }

    private string ReadProperties(Dictionary<string, string> data)
    {
        StringBuilder RDF = new StringBuilder();
        if (data.Count > 0)
        {
            RDF.Append("@prefix mosim: <http://www.dfki.de/mosim-ns#>. \n");
            foreach (KeyValuePair<string, string> entry in data)
            {
                RDF.Append("mosim:This " + entry.Key + " '" + entry.Value + "' . \n");
            }
            return RDF.ToString();
        }
        else
        {
            return "";
        }
    }

    public void ClearProperties()
    {
        properties.Clear();
        sProperties = "";
    }

    private string GetRoot(string content)
    {
        if (content.Contains("# Root: <")) {
            Regex rgx = new Regex("# Root: <(.*?)>");
            Match m = rgx.Match(content);
            return m.Value.Replace("# Root: <","").Replace(">","");
        } else
        {
            return null;
        }
    }
}

