using MMIStandard;
using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MMIAvatar))]
public class GazeTester : MonoBehaviour
{

    public MMISceneObject GazeTarget;
    public bool Execute = false;
    private bool lastState = false;

    private MMIAvatar avatar;
    private string lastInstructionID;


    // Start is called before the first frame update
    void Start()
    {
        this.avatar = this.GetComponent<MMIAvatar>();
    }

    void LateUpdate()
    {
        if (Execute && !lastState && GazeTarget !=null)
        {
            MInstruction instruction = new MInstruction(System.Guid.NewGuid().ToString(), "gaze", "Pose/Gaze")
            {
                Properties = new Dictionary<string, string>()
                {
                    { "TargetID", this.GazeTarget.MSceneObject.ID}
                }
            };

            this.lastInstructionID = instruction.ID;

            MMIAvatar avatar = this.GetComponent<MMIAvatar>();

            //Add the motion type to the cosim
            avatar.CoSimulator.SetPriority("Pose/Gaze", 10);

            avatar.CoSimulator.AssignInstruction(instruction, new MSimulationState(avatar.GetPosture(), avatar.GetPosture()));

        }

        if (!Execute && lastState)
        {
            this.avatar.CoSimulator.Abort(lastInstructionID);
        }

        lastState = Execute;

    }
}
