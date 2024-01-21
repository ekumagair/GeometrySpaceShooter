using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{
    public AudioClip[] clips;
    public float pitchMultMin = 1.0f;
    public float pitchMultMax = 1.0f;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        _audioSource.clip = clips[Random.Range(0, clips.Length)];
        _audioSource.pitch *= Random.Range(pitchMultMin, pitchMultMax);
        _audioSource.Play();

        if (_audioSource.loop == false)
        {
            StartCoroutine(DestroyAfterAudio());
        }
    }

    IEnumerator DestroyAfterAudio()
    {
        yield return new WaitForSeconds(_audioSource.clip.length * 1.1f);
        Destroy(gameObject);
    }
}
