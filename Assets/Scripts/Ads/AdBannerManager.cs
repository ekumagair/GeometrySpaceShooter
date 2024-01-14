using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdBannerManager : MonoBehaviour
{
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    [SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms.

    [HideInInspector] public bool loaded = false;

    public static AdBannerManager instance = null;

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
        // Set the banner position:
        Advertisement.Banner.SetPosition(_bannerPosition);

        while (Advertisement.isInitialized == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;

        // Load banner.
        LoadBanner();

        while (loaded == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;

        // Show banner.
        ShowBannerAd();
    }

    // Implement a method to call when the Load Banner button is clicked:
    public void LoadBanner()
    {
        loaded = false;

        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_adUnitId, options);
    }

    // Implement code to execute when the loadCallback event triggers:
    void OnBannerLoaded()
    {
        loaded = true;
        instance = this;
        if (Debug.isDebugBuild) { Debug.Log("Finished loading: " + _adUnitId); }
    }

    // Implement code to execute when the load errorCallback event triggers:
    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
        // Optionally execute additional code, such as attempting to load another ad.
    }

    // Implement a method to call when the Show Banner button is clicked:
    private void ShowBannerAd()
    {
        if (PurchaseManager.instance.HasRemovedAds() == true)
        {
            if (Debug.isDebugBuild) { Debug.LogWarning("Tried to show banner ad, but ads were removed."); }
            return;
        }

        // Set up options to notify the SDK of show events:
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        // Show the loaded Banner Ad Unit:
        Advertisement.Banner.Show(_adUnitId, options);
    }

    // Implement a method to call when the Hide Banner button is clicked:
    public void HideBannerAd()
    {
        // Hide the banner:
        Advertisement.Banner.Hide();
    }

    void OnBannerClicked() { }

    void OnBannerShown() { }

    void OnBannerHidden() { }

    void OnDestroy() { }
}
