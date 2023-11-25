using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomGlow : MonoBehaviour
{
    private bool _touchingEnemy = false;
    private float _alpha = 0;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _alpha);

        if(_touchingEnemy && _alpha < 1)
        {
            _alpha += Time.deltaTime;
        }
        if (!_touchingEnemy && _alpha > 0)
        {
            _alpha -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            _touchingEnemy = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _touchingEnemy = false;
        }
    }
}
