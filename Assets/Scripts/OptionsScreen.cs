using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsScreen : MonoBehaviour
{
    public Slider sliderSFX;
    public TMP_Text textSFX;
    public Slider sliderMusic;
    public TMP_Text textMusic;

    void Start()
    {
        if(SaveSystem.SaveExists())
        {
            sliderSFX.value = Options.soundVolume * 10;
            sliderMusic.value = Options.musicVolume * 10;
        }
        else
        {
            sliderSFX.value = 10;
            sliderMusic.value = 5;
        }
    }

    void Update()
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
        if(Options.projectileTrails == 0)
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
}
