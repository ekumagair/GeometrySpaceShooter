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
    public static bool IsPlayerAdult { get; private set; }

    public bool testMode;
    public TMP_Text debugText;

    private bool appliedPrivacySettings = false;
    private string privacySettings = "";

    #region Init

    void Start()
    {
        if (Debug.isDebugBuild)
        {
            testMode = true;
        }

        StartCoroutine(InitCoroutine());
    }

    private IEnumerator InitCoroutine()
    {
        if (testMode)
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

        while (!appliedPrivacySettings)
            yield return null;

        if (testMode)
        {
            AppendDebugText("privacySettings = [" + privacySettings + "]");
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

    private void AppendDebugText(string s, string color = "#FFFFFF")
    {
        if (!testMode)
        {
            debugText.text = "";
            return;
        }

        if (debugText.text != "")
            debugText.text += "<br>";

        debugText.text += "<color=" + color + ">";
        debugText.text += s;
        debugText.text += "</color>";
    }

    #endregion

    #region Age Verification

    private void RequestAgeSignals()
    {
        if (testMode)
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

        if (testMode)
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

        appliedPrivacySettings = true;
        privacySettings = "";

        if (testMode)
        {
            Debug.LogError("Age Signals failed: " + error);
            AppendDebugText("Age Signals failed: " + error, "#ff2929");
        }
    }

    private void ApplyPrivacySettings(string data)
    {
        var parts = data.Split('|');
        string status = parts[0];
        int ageLower = 0;
        int ageUpper = 0;

        if (parts.Length > 1)
        {
            int.TryParse(parts[1], out ageLower);
            int.TryParse(parts[2], out ageUpper);
        }

        if (status != "VERIFIED" && status != "DECLARED")
        {
            RestrictedAgeSettings();
        }
        else if (ageLower < 18 || ageUpper < 18)
        {
            RestrictedAgeSettings();
        }
        else
        {
            IsPlayerAdult = true;
        }

        appliedPrivacySettings = true;
        privacySettings = data;

        if (testMode)
        {
            AppendDebugText("Status: [" + status + "]", "#bfbfbf");
            AppendDebugText("Age lower: [" + ageLower.ToString() + "]", "#bfbfbf");
            AppendDebugText("Age upper: [" + ageUpper.ToString() + "]", "#bfbfbf");

#if !UNITY_EDITOR && !DISABLE_JAVA
            Debug.Log("Age Signals success: " + data);
            AppendDebugText("Age Signals success: [" + data + "]", "#29ff34");
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