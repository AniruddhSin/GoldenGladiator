using UnityEngine;

public class Damageable : MonoBehaviour
{
    public Health health {get; private set;}
    public bool isInvincible = false;

    void Awake()
    {
        health = GetComponent<Health>();
        if (!health)
        {
            Debug.LogError("No health component found on damageable target!");
        }
    }

    public void InflictDamage(float damage, GameObject source)
    {
        if (health && !isInvincible)
        {
            health.TakeDamage(damage, source);
        }
    }
}
