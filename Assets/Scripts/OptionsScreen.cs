using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;

public class OptionsScreen : MonoBehaviour
{
    public RectTransform scrollContent;

    [Header("SFX")]
    public Slider sliderSFX;
    public TMP_Text textSFX;

    [Header("Music")]
    public Slider sliderMusic;
    public TMP_Text textMusic;

    [Header("Remove Ads")]
    public PopUp removeAdsPopUp;

    [Header("Testing")]
    public GameObject deleteButton;
    public GameObject debugButton;
    public bool showDeleteButton;
    public bool showDebugButton;

    void Start()
    {
        if (SaveSystem.SaveExists())
        {
            sliderSFX.value = Options.soundVolume * 10;
            sliderMusic.value = Options.musicVolume * 10;
        }
        else
        {
            sliderSFX.value = 10;
            sliderMusic.value = 5;
        }

        SetText();
        SetTestButtons();
    }

    void Update()
    {
        SetText();
        SetTestButtons();
    }

    private void OnEnable()
    {
        scrollContent.anchoredPosition = new Vector2(0, 0);
    }

    private void SetText()
    {
        textSFX.text = "(" + Options.soundVolume * 100 + "%)";
        textMusic.text = "(" + Options.musicVolume * 100 + "%)";
    }

    public void SetSFXVolume()
    {
        Options.soundVolume = sliderSFX.value / 10;
        GameStats.SaveStats();
    }

    public void SetMusicVolume()
    {
        Options.musicVolume = sliderMusic.value / 10;
        GameStats.SaveStats();
    }

    public void SetProjectileTrails()
    {
        if (Options.projectileTrails == 0)
        {
            Options.projectileTrails = 1;
        }
        else
        {
            Options.projectileTrails = 0;
        }
        GameStats.SaveStats();
    }

    public void SetProjectileImpacts()
    {
        if (Options.projectileImpacts == 0)
        {
            Options.projectileImpacts = 1;
        }
        else
        {
            Options.projectileImpacts = 0;
        }
        GameStats.SaveStats();
    }

    public void RemoveAdsPopUp()
    {
#if !DISABLE_IAP
        if (PurchaseManager.fetchedProducts == true && PurchaseManager.productCollection != null)
        {
            removeAdsPopUp.SetButtonsTexts(new LocalizedString("PopUp", "button_cancel"), null, null, null);
            removeAdsPopUp.OpenPopUp(new LocalizedString("PopUp", "remove_ads_title"), new LocalizedString("PopUp", "remove_ads_desc"), 1);
        }
        else
        {
            PopUpOnlineError();
        }
#endif
    }

    public void PopUpOnlineError()
    {
        PopUp.instance.SetButtonsTexts(new LocalizedString("PopUp", "button_ok"), null, null, null);
        PopUp.instance.OpenPopUp(new LocalizedString("PopUp", "online_error_title"), new LocalizedString("PopUp", "online_error_desc"), 1);
    }

    private void SetTestButtons()
    {
        deleteButton.gameObject.SetActive(showDeleteButton);
        debugButton.gameObject.SetActive(showDebugButton);
    }
}
