using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // REFACTOR TO STATE MACHINE LATER WHEN ADDING ATTACKS
    public float moveSpeed = 3f;
    public float jumpForce = 1f;
    private float move = 0f;
    private bool isRunning = false;
    public InputSystem_Actions actions;
    private Rigidbody2D rb;
    public Transform groundCheckTransform;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Animator animator;
    private AnimatorStateInfo animState;
    
    void Awake()
    {
        actions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        actions.Player.Enable();
        actions.Player.Move.performed += Movement;
        actions.Player.Jump.performed += Jumping;

        actions.Player.Move.canceled += Movement;
        actions.Player.Jump.canceled += Jumping;
    }
    void OnDisable()
    {
        actions.Player.Disable();
        actions.Player.Move.performed -= Movement;
        actions.Player.Jump.performed -= Jumping;

        actions.Player.Move.canceled -= Movement;
        actions.Player.Jump.canceled -= Jumping;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayer);
        animator.SetBool("Grounded", isGrounded);
        animator.SetFloat("AirSpeedY", rb.linearVelocityY);
        rb.linearVelocityX = move * moveSpeed;
        //Debug.Log("0: "+move);
        //SetAnimation(move);
    }

    void Movement (InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isRunning = true;
        }else if (ctx.canceled)
        {
            isRunning = false;
        }
        animator.SetBool("IsRunning", isRunning);
        move = ctx.ReadValue<Vector2>().x;
        //Debug.Log($"{ctx.phase}  {ctx.ReadValue<Vector2>()}");
    }
    void Jumping (InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (isGrounded)
            {
                rb.linearVelocityY = jumpForce;
                animator.SetTrigger("Jump");
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
    }

    // animations
    /*
    private void SetAnimation(float moveValue)
    {
        animState = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log("1 "+animState.IsName("Run"));
        Debug.Log("2 "+animState.IsName("Idle"));
        if (isGrounded)
        {
            if(moveValue == 0)
            {
                animator.Play("Idle");
            }
            else
            {
                if (!animState.IsName("Run"))
                {
                    //Debug.Log("true");
                    animator.Play("Run");
                }
            }
        }
        else
        {
            if (rb.linearVelocityY > 0)
            {
                animator.Play("Jump");
            }
            else
            {
                animator.Play("Fall");   
            }
        }
    }
    */
}
