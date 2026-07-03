using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float wallSlideSpeed = 2f;
    public float jumpForce = 15f;
    private float jumpCheckCooldown = 0.5f;
    private float timeLastJumped;
    private PlayerInputHandler InputHandler;
    private Rigidbody2D rb;
    public Transform groundCheckTransform;
    //public float groundCheckRadius = 0.2f;
    public Vector2 groundBoxParam = new Vector2(0.45f, 0.05f);
    public Vector2 wallBoxParam = new Vector2(0.05f, 1f);
    public LayerMask groundLayer;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isWallSliding;
    public Transform wallCheckTransform;

    private Animator animator;

    private Health health;
    public Image healthImage;
    private SpriteRenderer spriteRenderer;
    private Transform attackRange;
    private Vector3 posAttackRange = new Vector3(0.92f, 0.82f, 0f);
    private Vector3 negAttackRange = new Vector3(-0.92f, 0.82f, 0f);
    private Vector3 posWallCheck = new Vector3(0.4f, 0.65f, 0f);
    private Vector3 negWallCheck = new Vector3(-0.42f, 0.65f, 0f);
    private Damageable d;
    [SerializeField] private Transform respawnLocation;
    
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
        timeLastJumped = Time.time - 1f;

    }

    // Update is called once per frame
    void Update()
    {
        //ground check
        //isGrounded = Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayer);
        isGrounded = Physics2D.OverlapBox(groundCheckTransform.position, groundBoxParam, 0f, groundLayer);
        animator.SetBool("Grounded", isGrounded);
        //jump check
        if (isGrounded && InputHandler.jumped)
        {
            if (Time.time - timeLastJumped > jumpCheckCooldown)
            {
                timeLastJumped = Time.time;
                rb.linearVelocityY = jumpForce;
                animator.SetTrigger("Jump");
            }
        }
        /*if (isWallSliding && InputHandler.jumped && InputHandler.inputAllowed)
        {
            Debug.Log("walljump");
            InputHandler.inputAllowed = false;
            StartCoroutine(ResumeInput());
            spriteRenderer.flipX = !spriteRenderer.flipX;
            rb.linearVelocityX = 1.5f*(spriteRenderer.flipX ? -moveSpeed : moveSpeed);
            rb.linearVelocityY = jumpForce*0.7f;
            animator.SetTrigger("Jump");
        }*/
        //wall check
        if (!isGrounded)
        {
            if (Physics2D.OverlapBox(wallCheckTransform.position, wallBoxParam, 0f, groundLayer) && InputHandler.moveX != 0f)
            {
                isWallSliding = true;
                rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -wallSlideSpeed, wallSlideSpeed);
            }
            else
            {
                isWallSliding = false;
            }
        }
        else
        {
            isWallSliding = false;
        }
        animator.SetBool("WallSlide", isWallSliding);
        animator.SetFloat("AirSpeedY", rb.linearVelocityY);
        //move check
        if (InputHandler.moveX < 0)
        {
            spriteRenderer.flipX = true;
            attackRange.localPosition = negAttackRange;
            wallCheckTransform.localPosition = negWallCheck;
        }else if (InputHandler.moveX > 0)
        {
            spriteRenderer.flipX = false;
            attackRange.localPosition = posAttackRange;
            wallCheckTransform.localPosition = posWallCheck;
        }
        if (!isWallSliding && InputHandler.inputAllowed)
        {
            rb.linearVelocityX = InputHandler.moveX * moveSpeed;
        }
        animator.SetBool("IsRunning", InputHandler.isSprinting);
        //Debug.Log("0: "+move);
        //SetAnimation(move);
        healthImage.fillAmount = health.currentHealth / health.maxHealth;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
        Gizmos.DrawWireCube(groundCheckTransform.position, new Vector3(groundBoxParam.x, groundBoxParam.y, 0f));
        Gizmos.DrawWireCube(wallCheckTransform.position, new Vector3(wallBoxParam.x, wallBoxParam.y, 0f));
    }

    private IEnumerator ResumeInput()
    {
        yield return new WaitForSeconds(0.1f);
        InputHandler.inputAllowed = true;
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
        transform.position = respawnLocation.position;
        d.isInvincible = false;
        InputHandler.inputAllowed = true;
    }

    public void TeleportPlayer(Vector2 destination)
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = destination;

        spriteRenderer.flipX = false;
        isGrounded = false;
        isWallSliding = false;
    }

}
