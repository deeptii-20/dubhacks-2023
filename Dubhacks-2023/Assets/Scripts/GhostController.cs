using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Implement ghost AI
Movement: chase player or wander until attacked
1-2 attack options (ranged + melee)
Include trigger for dialogue (asking to be brought to cemetery) after first defeat
*/

public enum GhostState {
    Angry = 0,      // wandering (default)
    Aggravated = 1, // chases + attacks player
    Peaceful = 2    // asks to be put to rest

}

public class GhostController : MonoBehaviour
{
    public string[] dialogue;

    // behavior states
    public GhostState defaultState = GhostState.Angry;
    public GhostState currState;

    // health
    public float maxHealth = 5;
    private float currHealth;

    // combat
    public float baseAttack = 1;
    public float baseAttackDuration = 0.5f;
    public float baseAttackCooldown = 3;
    private float currAttackCooldown;

    // movement
    public float baseSpeed = 2.5f;
    public Vector2[] possibleDirections = {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right,
    };
    public float[] possibleMovementTimes = {0.1f, 0.5f};
    private Vector2 currDirection;
    private float currMovementTime;

    // Player gameObject (for chase navigation)
    public GameObject Player;
    public float attackRange = 1;
    public float aggroRange = 3;

    // UI Manager
    public GameObject UIManager;

    // Start is called before the first frame update
    void Start()
    {
        currState = defaultState;
        currHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // if paused, do nothing
        if (UIManager.GetComponent<UIManager>().isPaused) {
            return;
        }

        // if health >= 0, switch to peaceful and show dialogue
        if (currHealth <= 0.0f && currState != GhostState.Peaceful) {
            currState = GhostState.Peaceful;
            StartCoroutine(UIManager.GetComponent<UIManager>().ShowOneResponseDialogue(
                dialogue,
                this.gameObject,
                Player
            ));
        }

        // change behavior based on current state
        switch(currState) {
            case GhostState.Angry:
                if (playerInAggroRange()) {
                    // if player is nearby, switch to aggravated
                    currState = GhostState.Aggravated;
                } else {
                    // else continue wandering
                    Wander();
                }
                break;
            case GhostState.Aggravated:
                if (playerInAttackRange()) {
                    // if player is in attack range, attack
                    Attack();
                } else if (playerInAggroRange()) {
                    // else if player is in aggro range, chase
                    currAttackCooldown = baseAttackCooldown / 3;
                    Chase();
                } else {
                    // else switch to angry
                    currState = GhostState.Angry;
                }                
                break;
            case GhostState.Peaceful:
                // if paceful, do nothing
                break;
        }
    }

    bool playerInAttackRange() {
        return Vector2.Distance((Vector2)Player.transform.position, (Vector2)transform.position) <= attackRange;
    }

    bool playerInAggroRange() {
        return Vector2.Distance((Vector2)Player.transform.position, (Vector2)transform.position) <= aggroRange;
    }

    void Wander() {
        // if old movement has finished, pick a new direction and move
        if (currMovementTime <= 0) {
            currDirection = possibleDirections[Random.Range(0, possibleDirections.Length)];
            currMovementTime = Random.Range(possibleMovementTimes[0], possibleMovementTimes[1]);
        }
        transform.Translate(currDirection * Time.deltaTime * baseSpeed);
        currMovementTime -= Time.deltaTime;
    }

    void Chase() {
        // chase the player
        float currDistToPlayer = Vector2.Distance(transform.position, Player.transform.position);
        Vector2 optimalDirection = new Vector2(0.0f, 0.0f);
        foreach (Vector2 direction in possibleDirections) {
            float newDistToPlayer = Vector2.Distance((Vector2)transform.position + direction, Player.transform.position);
            if (newDistToPlayer <= currDistToPlayer) {
                currDistToPlayer = newDistToPlayer;
                optimalDirection = direction;
            }
        }
        currDirection = optimalDirection;
        transform.Translate(currDirection * Time.deltaTime * baseSpeed);
    }

    void Attack() {
        if (currState != GhostState.Aggravated) {
            // only attack if aggravated
            return;
        }
        if (currAttackCooldown <= 0) {
            // start attack
            GameObject attackObj = transform.Find("Attack").gameObject;
            attackObj.GetComponent<GhostAttack>().SetAttackDuration(baseAttackDuration);
            attackObj.SetActive(true);

            // reset attack cooldown
            currAttackCooldown = baseAttackCooldown;
        } else {
            // wait for attack cooldown to end
            currAttackCooldown -= Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // take damage from player attacks
        if (collider.gameObject.tag == "Projectile") {
            currHealth -= collider.gameObject.GetComponent<ProjectileController>().attackDamage;
            collider.gameObject.GetComponent<ProjectileController>().Explode();
        }
        if (collider.gameObject.tag == "Slash") {
            currHealth -= collider.gameObject.GetComponent<SlashController>().attackDamage;
        }
    }

}
