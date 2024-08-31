using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
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
#endif

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

    public bool IsTesting()
    {
        return _testMode;
    }
}
