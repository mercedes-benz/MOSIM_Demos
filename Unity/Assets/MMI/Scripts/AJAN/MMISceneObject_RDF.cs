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

