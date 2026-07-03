using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private enum State
    {
        Patrol,
        Chase,
        Idle
    }
    [SerializeField] private State currentState;
    [SerializeField] private float patrolSpeed = 1f;
    [SerializeField] private Transform[] points;
    private float waypointThreshold = 0.1f;
    private int currentPoint = 0;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask playerLayer;
    private float detectionDistance = 0f;
    private Transform player;
    private Vector2 direction = Vector2.left;
    private Vector3 posAttackRange = new Vector3(-0.33f, 0.64f, 0f);
    private Vector3 negAttackRange = new Vector3(0.72f, 0.64f, 0f);
    [SerializeField] private Transform attackRange;
    [SerializeField] private BoxCollider2D attackCollider;
    private bool isAttacking = false;
    private float attackDistance = 0.8f;
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); 
    [SerializeField] private float attackDamage = 2f;
    private Health health;

    void Awake()
    {
        if (points == null || points.Length < 2)
        {
            Debug.LogError(transform.name + ": Did not find at least 2 waypoints to patrol between! \n Attempting manually");
            points = new Transform[2];
            points[0] = transform.parent.Find("LeftPoint");
            points[1] = transform.parent.Find("RightPoint");
        }
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
        if (attackRange == null)
        {
            attackRange = transform.Find("AttackRange");
        }
        if (!attackCollider)
        {
            attackCollider = attackRange.GetComponent<BoxCollider2D>();
        }

        health.OnDamaged += OnDamage;
        health.OnDie += OnDie;
    }

    void Start()
    {
        currentState = State.Patrol;
        attackCollider.enabled = false;
    }


    void Update()
    {
        DetectPlayer();

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase();
                break;
        }
    }
    public void setPatrol()
    {
        currentState = State.Patrol;
    }

    void Patrol()
    {
        animator.SetInteger("AnimState", 2);
        Transform target = points[currentPoint];

        MoveTowards(target.position.x);

        if (Mathf.Abs(transform.position.x - target.position.x) < waypointThreshold)
        {
            currentPoint = currentPoint == 0 ? 1 : 0;
        }
    }
    void Chase()
    {
        //Debug.Log(transform.name + ": Chasing");
        if (player == null)
        {
            currentState = State.Patrol;
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= attackDistance)
        {
            if (!isAttacking)
            {
                animator.SetInteger("AnimState", 0);
                animator.SetTrigger("Attack");    
                isAttacking = true; 
            }
            return;
        }
        else
        {
            isAttacking = false;
            animator.SetInteger("AnimState", 2);
        }

        MoveTowards(player.position.x);
    }
    private void DetectPlayer()
    {
        Vector2 start = new Vector2(
            transform.position.x,
            points[0].position.y
        );

        direction = spriteRenderer.flipX ? Vector2.right : Vector2.left;

        detectionDistance = Mathf.Abs(points[currentPoint].position.x - transform.position.x);


        RaycastHit2D hit = Physics2D.Raycast(
            start,
            direction,
            detectionDistance,
            playerLayer
        );


        if (hit.collider != null)
        {
            player = hit.transform;

            if (currentState == State.Patrol)
                currentState = State.Chase;
        }
        else if (currentState == State.Chase)
        {
            if (!(player.position.x > points[0].position.x && player.position.x < points[1].position.x))
            {
                player = null;
                currentState = State.Patrol;
            }
        }
    }

    void MoveTowards(float targetX)
    {
        float moveDirection = Mathf.Sign(targetX - transform.position.x);

        rb.linearVelocity = new Vector2(
            moveDirection * patrolSpeed,
            rb.linearVelocity.y
        );

        if (moveDirection != 0)
        {
            spriteRenderer.flipX = moveDirection > 0;
        }
        if (spriteRenderer.flipX)
        {
            attackRange.localPosition = negAttackRange;
        }
        else
        {
            attackRange.localPosition = posAttackRange;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        TryHit(collision);
    }

    public void EnableHitbox()
    {
        //Debug.Log("hitbox enabled");
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
        //Debug.Log("hitbox disabled");
        attackCollider.enabled = false;
        isAttacking = false;
    }

    private void TryHit(Collider2D other)
    {
        // if we already hit this game object or if we haven't but the hit object is not damageable
        if (hitObjects.Contains(other.gameObject) || !other.gameObject.TryGetComponent<Damageable>(out Damageable damageable))
            return;
        //damgeable should always exist if we passed the if statement
        if (other.gameObject.tag == "Player")
        {
            hitObjects.Add(other.gameObject);
            damageable.InflictDamage(attackDamage, this.gameObject);
        }
    }

    void OnDamage(float damage, GameObject source)
    {
        StartCoroutine(BlinkRed());
        animator.SetTrigger("Hurt");
    }
    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = Color.white;
    }

    void OnDie()
    {
        animator.SetTrigger("Death");
        patrolSpeed = 0f;
        currentState = State.Idle;
    }
    public void DieFinished()
    {
        GameManager.Instance.EnemyDied(this);
    }
    public void respawn()
    {
        patrolSpeed = 1f;
        transform.localPosition = Vector3.zero;
        currentState = State.Patrol;
        //Debug.Log(currentState);
        health.ResetHealth();
    }

    void OnDrawGizmos()
    {
        if (points == null || points.Length < 1)
            return;


        Vector3 start = new Vector3(
            transform.position.x,
            points[0].position.y,
            0
        );
        Vector3 gizmoDirection = new Vector3(direction.x, direction.y, 0);

        Gizmos.DrawLine(
            start,
            start + gizmoDirection * detectionDistance
        );
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            start,
            start + gizmoDirection * attackDistance
        );
    }
}
