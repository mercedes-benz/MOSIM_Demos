using MMICSharp.MMIStandard.Utils;
using MMIStandard;
using MMIUnity;
using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MMIUnity.TargetEngine.Scene.MMISceneObject))]
public class TrajectoryInstructions : MonoBehaviour
{
    public List<Transform> TrajectoryPoints = new List<Transform>();

    public float TranslationTolerance = 0.05f;
    public float RotationTolerance = 0.1f;

    private void OnGUI()
    {
        if(GUI.Button(new Rect(300,200,200,100), "Grasp object (trajectory)"))
        {

            MPathConstraint pathConstraint = new MPathConstraint()
            {
                PolygonPoints = new List<MGeometryConstraint>()
            };
            
            foreach(Transform transform in this.TrajectoryPoints)
            {
                MVector3 position = transform.position.ToMVector3();
                MVector3 rotationEuler = transform.rotation.eulerAngles.ToMVector3();


                pathConstraint.PolygonPoints.Add(new MGeometryConstraint("")
                {
                    TranslationConstraint = new MTranslationConstraint()
                    {
                        Type = MTranslationConstraintType.BOX,
                        Limits = position.ToMInterval3(this.TranslationTolerance)
                    },
                    RotationConstraint = new MRotationConstraint()
                    {
                        Limits = rotationEuler.ToMInterval3(this.RotationTolerance)
                    }
                }); 
            }

            MConstraint moveConstraint = new MConstraint(System.Guid.NewGuid().ToString())
            {
                PathConstraint = pathConstraint
            };

            MInstruction idleInstruction = new MInstruction(MInstructionFactory.GenerateID(), "Idle", "idle");

            MInstruction reachRight = new MInstruction(MInstructionFactory.GenerateID(), "reach right", "Pose/Reach")
            {
                Properties = PropertiesCreator.Create("TargetID", UnitySceneAccess.Instance["GraspTargetR"].ID, "Hand", "Right", "MinDistance", 2.0.ToString()),
            };

            MInstruction moveRight = new MInstruction(MInstructionFactory.GenerateID(), "move right", "Object/Move")
            {
                Properties = new Dictionary<string, string>()
                {
                    {"SubjectID", this.GetComponent<MMISceneObject>().MSceneObject.ID },
                    {"Hand", "Right" },
                    {"MinDistance", 2.0.ToString() },
                    {"trajectory", moveConstraint.ID }
                },
                StartCondition = reachRight.ID +":"+ mmiConstants.MSimulationEvent_End,
                 Constraints = new List<MConstraint>() { moveConstraint}
            };

            MMIAvatar avatar = GameObject.FindObjectOfType<MMIAvatar>();

            //this.CoSimulator.Abort();
            avatar.CoSimulator.AssignInstruction(idleInstruction, new MSimulationState() { Initial = avatar.GetPosture(), Current = avatar.GetPosture() });
            avatar.CoSimulator.AssignInstruction(reachRight, new MSimulationState() { Initial = avatar.GetPosture(), Current = avatar.GetPosture() });
            avatar.CoSimulator.AssignInstruction(moveRight, new MSimulationState() { Initial = avatar.GetPosture(), Current = avatar.GetPosture() });

        }
    }

}
