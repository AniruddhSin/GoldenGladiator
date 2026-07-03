using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Health health;
    private float fadeDuration = 1f;
    private Damageable damageable;
    private BoxCollider2D collide;
    private Rigidbody2D rb;
    private bool phase2Flag = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
        damageable = GetComponent<Damageable>();
        collide = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();


        health.OnDamaged += OnDamage;
        health.OnDie += OnDie;
    }

    public void FadeBoss (bool fadeOut)
    {
        StartCoroutine(Fade(fadeOut));
    }

    private IEnumerator Fade (bool fadeOut)
    {
        damageable.isInvincible = fadeOut;
        collide.enabled = !fadeOut;
        rb.simulated = !fadeOut;
        Color original = spriteRenderer.color;
        float startAlpha = fadeOut ? 1f : 0f;
        float endAlpha = fadeOut ? 0f : 1f;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float alpha = Mathf.Lerp(startAlpha, endAlpha, t / fadeDuration);
            spriteRenderer.color = new Color(original.r, original.g, original.b, alpha);

            yield return null;
        }

        spriteRenderer.color = new Color(original.r, original.g, original.b, endAlpha);

    }

    void OnDamage(float damage, GameObject source)
    {
        StartCoroutine(BlinkRed());
        if (health.currentHealth <= health.maxHealth * 0.35)
        {
            GameManager.Instance.NextPhase();
        }else if (!phase2Flag && health.currentHealth <= health.maxHealth * 0.65)
        {
            GameManager.Instance.NextPhase();
            phase2Flag = true;
        }
    }
    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = Color.white;
    }

    void OnDie()
    {
        Debug.Log("Boss Died");
        health.ResetHealth();
    }
}
