using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Analytics;

public class StartScene : MonoBehaviour
{
    [Header("General")]
    public GameObject[] canvases;

    [Space]

    [Header("Start Section")]
    public GameObject startButton;
    public GameObject removeAdsButton;
    public GameObject startObjects;
    public TMP_Text versionText;
    public LocalizeStringEvent levelTextLocalize;

    [Space]

    [Header("Upgrades Section")]
    public GameObject[] upgradePages;
    public int currentUpgradePage = 0;
    public Button upgradePreviousButton;
    public Button upgradeNextButton;
    public TMP_Text upgradePageText;

    [Space]

    [Header("Options Section")]
    public OptionsScreen optionsScreenScript;

    [Space]

    [Header("Rewards Section")]
    public TMP_Text killedEnemiesText;
    public Image killedEnemiesBarBg;
    public Image killedEnemiesBarFill;

    [HideInInspector] public int currentCanvas = 0;

    public enum StartOverride
    {
        Default,
        GoToUpgrades,
        GoToRemoveAds
    }
    public static StartOverride startOverride = StartOverride.Default;

    public static StartScene reference;

    private Upgrade[] _upgrades = new Upgrade[GameConstants.UPGRADE_AMOUNT];

    void Awake()
    {
        Time.timeScale = 1.0f;
        reference = this;
        Analytics.initializeOnStartup = false;
        Analytics.enabled = false;
        PerformanceReporting.enabled = false;
        Analytics.limitUserTracking = true;
        Analytics.deviceStatsEnabled = false;

        // Awake coroutine.
        if (GameStats.initializedGame == false)
        {
            StartCoroutine(AwakeCoroutine());
        }

        GameStats.loadStatsFinished = false;
        GameStats.LoadStats();

        // If has saved data, load the data and the prices for each upgrade.
        if (SaveSystem.SaveExists())
        {
            _upgrades = FindObjectsOfType<Upgrade>();
            foreach (var upgrade in _upgrades)
            {
                int gameStatsPrice = GameStats.upgradePrice[(int)upgrade.upgradeType];

                if (gameStatsPrice > 0)
                {
                    upgrade.price = gameStatsPrice;
                    upgrade.amountPurchased = GameStats.upgradePurchaseAmount[(int)upgrade.upgradeType];
                }

                upgrade.DisplayInfo();
            }
        }

        // Rewards section.
        if (GameStats.enemiesKilledTotal < 0)
        {
            GameStats.enemiesKilledTotal = 0;
        }

        killedEnemiesText.text = GameStats.enemiesKilledTotal.ToString();

        if (GameStats.enemiesKilledTotal < killedEnemiesBarBg.rectTransform.sizeDelta.y)
        {
            killedEnemiesBarFill.fillAmount = GameStats.enemiesKilledTotal / killedEnemiesBarBg.rectTransform.sizeDelta.y;
        }
        else
        {
            killedEnemiesBarFill.fillAmount = 1f;
        }
    }

    void Start()
    {
        GameStats.currentLevelPoints = 0;
        GameStats.multipliedCurrentScore = false;
        ScoreChain.tier = 0;
        ScoreChain.currentKills = 0;

        if (PopUp.instance != null)
        {
            PopUp.instance.ResetActions();
        }
        if (GameStats.points < 0)
        {
            GameStats.points = 0;
        }
        if (ScoreChain.scoreMultiplierAtStart < GameConstants.SCORE_MULTIPLIER_FLOOR)
        {
            ScoreChain.scoreMultiplierAtStart = GameConstants.SCORE_MULTIPLIER_FLOOR;
        }

        ScoreChain.scoreMultiplier = ScoreChain.scoreMultiplierAtStart;

        // Texts.
        UpdateTexts();

        // Start screen mode.
        switch (startOverride)
        {
            case StartOverride.Default:
                GoToCanvas(0);
                break;

            case StartOverride.GoToUpgrades:
                GoToCanvas(1);
                PersistentCanvas.reference.CreateButtonSound(0);
                break;

            case StartOverride.GoToRemoveAds:
                GoToRemoveAds();
                PersistentCanvas.reference.CreateButtonSound(0);
                break;

            default:
                GoToCanvas(0);
                break;
        }

        if (startOverride != StartOverride.Default)
        {
            startOverride = StartOverride.Default;
        }

        // Initialize.
        if (GameStats.initializedGame == false)
        {
            PersistentCanvas.reference.CreateFadeOutOverlay();

            if (Application.genuineCheckAvailable == true)
            {
                if (Application.genuine == false)
                {
                    GameStats.failedGenuine = true;

                    if (startButton != null)
                    {
                        Destroy(startButton);
                    }
                }
                else
                {
                    GameStats.failedGenuine = false;
                }
            }

            // Debug messages.
            StartCoroutine(DebugMessages());

            // Set as initialized.
            GameStats.initializedGame = true;
        }
    }

