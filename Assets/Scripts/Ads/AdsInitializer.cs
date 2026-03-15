using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !DISABLE_ADS
using UnityEngine.Advertisements;
#endif

#if !DISABLE_ADS
public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
#else
public class AdsInitializer : MonoBehaviour
#endif
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode; // Use fake ads for tests. Only use real ads for the final build.
    private string _gameId;

    public static bool failed = false;
    public static AdsInitializer instance = null;

#if !DISABLE_ADS
    void Awake()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        _testMode = true;
#endif

        SetMetaData();
        InitializeAds();
    }

    private void SetMetaData()
    {
        if (!Init.IsPlayerAdult)
        {
            // Disable targeted ads if the player is not an adult.

            // COPPA
            MetaData userMeta = new MetaData("user");
            userMeta.Set("nonbehavioral", "true");
            Advertisement.SetMetaData(userMeta);

            // GDPR/CCPA
            MetaData privacyMeta = new MetaData("privacy");
            privacyMeta.Set("consent", "false");
            Advertisement.SetMetaData(privacyMeta);

            // GDPR
            MetaData gdprMeta = new MetaData("gdpr");
            gdprMeta.Set("consent", "false");
            Advertisement.SetMetaData(gdprMeta);
        }
    }

    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, this);
    }

    public void OnInitializationComplete()
    {
        if (Debug.isDebugBuild) { Debug.Log("Unity Ads initialization complete."); }
        failed = false;
        instance = this;
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        failed = true;
        instance = this;
    }
#endif

    public bool IsTesting()
    {
        return _testMode;
    }
}
