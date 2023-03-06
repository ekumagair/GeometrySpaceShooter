using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] clips;
    public float pitchMultMin = 1.0f;
    public float pitchMultMax = 1.0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.pitch *= Random.Range(pitchMultMin, pitchMultMax);
        audioSource.Play();

        StartCoroutine(DestroyAfterAudio());
    }

    IEnumerator DestroyAfterAudio()
    {
        yield return new WaitForSeconds(audioSource.clip.length * 1.1f);
        Destroy(gameObject);
    }
}
