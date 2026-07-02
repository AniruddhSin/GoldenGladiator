using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float jumpForce = 1f;
    private PlayerInputHandler InputHandler;
    private Rigidbody2D rb;
    public Transform groundCheckTransform;
    //public float groundCheckRadius = 0.2f;
    public Vector2 boxParam = new Vector2(0.7f, 0.05f);
    public LayerMask groundLayer;
    private bool isGrounded;

    private Animator animator;

    private Health health;
    public Image healthImage;
    private SpriteRenderer spriteRenderer;
    private Transform attackRange;
    private Vector3 posAttackRange = new Vector3(0.92f, 0.82f, 0f);
    private Vector3 negAttackRange = new Vector3(-0.92f, 0.82f, 0f);
    private Damageable d;
    
    void Awake()
    {
        InputHandler = GetComponent<PlayerInputHandler>();
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        d = GetComponent<Damageable>();

        if (health != null)
        {
            health.OnDie += OnDie;
            health.OnHealed += OnHeal;
            health.OnDamaged += OnDamage;
        }
    }

    void Start()
    {
        attackRange = transform.Find("AttackRange");
    }

    // Update is called once per frame
    void Update()
    {
        //ground check
        //isGrounded = Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayer);
        isGrounded = Physics2D.OverlapBox(groundCheckTransform.position, boxParam, 0f, groundLayer);
        animator.SetBool("Grounded", isGrounded);
        //jump check
        if (isGrounded && InputHandler.jumped)
        {
            rb.linearVelocityY = jumpForce;
            animator.SetTrigger("Jump");
        }
        animator.SetFloat("AirSpeedY", rb.linearVelocityY);
        //move check
        if (InputHandler.moveX < 0)
        {
            spriteRenderer.flipX = true;
            attackRange.localPosition = negAttackRange;
        }else if (InputHandler.moveX > 0)
        {
            spriteRenderer.flipX = false;
            attackRange.localPosition = posAttackRange;
        }
        rb.linearVelocityX = InputHandler.moveX * moveSpeed;
        animator.SetBool("IsRunning", InputHandler.isSprinting);
        //Debug.Log("0: "+move);
        //SetAnimation(move);
        healthImage.fillAmount = health.currentHealth / health.maxHealth;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
        Gizmos.DrawWireCube(groundCheckTransform.position, new Vector3(0.7f, 0.05f, 0f));
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

    void OnHeal(float health)
    {
        StartCoroutine(BlinkGreen());
    }
    private IEnumerator BlinkGreen()
    {
        spriteRenderer.color = Color.green;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = Color.white;
    }

    void OnDie()
    {
        InputHandler.inputAllowed = false;
        rb.linearVelocity = Vector2.zero;
        d.isInvincible = true;
        animator.SetTrigger("Death");
    }
    void respawn()
    {
        animator.Play("Idle");
        health.ResetHealth();
        transform.position = Vector3.zero;
        d.isInvincible = false;
        InputHandler.inputAllowed = true;
    }

}
