using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rBody;

    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float jumpForce = 16f;

    private float movementInputDirection;
    private bool isFacingRight = true;

    // Start is called before the first frame update
    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckInput();
        CheckMovementDirection();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
            Flip();
        else if (!isFacingRight && movementInputDirection > 0)
            Flip();
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

        rBody.velocity = new Vector2(newPositionX, newPositionY);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        transform.Rotate(0f, 180f, 0f);
    }

    private void Jump()
    {
        rBody.velocity = new Vector2(rBody.velocity.x, jumpForce);
    }
}
