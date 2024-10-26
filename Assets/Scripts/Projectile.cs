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
    public bool limitEffectsScale = true;

    [Header("Impact")]
    public GameObject impactEffect;
    public GameObject impactSound;

    [Header("Trail Particles")]
    public ParticleSystem trail;

    [Header("Extras")]
    public SpriteRenderer[] extraOutline;
    public SpriteRenderer[] extraSprites;
    public ParticleSystem extraTrail;
    public GameObject extraImpactEffect;
    public ParticleSystem extraPerforationParticle;

    private SpriteRenderer _spriteRenderer;
    private ParticleSystem.MainModule _trailMainModule;
    private ParticleSystem.MainModule _extraTrailModule;
    private ParticleSystem.MainModule _extraPerforationModule;
    private float _extrasScale = 1.0f;
    private float _extrasTransparency = 1.0f;

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

        // Set effects based on scale.
        if (limitEffectsScale == true)
        {
            _extrasScale = Mathf.Clamp(gameObject.transform.localScale.x, 1.0f, 1.4f);
        }
        else
        {
            _extrasScale = gameObject.transform.localScale.x;
        }

        if (alwaysEnableEffects == false)
        {
            _extrasTransparency = Mathf.Clamp(gameObject.transform.localScale.x, 1.0f, 2.0f);
        }
        else
        {
            _extrasTransparency = 1.0f;
        }

        ShowExtraOutline(0, 2);
        ShowExtraOutline(1, 14);

        if (trail != null)
        {
            trail.gameObject.transform.localScale = new Vector3(_extrasScale, _extrasScale, _extrasScale);
        }

        if (extraTrail != null)
        {
            extraTrail.gameObject.SetActive(damage >= 6 || alwaysEnableEffects);
            _extraTrailModule = extraTrail.main;
            _extraTrailModule.startColor = ExtraEffectsColor(0.8f);
            extraTrail.gameObject.transform.localScale = new Vector3(_extrasScale, _extrasScale, _extrasScale);
        }

        if (extraPerforationParticle != null)
        {
            extraPerforationParticle.gameObject.SetActive(perforation > 1 || alwaysEnableEffects);
            _extraPerforationModule = extraPerforationParticle.main;
            _extraPerforationModule.startColor = ExtraEffectsColor(0.9f);
            extraPerforationParticle.gameObject.transform.localScale = new Vector3(_extrasScale, _extrasScale, _extrasScale);
        }

        for (int i = 0; i < extraSprites.Length; i++)
        {
            extraSprites[i].color = _spriteRenderer.color;
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
                        effectMainModule.startColor = ExtraEffectsColor(1.0f);
                    }
                    if (extraImpactEffect != null && Options.projectileImpacts == 1 && (damage >= 10 || alwaysEnableEffects))
                    {
                        var impact = Instantiate(extraImpactEffect, transform.position, transform.rotation);
                        SpriteRenderer[] sr = impact.GetComponentsInChildren<SpriteRenderer>();
                        foreach (SpriteRenderer item in sr)
                        {
                            item.color = ExtraEffectsColor(1.0f);
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
            extraTrail.gameObject.SetActive(true);
            extraTrail = null;
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
            extraOutline[i].color = ExtraEffectsColor(1.0f);
        }
    }

    private Color ExtraEffectsColor(float transparency)
    {
        return new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, (_spriteRenderer.color.a * transparency) / _extrasTransparency);
    }
}
