/*
 * Created on Tue Nov 10 2020
 *
 * The MIT License (MIT)
 * Copyright (c) 2020 André Antakli (German Research Center for Artificial Intelligence, DFKI).
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using MMICoSimulation;
using MMICSharp.MMIStandard.Utils;
using MMIStandard;
using MMIUnity.TargetEngine;
using MMIUnity.TargetEngine.Scene;
using System.Text;
using UnityEngine;


public class AJANAvatarBehavior : AvatarBehavior
{
    public AJANAgent ajan;

    protected override void GUIBehaviorInput()
    {
        
        if (GUI.Button(new Rect(10, 10, 120, 50), "Create Agent"))
        {
            ajan.createAgent();
            GetAllMSceneObjects();
        }

        if (GUI.Button(new Rect(140, 10, 120, 50), "Execute Agent"))
        {
            ajan.executeAgent();
        }

    }

    private void GetAllMSceneObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int i = 0;
        foreach (GameObject obj in allObjects)
        {
            MMISceneObject CC = obj.GetComponent<MMISceneObject>();
            if (CC != null)
            {
                Debug.Log(CC.MSceneObject.Name);
                Debug.Log(CC.MSceneObject.ID);
            }
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
