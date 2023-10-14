using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f; // Adjust the movement speed as needed.
    private Rigidbody2D rb;

    // Player variables
    private Vector2 facingDirection;

    // Dragging variables
    private static float START_DRAG_RADUS = 1.0f;
    private static float STOP_DRAG_RADIUS = 1.2f; // > start radius
    private static LayerMask DRAG_MASK; // Objects on Moveable layer
    private Rigidbody2D draggedRb; // whatever we're dragging, if any
    private Vector2 playerPrevPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        DRAG_MASK = LayerMask.GetMask("Moveable");
        facingDirection = new Vector2(0, -1);
    }

    private void Update()
    {
        UpdateDragIdentity();
    }

    private void FixedUpdate()
    {
        UpdateMove();

        // Drag goes after player movement update.
        UpdateDragMovement();
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


    ///// DRAGGING RELATED STUFF
    private void UpdateDragMovement()
    {
        if (draggedRb != null)
        {
            // Stop drag if too far away;
            if (Vector2.Distance(rb.position, draggedRb.position) > STOP_DRAG_RADIUS)
            {
                draggedRb = null;
            } else
            {
                // Move dragged rb according to player position
                draggedRb.position += (rb.position - playerPrevPos);
                playerPrevPos = rb.position;
            }
        }
    }

    private void UpdateDragIdentity()
    {
        if (Input.GetButtonDown("Drag"))
        {
            // Drag whatever object we're facing, or don't if not facing anything.
            // Also stop dragging if we're facing the object we're currently dragging.
            Rigidbody2D res = GetFacingMoveableObject();
            if (res == null || res == draggedRb)
            {
                // Stop dragging
                draggedRb = null;
            } else
            {
                // Start dragging
                draggedRb = res;
                playerPrevPos = rb.position;
            }
        }
    }

    private Rigidbody2D GetFacingMoveableObject()
    {
        Vector2 origin = rb.position;
        // Cast a ray in the facing direction.
        RaycastHit2D hit = Physics2D.Raycast(origin, facingDirection, START_DRAG_RADUS, DRAG_MASK);
        Rigidbody2D res = null;
        // Check if the ray hits something.
        if (hit.collider != null)
        {
            // Debug.Log("hit" + hit.collider.gameObject.name);
            res = hit.rigidbody;
        }
        return res;
    }
}
