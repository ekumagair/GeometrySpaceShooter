using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocaleDropdown : MonoBehaviour
{
    private Dropdown _dropdown;
    private float _timeSinceEnable = 0;

    void Start()
    {
        _dropdown = GetComponent<Dropdown>();

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

    public void LocaleSelected()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_dropdown.value];

        if (_timeSinceEnable > 0.2f)
        {
            PersistentCanvas.reference.CreateButtonSound(5);
        }
    }

    public void OnDropdownClick()
    {
        PersistentCanvas.reference.CreateButtonSound(3);
    }
}