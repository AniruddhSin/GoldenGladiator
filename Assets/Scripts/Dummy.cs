using UnityEngine;

public class Dummy : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Health health;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();

        health.OnDie += OnDie;
        health.OnDamaged += OnDamage;
    }

    void OnDamage(float damage, GameObject source)
    {
        Debug.Log("Dummy took" + damage + " damage!");
    }

    void OnDie()
    {
        health.ResetHealth();
        spriteRenderer.color = Random.ColorHSV();
    }
}
