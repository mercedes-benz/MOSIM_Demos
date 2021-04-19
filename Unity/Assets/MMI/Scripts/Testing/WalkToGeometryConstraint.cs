using MMIStandard;
using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkToGeometryConstraint : MonoBehaviour
{
    private void OnGUI()
    {
        if(GUI.Button(new Rect(100, 100, 100, 50), "Walk"))
        {

            MConstraint walkConstraint = new MConstraint(System.Guid.NewGuid().ToString())
            {
                GeometryConstraint = new MGeometryConstraint("")
                {
                    ParentToConstraint = new MTransform("", new MVector3(1, 0, 3), new MQuaternion(0, 0, 0, 1))
                }
            };

            MInstruction walkInstruction = new MInstruction("walkXY", "walk", "walk")
            {
                Properties = new Dictionary<string, string>()
                {
                    { "TargetID",walkConstraint.ID}
                },
                Constraints = new List<MConstraint>()
                {
                    walkConstraint
                }
            };

            this.GetComponent<MMIAvatar>().CoSimulator.AssignInstruction(walkInstruction, null);
        }
    }
}
