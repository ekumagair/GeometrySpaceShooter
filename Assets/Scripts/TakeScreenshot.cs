using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeScreenshot : MonoBehaviour
{
    // Attach this component to any GameObject in the scene to allow screenshots to be taken.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5) && Debug.isDebugBuild == true)
        {
            string filename = Application.productName + "_v" + Application.version.ToString() + "_screenshot " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".png";

            ScreenCapture.CaptureScreenshot(filename);
            Debug.Log("Took screenshot. Saved it as: " + filename);
        }
    }
}
