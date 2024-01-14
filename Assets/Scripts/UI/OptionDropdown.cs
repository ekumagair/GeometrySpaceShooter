using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class OptionDropdown : MonoBehaviour
{
    public enum DropdownType
    {
        Locale,
        Background
    }
    public DropdownType optionType;

    private Dropdown _dropdown;
    private float _timeSinceEnable = 0;

    void Start()
    {
        _dropdown = GetComponent<Dropdown>();

        switch (optionType)
        {
            case DropdownType.Locale:
                Locale currentSelectedLocale = LocalizationSettings.SelectedLocale;
                ILocalesProvider availableLocales = LocalizationSettings.AvailableLocales;

                if (currentSelectedLocale == availableLocales.GetLocale("en"))
                {
                    _dropdown.value = 0;
                }
                else if (currentSelectedLocale == availableLocales.GetLocale("pt-BR"))
                {
                    _dropdown.value = 1;
                }
                break;

            case DropdownType.Background:
                _dropdown.value = Options.backgroundType;
                break;

            default:
                Debug.LogWarning(gameObject.name + ": Invalid dropdown type.");
                break;
        }
    }

    private void OnEnable()
    {
        _timeSinceEnable = 0;
    }

    private void Update()
    {
        if (_timeSinceEnable < float.MaxValue - 1f)
        {
            _timeSinceEnable += Time.deltaTime;
        }
    }

    public void OptionSelected()
    {
        switch (optionType)
        {
            case DropdownType.Locale:
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_dropdown.value];
                break;

            case DropdownType.Background:
                Options.backgroundType = _dropdown.value;
                break;

            default:
                Debug.LogWarning(gameObject.name + ": Selected invalid dropdown type.");
                break;
        }

        if (_timeSinceEnable > 0.2f)
        {
            PersistentCanvas.reference.CreateButtonSound(5);
        }

        GameStats.SaveStats();
    }

    public void OnDropdownClick()
    {
        PersistentCanvas.reference.CreateButtonSound(3);
    }
}