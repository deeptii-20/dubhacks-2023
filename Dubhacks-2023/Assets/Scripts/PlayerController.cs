using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f; // Adjust the movement speed as needed.
    private Rigidbody2D rb;


    // State variables
    private bool isDragging;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        isDragging = false;
    }

    private void Update()
    {
        UpdateDrag();

    }

    private void FixedUpdate()
    {
        UpdateMove();
    }

    private void UpdateMove()
    {
        Vector2 moveInput;

        // Get input from the player (arrow keys or WASD)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Normalize the input vector to ensure equal movement speed in all directions
        moveInput.Normalize();

        // Apply velocity to the Rigidbody to move the player
        rb.velocity = moveInput * moveSpeed;
    }

    private void UpdateDrag()
    {
        if (Input.GetButtonDown("Drag"))
        {
            // TODO: Check if we're in front of something
            Debug.Log("Drag");
            isDragging = !isDragging;
        }
    }
}