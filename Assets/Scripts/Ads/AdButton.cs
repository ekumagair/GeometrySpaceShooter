using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

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
        if (destroyIfMultipliedScore == true && GameStats.multipliedCurrentScore == true)
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
            _button.interactable = GameStats.enableAdButttons == true && Advertisement.isInitialized == true && AdsInitializer.failed == false && AdRewardedManager.instance.HasAnyError() == false;
        }
        else
        {
            _button.interactable = false;
        }
    }

    public void DisplayAd()
    {
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
    }
}
