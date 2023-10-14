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
    // base ghost state
    public GhostState defaultState = GhostState.Angry;
    public float maxHealth;
    public float baseSpeed;
    public float baseAttack;
    public Vector2[] possibleDirections;

    // curr ghost state
    private GhostState currState;
    private float currHealth;
    private Vector2 currDirection;

    // UI objects
    public GameObject InteractionPrompt;
    public GameObject Dialogue;

    // Player gameObject (for chase navigation)
    public GameObject Player;


    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // if health >= 0, switch to peaceful
        if (currHealth <= 0.0f) {
            currState = GhostState.Peaceful;
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
                    Chase();
                } else {
                    // else switch to angry
                    currState = GhostState.Angry;
                }                
                break;
            case GhostState.Peaceful:
                if (playerInInteractRange()) {
                    // if player is nearby, show interaction prompt
                    if (Input.GetAxis("Interact") >= 0) {
                        // if player chooses to interact, show dialogue box
                        Debug.Log("interact with enemy");
                    }
                }
                break;
        }
    }

    bool playerInAttackRange() {
        return false;
    }

    bool playerInAggroRange() {
        return false;
    }

    bool playerInInteractRange() {
        return false;
    }

    void Wander() {
        // pick a random direction and move
        currDirection = Random.Range(0.0f, 1.0f) > 0.7f ? currDirection : possibleDirections[Random.Range(0, possibleDirections.Length)];
        transform.Translate(currDirection * Time.deltaTime * baseSpeed);
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
    }

    void Interact() {
        if (currState != GhostState.Peaceful) {
            // only interact if peaceful
            return;
        }

    }
}
