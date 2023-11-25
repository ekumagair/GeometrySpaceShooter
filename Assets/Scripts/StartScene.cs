using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartScene : MonoBehaviour
{
    public GameObject startButton;
    public GameObject[] canvases;
    public GameObject startObjects;
    public GameObject[] upgradePages;
    public int currentUpgradePage = 0;
    public Button upgradePreviousButton;
    public Button upgradeNextButton;
    public TMP_Text upgradePageText;
    public TMP_Text versionText;

    public static bool goToUpgrades = false;
    public static int currentCanvas = 0;

    void Awake()
    {
        Time.timeScale = 1.0f;

        // If has saved data, load the data and the prices for each upgrade.
        if (SaveSystem.SaveExists())
        {
            GameStats.LoadStats();

            Upgrade[] upgrades = FindObjectsOfType<Upgrade>();
            foreach (var upgrade in upgrades)
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
    }

    void Start()
    {
        GameStats.currentLevelPoints = 0;
        GameStats.multipliedCurrentScore = false;
        ScoreChain.scoreMultiplier = 1.0f;
        ScoreChain.tier = 0;
        ScoreChain.currentKills = 0;

        if (GameStats.points < 0)
        {
            GameStats.points = 0;
        }

        if (goToUpgrades)
        {
            GoToCanvas(1);
            PersistentCanvas.reference.CreateButtonSound(0);
            goToUpgrades = false;
        }
        else
        {
            GoToCanvas(0);
        }

        if (versionText != null)
        {
            versionText.text = "v" + Application.version.ToString();
        }

        if (GameStats.initializedGame == false)
        {
            PersistentCanvas.reference.CreateFadeOutOverlay();

            if (Application.genuineCheckAvailable == true)
            {
                if (Application.genuine == false && startButton != null)
                {
                    Destroy(startButton);
                }
            }

            GameStats.initializedGame = true;
        }
    }

    void Update()
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

        if (currentUpgradePage == 0)
        {
            upgradePreviousButton.interactable = false;
        }
        else
        {
            upgradePreviousButton.interactable = true;
        }

        if (currentUpgradePage == upgradePages.Length - 1)
        {
            upgradeNextButton.interactable = false;
        }
        else
        {
            upgradeNextButton.interactable = true;
        }

        upgradePageText.text = (currentUpgradePage + 1).ToString() + "/" + upgradePages.Length.ToString();

        for (int i = 0; i < canvases.Length; i++)
        {
            if (canvases[i].activeSelf == true && currentCanvas != i)
            {
                ChooseOneCanvas(currentCanvas);
            }
        }

        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                SaveSystem.DeleteSave();
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
        }
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

    public void ChangeUpgradePage(int increment)
    {
        currentUpgradePage += increment;
    }
}
