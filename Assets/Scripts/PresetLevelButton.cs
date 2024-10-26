using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PresetLevelButton : MonoBehaviour
{
    public int mustCompleteLevel = 0;
    public int extraLevelID = 0;
    public TMP_Text textLocked;
    public TMP_Text textUnlocked;
    public Image check;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        SetInteractable();
    }

    void Start()
    {
        SetInteractable();
    }

    public void SetInteractable()
    {
        if (GameStats.level > mustCompleteLevel)
        {
            _button.interactable = true;
            textLocked.enabled = false;
            textUnlocked.enabled = IsUnlocked();
            check.enabled = IsCompleted();
        }
        else
        {
            _button.interactable = false;
            textLocked.enabled = true;
            textUnlocked.enabled = false;
            check.enabled = false;
        }
    }

    public bool IsUnlocked()
    {
        return GameStats.completedExtraLevels[extraLevelID] == 0 && GameStats.level > mustCompleteLevel;
    }

    public bool IsCompleted()
    {
        return GameStats.completedExtraLevels[extraLevelID] == 1 && GameStats.level > mustCompleteLevel;
    }
}
