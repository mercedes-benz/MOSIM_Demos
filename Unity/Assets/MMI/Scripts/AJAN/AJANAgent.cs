using MMIStandard;
using MMIUnity.TargetEngine;
using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Thrift.Protocol;
using Thrift.Transport;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(MMIAvatar))]
public class AJANAgent : MonoBehaviour
{
    [Header("Fields to establish a connection with AJAN")]
    public MMISettings mmiSettings;
    public string AJANServer = "127.0.0.1";
    public int AJANPort = 8081;
    public string Repository = "http://localhost:8090/rdf4j/repositories/agents";
    public bool Report = false;

    private string AJANTemplate;
    public string AJANExecute;
    private MMIAvatar mmiAvatar;
  

    public int index = 0;
    public List<string> list = new List<string>();
    public Dictionary<string, string> templateList = new Dictionary<string, string>();

    [Header("Field to add High-Level Tasklist")]
    public bool TaskList = false;
    // TODO: Commented to fix error
    // private HighLevelTaskEditor HLTE; //Added for finding task list editor script


    private string AgentURI;

    void OnEnable()
    {
        // TODO: Commented to fix error
        // HLTE = GameObject.FindObjectOfType<HighLevelTaskEditor>(); //task list editor script is found
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
        Debug.Log(index);
        AJANTemplate = templateList[list[index]];
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
        graph.Append(avatar + " " + "<http://www.dfki.de/mosim-ns#transform>" + " " + "'" + transform.position.ToString() + "'" + ".");
        graph.Append(avatar + " " + "<http://www.dfki.de/mosim-ns#isLocatedAt>" + " " + "<http://www.dfki.de/mosim-ns#InitPosition>" + ".");
        if(Report)
            graph.Append(avatar + " " + "<http://www.ajan.de/ajan-ns#agentReportURI> 'http://localhost:4202/report'^^<http://www.w3.org/2001/XMLSchema#anyURI> .");
        // TODO: Commented to fix error
        // if(TaskList) 
        //     graph.Append(avatar + " " + "<http://www.dfki.de/mosim-ns#worksOn>" + " " + "'" + HLTE.URLTaskList() + "'^^<http://www.w3.org/2001/XMLSchema#anyURI>" + ".");

        setSceneInfos(graph);
        setSceneWriteInfos(graph);
        setSkeletonInfos(graph);
        setRegistryInfos(graph);
        setCosimInfos(graph);

        return graph.ToString();
    }

    private void setSceneInfos(StringBuilder graph)
    {
        string scene = "_:scene";
        graph.Append(scene + " " + "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" + " " + "<http://www.dfki.de/mosim-ns#Scene>" + ".");
        graph.Append(scene + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + mmiSettings.RemoteSceneAccessAddress + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        graph.Append(scene + " " + "<http://www.dfki.de/mosim-ns#port>" + " '" + mmiSettings.RemoteSceneAccessPort + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
    }

    private void setSceneWriteInfos(StringBuilder graph)
    {
        string scene = "_:sceneWrite";
        graph.Append(scene + " " + "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" + " " + "<http://www.dfki.de/mosim-ns#SceneWrite>" + ".");
        graph.Append(scene + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + mmiSettings.RemoteSceneWriteAddress + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        graph.Append(scene + " " + "<http://www.dfki.de/mosim-ns#port>" + " '" + mmiSettings.RemoteSceneWritePort + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
    }

    private void setSkeletonInfos(StringBuilder graph)
    {
        string scene = "_:skeleton";
        graph.Append(scene + " " + "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" + " " + "<http://www.dfki.de/mosim-ns#SkeletonAccess>" + ".");
        graph.Append(scene + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + mmiSettings.RemoteSkeletonAccessAddress + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        graph.Append(scene + " " + "<http://www.dfki.de/mosim-ns#port>" + " '" + mmiSettings.RemoteSkeletonAccessPort + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
    }

    private void setRegistryInfos(StringBuilder graph)
    {
        string registry = "_:registry";
        graph.Append(registry + " " + "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" + " " + "<http://www.dfki.de/mosim-ns#Registry>" + ".");
        graph.Append(registry + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + mmiSettings.MMIRegisterAddress + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        graph.Append(registry + " " + "<http://www.dfki.de/mosim-ns#port>" + " '" + mmiSettings.MMIRegisterPort + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
    }

    private void setCosimInfos(StringBuilder graph)
    {
        string cosim = "_:cosim";
        graph.Append(cosim + " " + "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" + " " + "<http://www.dfki.de/mosim-ns#CoSimulator>" + ".");
        graph.Append(cosim + " " + "<http://www.dfki.de/mosim-ns#host>" + " '" + mmiAvatar.RemoteCoSimulationAccessAddress + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
        graph.Append(cosim + " " + "<http://www.dfki.de/mosim-ns#port>" + " '" + mmiAvatar.RemoteCoSimulationAccessPort + "'^^<http://www.w3.org/2001/XMLSchema#string>" + ".");
    }

    public void executeAgent()
    {
        TTransport transport = new TSocket(AJANServer, AJANPort);
        TProtocol protocol = new TBinaryProtocol(transport);
        MAJANService.Client client = new MAJANService.Client(protocol);
        transport.Open();

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
        list.Clear();
        StartCoroutine(LoadTemplates());
    }

    IEnumerator LoadTemplates()
    {
        WWWForm form = new WWWForm();
        StringBuilder query = new StringBuilder();
        query.Append("PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>").Append("\n");
        query.Append("PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>").Append("\n");
        query.Append("PREFIX ajan: <http://www.ajan.de/ajan-ns#>").Append("\n");
        query.Append("SELECT ?label ?uri").Append("\n");
        query.Append("WHERE {?uri rdf:type ajan:AgentTemplate. ?uri rdfs:label ?label.}");
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
                    string template = line[1].Replace("\n", "").Replace("\r", "");
                    templateList.Add(line[0], template);
                    list.Add(line[0]);
                }
            }
        }
    }
}
