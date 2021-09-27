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
using MMIUnity.TargetEngine.Editor;
using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Thrift.Protocol;
using Thrift.Transport;
using UnityEngine;
using UnityEngine.Networking;

[ExecuteInEditMode]
[RequireComponent(typeof(MMIAvatar))]
public class AJANAgent : MonoBehaviour
{
    [Header("Fields to establish a connection with AJAN")]
    public MMISettings mmiSettings;
    public string AJANServer = "127.0.0.1";
    public int AJANPort = 8081;
    public int AgentCLPort = 8083;
    public string Repository = "http://localhost:8090/rdf4j/repositories/agents";
    public bool Report = false;
    public bool Docker = false;

    private string AJANTemplate;
    public string AJANExecute;
    private MMIAvatar mmiAvatar;

    private string dockerHost = "host.docker.internal";
    public int atIndex = 0;
    public List<string> atList = new List<string>();
    public List<AgentTemplate> templateList = new List<AgentTemplate>();

    public class AgentTemplate
    {
        public string label;
        public string uri;
        public List<string> endpoints = new List<string>();
    }

    public int caIndex = 0;
    public List<string> caList = new List<string>();

    // TODO: Commented to fix error
    [Header("Field to add High-Level Tasklist")]
    public bool TaskList = false;
    public int WorkerId = -1;
    private HighLevelTaskEditor HLTE; //Added for finding task list editor script


    private string AgentURI;

    void OnEnable()
    {
        // TODO: Commented to fix error
        HLTE = GameObject.FindObjectOfType<HighLevelTaskEditor>(); //task list editor script is found
    }

    void Start()
    {
        this.mmiAvatar = this.GetComponent<MMIAvatar>();
        Load();
    }

    void Update()
    {
    }

    void OnDestroy()
    {
        deleteAgent();
    }

    public void createAgent()
    {
        Debug.Log(atIndex);
        foreach (AgentTemplate entry in templateList)
        {
            if (entry.label.Equals(atList[atIndex]))
            {
                AJANTemplate = entry.uri;
                break;
            }

        }
        Debug.Log(AJANTemplate);
        if (AJANTemplate != null)
        {
            TTransport transport = new TSocket(AJANServer, AJANPort);
            TProtocol protocol = new TBinaryProtocol(transport);
            MAJANService.Client client = new MAJANService.Client(protocol);
            transport.Open();

            try
            {
                MRDFGraph knowledge = new MRDFGraph();
                knowledge.ContentType = "text/turtle";
                knowledge.Graph = InitializeGraph();
                AgentURI = client.CreateAgent(name, AJANTemplate, knowledge);
            }
            finally
            {
                transport.Close();
            }
        }
    }

