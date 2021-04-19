using MMICSharp.Common.Tools;
using MMIStandard;
using MMIUnity.TargetEngine.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MMIAvatar))]
public class TurnMMUTest : MonoBehaviour
{
    public MMISceneObject GraspTarget;
    public MMISceneObject ObjectToTurn;
    public MMISceneObject TurnAxis;
    public UnityHandPose HandPose;

    public int Repetitions = 5;
    public float Angle = 60;
    public HandType Hand = HandType.Left;


    public bool Execute = false;

    private bool lastState = false;
    private string lastInstructionID;
    private MMIAvatar avatar;


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
            //Pickup the object
            MSimulationState state = new MSimulationState(this.avatar.GetPosture(), this.avatar.GetPosture());


            MInstruction idleInstruction = new MInstruction(MInstructionFactory.GenerateID(), "idle: " + ObjectToTurn.name, "idle");

            MInstruction reachInstruction = new MInstruction(MInstructionFactory.GenerateID(), "Reach: " + ObjectToTurn.name, "Pose/Reach")
            {
                Properties = new Dictionary<string, string>() 
                {
                    { "Hand", "Right"},
                    { "TargetID", GraspTarget.MSceneObject.ID }
                }
            };


            if (this.HandPose != null)
            {

                MInstruction moveFingersInstruction = new MInstruction(System.Guid.NewGuid().ToString(), "Move fingers", "Pose/MoveFingers")
                {
                    Properties = new Dictionary<string, string>()
                    {
                        { "Hand", this.HandPose.HandType.ToString()}
                    },
                    Constraints = new List<MConstraint>(),
                    StartCondition = reachInstruction.ID + ":" + mmiConstants.MSimulationEvent_End
                };

                if (HandPose != null)
                {
                    string constraintID = Guid.NewGuid().ToString();
                    moveFingersInstruction.Properties.Add("HandPose", constraintID);
                    moveFingersInstruction.Constraints.Add(new MConstraint()
                    {
                        ID = constraintID,
                        PostureConstraint = HandPose.GetPostureConstraint()
                    });
                }


                this.avatar.CoSimulator.AssignInstruction(moveFingersInstruction, state);

            }

            MInstruction turnObjectInstruction = new MInstruction(Guid.NewGuid().ToString(), "turn: " + ObjectToTurn.name, "Object/Turn")
            {
                Properties = new Dictionary<string, string>()
                {
                    { "SubjectID", ObjectToTurn.MSceneObject.ID },
                    { "Axis", this.TurnAxis.MSceneObject.ID },
                    { "Repetitions", this.Repetitions.ToString() },
                    { "Angle", this.Angle.ToString(System.Globalization.CultureInfo.InvariantCulture) },
                    { "Hand", Hand.ToString() }
                },
                Constraints = new List<MConstraint>(),
                StartCondition = reachInstruction.ID +":" +mmiConstants.MSimulationEvent_End           
            };


            InstructionValidation validator = new InstructionValidation();
            MBoolResponse res = validator.Validate(turnObjectInstruction, this.avatar.MMUAccess.GetLoadableMMUs());

            if (!res.Successful)
            {
                foreach(string s in res.LogData)
                {
                    Debug.LogError(s);
                }
            }

            this.avatar.CoSimulator.AssignInstruction(idleInstruction, state);
            this.avatar.CoSimulator.AssignInstruction(reachInstruction, state);
            this.avatar.CoSimulator.AssignInstruction(turnObjectInstruction, state);
        }

        if (!Execute && lastState)
        {
            //Terminate
            this.avatar.CoSimulator.Abort();
        }

        lastState = Execute;
    }
}
