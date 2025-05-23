using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if !DISABLE_ADS
using UnityEngine.Advertisements;
#endif

public class AdButton : MonoBehaviour
{
    public bool destroyIfMultipliedScore = false;
    public bool constantlyCheckConditions = false;
    public AdRewardedManager.RewardType rewardType = AdRewardedManager.RewardType.NONE;

    private Upgrade _upgradeScript;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();

#if DISABLE_ADS
        gameObject.SetActive(false);
#endif
    }

    void Start()
    {
        if (GameStats.enableAdButttons == false || AdsInitializer.failed == true)
        {
            _button.interactable = false;
        }
    }

    private void OnEnable()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }

        _button.onClick.AddListener(DisplayAd);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(DisplayAd);
    }

    void Update()
    {
        if (destroyIfMultipliedScore == true && GameStats.multipliedCurrentScore == true && gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
        }
        if (constantlyCheckConditions == true)
        {
            EnableAdButton();
        }
    }

    public void EnableAdButton()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }

        if (AdRewardedManager.instance != null)
        {
            _button.interactable = GeneralAdCheck() && MultiplicationAdCheck();
        }
        else
        {
            _button.interactable = false;
        }
    }

    private bool GeneralAdCheck()
    {
#if !DISABLE_ADS
        return GameStats.enableAdButttons == true && Advertisement.isInitialized == true && AdsInitializer.failed == false && AdRewardedManager.instance.HasAnyError() == false && GameStats.failedGenuine == false;
#else
        return false;
#endif
    }

    private bool MultiplicationAdCheck()
    {
        if (rewardType != AdRewardedManager.RewardType.MULTIPLY_SCORE)
        {
            return true;
        }
        else
        {
            return GameStats.currentLevelPoints > 0 && GameStats.points < GameConstants.MAX_POINTS && GameStats.currentLevelPoints < GameConstants.MAX_POINTS;
        }
    }

    public void DisplayAd()
    {
#if !DISABLE_ADS
        if (rewardType == AdRewardedManager.RewardType.UPGRADE && _upgradeScript == null)
        {
            _upgradeScript = gameObject.transform.parent.gameObject.GetComponent<Upgrade>();
        }

        if (rewardType == AdRewardedManager.RewardType.UPGRADE && _upgradeScript != null)
        {
            AdRewardedManager.instance.upgradeScript = _upgradeScript;
        }
        else
        {
            AdRewardedManager.instance.upgradeScript = null;
        }

        AdRewardedManager.instance.ShowAd(rewardType);
#endif
    }
}
