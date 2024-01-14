using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class Upgrade : MonoBehaviour
{
    [Header("Components")]
    public TMP_Text title;
    public TMP_Text currentStat;
    public TMP_Text nextStat;
    public TMP_Text priceDisplay;
    public Button buyButton;
    public Button adButton;

    [Header("Localization")]
    public LocalizedString descriptionLocalization;

    [Header("Properties")]
    public float statIncrement;
    public int price;
    public float priceMultiplier = 1.1f;

    public enum UpgradeType
    {
        FiringSpeed,
        ExtraHealth,
        ProjectileSpeed,
        ProjectileDamage,
        ShootLevel,
        TouchFollowSpeed,
        Perforation,
        Laser
    }
    public UpgradeType upgradeType;

    [HideInInspector] public uint amountPurchased = 0;

    private AdButton _adButtonScript;
    private LocalizeStringEvent _titleLocalization = null;

    void Start()
    {
        DisplayInfo();
        ButtonCheck();
    }

    private void OnEnable()
    {
        // Make buttons disabled by default.
        if (adButton != null)
        {
            _adButtonScript = adButton.GetComponent<AdButton>();
            adButton.interactable = false;
        }
        if (buyButton != null)
        {
            buyButton.interactable = false;
        }

        // Get title localization.
        if (_titleLocalization == null)
        {
            _titleLocalization = title.gameObject.GetComponent<LocalizeStringEvent>();
        }
    }

    void Update()
    {
        DisplayInfo();
        ButtonCheck();
    }

    public void ButtonCheck()
    {
        if (GameStats.points < price)
        {
            // If you don't have enough points, disable buy button and enable ad button.
            buyButton.interactable = false;

            if (adButton != null && AdRewardedManager.instance != null)
            {
                if (AdRewardedManager.instance.loaded == true && _adButtonScript != null)
                {
                    _adButtonScript.EnableAdButton();
                }
                else
                {
                    adButton.interactable = false;
                }
            }
        }
        else
        {
            // If you have enough points, enable buy button and disable ad button.
            buyButton.interactable = true;

            if (adButton != null)
            {
                adButton.interactable = false;
            }
        }
    }

    public void DisplayInfo()
    {
        priceDisplay.text = price.ToString();

        switch (upgradeType)
        {
            case UpgradeType.FiringSpeed:
                currentStat.text = "+" + Mathf.RoundToInt((Player.firingSpeedDivider - 1.0f) * 100).ToString() + "%";
                nextStat.text = "+" + Mathf.RoundToInt((Player.firingSpeedDivider - (1.0f - statIncrement)) * 100).ToString() + "%";
                break;

            case UpgradeType.ExtraHealth:
                currentStat.text = Player.startHealth.ToString();
                nextStat.text = (Player.startHealth + statIncrement).ToString();
                break;

            case UpgradeType.ProjectileSpeed:
                currentStat.text = "+" + Mathf.RoundToInt((Player.projectileSpeedMultiplier - 1.0f) * 100).ToString() + "%";
                nextStat.text = "+" + Mathf.RoundToInt((Player.projectileSpeedMultiplier - (1.0f - statIncrement)) * 100).ToString() + "%";
                break;

            case UpgradeType.ProjectileDamage:
                currentStat.text = Player.projectileDamage.ToString();
                nextStat.text = (Player.projectileDamage + statIncrement).ToString();
                break;

            case UpgradeType.ShootLevel:
                currentStat.text = Player.shootLevel.ToString();
                nextStat.text = (Player.shootLevel + statIncrement).ToString();
                break;

            case UpgradeType.TouchFollowSpeed:
                currentStat.text = "+" + Mathf.RoundToInt((Player.moveSpeedMultiplier - 1.0f) * 100).ToString() + "%";
                nextStat.text = "+" + Mathf.RoundToInt((Player.moveSpeedMultiplier - (1.0f - statIncrement)) * 100).ToString() + "%";
                break;

            case UpgradeType.Perforation:
                currentStat.text = Player.projectilePerforation.ToString();
                nextStat.text = (Player.projectilePerforation + statIncrement).ToString();
                break;

            case UpgradeType.Laser:
                currentStat.text = Player.projectileAutoDamage.ToString();
                nextStat.text = (Player.projectileAutoDamage + statIncrement + AmountPurchasedAsInt()).ToString();
                break;

            default:
                break;
        }
    }

    public void BuyUpgradeWithPoints()
    {
        BuyUpgrade(true, true);
    }

    public void BuyUpgradeFromAd()
    {
        BuyUpgrade(false, !PurchaseManager.instance.HasRemovedAds());
    }

    private void BuyUpgrade(bool spendPoints, bool increasePrice)
    {
        if (GameStats.points < price && spendPoints == true)
        {
            if (Debug.isDebugBuild) { Debug.Log("Not enough points for upgrade " + upgradeType); }
            return;
        }

        switch (upgradeType)
        {
            case UpgradeType.FiringSpeed:
                Player.firingSpeedDivider += statIncrement;
                break;

            case UpgradeType.ExtraHealth:
                Player.startHealth += Mathf.RoundToInt(statIncrement);
                break;

            case UpgradeType.ProjectileSpeed:
                Player.projectileSpeedMultiplier += statIncrement;
                break;

            case UpgradeType.ProjectileDamage:
                Player.projectileDamage += Mathf.RoundToInt(statIncrement);
                break;

            case UpgradeType.ShootLevel:
                Player.shootLevel += Mathf.RoundToInt(statIncrement);
                break;

            case UpgradeType.TouchFollowSpeed:
                Player.moveSpeedMultiplier += statIncrement;
                break;

            case UpgradeType.Perforation:
                Player.projectilePerforation += Mathf.RoundToInt(statIncrement);
                break;

            case UpgradeType.Laser:
                Player.projectileAutoDamage += Mathf.RoundToInt(statIncrement) + AmountPurchasedAsInt();
                break;

            default:
                break;
        }

        if (spendPoints == true)
        {
            GameStats.points -= price;

            if (PersistentCanvas.reference != null && price > 0)
            {
                // Show spent points on the top left corner of the screen.
                PersistentCanvas.reference.CreateNumberChangeEffect(HUD.hudTopLeftCorner, "-" + price.ToString(), new Color(1f, 0.5f, 0.5f), -0.5f, 1);
            }
        }

        // Increase price.
        if (price < GameConstants.UPGRADE_MAX_PRICE && increasePrice == true)
        {
            price = Mathf.RoundToInt(price * priceMultiplier);
        }

        // Price limit.
        if (price > GameConstants.UPGRADE_MAX_PRICE)
        {
            price = GameConstants.UPGRADE_MAX_PRICE;
        }
        if (price < 0)
        {
            price = 0;
        }

        // Amount of purchases.
        if (amountPurchased < uint.MaxValue)
        {
            amountPurchased += 1;
        }
        if (GameStats.upgradePurchaseAmount[(int)upgradeType] < uint.MaxValue)
        {
            GameStats.upgradePurchaseAmount[(int)upgradeType] += 1;
        }

        // Save stats.
        GameStats.upgradePrice[(int)upgradeType] = price;
        GameStats.SaveStats();

        if (Debug.isDebugBuild) { Debug.Log("Purchased upgrade: ID " + (int)upgradeType + ", Amount " + GameStats.upgradePurchaseAmount[(int)upgradeType] + ", Price " + GameStats.upgradePrice[(int)upgradeType]); }
    }

    public int AmountPurchasedAsInt()
    {
        return (int)Mathf.Clamp(amountPurchased, 0, int.MaxValue);
    }

    public void AboutPopUp()
    {
        if (PersistentCanvas.reference != null)
        {
            PersistentCanvas.reference.CreateButtonSound(5);
        }

        PopUp.instance.SetButtonsTexts(new LocalizedString("PopUp", "button_ok"), null, null, null);
        PopUp.instance.OpenPopUp(_titleLocalization.StringReference, descriptionLocalization, 1);
    }
}
