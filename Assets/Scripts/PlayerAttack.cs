using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    private PlayerInputHandler InputHandler;
    private Animator animator;
    [SerializeField] private BoxCollider2D attackCollider;
    // keep track of already hit objects per swing so they dont take damage multiple times
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); 
    [SerializeField] private float attackDamage = 3f;
    private float attackCooldown = 0.55f;
    private float lastAttackTime = 0f;
    void Awake()
    {
        InputHandler = GetComponent<PlayerInputHandler>();
        animator = GetComponent<Animator>();
        if (!attackCollider)
            attackCollider = transform.Find("AttackRange").GetComponent<BoxCollider2D>();

        InputHandler.OnAttack += OnAttack;

    }
    void Start()
    {
        attackCollider.enabled = false;
    }
    void OnAttack()
    {
        if (Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack1");
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        TryHit(collision);
    }

    public void EnableHitbox()
    {
        Debug.Log("hitbox enabled");
        hitObjects.Clear();
        attackCollider.enabled = true;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            attackCollider.bounds.center,
            attackCollider.bounds.size,
            0f
        );
        foreach (Collider2D hit in hits)
        {
            TryHit(hit);
        }
    }

    public void DisableHitbox()
    {
        Debug.Log("hitbox disabled");
        attackCollider.enabled = false;
    }

    private void TryHit(Collider2D other)
    {
        // if we already hit this game object or if we haven't but the hit object is not damageable
        if (hitObjects.Contains(other.gameObject) || !other.gameObject.TryGetComponent<Damageable>(out Damageable damageable))
            return;
        //damgeable should always exist if we passed the if statement
        if (other.gameObject.tag != "Player")
        {
            hitObjects.Add(other.gameObject);
            damageable.InflictDamage(attackDamage, this.gameObject);
        }
    }
}
