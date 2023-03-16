using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartScene : MonoBehaviour
{
    public GameObject[] upgradePages;
    public int currentUpgradePage = 0;
    public Button upgradePreviousButton;
    public Button upgradeNextButton;
    public TMP_Text upgradePageText;

    public static bool goToUpgrades = false;
    PersistentCanvas persistentCanvas;
    Camera mainCamera;

    void Awake()
    {
        // If has saved data, load the data and the prices for each upgrade.
        if (PlayerPrefs.HasKey("player_start_hp"))
        {
            Upgrade[] upgrades = FindObjectsOfType<Upgrade>();
            foreach (var upgrade in upgrades)
            {
                if (PlayerPrefs.HasKey("upgrade_" + upgrade.upgradeType.ToString() + "_price"))
                {
                    upgrade.price = PlayerPrefs.GetInt("upgrade_" + upgrade.upgradeType.ToString() + "_price");
                }
            }

            GameStats.LoadStats();
        }
    }

    void Start()
    {
        persistentCanvas = GameObject.Find("PersistentCanvas").GetComponent<PersistentCanvas>();
        mainCamera = Camera.main;
        GameStats.currentLevelPoints = 0;
        GameStats.multipliedCurrentScore = false;

        if (GameStats.points < 0)
        {
            GameStats.points = 0;
        }

        if(goToUpgrades)
        {
            GoToUpgrades();
            persistentCanvas.CreateButtonSound();
            goToUpgrades = false;
        }

        persistentCanvas.SetLevelText();
    }

    void Update()
    {
        for (int i = 0; i < upgradePages.Length; i++)
        {
            if(i == currentUpgradePage)
            {
                upgradePages[i].gameObject.SetActive(true);
            }
            else
            {
                upgradePages[i].gameObject.SetActive(false);
            }
        }

        if(currentUpgradePage == 0)
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

        if(Debug.isDebugBuild)
        {
            if(Input.GetKeyDown(KeyCode.Delete))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoToStart()
    {
        mainCamera.transform.position = new Vector3(0, 0, mainCamera.transform.position.z);
    }

    public void GoToUpgrades()
    {
        mainCamera.transform.position = new Vector3(10, 0, mainCamera.transform.position.z);
    }

    public void GoToCredits()
    {
        mainCamera.transform.position = new Vector3(20, 0, mainCamera.transform.position.z);
    }

    public void ChangeUpgradePage(int increment)
    {
        currentUpgradePage += increment;
    }
}
