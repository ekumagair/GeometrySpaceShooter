using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public int damage = 1;
    public GameObject impactEffect;
    public GameObject impactSound;

    SpriteRenderer spriteRenderer;
    ParticleSystem trail;
    ParticleSystem.MainModule trailMainModule;

    [HideInInspector]
    public GameObject shooter;
    [HideInInspector]
    public string ignoreTag = "EditorOnly";

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trail = GetComponentInChildren<ParticleSystem>();
        trailMainModule = trail.main;
        trailMainModule.startColor = spriteRenderer.color;
    }

    void Update()
    {
        transform.Translate(transform.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != shooter && !collision.gameObject.CompareTag(ignoreTag))
        {
            Health hitHealth = collision.gameObject.GetComponent<Health>();

            if (hitHealth != null)
            {
                hitHealth.TakeDamage(damage);

                if (impactEffect != null)
                {
                    var effect = Instantiate(impactEffect, transform.position, transform.rotation);
                    ParticleSystem.MainModule effectMainModule = effect.GetComponent<ParticleSystem>().main;
                    effectMainModule.startColor = spriteRenderer.color;
                }
                if(impactSound != null)
                {
                    Instantiate(impactSound, transform.position, transform.rotation);
                }

                Destroy(gameObject);
            }
        }
    }
}
