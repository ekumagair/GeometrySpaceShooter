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

    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();

        if (GameStats.level > mustCompleteLevel)
        {
            _button.interactable = true;
            textLocked.enabled = false;
            textUnlocked.enabled = true;
        }
        else
        {
            _button.interactable = false;
            textLocked.enabled = true;
            textUnlocked.enabled = false;
        }
    }
}
