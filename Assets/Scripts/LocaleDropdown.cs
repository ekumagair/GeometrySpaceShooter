using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocaleDropdown : MonoBehaviour
{
    Dropdown dropdown;

    void Start()
    {
        dropdown = GetComponent<Dropdown>();

        Locale currentSelectedLocale = LocalizationSettings.SelectedLocale;
        ILocalesProvider availableLocales = LocalizationSettings.AvailableLocales;

        if (currentSelectedLocale == availableLocales.GetLocale("en"))
        {
            dropdown.value = 0;
        }
        else if (currentSelectedLocale == availableLocales.GetLocale("pt-BR"))
        {
            dropdown.value = 1;
        }
    }

    public void LocaleSelected()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[dropdown.value];
    }
}