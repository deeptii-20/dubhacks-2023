using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // UI manager + old man
    public GameObject UIManager;
    public GameObject OldMan;

    // health
    public float baseHealth = 50;
    public float baseHealthDrain = 1;
    public float baseHealthDrainCooldown = 5;
    public float baseHealthReplenish = 25;
    public float currHealth;
    private float currHealthDrainCooldown;
    public int numCapturedGhosts;

    // melee combat
    public GameObject slash;
    public float meleeAttackDamage = 1;
    public float baseMeleeAttackCooldown = 0.5f;
    public float meleeAttackHealthDrain = 3;
    private float currMeleeAttackCooldown;

    // ranged combat
    public GameObject projectile;
    public float rangedAttackDamage = 3;
    
    public float rangedAttackHealthDrain = 1;
    
    public float baseRangedAttackCooldown = 1;
    private float currRangedAttackCooldown;

    // interact
    private static float INTERACT_RADIUS = 1.5f;
    private static LayerMask INTERACT_MASK; // Objects on Interactable layer

    // movement
    public float moveSpeed = 10.0f; // Adjust the movement speed as needed.
    private Rigidbody2D rb;

    // Player variables
    public Vector2 facingDirection;

    // Dragging variables
    private static float START_DRAG_RADUS = 1.0f;
    private static float STOP_DRAG_RADIUS = 1.2f; // > start radius
    private static LayerMask DRAG_MASK; // Objects on Moveable layer
    private Rigidbody2D draggedRb; // whatever we're dragging, if any
    private Vector2 playerPrevPos;

    // instantiated
    public GameObject peacefulGhost;
    public GameObject ghost;
    public GameObject corpse;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        facingDirection = new Vector2(0, -1);
        DRAG_MASK = LayerMask.GetMask("Moveable");
        INTERACT_MASK = LayerMask.GetMask("Interactable");

        currHealth = baseHealth;
        numCapturedGhosts = 0;
    }

    private void Update()
    {
        // if paused, do nothing
        if (UIManager.GetComponent<UIManager>().isPaused) {
            return;
        }

        UpdateDragIdentity();

        UpdateAttack();
        currMeleeAttackCooldown -= Time.deltaTime;
        currRangedAttackCooldown -= Time.deltaTime;

        UpdateInteract();

        // take damage from health drain by carrying ghosts
        if (currHealthDrainCooldown <= 0) {
            currHealthDrainCooldown = baseHealthDrainCooldown;
            TakeDamage(baseHealthDrain * numCapturedGhosts, true);
        }
        currHealthDrainCooldown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // if paused, do nothing
        if (UIManager.GetComponent<UIManager>().isPaused) {
            rb.velocity = Vector2.zero;
            return;
        }

        UpdateMove();

        // Drag goes after player movement update.
        UpdateDragMovement();
    }

    ///// COMBAT RELATED STUFF
    private void UpdateAttack() {
        if (Input.GetButtonDown("Melee Attack") && currMeleeAttackCooldown <= 0 && currHealth >= meleeAttackHealthDrain) {
            currMeleeAttackCooldown = baseMeleeAttackCooldown;
            Quaternion rotate = Quaternion.Euler(0, 0, 0);
            bool isBehindPlayer = false;
            switch(facingDirection) {
                case Vector2 v when v.Equals(Vector2.up):
                    rotate = Quaternion.Euler(0, 0, 0);
                    isBehindPlayer = true;
                    break;
                case Vector2 v when v.Equals(new Vector2(-1, 1)):
                    rotate = Quaternion.Euler(0, 0, 45);
                    isBehindPlayer = true;
                    break;
                case Vector2 v when v.Equals(Vector2.left):
                    rotate = Quaternion.Euler(0, 0, 90);
                    break;
                case Vector2 v when v.Equals(new Vector2(-1, -1)):
                    rotate = Quaternion.Euler(0, 0, 135);
                    break;
                case Vector2 v when v.Equals(Vector2.down):
                    rotate = Quaternion.Euler(0, 0, 180);
                    break;
                case Vector2 v when v.Equals(new Vector2(1, -1)):
                    rotate = Quaternion.Euler(0, 0, 225);
                    break;
                case Vector2 v when v.Equals(Vector2.right):
                    rotate = Quaternion.Euler(180, 0, 270);
                    break;
                case Vector2 v when v.Equals(new Vector2(1, 1)):
                    rotate = Quaternion.Euler(0, 0, 315);
                    isBehindPlayer = true;
                    break;
            }
            GameObject s = Instantiate(slash, (Vector2)transform.position + facingDirection * 0.5f, rotate, transform);
            s.GetComponent<SlashController>().SetParams(meleeAttackDamage, baseMeleeAttackCooldown / 2);
            s.GetComponent<SpriteRenderer>().sortingOrder = isBehindPlayer ? -1 : 1;
            TakeDamage(meleeAttackHealthDrain, true);
            StartCoroutine(UpdateAttackAnimation());
        } else if (Input.GetButtonDown("Ranged Attack") && currRangedAttackCooldown <= 0 && currHealth >= rangedAttackHealthDrain) {
            currRangedAttackCooldown = baseRangedAttackCooldown;
            GameObject p = Instantiate(projectile, transform.position, transform.rotation);
            p.GetComponent<ProjectileController>().SetParams((Vector2)transform.position, facingDirection, rangedAttackDamage);
            TakeDamage(rangedAttackHealthDrain, true);
            StartCoroutine(UpdateAttackAnimation());
        }
    }

    public void TakeDamage(float drainAmount, bool isDrain) {
        currHealth = currHealth < drainAmount ? 0 : currHealth - drainAmount;
        UIManager.GetComponent<UIManager>().UpdateHealthOverlay(currHealth, baseHealth, isDrain);
    }

    public void KillVillager(GameObject villager) {
        // update UI
        currHealth = (currHealth + baseHealthReplenish > baseHealth) ? baseHealth :  currHealth + baseHealthReplenish;
        UIManager.GetComponent<UIManager>().UpdateHealthOverlay(currHealth, baseHealth, false);
        // destroy villager body
        Destroy(villager);

        // spawn corpse
        Instantiate(corpse, villager.transform.position, villager.transform.rotation);

        // spawn ghost
        GameObject g = Instantiate(ghost, (Vector2)villager.transform.position + (Vector2.up * 0.5f), villager.transform.rotation);
        g.GetComponent<GhostController>().UIManager = this.UIManager;
        g.GetComponent<GhostController>().Player = this.gameObject;
        g.GetComponent<GhostController>().dialogue = new string[] {
            "To think you betrayed our kindness for your own gain...",
            "If you feel any remorse for your actions, bring me to the cemetery so that I may rest."
        };
        OldMan.GetComponent<OldManController>().totalNumGhosts += 1;
    }

    public void CaptureGhost(GameObject enemy) {
        numCapturedGhosts++;
        // destroy ghost and add peaceful ghost to trail
        Destroy(enemy);
        GameObject pg = Instantiate(peacefulGhost, transform.position, transform.rotation);
        pg.GetComponent<PeacefulGhostController>().player = this.gameObject;
        pg.GetComponent<PeacefulGhostController>().distFromPlayer = numCapturedGhosts;
        pg.GetComponent<PeacefulGhostController>().baseSpeed = moveSpeed;
        pg.tag = "Ghost Trail";
    }

    public void ReleaseGhosts() {
        OldMan.GetComponent<OldManController>().numCapturedGhosts += numCapturedGhosts;
        foreach (GameObject pg in GameObject.FindGameObjectsWithTag("Ghost Trail")) {
            pg.tag = "Untagged";
            OldMan.GetComponent<OldManController>().PlacePeacefulGhost(pg);
        }
        // update dialogue
        StartCoroutine(UIManager.GetComponent<UIManager>().ShowOneResponseDialogue(
            OldMan.GetComponent<OldManController>().GetMonumentDialogue(numCapturedGhosts > 0),
            OldMan,
            this.gameObject
        ));
        numCapturedGhosts = 0;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // take damage from ghost attacks
        if (collider.gameObject.tag == "Hazard") {
            StartCoroutine(UpdateTakeDamageAnimation());
            TakeDamage(collider.gameObject.GetComponent<GhostAttack>().attackDamage, false);
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
                        StartCoroutine(UIManager.GetComponent<UIManager>().ShowOneResponseDialogue(
                            res.gameObject.GetComponent<VillagerController>().GetDialogue(),
                            res.gameObject,
                            this.gameObject
                        ));
                        break;
                    case "Monument":
                        ReleaseGhosts();
                        break;
                    case "OldMan":
                        StartCoroutine(UIManager.GetComponent<UIManager>().ShowOneResponseDialogue(
                            res.gameObject.GetComponent<OldManController>().GetTalkingDialogue(numCapturedGhosts > 0),
                            res.gameObject,
                            this.gameObject
                        ));
                        break;
                    case "Enemy":
                        if (res.gameObject.GetComponent<GhostController>().currState == GhostState.Peaceful) {
                            StartCoroutine(UIManager.GetComponent<UIManager>().ShowOneResponseDialogue(
                                res.gameObject.GetComponent<GhostController>().dialogue,
                                res.gameObject,
                                this.gameObject
                            ));
                        }
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
        UpdateMoveAnimation();
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

    private void UpdateMoveAnimation() {
        switch(facingDirection) {
            case Vector2 v when v.Equals(Vector3.down):
                GetComponent<Animator>().SetInteger("MoveDirection", 0);
                break;
            case Vector2 v when v.Equals(Vector3.up):
                GetComponent<Animator>().SetInteger("MoveDirection", 1);
                break;
            case Vector2 v when v.Equals(Vector3.left):
                GetComponent<Animator>().SetInteger("MoveDirection", 2);
                break;
            case Vector2 v when v.Equals(Vector3.right):
                GetComponent<Animator>().SetInteger("MoveDirection", 3);
                break; 
        }
    }

    private IEnumerator UpdateAttackAnimation() {
        GetComponent<Animator>().SetBool("Attack", true);
        yield return new WaitForSeconds(0.5f);
        GetComponent<Animator>().SetBool("Attack", false);
    }

    private IEnumerator UpdateTakeDamageAnimation() {
        GetComponent<Animator>().SetBool("TakeDamage", true);
        yield return new WaitForSeconds(0.1f);
        GetComponent<Animator>().SetBool("TakeDamage", false);
    }

}
