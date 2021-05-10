using MMIStandard;
using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachGeometryConstraint : MonoBehaviour
{
    public MMISceneObject ReachTarget;
    public HandType Hand;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(270, 100, 200, 25), "Reach (MConstraint)"))
        {

            MConstraint reachConstraint = new MConstraint(System.Guid.NewGuid().ToString())
            {
                GeometryConstraint = new MGeometryConstraint("")
                {
                    ParentToConstraint = new MTransform("", this.ReachTarget.MSceneObject.Transform.Position,this.ReachTarget.MSceneObject.Transform.Rotation)
                }
            };

            MInstruction reachInstruction = new MInstruction("reachXY", "reach", "Pose/Reach")
            {
                Properties = new Dictionary<string, string>()
                {
                    { "TargetID",reachConstraint.ID},
                    { "Hand",  this.Hand.ToString() }
                },
                Constraints = new List<MConstraint>()
                {
                    reachConstraint
                }
            };

            this.GetComponent<MMIAvatar>().CoSimulator.AssignInstruction(reachInstruction, null);
        }
    }
}
