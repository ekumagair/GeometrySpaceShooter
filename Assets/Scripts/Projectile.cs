using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Properties")]
    public float speed;
    public int damage = 1;
    public int perforation = 1;

    [Header("Impact")]
    public GameObject impactEffect;
    public GameObject impactSound;

    [Header("Trail Particles")]
    public ParticleSystem trail;

    [Header("Extras")]
    public SpriteRenderer[] extraOutline;
    public ParticleSystem extraTrail;

    private SpriteRenderer _spriteRenderer;
    private ParticleSystem.MainModule _trailMainModule;
    private ParticleSystem.MainModule _extraTrailModule;

    [HideInInspector] public GameObject shooter;
    [HideInInspector] public string ignoreTag = "EditorOnly";

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _trailMainModule = trail.main;
        _trailMainModule.startColor = _spriteRenderer.color;

        // Disable projectile trails.
        if (Options.projectileTrails == 0)
        {
            Destroy(trail.gameObject);
            Destroy(extraTrail.gameObject);
        }

        // Fail-safe.
        if (perforation <= 0)
        {
            perforation = 1;
        }

        // Extras.
        ShowExtraOutline(0, 2);
        ShowExtraOutline(1, 12);

        if (extraTrail != null)
        {
            extraTrail.gameObject.SetActive(damage >= 6);
            _extraTrailModule = extraTrail.main;
            _extraTrailModule.startColor = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _spriteRenderer.color.a * 0.8f);
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
                        DetachTrails();
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    public void DetachTrails()
    {
        if (trail != null)
        {
            trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            _trailMainModule.loop = false;
            trail.gameObject.transform.parent = null;
        }
        if (extraTrail != null)
        {
            extraTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            _extraTrailModule.loop = false;
            extraTrail.gameObject.transform.parent = null;
        }
    }

    public void ShowExtraOutline(int i, int damageMin)
    {
        if (extraOutline == null || extraOutline.Length < 1 || i >= extraOutline.Length)
        {
            return;
        }

        if (extraOutline[i] != null)
        {
            extraOutline[i].gameObject.SetActive(damage >= damageMin);
            extraOutline[i].color = _spriteRenderer.color;
        }
    }
}
