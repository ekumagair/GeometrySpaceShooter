using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Properties")]
    public float speed;
    public int damage = 1;
    public int perforation = 1;
    public bool alwaysEnableEffects = false;

    [Header("Impact")]
    public GameObject impactEffect;
    public GameObject impactSound;

    [Header("Trail Particles")]
    public ParticleSystem trail;

    [Header("Extras")]
    public SpriteRenderer[] extraOutline;
    public ParticleSystem extraTrail;
    public GameObject extraImpactEffect;
    public ParticleSystem extraPerforationParticle;

    private SpriteRenderer _spriteRenderer;
    private ParticleSystem.MainModule _trailMainModule;
    private ParticleSystem.MainModule _extraTrailModule;
    private ParticleSystem.MainModule _extraPerforationModule;

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
            if (trail != null)
                Destroy(trail.gameObject);

            if (extraTrail != null)
                Destroy(extraTrail.gameObject);

            if (extraPerforationParticle != null)
                Destroy(extraPerforationParticle.gameObject);
        }

        // Fail-safe.
        if (perforation <= 0)
        {
            perforation = 1;
        }

        // Extra visual effects.

        // Outline 0: Damage >= 2.
        // Extra trail: Damage >= 6.
        // Extra impact: Damage >= 10.
        // Outline 1: Damage >= 14.

        // Extra perforation particle: Perforation > 1.

        ShowExtraOutline(0, 2);
        ShowExtraOutline(1, 14);

        if (extraTrail != null)
        {
            extraTrail.gameObject.SetActive(damage >= 6 || alwaysEnableEffects);
            _extraTrailModule = extraTrail.main;
            _extraTrailModule.startColor = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _spriteRenderer.color.a * 0.8f);
        }

        if (extraPerforationParticle != null)
        {
            extraPerforationParticle.gameObject.SetActive(perforation > 1 || alwaysEnableEffects);
            _extraPerforationModule = extraPerforationParticle.main;
            _extraPerforationModule.startColor = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _spriteRenderer.color.a * 0.9f);
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
                    if (extraImpactEffect != null && Options.projectileImpacts == 1 && (damage >= 10 || alwaysEnableEffects))
                    {
                        var impact = Instantiate(extraImpactEffect, transform.position, transform.rotation);
                        SpriteRenderer[] sr = impact.GetComponentsInChildren<SpriteRenderer>();
                        foreach (SpriteRenderer item in sr)
                        {
                            item.color = _spriteRenderer.color;
                        }
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
            extraOutline[i].gameObject.SetActive(damage >= damageMin || alwaysEnableEffects);
            extraOutline[i].color = _spriteRenderer.color;
        }
    }
}
