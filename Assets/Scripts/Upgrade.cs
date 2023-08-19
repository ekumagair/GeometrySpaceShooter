using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class Upgrade : MonoBehaviour
{
    public TMP_Text currentStat;
    public TMP_Text nextStat;
    public TMP_Text priceDisplay;

    public float statIncrement;
    public int price;
    public float priceMultiplier = 1.1f;
    public Button buyButton;
    public Button adButton;

    public enum UpgradeType
    {
        FiringSpeed,
        ExtraHealth,
        ProjectileSpeed,
        ProjectileDamage,
        ShootLevel,
        TouchFollowSpeed
    }
    public UpgradeType upgradeType;

    PersistentCanvas persistentCanvas;

    void Start()
    {
        persistentCanvas = GameObject.Find("PersistentCanvas").GetComponent<PersistentCanvas>();

        // Make ad buttons disabled by default.
        if (adButton != null)
        {
            adButton.interactable = false;
        }
    }

    void Update()
    {
        priceDisplay.text = price.ToString();

        if (GameStats.points < price)
        {
            // If you don't have enough points, disable buy button and enable ad button.
            buyButton.interactable = false;

            if (adButton != null && GameStats.enableAdButttons == true && Advertisement.isInitialized == true)
            {
                adButton.interactable = true;
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

            default:
                break;
        }
    }

    public void BuyUpgrade(bool spendPoints)
    {
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

            default:
                break;
        }

        if (spendPoints)
        {
            GameStats.points -= price;

            if (persistentCanvas != null && price > 0)
            {
                // Show spent points on the top left corner of the screen.
                persistentCanvas.CreateNumberChangeEffect(HUD.hudTopLeftCorner, "-" + price.ToString(), new Color(1f, 0.5f, 0.5f), -0.5f, 1);
            }
        }

        // Increase price.
        if (price < 999999)
        {
            price = Mathf.RoundToInt(price * priceMultiplier);
        }

        // Price limit (6 digits).
        if (price > 999999)
        {
            price = 999999;
        }

        // Amount of purchases.
        if (GameStats.upgradePurchaseAmount[(int)upgradeType] < uint.MaxValue)
        {
            GameStats.upgradePurchaseAmount[(int)upgradeType] += 1;
        }

        // Save stats.
        GameStats.upgradePrice[(int)upgradeType] = price;
        GameStats.SaveStats();

        if (Debug.isDebugBuild)
        {
            Debug.Log("Purchased upgrade: ID " + (int)upgradeType + ", Amount " + GameStats.upgradePurchaseAmount[(int)upgradeType] + ", Price " + GameStats.upgradePrice[(int)upgradeType]);
        }
    }
}
