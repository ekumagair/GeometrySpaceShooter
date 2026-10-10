using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !DISABLE_ADS
using UnityEngine.Advertisements;
#endif

#if !DISABLE_ADS
public class AdInterstitialManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
#else
public class AdInterstitialManager : MonoBehaviour
#endif
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOSAdUnitId = "Interstitial_iOS";
    string _adUnitId = null;

    public static AdInterstitialManager Instance { get; private set; } = null;

    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSAdUnitId
            : _androidAdUnitId;
    }

#if !DISABLE_ADS
    private IEnumerator Start()
    {
        while (Advertisement.isInitialized == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;


        // Load reward ad.
        LoadAd();
    }
#endif

    // Load content to the Ad Unit:
    public void LoadAd()
    {
#if !DISABLE_ADS
        // IMPORTANT! Only load content AFTER initialization (initialization is handled in a different script).
        if (Debug.isDebugBuild) { Debug.Log("Loading Ad: " + _adUnitId); }

        Advertisement.Load(_adUnitId, this);
#endif
    }

    // Show the loaded content in the Ad Unit:
    public void ShowAd()
    {
#if !DISABLE_ADS
        if (PurchaseManager.Instance.HasRemovedAds() == true)
        {
            if (Debug.isDebugBuild) { Debug.LogWarning("Tried to show interstitial ad, but ads were removed."); }
            return;
        }

        // Note that if the ad content wasn't previously loaded, this method will fail
        if (Debug.isDebugBuild) { Debug.Log("Showing Ad: " + _adUnitId); }

        Advertisement.Show(_adUnitId, this);
#endif
    }

#if !DISABLE_ADS
    // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
        Instance = this;

        if (Debug.isDebugBuild) { Debug.Log("Finished loading: " + _adUnitId); }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId) { }

    public void OnUnityAdsShowClick(string adUnitId) { }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) { }
#endif
}
