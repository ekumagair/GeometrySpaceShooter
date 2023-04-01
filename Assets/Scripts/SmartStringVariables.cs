using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class SmartStringVariables : MonoBehaviour
{
    [SerializeField] private LocalizedString localizedString;
    [SerializeField] private TMP_Text tmpText;

    private void OnEnable()
    {
        localizedString.Arguments = new object[] {0, 1};
        localizedString.StringChanged += UpdateText;
    }

    private void OnDisable()
    {
        localizedString.StringChanged -= UpdateText;
    }

    private void UpdateText(string value)
    {
        tmpText.text = value;
    }

    public void RefreshString()
    {
        localizedString.Arguments[0] = GameStats.level;
        localizedString.Arguments[1] = GameStats.points;
        localizedString.RefreshString();
    }
}
