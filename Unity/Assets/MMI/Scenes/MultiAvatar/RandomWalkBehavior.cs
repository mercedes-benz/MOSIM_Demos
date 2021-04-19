using MMIStandard;
using MMIUnity.TargetEngine;
using MMIUnity.TargetEngine.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalkBehavior : AvatarBehavior
{
    private MInstruction currentInstruction;
    public MMISceneObject WalkTarget;

    protected override void Start()
    {
        base.Start();


        this.WalkTarget = new GameObject("WalkTarget" + this.name).AddComponent<MMISceneObject>();
        this.WalkTarget.transform.parent = UnitySceneAccess.Instance.transform;

        this.StartCoroutine(this.CheckInitialization());
    }

    IEnumerator CheckInitialization()
    {
        while (this.CoSimulator == null || !this.avatar.MMUAccess.IsInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }
        this.CoSimulator.MSimulationEventHandler += CoSimulator_MSimulationEventHandler;



        //this.StartInstruction();
        this.StartCoroutine(this.InstructionScheduler());
    }

    IEnumerator InstructionScheduler()
    {
        while (this.CoSimulator != null && this.avatar.MMUAccess.IsInitialized)
        {

            this.CoSimulator.Abort();
            this.StartInstruction();

            yield return new WaitForSeconds(Random.Range(10, 20));


        }


    }

    private void CoSimulator_MSimulationEventHandler(object sender, MSimulationEvent e)
    {
        if (e.Reference == this.currentInstruction.ID && e.Type == mmiConstants.MSimulationEvent_End)
        {
            //Start the next instruction
            this.StartInstruction();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void GUIBehaviorInput()
    {
        //base.GUIBehaviorInput();
    }

    private void StartInstruction()
    {
        this.WalkTarget.transform.position = new Vector3(Random.Range(-9, 9), 0, Random.Range(-9, 9));
        this.WalkTarget.Synchronize();

        this.currentInstruction = new MInstruction(MInstructionFactory.GenerateID(), "Walk", "Locomotion/Walk")
        {
            Properties = new Dictionary<string, string>()
            {
                { "TargetID", this.WalkTarget.MSceneObject.ID },
                { "ForcePath", true.ToString()},
                { "UseTargetOrientation", false.ToString()}
            }
        };

        this.CoSimulator.AssignInstruction(this.currentInstruction, null);

    }
}
