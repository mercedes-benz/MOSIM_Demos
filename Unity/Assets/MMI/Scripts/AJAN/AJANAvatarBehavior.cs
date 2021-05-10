using MMICoSimulation;
using MMICSharp.MMIStandard.Utils;
using MMIStandard;
using MMIUnity.TargetEngine;
using MMIUnity.TargetEngine.Scene;
using UnityEngine;


public class AJANAvatarBehavior : AvatarBehavior
{
    public AJANAgent ajan;

    protected override void GUIBehaviorInput()
    {
        
        if (GUI.Button(new Rect(10, 100, 120, 25), "Create Agent"))
        {
            ajan.createAgent();
        }

        if (GUI.Button(new Rect(140, 100, 120, 25), "Execute Agent"))
        {
            ajan.executeAgent();
        }

    }




    /// <summary>
    /// Callback for the co-simulation event handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CoSimulator_MSimulationEventHandler(object sender, MSimulationEvent e)
    {
        Debug.Log(e.Reference + " " + e.Name + " " + e.Type);
    }


}
