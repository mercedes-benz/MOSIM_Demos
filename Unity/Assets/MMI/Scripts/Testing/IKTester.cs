using MMIStandard;
using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTester : MonoBehaviour
{
    public bool NewInterface = false;
    public bool EnableIK = false;
    public MMISceneObject LeftHandTarget;
    public MMISceneObject RightHandTarget;



    // Update is called once per frame
    void LateUpdate()
    {
        if (EnableIK)
        {

            MMIAvatar avatar = this.GetComponent<MMIAvatar>();

            if (NewInterface)
            {
                List<MConstraint> constraints = new List<MConstraint>();


                if (LeftHandTarget != null)
                {
                    MJointConstraint leftHandConstraint = new MJointConstraint(MJointType.LeftWrist)
                    {
                        GeometryConstraint = new MGeometryConstraint()
                        {
                            ParentObjectID = "",
                            ParentToConstraint = this.LeftHandTarget.MSceneObject.Transform
                        }
                    };

                    constraints.Add(new MConstraint(System.Guid.NewGuid().ToString())
                    {
                        JointConstraint = leftHandConstraint
                    });
                }

                if (RightHandTarget != null)
                {
                    MJointConstraint rightHandConstraint = new MJointConstraint(MJointType.RightWrist)
                    {
                        GeometryConstraint = new MGeometryConstraint()
                        {
                            ParentObjectID = "",
                            ParentToConstraint = this.RightHandTarget.MSceneObject.Transform
                        }
                    };

                    constraints.Add(new MConstraint(System.Guid.NewGuid().ToString())
                    {
                        JointConstraint = rightHandConstraint
                    });
                }

                MIKServiceResult result = avatar.MMUAccess.ServiceAccess.IKService.CalculateIKPosture(avatar.GetRetargetedPosture(), constraints, new Dictionary<string, string>());
                avatar.AssignPostureValues(result.Posture);


            }
            else
            {
                List<MIKProperty> ikProperties = new List<MIKProperty>();

                if (LeftHandTarget != null)
                {
                    ikProperties.Add(new MIKProperty()
                    {
                        OperationType = MIKOperationType.SetPosition,
                        Target = MEndeffectorType.LeftHand,
                        Values = new List<double>() { LeftHandTarget.transform.position.x, LeftHandTarget.transform.position.y, LeftHandTarget.transform.position.z },
                        Weight = 1
                    });

                    ikProperties.Add(new MIKProperty()
                    {
                        OperationType = MIKOperationType.SetRotation,
                        Target = MEndeffectorType.LeftHand,
                        Values = new List<double>() { LeftHandTarget.transform.rotation.x, LeftHandTarget.transform.rotation.y, LeftHandTarget.transform.rotation.z, LeftHandTarget.transform.rotation.w },
                        Weight = 1
                    });
                }
                if (RightHandTarget != null)
                {
                    ikProperties.Add(new MIKProperty()
                    {
                        OperationType = MIKOperationType.SetPosition,
                        Target = MEndeffectorType.RightHand,
                        Values = new List<double>() { RightHandTarget.transform.position.x, RightHandTarget.transform.position.y, RightHandTarget.transform.position.z },
                        Weight = 1
                    });

                    ikProperties.Add(new MIKProperty()
                    {
                        OperationType = MIKOperationType.SetRotation,
                        Target = MEndeffectorType.RightHand,
                        Values = new List<double>() { RightHandTarget.transform.rotation.x, RightHandTarget.transform.rotation.y, RightHandTarget.transform.rotation.z, RightHandTarget.transform.rotation.w },
                        Weight = 1
                    });
                }

                MAvatarPostureValues posture = avatar.MMUAccess.ServiceAccess.IKService.ComputeIK(avatar.GetRetargetedPosture(), ikProperties);

                avatar.AssignPostureValues(posture);
            }

        }
    }
}

