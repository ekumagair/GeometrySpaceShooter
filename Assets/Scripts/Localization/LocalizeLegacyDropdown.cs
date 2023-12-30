using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizeLegacyDropdown : MonoBehaviour
{
    [SerializeField] private List<LocalizedString> dropdownOptions;
    private Dropdown _legacyDropdown;
    private Locale _currentLocale;

    private void Awake()
    {
        List<Dropdown.OptionData> tmpDropdownOptions = new List<Dropdown.OptionData>();
        for (int i = 0; i < dropdownOptions.Count; i++)
        {
            tmpDropdownOptions.Add(new Dropdown.OptionData(dropdownOptions[i].GetLocalizedString()));
        }
        if (!_legacyDropdown) _legacyDropdown = GetComponent<Dropdown>();
        _legacyDropdown.options = tmpDropdownOptions;
    }

    private void ChangedLocale(Locale newLocale)
    {
        if (_currentLocale == newLocale) return;
        _currentLocale = newLocale;
        List<Dropdown.OptionData> tmpDropdownOptions = new List<Dropdown.OptionData>();
        for (int i = 0; i < dropdownOptions.Count; i++)
        {
            tmpDropdownOptions.Add(new Dropdown.OptionData(dropdownOptions[i].GetLocalizedString()));
        }
        _legacyDropdown.options = tmpDropdownOptions;
    }

    private void Update()
    {
        LocalizationSettings.SelectedLocaleChanged += ChangedLocale;
    }
}