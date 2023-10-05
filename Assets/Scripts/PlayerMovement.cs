using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rBody;
    private Animator animator;

    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float wallSlideSpeed = 5f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float groundCheckRadius = 1f;

    [SerializeField] private float wallCheckDistance = 1f;
    [SerializeField] private int jumpAmount = 1;

    [SerializeField] private LayerMask jumpleGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;

    private int jumpAmountLeft;
    private float movementInputDirection;

    private bool canJump = false;
    private bool isWallSliding = false;

    private bool isFacingRight = true;
    private bool isGrounded = true;
    private bool isTouchingWall = false;

    private enum MovementState { idle, walking, jumping, wallSlide };
    private MovementState movementState = MovementState.idle;

    // Start is called before the first frame update
    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        jumpAmountLeft = jumpAmount;
    }

    // Update is called once per frame
    private void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckJump();
        CheckWallSliding();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurrounding();
    }

    private void CheckSurrounding()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, jumpleGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, jumpleGround);
    }

    private void CheckJump()
    {
        if (isGrounded && rBody.velocity.y <= 0)
            jumpAmountLeft = jumpAmount;

        if (jumpAmountLeft <= 0)
            canJump = false;
        else canJump = true;
    }

    private void CheckWallSliding()
    {
        if (isTouchingWall && !isGrounded && rBody.velocity.y < 0) isWallSliding = true;
        else isWallSliding = false;
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
            Flip();
        else if (!isFacingRight && movementInputDirection > 0)
            Flip();

        float currentPositionX = rBody.velocity.x;
        if (currentPositionX != 0) movementState = MovementState.walking;
        else movementState = MovementState.idle;

    }

    private void UpdateAnimations()
    {
        Debug.Log("is WallSliding" + isWallSliding + isGrounded);
        if (isWallSliding)
            movementState = MovementState.wallSlide;
        else if (!isGrounded)
            movementState = MovementState.jumping;

        int currentState = (int)movementState;

        animator.SetInteger("state", currentState);
        animator.SetFloat("yVelocity", rBody.velocity.y);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
            Jump();
    }

    private void ApplyMovement()
    {
        float newPositionX = movementSpeed * movementInputDirection;
        float newPositionY = rBody.velocity.y;

        if (isGrounded)
            rBody.velocity = new Vector2(newPositionX, newPositionY);

        if (isWallSliding)
            if (newPositionY < -wallSlideSpeed)
            {
                newPositionX = rBody.velocity.x;
                newPositionY = -wallSlideSpeed;

                rBody.velocity = new Vector2(newPositionX, newPositionY);
            }

        
    }

    private void Flip()
    {
        if (!isWallSliding)
        {
            isFacingRight = !isFacingRight;

            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            rBody.velocity = new Vector2(rBody.velocity.x, jumpForce);
            jumpAmountLeft--;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Vector3 positionVector = new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z);
        Gizmos.DrawLine(wallCheck.position, positionVector);
    }
}
