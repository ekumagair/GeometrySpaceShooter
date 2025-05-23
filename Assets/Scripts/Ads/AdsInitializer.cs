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
    [SerializeField] bool _testMode = true; // Use fake ads for tests. Only use real ads for the final build.
    private string _gameId;

    public static bool failed = false;
    public static AdsInitializer instance = null;

#if !DISABLE_ADS
    void Awake()
    {
        InitializeAds();
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
