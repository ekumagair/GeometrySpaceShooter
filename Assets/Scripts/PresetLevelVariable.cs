using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class PresetLevelVariable : IVariable
{
    public string xmlText;

    public object GetSourceValue(ISelectorInfo selector)
    {
        return PresetLevels.IntToLevelName(PresetLevels.currentPresetLevel);
    }
}
