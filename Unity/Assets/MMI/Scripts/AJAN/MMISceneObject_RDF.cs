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

    private StringBuilder RDFProperties = new StringBuilder();
    public string property;
    public string value;
    public string sProperties;

    void OnEnable()
    {
        msObject = GetComponent<MMISceneObject>(); 
    }

    void Start()
    {
        if (!msObject.MSceneObject.Properties.ContainsKey("RDF"))
        {
            string RDF = GetRDF();
            if (RDF.Length > 0)
            {
                msObject.MSceneObject.Properties.Add("RDF", RDF);
            }
        }
    }

    private string GetRDF()
    {
        StringBuilder RDF = new StringBuilder();
        if (path.Length > 0)
        {
            StreamReader reader = new StreamReader(path);
            breakdown = reader.ReadToEnd();
            reader.Close();
            string root = GetRoot(breakdown);
            if (root != null)
            {
                RDF.Append("@prefix mosim: <http://www.dfki.de/mosim-ns#>. \n");
                RDF.Append("@prefix bt: <http://www.ajan.de/behavior/bt-ns#>. \n");
                RDF.Append("mosim:This bt:behavior <" + GetRoot(breakdown) + "> . \n");
                RDF.Append(breakdown);
            }
        }
        RDF.Append(sProperties);
        return RDF.ToString();
    }

    public void AddProperty()
    {
        if (RDFProperties.Length == 0 && sProperties.Equals(""))
        {
            RDFProperties.Append("@prefix mosim: <http://www.dfki.de/mosim-ns#>. \n");
        } else if (RDFProperties.Length == 0 && !sProperties.Equals(""))
        {
            RDFProperties.Append(sProperties);
        }
        RDFProperties.Append("mosim:This mosim:" + property + " '" + value + "' . \n");
        this.property = null;
        this.value = null;
        sProperties = RDFProperties.ToString();
    }

    public void ClearProperties()
    {
        RDFProperties.Clear();
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

