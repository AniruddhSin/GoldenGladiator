using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage = 1f;
    [SerializeField] private float moveSpeed = 1f;
    private bool launched = false;
    [SerializeField] private float yResetLevel;


    void OnEnable()
    {
        launched = false;
    }

    void Update()
    {
        if (launched)
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;
            if (transform.position.y < yResetLevel)
            {
                launched = false;
                GameManager.Instance.MeteorPool.ReturnObject(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Damageable d = collision.gameObject.GetComponent<Damageable>();
            d.InflictDamage(damage, this.gameObject);
            GameManager.Instance.MeteorPool.ReturnObject(this.gameObject);
        }
    }
    public void Launch()
    {
        launched = true;
    }
}
