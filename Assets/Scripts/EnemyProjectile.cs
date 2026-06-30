using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage = 1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Damageable d = collision.gameObject.GetComponent<Damageable>();
            d.InflictDamage(damage, this.gameObject);
        }
    }
}
