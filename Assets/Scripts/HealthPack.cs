using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public float healValue = 2f;
    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Health h = collision.gameObject.GetComponent<Health>();
            h.Heal(healValue);
        }
    }
}
