using MMICoSimulation;
using MMICSharp.MMIStandard.Utils;
using MMIStandard;
using MMIUnity.TargetEngine;
using MMIUnity.TargetEngine.Scene;
using UnityEngine;
using System.Collections.Generic;

public class NeuralNetworkDemo : AvatarBehavior
{
    private string carryID;

    // Motion Types
    private readonly string MOTION_CARRY = "Object/Carry";
    private readonly string MOTION_GAZE = "Pose/Gaze";
    private readonly string MOTION_IDLE = "Pose/Idle";
    private readonly string MOTION_MOVEFINGERS = "Pose/MoveFingers";
    private readonly string MOTION_MOVE = "Object/Move";
    private readonly string MOTION_REACH = "Pose/Reach";
    private readonly string MOTION_RELEASE = "Object/Release";
    private readonly string MOTION_SIMPLE = "Object/Test";
    private readonly string MOTION_TURN = "Object/Turn";
    private readonly string MOTION_WALK = "Locomotion/Walk";
    private readonly string MOTION_RUN = "Locomotion/Running";

    public string Goal_1 = "Goal1";
    public string Goal_2 = "Goal2";
    private int last_goal = 1;

    protected override void GUIBehaviorInput()
    {
        if (GUI.Button(new Rect(10, 10, 120, 50), "Idle"))
        {
            MSkeletonAccess.Iface skeletonAccess = this.avatar.GetSkeletonAccess();
            skeletonAccess.SetChannelData(this.avatar.GetPosture());



            MInstruction instruction = new MInstruction(MInstructionFactory.GenerateID(), "Idle", MOTION_IDLE);
            //MInstruction instruction = new MInstruction(MInstructionFactory.GenerateID(), "MMUTest", "Object/Move");
            MSimulationState simstate = new MSimulationState(this.avatar.GetPosture(), this.avatar.GetPosture());



            this.CoSimulator.Abort();
            this.CoSimulator.AssignInstruction(instruction, simstate);
        }
        if (GUI.Button(new Rect(140, 10, 120, 50), "Walk"))
        {
            MInstruction walkInstruction = new MInstruction(MInstructionFactory.GenerateID(), "Walk", MOTION_WALK)
            {
                Properties = PropertiesCreator.Create("TargetID", UnitySceneAccess.Instance.GetSceneObjectByName(GetNextGoalName()).ID),
            };

            MInstruction idleInstruction = new MInstruction(MInstructionFactory.GenerateID(), "Idle", MOTION_IDLE)
            {
                //Start idle after walk has been finished
                EndCondition = walkInstruction.ID + ":" + mmiConstants.MSimulationEvent_End //synchronization constraint similar to bml "id:End"  (bml original: <bml start="id:End"/>
            };

            MInstruction idleInstruction2 = new MInstruction(MInstructionFactory.GenerateID(), "Idle", MOTION_IDLE)
            {
                //Start idle after walk has been finished
                StartCondition = walkInstruction.ID + ":" + mmiConstants.MSimulationEvent_End //synchronization constraint similar to bml "id:End"  (bml original: <bml start="id:End"/>
            };

            this.CoSimulator.Abort();


            MSimulationState currentState = new MSimulationState() { Initial = this.avatar.GetPosture(), Current = this.avatar.GetPosture() };

            //Assign walk and idle instruction
            this.CoSimulator.AssignInstruction(idleInstruction, currentState);
            this.CoSimulator.AssignInstruction(walkInstruction, currentState);
            this.CoSimulator.AssignInstruction(idleInstruction2, currentState);
            this.CoSimulator.MSimulationEventHandler += this.CoSimulator_MSimulationEventHandler;
        }
        if (GUI.Button(new Rect(270, 10, 120, 50), "Run"))
        {
            MInstruction walkInstruction = new MInstruction(MInstructionFactory.GenerateID(), "Walk", MOTION_RUN)
            {
                Properties = PropertiesCreator.Create("TargetID", UnitySceneAccess.Instance.GetSceneObjectByName(GetNextGoalName()).ID),
            };

            MInstruction idleInstruction = new MInstruction(MInstructionFactory.GenerateID(), "Idle", MOTION_IDLE)
            {
                //Start idle after walk has been finished
                EndCondition = walkInstruction.ID + ":" + mmiConstants.MSimulationEvent_End //synchronization constraint similar to bml "id:End"  (bml original: <bml start="id:End"/>
            };

            MInstruction idleInstruction2 = new MInstruction(MInstructionFactory.GenerateID(), "Idle", MOTION_IDLE)
            {
                //Start idle after walk has been finished
                StartCondition = walkInstruction.ID + ":" + mmiConstants.MSimulationEvent_End //synchronization constraint similar to bml "id:End"  (bml original: <bml start="id:End"/>
            };

            this.CoSimulator.Abort();


            MSimulationState currentState = new MSimulationState() { Initial = this.avatar.GetPosture(), Current = this.avatar.GetPosture() };

            //Assign walk and idle instruction
            this.CoSimulator.AssignInstruction(idleInstruction, currentState);
            this.CoSimulator.AssignInstruction(walkInstruction, currentState);
            this.CoSimulator.AssignInstruction(idleInstruction2, currentState);
            this.CoSimulator.MSimulationEventHandler += this.CoSimulator_MSimulationEventHandler;
        }


    }

    private string GetNextGoalName()
    {
        if (this.last_goal > 1)
        {
            this.last_goal = 0;
        }
        if(this.last_goal == 0)
        {
            this.last_goal++;
            return this.Goal_1;
        }
        else
        {
            this.last_goal++;
            return this.Goal_2;
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