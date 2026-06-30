using UnityEngine;

public class Damageable : MonoBehaviour
{
    public Health health {get; private set;}

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
        if (health)
        {
            health.TakeDamage(damage, source);
        }
    }
}
