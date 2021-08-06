using MMIStandard;
using MMIUnity.TargetEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AJANEvent : AvatarBehavior
{
    [Header("Selected AJAN AgentTemplate needs an endpoint with this capability!")]
    public string capability;
    private AJANAgent ajan;

    protected override void GUIBehaviorInput()
    {
        this.ajan = this.GetComponent<AJANAgent>();
        if (GUI.Button(new Rect(270, 10, 120, 50), "Send Event"))
        {
            if (ajan)
            {
                MRDFGraph knowledge = new MRDFGraph();
                knowledge.ContentType = "text/turtle";
                knowledge.Graph = "_:test <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Resource> .";
                Debug.Log("Send a new Event to AJANAgent capability: " + capability);
                ajan.sendEvent(capability, knowledge);
            }
        }
    }
}
