using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSetVolume : MonoBehaviour
{
    // Changes the volume of this object's AudioSource based on the settings.
    AudioSource _as;
    float baseVolume;

    public enum AudioType
    {
        SoundEffect,
        Music
    }
    [Tooltip("Whether this AudioSource represents a sound effect or the scene's music.")]
    public AudioType type = 0;

    public enum VolumeModifier
    {
        SetVolume,
        MultiplyVolume
    }
    [Tooltip("Whether this script will override the AudioSource's volume or multiply it.")]
    public VolumeModifier volumeModifier = 0;

    public bool constantlyUpdateVolume = true;

    void Start()
    {
        _as = GetComponent<AudioSource>();
        baseVolume = _as.volume;
        SetVolume();
    }

    void SetVolume()
    {
        float mult = 1.0f;

        if (volumeModifier == VolumeModifier.SetVolume)
        {
            mult = 1.0f;
        }
        else if (volumeModifier == VolumeModifier.MultiplyVolume)
        {
            mult = baseVolume;
        }

        switch (type)
        {
            case AudioType.SoundEffect:
                _as.volume = mult * Options.soundVolume;
                break;
            case AudioType.Music:
                _as.volume = mult * Options.musicVolume;
                break;
            default:
                break;
        }
    }
}