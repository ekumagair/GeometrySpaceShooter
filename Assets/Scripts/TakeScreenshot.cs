using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TakeScreenshot : MonoBehaviour
{
#if UNITY_STANDALONE && !UNITY_WEBGL
    public const string SCREENSHOT_DIR = "Screenshots";

    private static int _identifier = 0;
    private static string _previousDate = "";

    // Attach this component to any GameObject in the scene to allow screenshots to be taken.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            string dateStr = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            if (dateStr == _previousDate)
            {
                _identifier += 1;
            }
            else
            {
                _identifier = 0;
            }

            _previousDate = dateStr;

            string fileName = Application.productName + "_v" + Application.version.ToString() + " " + dateStr + " " + _identifier.ToString() + ".png";

            string folder = Path.Combine(Application.persistentDataPath, SCREENSHOT_DIR);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            ScreenCapture.CaptureScreenshot(Path.Combine(folder, fileName));

            if (Debug.isDebugBuild == true)
            {
                Debug.Log("Took screenshot. Saved it as: " + fileName);
            }
        }
    }
#endif
}