using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    }

    void Update()
    {
        priceDisplay.text = price.ToString();

        if(GameStats.points < price)
        {
            // If you don't have enough points, disable buy button and enable ad button.
            buyButton.interactable = false;

            if (adButton != null && Debug.isDebugBuild == false)
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
                persistentCanvas.CreateNumberChangeEffect(new Vector3(-110, 250, 0), "-" + price.ToString(), new Color(1f, 0.5f, 0.5f), -0.25f);
            }
        }
        
        price = Mathf.RoundToInt(price * priceMultiplier);

        // Price limit (6 digits).
        if(price > 999999)
        {
            price = 999999;
        }

        // Save stats.
        PlayerPrefs.SetInt("upgrade_" + upgradeType.ToString() + "_price", price);
        GameStats.SaveStats();
        Debug.Log("Saved Int: " + "upgrade_" + upgradeType.ToString() + "_price");
        Debug.Log("Saved player health is " + PlayerPrefs.GetInt("player_start_hp") + " / " + Player.startHealth);
    }
}
