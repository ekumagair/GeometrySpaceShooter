using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetLevels : MonoBehaviour
{
    public GameObject[] presetLevels;

    public static int currentPresetLevel = 0;

    void Start()
    {
        if (GameStats.currentLevelType == 1)
        {
            if (presetLevels[currentPresetLevel] != null)
            {
                Instantiate(presetLevels[currentPresetLevel], new Vector3(0, 7, 0), transform.rotation);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static string IntToLevelName(int integer)
    {
        switch (integer)
        {
            case 0:
                return "A";

            case 1:
                return "B";

            case 2:
                return "C";

            case 3:
                return "D";

            case 4:
                return "E";

            case 5:
                return "F";

            default:
                return "";
        }
    }
}
