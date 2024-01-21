using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardGoal : MonoBehaviour
{
    [Header("Properties")]
    public int enemyTarget;
    public int claimedRewardsIndex;
    public int claimedRewardsValue = 1;

    [Header("Rewards")]
    public int starReward = 0;
    public float multiplierReward = 0.0f;
    public Upgrade.UpgradeType upgradeType = Upgrade.UpgradeType.None;
    public float upgradeIncrement = 0.0f;

    [Header("Components")]
    public TMP_Text enemyTargetText;
    public Button claimButton;
    public ParticleSystem claimParticles;
    public VerticalLayoutGroup rewardDisplayGroup;
    public Image trophyIcon;

    [Space]

    public RectTransform starRewardRoot;
    public TMP_Text starRewardText;

    [Space]

    public RectTransform multiplierRewardRoot;
    public TMP_Text multiplierRewardText;

    [Space]

    public RectTransform changeStatRoot;
    public TMP_Text changeStatText;

    private RectTransform _rectTransform;
    private string _localizedStatDescription;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = new Vector2(0, -enemyTarget);
        claimParticles.gameObject.SetActive(false);
        SetInfos();
    }

    void Start()
    {
        SetInfos();
    }

    void OnEnable()
    {
        if (IsAlreadyClaimed() && claimParticles != null)
        {
            claimParticles.gameObject.SetActive(false);
        }
        if (upgradeType != Upgrade.UpgradeType.None)
        {
            _localizedStatDescription = Upgrade.GetUpgradeName(upgradeType).GetLocalizedString();
        }
    }

    void Update()
    {
        SetInfos();
    }

    public void SetInfos()
    {
        claimButton.gameObject.SetActive(!IsAlreadyClaimed());
        claimButton.interactable = IsUnlocked();
        trophyIcon.enabled = !claimButton.gameObject.activeInHierarchy;

        enemyTargetText.text = enemyTarget.ToString();

        rewardDisplayGroup.gameObject.SetActive(true);

        starRewardRoot.gameObject.SetActive(starReward > 0 && !IsAlreadyClaimed());
        starRewardText.text = "+" + starReward.ToString();

        multiplierRewardRoot.gameObject.SetActive(multiplierReward > 1.0f && multiplierReward <= ScoreChain.scoreMultiplierMinimum);
        multiplierRewardText.text = "x" + multiplierReward.ToString();

        changeStatRoot.gameObject.SetActive(upgradeType != Upgrade.UpgradeType.None);

        if (changeStatRoot.gameObject.activeInHierarchy)
        {
            changeStatText.text = _localizedStatDescription + ": " + ((upgradeIncrement > 0) ? "+" : "") + (upgradeIncrement * Upgrade.GetUpgradeDisplayMultiplier(upgradeType)).ToString();
        
            if (Upgrade.GetUpgradeDisplayMultiplier(upgradeType) == 100f)
            {
                changeStatText.text += "%";
            }
        }
    }

    public bool IsUnlocked()
    {
        return GameStats.enemiesKilledTotal >= enemyTarget;
    }

    public bool IsAlreadyClaimed()
    {
        return GameStats.claimedRewards[claimedRewardsIndex] >= claimedRewardsValue;
    }

    public void Claim()
    {
        GameStats.claimedRewards[claimedRewardsIndex] = claimedRewardsValue;

        // Give points.
        if (starReward > 0)
        {
            GameStats.AddPoints(starReward);
            PersistentCanvas.reference.CreateNumberChangeEffect(HUD.hudTopLeftCorner, "+" + starReward.ToString(), Color.green, -0.55f, 0.25f);
        }

        // Increase score multiplier minimum.
        if (multiplierReward > 1.0f && multiplierReward > ScoreChain.scoreMultiplierMinimum)
        {
            ScoreChain.scoreMultiplierMinimum = multiplierReward;
        }

        // Change stats.
        if (upgradeType != Upgrade.UpgradeType.None && upgradeIncrement != 0.0f)
        {
            Upgrade.ChangePlayerStat(upgradeType, upgradeIncrement);
        }

        GameStats.claimedRewardsTotal++;
        GameStats.SaveStats();
        PersistentCanvas.reference.CreateButtonSound(4);

        // Show ad.
        if (AdInterstitialManager.instance != null && PurchaseManager.instance.HasRemovedAds() == false && GameStats.claimedRewardsTotal > 1)
        {
            AdInterstitialManager.instance.ShowAd();
        }

        claimParticles.gameObject.SetActive(true);
        claimParticles.Play();
    }
}
