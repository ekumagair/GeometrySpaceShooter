using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public static Projectile CreateShot(GameObject prefab, Transform transformReference, float angle, GameObject shooter, Color color, float speedMultiplier, int overrideDamage)
    {
        var p = Instantiate(prefab, transformReference.position, transformReference.rotation);
        Projectile script = p.GetComponent<Projectile>();
        SpriteRenderer spriteRenderer = p.GetComponent<SpriteRenderer>();

        if (script != null)
        {
            script.shooter = shooter;
            script.speed *= speedMultiplier;

            if (shooter.tag != "")
            {
                script.ignoreTag = shooter.tag;
            }
            if (overrideDamage > 0)
            {
                script.damage = overrideDamage;
            }
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }

        p.transform.rotation = Quaternion.Euler(p.transform.rotation.x, p.transform.rotation.y, angle);

        return script;
    }

    public static Projectile CreateShot(GameObject prefab, Transform transformReference, float angle, GameObject shooter, Color color, float speedMultiplier, int overrideDamage, int overridePerforation)
    {
        Projectile script = CreateShot(prefab, transformReference, angle, shooter, color, speedMultiplier, overrideDamage);
        script.perforation = overridePerforation;
        return script;
    }

    public static float AimDirection(Transform target, Transform origin)
    {
        float offset = 90f;

        Vector3 direction = (target.position - origin.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return (angle + offset) / 2;
    }

    public static void DestroyAllProjectiles()
    {
        Projectile[] projectiles = GameObject.FindObjectsOfType<Projectile>();

        if (projectiles.Length > 0)
        {
            foreach (Projectile p in projectiles)
            {
                Destroy(p.gameObject);
            }
        }
    }
}
