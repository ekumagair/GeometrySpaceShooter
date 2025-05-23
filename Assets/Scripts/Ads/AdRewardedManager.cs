using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !DISABLE_ADS
using UnityEngine.Advertisements;
#endif

#if !DISABLE_ADS
public class AdRewardedManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
#else
public class AdRewardedManager : MonoBehaviour
#endif
{
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms

    public static AdRewardedManager instance = null;

    public enum RewardType
    {
        NONE,
        UPGRADE,
        MULTIPLY_SCORE,
        REVIVE,
    }
    [HideInInspector] public RewardType currentReward = RewardType.NONE;

    [HideInInspector] public bool loaded = false;
    [HideInInspector] public bool errorNoFill = false;
    [HideInInspector] public bool errorInternal = false;
    [HideInInspector] public bool errorInitializeFailed = false;
    [HideInInspector] public bool errorUnknown = false;
    [HideInInspector] public Upgrade upgradeScript;

    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif
    }

    private IEnumerator Start()
    {
#if !DISABLE_ADS
        while (Advertisement.isInitialized == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;

        // Load reward ad.
        LoadAd();
#else
        yield return null;
#endif
    }

#if !DISABLE_ADS
    // Load content to the Ad Unit:
    public void LoadAd()
    {
        ResetErrorFlags();

        // IMPORTANT! Only load content AFTER initialization (initialization is handled in a different script).
        if (Debug.isDebugBuild) { Debug.Log("Loading Ad: " + _adUnitId); }
        loaded = false;
        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId) 
    {
        loaded = true;
        instance = this;
        if (Debug.isDebugBuild) { Debug.Log("Finished loading: " + _adUnitId); }
    }
#endif

    public void ShowAd(RewardType reward)
    {
#if !DISABLE_ADS
        currentReward = reward;
        Advertisement.Show(_adUnitId, this);
#endif
    }

#if !DISABLE_ADS
    // Give reward if ad is completed.
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {

        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            if (Debug.isDebugBuild) { Debug.Log("Unity Ads Rewarded Ad Completed"); }
            if (Debug.isDebugBuild) { Debug.Log("Current Ad reward: " + currentReward); }

            // Grant a reward.
            switch (currentReward)
            {
                case RewardType.UPGRADE:
                    if (upgradeScript != null)
                    {
                        upgradeScript.BuyUpgradeFromAd();
                    }
                    break;

                case RewardType.MULTIPLY_SCORE:
                    if (GameplayManager.instance != null)
                    {
                        GameplayManager.instance.MultiplyCurrentScoreBy2();
                    }
                    break;

                case RewardType.REVIVE:
                    if (GameplayManager.instance != null)
                    {
                        GameplayManager.instance.RevivePlayer();
                    }
                    break;

                default:
                    if (Debug.isDebugBuild) { Debug.LogWarning("Reward ad didn't give any rewards!"); }
                    break;
            }
        }

        currentReward = RewardType.NONE;
        upgradeScript = null;

        LoadAd();
}
#endif

    public void ResetErrorFlags()
    {
        errorNoFill = false;
        errorInternal = false;
        errorInitializeFailed = false;
    }

    public bool HasAnyError()
    {
        return errorNoFill == true || errorInternal == true || errorInitializeFailed == true || errorUnknown == true;
    }

#if !DISABLE_ADS
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        ResetErrorFlags();

        switch (error)
        {
            case UnityAdsLoadError.INITIALIZE_FAILED:
                errorInitializeFailed = true;
                break;

            case UnityAdsLoadError.INTERNAL_ERROR:
                errorInternal = true;
                break;

            case UnityAdsLoadError.INVALID_ARGUMENT:
                break;

            case UnityAdsLoadError.NO_FILL:
                errorNoFill = true;
                break;

            case UnityAdsLoadError.TIMEOUT:
                break;

            case UnityAdsLoadError.UNKNOWN:
                errorUnknown = true;
                break;

            default:
                break;
        }

        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId) { }

    public void OnUnityAdsShowClick(string adUnitId) { }
#endif
}
