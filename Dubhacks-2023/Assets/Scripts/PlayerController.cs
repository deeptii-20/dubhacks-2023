using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f; // Adjust the movement speed as needed.
    private Rigidbody2D rb;
    private Transform tr;

    private Vector2 facingDirection;

    // Dragging variables
    private static float DRAG_RADIUS = 1.0f;
    private static LayerMask DRAG_MASK;
    private bool isDragging;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();

        DRAG_MASK = LayerMask.GetMask("Moveable");
        isDragging = false;
        facingDirection = new Vector2(0, -1);
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

        if (moveInput.magnitude > 0)
        {
            facingDirection = moveInput;
        }

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
            Debug.Log(facingDirection);
            isDragging = !isDragging;
            IsFacingMoveableObject();
        }
    }

    private void IsFacingMoveableObject()
    {
        Vector2 origin = tr.position;
        // Cast a ray in the facing direction.
        RaycastHit2D hit = Physics2D.Raycast(origin, facingDirection, DRAG_RADIUS, DRAG_MASK);
        // Check if the ray hits something.
        if (hit.collider != null)
        {
            Debug.Log("Ray hit: " + hit.collider.gameObject.name);

            // You can perform actions or interactions with the object that was hit.
        }
    }
}
