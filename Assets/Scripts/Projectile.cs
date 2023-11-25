using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public int damage = 1;
    public int perforation = 1;
    public GameObject impactEffect;
    public GameObject impactSound;

    private SpriteRenderer _spriteRenderer;
    private ParticleSystem _trail;
    private ParticleSystem.MainModule _trailMainModule;

    [HideInInspector] public GameObject shooter;
    [HideInInspector] public string ignoreTag = "EditorOnly";

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _trail = GetComponentInChildren<ParticleSystem>();
        _trailMainModule = _trail.main;
        _trailMainModule.startColor = _spriteRenderer.color;

        // Disable projectile trails.
        if (Options.projectileTrails == 0)
        {
            Destroy(_trail.gameObject);
        }

        // Fail-safe.
        if (perforation <= 0)
        {
            perforation = 1;
        }
    }

    void Update()
    {
        transform.Translate(transform.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != shooter && !collision.gameObject.CompareTag(ignoreTag) && perforation > 0)
        {
            Health hitHealth = collision.gameObject.GetComponent<Health>();

            if (hitHealth != null)
            {
                if (hitHealth.invincible == false)
                {
                    hitHealth.TakeDamage(damage);

                    if (impactEffect != null && Options.projectileImpacts == 1)
                    {
                        var effect = Instantiate(impactEffect, transform.position, transform.rotation);
                        ParticleSystem.MainModule effectMainModule = effect.GetComponent<ParticleSystem>().main;
                        effectMainModule.startColor = _spriteRenderer.color;
                    }
                    if (impactSound != null)
                    {
                        Instantiate(impactSound, transform.position, transform.rotation);
                    }

                    perforation--;

                    if (perforation <= 0)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
