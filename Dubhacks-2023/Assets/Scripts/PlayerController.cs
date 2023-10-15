using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // UI manager
    public GameObject UIManager;

    // health
    public float baseHealth;
    public float baseHealthDrain;
    public float baseHealthDrainCooldown;
    private float currHealth;
    private float currHealthDrainCooldown;
    private int numCapturedGhosts;

    // combat
    public float attackDamage;

    // interact
    private static float INTERACT_RADIUS = 1.5f;
    private static LayerMask INTERACT_MASK; // Objects on Interactable layer

    // movement
    public float moveSpeed = 10.0f; // Adjust the movement speed as needed.
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
        INTERACT_MASK = LayerMask.GetMask("Interactable");
        facingDirection = new Vector2(0, -1);

        currHealth = baseHealth;
        numCapturedGhosts = 0;
    }

    private void Update()
    {
        UpdateDragIdentity();

        UpdateAttack();

        UpdateInteract();

        // take damage from health drain by carrying ghosts
        if (currHealthDrainCooldown <= 0) {
            currHealthDrainCooldown = baseHealthDrainCooldown;
            currHealth -= baseHealthDrain * numCapturedGhosts;
        }
        currHealthDrainCooldown -= Time.deltaTime;

        UIManager.GetComponent<UIManager>().UpdateGameOverlay(currHealth, numCapturedGhosts);
    }

    private void FixedUpdate()
    {
        UpdateMove();

        // Drag goes after player movement update.
        UpdateDragMovement();
    }

    ///// COMBAT RELATED STUFF
    private void UpdateAttack() {
        if (Input.GetAxis("Attack") > 0) {
            Debug.Log("Attack");
        }
    }

     public void CaptureGhost(GameObject enemy) {
        numCapturedGhosts++;
        enemy.GetComponent<GhostController>().Captured();
    }

    public void KillGhost(GameObject enemy) {
        enemy.GetComponent<GhostController>().Dead();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // take damage from ghost attacks
        if (collider.gameObject.tag == "Hazard") {
            currHealth -= collider.gameObject.GetComponent<GhostAttack>().attackDamage;
        }
    }

    ///// EXPLORATION/INTERACTION RELATED STUFF

    private void UpdateInteract() {
        if (Input.GetAxis("Interact") > 0) {
            // check what the closest facing object is
            Rigidbody2D res = GetFacingObject(INTERACT_MASK, INTERACT_RADIUS);
            if (res != null) {
                switch(res.gameObject.tag) {
                    case "Villager":
                        Debug.Log("Interact with villager");
                        if (res.gameObject.GetComponent<VillagerController>().currState != VillagerState.Suspicious) {
                            StartCoroutine(UIManager.GetComponent<UIManager>().ShowVillagerDialogue(
                                res.gameObject.GetComponent<VillagerController>().dialogue
                            ));
                        }
                        break;
                    default:
                        break;
                }
            }
            
        }
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
            Rigidbody2D res = GetFacingObject(DRAG_MASK, START_DRAG_RADUS);
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

    private Rigidbody2D GetFacingObject(LayerMask layerMask, float radius)
    {
        Vector2 origin = rb.position;
        // Cast a ray in the facing direction.
        RaycastHit2D hit = Physics2D.Raycast(origin, facingDirection, radius, layerMask);
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