    void Update()
    {
        // Show/hide upgrade pages.
        if (upgradePages.Length > 0)
        {
            for (int i = 0; i < upgradePages.Length; i++)
            {
                if (i == currentUpgradePage)
                {
                    upgradePages[i].gameObject.SetActive(true);
                }
                else
                {
                    upgradePages[i].gameObject.SetActive(false);
                }
            }
        }

        // Enable/disable upgrade page buttons.
        if (upgradePreviousButton != null)
        {
            if (currentUpgradePage == 0)
            {
                upgradePreviousButton.interactable = false;
            }
            else
            {
                upgradePreviousButton.interactable = true;
            }
        }

        if (upgradeNextButton != null)
        {
            if (currentUpgradePage == upgradePages.Length - 1)
            {
                upgradeNextButton.interactable = false;
            }
            else
            {
                upgradeNextButton.interactable = true;
            }
        }

        // Set upgrade page text.
        if (upgradePageText != null)
        {
            if (upgradePages.Length > 0)
            {
                upgradePageText.text = (currentUpgradePage + 1).ToString() + "/" + upgradePages.Length.ToString();
            }
            else
            {
                upgradePageText.text = "";
            }
        }

        // Show/hide remove ads button.
        removeAdsButton.SetActive(!PurchaseManager.instance.HasRemovedAds());

#if UNITY_EDITOR || UNITY_STANDALONE
        // Debug inputs.
        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                SaveSystem.DeleteSave();
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameStats.claimedRewards = new int[GameConstants.REWARDS_AMOUNT];
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                LevelGenerator.campaignDifficulty--;
                GameStats.level--;
                levelTextLocalize.RefreshString();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                LevelGenerator.campaignDifficulty++;
                GameStats.level++;
                levelTextLocalize.RefreshString();
            }
        }
#endif
    }

    public void PlayGame()
    {
        GameStats.currentLevelType = GameStats.LevelType.MAIN;
        SceneManager.LoadScene("GameScene");
    }

    public void PlayPresetLevel(int level)
    {
        GameStats.currentLevelType = GameStats.LevelType.PRESET;
        PresetLevels.currentPresetLevel = level;
        LoadingScreen.CallLoadScreen(LoadingScreen.Scenes.Game);
    }

    public void GoToCanvas(int c)
    {
        ChooseOneCanvas(c);
        UpdateTexts();
    }

    public void GoToRemoveAds()
    {
        GoToCanvas(3);
        optionsScreenScript.scrollContent.anchoredPosition = new Vector2(0, 200);
    }

    void ChooseOneCanvas(int c)
    {
        for (int i = 0; i < canvases.Length; i++)
        {
            if (i == c)
            {
                canvases[i].gameObject.SetActive(true);
            }
            else
            {
                canvases[i].gameObject.SetActive(false);
            }
        }

        if (startObjects != null)
        {
            if (c == 0)
            {
                startObjects.SetActive(true);
            }
            else
            {
                startObjects.SetActive(false);
            }
        }

        currentCanvas = c;
    }

    public void UpdateTexts()
    {
        if (versionText != null)
        {
            versionText.text = "v" + Application.version.ToString();
        }
        if (levelTextLocalize != null)
        {
            levelTextLocalize.RefreshString();
        }
    }

    public void ChangeUpgradePage(int increment)
    {
        currentUpgradePage += increment;
    }

    public Upgrade[] GetAllUpgrades()
    {
        return _upgrades;
    }

    public void PopUpAdButtons()
    {
        PopUp.instance.SetButtonsTexts(new LocalizedString("PopUp", "button_ok"), null, null, null);
        PopUp.instance.OpenPopUp(new LocalizedString("PopUp", "disabled_rewards_title"), new LocalizedString("PopUp", "disabled_rewards_desc"), 1);
    }

    public void PopUpFakeStore()
    {
        PopUp.instance.SetButtonsTexts(new LocalizedString("PopUp", "button_ok"), null, null, null);
        PopUp.instance.OpenPopUp(new LocalizedString("PopUp", "fake_store_title"), new LocalizedString("PopUp", "fake_store_desc"), 1);
    }

    public void PopUpDeleteSave()
    {
        PopUp.instance.SetButtonsTexts(new LocalizedString("PopUp", "button_accept"), new LocalizedString("PopUp", "button_cancel"), null, null);
        PopUp.instance.SetActions(DeleteSaveAccept, PopUp.instance.ClosePopUp, null, null);
        PopUp.instance.OpenPopUp(new LocalizedString("PopUp", "delete_save_title"), new LocalizedString("PopUp", "delete_save_desc"), 2);
    }

    public void DeleteSaveAccept()
    {
        SaveSystem.DeleteSave();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        PopUp.instance.ClosePopUp();
    }

    private IEnumerator DebugMessages()
    {
        yield return null;

        if (GameStats.enableAdButttons == false)
        {
            if (PurchaseManager.instance.UsingFakeStore() == true)
            {
                PopUp.instance.SetActions(PopUpFakeStore, null, null, null);
            }
            else
            {
                PopUp.instance.ResetActions();
            }

            PopUpAdButtons();
        }
        else if (PurchaseManager.instance.UsingFakeStore() == true)
        {
            PopUp.instance.ResetActions();
            PopUpFakeStore();
        }
    }

    private IEnumerator AwakeCoroutine()
    {
        yield return new WaitForSeconds(0.01f);

        // Canvas fail-safe. Prevent all of them from showing at the same time.
        if (GameStats.loadStatsFinished == true)
        {
            GoToCanvas(0);
            if (Debug.isDebugBuild)
            {
                Debug.Log("Stat loading successful. Going to canvas 0.");
            }
        }

        yield return new WaitForSeconds(0.05f);

        if (GameStats.loadStatsFinished == false)
        {
            GoToCanvas(0);
            Debug.LogError("Stat loading failed. Going to canvas 0. " + "(v" + Application.version.ToString() + ")");
        }

        UpdateTexts();
    }
}
