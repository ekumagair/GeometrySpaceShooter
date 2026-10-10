using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;

#if !DISABLE_ADS
using UnityEngine.Advertisements;
#endif

public class Init : MonoBehaviour
{
    public static bool IsPlayerAdult { get; private set; } = false;

    public TMP_Text debugText;

    private bool _testMode;
    private bool _appliedPrivacySettings = false;
    private string _privacySettings = "";

    #region Init

    void Start()
    {
#if SERVICE_TEST || DEVELOPMENT_BUILD || UNITY_EDITOR
        _testMode = true;
#endif

        StartCoroutine(InitCoroutine());
    }

    private IEnumerator InitCoroutine()
    {
        if (_testMode)
        {
            debugText.gameObject.SetActive(true);
            AppendDebugText("Init begin. " + Application.productName + " version " + Application.version.ToString());

            yield return new WaitForSeconds(1);
        }
        else
        {
            debugText.gameObject.SetActive(false);
        }

        yield return null;

        // Age Verification.
        RequestAgeSignals();

        while (!_appliedPrivacySettings)
            yield return null;

        if (_testMode)
        {
            AppendDebugText("privacySettings = [" + _privacySettings + "]");
            AppendDebugText("Player considered adult? [" + IsPlayerAdult.ToString() + "]", "#7477ff");

            yield return new WaitForSeconds(3);

            for (int i = 6; i > 0; i--)
            {
                AppendDebugText("Init finished. Starting in " + i.ToString());
                yield return new WaitForSeconds(1);
            }

            Debug.Log("Init finished. Going to StartScene.");
        }

        LoadingScreen.CallLoadScreen(LoadingScreen.Scenes.Start, true);
    }

    #endregion

    #region Debug

    private void AppendDebugText(string s, string color = "#FFFFFF", bool error = false)
    {
        if (!_testMode)
        {
            debugText.text = "";
            return;
        }

        if (debugText.text != "")
            debugText.text += "<br>";

        debugText.text += "<color=" + color + ">";
        debugText.text += s;
        debugText.text += "</color>";

        if (!error)
            Debug.Log(s);
        else
            Debug.LogError(s);
    }

    #endregion

    #region Age Verification

    private void RequestAgeSignals()
    {
        if (_testMode)
        {
            AppendDebugText("Request age signals [start].");
        }

#if UNITY_ANDROID && !UNITY_EDITOR && !DISABLE_JAVA
        using (AndroidJavaClass plugin = new AndroidJavaClass("com.EduardoKumagai.GeometrySpaceShooter.agesignals.AgeSignalsPlugin"))
        {
            plugin.CallStatic("CheckAgeSignals", gameObject.name);
        }
#else
        // Fallback for editor testing.
        OnAgeSignalsResult("UNKNOWN||");
#endif

        if (_testMode)
        {
            AppendDebugText("Requested age signals [end].");
        }
    }

    public void OnAgeSignalsDebug(string message)
    {
        AppendDebugText("[From java] " + message, "#ffe7a6");
    }

    public void OnAgeSignalsResult(string data)
    {
        ApplyPrivacySettings(data);
    }

    public void OnAgeSignalsError(string error)
    {
        // Safest fallback: treat as child.
        RestrictedAgeSettings();

        _appliedPrivacySettings = true;
        _privacySettings = "";

        if (_testMode)
        {
            AppendDebugText("Age Signals failed: " + error, "#ff2929", true);
        }
    }

    private void ApplyPrivacySettings(string data)
    {
        var parts = data.Split('|');
        string installId = parts[0];
        int ageLower = 0;
        int ageUpper = 0;

        if (parts.Length > 1)
        {
            int.TryParse(parts[1], out ageLower);
            int.TryParse(parts[2], out ageUpper);
        }

        if (ageLower < 18 || ageUpper < 18)
        {
            RestrictedAgeSettings();
        }
        else
        {
            IsPlayerAdult = true;
        }

        _appliedPrivacySettings = true;
        _privacySettings = data;

        if (_testMode)
        {
            AppendDebugText("Status: [" + installId + "]", "#bfbfbf");
            AppendDebugText("Age lower: [" + ageLower.ToString() + "]", "#bfbfbf");
            AppendDebugText("Age upper: [" + ageUpper.ToString() + "]", "#bfbfbf");

#if !UNITY_EDITOR && !DISABLE_JAVA
            AppendDebugText("Age Signals finished: [" + data + "]", "#29ff34");
#else
            AppendDebugText("Age Signals not used: [" + data + "]", "#29ff34");
#endif
        }
    }

    private void RestrictedAgeSettings()
    {
        Analytics.initializeOnStartup = false;
        Analytics.enabled = false;
        Analytics.limitUserTracking = true;
        Analytics.deviceStatsEnabled = false;
        PerformanceReporting.enabled = false;

        IsPlayerAdult = false;
    }

    #endregion
}