    private string InitializeGraph()
    {
        StringBuilder graph = new StringBuilder();
        string avatar = "<127.0.0.1:9000/avatars/" + name + ">";

        graph.Append(avatar + " " + "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" + " " + "<http://www.dfki.de/mosim-ns#Avatar>" + ".");
        graph.Append(avatar + " " + "<http://www.dfki.de/mosim-ns#id>" + " " + "'" + mmiAvatar.MAvatar.ID + "'" + ".");
        graph.Append(avatar + " " + "<http://www.dfki.de/mosim-ns#clPort>" + " " + "'" + AgentCLPort + "'" + ".");
        graph.Append(avatar + " " + "<http://www.dfki.de/mosim-ns#transform>" + " " + "'" + transform.position.ToString() + "'" + ".");
        graph.Append(avatar + " " + "<http://www.dfki.de/mosim-ns#isLocatedAt>" + " " + "<http://www.dfki.de/mosim-ns#InitPosition>" + ".");
        if(Report)
            graph.Append(avatar + " " + "<http://www.ajan.de/ajan-ns#agentReportURI> 'http://localhost:4202/report'^^<http://www.w3.org/2001/XMLSchema#anyURI> .");
        // TODO: Commented to fix error
        if (TaskList && HLTE != null && setWorkerId())
        {
            graph.Append(avatar + " " + "<http://www.dfki.de/mosim-ns#tokenHLTE>" + " " + "'" + HLTE.accessToken + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
            graph.Append(avatar + " " + "<http://www.dfki.de/mosim-ns#workerId>" + " " + "'" + WorkerId + "'^^<http://www.w3.org/2001/XMLSchema#int>" + ".");
            graph.Append(avatar + " " + "<http://www.dfki.de/mosim-ns#worksOn>" + " " + "'" + HLTE.URLTaskList() + "'^^<http://www.w3.org/2001/XMLSchema#anyURI>" + ".");
        }

        setSceneInfos(graph);
        setSceneWriteInfos(graph);
        setSkeletonInfos(graph);
        setRegistryInfos(graph);
        setCosimInfos(graph);

        return graph.ToString();
    }

    private bool setWorkerId()
    {
        bool available = false;
        ulong avatarId = 0;
        List<HighLevelTaskEditor.TJsonAvatars> jsonAvatar = HLTE.avatarJson;
        foreach (HighLevelTaskEditor.TJsonAvatars avatar in jsonAvatar)
        {
            if (avatar.avatar == name)
            {
                avatarId = avatar.id;
                available = true;
                break;
            }
        }
        if (available)
        {
            List<HighLevelTaskEditor.TJsonWorkers> jsonWorkers = HLTE.workersJson;
            foreach (HighLevelTaskEditor.TJsonWorkers worker in jsonWorkers)
            {
                if (worker.avatarid == avatarId && worker.simulate)
                {
                    WorkerId = (int)worker.id;
                    return available;
                }
            }
        }
        return available;
    }

    private void setSceneInfos(StringBuilder graph)
    {
        string scene = "_:scene";
        graph.Append(scene + " " + "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" + " " + "<http://www.dfki.de/mosim-ns#Scene>" + ".");
        if (Docker && (mmiSettings.RemoteSceneAccessAddress == "localhost" || mmiSettings.RemoteSceneAccessAddress == "127.0.0.1"))
            graph.Append(scene + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + dockerHost + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        else graph.Append(scene + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + mmiSettings.RemoteSceneAccessAddress + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        graph.Append(scene + " " + "<http://www.dfki.de/mosim-ns#port>" + " '" + mmiSettings.RemoteSceneAccessPort + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
    }

    private void setSceneWriteInfos(StringBuilder graph)
    {
        string scene = "_:sceneWrite";
        graph.Append(scene + " " + "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" + " " + "<http://www.dfki.de/mosim-ns#SceneWrite>" + ".");
        if (Docker && (mmiSettings.RemoteSceneWriteAddress == "localhost" || mmiSettings.RemoteSceneWriteAddress == "127.0.0.1"))
            graph.Append(scene + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + dockerHost + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        else graph.Append(scene + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + mmiSettings.RemoteSceneWriteAddress + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        graph.Append(scene + " " + "<http://www.dfki.de/mosim-ns#port>" + " '" + mmiSettings.RemoteSceneWritePort + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
    }

    private void setSkeletonInfos(StringBuilder graph)
    {
        string skeleton = "_:skeleton";
        graph.Append(skeleton + " " + "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" + " " + "<http://www.dfki.de/mosim-ns#SkeletonAccess>" + ".");
        if (Docker && (mmiSettings.RemoteSkeletonAccessAddress == "localhost" || mmiSettings.RemoteSkeletonAccessAddress == "127.0.0.1"))
            graph.Append(skeleton + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + dockerHost + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        else graph.Append(skeleton + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + mmiSettings.RemoteSkeletonAccessAddress + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        graph.Append(skeleton + " " + "<http://www.dfki.de/mosim-ns#port>" + " '" + mmiSettings.RemoteSkeletonAccessPort + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
    }

    private void setRegistryInfos(StringBuilder graph)
    {
        string registry = "_:registry";
        graph.Append(registry + " " + "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" + " " + "<http://www.dfki.de/mosim-ns#Registry>" + ".");
        if (Docker && (mmiSettings.MMIRegisterAddress == "localhost" || mmiSettings.MMIRegisterAddress == "127.0.0.1"))
            graph.Append(registry + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + dockerHost + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        else graph.Append(registry + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + mmiSettings.MMIRegisterAddress + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        graph.Append(registry + " " + "<http://www.dfki.de/mosim-ns#port>" + " '" + mmiSettings.MMIRegisterPort + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
    }

    private void setCosimInfos(StringBuilder graph)
    {
        string cosim = "_:cosim";
        graph.Append(cosim + " " + "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" + " " + "<http://www.dfki.de/mosim-ns#CoSimulator>" + ".");
        if (Docker && (mmiAvatar.RemoteCoSimulationAccessAddress == "localhost" || mmiAvatar.RemoteCoSimulationAccessAddress == "127.0.0.1"))
            graph.Append(cosim + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + dockerHost + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        else graph.Append(cosim + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + mmiAvatar.RemoteCoSimulationAccessAddress + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        graph.Append(cosim + " " + "<http://www.dfki.de/mosim-ns#port>" + " '" + mmiAvatar.RemoteCoSimulationAccessPort + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
    }

    public void executeAgent()
    {
        TTransport transport = new TSocket(AJANServer, AJANPort);
        TProtocol protocol = new TBinaryProtocol(transport);
        MAJANService.Client client = new MAJANService.Client(protocol);
        transport.Open();

        Debug.Log(caIndex);
        AJANExecute = caList[caIndex];

        try
        {
            MRDFGraph knowledge = new MRDFGraph();
            knowledge.ContentType = "text/turtle";
            knowledge.Graph = "_:test <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Resource> .";
            AgentURI = client.ExecuteAgent(name, AJANExecute, knowledge);
        }
        finally
        {
            transport.Close();
        }
    }

    public void sendEvent(string endpoint, MRDFGraph knowledge)
    {
        TTransport transport = new TSocket(AJANServer, AJANPort);
        TProtocol protocol = new TBinaryProtocol(transport);
        MAJANService.Client client = new MAJANService.Client(protocol);
        transport.Open();

        try
        {
            AgentURI = client.ExecuteAgent(name, endpoint, knowledge);
        }
        finally
        {
            transport.Close();
        }
    }

    private void deleteAgent()
    {
        TTransport transport = new TSocket(AJANServer, AJANPort);
        TProtocol protocol = new TBinaryProtocol(transport);
        MAJANService.Client client = new MAJANService.Client(protocol);
        transport.Open();

        try
        {
            client.DeleteAgent(this.name);
        }
        finally
        {
            transport.Close();
        }
    }

    public void Load()
    {
        atList.Clear();
        StartCoroutine(LoadTemplates());
    }

    public void SetCapabilities()
    {
        caList.Clear();
        foreach (AgentTemplate entry in templateList)
        {
            if (entry.label.Equals(atList[atIndex]))
            {
                foreach(string endpoint in entry.endpoints)
                {
                    caList.Add(endpoint);
                }
                break;
            }

        }
    }

    IEnumerator LoadTemplates()
    {
        WWWForm form = new WWWForm();
        StringBuilder query = new StringBuilder();
        query.Append("PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>").Append("\n");
        query.Append("PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>").Append("\n");
        query.Append("PREFIX ajan: <http://www.ajan.de/ajan-ns#>").Append("\n");
        query.Append("SELECT DISTINCT ?label ?uri ?capability").Append("\n");
        query.Append("WHERE {?uri rdf:type ajan:AgentTemplate. ?uri rdfs:label ?label. ?uri ajan:endpoint ?endpoint. ?endpoint ajan:capability ?capability .}");
        form.AddField("query", query.ToString());
        using (UnityWebRequest www = UnityWebRequest.Post(Repository, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                string response = www.downloadHandler.text;
                string[] lines = response.Split('\n');
                templateList.Clear();
                for (int i = 1; i < lines.Length-1; i++)
                {
                    string[] line = lines[i].Split(',');
                    string label = line[0].Replace("\n", "").Replace("\r", "");
                    AgentTemplate template = new AgentTemplate();
                    foreach (AgentTemplate entry in templateList)
                    {
                        if (entry.label.Equals(label))
                        {
                            template = entry;
                            break;
                        }
                    }
                    if (template.label == null)
                    {
                        template.label = label;
                        template.uri = line[1].Replace("\n", "").Replace("\r", "");
                        templateList.Add(template);
                        atList.Add(line[0]);
                    }
                    template.endpoints.Add(line[2].Replace("\n", "").Replace("\r", ""));
                }
            }
        }
    }
}
