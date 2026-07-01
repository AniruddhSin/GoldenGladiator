using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [field: SerializeField]
    public float maxHealth {get; private set;} = 10f;
    [field: SerializeField]
    public float currentHealth {get; private set;}

    public UnityAction<float, GameObject> OnDamaged;
    public UnityAction<float> OnHealed;
    public UnityAction OnDie;

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
    public void Heal(float health)
    {
        float healthBefore = currentHealth;
        currentHealth += health;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        float trueHeal = currentHealth - healthBefore;
        if (trueHeal > 0f)
        {
            OnHealed?.Invoke(trueHeal);
        }
    }
    public void TakeDamage(float damage, GameObject source)
    {
        float healthBefore = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        float trueDamage = healthBefore - currentHealth;
        if (damage > 0f)
        {
            OnDamaged?.Invoke(trueDamage, source);
        }
        HandleDeath();
    }

    private void HandleDeath()
    {
        if (currentHealth <= 0)
        {
            OnDie?.Invoke();
        }
    }
}
