using MMIStandard;
using MMIUnity.TargetEngine.Scene;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MMIAvatar))]
public class MoveFingers : MonoBehaviour
{
    public UnityHandPose LeftHandPose;
    public UnityHandPose RightHandPose;


    public bool Execute = false;
    public bool Release = false;

    private MMIAvatar avatar;
    private bool lastState = false;
    private string lastInstructionID;


    // Start is called before the first frame update
    void Start()
    {
        this.avatar = this.GetComponent<MMIAvatar>();    
    }

    // Update is called once per frame
    void Update()
    {
        if (Execute && !lastState)
        {


            if (LeftHandPose != null)
            {
                //Create the instruction to move the fingers
                MInstruction moveFingersInstructions = new MInstruction(System.Guid.NewGuid().ToString(), "Move fingers", "Pose/MoveFingers")
                {
                    Properties = new Dictionary<string, string>()
                    {
                        {"Release", this.Release.ToString() },
                        {"Hand", "Left" }
                    },

                    Constraints = new List<MConstraint>()
                };

                string constraintID = System.Guid.NewGuid().ToString();
                moveFingersInstructions.Properties.Add("HandPose", constraintID);
                moveFingersInstructions.Constraints.Add(new MConstraint()
                {
                    ID = constraintID,
                    PostureConstraint = this.LeftHandPose.GetPostureConstraint()
                });

                this.avatar.CoSimulator.AssignInstruction(moveFingersInstructions, new MSimulationState(avatar.GetPosture(), avatar.GetPosture()));

            }

            if (this.RightHandPose != null)
            {
                //Create the instruction to move the fingers
                MInstruction moveFingersInstructions = new MInstruction(System.Guid.NewGuid().ToString(), "Move fingers", "Pose/MoveFingers")
                {
                    Properties = new Dictionary<string, string>()
                    {
                        {"Release", this.Release.ToString() },
                        {"Hand", "Right" }
                    },

                    Constraints = new List<MConstraint>()
                };

                string constraintID = System.Guid.NewGuid().ToString();
                moveFingersInstructions.Properties.Add("HandPose", constraintID);
                moveFingersInstructions.Constraints.Add(new MConstraint()
                {
                    ID = constraintID,
                    PostureConstraint = this.RightHandPose.GetPostureConstraint()
                });

                this.avatar.CoSimulator.AssignInstruction(moveFingersInstructions, new MSimulationState(avatar.GetPosture(), avatar.GetPosture()));
            }
        }

        if(!Execute && lastState)
        {
            //Terminate
            this.avatar.CoSimulator.Abort(this.lastInstructionID);
        }

        lastState = Execute;

    }
}
