using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PresetLevelButton : MonoBehaviour
{
    public int mustCompleteLevel = 0;
    public TMP_Text textLocked;
    public TMP_Text textUnlocked;

    Button button;

    void Start()
    {
        button = GetComponent<Button>();

        if (GameStats.level > mustCompleteLevel)
        {
            button.interactable = true;
            textLocked.enabled = false;
            textUnlocked.enabled = true;
        }
        else
        {
            button.interactable = false;
            textLocked.enabled = true;
            textUnlocked.enabled = false;
        }
    }
}
